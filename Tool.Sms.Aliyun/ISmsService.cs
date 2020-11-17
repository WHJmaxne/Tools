using System;
using System.Threading.Tasks;

namespace Tool.Sms.Aliyun
{
    public interface ISmsService
    {
        Task<(bool success, string response)> SendAsync(SmsObject sms);
    }
}
