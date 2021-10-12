using Kit.Services.Web;
using Kit.Sql.Attributes;
using Kit.Sql.Sqlite;
using SOEWeb.Shared.Interfaces;
using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SOEWeb.Shared
{
    [Preserve]
    public class ClassTime : OfflineSync
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int IdSubject { get; set; }
        public string Group { get; set; }
        public DayOfWeek Day { get; set; }

        [XmlIgnore]
        public TimeSpan Begin { get; set; }
        [Ignore]
        public long BeginTicks
        {
            get => this.Begin.Ticks;
            set => this.Begin = TimeSpan.FromTicks(value);
        }
        [XmlIgnore]
        public TimeSpan End { get; set; }
        [Ignore]
        public long EndTicks
        {
            get => this.End.Ticks;
            set => this.End = TimeSpan.FromTicks(value);
        }
        public ClassTime()
        {

        }

        public ClassTime(int Id, string Group, int IdSubject, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            this.Id = Id;
            this.Group = Group;
            this.Day = Day;
            this.IdSubject = IdSubject;
            this.Begin = Begin;
            this.End = End;
        }

        public override async Task<bool> Sync(IApplicationData app, ISyncService apiService)
        {
            await Task.Yield();
            if (!await CheckUser(app, apiService))
            {
                return false;
            }
            Subject subject = app.LiteConnection.Find<Subject>(this.IdSubject);
            if (subject.IsOffline)
            {
                if (!await subject.Sync(app, apiService))
                {
                    return false;
                }
            }
            Response<ClassTime> response = await apiService.Sync(this);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                ClassTime classTime = response.Extra;
                app.LiteConnection.EXEC($"UPDATE CLASSTIME SET ID='{classTime.Id}',IsOffline='0' where Id='{this.Id}'");
                app.LiteConnection.Update(this, x => x.Id == this.Id, false);
                return true;
            }
            return false;
        }


    }
}
