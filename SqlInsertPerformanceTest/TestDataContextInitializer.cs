using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SqlInsertPerformanceTest
{
    public class TestDataContextInitializer : CreateDatabaseIfNotExists<TestDataContext>
    {
    }
}
