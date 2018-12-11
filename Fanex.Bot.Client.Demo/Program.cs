namespace Fanex.Bot.Client.Console
{
    using System;
    using System.Configuration;

    internal class Program
    {
        public static void Main(string[] args)
        {
            BotClientManager.UseConfig(new Configuration.BotSettings(
                    new Uri(ConfigurationManager.AppSettings["FanexBotClient:BotServiceUrl"]),
                    ConfigurationManager.AppSettings["FanexBotClient:ClientId"],
                    ConfigurationManager.AppSettings["FanexBotClient:ClientPassword"]));

            var botConnector = new BotConnector();
            var message = "Missing revision \n Lrf_CustomerSel \n+ 20181031@Patrick: DB052 indexes revolution[RedmineID: #103572]";
            botConnector.Send(message, "29:1CrTamk06NfEFPFPTCv6jnB86xX-fUEdB3Ifwoz6O3iI");
            botConnector.Send("test message from Bot Client Test 2", "29:1CrTamk06NfEFPFPTCv6jnB86xX-fUEdB3Ifwoz6O3iI");
        }
    }
}