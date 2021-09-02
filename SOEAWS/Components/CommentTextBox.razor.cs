using Microsoft.AspNetCore.Components;
using SOEAWS.Models;
using SOEAWS.Services;
using SOEWeb.Shared;

namespace SOEAWS.Components
{
    public partial class CommentTextBox : IStateHasChanged
    {
        const int MAX_TEXT_COUNT = 280;
        public string CommentText { get;  set; }
  
        public bool IsPostingComment { get; set; }
        [Inject]
        public ICommentsService CommentsService { get; set; }
        [Parameter]
        public CommentsData Data { get; set; }
        [Parameter]
        public SubjectComment ParentComment { get; set; }

        public CommentTextBox()
        {
            CommentText = string.Empty;
        }
        private void CommentTextChanged(ChangeEventArgs obj)
        {
            if (obj.Value is string s)
            {
                this.CommentText = s;
                this.InvokeStateHasChanged();
            }
        }
        public void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }

    }
}