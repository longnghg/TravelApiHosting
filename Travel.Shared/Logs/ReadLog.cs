using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.Logs
{
    public static class ReadLog
    {
        public  static List<LogModel> SimpleRead(string path)
        {
            var list = new List<LogModel>();
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    var serializer = new JsonSerializer();
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        jsonTextReader.SupportMultipleContent = true;

                        while (jsonTextReader.Read())
                        {
                            JObject obj = JObject.Load(jsonTextReader);
                            var logEntry = JsonConvert.DeserializeObject<LogModel>(obj.ToString());
                            list.Add(logEntry);
                        }
                    }
                }
            }
            return list;
        }
    }
}
