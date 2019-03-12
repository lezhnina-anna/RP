using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        static readonly string url = "http://localhost:5000/api/values/";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TextDetails(string id)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url + id);
            string[] result = (await response.Content.ReadAsStringAsync()).Split('_');
            if (result[0] == "null") {
                ViewData["TextDetails"] = "";
                ViewData["TextData"] = "";
            } else {
                ViewData["TextDetails"] = result[0];
                ViewData["TextData"] = result[1];
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string data)
        {
            string id = await GetResponse(data);
            Console.WriteLine("id: " + id);
            return Redirect("TextDetails/" + id); ;
        }

        private async Task<string> GetResponse(string data)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
