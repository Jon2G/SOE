using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kit;
using Kit.Sql.Helpers;
using Kit.Sql.SqlServer;

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

        [HttpGet("EnrollDevice/{DeviceKey}/{DeviceBrand}/{Platform}/{Name}/{Model}/{AppKey}/{Boleta}/{Password}")]
        public ActionResult<string> EnrollDevice(string DeviceKey, string DeviceBrand, string Platform, string Name, string Model, string AppKey, string Boleta, string Password)
        {
            if (string.IsNullOrEmpty(DeviceKey) || string.IsNullOrEmpty(AppKey) || string.IsNullOrEmpty(Boleta))
            {
                return "INVALID_REQUEST";
            }
            try
            {
                if (UserLogin(Boleta, Password) != "OK")
                {
                    return "INVALID_REQUEST";
                }
                User User =SOEAPI.User.GetByBoleta(Connection,Boleta);
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

        [HttpGet("LogIn/{UserName}/{Password}")]
        public ActionResult<string> LogIn(string UserName, string Password)
        {
            return UserLogin(UserName, Password);
        }
        private string UserLogin(string Boleta, string PasswordPin)
        {
            if (string.IsNullOrEmpty(Boleta) || string.IsNullOrEmpty(PasswordPin))
            {
                return "INVALID_REQUEST";
            }
            try
            {
                int id = Connection.Single<int>(
                     @"select ID From USERS where (MAIL=@MAIL OR BOLETA=@BOLETA) and PASSWORD_PIN=@PASSWORD_PIN"
                     , System.Data.CommandType.Text
                      , new SqlParameter("BOLETA", Boleta)
                      , new SqlParameter("PASSWORD_PIN", PasswordPin)
                      );
                if (id > 0)
                {
                    return "OK";
                }
                else
                {
                    return "KO";
                }

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return ex.Message;
            }
        }






    }
}
