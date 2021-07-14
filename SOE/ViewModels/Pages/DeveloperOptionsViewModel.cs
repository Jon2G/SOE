using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kit;
using Kit.Extensions;
using Kit.Forms.Extensions;
using Kit.Model;
using SOE.Data;
using Xamarin.Essentials;

namespace SOE.ViewModels.Pages
{
    public class DeveloperOptionsViewModel : ModelBase
    {
        private ICommand _ImportDatabaseCommand;
        public ICommand ImportDatabaseCommand => _ImportDatabaseCommand ??= new Command(ImportDatabase);

        private ICommand _ExportDatabaseCommand;
        public ICommand ExportDatabaseCommand => _ExportDatabaseCommand ??= new Command(ExportDatabase);
        /// <summary>
        /// Esta función exportará una base de datos manualmente.
        /// </summary>
        private async void ExportDatabase()
        {
            using (UserDialogs.Instance.Loading("Cargando..."))
            {
                Log.Logger.Debug("Se solicito la base de datos lite");
                if (!await AbrirArchivo(AppData.Instance.LiteConnection.DatabasePath, "Base de datos local"))
                {
                    await UserDialogs.Instance.AlertAsync("No se pudo abrir el archivo", "Mensaje informativo");
                }
            }
        }

        private static async Task<bool> AbrirArchivo(string ruta, string titulo)
        {
            if (!await Permisos.RequestStorage())
            {
                return false;
            }
            FileInfo file = new FileInfo(ruta);
            if (!file.Exists)
            {
                await UserDialogs.Instance.AlertAsync("No se encontro el archivo", "Mensaje informativo");
                return false;
            }
            try
            {
                ////
                string temporal = file.FullName;

                if (file.Directory.FullName != FileSystem.CacheDirectory)
                {
                    string nombre = file.Name;
                    string nombre_sin_extension = nombre.Replace(file.Extension, string.Empty);
                    //name de archivo temporal SOE_42665AA6695A.db
                    temporal = $"{nombre_sin_extension}_{Guid.NewGuid():N}{file.Extension}";
                    //../cache/SOE_42665AA6695A.db
                    temporal = Path.Combine(FileSystem.CacheDirectory, temporal);
                    //Copiar el original en el temporal
                    File.Copy(file.FullName, temporal);
                }

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = titulo,
                    File = new ShareFile(temporal)
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Al abrir el archivo");
                await UserDialogs.Instance.AlertAsync($"No se pudo abrir el archivo\n{ex?.Message}", "Mensaje informativo");
                return false;
            }


        }

        /// <summary>
        /// Esta función importará una base de datos manualmente.
        /// </summary>
        private void ImportDatabase()
        {

        }
    }
}
