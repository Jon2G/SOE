using System.Collections.Generic;
using System.Data.SqlClient;

namespace Tests.Models
{
    public class AppColor
    {
        public string Light { get; set; }
        public string Dark { get; set; }
        public string Identifier { get; set; }


        public static List<AppColor> GetAll()
        {
            List<AppColor> colors = new ();
            using (SqlConnection con=new (@"Data Source=soe-database-instance.c5kv8sy96hgu.us-east-2.rds.amazonaws.com,1433;Initial Catalog=SOE_DATABASE;Integrated Security=False;Persist Security Info=True;User ID=admin;Encrypt=False;Password=Octopus$2021.;"))
            {
                con.Open();
                using (SqlCommand cmd=new ("select IDENTIFIER,COLOR,DARK_COLOR from APP_COLORS", con))
                {
                    using (SqlDataReader reader=cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            colors.Add(new ()
                            {
                                Identifier = reader[0].ToString(),
                                Light = reader[1].ToString(),
                                Dark = reader[2].ToString()
                            });
                        }
                    }
                }
            }
            return colors;
        }
    }
}
