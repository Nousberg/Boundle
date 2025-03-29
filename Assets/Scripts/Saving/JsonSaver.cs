using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Saving
{
    public static class JsonSaver
    {
        private static List<char> disallowedSymbols = new List<char>() { '#', '@', '!', '*', '&', '%', '$', '^' };

        public static bool Save(object data, string savePath, string name)
        {
            foreach (char c in name.ToCharArray())
            {
                if (disallowedSymbols.Contains(c))
                    return false;
            }

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string json = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            File.WriteAllText(savePath + name, json);
            return true;
        }
        public static T Load<T>(string savePath, string name)
        {
            if (!File.Exists(savePath + name))
                return default;

            string json = File.ReadAllText(savePath + name);
            return JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
        }
    }
}