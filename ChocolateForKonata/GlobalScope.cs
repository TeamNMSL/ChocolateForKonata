using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace ChocolateForKonata
{
    public static class GlobalScope
    {
        public static void Initialization() {
            if (!File.Exists(@".\BotSettings"))
            {
                File.WriteAllText(@".\BotSettings", "Path=\nAdminUin=");
                Console.WriteLine("BotSettings Created,Edit it and run bot again\n[Help]\nPath:The path of bot data like C:\\User\\Konata\\Document\\Kagami\nAdminUin:The QQUin of Admin,like Admin=114514,1919,810");
                Console.ReadKey();
                Environment.Exit(0);
            }
            var BotCfgContent=File.ReadAllText(@".\BotSettings");
            foreach (var item in BotCfgContent.Split("\n"))
            {
                if (item.StartsWith("Path="))
                {
                    string t = item;
                    Path.AppPath = t.Replace("Path=","").Replace("\r","").Replace("\n","");
                }
                else if (item.StartsWith("AdminUin="))
                {
                    string[] t= item.Replace("AdminUin=", "").Replace("\r", "").Replace("\n", "").Split(",");
                    foreach (var i in t)
                    {
                        Cfgs.BotAdmins.Add(ulong.Parse(i));
                    }
                }
            }
            if (!Directory.Exists(Path.AppPath))
                Directory.CreateDirectory(Path.AppPath);
            Path.DatabasePath = $"{Path.AppPath}\\Databases";
            if (!Directory.Exists(Path.DatabasePath))
                Directory.CreateDirectory(Path.DatabasePath);
            if (!File.Exists($"{Path.DatabasePath}\\Switches"))
                BotInternal.CanBeUse.Initial();
            Path.TmpPath = $"{Path.AppPath}\\Tmp";
            if (!Directory.Exists(Path.TmpPath))
                Directory.CreateDirectory(Path.TmpPath);
            Path.HsoPath = $"{Path.AppPath}\\HsoPicture";
            if (!Directory.Exists(Path.HsoPath))
                Directory.CreateDirectory(Path.HsoPath);
            Path.imgPath = $"{Path.AppPath}\\Images";
            if (!Directory.Exists(Path.HsoPath))
                Directory.CreateDirectory(Path.HsoPath);
            Cfgs.imgApi = File.ReadAllText($"{GlobalScope.Path.AppPath}\\imgApi").Replace("\r","").Replace("\n","");

        }
        public static class Path {
            public static string AppPath;
            public static string DatabasePath;
            public static string TmpPath;
            public static string HsoPath;
            public static string imgPath;
        }
        public static class Cfgs {
            public static List<ulong>BotAdmins=new List<ulong>();
            public static List<string> FunctionList = new List<string>() {
            "复读","图","高强度复读"
            };
            public static string imgApi = "";
        }
    }
}
