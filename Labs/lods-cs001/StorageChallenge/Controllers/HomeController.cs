using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StorageChallenge.Models;
using StorageChallenge.Testing;
using Microsoft.Azure;

namespace StorageChallenge.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Files()
        {
            ViewBag.Result = new BlobTestResult();
            var fd = new FileTestData {Advanced = CloudConfigurationManager.GetSetting("Advanced") == "true" };

            if (fd.Advanced)
            {
                fd.storageAccountName = CloudConfigurationManager.GetSetting("StorageAccountName");
                fd.storageAccountKey = CloudConfigurationManager.GetSetting("StorageAccountKey");
                fd.storageAccountSAS = CloudConfigurationManager.GetSetting("StorageAccountSAS");
            }
            return View(fd);
        }
        [HttpPost]
        public ActionResult Files(FileTestData data)
        {
            var test = new TestProcessor();
            BlobTestResult result = null;
            if(TestType.TestPublicStorage && TestType.TestPrivateStorage)
            {
                var privateResult = test.TestPrivateBlob(data);
                var publicResult = test.TestPublicBlob(data);
                result = new BlobTestResult(privateResult,publicResult);
            }
            else if(TestType.TestPublicStorage)
            {
                result = test.TestPublicBlob(data);
            }
            else
            {
                result = test.TestPrivateBlob(data);
            }
            ViewBag.Result = result;
            return View(data);
        }
        [HttpGet]
        public ActionResult Database()
        {
            SQLTestData sd = new SQLTestData() {Advanced = CloudConfigurationManager.GetSetting("Advanced") == "true" };
            if(sd.Advanced)
            {
                sd.SQLConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CorpDataConnectionString"].ConnectionString;
                sd.MySQLConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString;
            }
            ViewBag.Result = new DataTestResult();
            return View(sd);
        }
        [HttpPost]
        public ActionResult Database(SQLTestData data)
        {

            var test = new TestProcessor();
            DataTestResult result = new DataTestResult();
            if (TestType.TestSQLServer && TestType.TestMySQL)
            {
                var sqlResult = test.TestSQLServer(data);
                var mySqlResult = test.TestMySQL(data);
                result = new DataTestResult(sqlResult, mySqlResult);
            }
            else if (TestType.TestSQLServer)
            {
                result = test.TestSQLServer(data);
            }
            else
            {
                result = test.TestMySQL(data);
            }
            result.Ignore = false;
            ViewBag.Result = result;
            return View(data);
        }

        [HttpGet]
        public ActionResult NoSQL()
        {
            CosmosTestData cd = new CosmosTestData() { Advanced = CloudConfigurationManager.GetSetting("Advanced") == "true" };
            if (cd.Advanced)
            {
                cd.CosmosDBUri = CloudConfigurationManager.GetSetting("ListingsURI");
                cd.CosmosDBKey = CloudConfigurationManager.GetSetting("ListingsKey");
                cd.SearchName = CloudConfigurationManager.GetSetting("SearchAccount");
                cd.SearchKey = CloudConfigurationManager.GetSetting("SearchKey");
            }
            ViewBag.Result = new DataTestResult() { Ignore = true };
            return View(cd);
        }

        [HttpPost]
        public ActionResult NoSQL(CosmosTestData data)
        {
            var test = new TestProcessor();
            NoSQLTestResult result = new NoSQLTestResult() { Ignore = false };
            if (TestType.TestCosmosDB && TestType.TestSearch)
            {
                var cosmosDBResult = test.TestCosmos(data);
                var searchResult = test.TestSearch(data);
                result = new NoSQLTestResult(cosmosDBResult, searchResult);
            }
            else if (TestType.TestCosmosDB)
            {
                result = test.TestCosmos(data);
            }
            else
            {
                result = test.TestSearch(data);
            }
            result.Ignore = false;
            ViewBag.Result = result;
            return View(data);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }


}