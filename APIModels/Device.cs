using System;

namespace SOEWeb.Shared
{
    public class Device
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int UserId { get; set; }
        public string DeviceKey { get; set; }
        public string Brand { get; set; }
        public string Platform { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public DateTime LastTimeSeen { get; set; }
        public bool Enabled { get; set; }

    }
}
