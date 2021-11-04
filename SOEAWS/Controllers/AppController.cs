using Amazon.Lambda.Core;
using Kit;
using Kit.Services.Web;
using Kit.Sql.Helpers;
using Kit.Sql.Readers;
using Kit.Sql.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radzen.Blazor.Rendering;
using SOEAWS.Services;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using SOEWeb.Shared.Processors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Device = SOEAWS.Models.Device;
using WResponse = Kit.Services.Web.Response;

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
        public ActionResult<WResponse> Hello()
        {
            return new WResponse(APIResponseResult.OK, $"2.0.1 - Hello there, it's {DateExtensions.MexicoCityCurrentDateTime().ToShortTimeString()} o'clock , {DateExtensions.MexicoCityCurrentDateTime().ToShortDateString()}");
        }
        [HttpGet("TestDb")]
        public ActionResult<WResponse> TestDb()
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            if (SQLServerConnection.TestConnection(WebData.Connection) is Exception ex)
            {
                return new WResponse(APIResponseResult.INTERNAL_ERROR, ex.ToString());
            }
            sp.Stop();
            return new WResponse(APIResponseResult.OK, $"Time elapsed:{sp.Elapsed:G}");
        }

        [HttpGet("SignUp/{Boleta}/{Nombre}/{NickName}/{Email}/{SchoolId}/{Type}/{Device}")]
        public ActionResult<WResponse> SignUp(string Boleta, string Nombre,
            string NickName, string Email,
            int SchoolId, int Type, string Device)
        {
            try
            {
                if (!NickNameIsAvaible(NickName, Boleta))
                {
                    return new WResponse(APIResponseResult.INVALID_REQUEST,
                        $"El nickname '{NickName}' ya esta en uso por otro usuario.\nPor favor escoge uno diferente.");
                }
                return this.SignUp(Boleta, Nombre, NickName, Email, SchoolId, (UserType)Type, JsonConvert.DeserializeObject<Device>(Device));
            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new WResponse(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        private Response<int> SignUp(string Boleta, string Nombre, string NickName, string Email, int SchoolId, UserType Type, Device Device)
        {
            if (string.IsNullOrEmpty(Boleta)
                || !Validations.IsValidEmail(Email)
                || !Validations.IsValidBoleta(Boleta)
                || SchoolId <= 0
                || string.IsNullOrEmpty(Nombre)
                || string.IsNullOrEmpty(NickName)
                || string.IsNullOrEmpty(Device.DeviceKey)
                || Type == UserType.INVALID)
            {
                return new Response<int>(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            try
            {
                Response<int> response = ResponseExtensions.FromSql<int>("SP_SIGNUP"
                    , new SqlParameter("BOLETA", Boleta)
                    , new SqlParameter("NAME", Nombre)
                    , new SqlParameter("NICK_NAME", NickName)
                    , new SqlParameter("MAIL", Email)
                    , new SqlParameter("SCHOOL_ID", SchoolId)
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
                return new Response<int>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }

        }
        [HttpPost("PostClassTime/{User}")]
        public ActionResult<Response<string>> PostClassTime(string User, [FromBody] byte[] HTML)
        {
            this._logger.Log(LogLevel.Debug, "PostClassTime");
            return ClassTimeDigester.Digest(HTML, User, this._logger);
        }
        [HttpPost("PostGrades/{User}")]
        public ActionResult<WResponse> PostGrades(string User, [FromBody] byte[] HTML)
        {
            this._logger.Log(LogLevel.Debug, "PostGrades");
            return GradesDigester.Digest(HTML, User, this._logger);
        }
        [HttpGet("PostCareer/{CareerName}/{User}")]
        public ActionResult<Response<int>> PostCareer(string CareerName, string User)
        {
            this._logger.Log(LogLevel.Debug, "PostCareer");
            if (!Validations.IsValidBoleta(User) && !Validations.IsValidEmail(User))
            {
                return Response<int>.InvalidRequest;
            }
            try
            {
                return ResponseExtensions.FromSql<int>("SP_GET_ADD_CAREER"
                    , new SqlParameter("CAREER_NAME", CareerName)
                    , new SqlParameter("USER", User)
                );
            }
            catch (Exception ex)
            {
                this._logger.Log(LogLevel.Error, ex, ex?.Message);
                return new Response<int>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpPost("PostToDo/{User}")]
        public ActionResult<WResponse> PostToDo(string User, [FromBody] byte[] TodoBytes)
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
                    return WResponse.Error;
                }
                if (DateExtensions.MexicoCityCurrentDateTime() > Todo.Date.Add(Todo.Time))
                {
                    return new WResponse(APIResponseResult.INVALID_REQUEST,
                        "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla");
                }

                return ResponseExtensions.FromSql("SP_POST_TODO"
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
                return new WResponse(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpPost("PostTodoPicture/{ToDoGuid}")]
        public ActionResult<WResponse> PostTodoPicture(Guid ToDoGuid, [FromBody] byte[] Img)
        {
            if (Img is null || Img.Length <= 0 || Guid.Empty == ToDoGuid)
            {
                return WResponse.NotExecuted;
            }


            return ResponseExtensions.FromSql("SP_POST_TODO_PICTURE"
                , new SqlParameter("TODO_GUID", ToDoGuid)
                , new SqlParameter("IMG", Img));
        }


        [HttpGet("ShareTodo/{ToDoGuid}")]
        public ActionResult<Response<TodoBase>> ShareTodo(Guid ToDoGuid)
        {
            TodoBase todo = TodoService.Find(ToDoGuid, this._logger, out string UserNick);
            if (todo is null)
            {
                return Response<TodoBase>.Error;
            }
            return new Response<TodoBase>(
                APIResponseResult.OK,
                UserNick, todo);
        }
        [HttpGet("GetArchieveIds/{ArchieveGuid}")]
        public ActionResult<Response<int[]>> GetArchieveIds(Guid ArchieveGuid)
        {
            int[] ids = ArchieveService.GetIdsByGuid(ArchieveGuid, this._logger);
            return new Response<int[]>(
                APIResponseResult.OK,
                "Ok", ids);
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
        public ActionResult<WResponse> PostReminder(string User, [FromBody] byte[] TodoBytes)
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
                    return WResponse.Error;
                }
                if (DateExtensions.MexicoCityCurrentDateTime() > Reminder.Date)
                {
                    return new WResponse(APIResponseResult.INVALID_REQUEST,
                        "Este recordatorio ya ha expirado, cambie la fecha de entrega si desea compartirla");
                }

                return ResponseExtensions.FromSql("SP_POST_REMINDER"
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
                return new WResponse(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpGet("ShareReminder/{ToDoGuid}")]
        public ActionResult<Response<ReminderBase>> ShareReminder(Guid ToDoGuid)
        {
            ReminderBase reminder = ReminderService.Find(ToDoGuid, this._logger, out string NickName);

            if (reminder is null)
            {
                return Response<ReminderBase>.Error;
            }
            return new Response<ReminderBase>(
                APIResponseResult.OK,
                NickName, reminder);
        }
        [HttpGet("GetClassmates/{Group}/{TeacherId}/{SubjectId}")]
        public ActionResult<Response<IEnumerable<Classmate>>> GetClassmates(string Group, string TeacherId, string SubjectId)
        {
            List<Classmate> Classmates = new List<Classmate>();
            WebData.Connection.Read(
                "SP_GET_CLASSMATES",
                (reader) =>
                {
                    Classmates.Add(new Classmate(Convert.ToString(reader[0]), Convert.ToString(reader[1])));
                }
                , new CommandConfig() { CommandType = CommandType.StoredProcedure },
                new SqlParameter("GROUP", Group)
                , new SqlParameter("SUBJECT_ID", SubjectId)
                , new SqlParameter("TEACHER_ID", TeacherId));
            return new Response<IEnumerable<Classmate>>(
                APIResponseResult.OK,
                "Ok", Classmates.ToArray());
        }
        [HttpPost("PostLink/{SubjectId}/{IdTeacher}/{User}/{Group}")]
        public ActionResult<Response<Guid>> PostLink(
            string SubjectId, string IdTeacher, string User, string Group, [FromBody] byte[] JsonLinkBytes)
        {
            if (string.IsNullOrEmpty(Group)
            || string.IsNullOrEmpty(User) ||
            (!Validations.IsValidEmail(User)
             && !Validations.IsValidBoleta(User))

            )
            {
                return Response<Guid>.InvalidRequest;
            }
            Guid guid = Guid.Empty;
            try
            {
                string JsonLink = Encoding.UTF8.GetString(JsonLinkBytes);
                if (JsonConvert.DeserializeObject<Link>(JsonLink) is not Link Link)
                {
                    return Response<Guid>.InvalidRequest;
                }
                if (string.IsNullOrEmpty(Link.Url) || !UriExtensions.IsValidUrl(Link.Url, out Uri uri))
                {
                    return Response<Guid>.InvalidRequest;
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
            return new Response<Guid>(
               guid == Guid.Empty ?
                   APIResponseResult.INTERNAL_ERROR : APIResponseResult.OK,
               guid == Guid.Empty ? "Error" : "Ok",
               guid);
        }
        [HttpGet("GetLinks/{Group}/{TeacherId}/{SubjectId}/{UserId}")]
        public ActionResult<Response<IEnumerable<Link>>> GetLinks(string Group, string TeacherId, string SubjectId, int UserId)
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
            return new Response<IEnumerable<Link>>(
                APIResponseResult.OK,
                "Ok", Links.ToArray());
        }
        [HttpGet("ReportLink/{UserId}/{LinkId}/{ReportReason}")]
        public ActionResult<WResponse> ReportLink(int UserId, Guid LinkId, int ReportReason) =>
            this.ReportLink(UserId, LinkId, (ReportReason)ReportReason);
        private ActionResult<WResponse> ReportLink(int UserId, Guid LinkId, ReportReason ReportReason)
        {
            if (LinkId == Guid.Empty || UserId <= 0)
            {
                return WResponse.InvalidRequest;
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
            return new WResponse(APIResponseResult.OK, "Ok");
        }
        [HttpGet("DeleteLink/{UserId}/{LinkId}")]
        public ActionResult<WResponse> DeleteLink(int UserId, Guid LinkId)
        {
            if (LinkId == Guid.Empty || UserId <= 0)
            {
                return WResponse.InvalidRequest;
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
            return new WResponse(APIResponseResult.OK, "Ok");
        }
        ////---------------------------------------------------------------------------------------------------------------------
        [HttpPost("PostContact/{User}/{SchoolId}")]
        public ActionResult<Response<Guid>> PostContact(string User, int SchoolId, [FromBody] byte[] JsonContactBytes)
        {
            if (SchoolId <= 0
            || string.IsNullOrEmpty(User) ||
            (!Validations.IsValidEmail(User)
             && !Validations.IsValidBoleta(User))
            )
            {
                return Response<Guid>.InvalidRequest;
            }
            Guid guid = Guid.Empty;
            try
            {
                string JsonContact = Encoding.UTF8.GetString(JsonContactBytes);
                if (JsonConvert.DeserializeObject<SchoolContact>(JsonContact) is not SchoolContact Contact)
                {
                    return Response<Guid>.InvalidRequest;
                }
                if (!string.IsNullOrEmpty(Contact.Url))
                {
                    if (!UriExtensions.IsValidUrl(Contact.Url, out Uri uri))
                    {
                        return Response<Guid>.InvalidRequest;
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
                return new Response<Guid>(APIResponseResult.NOT_READ, "Contacto no leido", guid);
            }
            return new Response<Guid>(APIResponseResult.OK, "Ok", guid);
        }
        [HttpGet("GetContacts/{SchoolId}/{UserId}")]
        public ActionResult<Response<IEnumerable<ContactsByDeparment>>> GetContacts(int SchoolId, int UserId)
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
            return new Response<IEnumerable<ContactsByDeparment>>(APIResponseResult.OK, "Ok", Contacts.Values);
        }
        [HttpGet("ReportContact/{ContactId}/{ReportReason}/{UserId}")]
        public ActionResult<WResponse> ReportContact(Guid ContactId, int ReportReason, int UserId) =>
            this.ReportContact(ContactId, (ReportReason)ReportReason, UserId);
        private ActionResult<WResponse> ReportContact(Guid ContactId, ReportReason ReportReason, int UserId)
        {
            if (ContactId == Guid.Empty)
            {
                return WResponse.InvalidRequest;
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
            return new WResponse(APIResponseResult.OK, "Ok");
        }
        [HttpGet("DeleteContact/{UserId}/{ContactId}")]
        public ActionResult<WResponse> DeleteContact(int UserId, Guid ContactId)
        {
            if (ContactId == Guid.Empty || UserId <= 0)
            {
                return WResponse.InvalidRequest;
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
            return new WResponse(APIResponseResult.OK, "Ok");
        }

        [HttpGet("GetSchoolId/{UserId}")]
        public ActionResult<Response<int>> GetSchoolId(int UserId)
        {
            int SchoolId = -1;
            if (UserId <= 0)
            {
                return Response<int>.InvalidRequest;
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
            return new Response<int>(APIResponseResult.OK, "Ok", SchoolId);
        }

        [HttpGet("IsNickNameAvaible/{nickname}")]
        public ActionResult<WResponse> IsNickNameAvaible(string nickname)
        {
            if (!Validations.IsValidNickName(nickname))
            {
                return WResponse.InvalidRequest;
            }
            bool avaible = NickNameIsAvaible(nickname);
            return new WResponse(avaible ? APIResponseResult.YES : APIResponseResult.NO, "Ok");
        }

        private bool NickNameIsAvaible(string nickname, string boleta = null)
        {
            if (!Validations.IsValidNickName(nickname))
            {
                return true;
            }
            bool result = false;
            try
            {
                using (var con = WebData.Connection)
                {
                    result = WebData.Connection.Single<bool>(
                        "SP_IsNickNameAvaible",
                        CommandType.StoredProcedure
                        , new SqlParameter("NICK_NAME", nickname)
                        , new SqlParameter("BOLETA", (object)boleta ?? DBNull.Value));
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "IsNickNameAvaible");
            }
            return result;
        }
        [HttpGet("BoletaIsRegistered/{boleta}/{schoolId}")]
        public ActionResult<WResponse> BoletaIsRegistered(string boleta, int schoolId)
        {
            bool registered = false;
            if (!Validations.IsValidBoleta(boleta) || schoolId <= 0)
            {
                return WResponse.InvalidRequest;
            }

            try
            {
                registered = WebData.Connection.Single<bool>(
                    "SP_BoletaIsRegistered",
                    CommandType.StoredProcedure
                    , new SqlParameter("BOLETA", boleta)
                    , new SqlParameter("SCHOOL_ID", schoolId));
                WebData.Connection.Close();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "IsNickNameAvaible");
            }

            return new WResponse(registered ? APIResponseResult.YES : APIResponseResult.NO, "Ok");
        }
        [HttpGet("PostSubject/{UserId}/{Group}/{Suffix}/{TeacherId}/{SubjectName}")]
        public ActionResult<Response<Subject>> PostSubject(int UserId, string Group, string Suffix, int TeacherId, string SubjectName)
        {
            if (UserId <= 0)
            {
                return Response<Subject>.InvalidRequest;
            }
            try
            {
                Subject subject = ClassTimeDigester.PostSubject(UserId, Group, Suffix, TeacherId, SubjectName);
                return new Response<Subject>(APIResponseResult.OK, "OK", subject);
            }
            catch (Exception ex)
            {
                return new Response<Subject>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpGet("PostTeacher/{TeacherName}")]
        public ActionResult<Response<Teacher>> PostTeacher(string TeacherName)
        {
            if (string.IsNullOrEmpty(TeacherName))
            {
                return Response<Teacher>.InvalidRequest;
            }
            try
            {
                Teacher teacher = ClassTimeDigester.PostTeacher(TeacherName);
                return new Response<Teacher>(APIResponseResult.OK, "OK", teacher);
            }
            catch (Exception ex)
            {
                return new Response<Teacher>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }

        }
        [HttpGet("PostClassTime/{TeacherId}/{SubjectId},{Day},{Begin},{End}")]
        public ActionResult<Response<ClassTime>> PostClassTime(int TeacherId, int SubjectId, int Day, TimeSpan Begin, TimeSpan End)
        {
            DayOfWeek EnumDay = (DayOfWeek)(Day);
            try
            {
                ClassTime classTime = ClassTimeDigester.PostClassTimeFrom(TeacherId, SubjectId, EnumDay, Begin, End);
                return new Response<ClassTime>(APIResponseResult.OK, "OK", classTime);
            }
            catch (Exception ex)
            {
                return new Response<ClassTime>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        [HttpGet("PostGrade/{partial}/{text_score},{numeric_score},{group},{User}")]
        public ActionResult<Response<Grade>> PostGrade(int partial, string text_score, int numeric_score, string group, string User)
        {
            GradePartial EnumPartial = (GradePartial)(partial);
            try
            {
                Grade grade = GradesDigester.PostGrade(EnumPartial, text_score, numeric_score, group, User);
                return new Response<Grade>(APIResponseResult.OK, "OK", grade);
            }
            catch (Exception ex)
            {
                return new Response<Grade>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }

    }
}
