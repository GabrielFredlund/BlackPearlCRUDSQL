using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NecklaceDB;
using NecklaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NecklaceRepository;
using System.IO;

namespace NecklaceApplication
{

    public class Program
    {


        private static DbContextOptionsBuilder<NecklaceDbContext> _optionsBuilder;
        static void Main(string[] args)
        {
            if (!BuildOptions())
                return; //Terminate if not build correctly

            //SeedDataBase();
            //QueryDatabaseAsync().Wait();
            //QueryDatabase_Linq();
            //QueryDatabase_DataModel_Linq();
            QueryDatabaseCRUD().Wait();

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

        #region Uncomment to seed and query the Database

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
        #endregion

        private static async Task QueryDatabaseCRUD()
        {
            Console.WriteLine("\n\nQuery Database CRUDE");
            Console.WriteLine("--------------------");





            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                Console.WriteLine("\nNecklace CRUD Async-Testing");
                Console.WriteLine("___________________________");


                var _repo = new NecklaceRepository.NecklaceRepository(db);

                Console.WriteLine("\nTesting CreateAsync-Necklace");
                Console.WriteLine("____________________________");
                var NewNecklace = Necklace.Factory.CreateRandomNecklace(25);
                await _repo.CreateAsync(NewNecklace);
                Console.WriteLine("\nNew Necklace added.");

                Console.WriteLine("\nTesting ReadAllAsync-Necklace");
                var AllNecklaces = await _repo.ReadAllAsync();// läser in alla
                Console.WriteLine($"Amount of Necklaces {AllNecklaces.Count()}");
                Console.WriteLine($"\nFirst 5 Necklaces");
                var allNecklaces = AllNecklaces.Take(5).ToList();
                
                foreach (var n in allNecklaces)
                {
                    
                    Console.WriteLine($"Necklace with ID: {n.NecklaceID}");
                }

               

                Console.WriteLine("\nTesting ReadAsync-Necklace");
                var LastNecklace1 = AllNecklaces.Last();
                var LastNecklace2 = await _repo.ReadAsync(LastNecklace1.NecklaceID);
                Console.WriteLine($"Latest Necklace: \n {LastNecklace1.NecklaceID}");
                Console.WriteLine($"Read Necklace with NecklaceID == Last Necklace \n{LastNecklace2.NecklaceID}");
                if (LastNecklace1 == LastNecklace2)
                    Console.WriteLine("Necklaces are equal");
                else
                    Console.WriteLine("Error: Necklaces are not equal.");


                Console.WriteLine("\nTesting DeleteAsync-Necklace");
                Console.WriteLine("____________________________");
                var LastNecklacet1ToDelete = AllNecklaces.Last();
                var DelNecklacet1 = await _repo.DeleteAsync(LastNecklacet1ToDelete.NecklaceID);
                Console.WriteLine($"Necklace to delete.\n{LastNecklacet1ToDelete.NecklaceID}");

                Console.WriteLine($"Necklace was deleted.");
                Console.WriteLine("______________________");


                if (DelNecklacet1 != null && DelNecklacet1 == LastNecklacet1ToDelete)
                    Console.WriteLine("Necklace Equal");
                else
                    Console.WriteLine("ERROR: Necklace not equal");

                var DelNecklacet2 = await _repo.ReadAsync(LastNecklacet1ToDelete.NecklaceID);
                if (DelNecklacet2 != null)
                    Console.WriteLine("ERROR: Necklace not removed");
                else
                    Console.WriteLine("Necklace confirmed removed from Db");



            }

            using (var db = new NecklaceDbContext(_optionsBuilder.Options))
            {
                var _repo = new NecklaceRepository.PearlRepository(db);

                Console.WriteLine("\nPearl CRUD Async-Testing");
                Console.WriteLine("________________________");


                Console.WriteLine("\nTesting CreateAsync-Pearl");
                Console.WriteLine("--------------------");
                var NewPearl1 = Pearl.Factory.CreateRandomPearl();
                Console.WriteLine($"Pearl created.\n{NewPearl1}");



                Console.WriteLine("\nTesting ReadAllAsync-Pearl");
                var AllPearls = await _repo.ReadAllAsync();// läser in alla
                Console.WriteLine($"Amount of Pearls {AllPearls.Count()}");
                Console.WriteLine($"\nFirst 5 Pearls");
                var allPearls = AllPearls.Take(5).ToList();
               
                foreach (var n in allPearls)
                {
                    Console.WriteLine(n);
                }

                Console.WriteLine("\nTesting ReadAsync-Pearl");
                var LastPearl1 = AllPearls.Last();
                var LastPearl2 = await _repo.ReadAsync(LastPearl1.PearlID);
                Console.WriteLine($"\nLatest Pearl: \n {LastPearl1}");
                Console.WriteLine($"\nRead Pearl with PearlID == Last Necklace \n{LastPearl2}");
                if (LastPearl1 == LastPearl2)
                    Console.WriteLine("Pearls are equal");
                else
                    Console.WriteLine("Error: Pearls are not equal.");


                Console.WriteLine("\nTesting UpdateAsync-Pearl");
                LastPearl2.Size = 15;
                var LastPearl3 = await _repo.UpdateAsync(LastPearl2);
                Console.WriteLine($"Last Necklace with updated ID Value \n{LastPearl2.Size} == {LastPearl3.Size}");
                if (LastPearl2.Size == LastPearl3.Size)
                {
                    Console.WriteLine("Pearl updated");
                    LastPearl3.Size = 25;

                    LastPearl3 = await _repo.UpdateAsync(LastPearl3);
                    Console.WriteLine($"Last Pearl with updated sizes \n{LastPearl3}");

                }
                else
                    Console.WriteLine("Error: Pearl is not updated");

                Console.WriteLine("\nTesting DeleteAsync-Pearl");
                Console.WriteLine("___________________");
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


            }



        }

    }
}
