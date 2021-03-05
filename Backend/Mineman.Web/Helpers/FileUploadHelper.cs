using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Mineman.Web.Helpers
{
    public class FileUploadHelper
    {
        public static ZipArchive ZipFromFormFile(IFormFile file)
        {
            return new ZipArchive(file.OpenReadStream(), ZipArchiveMode.Read);
        }
    }
}
