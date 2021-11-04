﻿using SOEWeb.Shared;
using SOE.Data;

namespace SOE.Services
{
    internal class TeacherService
    {
        internal static void Save(Teacher teacher)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(teacher);
        }

        internal static Teacher Get(int idTeacher) =>
            AppData.Instance.LiteConnection.Table<Teacher>().FirstOrDefault(x => x.Id == idTeacher);
    }
}
