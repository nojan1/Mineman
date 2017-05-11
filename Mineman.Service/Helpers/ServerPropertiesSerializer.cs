using Mineman.Common.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.IO;

namespace Mineman.Service.Helpers
{
    public static class ServerPropertiesSerializer
    {
        public static void WriteToFile(ServerProperties serverProperties, string filename)
        {
            File.WriteAllText(filename, Serialize(serverProperties));
        }

        public static string Serialize(ServerProperties serverProperties)
        {
            var lines = new List<string>();

            foreach (var property in serverProperties.GetType().GetProperties())
            {
                var name = property.Name.ToLower()
                                        .Replace("__", ".")
                                        .Replace('_', '-');

                var value = property.GetValue(serverProperties)?.ToString().ToLower() ?? "";

                lines.Add($"{name}={value}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        public static ServerProperties Deserialize(string data)
        {
            var serverProperties = new ServerProperties();
            var properties = typeof(ServerProperties).GetProperties();

            foreach (var line in data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    var name = parts[0].Trim()
                                       .Replace(".", "__")
                                       .Replace('-', '_');

                    var rawValue = parts[1].Trim();

                    var property = properties.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
                    if (property != null)
                    {
                        var value = Convert.ChangeType(rawValue, property.PropertyType);

                        property.SetValue(serverProperties, value);
                    }
                }
            }

            return serverProperties;
        }

        public static ServerProperties Merge(ServerProperties existingProperties, ServerProperties newProperties)
        {
            var properties = typeof(ServerProperties).GetProperties();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(NonUserChangableProperty)) == null)
                {
                    var value = property.GetValue(newProperties);

                    if (value != null)
                    {
                        property.SetValue(existingProperties, value);
                    }
                }
            }

            return existingProperties;
        }
    }
}
