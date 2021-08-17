using System;
using System.Collections.Generic;
using SOEWeb.Shared;
using SOE.Data;

namespace SOE.Services
{
    internal static class SubjectService
    {
        internal static List<Subject> ToList()
        {
            return AppData.Instance.LiteConnection.Table<Subject>().OrderBy(x => x.Group).ToList();
        }
        internal static Subject Get(int Id)
        {
            return AppData.Instance.LiteConnection.Find<Subject>(Id);
        }

        internal static void Save(Subject subject)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(subject);
        }
    }
}
