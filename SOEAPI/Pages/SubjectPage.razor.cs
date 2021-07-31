using Kit.Sql.Helpers;
using Kit.Sql.Readers;
using Microsoft.AspNetCore.Components;
using SOEWeb.Server.Components;
using SOEWeb.Server.Services;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOEWeb.Server.Pages
{
    public partial class SubjectPage
    {
        #region Atributtes
        [Parameter]
        public string UserId { get; set; }
        [Parameter]
        public string SubjectId { get; set; }
        #endregion
        public Subject Subject { get; set; }

        const int MAX_TEXT_COUNT = 280;
        private string CommentText = string.Empty;


        public InfiniteScrolling<SubjectComment> MainInfiniteScrolling { get; set; }


        public SubjectPage()
        {

        }
        private void CommentTextChanged(ChangeEventArgs obj)
        {
            if (obj.Value is string s)
            {
                this.CommentText = s;
                StateHasChanged();
            }
        }

        private async Task PostComment(SubjectComment parent)
        {
            await PostComment(UserId, this.CommentText, ParentComment: parent);
            this.CommentText = String.Empty;
        }

        private async Task PostComment()
        {
            await PostComment(null);
        }

        private void Rate(SubjectComment value, bool? Vote)
        {
            WebData.Connection.EXEC("SP_RATE_SUBJECT_COMMENTS", CommandType.StoredProcedure
                , new SqlParameter("ID", value.Id)
                , new SqlParameter("POSITIVE", (object)Vote ?? DBNull.Value)
                , new SqlParameter("USER_ID", UserId)
            );

            if (Vote is null)
            {
                if (value.Vote is true)
                {
                    value.Votes--;
                }
                else
                {
                    value.Votes++;
                }
            }
            else
            {
                if (value.Vote is null)
                {
                    value.Votes += Vote is true ? 1 : -1;
                }
                else
                {
                    value.Votes += Vote is true ? 2 : -2;
                }
            }
            value.Vote = Vote;
        }
        private void ShowCommentsBox(SubjectComment value)
        {
            value.CommentBox = !value.CommentBox;
        }
        private async Task ShowSubComments(SubjectComment value)
        {
            value.OpenSubComments = !value.OpenSubComments;
            if (value.OpenSubComments)
            {
                await GetComments(value,
                    new InfiniteScrollingItemsProviderRequest(0, CancellationToken.None));
                value.CommentBox = true;
            }
            else
            {
                value.CommentBox = false;
            }
        }


        internal void InvokeStateHasChanged()
        {
            this.StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!int.TryParse(this.SubjectId, out int SubjectId))
            {
                throw new ArgumentException("Id de materia invalido");
            }
            if (!int.TryParse(this.UserId, out _))
            {
                throw new ArgumentException("Usuario invalido");
            }
            this.Subject = SubjectService.GetById(SubjectId);
            this.StateHasChanged();
        }
        #region CommentsService
        public bool IsPostingComment { get; set; }

        internal async Task<IEnumerable<SubjectComment>> GetComments(
            SubjectComment ParentComment,
            InfiniteScrollingItemsProviderRequest request)
        {
            await Task.Yield();
            List<SubjectComment> Comments = new List<SubjectComment>();
            try
            {
                using (var reader = WebData.Connection.Read("SP_GET_SUBJECT_COMMENTS", CommandType.StoredProcedure
                    , new SqlParameter("SUBJECT_ID", this.SubjectId)
                    , new SqlParameter("OFFSET", request.StartIndex)
                    , new SqlParameter("PARENT_COMMENT_ID", (object)ParentComment?.Id ?? DBNull.Value)
                    , new SqlParameter("USER_ID", UserId)
                ))
                {
                    while (reader.Read())
                    {
                        Comments.Add(new SubjectComment()
                        {
                            Id = (Guid)reader[0],
                            Text = Convert.ToString(reader[1]),
                            UserId = (Guid)reader[2],
                            UserName = Convert.ToString(reader[3]),
                            HasSubComments = Convert.ToBoolean(reader[4]),
                            Votes = Convert.ToInt32(reader[7]),
                            Vote = SQLHelper.ToBool(reader[8],null)
                        });
                    }
                }
                Comments.TrimExcess();
                if (Comments.Any())
                {
                    if (ParentComment is null)
                    {
                        foreach (SubjectComment comment in Comments)
                        {
                            var sub_comments =
                                await this.GetComments(comment, InfiniteScrollingItemsProviderRequest.Zero);
                            if (sub_comments?.Any() ?? false)
                                comment.AddRange(sub_comments);
                        }
                        //this.HasNoCommentsLeft = false;
                    }
                }
                else if (ParentComment is null)
                {
                    InvokeStateHasChanged();
                }

            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
            }
            return Comments;
        }
        internal Task<IEnumerable<SubjectComment>> GetComments(InfiniteScrollingItemsProviderRequest request)
            => this.GetComments(null, request);

        internal async Task PostComment(string UserId, string CommentText, SubjectComment ParentComment)
        {
            await Task.Yield();
            try
            {
                SubjectComment comment = new SubjectComment();
                this.IsPostingComment = true;
                using (IReader reader = WebData.Connection.Read("SP_ADD_SUBJECT_COMMENTS", CommandType.StoredProcedure
                    , new SqlParameter("SUBJECT_ID", this.SubjectId)
                    , new SqlParameter("TEXT", CommentText)
                    , new SqlParameter("USER_ID", UserId)
                    , new SqlParameter("PARENT_COMMENT_ID", (object)ParentComment?.Id ?? DBNull.Value)))
                {
                    if (reader.Read())
                    {
                        comment = new SubjectComment()
                        {
                            Id = (Guid)reader[0],
                            UserId = (Guid)reader[1],
                            HasSubComments = Convert.ToBoolean(reader[2]),
                            Text = Convert.ToString(reader[3]),
                            UserName = Convert.ToString(reader[4]),
                            Votes = Convert.ToInt32(reader[5])
                        };
                    }
                }


                if (ParentComment is not null)
                {
                    ParentComment.Clear();
                    var sub_comments = await GetComments(ParentComment,
                        new InfiniteScrollingItemsProviderRequest(0, CancellationToken.None));
                    if (sub_comments.Any())
                    {
                        ParentComment.HasSubComments = true;
                        ParentComment.OpenSubComments = true;
                        ParentComment.AddRange(sub_comments);
                        InvokeStateHasChanged();
                    }
                }
                else
                {
                    this.MainInfiniteScrolling.Insert(0, comment);
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
            }
            finally
            {
                this.IsPostingComment = false;
            }
        }
        #endregion
    }
}
