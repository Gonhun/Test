using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult LoadData([DataSourceRequest] DataSourceRequest request, string param)
        {
            try
            {
                ClsUpload cls = new ClsUpload();
                return Json(cls.showAll(param).ToDataSourceResult(request));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult UploadFiles()
        {
            DataTable dt = new DataTable();
            var iStrUpload = string.Empty;
            string id = System.Guid.NewGuid().ToString();
            ClsUpload clsUpload = new ClsUpload();
            string Name;
            string iStrRemarks;
            bool iStatus;

            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = fileContent.FileName.Substring(fileContent.FileName.LastIndexOf("\\") + 1);
                        Name = fileContent.FileName;
                        var path = Path.Combine(Server.MapPath("~/Csv/upload"), fileName);
                        fileContent.SaveAs(path);

                        dt = clsUpload.ProcessCSV(path, id);
                        iStrUpload = clsUpload.ProcessBulkCopy(dt, "DB_IACCESSConnectionString", "dbo.TBLM_CONTENT");
                    }
                }

                if (iStrUpload == "Complete")
                {
                    iStrRemarks = "Upload Success";
                    iStatus = true;
                }
                else
                {
                    iStrRemarks = iStrUpload;
                    iStatus = false;
                }

                return Json(new { status = iStatus, remarks = iStrRemarks, JsonRequestBehavior.AllowGet });
            }
            catch(Exception e)
            {
                return Json(new { status = false, remarks = e.ToString(), JsonRequestBehavior.AllowGet });
            }
        }
    }
}