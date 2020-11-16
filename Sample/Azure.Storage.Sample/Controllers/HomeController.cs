using Azure.Storage.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tool.Azure.Storage;

namespace Azure.Storage.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzureStorageService _storage;

        public HomeController(ILogger<HomeController> logger, IAzureStorageService storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "REG07.json");
            var file = new FileInfo(filePath);
            using var stream = file.Open(FileMode.OpenOrCreate);
            string path = await _storage.UploadFilesFromStreamAsync(stream, "test/REG07.json");
            return View();
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
    }
}
