using System;
using System.IO;
using System.Threading.Tasks;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;

namespace SOE.Data.Images
{
    public class Keeper : IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid Guid { get; set; }

        public static DirectoryInfo Directory => new DirectoryInfo(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal), nameof(Keeper)));

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

        internal async Task Save(Archive archive)
        {
            archive.IdKeeper = this.Id;
            FileInfo file =
                new FileInfo(System.IO.Path.Combine(Keeper.Directory.FullName, $"{Guid.NewGuid():N}{archive.Extension}"));
            await Save(archive.GetStream(), file);
            archive.Path = file.FullName;
            AppData.Instance.LiteConnection.Insert(archive);
        }






        public static Task<FileInfo> Save(Task<Stream> GetStream,string FileExtension=".png")
        {
            FileInfo TargetFile =
                new FileInfo(System.IO.Path.Combine(Keeper.Directory.FullName, $"{Guid.NewGuid():N}{FileExtension}"));
            return Save(GetStream, TargetFile);
        }
        public static async Task<FileInfo> Save(Task<Stream> GetStream, FileInfo TargetFile)
        {
            //await Task.Yield();
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
            return TargetFile;
        }
    }
}
