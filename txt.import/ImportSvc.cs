using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using txt.import.Class;

namespace txt.import
{
    public class ImportSvc
    {
        public virtual void Test()
        {

        }
        public virtual void Import()
        {
            CollectFileToProcess();
            var result = GetAllFileDataList();
            AddToDB(result);
        }
        public virtual void CollectFileToProcess()
        {
            try
            {
                var cur = System.IO.Directory.GetParent(@"./").FullName;
                var path = Directory.GetParent(cur).FullName;
                var files = Directory.EnumerateFiles(path, "*.*")
                .Where(s => s.EndsWith(".txt"));
                foreach (var item in files)
                {
                    var name = Path.GetFileNameWithoutExtension(item);
                    if (name.Contains("_PASS") || name.Contains("_FAIL"))
                    {
                        MoveFile(item, "Process");
                    }
                }

            }
            catch (Exception ex)
            {

                throw new CustomException("CollectFile");
            }

        }
        public virtual List<Result> GetAllFileDataList()
        {
            var result = new List<Result>();
            var cur = Path.Combine(Environment.CurrentDirectory, "Process");
            var files = Directory.EnumerateFiles(cur, "*.*");
            foreach (var item in files)
            {
                try
                {
                    var res = ReadFile(item);
                    if (res != null)
                    {
                        var result2 = SubstringObj(res);
                        result.AddRange(result2);
                    }
                    MoveFile(item, "Success");
                }
                catch (Exception ex)
                {
                    MoveFile(item, "Failed");
                    Console.WriteLine("ReadFileError"+ Path.GetFileName(item));
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message);
                    return null;

                }
            }
            return result;
        }
        public virtual List<Result> ReadFile(string filepath)
        {

            //var FileName = "6-1-2020_PASS.txt";
            //var cur = System.IO.Directory.GetParent(@"./").FullName;
            //var path = Directory.GetParent(cur).FullName;
            var filename = Path.GetFileName(filepath);
            var lines = File.ReadAllLines(filepath);

            var list_reult = new List<Result>();
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (filename.Contains("_PASS"))
                {
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
                else if (filename.Contains("_FAIL"))
                {
                    list_reult.Add(new Result()
                    {
                        //i_highvoltage_id = data[0],
                        c_test_voltage = "",
                        c_test_current = "",
                        c_test_time = "",
                        d_highvoltage = data[6],
                        i_pass = data[1],
                        c_series = data[0],
                        c_computer = ""

                    });
                }

            }
            return list_reult;



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
                    Decimal.TryParse(item.c_test_voltage, System.Globalization.NumberStyles.Float, null, out decimal c_test_voltage_con);
                    Decimal.TryParse(item.c_test_current, System.Globalization.NumberStyles.Float, null, out decimal c_test_current_con);
                    Decimal.TryParse(item.c_test_time, System.Globalization.NumberStyles.Float, null, out decimal c_test_time_con);
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
        public virtual void AddToDB(List<Result> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    //Log
                    return;
                }
                var cs = "Server=localhost;Port=3306;Database=store;User Id=root;";

                using (var con = new MySqlConnection(cs))
                {
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

                    var row_result = cmd.ExecuteNonQuery();
                    Console.WriteLine("Insert :"+ row_result+" rows");
                }
                //Console.WriteLine($"PostgreSQL version: {version}");
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public virtual void MoveFile(string file,string destination) {
            var appPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), destination);
            if (!Directory.Exists(appPath))
            {
                System.IO.Directory.CreateDirectory(appPath);
            }
            var filename = Path.GetFileName(file);
            var filenamewithoutext = Path.GetFileNameWithoutExtension(file);
            var ext = Path.GetExtension(file);

            if (File.Exists(Path.Combine(appPath, filename)))
            {
                filename = String.Format("{0}_{1}{2}", filenamewithoutext, DateTime.Now.ToFileTime(), ext);

            }
            System.IO.File.Move(file, Path.Combine(appPath, filename));
        }
    }
}
