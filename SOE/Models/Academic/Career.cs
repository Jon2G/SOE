using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using System;

namespace SOE.Models.Academic
{
    public class Career : IGuid
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public bool IsOffline { get; set; }
    }
}
