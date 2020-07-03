using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StorageChallenge.Models
{
    public class BlobTestResult
    {
        public List<string> PrivateBlobs { get; set; }
        public List<string> PublicBlobs { get; set; }
        public string Status { get; set; }
        public bool Passed { get; set; }
        public string SAS { get; set; }
        public bool Ignore { get; set; }
        public BlobTestResult()
        {
            PrivateBlobs = new List<string>();
            PublicBlobs = new List<string>();
            Ignore = true;
        }
        public BlobTestResult(BlobTestResult privateResult, BlobTestResult publicResult)
        {
            this.Ignore = false;
            this.PrivateBlobs = new List<string>(privateResult.PrivateBlobs);
            this.PublicBlobs = new List<string>(publicResult.PublicBlobs);
            this.SAS = privateResult.SAS;
            if (publicResult.Passed && privateResult.Passed)
            {
                this.Status = "You have correctly configured the public and private blob containers.  You can test the links below to confirm:";
                this.Passed = true;
            }
            else if (publicResult.Passed)
            {
                this.Status = "You have properly configured the public blob container but not the private blob container." + privateResult.Status;
                this.Passed = false;
            }
            else if (privateResult.Passed)
            {
                this.Status = "You have properly configured the private blob container but not the public blob container." + publicResult.Status;
                this.Passed = false;
            }
            else
            {
                this.Status = "There are errors with the configuration of both the public blob container and the private blob container.  The issue may be with either the storage account or the storage account key.";
                this.Passed = false;
            }
        }

    }

}