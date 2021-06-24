using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kit;
using Kit.Sql.Helpers;
using Kit.Sql.SqlServer;
using APIModels;
using APIModels.Enums;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Newtonsoft.Json;

namespace SOEAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppAuthenticationController : ControllerBase
    {
        private readonly SQLServerConnection Connection;
        private readonly ILogger<AppAuthenticationController> _logger;
        public AppAuthenticationController(ILogger<AppAuthenticationController> logger)
        {
            Connection = new SQLServerConnection("APP_AUTHENTICATION", "192.168.0.32\\SQLEXPRESS", "1433", "sa", "12345678");
            _logger = logger;

        }
        [HttpGet()]
        public ActionResult<string> Hello()
        {
            return $"Hello there, it's {DateTime.Now.ToShortTimeString()} o'clock , {DateTime.Now.ToShortDateString()}";
        }
        [HttpGet("IsDeviceEnrolled/{DeviceKey}")]
        public ActionResult<string> IsDeviceEnrolled(string DeviceKey)
        {
            if (string.IsNullOrEmpty(DeviceKey))
            {
                return "INVALID_REQUEST";
            }
            try
            {
                Device device = Device.GetByKey(Connection, DeviceKey);
                if (device is null)
                {
                    return "UNREGISTERED";
                }
                if (!device.Enabled)
                {
                    return "DISABLED";
                }
                User user = SOEAPI.User.GetByDevice(Connection, device);
                if (user is null)
                {
                    return "INVALID_REQUEST";
                }
                if (user.Banned)
                {
                    return "BANNED";
                }
                if (user.Deleted)
                {
                    return "DELETED";
                }
                device.UpdateLastTimeSeen(Connection, _logger);
                return "OK";

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return "ERROR";
            }
        }

        //[HttpGet("EnrollDevice/{DeviceKey}/{DeviceBrand}/{Platform}/{Name}/{Model}/{AppKey}/{Boleta}/{Password}")]
        private string EnrollDevice(string DeviceKey, string DeviceBrand, string Platform, string Name, string Model, string AppKey, string Boleta, string Password, string School)
        {
            if (string.IsNullOrEmpty(DeviceKey) || string.IsNullOrEmpty(AppKey) || string.IsNullOrEmpty(Boleta))
            {
                return "INVALID_REQUEST";
            }
            try
            {
                if (UserLogin(Boleta, Password, School).ResponseResult != APIResponseResult.OK)
                {
                    return "INVALID_REQUEST";
                }
                User User = SOEAPI.User.GetByBoleta(Connection, Boleta);
                if (User is null)
                {
                    return "USER_NOT_FOUND";
                }
                if (string.IsNullOrEmpty(DeviceBrand))
                {
                    DeviceBrand = "GENERIC";
                }
                if (string.IsNullOrEmpty(Platform))
                {
                    Platform = "GENERIC";
                }
                Device NewDevice = Device.GetByKey(Connection, DeviceKey);
                if (NewDevice is null)
                {
                    NewDevice = new Device()
                    {
                        Brand = DeviceBrand,
                        DeviceKey = DeviceKey,
                        Enabled = true,
                        Guid = Guid.NewGuid(),
                        Id = -1,
                        LastTimeSeen = DateTime.Now,
                        Model = Model,
                        Name = Name,
                        Platform = Platform,
                        UserId = User.Id
                    };
                    NewDevice.Save(Connection);
                }
                return "SUCCES";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return "ERROR";
            }
        }

        [HttpGet("LogIn/{UserName}/{Password}/{School}")]
        [HttpGet("LogIn/{UserName}/{Password}")]
        public ActionResult<Response> LogIn(string UserName, string Password, string School = null)
        {
            return UserLogin(UserName, Password, School);
        }
        private Response UserLogin(string Usuario, string PasswordPin, string School)
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin)
                                              || PasswordPin.Length < 8)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }

            object mail = DBNull.Value;
            object boleta = DBNull.Value;

            if (Validations.IsValidBoleta(Usuario))
            {
                boleta = Usuario;
            }
            else if (Validations.IsValidEmail(Usuario))
            {
                mail = Usuario;
            }
            else
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            try
            {
                Response response = APIModels.Response.From(Connection.Read("SP_LOGIN"
                    , CommandType.StoredProcedure
                    , new SqlParameter("Mail", mail)
                    , new SqlParameter("Boleta", boleta)
                    , new SqlParameter("PASSWORD_PIN", PasswordPin)
                    , new SqlParameter("SCHOOL_NAME", (object)School ?? DBNull.Value)
                ));
                switch (response.ResponseResult)
                {
                    case APIResponseResult.SHOULD_ENROLL:
                        if (boleta != DBNull.Value)
                        {
                            return new Response(APIResponseResult.KO, "Usuario o contraseña incorrectos");
                        }
                        break;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }

        [HttpGet("SignUp/{Boleta}/{Nombre}/{Email}/{Password}/{School}/{PasswordSAES}/{Type}/{Device}")]
        public ActionResult<Response> SignUp(string Boleta, string Nombre, string Email, string Password, string School, string PasswordSAES, int Type, string Device)
        {
            try
            {
                return SignUp(Boleta, Nombre, Email, Password, School, PasswordSAES, (UserType)Type, JsonConvert.DeserializeObject<Device>(Device));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }

        private Response SignUp(string Boleta, string Nombre, string Email, string Password, string School, string PasswordSAES, UserType Type, Device Device)
        {
            if (string.IsNullOrEmpty(Boleta) || string.IsNullOrEmpty(Password) || Password.Length < 8
                || string.IsNullOrEmpty(PasswordSAES)
                || !Validations.IsValidEmail(Email)
                || !Validations.IsValidBoleta(Boleta)
                || string.IsNullOrEmpty(School)
                || string.IsNullOrEmpty(Device.DeviceKey)
                || Type == UserType.INVALID)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            try
            {
                Response response = APIModels.Response.From(
                    Connection.Read("SP_SIGNUP"
                    , CommandType.StoredProcedure
                    , new SqlParameter("BOLETA", Boleta)
                    , new SqlParameter("NAME", Nombre)
                    , new SqlParameter("MAIL", Email)
                    , new SqlParameter("PASSWORD_PIN", Password)
                    , new SqlParameter("PASSWORD_SAES", PasswordSAES)
                    , new SqlParameter("SCHOOL_NAME", School)
                    , new SqlParameter("DEVICE_KEY", Device.DeviceKey)
                    , new SqlParameter("BRAND", Device.Brand)
                    , new SqlParameter("PLATFORM", Device.Platform)
                    , new SqlParameter("MODEL", Device.Model)
                    , new SqlParameter("D_NAME", Device.Name)
                    , new SqlParameter("TYPE", (int)Type)
                ));
                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }

        }


    }
}
