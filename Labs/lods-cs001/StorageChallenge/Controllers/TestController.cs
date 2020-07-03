using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using StorageChallenge.Models;
using StorageChallenge.Testing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;

namespace StorageChallenge.Controllers
{
    public class TestController : ApiController
    {
        private TestProcessor test = new TestProcessor();
        private FileTestData fileTestData
        {
            get
            {
                return new FileTestData
                {
                    Advanced = true,
                    storageAccountKey = CloudConfigurationManager.GetSetting("StorageAccountKey"),
                    storageAccountName = CloudConfigurationManager.GetSetting("StorageAccountName"),
                    storageAccountSAS = CloudConfigurationManager.GetSetting("StorageAccountSAS")
                };
            }
        }
        private SQLTestData sqlTestData
        {
            get
            {
                return new SQLTestData
                {
                    Advanced = true,
                    MySQLConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString,
                    SQLConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CorpDataConnectionString"].ConnectionString
                };
            }
        }
        private CosmosTestData cosmosTestData
        {
            get
            {
                return new CosmosTestData
                {
                    Advanced = true,
                    CosmosDBKey = CloudConfigurationManager.GetSetting("ListingsKey"),
                    CosmosDBUri = CloudConfigurationManager.GetSetting("ListingsURI"),
                    SearchKey = CloudConfigurationManager.GetSetting("SearchKey"),
                    SearchName = CloudConfigurationManager.GetSetting("SearchAccount"),

                };
            }
        }


        [HttpGet]
        public TestResult PublicStorage()
        {
            var t = test.TestPublicBlob(this.fileTestData);
            var result = new TestResult { Passed = t.Passed, Status = t.Status };
            return result;
        }
        [HttpGet]
        public TestResult PrivateStorage()
        {
            var t = test.TestPrivateBlob(this.fileTestData);
            var result = new TestResult { Passed = t.Passed, Status = t.Status };
            return result;
        }
        [HttpGet]
        public TestResult SQLData()
        {
            var t = test.TestSQLServer(this.sqlTestData);
            var result = new TestResult { Passed = t.Passed, Status = t.Status };
            return result;
        }
        [HttpGet]
        public TestResult MySQLData()
        {
            var t = test.TestMySQL(this.sqlTestData);
            var result = new TestResult { Passed = t.Passed, Status = t.Status };
            return result;
        }
        [HttpGet]
        public TestResult CosmosDB()
        {
            var t = test.TestCosmos(this.cosmosTestData);
            var result = new TestResult { Passed = t.Passed, Status = t.Status };
            return result;
        }
        [HttpGet]
        public TestResult Search()
        {
            var t = test.TestSearch(this.cosmosTestData);
            var result = new TestResult { Passed = t.Passed, Status = t.Status };
            return result;
        }
        [HttpGet]
        public TestResult TestRun(bool passed)
        {
            var result = new TestResult() { Passed = passed, Status = passed ? "Congrats, you passed" : "Fail" };

            return result;
        }
        [HttpGet]
        public async Task<bool> LoadCosmosDB()
        {
            var result = true;
            try
            {
                var CosmosDBKey = CloudConfigurationManager.GetSetting("ListingsKey");
                var CosmosDBUri = CloudConfigurationManager.GetSetting("ListingsURI");
                var databaseName = "realEstate";
                var collectionName = "listings";
                var databaseID = databaseName;
                DocumentClient client = new DocumentClient(new Uri(CosmosDBUri), CosmosDBKey);
                var database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseID });
                DocumentCollection collectionSpec = new DocumentCollection
                {
                    Id = collectionName
                };
                DocumentCollection collection = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseID), collectionSpec, new RequestOptions { OfferThroughput = 400  });


                IQueryable<Listing> query = client.CreateDocumentQuery<Listing>(
    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
                if(query.Count<Listing>() == 0)
                {
                    string[] documents = { "https://lodschallenge.blob.core.windows.net/storagechallenges/Listing1.json",
    "https://lodschallenge.blob.core.windows.net/storagechallenges/Listing2.json",
    "https://lodschallenge.blob.core.windows.net/storagechallenges/Listing3.json"};
                    //Load Documents
                    foreach(string documentUri in documents)
                    {
                        await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri("realEstate", "listings"), getListing(documentUri));
                    }

                }


            }
            catch
            {
                result = false;
            }
            return result;

        }

        private Listing getListing(string documentUri)
        {
            Listing result = null;
            var request = HttpWebRequest.Create(documentUri);
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var data = reader.ReadToEnd();
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<Listing>(data);
                }
            }
                
            return result;
        }
    }
    public class TestResult
    {
        public bool Passed { get; set; }
        public string Status { get; set; }
    }
}
