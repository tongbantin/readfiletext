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
            var result2 = SubstringObj(result);
            AddToDB(result2);
        }
        public virtual List<Result> SubstringObj(List<Result> data)
        {
            try
            {
                var result = new List<Result>();
                foreach (var item in data)
                {
                    //serial series
                    var series = item.c_series.Trim().Split(" ");
                    var serial = series[2];
                    var serie_arr = series.Skip(3).Take(series.Length - 3).ToArray();
                    var serie = String.Join(' ', serie_arr);
                    //date time 
                    var date = item.d_highvoltage.Trim().Replace("Date Time : ", "");
                    //voltage convert 
                    decimal c_test_voltage_con = Decimal.Parse(item.c_test_voltage, System.Globalization.NumberStyles.Float);
                    decimal c_test_current_con = Decimal.Parse(item.c_test_current, System.Globalization.NumberStyles.Float);
                    decimal c_test_time_con = Decimal.Parse(item.c_test_time, System.Globalization.NumberStyles.Float);
                    //ipasss tim
                    var i_pass_tim = item.i_pass.Trim().Replace("Result : ", "");

                    item.c_serial = serial;
                    item.c_series = serie;
                    item.d_highvoltage = date;
                    item.c_test_voltage = c_test_voltage_con.ToString();
                    item.c_test_current = c_test_current_con.ToString();
                    item.c_test_time = c_test_time_con.ToString();
                    item.i_pass = i_pass_tim.ToString();
                    result.Add(item);
                   
                }
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        public virtual List<Result> ReadFile()
        {

            //var FileName = "6-1-2020_PASS.txt";
            var cur = System.IO.Directory.GetParent(@"./").FullName;
            var path = Directory.GetParent(cur).FullName;
            var lines = File.ReadAllLines(Path.Combine(path, "6-1-2020_PASS.txt"));
            
            var list_reult = new List<Result>();
            foreach (var line in lines)
            {
                var data = line.Split(',');
                list_reult.Add(new Result()
                {
                    //i_highvoltage_id = data[0],
                    c_test_voltage = data[16],
                    c_test_current = data[17],
                    c_test_time = data[19],
                    d_highvoltage = data[6],
                    i_pass = data[1],
                    c_series = data[0],
                    c_computer = ""

                });
            }

            System.IO.File.Move(path+"6-1-2020_PASS.txt",path+"6-1-2020_PASS_uploaded.txt");

            return list_reult;
        }
        public virtual void AddToDB(List<Result> data)
        {
            try
            {
                var cs = "Server=localhost;Port=3306;Database=store;User Id=root;";

                using var con = new MySqlConnection(cs);
                con.Open();

                var sql = "INSERT INTO `b_highvoltage` ( `c_test_voltage`, `c_test_current`, `c_test_time`, `d_highvoltage`, `i_pass`, `c_serial`, `c_series`, `c_computer`) VALUES ";
                foreach (var item in data)
                {
                    var row = String.Format(@"('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", item.c_test_voltage, item.c_test_current, item.c_test_time, item.d_highvoltage, item.i_pass, item.c_serial, item.c_series, item.c_computer);
                    sql += row;
                    if (item != data.LastOrDefault())
                    {
                        sql += ',';
                    }
                }
                using var cmd = new MySqlCommand(sql, con);

                var version = cmd.ExecuteNonQuery();

                //Console.WriteLine($"PostgreSQL version: {version}");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
