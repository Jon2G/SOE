using Microsoft.AspNetCore.Components;
using SOEAWS.Models;
using SOEAWS.Services;
using SOEWeb.Shared;

namespace SOEAWS.Components
{
    public partial class CommentActions : IStateHasChanged
    {
        [Parameter]
        public SubjectComment Comment { get; set; }
        [Inject]
        public ICommentsService CommentsService { get; set; }
        [Parameter]
        public CommentsData Data { get; set; }
        [Parameter]
        public bool IsSubComment { get; set; }

        public CommentActions()
        {

        }
        public void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }

    }
}