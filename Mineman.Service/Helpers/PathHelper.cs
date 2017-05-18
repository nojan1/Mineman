using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mineman.Service.Helpers
{
    public static class PathHelperExtensions
    {
        public static string BuildPath(this IHostingEnvironment environment, params string[] pathParts)
        {
            var path = Path.Combine(pathParts);

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(environment.ContentRootPath, path);
            }

            return path;
        }
    }
}
