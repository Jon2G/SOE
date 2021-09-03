using Kit;
using Microsoft.Extensions.Logging;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SOEAWS.Services
{
    public static class ArchieveService
    {
        internal static int[] GetIdsByGuid(Guid ArchieveGuid, ILogger logger)
        {
            int[] ids = new int[0] { };
            try
            {
                using (SqlConnection con = WebData.Connection)
                {
                    ids = con.Lista<int>(
                            "SP_GET_ARCHIEVE_ID_BY_GUID",
                            CommandType.StoredProcedure, 0,
                            new SqlParameter("GUID", ArchieveGuid))
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "GetIdsByGuid");
            }

            return ids;
        }
    }
}
