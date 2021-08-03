using System;
using System.Text.RegularExpressions;

namespace SOEWeb.Shared
{
    public static class Validations
    {
        public static bool IsValidBoleta(string boleta) => Regex.IsMatch(boleta, "20([0-9]{8})");

        public static bool IsValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidNickName(string NickName) => 
            NickName.Length >= 4 && NickName.Length <= 10;
    }
}
