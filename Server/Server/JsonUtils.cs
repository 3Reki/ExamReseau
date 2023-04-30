using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetworkServer
{
    internal static class JsonUtils
    {
        public static void WriteToFile(object obj, string fileName)
        {
            var jsonString = JsonSerializer.Serialize(obj);
            File.WriteAllText(fileName, jsonString);
        }

        public static T ReadFromFile<T>(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
