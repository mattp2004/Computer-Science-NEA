using GameServer.src.misc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Reflection;

namespace GameServer.src.config
{
    class customConfig
    {
        public string ServerName { get; set; }
        public string ServerIP { get; set; }
        public ushort ServerPort { get; set; }
        public bool DevServer { get; set; }
        public string DefaultGame { get; set; }
        public bool Whitelisted { get; set; }
    }

    class Config
    {
        //https://stackoverflow.com/questions/7881148/how-do-i-get-the-exe-name-of-a-c-sharp-console-application
        public static customConfig configs;
        public static string configName = "config.json";
        public static string logsName = "Logs";
        public static string fileName = Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase) +".exe";
        public static string version = "0.1";
        public static string serverName = "";
        public static string serverIP = "";
        public static ushort serverPort;
        public static bool DevServer = false;
        public static string DefaultGame = "null";
        public static bool Whitelisted = false;

        public static void UpdateConfig()
        {
            try
            {
                string configRaw = System.IO.File.ReadAllText(configName);
                configs = JsonConvert.DeserializeObject<customConfig>(configRaw);
                SetJsonConfig();
            }
            catch(Exception e)
            {
                Util.Error(e);
            }
        }

        public static void GenerateConfig()
        {
            Console.WriteLine("Generating new config");
            customConfig newConfig = new customConfig();
            newConfig.ServerName = "GameServer";
            newConfig.ServerIP = "localhost";
            newConfig.ServerPort = 6000;
            newConfig.DevServer = false;
            newConfig.DefaultGame = "RPS";
            newConfig.Whitelisted = false;

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            //https://stackoverflow.com/questions/57272814/serialize-to-json-to-get-one-object-per-line
            using (StreamWriter sw = new StreamWriter(configName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, newConfig);
            }
        }

        private static void SetJsonConfig()
        {
            serverName = configs.ServerName;
            serverIP = configs.ServerIP;
            serverPort = configs.ServerPort;
            DevServer = configs.DevServer;
            DefaultGame = configs.DefaultGame;
            Whitelisted = configs.Whitelisted;
        }
    }
}
