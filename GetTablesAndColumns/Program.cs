using System;
using System.Data;
using System.Data.SqlClient;


namespace GetTablesAndColumns
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;User ID=sa;Password=sa;Persist Security Info=True"))
            {
                conn.Open();

                ////如果需要database中全部table，則使用conn.GetSchema("Tables")即可
                //string[] restrictions = new string[4];
                //restrictions[1] = "dbo";
                ////修改table名稱
                //restrictions[2] = "你的Table名稱";
                //DataTable tables = conn.GetSchema("Tables", restrictions);
                var tables = conn.GetSchema("Tables");

                ////有要過濾時再另外處理
                //var dv = tables.Select("TABLE_NAME like 'Order*'").CopyToDataTable().DefaultView;
                var dv = tables.DefaultView;
                dv.Sort = "TABLE_NAME asc";
                var schema = dv.ToTable();
                
                const string selectQuery = "select * from @tableName";
                var command = new SqlCommand(selectQuery, conn);
                var ad = new SqlDataAdapter(command);
                var ds = new DataSet();

                foreach (DataRow row in schema.Rows)
                {
                    Console.WriteLine("TABLE_NAME=" + row["TABLE_NAME"]);

                    command.CommandText = selectQuery.Replace("@tableName", row["TABLE_NAME"].ToString());
                    ad.FillSchema(ds, SchemaType.Mapped, row["TABLE_NAME"].ToString());
                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        Console.WriteLine($"  ├ DataType = {dc.DataType.Name}, ColumnName = {dc.ColumnName}");
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
