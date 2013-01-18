using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                //TestViaEF(customers);
                TestViaDbHelper(customers);
                TestViaBulkCopy(customers);
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
            Console.WriteLine("{0} EF", sw.ElapsedMilliseconds);
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
            Console.WriteLine("{0} DbHelper", sw.ElapsedMilliseconds);
        }

        private static void TestViaBulkCopy(List<Customer> customers)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();

            DataTable customerTable = new DataTable("customers");

            // Add three column objects to the table. 
            DataColumn Id = new DataColumn();
            Id.DataType = System.Type.GetType("System.Int32");
            Id.ColumnName = "Id";
            Id.AutoIncrement = true;
            customerTable.Columns.Add(Id);

            DataColumn firstName = new DataColumn();
            firstName.DataType = System.Type.GetType("System.String");
            firstName.ColumnName = "FirstName";
            customerTable.Columns.Add(firstName);

            DataColumn lastName = new DataColumn();
            lastName.DataType = System.Type.GetType("System.String");
            lastName.ColumnName = "LastName";
            customerTable.Columns.Add(lastName);

            DataColumn age = new DataColumn();
            age.DataType = System.Type.GetType("System.Int32");
            age.ColumnName = "Age";
            customerTable.Columns.Add(age);

            // Create an array for DataColumn objects.
            DataColumn[] keys = new DataColumn[1];
            keys[0] = Id;
            customerTable.PrimaryKey = keys;

            foreach (var customer in customers)
            {
                // Add some new rows to the collection. 
                DataRow row = customerTable.NewRow();
                row["FirstName"] = customer.FirstName;
                row["LastName"] = customer.LastName;
                row["Age"] = customer.Age;
                customerTable.Rows.Add(row);
            }

            customerTable.AcceptChanges();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["TestDataContext"].ConnectionString))
            {
                bulkCopy.DestinationTableName = "dbo.Customers";
                bulkCopy.WriteToServer(customerTable);
            }

            sw.Stop();
            Console.WriteLine("{0} Bulk", sw.ElapsedMilliseconds);
        }
    }
}
