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
            var lines = File.ReadAllLines("C:\\Users\\tongb\\Documents\\6-1-2020_PASS.txt");
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
        public virtual void AddToDB(List<Result> data)
        {

        }
    }
}
