using APIModels;
using System;
using System.Collections.Generic;
using System.Text;
using SOE.Data;

namespace SOE.Services
{
    internal class TeacherService
    {
        internal static void Save(Teacher teacher)
        {
            AppData.Instance.LiteConnection.InsertOrReplace(teacher);
        }
    }
}
