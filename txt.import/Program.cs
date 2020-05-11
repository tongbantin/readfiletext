using System;

namespace txt.import
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportSvc svc = new ImportSvc();
            svc.Import();
            Console.ReadKey();
        }
    }
}
