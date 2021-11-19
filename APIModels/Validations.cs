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
            var match = Regex.Match(boleta, "20([0-9]{8})");
            return (match.Success && match.Value == boleta);
        }

        public static bool IsValidUser(string user) => IsValidBoleta(user) || IsValidEmail(user);


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


        public static string ValidateLogin(string Boleta, string NickName, string Email, School School, string DeviceKey, UserType UserType)
        {
            if (string.IsNullOrEmpty(Boleta))
            {
                return "La boleta no puede estar vacia.";
            }
            if (!Validations.IsValidBoleta(Boleta))
            {
                return "Su boleta parece ser invalida.";
            }
            if (!Validations.IsValidNickName(NickName))
            {
                return "El nickname debe contener al menos 4 carácteres";
            }

            if (!Validations.IsValidEmail(Email))
            {
                return "El correo es invalido";
            }

            if (School is null)
            {
                return "Debe seleccionar una escuela";
            }

            if (School.Id <= 0)
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
