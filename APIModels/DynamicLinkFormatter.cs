using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared
{
    public static class DynamicLinkFormatter
    {
        private const string FireBaseUrl = "https://soeapp.page.link/H3Ed";
        private const string FireBaseLargeUrl = "https://soeapp.page.link/?link=https://play.google.com/store/apps/details{0}&apn=com.soe.soe_app&isi=1580151317&ibi=com.soe.soe-ap";

        public static string GetDynamicUrl(string Verb, Dictionary<string, string> parameters)
        {
            string query = $"?params={Verb}";
            foreach (KeyValuePair<string, string> param in parameters)
            {
                query += $"{param.Value}={param.Key}?";
            }

            if (!query.EndsWith("?"))
            {
                query += '?';
            }
            return string.Format(FireBaseLargeUrl, query);
        }
    }
}
