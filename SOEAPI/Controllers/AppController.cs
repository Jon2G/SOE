using Amazon.Lambda.Core;
using Kit.Sql.Readers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOEWeb.Server.Processors;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Device = SOEWeb.Server.Models.Device;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace SOEWeb.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly ILogger<AppController> _logger;
        public AppController(ILogger<AppController> logger)
        {
            this._logger = logger;
        }
        [HttpGet()]
        [HttpGet("Hello")]
        public ActionResult<Response> Hello()
        {
            return new Response(APIResponseResult.OK, $"Hello there, it's {DateTime.Now.ToShortTimeString()} o'clock , {DateTime.Now.ToShortDateString()}");
        }
        [HttpGet("TestDb")]
        public ActionResult<Response> TestDb()
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            if (WebData.Connection.TestConnection() is Exception ex)
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
            return this.UserLogin(UserName, Password, DeviceKey, School);
        }
        private Response UserLogin(string Usuario, string PasswordPin, string DeviceKey, string School)
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin)
                                              || PasswordPin.Length < 8
                                              || string.IsNullOrEmpty(DeviceKey))
            {
                return SOEWeb.Shared.Response.InvalidRequest;
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
                Response response = SOEWeb.Shared.Response.From(WebData.Connection.Read("SP_LOGIN"
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
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpGet("SignUp/{Boleta}/{Nombre}/{NickName}/{Email}/{Password}/{School}/{Type}/{Device}")]
        public ActionResult<Response> SignUp(string Boleta, string Nombre, string NickName, string Email, string Password, string School, int Type, string Device)
        {
            try
            {
                return this.SignUp(Boleta, Nombre, NickName, Email, Password, School, (UserType)Type, JsonConvert.DeserializeObject<Device>(Device));
            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        private Response SignUp(string Boleta, string Nombre, string NickName, string Email, string Password, string School, UserType Type, Device Device)
        {
            if (string.IsNullOrEmpty(Boleta) || string.IsNullOrEmpty(Password) || Password.Length < 8
                || !Validations.IsValidEmail(Email)
                || !Validations.IsValidBoleta(Boleta)
                || string.IsNullOrEmpty(School)
                || string.IsNullOrEmpty(Nombre)
                || string.IsNullOrEmpty(NickName)
                || string.IsNullOrEmpty(Device.DeviceKey)
                || Type == UserType.INVALID)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            try
            {
                Response response = SOEWeb.Shared.Response.From(
                    WebData.Connection.Read("SP_SIGNUP"
                    , CommandType.StoredProcedure
                    , new SqlParameter("BOLETA", Boleta)
                    , new SqlParameter("NAME", Nombre)
                    , new SqlParameter("NICK_NAME", NickName)
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
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }

        }
        [HttpPost("PostClassTime/{User}")]
        public ActionResult<Response> PostClassTime(string User, [FromBody] byte[] HTML)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            string xml = ClassTimeDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), User, WebData.Connection, this._logger);
            if (string.IsNullOrEmpty(xml))
            {
                return SOEWeb.Shared.Response.Error;
            }
            sp.Stop();
            return new Response(APIResponseResult.OK, xml, $"Time elapsed:{sp.Elapsed:G}");
        }
        [HttpPost("PostGrades/{User}")]
        public ActionResult<Response> PostGrades(string User, [FromBody] byte[] HTML)
        {
            string xml = GradesDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), User, WebData.Connection, this._logger);
            if (string.IsNullOrEmpty(xml))
            {
                return SOEWeb.Shared.Response.Error;
            }
            return new Response(APIResponseResult.OK, xml);
        }
        [HttpGet("PostCareer/{CareerName}/{User}")]
        public ActionResult<Response> PostCareer(string CareerName, string User)
        {
            if (!Validations.IsValidBoleta(User) && !Validations.IsValidEmail(User))
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }
            try
            {
                return SOEWeb.Shared.Response.From(WebData.Connection.Read("SP_GET_ADD_CAREER"
                    , CommandType.StoredProcedure
                    , new SqlParameter("CAREER_NAME", CareerName)
                    , new SqlParameter("USER", User)
                ));
            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpPost("PostToDo/{User}")]
        public ActionResult<Response> PostToDo(string User, [FromBody] byte[] TodoBytes)
        {
            try
            {
                string todo_json = Encoding.UTF8.GetString(TodoBytes);
                TodoBase Todo = JsonConvert.DeserializeObject<TodoBase>(todo_json);
                if (Todo is null || string.IsNullOrEmpty(Todo.Title)
                                 || Guid.Empty == Todo.Guid
                                 || Todo.Subject is null
                                 || Todo.Subject.Id <= 0
                                 || Todo.Subject.IdTeacher <= 0)
                {
                    return SOEWeb.Shared.Response.Error;
                }
                if (DateTime.Now > Todo.Date)
                {
                    return new Response(APIResponseResult.INVALID_REQUEST,
                        "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla");
                }

                return SOEWeb.Shared.Response.From(WebData.Connection.Read("SP_POST_TODO"
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
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }

        [HttpPost("PostTodoPicture/{ToDoGuid}")]
        public ActionResult<Response> PostTodoPicture(Guid ToDoGuid, [FromBody] byte[] Img)
        {
            if (Img is null || Img.Length <= 0 || Guid.Empty == ToDoGuid)
            {
                return SOEWeb.Shared.Response.Error;
            }
            return SOEWeb.Shared.Response.From(WebData.Connection.Read("SP_POST_TODO_PICTURE"
                , CommandType.StoredProcedure
                , new SqlParameter("TODO_GUID", ToDoGuid)
                , new SqlParameter("IMG", Img)
            ));
        }

        private TodoBase TodoFrom(IReader reader)
        {
            if (reader.Read())
            {
                return new TodoBase()
                {
                    Guid = Guid.Parse(Convert.ToString(reader[0])),
                    Title = Convert.ToString(reader[1]),
                    Description = Convert.ToString(reader[2]),
                    Date = Convert.ToDateTime(reader[3]),
                    Time = TimeSpan.Parse(Convert.ToString(reader[4])),
                    Subject = new Subject()
                    {
                        Id = Convert.ToInt32(reader[5]),
                        IdTeacher = Convert.ToInt32(Convert.ToInt32(reader[6]))
                    }
                };
            }

            return null;
        }

        [HttpGet("ShareTodo/{ToDoGuid}")]
        public ActionResult<Response> ShareTodo(Guid ToDoGuid)
        {
            TodoBase todo = this.TodoFrom(WebData.Connection.Read(
                "SP_GET_TODO_BY_GUID",
                CommandType.StoredProcedure,
                new SqlParameter("GUID", ToDoGuid)));
            if (todo is null)
            {
                return SOEWeb.Shared.Response.Error;
            }
            return new Response(
                APIResponseResult.OK,
                "Ok",
                JsonConvert.SerializeObject(todo));
        }

        [HttpGet("GetArchieveIds/{ArchieveGuid}")]
        public ActionResult<Response> GetArchieveIds(Guid ArchieveGuid)
        {
            int[] ids = WebData.Connection.Lista<int>(
                "SP_GET_ARCHIEVE_ID_BY_GUID",
                CommandType.StoredProcedure, 0,
                new SqlParameter("GUID", ArchieveGuid))
                .ToArray();
            return new Response(
                APIResponseResult.OK,
                "Ok",
                JsonConvert.SerializeObject(ids));
        }
        [HttpGet("GetArchieveById/{Id}")]
        public FileContentResult GetArchieveById(int Id)
        {
            byte[] result = (byte[])WebData.Connection.Single("SP_GET_ARCHIEVE_BY_ID"
                , CommandType.StoredProcedure, new SqlParameter("ID", Id));
            if (result is null)
            {
                return null;
            }
            return this.File(result, "application/pdf", "picture.png");
        }
        [HttpGet("GetClassmates/{Group}")]
        public ActionResult<Response> GetClassmates(string Group)
        {
            List<Classmate> Classmates = new List<Classmate>();
            using (var reader = WebData.Connection.Read(
                "SP_GET_CLASSMATES",
                CommandType.StoredProcedure,
                new SqlParameter("GROUP", Group)))
            {
                while (reader.Read())
                {
                    Classmates.Add(new Classmate(Convert.ToString(reader[0]), Convert.ToString(reader[1])));
                }
            }
            return new Response(
                APIResponseResult.OK,
                "Ok",
                JsonConvert.SerializeObject(Classmates.ToArray()));
        }
    }
}
