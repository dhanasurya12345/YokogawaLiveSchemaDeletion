using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CollectingSchema
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> dataSourceListId = new List<string>();
            Console.WriteLine("Please enter the connectionstring string");
            string connectionStiring = Console.ReadLine();
            var datatable = new DataTable();
            NpgsqlCommand command = new NpgsqlCommand();
            var connection = new NpgsqlConnection
            {
                ConnectionString = connectionStiring
                //ConnectionString = "Host=salesforcestagingdb-do-user-7767011-0.b.db.ondigitalocean.com;Port=25060;Database=boldbi-db-imdb;Username=doadmin;Password=AVNS_rqo32TL59uxzsDym1Jx;"
            };
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT schema_name FROM information_schema.schemata;";
            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();
            using (DataSet dataset = new DataSet() { EnforceConstraints = false })
            {
                dataset.Tables.Add(datatable);
                datatable.Load(reader, LoadOption.OverwriteChanges);
                dataset.Tables.Remove(datatable);
            }
            for (int i = 0; i < datatable.Rows.Count; i++)
            {
                if (datatable.Rows[i].ItemArray.FirstOrDefault().ToString().Length > 47 && datatable.Rows[i].ItemArray.FirstOrDefault().ToString().Contains("_"))
                {
                    dataSourceListId.Add(datatable.Rows[i].ItemArray.FirstOrDefault().ToString());
                    Console.WriteLine(datatable.Rows[i].ItemArray.FirstOrDefault().ToString());
                    Console.WriteLine();
                }
            }
            Console.WriteLine("{0} - schemas in DB", dataSourceListId.Count);
            Console.WriteLine("Please check and type yes to delete all the live schemas from the database.");
            string input = Console.ReadLine();
            if (input.ToLower() == "yes")
            {
                foreach (var id in dataSourceListId)
                {
                    string query2 = string.Format("DROP SCHEMA IF EXISTS \"{0}\" CASCADE;", id);
                    command.CommandText = query2;
                    command.ExecuteNonQuery();
                }

            }
            connection.Close();

        }
    }
}
