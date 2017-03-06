using System;

namespace AuditManager
{

    class Program
    {
        static void Main(string[] args)
        {
            int n = 60;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(((j == i) || j == (n - 1 - i )) ? "*" : "_");
                }
                Console.WriteLine("");
            }
            Console.ReadKey();
        }
    }
}
