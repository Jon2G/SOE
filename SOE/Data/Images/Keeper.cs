using System;
using System.IO;
using System.Threading.Tasks;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Log = Kit.Log;

namespace SOE.Data.Images
{
    public class Keeper : IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid Guid { get; set; }

        public static DirectoryInfo Directory =>
            new(Path.Combine(Kit.Tools.Instance.LibraryPath, nameof(Keeper)));
        internal static void Delete(int IdKeeper)
        {
            foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>().Where(x => x.IdKeeper == IdKeeper))
            {
                archive.Delete();
            }
            AppData.Instance.LiteConnection.Table<Keeper>().Where(x => x.Id == IdKeeper).Delete();
        }
        public static Keeper New()
        {
            Keeper keeper = new Keeper();
            AppData.Instance.LiteConnection.Insert(keeper);
            return keeper;
        }

        internal async Task Save(PhotoArchive archive)
        {
            await Task.Yield();
            archive.IdKeeper = this.Id;
            FileInfo file =
                new FileInfo(System.IO.Path.Combine(Keeper.Directory.FullName, $"{Guid.NewGuid():N}{archive.Extension}"));
            await Save(archive.GetStream(), file);
            archive.Path = file.FullName;
            file.Refresh();
            if (file.Exists)
                AppData.Instance.LiteConnection.Insert(archive);
        }
        //internal async Task Save(Archive archive)
        //{
        //    archive.IdKeeper = this.Id;
        //    FileInfo file =
        //        new FileInfo(System.IO.Path.Combine(Keeper.Directory.FullName, $"{Guid.NewGuid():N}{archive.Extension}"));
        //    await Save(archive.GetStream(), file);
        //    archive.Path = file.FullName;
        //    AppData.Instance.LiteConnection.Insert(archive);
        //}
        public static async Task<FileImageSource> GetAvatar()
        {
            await Task.Yield();
            string filepath = System.IO.Path.Combine(Keeper.Directory.FullName, "Avatar.png");
            FileInfo TargetFile = new(filepath);
            if (!TargetFile.Exists)
            {
                return null;
            }
            return (FileImageSource)FileImageSource.FromFile(TargetFile.FullName);
        }
        public static Task<FileInfo> SaveAvatar(Task<Stream> GetStream) => Save(GetStream, "Avatar");
        public static Task<FileInfo> Save(Task<Stream> GetStream, string FileName = null, string FileExtension = ".png")
        {
            FileName ??= Guid.NewGuid().ToString("N");
            string filepath = System.IO.Path.Combine(Keeper.Directory.FullName, $"{FileName}{FileExtension}");
            FileInfo TargetFile = new(filepath);
            return Save(GetStream, TargetFile);
        }
        public static async Task<FileInfo> Save(Task<Stream> GetStream, FileInfo TargetFile)
        {
            await Task.Yield();
            try
            {
                if (!TargetFile.Directory.Exists)
                {
                    TargetFile.Directory.Create();
                }

                using (var fileStream = new FileStream(TargetFile.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (Stream image = await GetStream)
                    {
                        image.Position = 0;
                        await image.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Keeper.Save");
            }
            return TargetFile;
        }

        internal static void ClearAllFiles()
        {
            try
            {
                if (!Directory.Exists)
                {
                    Directory.Create();
                }
                Directory.GetFiles().ForEach(x =>
                {
                    try
                    {
                        x.Delete();
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e, "ClearAllFiles");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "ClearAllFiles");
            }
        }
    }
}
