using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NecklaceRepository;
using NecklaceDB;
using NecklaceModels;
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

            SeedDataBase();
            QueryDatabaseAsync().Wait();
            //QueryDatabase_Linq();
            QueryDatabase_DataModel_Linq();
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
                Console.WriteLine("Testing ReadAllAsync()");
                var AllPearls = await _repo.ReadAllAsync();// läser in alla
                Console.WriteLine($"Nr of Necklace {AllPearls.Count()}");
                Console.WriteLine($"\nFirst 5 Necklace");
                AllPearls.Take(5).Print();

                Console.WriteLine("\nTesting ReadAsync()");
                Console.WriteLine("--------------------");
                var LastPearl1 = AllPearls.Last();
                var LastPearl2 = await _repo.ReadAsync(LastPearl1.PearlID);
                Console.WriteLine($"Last Pearl.\n{LastPearl1}");
                Console.WriteLine($"Read Pearl with NecklaceID == Last Necklace\n{LastPearl2}");
                if (LastPearl1 == LastPearl2)
                    Console.WriteLine("Pearls Equal");
                else
                    Console.WriteLine("ERROR: Pearl not equal");

                Console.WriteLine($"Pearl LAST 1 {LastPearl1}");
                Console.WriteLine($"Pearl LAST 2 {LastPearl2}");


                Console.WriteLine("\nTesting DeleteAsync()");
                Console.WriteLine("--------------------");
                var Pearl1ToDelete = AllPearls.Last();
                var DelPearl1 = await _repo.DeleteAsync(Pearl1ToDelete.PearlID);
                Console.WriteLine($"Pearl to delete.\n{Pearl1ToDelete}");
                Console.WriteLine($"Deleted Pearl.\n{DelPearl1}");

                if (DelPearl1 != null && DelPearl1 == Pearl1ToDelete)
                    Console.WriteLine("Pearl Equal");
                else
                    Console.WriteLine("ERROR: Pearl not equal");

                var DelPearl2 = await _repo.ReadAsync(DelPearl1.PearlID);
                if (DelPearl2 != null)
                    Console.WriteLine("ERROR: Pearl not removed");
                else
                    Console.WriteLine("Pearl confirmed removed from Db");

                Console.WriteLine("\nTesting CreateAsync()");
                Console.WriteLine("--------------------");
               
                
                var NewPearl1 = Pearl.Factory.CreateRandomPearl();

                Console.WriteLine($"Pearl created.\n{NewPearl1}");
                
           
               
                    Console.WriteLine("\nHalsbandet CRUD");
                Console.WriteLine("--------------------");


                    
                    Console.WriteLine("Testing ReadAllAsync()");
                    Console.WriteLine("--------------------");
                var AllNecklaces = await _repo.ReadAllAsync();// läser in alla
                    Console.WriteLine($"Nr of Necklace {AllNecklaces.Count()}");
                    Console.WriteLine($"\nFirst 5 Necklace");
                    AllNecklaces.Take(5).Print();


                Console.WriteLine("\nTesting ReadAsync()");
                Console.WriteLine("--------------------");
                var LastNecklacet1 = AllNecklaces.Last();
                var LastNecklace2 = await _repo.ReadAsync(LastNecklacet1.NecklaceID);
                Console.WriteLine($"Last Necklaces with Pearls.\n{LastNecklacet1}");
                Console.WriteLine($"Read Necklaces with NecklacesID == Last Necklaces\n{LastNecklace2}");
                if (LastNecklacet1 == LastNecklace2)
                    Console.WriteLine("Necklaces Equal");
                else
                    Console.WriteLine("ERROR: Necklaces not equal");

                Console.WriteLine($"Halsband 1 {LastNecklacet1}");
                Console.WriteLine($"Halsband 2 {LastNecklace2}");

                Console.WriteLine("\nTesting CreateAsync()");
                Console.WriteLine("--------------------");
                var NewNecklace1 = Necklace.Factory.CreateRandomNecklace(1);
                Console.WriteLine($"Necklace created.\n{NewNecklace1}");



                Console.WriteLine("\nTesting DeleteAsync()");

                var LastNecklacet1ToDelete = AllNecklaces.Last();
                var DelNecklacet1 = await _repo.DeleteAsync(LastNecklacet1ToDelete.NecklaceID);
                Console.WriteLine($"Necklace to delete.\n{LastNecklacet1ToDelete}");

                Console.WriteLine($"Deleted Necklace.\n{DelNecklacet1}");

                if (DelNecklacet1 != null && DelNecklacet1 == LastNecklacet1ToDelete)
                    Console.WriteLine("Necklace Equal");
                else
                    Console.WriteLine("ERROR: Necklace not equal");

                var DelNecklacet2 = await _repo.ReadAsync(DelNecklacet1.NecklaceID);
                if (DelNecklacet2 != null)
                    Console.WriteLine("ERROR: Necklace not removed");
                else
                    Console.WriteLine("Necklace confirmed removed from Db");





            }
        }

    }
}