using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System;
using SignalRHttps.Hubs;

namespace core_server.Controllers
{
    [ApiController]
    [Route("api/progress")] // 路由
    public class ProgressController : Controller
    {
        private readonly ILogger<ProgressController> _logger;
        private readonly IHubContext<ProgressHub> progressHubContext;

        public ProgressController(ILogger<ProgressController> logger, IHubContext<ProgressHub> _hubContext)
        {
            _logger = logger;
            progressHubContext = _hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Port(Dictionary<string, string> inModel)
        {
            Dictionary<string, string> outModel = new Dictionary<string, string>();
            int progress = 0;
            for (int i = 1; i <= 10; i++)
            {
                Thread.Sleep(500);
                progress = i * 10;
                // UpdProgress用于客户端请求的方法或事件
                await progressHubContext.Clients.All.SendAsync("UpdProgress", progress);
            }
            outModel["ResultMsg"] = "上传图片完成，正在处理中...";
            outModel["status"] = "200";
            return Json(outModel);
        }

        [HttpGet]
        public async Task<IActionResult> Port1()
        {
            int progress = 0;

            for (int i = 1; i <= 10; i++)
            {
                Thread.Sleep(500); // 暂停1秒
                progress = i * 10;
                await progressHubContext.Clients.All.SendAsync("HandleProgress", progress);
            }
            return Accepted(1);
        }
    }
}