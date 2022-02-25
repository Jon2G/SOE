using AsyncAwaitBestPractices;
using Google.Cloud.Firestore;
using Kit;
using Kit.Forms.Extensions;
using SOE.Enums;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.Data.Archives
{
    [FirestoreData]
    public class PhotoArchive : Archive<FileImageSource>
    {
        private FileImageSource _Value;
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

        public PhotoArchive() : base()
        {

        }

        private async Task CompressAsync(FileImageSource cached)
        {
            await Task.Yield();
            Value = await cached.CompressImage(80);
            FileInfo file = new(Value.File);
            var size = file.Length.ToSize(BytesConverter.SizeUnits.MB);
            if (size >= 9)
            {
                await CompressAsync(Value);
            }
            this.Path = Value.File;
        }

        public async Task LoadImage()
        {
            await Task.Yield();
        }
        public PhotoArchive(Archive archive, FileImageSource Value) : base(archive, Value)
        {

        }
    }
}
