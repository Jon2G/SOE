using System;
using System.Collections.Generic;
using SOEWeb.Shared;
using SOE.Data;
using SOEWeb.Shared.Interfaces;
using System.Linq.Expressions;

namespace SOE.Services
{
    public static class SubjectService
    {

        public static List<Subject> ToList(Expression<Func<Subject,bool>> where)
        {
            return AppData.Instance.LiteConnection.Table<Subject>().Where(where).OrderBy(x => x.Group).ToList();
        }
        public static List<Subject> ToList()
        {
            return AppData.Instance.LiteConnection.Table<Subject>().OrderBy(x => x.Group).ToList();
        }
        public static Subject Get(int Id)
        {
            return AppData.Instance.LiteConnection.Find<Subject>(Id);
        }

        public static void Save(Subject subject)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(subject);
        }
    }
}
