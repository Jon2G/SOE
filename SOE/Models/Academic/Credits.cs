using System;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;

namespace SOE.Models.Academic
{
    [Table("CREDITS")]
    public class Credits : IGuid
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        [MaxLength(10),Column("BOLETA")]
        public string UserId { get; set; }
        [Column("CREDITOS_OBTENIDOS")]
        public double CurrentCredits { get; set; }
        [Column("CREDITOS_TOTALES")]
        public double TotalCredits { get; set; }
        [Column("PORCENTAJE")]
        public double Percentage { get; set; }
        public Guid Guid { get; set; }
        public Credits() { }
    }
}
