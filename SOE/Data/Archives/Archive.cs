using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit.Model;
using Microsoft.AppCenter.Crashes;
using SOE.API;
using SOE.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Log = Kit.Log;

namespace SOE.Data.Archives
{
    [FirestoreData]
    public abstract class Archive<T> : Archive
    {
        public virtual T Value { get; set; }
        protected Archive(FileResult fileResult, FileType fileType) : base(fileResult, fileType)
        {

        }
        protected Archive(Archive archive, T value) : base(archive)
        {
            this.Value = value;
        }

        protected Archive() : base()
        {

        }
    }
    [FireStoreCollection("Archive"), FirestoreData]
    public abstract class Archive : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public string ParentId { get; set; }
        [FirestoreProperty]
        public string Path { get; set; }
        [FirestoreProperty]
        public string Extension { get; set; }
        [FirestoreProperty]
        public FileType FileType { get; set; }

        protected Archive(Archive archive)
        {
            this.Path = archive.Path;
            this.Extension = archive.Extension;
            this.FileType = archive.FileType;
        }
        protected Archive(FileResult FileResult, FileType FileType)
        {
            FileInfo file = new FileInfo(FileResult.FullPath);
            this.Extension = file.Extension;
            this.Path = file.FullName;
            this.FileType = FileType;
        }
        protected Archive()
        {

        }

        internal void Delete()
        {
            try
            {
                FileInfo file = new FileInfo(this.Path);
                if (file.Exists)
                {
                    file.Delete();
                }
                AppData.Instance.LiteConnection.Delete(this);
            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "Al eliminar un archivo de keeper");
                Acr.UserDialogs.UserDialogs.Instance.Alert("No se pudo eliminar este archivo", "Alerta");
            }
        }
        public static async Task<Stream> GetStream(string path)
        {
            MemoryStream stream = new MemoryStream();
            using (FileStream SourceStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                await SourceStream.CopyToAsync(stream);
            }
            return stream;

        }
        public static CollectionReference Collection =>
            FireBaseConnection.Instance.UserDocument.Collection<Archive>();
        public static async IAsyncEnumerable<T> Query<T>(Query query) where T : Archive, new()
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<T>();
            }
        }
    }
}
