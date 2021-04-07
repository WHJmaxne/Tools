using Tools.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tool.Azure.Storage;
using Tool.Sms.Aliyun;
using Tool.VerifyCode;
using Aspose.Pdf;
using Microsoft.Azure.Storage.DataMovement;

namespace Tools.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzureStorageService _storage;
        private readonly ISmsService _sms;
        private readonly IVerifyCodeService _verifyCodeService;

        public HomeController(ILogger<HomeController> logger, IAzureStorageService storage, ISmsService sms, IVerifyCodeService verifyCodeService)
        {
            _logger = logger;
            _storage = storage;
            _sms = sms;
            _verifyCodeService = verifyCodeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConvertWord()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "TempFile", "eRetail2.0接口文档.pdf");
            Document pdfDocument = new Document(filePath);
            string saveFile = filePath.Replace(".pdf", ".docx");
            pdfDocument.Save(saveFile, SaveFormat.DocX);
            return Ok();
        }

        public async Task<IActionResult> Storage()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "REG07.json");
            var file = new FileInfo(filePath);
            using var stream = file.Open(FileMode.OpenOrCreate);
            string path = await _storage.UploadFilesFromStreamAsync(stream, "test/REG07.json");
            return Ok();
        }

        public async Task<IActionResult> LargeFileUpload()
        {
            string sourcePath = Path.Combine("D:\\FTP\\餐饮从业人员学生营养知识技能培训课程", "1我国中小学生营养状况.mp4");
            string path = await _storage.LargeFileTransferAsync(sourcePath, "test001/1我国中小学生营养状况.mp4", ProgressCallback);
            return Ok();
        }

        public async Task<IActionResult> GetSasUrl()
        {
            string url = _storage.GetSecureURl("");
            return Ok(url);
        }

        public async Task<IActionResult> SendSms()
        {
            IDictionary<string, string> data = new Dictionary<string, string> { { "name", "n1盒子" } };
            SmsObject sms = new SmsObject
            {
                Data = data,
                Mobile = "手机号",
                Signature = "签名",
                TempletKey = "模板Id",
                OutId = ""
            };
            await this._sms.SendAsync(sms);
            return Ok();
        }

        public IActionResult GetVerifyCode()
        {
            string code = this._verifyCodeService.CreateVerifyCode();
            var img = this._verifyCodeService.BytesVerifyCode(code);
            return File(img, "image/jpeg");
        }

        public IActionResult GetSlideVerifyCode()
        {
            var result = this._verifyCodeService.SlideVerifyCode();
            // X 坐标记录在缓存中用作验证
            result.PositionX = default;
            return Json(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void ProgressCallback(TransferStatus transfer)
        {
            Console.WriteLine("Bytes uploaded: {0}", transfer.BytesTransferred);
        }
    }
}
