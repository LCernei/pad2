using System;
using Cassandra;

namespace CasandraDB
{
    class Program
    {
        static void Main(string[] args)
        {
            
            while (true)
            {
                var cluster = Cluster.Builder()
                    .AddContactPoints("172.17.0.2")
                    .Build();
                var session = cluster.Connect("my_keyspace");
                var rs = session.Execute("SELECT * FROM Movies");
                foreach (var row in rs)
                {
                    var value = row.GetValue<string>("name");
                    Console.WriteLine(value);
                }

                session.Execute("insert into movies(id, name) values(2,'Shrek4')");

                Console.WriteLine("Hello World!");
            }
        }
    }
}
