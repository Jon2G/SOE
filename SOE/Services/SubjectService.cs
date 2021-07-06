using System;
using System.Collections.Generic;
using APIModels;
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
        internal static int GetId(string group)
        {
            return GetByGroup(group)?.Id?? -1;
        }

        internal static Subject FreeHour()
        {
            return new Subject(-1, -1, "Hora libre", Xamarin.Forms.Color.Gainsboro.ToHex(), Xamarin.Forms.Color.Gainsboro.ToHex(), String.Empty);
        }

        internal static void Save(Subject subject)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(subject);
        }

        internal static Subject GetByGroup(string group) 
            => AppData.Instance.LiteConnection.Table<Subject>().FirstOrDefault();
    }
}
