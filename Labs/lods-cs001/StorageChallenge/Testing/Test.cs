using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StorageChallenge.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace StorageChallenge.Testing
{
    public class TestProcessor
    {
        System.Web.Caching.Cache cache = null;
        public TestProcessor()
        {
            cache = System.Web.HttpContext.Current.Cache;
        }
        public BlobTestResult TestPublicBlob(FileTestData data)
        {
            var result = new BlobTestResult { Passed = false, Ignore = false };
            try
            {
                var account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(data.storageAccountName, data.storageAccountKey), true);
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("public");
                if (container.Exists())
                {
                    if (container.GetPermissions().PublicAccess == BlobContainerPublicAccessType.Blob)
                    {
                        foreach (var blob in container.ListBlobs())
                        {
                            result.PublicBlobs.Add(blob.Uri.AbsoluteUri);
                        }
                        if (result.PublicBlobs.Count > 0)
                        {

                            result.Status = "You have completed this challenge successfully.  You can test the links below:";
                            result.Passed = true;
                        }
                        else
                        {
                            result.Status = "You have configured the storage container correctly, but there are no blobs in the container.";
                        }
                    }
                    else
                    {
                        result.Status = "Your container does not have the correct security setting.";
                    }
                }
                else
                {
                    result.Status = "Public container does not exist.";
                }
            }
            catch
            {
                result.Status = "Invalid storage account or key.";

            }
            setTestStatus(result, TestTypeEnum.PublicStorage);
            return result;

        }



        public BlobTestResult TestPrivateBlob(FileTestData data)
        {
            //Normalize the SAS - CLI and PowerShell differ.
            data.storageAccountSAS = data.storageAccountSAS.Replace("?", "");
            var result = new BlobTestResult { Passed = false, Ignore = false, SAS = data.storageAccountSAS };
            try
            {
                var account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(data.storageAccountName, data.storageAccountKey), true);
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("private");
                //var metaDataExists = false;
                if (container.Exists())
                {
                    if (container.GetPermissions().PublicAccess == BlobContainerPublicAccessType.Off)
                    {
                        foreach (var blob in container.ListBlobs(blobListingDetails: BlobListingDetails.Metadata))
                        {
                            result.PrivateBlobs.Add(blob.Uri.AbsoluteUri);
                          //Add test for metadata  
                            
                        }
                        if (result.PrivateBlobs.Count > 0)
                        {
                            //Test SAS
                            bool SASpermission = false;
                            bool SASobject = false;
                            foreach (var SASProp in data.storageAccountSAS.Split("&".ToCharArray()))
                            {
                                //Reference SAS from CLI - sp=r&sv=2017-07-29&sr=c&sig=jfqVykBTpWib53o65r4UO1irdJWosR3bAQac1tocC2Q%3D
                                var pair = SASProp.Split("=".ToCharArray());
                                if (pair.Count() == 2)
                                {
                                    if (pair[0] == "sp") { SASpermission = pair[1] == "r"; }
                                    if (pair[0] == "sr") { SASobject = pair[1] == "c"; }
                                }
                            }
                            if (SASpermission && SASobject)
                            {
                               
                                var url = result.PrivateBlobs[0] + "?" + data.storageAccountSAS;
                                var rqst = WebRequest.CreateHttp(url);
                                try
                                {
                                    using (var resp = rqst.GetResponse())
                                    {
                                        if (resp.ContentLength > 0)
                                        {
                                            result.Passed = true;
                                            result.Status = "You have successfully created and populated a private blob container, and generated a valid SAS token for the container.";
                                        }
                                        else
                                        {
                                            result.Passed = false;
                                            result.Status = "There is a problem with your SAS. It has the correct format but did not provide read access to the blobs in the private container.";

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    result.Passed = false;
                                    result.Status = "There is a problem with your SAS. An error was returned when trying to retrieve a file form the private blob container using the token. " + ex.Message;
                                }
                            }

                        }
                        else
                        {
                            result.Status = "You have configured the storage container correctly, but there are no blobs in the container.";
                        }
                    }
                    else
                    {
                        result.Status = "Your container does not have the correct security setting.";
                    }
                }
                else
                {
                    result.Status = "Private container does not exist.";
                }
            }
            catch
            {
                result.Status = "Invalid storage account or key.";

            }
            setTestStatus(result, TestTypeEnum.PrivateStorage);
            return result;

        }

        public DataTestResult TestSQLServer(SQLTestData data)
        {
            DataTestResult result = new DataTestResult();
            try
            {
                using (var conn = new SqlConnection(data.SQLConnection))
                {
                    var SQL = "SELECT * FROM Sales.SalesPerson;";
                    var cmd = new SqlCommand(SQL, conn);
                    conn.Open();
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        result.SalesPeople.Add(new SalesPerson { ID = (int)rdr["ID"], FirstName = rdr["FirstName"].ToString(), LastName = rdr["LastName"].ToString(), Phone = rdr["Phone"].ToString() });
                    }
                    if(result.SalesPeople.Count==0)
                    {
                        result.Passed = false;
                        result.Status = "The connection to SQL Server worked, but there was no data";
                    } else
                    {
                        result.Passed = true;
                        result.Status = "Passed all SQL Server tests.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Status = $"Encountered an error: {ex.Message}";
            }


            return result;
        }

        public DataTestResult TestMySQL(SQLTestData data)
        {
            DataTestResult result = new DataTestResult();
            try
            {
                MySqlConnection conn = new MySqlConnection(data.MySQLConnection);
                string SQL = "SELECT * FROM customer;";
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                conn.Open();
                try
                {
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        result.Customers.Add(new Customer { id = (int)rdr["id"], name = rdr["name"].ToString() });
                    }
                    if (result.Customers.Count == 0)
                    {
                        result.Passed = false;
                        result.Status = "The connection to MySQL is correct but returned no data";
                    } else
                    {
                        result.Passed = true;
                        result.Status = "Passed all MySQL tests.";
                    }
                } finally
                {
                    conn.Close();
                }
            } catch(Exception ex)
            {
                result.Passed = false;
                result.Status = $"Encountered an error: {ex.Message}";
            }


            return result;

        }

        public NoSQLTestResult TestCosmos(CosmosTestData data)
        {
            var result = new NoSQLTestResult();
            try
            {
                
                DocumentClient client = new DocumentClient(new Uri(data.CosmosDBUri), data.CosmosDBKey);
                // Query using partition key
                IQueryable<Listing> query = client.CreateDocumentQuery<Listing>(
                    UriFactory.CreateDocumentCollectionUri("realEstate", "listings"))
                    .Where(m => m.State != "AZ");
                foreach (var listing in query)
                {
                    result.AllListings.Add(listing);
                }
                if(result.AllListings.Count>0)
                {
                    result.Passed = true;
                    result.Status = "Cosmos DB is properly configured.";
                } else
                {
                    result.Passed = false;
                    result.Status = "Cosmos DB does not have any qualifying data.";
                }
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Status = $"There was an error processing the Cosmos DB database: {ex.ToString()}";
            }

            return result;
        }

        public NoSQLTestResult TestSearch(CosmosTestData data)
        {
            var result = new NoSQLTestResult();
            try
            {
                string searchServiceName = data.SearchName;

                string queryApiKey = data.SearchKey;

                SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, "documentdb-index", new SearchCredentials(queryApiKey));
                SearchParameters parameters;
 

                parameters =
                    new SearchParameters()
                    {
                        Select = new[] { "PropertyID", "Street", "City", "State", "PostalCode", "Description", "Price" },
                        Filter = "Price lt 900000000000.00"
                    };

                DocumentSearchResult<Listing> results = indexClient.Documents.Search<Listing>("*", parameters);
                foreach(SearchResult<Listing> document in results.Results)
                {
                    result.SearchResults.Add(document.Document);
                }
                if (result.SearchResults.Count > 0)
                {
                    result.Passed = true;
                    result.Status = "Search is properly configured";
                } else
                {
                    result.Passed = false;
                    result.Status = "Search returned zero results";
                }
            } catch(Exception ex)
            {
                result.Status = $"Search encountered an error: {ex.Message}";
                result.Passed = false;
            }
            return result;
        }

        private void setTestStatus(BlobTestResult result, TestTypeEnum type)
        {
            if (result.Passed)
            {
                TestStatus = TestStatus | (int)type;
            }
            else
            {
                TestStatus = TestStatus & ~(int)type;

            }
        }
        private int TestStatus
        {
            get
            {
                int status = 0;
                if(cache["TestStatus"]!=null)
                {
                    status = (int)cache["TestStatus"];
                }
                return status;
            }
            set
            {
                cache["TestStatus"] = value;
            }
        }
    }

}