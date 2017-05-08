using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Service.Models
{
    public enum DockerImageCreationStreamObjectType
    {
        Info,
        Error
    }

    public class DockerImageCreationStreamObject
    {
        public string Info { get; private set; }
        public DockerImageCreationStreamObjectType Type { get; private set; }

        public DockerImageCreationStreamObject(string rawJson)
        {
            dynamic rawObject = JObject.Parse(rawJson);

            if(rawObject.error == null)
            {
                Type = DockerImageCreationStreamObjectType.Info;
                
                if(rawObject.stream != null)
                {
                    Info = rawObject.stream;
                }
                else if(rawObject.status != null)
                {
                    Info = rawObject.status + " " + rawObject.progressDetail.ToString();
                }
                else
                {
                    Info = rawJson;
                }

                Info = Info.Trim();
            }
            else
            {
                Type = DockerImageCreationStreamObjectType.Error;
                Info = rawObject.error + " " + rawObject.errorDetails;
            }
        }
    }
}
