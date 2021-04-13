using System;
using System.Threading.Tasks;

namespace Tool.Sms.Aliyun
{
    public interface ISmsService
    {
        Task<(bool success, string response)> SendAsync(SmsObject sms);
        Task<(bool success, string response)> SendVerifyCodeAsync(string Phone);
        Task<(bool success, string response)> SendVerifyCodeAsync(string Phone, string Code, string Signature, string TempleteKey, string OutId);
    }
}
