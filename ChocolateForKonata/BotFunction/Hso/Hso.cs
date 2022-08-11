using AnimatedGif;
using ICSharpCode.SharpZipLib.Zip;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using System.Collections;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ChocolateForKonata.BotFunction.Hso
{
    public static  class Hso
    {
        static public (bool,string) imgUpload(string filePath)
        {
            var client = new RestClient(GlobalScope.Cfgs.imgApi);
            var request = new RestRequest("", Method.Post);
            client.Options.MaxTimeout = 30000;
            request.AddFile("source", filePath);
            var response = client.Execute(request);
            client.Dispose();
            request = null;
            try
            {
                JObject resultJson = (JObject)JsonConvert.DeserializeObject(response.Content);
                string url = resultJson["image"]["url"].ToString();
                return (true, url);
                
            }
            catch (Exception e)
            {

                return (false, e.Message);
            }
        }
        private static string GenerateQRByThoughtWorks(string content)
        {
            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            encoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            encoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;//错误效验、错误更正(有4个等级)
            encoder.QRCodeBackgroundColor = Color.White;
            encoder.QRCodeForegroundColor = Color.Green;
            Bitmap bcodeBitmap = encoder.Encode(content, Encoding.ASCII);
            string path = $"{GlobalScope.Path.TmpPath}\\HsoQR_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfffffff")}.png";
            bcodeBitmap.Save(path+".progress.png", ImageFormat.Png);
            bcodeBitmap.Dispose();
            Bitmap QR = new Bitmap(431, 866);
            using (Graphics g = Graphics.FromImage(QR)) { 
                //94,102,238,238
                g.DrawImage(Image.FromStream(new FileStream($"{GlobalScope.Path.imgPath}\\HsoBack\\Background.jpg", FileMode.Open, FileAccess.Read)),0,0,431,866);
                g.DrawImage(Image.FromStream(new FileStream(path + ".progress.png", FileMode.Open, FileAccess.Read)), 94, 102,238,238);
                g.Save();
            }
            QR.Save(path,ImageFormat.Png);
            return path;
        }

       



        public static MessageBuilder GetHso(Konata.Core.Events.Model.GroupMessageEvent e,Bot bot,string Mode="null") {
            

                string basepath = $"{GlobalScope.Path.HsoPath}\\DownloadFromPixiv";//图库路径
                string artistpath = Util.Rand.Random_Folders(basepath);//画师图库路径
                string artist = artistpath.Replace(basepath + "\\", "");//图库路径下的画师相对路径,画师名
                string Pic = Util.Rand.Random_File(artistpath);//图片绝对路径
                string picinfo = Pic.Replace(artistpath + "\\", "");//图片绝对路径去掉画师图库路径所得到的string
            if (Mode == "GifDemo")
            {
                Pic = @"D:\GeneralData\Pictures\DownloadFromPixiv\(871625)lambda\(95551392)白金燐子　gif@60ms.zip";
                picinfo = "(95551392)白金燐子　gif@60ms.zip";
                
            }
            string format=picinfo.Substring(picinfo.IndexOf("."));
                if (Pic.EndsWith("zip"))
                {
                    Pic = HsoGIF(Pic, picinfo);
                }
                var dt = DateTime.Now;
                string fileName = $"{Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds)}{format}";
                File.Copy(Pic, $"{GlobalScope.Path.TmpPath}\\{fileName}");
                var tryUploadResult = imgUpload(Pic);
                if (tryUploadResult.Item1)
                {

                    
                    var chainToSend = new MultiMsgChain()
                       .AddMessage(3396014855, "傻逼",
                       new MessageBuilder()
                       .Text($"图片来力~~~~\nURL:{tryUploadResult.Item2}\n时间不等人，请珍惜时间")
                       .Build())
                       .AddMessage(3396014855, "傻逼"
                       , new MessageBuilder()
                           .Text($"画师:{artist}\n")
                           .Text($"文件名:{picinfo}")
                           .Build())
                       .AddMessage(bot.Uin, bot.Name,
                       new MessageBuilder()
                       .Text($"感谢AkulaKirov提供的图床QWQ")
                       .Build());
                string msgString = new MessageBuilder(chainToSend).Build().ToString();
                
                string m_fileNameWithAfter = msgString.Substring(msgString.IndexOf("m_fileName=")+ "m_fileName=".Length+1);
                string mfn = m_fileNameWithAfter.Substring(0, m_fileNameWithAfter.IndexOf("\""));
                
                UsersData.HsoMsgList.Add(mfn);
                bot.SendGroupMessage(e.GroupUin, chainToSend);
                

                return null;
                }
                else
                {
                    return new MessageBuilder()
                  .Text($"Failed to send funny picture because\n{tryUploadResult.Item2}");
                }
               
            
            
            
        }

        private static string? HsoGIF(string pic,string picinfo)
        {
            string foldName = $"{GlobalScope.Path.TmpPath}\\HsoGifFold_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ssfffffff")}";
            if (!Decompress(pic, foldName))
            {
                
                return null;
            }
            int fps;
            fps = int.Parse(Util.Texts.Between(picinfo, "@", "ms.zip"));

            try
            {
                using (var gif = AnimatedGif.AnimatedGif.Create($"{foldName}\\HsoGif.gif", (int)1000 / fps))
                {
                    DirectoryInfo root = new DirectoryInfo(foldName);
                    FileInfo[] files = root.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        gif.AddFrame(Image.FromFile(file.FullName), delay: -1, quality: GifQuality.Default);
                    }

                }
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Out of Memory Exception Occured while megering gif");
                return $"{foldName}\\HsoGif.gif";

            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return null;
            }
            return $"{foldName}\\HsoGif.gif";


        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="targetPath">目标路经</param>
        static public bool Decompress(string sourceFile, string targetPath)
        {
            if (!File.Exists(sourceFile))
            {
                throw new FileNotFoundException(string.Format("未能找到文件 '{0}' ", sourceFile));
            }
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(sourceFile)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directorName = Path.Combine(targetPath, Path.GetDirectoryName(theEntry.Name));
                    string fileName = Path.Combine(directorName, Path.GetFileName(theEntry.Name));
                    // 创建目录
                    if (directorName.Length > 0)
                    {
                        Directory.CreateDirectory(directorName);
                    }
                    if (fileName != string.Empty)
                    {
                        using (FileStream streamWriter = File.Create(fileName))
                        {
                            int size = 4096;
                            byte[] data = new byte[4 * 1024];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else break;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public static void update(GroupMessageEvent e, Bot bot)
        {
            try
            {

                List<string> itemlist = new();
                foreach (var item in getArtists())
                {
                    itemlist.Add($"pxder -u {item}");
                }
                if (File.Exists($"{GlobalScope.Path.AppPath}\\PxderHso.ps1"))
                    File.Delete($"{GlobalScope.Path.AppPath}\\PxderHso.ps1");
                File.WriteAllText($"{GlobalScope.Path.AppPath}\\PxderHso.ps1", String.Join('\n', itemlist));
                bot.SendGroupMessage(e.GroupUin, "已输出脚本到Bot目录,请自行执行");
               
            }
            catch (Exception ex)
            {

                bot.SendGroupMessage(e.GroupUin, $"[Hso]任务失败，发生了异常\n{ex.ToString()}");
            }
        }

        private static IEnumerable<string> getArtists()
        {
            string dirPath = GlobalScope.Path.HsoPath+ "\\DownloadFromPixiv";
            ArrayList list = new ArrayList();
            List<string> dirs = new List<string>(Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories));
            List<string> artists=new();
            foreach (var item in dirs)
            {
                artists.Add(Util.Texts.Between(item.Replace(dirPath + "\\", ""),"(",")"));
            }
            return artists;
        }
    }
}
