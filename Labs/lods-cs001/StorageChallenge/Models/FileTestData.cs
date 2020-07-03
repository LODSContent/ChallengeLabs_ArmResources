using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorageChallenge.Models
{
    public class FileTestData
    {
        [Display(Name = "Storage account name")]
        public string storageAccountName { get; set; }
        [Display(Name = "Storage account key")]
        public string storageAccountKey { get; set; }
        [Display(Name = "Shared access signature")]
        public string storageAccountSAS { get; set; }
        public bool Advanced { get; set; }
    }
}