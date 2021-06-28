using SchoolOrganizer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Data.Images;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.Models
{
    public class TaskPageModel
    {
        //internal static async Task<FileImageSource> SaveImage(FileResult result)
        //{
        //    FileInfo target = new FileInfo(Path.Combine(AppData.Instance.Directory.FullName, result.FileName));
        //    await Keeper.Save(result.OpenReadAsync(), target);
        //    return GetImage(target);
        //}
        //internal static FileImageSource GetImage(FileInfo av)
        //{
        //    DirectoryInfo directory = AppData.Instance.Directory;
        //    if (!directory.Exists)
        //    {
        //        directory.Create();
        //        return null;
        //    }

        //    FileInfo avatar = new FileInfo(av.FullName);
        //    if (!avatar.Exists)
        //    {
        //        return null;
        //    }
        //    return (FileImageSource)FileImageSource.FromFile(avatar.FullName);
        //}
    }
}
