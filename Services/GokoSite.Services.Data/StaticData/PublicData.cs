﻿namespace GokoSite.Services.Data.StaticData
{
    using System;
    using System.IO;
    using System.Net;

    public class PublicData
    {
        public static string apiKey => "RGAPI-a8fed5f3-a8fc-4f25-9602-39132289dafa";

        public static string ddVerision => GetLatestVersion();

        private static string GetLatestVersion()
        {
            string url = "https://ddragon.leagueoflegends.com/api/versions.json";
            var req = (HttpWebRequest)WebRequest.Create(url);
            var latestVersion = string.Empty;

            req.Method = "GET";

            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("error code: " + res.StatusCode);
                }

                using (Stream resStream = res.GetResponseStream())
                {
                    if (resStream != null)
                    {
                        using (StreamReader reader = new StreamReader(resStream))
                        {
                            string[] result = reader.ReadToEnd().Split(new char[] { '[', ']', '\\', '\"', '{', '}', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

                            latestVersion = result[0];
                        }
                    }
                }
            }

            return latestVersion;
        }
    }
}
