using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StorageChallenge.Models;

namespace StorageChallenge.Testing
{
    public class TestType
    {
        private static int testValue;
        static TestType()
        {
            testValue = int.Parse(CloudConfigurationManager.GetSetting("TestType"));
        }
        public static bool TestPublicStorage { get { return (testValue & (int)TestTypeEnum.PublicStorage) > 0; } }
        public static bool TestPrivateStorage { get { return (testValue & (int)TestTypeEnum.PrivateStorage) > 0; } }
        public static bool TestSQLServer { get { return (testValue & (int)TestTypeEnum.SQLServer) > 0; } }
        public static bool TestMySQL { get { return (testValue & (int)TestTypeEnum.MySQL) > 0; } }
        public static bool TestCosmosDB { get { return (testValue & (int)TestTypeEnum.CosmosDB) > 0; } }
        public static bool TestSearch { get { return (testValue & (int)TestTypeEnum.Search) > 0; } }
        public static bool TestStorage { get { return (testValue & (int)(TestTypeEnum.PublicStorage | TestTypeEnum.PrivateStorage)) > 0; } }
        public static bool TestRelational { get { return (testValue & (int)(TestTypeEnum.SQLServer | TestTypeEnum.MySQL)) > 0; } }
        public static bool TestNoSQL { get { return (testValue & (int)(TestTypeEnum.CosmosDB | TestTypeEnum.Search)) > 0; } }

    }
    public enum TestTypeEnum : int
    {
        PublicStorage = 1,
        PrivateStorage = 2,
        SQLServer = 4,
        MySQL = 8,
        CosmosDB = 16,
        Search = 32
    }
 }