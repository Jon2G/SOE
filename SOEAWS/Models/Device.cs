﻿using Kit;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Kit.Sql.SqlServer;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace SOEAWS.Models
{
    public class Device:IGuid
    {
        [Column("ID"),PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        [Column("GUID"), NotNull, Unique]
        public Guid Guid { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("DEVICE_KEY"),MaxLength(100)]
        public string DeviceKey { get; set; }
        [Column("BRAND"), MaxLength(100)]
        public string Brand { get; set; }
        [Column("PLATFORM"), MaxLength(100)]
        public string Platform { get; set; }
        [Column("MODEL"), MaxLength(100)]
        public string Model { get; set; }
        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; }
        [Column("LAST_TIME_SEEN"),NotNull]
        public DateTime LastTimeSeen { get; set; }
        [Column("ENABLED")]
        public bool Enabled { get; set; }

        internal static Device GetById(SQLServerConnection Connection, int DeviceId) =>
            Connection.Table<Device>().FirstOrDefault(x => x.Id == DeviceId);

        internal static Device GetByKey(SQLServerConnection Connection, string Key) =>
            Connection.Table<Device>().FirstOrDefault(x => x.DeviceKey == Key);

        public void UpdateLastTimeSeen(SqlConnection Connection, ILogger logger)
        {
            try
            {
                Connection.Execute(@"UPDATE DEVICES SET LAST_TIME_SEEN=GETDATE() WHERE ID=@ID",System.Data.CommandType.Text
                    , new SqlParameter("ID", this.Id));
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, ex?.Message);
            }
        }

        public void Save(SQLServerConnection Connection)
        {
            if (this.Id <= 0)
            {
                Connection.Insert(this);
                return;
            }
            Connection.Update(this);
            return;
        }
    }
}
