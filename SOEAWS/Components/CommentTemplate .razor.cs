using Microsoft.AspNetCore.Components;
using SOEAWS.Models;
using SOEAWS.Services;
using SOEWeb.Shared;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SOEAWS.Components
{
    public partial class CommentTemplate
    {
        [Inject]
        public ICommentsService CommentsService { get; set; }
        [Parameter] 
        public SubjectComment Comment { get; set; }
        [Parameter]
        public CommentsData Data { get; set; }
        [Parameter]
        public bool IsSubComment { get; set; }
        public CommentTemplate()
        {
            Comment = new SubjectComment();
        }

    }
}