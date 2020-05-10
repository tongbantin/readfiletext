using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using txt.import.Class;

namespace txt.import
{
    public class ImportSvc
    {
        public virtual void Test() { 
            
        }
        public virtual void Import()
        {
            var result = ReadFile();
            AddToDB(result);
        }
        public virtual List<Result> ReadFile()
        {
            var lines = File.ReadAllLines(Path.Combine(System.IO.Directory.GetParent(@"./").FullName, "6-1-2020_PASS.txt"));
            var list_reult = new List<Result>();
            foreach (var line in lines)
            {
                var data = line.Split(',');
                list_reult.Add(new Result()
                {
                    //i_highvoltage_id = data[0],
                    c_test_voltage = data[17],
                    c_test_current = data[18],
                    c_test_time = data[20],
                    d_highvoltage = data[7],
                    i_pass = data[2],
                    c_serial = data[0],
                    c_series = data[1],
                    c_computer = ""

                });
            }
            return list_reult;
        }
        public virtual void AddToDB(List<Result> data)
        {
            var cs = "Server=localhost;Port=3306;Database=store;User Id=root;";

            using var con = new MySqlConnection(cs);
            con.Open();

            var sql = "INSERT INTO `b_highvoltage` (`i_highvoltage_id`, `c_test_voltage`, `c_test_current`, `c_test_time`, `d_highvoltage`, `i_pass`, `c_serial`, `c_series`, `c_computer`) " +
                "VALUES ('1', '123', '123', '123', '12312', '3213213', '213213', '21321', '3123123');";

            using var cmd = new MySqlCommand(sql, con);

            var version = cmd.ExecuteReader();

            //Console.WriteLine($"PostgreSQL version: {version}");
        }
    }
}
