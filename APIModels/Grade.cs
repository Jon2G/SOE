using Kit.Daemon.Sync;
using Kit.Services.Web;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Kit.Sql.Sqlite;
using SOEWeb.Shared.Enums;
using SOEWeb.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace SOEWeb.Shared
{

    [Table("Grades")]
    public class Grade : OfflineSync, IGuid
    {
        public const int Undefined = -1;
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public GradePartial Partial { get; set; }
        public string TextScore { get; set; }
        public int NumericScore { get; set; }
        public int SubjectId { get; set; }
        public Guid Guid { get; set; }

        public Grade() { }
        public Grade(GradePartial Partial, string TextScore, int NumericScore, int SubjectId)
        {
            this.TextScore = TextScore;
            this.NumericScore = NumericScore;
            this.Partial = Partial;
            this.SubjectId = SubjectId;
        }
        public override async Task<bool> Sync(IApplicationData app, ISyncService apiService)
        {
            await Task.Yield();
            if (!await CheckUser(app, apiService))
            {
                return false;
            }
            Subject subject = app.LiteConnection.Find<Subject>(x => x.Id == this.SubjectId);
            if (subject.IsOffline)
            {
                if (!await subject.Sync(app, apiService))
                {
                    return false;
                }
            }
            Response<Grade> response =await apiService.Sync(this);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                Grade grade = response.Extra;
                app.LiteConnection.EXEC($"update grades set IsOffline='0',Id='{grade.Id}',SyncGuid='{grade.Guid}' where Id='{this.Id}'");
                return true;
            }
            return false;
        }

    }
}
