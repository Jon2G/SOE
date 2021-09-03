using Kit;
using Kit.Razor.Data;
using Kit.Sql.Helpers;
using Kit.Sql.Readers;
using SOEAWS.Components;
using SOEAWS.Models;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOEAWS.Services
{
    public class CommentsService : ICommentsService
    {
        public const int STEP = 25;
        private RequestRange Range { get; set; }
        public List<SubjectComment> Comments { get; }
        public CommentsService()
        {
            Comments = new List<SubjectComment>();

        }

        public void Clear()
        {
            Comments.Clear();
            Range = null;
        }
        public async Task GetCommentsAsync(CommentsData data, bool Step = true)
        {
            await Task.Yield();
            if (Range is not null)
            {
                if (!Range.CanContinue)
                {
                    return;
                }
                if (Step)
                {
                    Range.NextStep();
                }
            }
            else
            {
                Range = new RequestRange(STEP);
            }

            await GetCommentsAsync(data, Range, Range.RenewCancellationTokenToken(), null);
            data.IStateHasChanged.InvokeStateHasChanged();
        }
        public Task GetCommentsAsync(CommentsData data, RequestRange range)
            => GetCommentsAsync(data, range, range.RenewCancellationTokenToken());
        public async Task GetCommentsAsync(CommentsData data, RequestRange range,
            CancellationTokenSource token, SubjectComment ParentComment = null)
        {
            await Task.Yield();
            int readRecords = 0;
            IList<SubjectComment> list = ParentComment ?? Comments;
            try
            {
                WebData.Connection.Read("SP_GET_SUBJECT_COMMENTS",
                    (reader) =>
                {
                    list.Add(new SubjectComment()
                    {
                        Id = (Guid)reader[0],
                        Text = Convert.ToString(reader[1]),
                        UserId = (Guid)reader[2],
                        UserName = Convert.ToString(reader[3]),
                        HasSubComments = Convert.ToBoolean(reader[4]),
                        Votes = Convert.ToInt32(reader[7]),
                        Vote = SQLHelper.ToBool(reader[8], null),
                        TeacherName = Convert.ToString(reader[9]),
                        GroupName = Convert.ToString(reader[10])
                    });
                    readRecords++;
                }
                    , CommandType.StoredProcedure
                    , new SqlParameter("SUBJECT_ID", data.SubjectId)
                    , new SqlParameter("OFFSET", range.From)
                    , new SqlParameter("PARENT_COMMENT_ID", (object)ParentComment?.Id ?? DBNull.Value)
                    , new SqlParameter("USER_ID", data.UserId)
                );
                if (readRecords < range.Step)
                {
                    range.NoMoreRecords();
                }
                Comments.TrimExcess();
                if (Comments.Any())
                {
                    if (ParentComment is not null)
                    {
                        ParentComment.HasSubComments = ParentComment.Any();
                    }
                }
                else if (ParentComment is null)
                {
                    data.IStateHasChanged.InvokeStateHasChanged();
                }
            }
            catch (OperationCanceledException)
            {
                Log.Logger.Information($"Operation cancelled readrecords:{readRecords}");
            }
            catch (Exception ex)
            {
                Log.Logger.Information(ex, "readrecords:{readRecords},{0}");
            }
        }
        public async Task PostComment(CommentsData data, CommentTextBox textBox, SubjectComment ParentComment = null)
        {
            await Task.Yield();
            try
            {
                SubjectComment comment = new SubjectComment();
                textBox.IsPostingComment = true;
                WebData.Connection.Read("SP_ADD_SUBJECT_COMMENTS",
                    (reader) =>
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
                    , CommandType.StoredProcedure
                    , new SqlParameter("SUBJECT_ID", data.SubjectId)
                    , new SqlParameter("TEXT", textBox.CommentText)
                    , new SqlParameter("USER_ID", data.UserId)
                    , new SqlParameter("GROUP_ID", data.GroupId)
                    , new SqlParameter("TEACHER_ID", data.TeacherId)
                    , new SqlParameter("PARENT_COMMENT_ID", (object)ParentComment?.Id ?? DBNull.Value));
                if (ParentComment is not null)
                {
                    ParentComment.Clear();
                    ParentComment.Range = new RequestRange(STEP);
                    await GetCommentsAsync(data, ParentComment.Range, ParentComment.Range.RenewCancellationTokenToken(), ParentComment);
                    if (ParentComment.Any())
                    {
                        ParentComment.OpenSubComments = true;
                        data.IStateHasChanged.InvokeStateHasChanged();
                    }
                }
                else
                {
                    this.Comments.Insert(0, comment);
                    data.IStateHasChanged.InvokeStateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
            }
            finally
            {
                textBox.CommentText = string.Empty;
                textBox.IsPostingComment = false;
            }
        }
        public void ShowCommentsBox(CommentsData data, SubjectComment value)
        {
            value.CommentBox = !value.CommentBox;
            data.IStateHasChanged.InvokeStateHasChanged();
        }
        public async Task ShowSubComments(CommentsData data, SubjectComment value)
        {
            value.OpenSubComments = !value.OpenSubComments;
            if (value.OpenSubComments)
            {
                value.Range = new RequestRange(STEP);
                await this.GetCommentsAsync(data, value.Range, value.Range.RenewCancellationTokenToken(), value);
                value.CommentBox = true;
            }
            else
            {
                value.CommentBox = false;
                value.Clear();
            }
        }
        public void Rate(CommentsData data, SubjectComment value, bool? Vote)
        {
            WebData.Connection.Execute("SP_RATE_SUBJECT_COMMENTS", CommandType.StoredProcedure
                , new SqlParameter("ID", value.Id)
                , new SqlParameter("POSITIVE", (object)Vote ?? DBNull.Value)
                , new SqlParameter("USER_ID", data.UserId)
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
    }
}
