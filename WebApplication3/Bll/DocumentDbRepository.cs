using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication3.Models;

namespace WebApplication3.Bll
{
    public static class DocumentDbRepository
    {
        private static Database database;
        private static DocumentCollection collection;

        public static Database Database
        {
            get
            {
                if (database != null)
                    return database;

                database = ReadOrCreateDatabase();
                return database;
            }
        }

        public static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                    collection = ReadOrCreateCollection(Database.SelfLink);

                return collection;
            }
        }


        private static DocumentClient client
            = new DocumentClient(new Uri("https://szerda.documents.azure.com:443/"), "HOyhplXqMALt7nHyznAC0Oyba8pLm3LOPeJ4lNzXQnuQAIN2kY1gjFTEPYDq9UKAYqntcIIxjIPC0ONhvxnqpQ==");


        private static Database ReadOrCreateDatabase()
        {
            var db = client.CreateDatabaseQuery()
                            .AsEnumerable().FirstOrDefault(d => d.Id == "TodoDatabase");

            if (db != null)
                return db;

            db = client.CreateDatabaseAsync(new Database { Id = "TodoDatabase" }).Result;
            return db;
        }

        private static DocumentCollection ReadOrCreateCollection(string dbLink)
        {
            var coll = client.CreateDocumentCollectionQuery(dbLink)
                          .AsEnumerable().FirstOrDefault(c => c.Id == "TodoItems");

            if (coll != null)
                return coll;

            coll = client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = "TodoItems" }).Result;
            return coll;
        }

        public static async Task CreateItemAsync(Item item)
        {
            await client.CreateDocumentAsync(Collection.SelfLink, item);
        }

        public static IEnumerable<Item> GetIncompleteItems()
        {
            return
                client.CreateDocumentQuery<Item>(Collection.DocumentsLink) //SELECT * FROM TodoItems
                       .AsEnumerable().Where(i => i.Completed == false).AsEnumerable().ToList();
        }

        // Egy elem lekérdezése id alapján
        // Hasonló a szűréshez; ez is LInQ-et használ a CreateDocumentQuery hívás eredményén; Collection
        public static Item GetItem(string id)
        {
            return client.CreateDocumentQuery<Item>(Collection.DocumentsLink)
                .Where(i => i.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }

        // Update: lekérdezi az elemet id alapján (ugyanúgy, mint a GetItem)
        // ezután a ReplaceDocumentAsync hívással felülcsapja az újjal
        // .SelfLink adja meg, hogy melyik dokumentumot kell felülírni
        public async static Task UpdateItemAsync(string id, Item item)
        {
            var original = client.CreateDocumentQuery(Collection.DocumentsLink)
               .Where(i => i.Id == id)
               .AsEnumerable()
               .FirstOrDefault();
            await client.ReplaceDocumentAsync(original.SelfLink, item);
        }
    }
}