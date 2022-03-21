using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using NecklaceDB;
using NecklaceModels;
using NecklaceRepository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace NecklaceApplication
{
    // vet inte om den ska vara  med
    static class MyLinqExtensions
    {
        public static void Print<T>(this IEnumerable<T> collection)
        {
            collection.ToList().ForEach(item => Console.WriteLine(item));
        }
    }
    class Program
    {
      private static DbContextOptionsBuilder<NecklaceDbContext> _optionsBuilder;
        static void Main(string[] args)
        {
            if (!BuildOptions())
                return; //Terminate if not build correctly

            //SeedDataBase();
            QueryDatabaseAsync().Wait();
            //QueryDatabase_Linq();
            //QueryDatabase_DataModel_Linq();
            QueryDatabaseCRUD().Wait();

            Console.WriteLine("\nPress any key to terminate");
            Console.ReadKey();
        }

        private static bool BuildOptions()
        {
            _optionsBuilder = new DbContextOptionsBuilder<NecklaceDbContext>();

            #region Ensuring appsettings.json is in the right location
            Console.WriteLine($"DbConnections Directory: {DBConnection.DbConnectionsDirectory}");

            var connectionString = DBConnection.ConfigurationRoot.GetConnectionString("SQLServer_necklace");
            if (!string.IsNullOrEmpty(connectionString))
                Console.WriteLine($"Connection string to Database: {connectionString}");
            else
            {
                Console.WriteLine($"Please copy the 'DbConnections.json' to this location");
                return false;
            }
            #endregion

            _optionsBuilder.UseSqlServer(connectionString);
            return true;
        }

        private static void SeedDataBase()
        {
            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                //Create some customers
                var rnd = new Random();
                var NecklaceList = new List<Necklace>();
                for (int i = 0; i < 1000; i++)
                {
                    NecklaceList.Add(Necklace.Factory.CreateRandomNecklace(rnd.Next(5, 50)));
                }

                //Add Pearls and Necklaces to the Database
                foreach (var n in NecklaceList)
                {
                    db.Necklaces.Add(n);
                }

                db.SaveChanges();
            }
        }

        private static async Task QueryDatabaseAsync()
        {
            Console.WriteLine("\nQuery Database Async");
            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                var necklaceCount = await db.Necklaces.CountAsync();
                var pearlCount = await db.Pearls.CountAsync();

                Console.WriteLine($"Nr of Necklaces: {necklaceCount}");
                Console.WriteLine($"Nr of Pearls: {pearlCount}");
            }
        }
        private static void QueryDatabase_Linq()
        {
            Console.WriteLine("\nQuery Database using Linq");
            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                //Use .AsEnumerable() to make sure the Db request is fully translated to be managed by Linq.
                var necklaces = db.Necklaces.AsEnumerable().ToList();
                var pearls = db.Pearls.AsEnumerable().ToList();

                Console.WriteLine($"\nOuterJoin: Necklace - Pearls via GroupJoin: Descending by Necklace Price");
                var list1 = necklaces.GroupJoin(pearls, n => n.NecklaceID, pList => pList.NecklaceID, (n, pList) => new { n.NecklaceID, pList });
                foreach (var pearlGroup in list1.OrderByDescending(pg => pg.pList.Sum(p => p.Price)))
                {
                    Console.WriteLine($"Necklace: {pearlGroup.NecklaceID}, Nr Of Pearls: {pearlGroup.pList.Count()}, Price: {pearlGroup.pList.Sum(p => p.Price):C2}");
                }

                Console.WriteLine($"\nOuterJoin: Customer - Order via GroupJoin: Customer with highest ordervalue");
                var MostExpensive = list1.OrderByDescending(pg => pg.pList.Sum(p => p.Price)).First();

                Console.WriteLine($"Most expensive Necklace: {MostExpensive.NecklaceID}, Nr Of Pearls: {MostExpensive.pList.Count()}, Price: {MostExpensive.pList.Sum(p => p.Price):C2}");
            }
        }
        private static void QueryDatabase_DataModel_Linq()
        {
            Console.WriteLine("\nQuery Database using fully loaded datamodels");
            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                //Use .AsEnumerable() to make sure the Db request is fully translated to be managed by Linq.
                //Use ToList() to ensure the Model is fully loaded
                var necklaces = db.Necklaces.ToList();
                var pearls = db.Pearls.ToList();

                var MostExpensiveNecklace = necklaces.OrderByDescending(n => n.Price).First();
                Console.WriteLine($"Most expensive Necklace: {MostExpensiveNecklace.NecklaceID}, Nr Of Pearls: {MostExpensiveNecklace.Pearls.Count()}, Price: {MostExpensiveNecklace.Pearls.Sum(p => p.Price):C2}");

                Console.WriteLine($"Most expensive Pearls");
                foreach (var pearl in pearls.OrderByDescending(p => p.Price).Take(5))
                {
                    Console.WriteLine($"Pearl: {pearl}, Price: {pearl.Price:C2}, in Necklace {pearl.Necklace.NecklaceID}");
                }
            }
        }


        // För pärlan
        private static async Task QueryDatabaseCRUD()
        {
            Console.WriteLine("\nPärlan CRUD");
            Console.WriteLine("--------------------");

            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                var _repo = new PearlRepository(db);

                //    //First 5 Halsbanden FUNKAR
                Console.WriteLine("Testing ReadAllAsync()");
            var AllPearls = await _repo.ReadAllAsync();// läser in alla
            Console.WriteLine($"Nr of Necklace {AllPearls.Count()}");
            Console.WriteLine($"\nFirst 5 Necklace");
            AllPearls.Take(5).Print();

            // loopa igenom alla pärlor
            //foreach (var VARIABLE in AllPearls)
            //{
            //    Console.WriteLine($"LARS {VARIABLE}");
            //}

            }






            // För halsbandet
            Console.WriteLine("\nHalsbandet CRUD");
            Console.WriteLine("--------------------");

            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                var _repo = new NecklaceRepository.NecklaceRepository();

                //First 5 Halsbanden FUNKAR
                Console.WriteLine("Testing ReadAllAsync()");
                var AllNecklaces = await _repo.ReadAllAsync();// läser in alla
                Console.WriteLine($"Nr of Necklace {AllNecklaces.Count()}");
                Console.WriteLine($"\nFirst 5 Necklace");
                AllNecklaces.Take(5).Print();

                // loopa igenom alla pärlor
                //foreach (var VARIABLE in AllNecklaces)
                //{
                //    Console.WriteLine($"LARS {VARIABLE}");
                //}






            }
        }

    }
}
