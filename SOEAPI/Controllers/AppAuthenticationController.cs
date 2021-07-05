using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Kit;
using Kit.Sql.Helpers;
using Kit.Sql.SqlServer;
using APIModels;
using APIModels.Enums;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Kit.Services.Web;
using Newtonsoft.Json;
using SOEAPI.Processors;

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
            Connection = new SQLServerConnection(@"Server=tcp:soe-app.database.windows.net,1433;Initial Catalog=SOE_DATABASE;Persist Security Info=False;User ID=soeapp.soporte;Password=Octopus$2021.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            _logger = logger;

        }
        [HttpGet()]
        public ActionResult<string> Hello()
        {
            return $"Hello there, it's {DateTime.Now.ToShortTimeString()} o'clock , {DateTime.Now.ToShortDateString()}";
        }
        [HttpGet("TestDb")]
        public ActionResult<Response> TestDb()
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            if (Connection.TestConnection() is Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.ToString());
            }
            sp.Stop();
            return new Response(APIResponseResult.OK, $"Time elapsed:{sp.Elapsed:G}");
        }
        [HttpGet("LogIn/{UserName}/{Password}/{DeviceKey}/{School}")]
        [HttpGet("LogIn/{UserName}/{Password}/{DeviceKey}")]
        public ActionResult<Response> LogIn(string UserName, string Password, string DeviceKey, string School = null)
        {
            return UserLogin(UserName, Password, DeviceKey, School);
        }
        private Response UserLogin(string Usuario, string PasswordPin, string DeviceKey, string School)
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin)
                                              || PasswordPin.Length < 8
                                              || string.IsNullOrEmpty(DeviceKey))
            {
                return APIModels.Response.InvalidRequest;
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
                    , new SqlParameter("DEVICE_KEY", DeviceKey)
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
        [HttpGet("SignUp/{Boleta}/{Nombre}/{Email}/{Password}/{School}/{Type}/{Device}")]
        public ActionResult<Response> SignUp(string Boleta, string Nombre, string Email, string Password, string School, int Type, string Device)
        {
            try
            {
                return SignUp(Boleta, Nombre, Email, Password, School, (UserType)Type, JsonConvert.DeserializeObject<Device>(Device));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        private Response SignUp(string Boleta, string Nombre, string Email, string Password, string School, UserType Type, Device Device)
        {
            if (string.IsNullOrEmpty(Boleta) || string.IsNullOrEmpty(Password) || Password.Length < 8
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
        [HttpPost("PostClassTime/{User}")]
        public ActionResult<Response> PostClassTime(string User, [FromBody] byte[] HTML)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            string xml = ClassTimeDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), User, Connection, this._logger);
            if (string.IsNullOrEmpty(xml))
            {
                return APIModels.Response.Error;
            }
            sp.Stop();
            return new Response(APIResponseResult.OK, xml, $"Time elapsed:{sp.Elapsed:G}");
        }
        [HttpPost("PostGrades/{User}")]
        public ActionResult<Response> PostGrades(string User, [FromBody] byte[] HTML)
        {
            string xml = GradesDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), User, Connection, this._logger);
            if (string.IsNullOrEmpty(xml))
            {
                return APIModels.Response.Error;
            }
            return new Response(APIResponseResult.OK, xml);
        }
        [HttpGet("PostCareer/{CareerName}/{User}")]
        public ActionResult<Response> PostCareer(string CareerName, string User)
        {
            if (!Validations.IsValidBoleta(User) && !Validations.IsValidEmail(User))
            {
                return APIModels.Response.InvalidRequest;
            }
            try
            {
                return APIModels.Response.From(Connection.Read("SP_GET_ADD_CAREER"
                    , CommandType.StoredProcedure
                    , new SqlParameter("CAREER_NAME", CareerName)
                    , new SqlParameter("USER", User)
                ));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpGet("PostToDo/{User}/{Todo}")]
        public Response PostToDo(string User, TodoBase Todo)
        {
            if (Todo is null || string.IsNullOrEmpty(Todo.Title)
                             || Guid.Empty == Todo.Guid
                             || Todo.Subject is null
                             || Todo.Subject.Id <= 0
                             || Todo.Subject.IdTeacher <= 0)
            {
                return APIModels.Response.Error;
            }
            if (Todo.Date > DateTime.Now)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla");
            }

            try
            {
                return APIModels.Response.From(Connection.Read("SP_POST_TODO"
                    , CommandType.StoredProcedure
                    , new SqlParameter("GUID", Todo.Guid)
                    , new SqlParameter("SUBJECT_ID", Todo.Subject.Id)
                    , new SqlParameter("TEACHER_ID", Todo.Subject.IdTeacher)
                    , new SqlParameter("USER", User)
                    , new SqlParameter("TITLE", Todo.Title)
                    , new SqlParameter("DESCRIPTION", Todo.Description)
                    , new SqlParameter("T_DATE", Todo.Date)
                    , new SqlParameter("T_TIME", Todo.Time)
                    , new SqlParameter("GROUP", Todo.Subject.Group)
                ));

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }

        [HttpPost("PostTodoPicture/{ToDoGuid}")]
        public Response PostTodoPicture(Guid ToDoGuid,[FromBody] byte[] Img)
        {
            if (Img is null || Img.Length <= 0 || Guid.Empty == ToDoGuid)
            {
                return APIModels.Response.Error;
            }
            return APIModels.Response.From(Connection.Read("SP_POST_TODO_PICTURE"
                , CommandType.StoredProcedure
                , new SqlParameter("TODO_GUID", ToDoGuid)
                , new SqlParameter("IMG",Img)
            ));
        }
        [HttpPost("ShareTodo/{ToDoGuid}")]
        public Response ShareTodo(Guid ToDoGuid)
        {

        }
    }
}
