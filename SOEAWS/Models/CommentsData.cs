﻿using SOEAWS.Services;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SOEAWS.Models
{
    public struct CommentsData
    {
        public readonly int UserId;
        public readonly Teacher Teacher;
        public readonly int GroupId;
        public readonly int SubjectId;
        public readonly string NickName;
        public readonly Subject Subject;
        public readonly IStateHasChanged IStateHasChanged;
        public CommentsData(int UserId, int TeacherId,
            int GroupId, int SubjectId, IStateHasChanged IStateHasChanged)
        {
            this.UserId = UserId;
            this.GroupId = GroupId;
            this.SubjectId = SubjectId;
            this.IStateHasChanged = IStateHasChanged;
            using (var con = WebData.Connection)
            {
                this.NickName = UserBase.GetNickName(UserId, con);
            }
            this.SubjectId = SubjectId;
            this.Subject = SubjectService.GetById(SubjectId);
            this.Teacher = TeacherService.GetById(TeacherId);
        }
    }
}
