using Kit;
using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SOE.Interfaces;
using System.Linq;
using Xamarin.Forms;

namespace SOE.Services
{
    public static class UpdateService
    {
        private static bool HasInternet()
        {
            try
            {
                if (!CrossConnectivity.IsSupported)
                {
                    return false;
                }

                if (CrossConnectivity.Current.IsConnected)
                {
                    if (CrossConnectivity.Current.ConnectionTypes.Any(x => x == ConnectionType.WiFi))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e )
            {
               Log.Logger.Error(e,"Al determinar si hay conexión a internet");
            }
            return false;
        }

        public static void AvaibleUpdate()
        {
            if (!HasInternet())
            {
                return;
            }
            IAvailableUpdateService availableUpdateService = DependencyService.Get<IAvailableUpdateService>();
            availableUpdateService?.HasUpdate();
        }
    }
}
