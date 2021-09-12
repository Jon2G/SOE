using SOEWeb.Shared.Enums;
using System;
using System.Text.RegularExpressions;

namespace SOEWeb.Shared
{
    public static class Validations
    {
        public static bool IsValidBoleta(string boleta)
        {
            if (string.IsNullOrEmpty(boleta))
            {
                return false;
            }
            return Regex.IsMatch(boleta, "20([0-9]{8})");
        }
        public static bool IsValidUser(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return false;
            }
            if (Regex.IsMatch(user, "20([0-9]{8})"))
            {
                return true;
            }else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(user);
                    return addr.Address == user;
                }
                catch
                {
                    return false;
                }
            }
                
            return false;
        }

        public static bool IsValidUrl(string url,out Uri uri)
        {
            if (string.IsNullOrEmpty(url))
            {
                uri = null;
                return false;
            }
            if ((Uri.TryCreate(url, UriKind.Absolute, out uri) ||
                 Uri.TryCreate("http://" + url, UriKind.Absolute, out uri)) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return true;
            }
            return false;
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
            !string.IsNullOrEmpty(NickName) && NickName.Length >= 4 && NickName.Length <= 10;

        public static string ValidateLogin(string Boleta, string Password, string NickName, string Email, School School, string DeviceKey, UserType UserType)
        {
            if (string.IsNullOrEmpty(Boleta))
            {
                return "La boleta no puede estar vacia.";
            }
            if (!Validations.IsValidBoleta(Boleta))
            {
                return "Su boleta parece ser invalida.";
            }

            if (string.IsNullOrEmpty(Password))
            {
                return "El password no debe estar vacio";
            }
            if (Password.Length < 8)
            {
                return "El password debe contener al menos 8 carácteres";
            }

            if (!Validations.IsValidNickName(NickName))
            {
                return "El password debe contener al menos 8 carácteres";
            }

            if (!Validations.IsValidEmail(Email))
            {
                return "El correo es invalido";
            }

            if (School is null)
            {
                return "Debe seleccionar una escuela";
            }

            if (string.IsNullOrEmpty(School.Name))
            {
                return "La escuela no debe estar vacia";
            }

            if (string.IsNullOrEmpty(DeviceKey))
            {
                return "Dispositivo no soportado";
            }

            if (UserType == UserType.INVALID)
            {
                return "Tipo de usuario no valido";
            }

            return null;
        }
    }
}
