using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChocolateForKonata.BotInternal;
using Konata.Core.Interfaces.Api;

namespace ChocolateForKonata
{
    static public class GroupMessage
    {
        internal static void Main(object? sender, GroupMessageEvent e, Bot bot)
        {
            Console.WriteLine(e.Chain.ToString());
            try
            {
                if (e.MemberUin == bot.Uin)                   
                {
                    tryRecall(e,bot);
                    return;
                }
                if (e.Message == null)
                    return;
                string commandString = e.Message.Chain.ToString();
                //Console.WriteLine(commandString);
                commandString = CommandStandarlize(commandString);
                var textChain = e.Message.Chain.GetChain<TextChain>;
                MessageBuilder Reply = null;
                if (commandString == "/ping")
                    Reply = BotFunction.Sys.Ping();
                else if (commandString == "/c restart another")
                    Reply = BotFunction.Sys.RestartAnotherBot();
                else if (commandString.StartsWith("/c module "))
                    Reply = BotFunction.Sys.Switches.SwitchMain(e, bot);
                else if (commandString == "图来" && CanBeUse.test("图", e))
                    Reply = BotFunction.Hso.Hso.GetHso(e, bot);
                else if (commandString == "/c hso update")
                    BotFunction.Hso.Hso.update(e, bot);
                else if (commandString.ToLower() == "/c advancedcmd hso demo" && CanBeUse.test("图", e))
                    Reply = BotFunction.Hso.Hso.GetHso(e, bot, "GifDemo");
                else if (Util.Rand.CanIDo(0.05f) && CanBeUse.test("复读", e))
                    Reply = BotFunction.Sys.Repeat(e.Message.Chain);
                else if (CanBeUse.test("高强度复读", e))
                    Reply = BotFunction.Sys.Repeat(e.Message.Chain);

                if (Reply != null)
                    bot.SendGroupMessage(e.GroupUin, Reply);
                return;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
          
            
        }

        private static void tryRecall(GroupMessageEvent e, Bot bot)
        {
            if (e.Message.Chain[0].Type == Konata.Core.Message.Model.XmlChain.ChainType.Xml)
            {
                var msgString = e.Message.Chain.ToString();
                string m_fileNameWithAfter = msgString.Substring(msgString.IndexOf("m_fileName=") + "m_fileName=".Length + 1);
                string mfn = m_fileNameWithAfter.Substring(0, m_fileNameWithAfter.IndexOf("\""));
                if (UsersData.HsoMsgList.Contains(mfn))
                {
                    
                    UsersData.HsoMsgList.Remove(mfn);
                    Task.Run(() => {
                        Thread.Sleep(90000);
                        bot.RecallMessage(e.Message);
                    });
                }
            }
            
        }

        private static string CommandStandarlize(string commandString)
        {
            if (commandString == "[KQ:image,file=B407F708A2C6A506342098DF7CAC4A57,width=198,height=82,length=7746,type=1000]")
                return "图来";
            if (commandString == "[KQ:image,file=523F541F30A684B471EAB31695310299,width=614,height=587,length=31926,type=1000]")
                return "图来";

            return commandString;
        }
    }
}
