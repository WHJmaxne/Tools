﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tool.Sms.Aliyun
{
    public class SmsService : ISmsService
    {
        private readonly SmsOptions _option;
        public SmsService(IOptions<SmsOptions> option)
        {
            _option = option.Value;
        }
        public async Task<(bool success, string response)> SendAsync(SmsObject sms)
        {
            var paramers = new Dictionary<string, string>();
            paramers.Add("PhoneNumbers", sms.Mobile);
            paramers.Add("SignName", sms.Signature);
            paramers.Add("TemplateCode", sms.TempletKey);
            paramers.Add("TemplateParam", JsonConvert.SerializeObject(sms.Data));
            paramers.Add("OutId", sms.OutId);
            paramers.Add("AccessKeyId", _option.AccessKeyId);

            try
            {
                string url = GetSignUrl(paramers, _option.AccessKeySecret);

                int retryTimes = 1;
                var reply = await HttpGetAsync(url);
                while (500 <= reply.StatusCode && _option.AutoRetry && retryTimes < _option.MaxRetryNumber)
                {
                    url = GetSignUrl(paramers, _option.AccessKeySecret);
                    reply = await HttpGetAsync(url);
                    retryTimes++;
                }

                if (!string.IsNullOrEmpty(reply.response))
                {
                    var res = JsonConvert.DeserializeObject<Dictionary<string, string>>(reply.response);
                    if (res != null && res.ContainsKey("Code") && "OK".Equals(res["Code"]))
                    {
                        return (true, response: reply.response);
                    }
                }

                return (false, response: reply.response);
            }
            catch (Exception ex)
            {
                return (false, response: ex.Message);
            }
        }


        private string GetSignUrl(Dictionary<string, string> parameters, string accessSecret)
        {
            var imutableMap = new Dictionary<string, string>(parameters);
            imutableMap.Add("Timestamp", FormatIso8601Date(DateTime.Now));
            imutableMap.Add("SignatureMethod", "HMAC-SHA1");
            imutableMap.Add("SignatureVersion", "1.0");
            imutableMap.Add("SignatureNonce", Guid.NewGuid().ToString());
            imutableMap.Add("Action", _option.Action);
            imutableMap.Add("Version", _option.Version);
            imutableMap.Add("Format", _option.Format);
            imutableMap.Add("RegionId", _option.RegionId);

            IDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(imutableMap, StringComparer.Ordinal);
            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach (var p in sortedDictionary)
            {
                canonicalizedQueryString.Append("&")
                .Append(PercentEncode(p.Key)).Append("=")
                .Append(PercentEncode(p.Value));
            }

            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append("GET");
            stringToSign.Append(_option.SEPARATOR);
            stringToSign.Append(PercentEncode("/"));
            stringToSign.Append(_option.SEPARATOR);
            stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

            string signature = SignString(stringToSign.ToString(), accessSecret + "&");

            imutableMap.Add("Signature", signature);

            return ComposeUrl(_option.Domain, imutableMap);
        }

        private static string FormatIso8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
        }

        /// <summary>
        /// 签名
        /// </summary>
        public static string SignString(string source, string accessSecret)
        {
            using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        private static string ComposeUrl(string endpoint, Dictionary<String, String> parameters)
        {
            StringBuilder urlBuilder = new StringBuilder("");
            urlBuilder.Append("http://").Append(endpoint);
            if (-1 == urlBuilder.ToString().IndexOf("?"))
            {
                urlBuilder.Append("/?");
            }
            string query = ConcatQueryString(parameters);
            return urlBuilder.Append(query).ToString();
        }

        private static string ConcatQueryString(Dictionary<string, string> parameters)
        {
            if (null == parameters)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();

            foreach (var entry in parameters)
            {
                String key = entry.Key;
                String val = entry.Value;

                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null)
                {
                    sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
                }
                sb.Append("&");
            }

            int strIndex = sb.Length;
            if (parameters.Count > 0)
                sb.Remove(strIndex - 1, 1);

            return sb.ToString();
        }

        private static string PercentEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        private async Task<(int StatusCode, string response)> HttpGetAsync(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.AutomaticDecompression = DecompressionMethods.GZip;

            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * _option.TimeoutInMilliSeconds);
                HttpResponseMessage response = await http.GetAsync(url);
                return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<(bool success, string response)> SendVerifyCodeAsync(string Phone)
        {
            if (this._option.VerifyCodeOptions == null)
            {
                throw new ArgumentNullException("VerifyCodeOptions is Invalid");
            }
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();
            var res = await this.SendVerifyCodeAsync(Phone, code, this._option.VerifyCodeOptions.Signature, this._option.VerifyCodeOptions.TempleteKey, this._option.VerifyCodeOptions.OutId);
            return res;
        }

        public async Task<(bool success, string response)> SendVerifyCodeAsync(string Phone, string Code, string Signature, string TempleteKey, string OutId = "")
        {
            if (string.IsNullOrWhiteSpace(Signature) || string.IsNullOrWhiteSpace(TempleteKey))
            {
                throw new ArgumentNullException("Signature Or TempleteKey is Invalid");
            }
            if (string.IsNullOrWhiteSpace(Phone))
            {
                throw new ArgumentNullException("Phone is Invalid");
            }
            if (string.IsNullOrWhiteSpace(Code))
            {
                throw new ArgumentNullException("Code is Invalid");
            }

            IDictionary<string, string> data = new Dictionary<string, string>();
            data.Add("code", Code);
            SmsObject sms = new SmsObject
            {
                Data = data,
                Mobile = Phone,
                Signature = Signature,
                TempletKey = TempleteKey,
                OutId = OutId
            };
            var res = await this.SendAsync(sms);
            if (res.success)
            {
                res.response = Code;
            }
            return res;
        }
    }
}
