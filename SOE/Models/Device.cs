using FirestoreLINQ;
using Google.Cloud.Firestore;
using SOE.API;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace SOE.Models
{
    [Preserve(AllMembers = true), FireStoreCollection("Devices"), FirestoreData]
    public class Device
    {
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public string DeviceKey { get; set; }
        [FirestoreProperty]
        public string Brand { get; set; }
        [FirestoreProperty]
        public string Platform { get; set; }
        [FirestoreProperty]
        public string Model { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public Timestamp LastTimeSeen { get; set; }

        public Device()
        {

        }
        internal async Task Save()
        {
            await Task.Yield();
            await FireBaseConnection.Instance.UserDocument.Collection<Device>()
                .Document(DeviceKey).SetAsync(this);
        }
    }
}
