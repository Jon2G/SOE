using Amazon.Lambda.Core;
using Kit;
using Kit.Sql.Readers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radzen.Blazor.Rendering;
using SOEAWS.Processors;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Device = SOEAWS.Models.Device;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace SOEAWS.Controllers
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
            return new Response(APIResponseResult.OK, $"2.0.1 - Hello there, it's {DateExtensions.MexicoCityCurrentDateTime().ToShortTimeString()} o'clock , {DateExtensions.MexicoCityCurrentDateTime().ToShortDateString()}");
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
                Response response = SOEWeb.Shared.Response.FromSql("SP_LOGIN"
                    , new SqlParameter("Mail", mail)
                    , new SqlParameter("Boleta", boleta)
                    , new SqlParameter("PASSWORD_PIN", PasswordPin)
                    , new SqlParameter("SCHOOL_NAME", (object)School ?? DBNull.Value)
                    , new SqlParameter("DEVICE_KEY", DeviceKey)
                );
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
                Response response = SOEWeb.Shared.Response.FromSql("SP_SIGNUP"
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
                );
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
            this._logger.Log(LogLevel.Debug, "PostClassTime");
            Stopwatch sp = new Stopwatch();
            sp.Start();
            var result = ClassTimeDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), User, this._logger);
            if (string.IsNullOrEmpty(result.Value))
            {
                return result.ToResponse();
            }
            sp.Stop();
            return new Response(APIResponseResult.OK, result.Value, $"Time elapsed:{sp.Elapsed:G}");
        }
        [HttpPost("PostGrades/{User}")]
        public ActionResult<Response> PostGrades(string User, [FromBody] byte[] HTML)
        {
            this._logger.Log(LogLevel.Debug, "PostGrades");
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
            this._logger.Log(LogLevel.Debug, "PostCareer");
            if (!Validations.IsValidBoleta(User) && !Validations.IsValidEmail(User))
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }
            try
            {
                return SOEWeb.Shared.Response.FromSql("SP_GET_ADD_CAREER"
                    , new SqlParameter("CAREER_NAME", CareerName)
                    , new SqlParameter("USER", User)
                );
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
                                 || Todo.Subject.IdTeacher <= 0
                                 || string.IsNullOrEmpty(User) ||
                                 (!Validations.IsValidEmail(User) && !Validations.IsValidBoleta(User)))
                {
                    return SOEWeb.Shared.Response.Error;
                }
                if (DateExtensions.MexicoCityCurrentDateTime() > Todo.Date.Add(Todo.Time))
                {
                    return new Response(APIResponseResult.INVALID_REQUEST,
                        "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla");
                }

                return SOEWeb.Shared.Response.FromSql("SP_POST_TODO"
                    , new SqlParameter("GUID", Todo.Guid)
                    , new SqlParameter("SUBJECT_ID", Todo.Subject.Id)
                    , new SqlParameter("TEACHER_ID", Todo.Subject.IdTeacher)
                    , new SqlParameter("USER", User)
                    , new SqlParameter("TITLE", Todo.Title)
                    , new SqlParameter("DESCRIPTION", Todo.Description)
                    , new SqlParameter("T_DATE", Todo.Date)
                    , new SqlParameter("T_TIME", Todo.Time)
                    , new SqlParameter("GROUP", Todo.Subject.Group)
                );

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
                return SOEWeb.Shared.Response.NotExecuted;
            }


            return SOEWeb.Shared.Response.FromSql("SP_POST_TODO_PICTURE"
                , new SqlParameter("TODO_GUID", ToDoGuid)
                , new SqlParameter("IMG", Img));
        }
        private TodoBase TodoFrom(IReader reader)
        {
            TodoBase todo = null;
            if (reader.Read())
            {
                todo = new TodoBase()
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
            reader.Dispose();
            return todo;
        }
        private ReminderBase ReminderFrom(IReader reader)
        {
            ReminderBase reminder = null;
            if (reader.Read())
            {
                reminder = new ReminderBase()
                {
                    Guid = Guid.Parse(Convert.ToString(reader[0])),
                    Title = Convert.ToString(reader[1]),
                    Date = Convert.ToDateTime(reader[2]),
                    Time = TimeSpan.Parse(Convert.ToString(reader[3]))
                };
                if (reader[4] != DBNull.Value)
                {
                    reminder.Subject = new Subject()
                    {
                        Id = Convert.ToInt32(reader[4]),
                        IdTeacher = Convert.ToInt32(Convert.ToInt32(reader[5]))
                    };
                }
            }
            reader.Dispose();
            return reminder;
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


        [HttpPost("PostReminder/{User}")]
        public ActionResult<Response> PostReminder(string User, [FromBody] byte[] TodoBytes)
        {
            try
            {
                string reminder_json = Encoding.UTF8.GetString(TodoBytes);
                ReminderBase Reminder = JsonConvert.DeserializeObject<ReminderBase>(reminder_json);

                if (Reminder is null || string.IsNullOrEmpty(Reminder.Title)
                                 || Guid.Empty == Reminder.Guid
                                 || string.IsNullOrEmpty(User) ||
                                 (!Validations.IsValidEmail(User) && !Validations.IsValidBoleta(User)))
                {
                    return SOEWeb.Shared.Response.Error;
                }
                if (DateExtensions.MexicoCityCurrentDateTime() > Reminder.Date)
                {
                    return new Response(APIResponseResult.INVALID_REQUEST,
                        "Este recordatorio ya ha expirado, cambie la fecha de entrega si desea compartirla");
                }

                return SOEWeb.Shared.Response.FromSql("SP_POST_REMINDER"
                    , new SqlParameter("GUID", Reminder.Guid)
                    , new SqlParameter("SUBJECT_ID", (object)Reminder.Subject?.Id ?? DBNull.Value)
                    , new SqlParameter("TEACHER_ID", (object)Reminder.Subject?.IdTeacher ?? DBNull.Value)
                    , new SqlParameter("USER", User)
                    , new SqlParameter("TITLE", Reminder.Title)
                    , new SqlParameter("R_DATE", Reminder.Date)
                    , new SqlParameter("R_TIME", Reminder.Time)
                    , new SqlParameter("GROUP", (object)Reminder.Subject?.Group ?? DBNull.Value)
                );

            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpGet("ShareReminder/{ToDoGuid}")]
        public ActionResult<Response> ShareReminder(Guid ToDoGuid)
        {
            ReminderBase todo = this.ReminderFrom(WebData.Connection.Read(
                "SP_GET_REMINDER_BY_GUID",
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
        [HttpGet("GetClassmates/{Group}/{TeacherId}/{SubjectId}")]
        public ActionResult<Response> GetClassmates(string Group, string TeacherId, string SubjectId)
        {
            List<Classmate> Classmates = new List<Classmate>();
            using (var reader = WebData.Connection.Read(
                "SP_GET_CLASSMATES",
                CommandType.StoredProcedure,
                new SqlParameter("GROUP", Group)
                , new SqlParameter("SUBJECT_ID", SubjectId)
                , new SqlParameter("TEACHER_ID", TeacherId)
            ))
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
        [HttpPost("PostLink/{SubjectId}/{IdTeacher}/{User}/{Group}")]
        public ActionResult<Response> PostLink(
            string SubjectId, string IdTeacher, string User, string Group, [FromBody] byte[] JsonLinkBytes)
        {
            if (string.IsNullOrEmpty(Group)
            || string.IsNullOrEmpty(User) ||
            (!Validations.IsValidEmail(User)
             && !Validations.IsValidBoleta(User))

            )
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }
            Guid guid = Guid.Empty;
            try
            {
                string JsonLink = Encoding.UTF8.GetString(JsonLinkBytes);
                if (JsonConvert.DeserializeObject<Link>(JsonLink) is not Link Link)
                {
                    return SOEWeb.Shared.Response.InvalidRequest;
                }
                if (string.IsNullOrEmpty(Link.Url) || !Validations.IsValidUrl(Link.Url,out Uri uri))
                {
                    return SOEWeb.Shared.Response.InvalidRequest;
                }

                using IReader reader = WebData.Connection.Read(
                    "SP_POST_LINK",
                    CommandType.StoredProcedure
                    , new SqlParameter("SUBJECT_ID", SubjectId)
                    , new SqlParameter("TEACHER_ID", IdTeacher)
                    , new SqlParameter("USER", User)
                    , new SqlParameter("GROUP", Group)
                    , new SqlParameter("URL", uri.AbsoluteUri)
                    , new SqlParameter("NAME", Link.Name)
                    );
                if (reader.Read())
                {
                    guid = (Guid)reader[0];
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PostLink");
            }
            return new Response(
               guid == Guid.Empty ?
                   APIResponseResult.INTERNAL_ERROR : APIResponseResult.OK,
               guid == Guid.Empty ? "Error" : "Ok",
               guid.ToString("N"));
        }
        [HttpGet("GetLinks/{Group}/{TeacherId}/{SubjectId}/{UserId}")]
        public ActionResult<Response> GetLinks(string Group, string TeacherId, string SubjectId, int UserId)
        {
            List<Link> Links = new List<Link>();
            using (var reader = WebData.Connection.Read(
                "SP_GET_LINKS",
                CommandType.StoredProcedure
                , new SqlParameter("SUBJECT_ID", SubjectId)
                , new SqlParameter("TEACHER_ID", TeacherId),
                new SqlParameter("GROUP", Group)))
            {
                while (reader.Read())
                {
                    Links.Add(
                        new Link(Convert.ToString(reader[0]), Convert.ToString(reader[1]))
                        {
                            Guid = (Guid)reader[2],
                            IsOwner = Convert.ToInt32(reader[3]) == UserId
                        });
                }
            }
            return new Response(
                APIResponseResult.OK,
                "Ok",
                JsonConvert.SerializeObject(Links.ToArray()));
        }
        [HttpGet("ReportLink/{UserId}/{LinkId}/{ReportReason}")]
        public ActionResult<Response> ReportLink(int UserId, Guid LinkId, int ReportReason) =>
            this.ReportLink(UserId, LinkId, (ReportReason)ReportReason);
        private ActionResult<Response> ReportLink(int UserId, Guid LinkId, ReportReason ReportReason)
        {
            if (LinkId == Guid.Empty || UserId <= 0)
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }

            try
            {
                WebData.Connection.EXEC(
                    "SP_REPORT_LINK",
                    CommandType.StoredProcedure
                    , new SqlParameter("LINK_ID", LinkId)
                    , new SqlParameter("REPORT_REASON_ID", (int)ReportReason)
                    , new SqlParameter("USER_ID", UserId));
                WebData.Connection.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PostLink");
            }
            return new Response(APIResponseResult.OK, "Ok");
        }
        [HttpGet("DeleteLink/{UserId}/{LinkId}")]
        public ActionResult<Response> DeleteLink(int UserId, Guid LinkId)
        {
            if (LinkId == Guid.Empty || UserId <= 0)
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }

            try
            {
                WebData.Connection.EXEC(
                    "SP_DELETE_LINK",
                    CommandType.StoredProcedure
                    , new SqlParameter("LINK_ID", LinkId)
                    , new SqlParameter("USER_ID", UserId));
                WebData.Connection.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PostLink");
            }
            return new Response(APIResponseResult.OK, "Ok");
        }
    }
}
