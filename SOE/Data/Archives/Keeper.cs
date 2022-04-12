using FFImageLoading;

using Kit;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Log = Kit.Log;

namespace SOE.Data.Archives
{
    public static class Keeper
    {
        public static readonly DirectoryInfo Directory =
            new(Path.Combine(Kit.Tools.Instance.LibraryPath, nameof(Keeper)));


        //internal static void Delete(int IdKeeper)
        //{
        //    foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>().Where(x => x.IdKeeper == IdKeeper))
        //    {
        //        archive.Delete();
        //    }
        //    AppData.Instance.LiteConnection.Table<Keeper>().Where(x => x.Guid == IdKeeper).Delete();
        //}
        public static async Task<PhotoArchive> Save(PhotoArchive archive)
        {
            await Task.Yield();
            string path = archive.Path;
            FileInfo file =
                new FileInfo(System.IO.Path.Combine(Keeper.Directory.FullName,
                    $"{Guid.NewGuid():N}{archive.Extension}"));
            archive.Path = file.FullName;
            IDocumentReference result = await
                Archive.Collection.AddAsync(archive);
            archive.DocumentId = result.Id;
            using (Stream stream = await Archive.GetStream(path))
            {
                await Blobs.Save(new Blobs()
                {
                    ArchiveDocumentId = archive.DocumentId,
                    Base64 = stream.ToByteArray().ToImageString()
                });
                await Save(stream, file);
            }
            return archive;
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
        public static void DeleteAvatar() => Delete("Avatar");
        public static Task<FileInfo> SaveAvatar(Task<Stream> GetStream) => Save(GetStream, "Avatar");
        public static async Task<FileInfo> Save(Task<Stream> GetStream, string FileName = null, string FileExtension = ".png")
        {
            await Task.Yield();
            FileName ??= Guid.NewGuid().ToString("N");
            string filepath = System.IO.Path.Combine(Keeper.Directory.FullName, $"{FileName}{FileExtension}");
            FileInfo TargetFile = new(filepath);
            return await Save(await GetStream, TargetFile);
        }
        public static async Task<FileInfo> Save(Stream stream, FileInfo TargetFile)
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
                    using (Stream image = stream)
                    {
                        image.Position = 0;
                        await image.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "Keeper.Save");
            }
            return TargetFile;
        }
        public static void Delete(string FileName, string FileExtension = ".png")
        {
            string filepath = System.IO.Path.Combine(Keeper.Directory.FullName, $"{FileName}{FileExtension}");
            FileInfo TargetFile = new(filepath);
            if (TargetFile.Exists)
                TargetFile.Delete();
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
        public static IAsyncEnumerable<T> GetById<T>(string documentId) where T : Archive, new()
        {
            IQuery q = Archive.Collection.WhereEqualsTo(nameof(Archive.ParentId), documentId);
            return Archive.IQuery<T>(q);
        }
    }
}
