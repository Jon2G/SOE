using FirestoreLINQ;

using Kit.Model;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
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
    [FireStoreCollection("Archive")]
    public abstract class Archive : ModelBase
    {
        [Id]
        public string DocumentId { get; set; }

        public string ParentId { get; set; }

        public string Path { get; set; }

        public string Extension { get; set; }

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
                //AppData.Instance.LiteDatabase.Delete(this);
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
        public static ICollectionReference Collection =>
            FireBaseConnection.UserDocument.Collection<Archive>();
        public static async IAsyncEnumerable<T> IQuery<T>(IQuery IQuery) where T : Archive, new()
        {
            IQuerySnapshot capitalQuerySnapshot = await IQuery.GetAsync();
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<T>();
            }
        }
    }
}
