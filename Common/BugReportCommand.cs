using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    /*
    public class BugReportCommand : ModCommand
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public override CommandType Type => CommandType.Chat;
        public override string Command => "bug";
        public override string Usage => "/bug \"<description>\" {<tags>} \"<true/false>\"";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Match:
            //   "anything in quotes"
            //   {anything in curly braces}
            //   "anything in quotes again"
            var pattern = "\\\"([^\\\"]+)\\\"\\s*\\{([^}]+)\\}\\s*\\\"([^\\\"]+)\\\"";
            var match = Regex.Match(input, pattern);

            if (!match.Success)
            {
                caller.Reply("Usage: /bug \"<description>\" {<tag1, tag2>} \"<true/false>\"");
                return;
            }

            string reportString = match.Groups[1].Value.Trim();
            string tags = match.Groups[2].Value.Trim();
            string urgentStr = match.Groups[3].Value.Trim();

            if (!bool.TryParse(urgentStr, out bool urgent))
            {
                caller.Reply("Urgent value must be \"true\" or \"false\".");
                return;
            }

            _ = SendReportAsync(reportString, tags, urgent);

            caller.Reply("Bug report sent! Thank you for helping improve the mod.");
        }

        private async Task SendReportAsync(string reportString, string tags, bool urgent)
        {
            var json = $"{{\"ReportString\":\"{reportString}\",\"ReportTags\":\"{tags}\",\"Urgent\":{urgent.ToString().ToLower()}}}";

            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://holecatbugreport.constantinethewyvernofficial.workers.dev/", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Main.NewText($"Failed to send bug report: {ex.Message}", 255, 0, 0);
            }
        }
    }
    */
}
