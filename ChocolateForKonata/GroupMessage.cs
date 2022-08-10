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
            
            try
            {
                if (e.MemberUin == bot.Uin)
                    return;
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
                else if (commandString == "色图来" && CanBeUse.test("色图", e))
                    Reply = BotFunction.Hso.Hso.GetHso(e, bot);
                else if (commandString == "色图开" && CanBeUse.test("色图", e))
                    Reply = BotFunction.Hso.Hso.GetHso(e, bot,"Multi");
                else if (commandString == "/c hso update")
                    BotFunction.Hso.Hso.update(e, bot);
                else if (commandString.ToLower() == "/c advancedcmd hso demo" && CanBeUse.test("色图", e))
                    Reply = BotFunction.Hso.Hso.GetHso(e, bot, "GifDemo");
                else if (commandString == @"Task.Run(()=>{CycleHso(10,2000)});" && e.MemberUin == 1848200159)
                    BotFunction.Hso.Hso.TencentHsoBanTest(e, bot);
                else if (Util.Rand.CanIDo(0.05f) && CanBeUse.test("复读", e))
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

        private static string CommandStandarlize(string commandString)
        {
            if (commandString == "[KQ:image,file=B407F708A2C6A506342098DF7CAC4A57,width=198,height=82,length=7746,type=1000]")
                return "色图来";
            if (commandString == "[KQ:image,file=523F541F30A684B471EAB31695310299,width=614,height=587,length=31926,type=1000]")
                return "色图开";

            return commandString;
        }
    }
}
