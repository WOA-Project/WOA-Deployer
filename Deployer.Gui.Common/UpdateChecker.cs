using System;
using System.Linq;
using System.Text.RegularExpressions;
using AutoUpdaterDotNET;
using HtmlAgilityPack;
using Serilog;

namespace Deployer.Gui
{
    public static class UpdateChecker
    {
        private static string url;

        public static void CheckForUpdates(string baseUrl)
        {
            url = $"{baseUrl}/releases/latest";

            AutoUpdater.ParseUpdateInfoEvent += AutoUpdaterOnParseUpdateInfoEvent;

            AutoUpdater.Start(url);
        }

        private static void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            try
            {
                args.UpdateInfo = GetUpdateInfo(args.RemoteData);
            }
            catch (Exception e)
            {
                Log.Verbose(e, "Error while checking for updates");
                args.UpdateInfo = new UpdateInfoEventArgs();
            }            
        }

        private static UpdateInfoEventArgs GetUpdateInfo(string argsRemoteData)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(argsRemoteData);

            return new UpdateInfoEventArgs
            {
                CurrentVersion = GetVersion(htmlDoc),
                DownloadURL = GetDownload(htmlDoc),
                ChangelogURL = url,
            };
        }

        private static Version GetVersion(HtmlDocument htmlDoc)
        {
            var versioNode = htmlDoc.DocumentNode.Descendants("span").First(d =>
                d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("css-truncate-target"));
            var match = Regex.Match(versioNode.InnerText, @"(\*|\d+(\.\d+)*(\.\*)?)", RegexOptions.Singleline);
            var version = new Version(match.Value);
            return version;
        }

        private static string GetDownload(HtmlDocument htmlDoc)
        {
            var fileNameNode = htmlDoc.DocumentNode.Descendants("a").First(d =>
                d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("d-flex flex-items-center"));

            var relativeDownloadUrl = fileNameNode.Attributes["href"].Value;
            var downloadUrl = "https://github.com" + relativeDownloadUrl;
            return downloadUrl;
        }
    }
}