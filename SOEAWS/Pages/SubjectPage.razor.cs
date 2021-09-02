using Kit.Sql.Helpers;
using Kit.Sql.Readers;
using Microsoft.AspNetCore.Components;
using SOEAWS.Services;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kit.Razor.Components;
using SOEAWS.Models;

namespace SOEAWS.Pages
{
    public partial class SubjectPage : IStateHasChanged
    {
        [Inject]
        public ICommentsService CommentsService { get; set; }
        public CommentsData Data { get; private set; }
        #region Atributtes
        [Parameter]
        public string UserId { get; set; }
        [Parameter]
        public string TeacherId { get; set; }
        [Parameter]
        public string GroupId { get; set; }
        [Parameter]
        public string SubjectId { get; set; }
        #endregion


        public SubjectPage()
        {

        }

        protected override void OnInitialized()
        {
            CommentsService.Clear();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!int.TryParse(this.SubjectId, out int SubjectId))
            {
                throw new ArgumentException("Id de materia invalido");
            }

            if (!int.TryParse(this.TeacherId, out int TeacherId))
            {
                throw new ArgumentException("Maestro invalido");
            }
            if (!int.TryParse(this.GroupId, out int GroupId))
            {
                throw new ArgumentException("Grupo invalido");
            }

            if (!int.TryParse(this.UserId, out int UserId))
            {
                throw new ArgumentException("Usuario invalido");
            }

            Data = new CommentsData(UserId, TeacherId, GroupId, SubjectId, this);
            this.InvokeStateHasChanged();
        }
        public void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }
    }
}
