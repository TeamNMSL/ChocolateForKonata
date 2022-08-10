using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace BotWeb.Controllers
{
    [Route("api/Setu")]
    [ApiController]
    public class Setu : ControllerBase
    {
        [HttpGet]
        public string GetHso() {
            return "111";
            string hsoPath = BotWeb.Rand.Random_File(BotWeb.Rand.Random_Folders(@"D:\ServerData\Chocolate\HsoPicture\DownloadFromPixiv"));
            var resp = File(System.IO.File.ReadAllBytes(hsoPath), "image/jpeg");
            
            //return resp;
        }
    }
}
