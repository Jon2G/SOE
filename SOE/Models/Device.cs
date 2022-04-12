using FirestoreLINQ;
using SOE.API;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace SOE.Models
{
    [Preserve(AllMembers = true), FireStoreCollection("Devices")]
    public class Device
    {
        public string DeviceKey { get; set; }

        public string Brand { get; set; }

        public string Platform { get; set; }

        public string Model { get; set; }

        public string Name { get; set; }

        public DateTime LastTimeSeen { get; set; }

        public Device()
        {

        }
        internal async Task Save()
        {
            await Task.Yield();
            await FireBaseConnection.UserDocument.Collection<Device>()
                .Document(DeviceKey).SetAsync(this);
        }
    }
}
