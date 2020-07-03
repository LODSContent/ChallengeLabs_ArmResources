using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorageChallenge.Models
{
    public class Customer
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class SalesPerson
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }


    public class SQLTestData
    {
        [Display(Name = "SQL Server connection string")]
        public string SQLConnection { get; set; }
        [Display(Name = "MySQL connection string")]
        public string MySQLConnection { get; set; }
        public bool Advanced { get; set; }

    }
    public class DataTestResult
    {
        public List<Customer> Customers { get; set; }
        public List<SalesPerson> SalesPeople { get; set; }
        public string Status { get; set; }
        public bool Passed { get; set; }
        public bool Ignore { get; set; }
        public DataTestResult()
        {
            Customers = new List<Customer>();
            SalesPeople = new List<SalesPerson>();
            Ignore = true;
        }
        public DataTestResult(DataTestResult SQLResult, DataTestResult MySQLResult)
        {
            this.Passed = SQLResult.Passed && MySQLResult.Passed;
            this.Ignore = false;
            this.Customers = new List<Customer>(MySQLResult.Customers);
            this.SalesPeople = new List<SalesPerson>(SQLResult.SalesPeople);
            if(this.Passed)
            {
                this.Status = "You have configured both SQL Server and MySQL correctly.";
            } else if (!SQLResult.Passed && !MySQLResult.Passed)
            {
                this.Status = $"Both SQL Server and MySQL are incorrectly configured.  SQL message: {SQLResult.Status}. MySQL message: {MySQLResult.Status}";
            } else if (!SQLResult.Passed)
            {
                this.Status = $"You have correctly configured MySQL, but have incorrectly configured SQL Server: {SQLResult.Status}";
            } else
            {
                this.Status = $"You have correctly configured SQL Server, but have incorrectly configured MySQL: {MySQLResult.Status}";
            }

        }
    }
}