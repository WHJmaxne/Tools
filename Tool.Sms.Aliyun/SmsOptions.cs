using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sms.Aliyun
{
    public class SmsOptions
    {
        public string SEPARATOR { get; private set; } = "&";
        public string RegionId { get; set; } = "cn-hangzhou";
        public string Version { get; set; } = "2017-05-25";
        public string Action { get; set; } = "SendSms";
        public string Format { get; set; } = "JSON";
        public string Domain { get; set; } = "dysmsapi.aliyuncs.com";
        public int MaxRetryNumber { get; set; } = 3;
        public bool AutoRetry { get; set; } = true;
        public int TimeoutInMilliSeconds { get; set; } = 100000;


        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
    }
}
