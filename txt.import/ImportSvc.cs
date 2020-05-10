using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            try
            {
                var lines = File.ReadAllLines(Path.Combine(System.IO.Directory.GetParent(@"./").FullName, "6-1-2020_PASS.txt"));
                var list_reult = new List<Result>();
                foreach (var line in lines)
                {
                    var data = line.Split(',');
                    list_reult.Add(new Result()
                    {
                        c_serial = data[0],
                    });
                }
                return list_reult;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public virtual void AddToDB(List<Result> data)
        {
            try
            {
                var cs = "Server=localhost;Port=3306;Database=store;User Id=root;";

                using var con = new MySqlConnection(cs);
                con.Open();

                var sql = "INSERT INTO `b_highvoltage` ( `c_test_voltage`, `c_test_current`, `c_test_time`, `d_highvoltage`, `i_pass`, `c_serial`, `c_series`, `c_computer`) VALUES ;";
                foreach (var item in data)
                {
                    var row = String.Format(@"('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", item.c_serial, item.c_serial, item.c_serial, item.c_serial, item.c_serial, item.c_serial, item.c_serial, item.c_serial, item.c_serial);
                    sql += row;
                    if (item != data.LastOrDefault())
                    {
                        sql += ',';
                    }
                }
                using var cmd = new MySqlCommand(sql, con);

                var version = cmd.ExecuteReader();

                //Console.WriteLine($"PostgreSQL version: {version}");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
