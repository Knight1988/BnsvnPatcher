using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using bnsvn_dat.Business;
using bnsvn_dat.DataAccess;
using bnsvn_dat.Models;
using FileResult = bnsvn_dat.Models.FileResult;

namespace bnsvn_dat.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public void AppendToFile(string fullPath, Stream content)
        {
            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (content)
                    {
                        content.CopyTo(stream);
                    }
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public ActionResult ChunkUploadSave(IEnumerable<HttpPostedFileBase> files, string metaData)
        {
            if (metaData == null)
            {
                return ChunkUploadAsyncSave(files);
            }

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(metaData));
            var serializer = new DataContractJsonSerializer(typeof(ChunkMetaData));
            ChunkMetaData chunkData = serializer.ReadObject(ms) as ChunkMetaData;
            string path = String.Empty;
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    path = Path.Combine(Server.MapPath("~/App_Data"), chunkData.FileName);

                    AppendToFile(path, file.InputStream);
                }
            }

            FileResult fileBlob = new FileResult();
            fileBlob.Uploaded = chunkData.TotalChunks - 1 <= chunkData.ChunkIndex;
            fileBlob.FileUid = chunkData.UploadUid;

            return Json(fileBlob);
        }

        public ActionResult ChunkUploadRemove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult ChunkUploadAsyncSave(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/bnsdat"), fileName);

                    file.SaveAs(physicalPath);
                }
            }

            MoveToBnsdat();

            // Remove old dir
            var dirs = Directory.GetDirectories(Server.MapPath("~/bnsdat"));
            foreach(var dir in dirs)
            {
                Directory.Delete(dir, true);
            }
            // Extract all dat files
            var bnsdat = new Process();
            bnsdat.StartInfo.FileName = Server.MapPath("~/bnsdat/extractall.bat");
            bnsdat.StartInfo.WorkingDirectory = Server.MapPath("~/bnsdat/");
            bnsdat.Start();
            bnsdat.WaitForExit();

            // Return an empty string to signify success
            return RedirectToAction("Index");
        }

        public ActionResult UploadProfile(IEnumerable<HttpPostedFileBase> files)
        {
            if (Session["username"] == null) RedirectToAction("Login");
            
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/Profiles"), Session["username"].ToString(), fileName);

                    file.SaveAs(physicalPath);
                }
            }
            
            return View();
        }

        public ActionResult UploadProfile()
        {
            if (Session["username"] == null) RedirectToAction("Login");
            return View();
        }
        
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult GetAllOutdatedProfile()
        {
            var db = new Model();
            var profiles = db.Profiles.Where(p => p.Version != db.Infoes.First().Version);
            return Json(profiles);
        }

        public ActionResult CompileProfile(int profileId)
        {
            var db = new Model();
            var profile = db.Profiles.Single(p => p.ProfileId == profileId);

            var profilePath = Server.MapPath(profile.Path);
            var xmlPatch = XmlPatchBusiness.ReadPatch(profilePath);

            return null;
        }

        private void MoveToBnsdat()
        {
            var sourceDir = Server.MapPath("~/App_Data");
            var destDir = Server.MapPath("~/bnsdat");
            var files = Directory.GetFiles(sourceDir, "*.dat");

            foreach (var file in files)
            {
                var sourceFile = new FileInfo(file);
                sourceFile.CopyTo(Path.Combine(destDir, sourceFile.Name), true);
                sourceFile.Delete();
            }
        }
    }
}