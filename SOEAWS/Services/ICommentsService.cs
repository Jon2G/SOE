using Kit.Razor.Data;
using SOEAWS.Components;
using SOEAWS.Models;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOEAWS.Services
{
    public interface ICommentsService
    {
        public List<SubjectComment> Comments { get;  }
        public void Clear();
        public Task GetCommentsAsync(CommentsData data, bool Step = true);
        public Task GetCommentsAsync(CommentsData data, RequestRange range);
        public Task GetCommentsAsync(CommentsData data, RequestRange range, CancellationTokenSource token, SubjectComment ParentComment = null);
        public Task PostComment(CommentsData data, CommentTextBox textBox, SubjectComment ParentComment=null);
        public Task ShowSubComments(CommentsData data, SubjectComment value);
        public void Rate(CommentsData data, SubjectComment value, bool? Vote);
        public void ShowCommentsBox(CommentsData data, SubjectComment value);
    }
}
