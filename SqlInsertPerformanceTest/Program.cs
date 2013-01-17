using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInsertPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var customers = new List<Customer>();

            for (int i = 0; i < 10000; i++)
            {
                customers.Add(new Customer()
                {
                    FirstName = string.Format("First {0}", i),
                    LastName = string.Format("Last {0}", i),
                    Age = i
                });
            }


            for (int i = 0; i < 10; i++)
            {
                TestViaEF(customers);
                TestViaDbHelper(customers);
            }

            Console.ReadLine();
        }
  
        private static void TestViaEF(List<Customer> customers)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (TestDataContext db = new TestDataContext())
            {
                foreach (var customer in customers)
                {
                    db.Customers.Add(customer);
                }
                db.SaveChanges();
            }

            sw.Stop();
            Console.WriteLine("Save via EF in {0} milliseconds.", sw.ElapsedMilliseconds);
        }

        private static void TestViaDbHelper(List<Customer> customers)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sql = "INSERT INTO Customers VALUES (@FirstName, @LastName, @Age)";

            DbHelper.ExecuteNonQuery(sql, ConfigurationManager.ConnectionStrings["TestDataContext"].ConnectionString, customers,
               x =>
               {
                   return new IDbDataParameter[]
                                {
                                    DbHelper.CreateParameter("@FirstName", x.FirstName),
                                    DbHelper.CreateParameter("@LastName", x.LastName),
                                    DbHelper.CreateParameter("@Age", x.Age)
                                };
               });

            sw.Stop();
            Console.WriteLine("Save via DbHelper in {0} milliseconds.", sw.ElapsedMilliseconds);
        }
    }
}
