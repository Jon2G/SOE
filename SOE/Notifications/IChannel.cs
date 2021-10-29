using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Notifications
{
    public  interface IChannel
    {
        public string ChannelId { get; }
        public bool IsRegistered { get; }
        void Register();
    }
}
