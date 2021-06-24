using System;
using System.Threading.Tasks;
using APIModels;
using APIModels.Enums;
using Kit;
using Kit.Daemon.Devices;
using Kit.Services.Web;
using Kit.Sql.Base;
using Newtonsoft.Json;
using SOE.Data;
using SOE.Models.Data;
using Device = Kit.Daemon.Devices.Device;

namespace SOE.API
{
    public class APIService
    {
        private readonly Kit.Services.Web.WebService WebService;

        public APIService()
        {
            WebService = new WebService("https://192.168.0.32:44371/AppAuthentication");
        }

        public async Task<bool> IsAuthorizated(SqlBase sql)
        {
            //try
            //{
            //    //if (Tools.Debugging)
            //    //{
            //    //    return await Task.FromResult(true);
            //    //}
            //    await Task.Yield();
            //    DeviceInformation = DeviceInformation.Get(sql);
            //    bool Autorized = false;
            //    ProjectActivationState state = ProjectActivationState.Unknown;
            //    state = await Autheticate(AppName);
            //    switch (state)
            //    {
            //        case ProjectActivationState.Active:
            //            Log.Logger.Information("Project is active");
            //            Autorized = true;
            //            DeviceInformation.LastAuthorizedTime = DateTime.Now;
            //            sql.InsertOrReplace(DeviceInformation);
            //            break;

            //        case ProjectActivationState.Expired:
            //            this.Reason = "La licencia para usar esta aplicación ha expirado";
            //            await Tools.Instance.Dialogs.CustomMessageBox.Show(this.Reason, "Acceso denegado");
            //            break;

            //        case ProjectActivationState.Denied:
            //            Log.Logger.Information("Acces denied");
            //            this.Reason = "Este dispositivo no cuenta con la licencia para usar esta aplicación";
            //            await Tools.Instance.Dialogs.CustomMessageBox.Show(this.Reason, "Acceso denegado");
            //            break;

            //        case ProjectActivationState.LoginRequired:
            //            this.Reason = "Este dispositivo debe ser registrado con una licencia valida antes de poder acceder a la aplicación";
            //            await Tools.Instance.Dialogs.CustomMessageBox.Show(this.Reason, "Acceso denegado");
            //            await OpenRegisterForm();
            //            Autorized = await IsAuthorizated(sql);
            //            break;

            //        case ProjectActivationState.ConnectionFailed:
            //            this.Reason = "Revise su conexión a internet";
            //            await Tools.Instance.Dialogs.CustomMessageBox.Show(this.Reason, "Atención");
            //            return CanBeAuthorizedByTime();
            //    }
            //    return Autorized;
            //}
            //catch (Exception ex)
            //{
            //    await Kit.Tools.Instance.Dialogs.CustomMessageBox.Show(ex.Message, "Alerta", CustomMessageBoxButton.OK, CustomMessageBoxImage.Error);
            //    Log.Logger.Error(ex, "Al comprobar la licencia");
            //    return false;
            //}
            return true;
        }
        public bool CanBeAuthorizedByTime()
        {
            //if (this.DeviceInformation is not null)
            //{
            //    double days = Math.Abs((DateTime.Now - this.DeviceInformation.LastAuthorizedTime).TotalDays);
            //    return days < 7;
            //}
            return false;
        }
        public async Task<bool> RegisterDevice(string userName, string password)
        {
            //string DeviceBrand = GetDeviceBrand();
            //string Platform = GetDevicePlatform();
            //string Name = GetDeviceName();
            //string Model = GetDeviceModel();
            //switch (await WebService.EnrollDevice(DeviceBrand, Platform, Name, Model, AppKey, userName, password))
            //{
            //    case "NO_DEVICES_LEFT":

            //        await Tools.Instance.Dialogs.CustomMessageBox.Show("No le quedan mas dispositivos para este proyecto", "Atención");

            //        break;

            //    case "PROJECT_NOT_ENROLLED":

            //        await Tools.Instance.Dialogs.CustomMessageBox.Show("No esta contratado este servicio", "Atención");

            //        break;

            //    case "SUCCES":
            //        int left = await GetDevicesLeft(AppKey, userName);
            //        if (left != -1)
            //        {
            //            await Tools.Instance.Dialogs.CustomMessageBox.Show($"Registro exitoso, le quedan: {left} dispositivos", "Atención");
            //        }
            //        else
            //        {
            //            await Tools.Instance.Dialogs.CustomMessageBox.Show($"Registro exitoso", "Atención");
            //        }
            //        return true;
            //}
            return false;
        }

        public async Task<Response> Login(string Usuario, string PasswordPin, string school = null)
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin) || PasswordPin.Length < 8
                                              || (!Validations.IsValidEmail(Usuario) && !Validations.IsValidBoleta(Usuario)))
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("LogIn", Usuario, PasswordPin, school);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }

        public async Task<Response> SignUp(string PasswordPin, UserType Type, APIModels.Device Device)
        {
            User User = AppData.Instance.User;
            if (string.IsNullOrEmpty(User.Boleta)
                || string.IsNullOrEmpty(User.Password)
                || string.IsNullOrEmpty(PasswordPin)
                || PasswordPin.Length < 8
                || !Validations.IsValidEmail(User.Email)
                || !Validations.IsValidBoleta(User.Boleta)
                || User.School is null
                || string.IsNullOrEmpty(User.School.Name)
                || string.IsNullOrEmpty(Device.DeviceKey)
                || Type == UserType.INVALID)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("SignUp",
                 User.Boleta, User.Name, User.Email,
                 PasswordPin, User.School.Name, User.Password, ((int)Type).ToString(), JsonConvert.SerializeObject(Device));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }

    }
}
