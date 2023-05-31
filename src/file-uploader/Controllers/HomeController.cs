using AutoWrapper.Wrappers;
using FileUploader.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DextersLabor;

namespace FileUploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MyDbContext _context;
        private const string _uploadDir = "uploadedFiles";
        public static DextersLabor.EfCoreHelper.SqlConnectionType CurrentSqlConnectionType;

        public HomeController(IWebHostEnvironment webHostEnvironment, MyDbContext context)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            CurrentSqlConnectionType = EfCoreHelper.GetSqlConnectionType(_context);
            initSqlMasterdata();
        }

        public IActionResult Index() => RedirectToAction(nameof(Dropzone));//=> View();
        public IActionResult Dropbasket() => View();
        public IActionResult Dropzone()
        {
            // * still does not work since a FormattableString is needed (starting w/ $)
            // var sql = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "sql", "FindLastUpdate.sql"));
            // * you must name the col as [Value]
            FormattableString sql = $"SELECT max(TrainingDateTime) as [Value] FROM[kettler].[Trainings]";
            var result = _context.Database.SqlQuery<string>(sql);
            ViewBag.LastUpdate = result.First();
            return View();
        }

        public IActionResult FindStreaks()
        {
            string sql = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "sql", "FindStreaks.sql"));
            _context.Database.ExecuteSqlRaw(sql);
            return RedirectToAction(nameof(Index));
        }


        // from dropzone, uses scripts/dropzone.js
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            WebHelper.IFormFileToFile(file, Path.Combine(_webHostEnvironment.WebRootPath, _uploadDir));
            return RedirectToAction(nameof(Index));
        }

        private string correctTrainingXml(string xml)
        {
            // xml MUST be formatted properly, rearrange training and record nodes
            xml = xml.Replace("</Training>", "<Records>");
            xml = string.Concat(
                xml,
                "</Records>", Environment.NewLine,
                "</Training>");
            return xml;
        }


        public IActionResult SaveAndDeleteFiles()
        {
            try
            {
                var timeIsNow = DateTime.Now;
                var fileInfos = FileHelper.GetFileInfoFromDirectory(Path.Combine(_webHostEnvironment.WebRootPath, _uploadDir), "*.xml");
                string xml;
                Training training;
                foreach (var item in fileInfos)
                {
                    // Create Db object here
                    xml = System.IO.File.ReadAllText(item.FullName);
                    training = SerializeHelper.GetObjectFromXmlString<Training>(correctTrainingXml(xml), "Training");

                    // save, if no duplicate
                    if (!isDuplicate(item.Name))
                    {
                        training.Process(timeIsNow, item.Name);
                        save(training);
                    }

                    // now delete files
                    item.Delete();
                }
                return View(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult Process()
        {
            var fileInfos = FileHelper.GetFileInfoFromDirectory(Path.Combine(_webHostEnvironment.WebRootPath, _uploadDir), "*.xml");
            return View(fileInfos);
        }

        private bool isDuplicate(string fileName) => _context.Trainings.Any(t => t.FileName == fileName);


        private void save(Training training)
        {
            _context.Trainings.Add(training);
            _context.SaveChanges();
        }

        /// <summary>
        /// Initializes default domain objects (master data) on fresh Db
        /// </summary>
        private void initSqlMasterdata()
        {
            // INMEMORY has no tables..
            if (!_context.Database.IsInMemory())
            {
                if (!EfCoreHelper.TableExists(_context, "dbo", "dim_Date"))
                {
                    var sql = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "sql/CreateDimDate_1_Table.sql"));
                    _context.Database.ExecuteSqlRaw(sql);
                }
                if (!EfCoreHelper.TableExists(_context, "kettler", "dim_Date"))
                {
                    var sql = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "Sql/CreateDimDate_2_View.sql"));
                    _context.Database.ExecuteSqlRaw(sql);
                }
            }

        }


        // from dropbasket
        [HttpPost]
        public IActionResult UploadFiles()
        {
            long size = 0;
            // get submitted files
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                string filename = _webHostEnvironment.WebRootPath + $@"\uploadedFiles\{file.FileName}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            string message = $"{files.Count} file(s) / {size} bytes uploaded successfully!";
            return Json(message);
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
