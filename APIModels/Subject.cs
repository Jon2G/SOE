using Kit.Services.Web;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Kit.Sql.Sqlite;
using Newtonsoft.Json;
using SOEWeb.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace SOEWeb.Shared
{
    [Preserve]
    public class Subject : OfflineSync, IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        private ThemeColor _ThemeColor;
        [Ignore, JsonIgnore]

        public ThemeColor ThemeColor
        {
            get => _ThemeColor;
            set
            {
                _ThemeColor = value;
                Raise(() => ThemeColor);
            }
        }

        public string Color
        {
            get => ThemeColor?.Light;
            set
            {
                if (ThemeColor is null)
                {
                    ThemeColor = new ThemeColor() { Light = value };
                }
                else
                {
                    ThemeColor.Light = value;
                }
            }
        }
        public string ColorDark
        {
            get => ThemeColor?.Dark;
            set
            {
                if (ThemeColor is null)
                {
                    ThemeColor = new ThemeColor() { Dark = value };
                }
                else
                {
                    ThemeColor.Dark = value;
                }
            }
        }


        public string Group { get; set; }
        public int GroupId { get; set; }
        public int IdTeacher { get; set; }
        public Guid Guid { get; set; }
        public Subject()
        {

        }
        public Subject(int Id, int IdTeacher, string Name, ThemeColor color, string Group)
        {
            this.Id = Id;
            this.IdTeacher = IdTeacher;
            this.Name = Name;
            this.ThemeColor = color;
            this.Group = Group;
        }
        public override async Task<bool> Sync(IApplicationData app, ISyncService apiService)
        {
            if (!await CheckUser(app, apiService))
            {
                return false;
            }
            Teacher teacher = app.LiteConnection.Find<Teacher>(this.IdTeacher);
            if (teacher is null)
            {
                return false;
            }
            if (teacher.IsOffline)
            {
                if (!await teacher.Sync(app, apiService))
                {
                    return false;
                }
                this.IdTeacher = teacher.Id;
                return await Sync(app, apiService);
            }
            Response<Subject> response = await apiService.Sync(this);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Subject subject = response.Extra;
                app.LiteConnection.EXEC($"UPDATE SUBJECT SET Id='{subject.Id}',Color='{subject.Color}',ColorDark='{subject.ColorDark}',GroupId='{subject.GroupId}',IdTeacher='{subject.IdTeacher}',Guid='{subject.Guid}',IsOffline='0' where Id={this.Id}");
                app.LiteConnection.EXEC($"UPDATE ClassTime SET IdSubject='{subject.Id}',IsOffline='0' where IdSubject={this.Id}");
                app.LiteConnection.EXEC($"UPDATE Grades SET SubjectId='{subject.Id}',IsOffline='0' where SubjectId={this.Id}");
                app.LiteConnection.EXEC($"UPDATE Reminder SET SubjectId='{subject.Id}' where SubjectId={this.Id}");
                app.LiteConnection.EXEC($"UPDATE ToDo SET SubjectId='{subject.Id}' where SubjectId={this.Id}");
                this.Id = subject.Id;
                this.IsOffline = false;
                this.ThemeColor = subject.ThemeColor;
                return true;
            }
            return false;
        }

    }
}
