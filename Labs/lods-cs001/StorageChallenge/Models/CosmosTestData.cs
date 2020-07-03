using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorageChallenge.Models
{
    public class CosmosTestData
    {
        [Display(Name = "Cosmos DB Uri")]
        public string CosmosDBUri { get; set; }
        [Display(Name = "Cosmos DB Key")]
        public string CosmosDBKey { get; set; }
        [Display(Name = "Azure Search name")]
        public string SearchName { get; set; }
        [Display(Name = "Azure Search key")]
        public string SearchKey { get; set; }
        [Display(Name = "Search string")]
        public string SearchString { get; set; }
        public bool Advanced { get; set; }
    }

    public class Listing
    {
        public string PropertyID { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
    public class NoSQLTestResult
    {
        public List<Listing> AllListings { get; set; }
        public List<Listing> SearchResults { get; set; }
        public string Status { get; set; }
        public bool Passed { get; set; }
        public bool Ignore { get; set; }
        public NoSQLTestResult()
        {
            AllListings = new List<Listing>();
            SearchResults = new List<Listing>();
            Passed = false;
            Ignore = false;
        }
        public NoSQLTestResult(NoSQLTestResult cosmosResults, NoSQLTestResult searchResults)
        {
            this.Passed = cosmosResults.Passed && searchResults.Passed;
            this.Ignore = false;
            this.AllListings = new List<Listing>(cosmosResults.AllListings);
            this.SearchResults = new List<Listing>(searchResults.SearchResults);
            if (this.Passed)
            {
                this.Status = "You have successfully configured Cosmos DB and Azure Search.";
            }
            else if (!cosmosResults.Passed && !searchResults.Passed)
            {
                this.Status = $"There are issues with the Cosmos DB and Azure Search configurations.  Cosmos DB: {cosmosResults.Status}; Search: {searchResults.Status}";
            }
            else if (!cosmosResults.Passed)
            {
                this.Status = $"There are issues with the Cosmos DB configuration: {cosmosResults.Status}";
            }
            else
            {
                this.Status = $"There are issues with the search configuration: {searchResults.Status}";
            }

        }
    }
}