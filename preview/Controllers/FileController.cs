using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace preview.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upload([FromServices]IHostingEnvironment environment)
        {

            var data = new PicData();
            string path = string.Empty;
            string strpath = string.Empty;
            var files = Request.Form.Files.Where(c => c.Name == "MyPhoto01");
            if (files == null || files.Count() <= 0) { data.Msg = "请选择上传的文件。"; return Json(data); }
            //格式限制
            var allowType = new string[] { "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation" };
            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 40)
                {
                    foreach (var file in files)
                    {
                        strpath = Path.Combine("Upload", DateTime.Now.ToString("MMddHHmmss") + file.FileName);
                        path = Path.Combine(environment.WebRootPath, strpath);

                        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    data.Msg = "上传成功";
                    data.PhotoPath = strpath;
                    return Json(data);
                }
                else
                {
                    data.Msg = "文件过大";
                    return Json(data);
                }
            }
            else

            {
                data.Msg = "文件格式错误";
                return Json(data);
            }
        }
        public class PicData
        {
            public string Msg { get; set; }
            public string PhotoPath { get; set; }
        }
    }
}