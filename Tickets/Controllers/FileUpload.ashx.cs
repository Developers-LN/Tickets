using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tickets.Controllers
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class FileUpload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string fileUrl = "";
            string sourceFileName = "";
            if (context.Request.Files.Count > 0)
            {
                string patch = context.Server.MapPath("~") + "generalRaffle";
                if (!System.IO.Directory.Exists(patch))
                {
                    System.IO.Directory.CreateDirectory(patch);
                }

                var pathName = context.Request.Form["pathName"];
                patch += "/" + pathName;
                if (!System.IO.Directory.Exists(patch))
                {
                    System.IO.Directory.CreateDirectory(patch);
                }

                HttpPostedFile file = context.Request.Files[0];
                string fileName = Guid.NewGuid().ToString() + ".xml";
                sourceFileName = file.FileName;
                file.SaveAs(patch + "/" + fileName);
                fileUrl = patch + "/" + fileName;
            }

            context.Response.ContentType = "text/plain";
            var result = new { result = true, fileUrl = fileUrl, sourceFileName = sourceFileName };
            context.Response.Write( Newtonsoft.Json.JsonConvert.SerializeObject( result));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
