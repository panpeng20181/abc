using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using preview.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace preview.Controllers
{
    public class HomeController : Controller
    {
        private MongoDBOperation<BsonDocument> mongo = null;
        public HomeController()
        {
            mongo = MongoDBOperation<BsonDocument>.GetMongoDBInstance();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult UploadFile()
        {
            return View();
        }
        [BindProperty]
        public FileUpload FileUpload { get; set; }

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
                        strpath = Path.Combine("Upload", file.FileName);
                        path = Path.Combine(environment.WebRootPath, strpath);

                        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var document = new BsonDocument
                        {
                            { "name", file.FileName },
                            { "type", file.ContentType },
                            { "count", GetMD5HashFromFile(path) },
                        };
                        mongo.InsertOneData(document);//插入一行数据
                    }
                    data.Msg = "上传成功";
                    data.PhotoPath = strpath;
                    return Redirect("/Home/Index");
                    //return Json(data);

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
        /// <summary>
         /// 获取文件MD5值
         /// </summary>
         /// <param name="fileName">文件绝对路径</param>
         /// <returns>MD5值</returns>
         public static string GetMD5HashFromFile(string fileName)
         {
             try
             {
                 FileStream file = new FileStream(fileName, FileMode.Open);
                 System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                 byte[] retVal = md5.ComputeHash(file);
                 file.Close();
 
                 StringBuilder sb = new StringBuilder();
                 for (int i = 0; i<retVal.Length; i++)
                 {
                     sb.Append(retVal[i].ToString("x2"));
                 }
                 return sb.ToString();
             }
             catch (Exception ex)
             {
                 throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
             }
         }
        public class PicData
        {
            public string Msg { get; set; }
            public string PhotoPath { get; set; }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public ActionResult Index()
        {

            List<MongodbModel> mdlist = new List<MongodbModel>();


            //MongodbModel model = new MongodbModel();
            if (mongo != null)
            {
                //long result = mongo.DeleteData();//删除


                List<BsonDocument> document1 = mongo.FindAnsyncData();
                //List<BsonDocument> list = mongo.FindData();
                //mongo.UpdateOneData(list[0]["_id"].ToString(), document);

                //BsonDocument 支持索引查询。
                ViewData["name"] = document1[0]["name"].ToString();
                ViewData["type"] = document1[1]["type"].ToString();
                ViewBag.Name = document1[0]["name"].ToString();
                ViewBag.Type = document1[1]["type"].ToString();

                for (int i = 0; i < document1.Count; i++)
                {
                    MongodbModel model = new MongodbModel();

                    model.name = document1[i]["name"].ToString();
                    model.type = document1[i]["type"].ToString();
                    model.count = document1[i]["count"].ToString();

                    mdlist.Add(model);
                }
            }
            //return PartialView("/*part*/", mdlist);
            return View(mdlist);
        }

        public IActionResult querymongodb(string dbname)//[FromBody] object paras)// string[] arr)
        {
            //if (arr.Length <= 0)
            //{
            //    return null;
            //}
            dbname = Request.Query["dbname"].ToString();// Get请求

            //dbname = Request.Form["dbname"].ToString();//POST请求
            if (string.IsNullOrEmpty(dbname.ToString()))
            {
                return null;
            }

            var document = new BsonDocument
            {
                { "name", dbname.ToString() },
                { "type", "DB"},
            };
            List<MongodbModel> mdlist = new List<MongodbModel>();

            if (mongo != null)
            {
                List<BsonDocument> document1 = mongo.FindFilterlData(document);
                for (int i = 0; i < document1.Count; i++)
                {
                    MongodbModel model = new MongodbModel();

                    model.name = document1[i]["name"].ToString();
                    model.type = document1[i]["type"].ToString();
                    model.count = document1[i]["count"].ToString();

                    mdlist.Add(model);
                }
            }
            return PartialView("part", mdlist);
            //return View(mdlist);
        }

        [HttpPost]
        public IActionResult querymon(string dbname)
        {
            dbname = Request.Form["dbname"].ToString();

            var document = new BsonDocument
            {
                { "name", dbname },
                { "type", dbname},
            };
            List<MongodbModel> mdlist = new List<MongodbModel>();

            if (mongo != null)
            {
                List<BsonDocument> document1 = mongo.FindFilterlData(document);
                for (int i = 0; i < document1.Count; i++)
                {
                    MongodbModel model = new MongodbModel();

                    model.name = document1[i]["name"].ToString();
                    model.type = document1[i]["type"].ToString();
                    model.count = document1[i]["count"].ToString();

                    mdlist.Add(model);
                }
            }
            return PartialView("part", mdlist);
            //return View(mdlist);
        }
        [HttpPost]
        public IActionResult OnPostSearchquerymon(string dbname2)
        {
            if (string.IsNullOrEmpty(dbname2))
            {
                return null;
            }
            var document = new BsonDocument
            {
                { "name", dbname2 },
                { "type", "DB"},
            };
            List<MongodbModel> mdlist = new List<MongodbModel>();

            if (mongo != null)
            {
                List<BsonDocument> document1 = mongo.FindFilterlData(document);
                for (int i = 0; i < document1.Count; i++)
                {
                    MongodbModel model = new MongodbModel();

                    model.name = document1[i]["name"].ToString();
                    model.type = document1[i]["type"].ToString();
                    model.count = document1[i]["count"].ToString();

                    mdlist.Add(model);
                }
            }
            return View(mdlist);
        }

        public IActionResult Create(string id, string name1, string name2, int type)
        {
            
            name1 = "C:\\Users\\Admin\\source\\repos\\WebApplication1\\preview\\wwwroot\\Upload\\" + "" + name1 + "";
            if (type == 1)
            {
                name2 = "C:\\Users\\Admin\\source\\repos\\WebApplication1\\preview\\wwwroot\\Upload\\" + "" + id + ".pdf";
            }
            else
            {
                name2 = "C:\\Users\\Admin\\source\\repos\\WebApplication1\\preview\\wwwroot\\Upload\\" + "" + id + ".html";
            }
            RunBat("C:\\Program Files (x86)\\OpenOffice 4\\program\\pdf.bat", name1, name2);

            if (type == 1)
            {
                return Redirect("/Upload/" + id + ".pdf");
            }
            else
            {
                return Redirect("/Upload/" + id + ".html");
            }


            //return View();
        }
        ///batPath为.bat 可执行文件的绝对路径
        private void RunBat(string batPath,string name1,string name2)
        {
            Process p = new Process();
            string path = batPath;//bat路径  
            ProcessStartInfo pi = new ProcessStartInfo(path, ""+name1+" "+name2+"");//第二个参数为传入的参数，string类型以空格分隔各个参数  
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;
            p.StartInfo = pi;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();


            //Process pro = new Process();
            //FileInfo file = new FileInfo(batPath);
            //pro.StartInfo.WorkingDirectory = file.Directory.FullName;
            //pro.StartInfo.FileName = batPath;
            //pro.StartInfo.CreateNoWindow=true;
            //pro.Start();
            //pro.WaitForExit();
        }
        //[HttpPost]
        //public async Task<IActionResult> Create(string dbname)
        //{
        //    dbname = Request.Form["dbname"].ToString();
        //    string dbtye = Request.Form["dbtye"].ToString();
        //    string dbcount = Request.Form["dbcount"].ToString();

        //    if (string.IsNullOrEmpty(dbname))
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var document = new BsonDocument
        //    {
        //        { "name", dbname },
        //        { "type", dbtye},
        //        { "count",dbcount},
        //    };

        //    if (mongo != null)
        //    {
        //        await mongo.InsertAsyncOneData(document);
        //    }
        //    return RedirectToAction(nameof(Index));
        //}
    }
}