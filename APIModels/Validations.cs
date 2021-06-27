﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace APIModels
{
    public static class Validations
    {
        public static bool IsValidBoleta(string boleta) => Regex.IsMatch(boleta, "20([0-9]{8})");

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
    }
}