using AsyncAwaitBestPractices;
using System;
using System.IO;
using System.Threading.Tasks;
using Kit.Forms.Extensions;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;
using Log = Kit.Log;
using Kit;

namespace SOE.Data.Images
{
    [Table("Archive")]
    public class PhotoArchive : Archive<FileImageSource>
    {
        private FileImageSource _Value;
        [Ignore]
        public override FileImageSource Value
        {
            get => this._Value;
            set
            {
                this._Value = value;
                Raise(() => Value);
            }
        }

        public PhotoArchive(string PhotoPath, FileType FileType, bool Compress = true) : base(new FileResult(PhotoPath), FileType)
        {
            if (Compress)
            {
                CompressAsync((FileImageSource)FileImageSource.FromFile(PhotoPath)).SafeFireAndForget();
            }
            else
            {
                Value = (FileImageSource)FileImageSource.FromFile(PhotoPath);
            }

        }

        private async Task CompressAsync(FileImageSource cached)
        {
            await Task.Yield();
            Value = await cached.CompressImage(80);
            this.Path = Value.File;
        }

        public PhotoArchive(Archive archive, FileImageSource Value) : base(archive, Value)
        {

        }
    }
    [Table("Archive")]
    public class Archive<T> : Archive
    {
        [Ignore]
        public virtual T Value { get; set; }

        public Archive(FileResult FileResult, FileType FileType) : base(FileResult, FileType)
        {

        }

        public Archive(Archive archive, T Value) : base(archive)
        {
            this.Value = Value;
        }
    }
    [Table("Archive")]
    public class Archive : ModelBase, IGuid
    {
        public Guid Guid { get; set; }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int IdKeeper { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public FileType FileType { get; set; }

        public Archive(Archive archive)
        {
            Guid = archive.Guid;
            Id = archive.Id;
            IdKeeper = archive.IdKeeper;
            Path = archive.Path;
            Extension = archive.Extension;
            FileType = archive.FileType;
        }
        public Archive(FileResult FileResult, FileType FileType)
        {
            FileInfo file = new FileInfo(FileResult.FullPath);
            this.Extension = file.Extension;
            this.Path = file.FullName;
            this.FileType = FileType;
        }
        public Archive()
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
                Log.Logger.Error(ex, "Al eliminar un archivo de keeper");
                Acr.UserDialogs.UserDialogs.Instance.Alert("No se pudo eliminar este archivo", "Alerta");
            }
        }

        public async Task<Stream> GetStream()
        {
            MemoryStream stream = new MemoryStream();
            using (FileStream SourceStream =new FileStream(this.Path,FileMode.Open,FileAccess.Read))
            {
                await SourceStream.CopyToAsync(stream);
            }
            return stream;

        }
    }
}
