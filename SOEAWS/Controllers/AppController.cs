using Amazon.Lambda.Core;
using Kit;
using Kit.Sql.Helpers;
using Kit.Sql.Readers;
using Kit.Sql.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radzen.Blazor.Rendering;
using SOEAWS.Processors;
using SOEAWS.Services;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
            if (SQLServerConnection.TestConnection(WebData.Connection) is Exception ex)
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
            string xml = GradesDigester.Digest(System.Text.Encoding.UTF8.GetString(HTML), User, this._logger);
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


        [HttpGet("ShareTodo/{ToDoGuid}")]
        public ActionResult<Response> ShareTodo(Guid ToDoGuid)
        {
            TodoBase todo = TodoService.Find(ToDoGuid, this._logger,out string UserNick);
            if (todo is null)
            {
                return SOEWeb.Shared.Response.Error;
            }
            return new Response(
                APIResponseResult.OK,
                UserNick,
                JsonConvert.SerializeObject(todo));
        }
        [HttpGet("GetArchieveIds/{ArchieveGuid}")]
        public ActionResult<Response> GetArchieveIds(Guid ArchieveGuid)
        {
            int[] ids = ArchieveService.GetIdsByGuid(ArchieveGuid, this._logger);
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
            ReminderBase todo = ReminderService.Find(ToDoGuid, this._logger,out string NickName);

            if (todo is null)
            {
                return SOEWeb.Shared.Response.Error;
            }
            return new Response(
                APIResponseResult.OK,
                NickName,
                JsonConvert.SerializeObject(todo));
        }
        [HttpGet("GetClassmates/{Group}/{TeacherId}/{SubjectId}")]
        public ActionResult<Response> GetClassmates(string Group, string TeacherId, string SubjectId)
        {
            List<Classmate> Classmates = new List<Classmate>();
            WebData.Connection.Read(
                "SP_GET_CLASSMATES",
                (reader) =>
                {
                    Classmates.Add(new Classmate(Convert.ToString(reader[0]), Convert.ToString(reader[1])));
                }
                ,new CommandConfig(){CommandType = CommandType.StoredProcedure },
                new SqlParameter("GROUP", Group)
                , new SqlParameter("SUBJECT_ID", SubjectId)
                , new SqlParameter("TEACHER_ID", TeacherId));
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
                if (string.IsNullOrEmpty(Link.Url) || !UriExtensions.IsValidUrl(Link.Url, out Uri uri))
                {
                    return SOEWeb.Shared.Response.InvalidRequest;
                }

                WebData.Connection.Read(
                    "SP_POST_LINK",
                    (reader) =>
                    {
                        guid = (Guid)reader[0];
                    }
                    , new CommandConfig() { CommandType = CommandType.StoredProcedure }
                    , new SqlParameter("SUBJECT_ID", SubjectId)
                    , new SqlParameter("TEACHER_ID", IdTeacher)
                    , new SqlParameter("USER", User)
                    , new SqlParameter("GROUP", Group)
                    , new SqlParameter("URL", uri.AbsoluteUri)
                    , new SqlParameter("NAME", Link.Name)
                    );
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
            WebData.Connection.Read(
                "SP_GET_LINKS",
                (reader) =>
                {
                    Links.Add(
                        new Link(Convert.ToString(reader[0]), Convert.ToString(reader[1]))
                        {
                            Guid = (Guid)reader[2],
                            IsOwner = Convert.ToInt32(reader[3]) == UserId
                        });
                }
                , new CommandConfig() { CommandType = CommandType.StoredProcedure }
                , new SqlParameter("SUBJECT_ID", SubjectId)
                , new SqlParameter("TEACHER_ID", TeacherId),
                new SqlParameter("GROUP", Group));
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
                WebData.Connection.Execute(
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
                WebData.Connection.Execute(
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
        ////---------------------------------------------------------------------------------------------------------------------
        [HttpPost("PostContact/{User}/{SchoolId}")]
        public ActionResult<Response> PostContact(string User, int SchoolId, [FromBody] byte[] JsonContactBytes)
        {
            if (SchoolId <= 0
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
                string JsonContact = Encoding.UTF8.GetString(JsonContactBytes);
                if (JsonConvert.DeserializeObject<SchoolContact>(JsonContact) is not SchoolContact Contact)
                {
                    return SOEWeb.Shared.Response.InvalidRequest;
                }
                if (!string.IsNullOrEmpty(Contact.Url))
                {
                    if (!UriExtensions.IsValidUrl(Contact.Url, out Uri uri))
                    {
                        return SOEWeb.Shared.Response.InvalidRequest;
                    }
                    Contact.Url = uri.AbsoluteUri;
                }
                WebData.Connection.Read(
                     "SP_ADDCONTACT",
                     (reader) =>
                     {
                         guid = (Guid)reader[0];
                     }, new CommandConfig() { CommandType = CommandType.StoredProcedure }
                     , new SqlParameter("CONTACT_ID", Contact.Guid == Guid.Empty ? DBNull.Value : Contact.Guid)
                     , new SqlParameter("NAME", Contact.Name)
                     , new SqlParameter("PHONE", string.IsNullOrEmpty(Contact.Phone) ? DBNull.Value : Contact.Phone)
                     , new SqlParameter("MAIL", string.IsNullOrEmpty(Contact.Correo) ? DBNull.Value : Contact.Correo)
                     , new SqlParameter("DEPARTMENT_NAME", Contact.Departament.Name)
                     , new SqlParameter("URL", string.IsNullOrEmpty(Contact.Url) ? DBNull.Value : Contact.Url)
                     , new SqlParameter("USER", User)
                     , new SqlParameter("SCHOOL_ID", SchoolId)
                     );
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PostContact");
            }
            if (guid == Guid.Empty)
            {
                return new Response(APIResponseResult.NOT_READ,"Contacto no leido",guid.ToString("N"));
            }
            return new Response(APIResponseResult.OK, "Ok",guid.ToString("N"));
        }
        [HttpGet("GetContacts/{SchoolId}/{UserId}")]
        public ActionResult<Response> GetContacts(int SchoolId, int UserId)
        {
            Dictionary<Guid, ContactsByDeparment> Contacts = new();
            WebData.Connection.Read(
                "SP_GET_CONTACT",
                (reader) =>
                {
                    Guid IdDepartamento = (Guid)reader[6];
                    SchoolContact contact = new SchoolContact()
                    {
                        Guid = (Guid)reader[0],
                        Name = reader[1].ToString(),
                        Phone = Sqlh.IfNull<string>(reader[2], null),
                        Correo = Sqlh.IfNull<string>(reader[3], null),
                        Url = Sqlh.IfNull<string>(reader[4], null),
                        IsOwner = Convert.ToInt32(reader[5]) == UserId
                    };
                    if (Contacts.TryGetValue(IdDepartamento, out ContactsByDeparment Departament))
                    {
                        contact.Departament = Departament.Departament;
                        Departament.Add(contact);
                    }
                    else
                    {
                        contact.Departament = new Departament()
                        {
                            Guid = IdDepartamento,
                            Name = reader[7].ToString()
                        };
                        ContactsByDeparment bydeparment = new(contact.Departament);
                        bydeparment.Add(contact);
                        Contacts.Add(IdDepartamento, bydeparment);
                    }
                }
                , new CommandConfig() { CommandType = CommandType.StoredProcedure }
                , new SqlParameter("SCHOOL_ID", SchoolId)
            );
            string json = JsonConvert.SerializeObject(Contacts.Values.ToArray(), Formatting.Indented, new JsonSerializerSettings()
            {
                CheckAdditionalContent = true
            });
            return new Response(
                APIResponseResult.OK,
                "Ok", json);
        }
        [HttpGet("ReportContact/{ContactId}/{ReportReason}/{UserId}")]
        public ActionResult<Response> ReportContact(Guid ContactId, int ReportReason, int UserId) =>
            this.ReportContact(ContactId, (ReportReason)ReportReason, UserId);
        private ActionResult<Response> ReportContact(Guid ContactId, ReportReason ReportReason, int UserId)
        {
            if (ContactId == Guid.Empty)
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }

            try
            {
                WebData.Connection.Execute(
                    "SP_CONTACT_REPORT",
                    CommandType.StoredProcedure
                    , new SqlParameter("CONTACT_ID", ContactId)
                    , new SqlParameter("REPORT_REASON_ID", (int)ReportReason)
                    , new SqlParameter("USER_ID", UserId));
                WebData.Connection.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "ReportContact");
            }
            return new Response(APIResponseResult.OK, "Ok");
        }
        [HttpGet("DeleteContact/{UserId}/{ContactId}")]
        public ActionResult<Response> DeleteContact(int UserId, Guid ContactId)
        {
            if (ContactId == Guid.Empty || UserId <= 0)
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }

            try
            {
                WebData.Connection.Execute(
                    "SP_DELETE_CONTACT",
                    CommandType.StoredProcedure
                    , new SqlParameter("CONTACT_ID", ContactId)
                    , new SqlParameter("USER_ID", UserId));
                WebData.Connection.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "DeleteContact");
            }
            return new Response(APIResponseResult.OK, "Ok");
        }

        [HttpGet("GetSchoolId/{UserId}")]
        public ActionResult<Response> GetSchoolId(int UserId)
        {
            int SchoolId = -1;
            if (UserId <= 0)
            {
                return SOEWeb.Shared.Response.InvalidRequest;
            }

            try
            {
                SchoolId = WebData.Connection.Single<int>("SP_GETSCHOOL_ID_BY_USER_ID",
                       CommandType.StoredProcedure
                       , new SqlParameter("USER_ID", UserId));
                WebData.Connection.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "GetSchoolId");
            }
            return new Response(APIResponseResult.OK, "Ok", SchoolId.ToString());
        }
    }
}
