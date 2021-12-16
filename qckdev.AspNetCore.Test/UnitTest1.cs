using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace qckdev.AspNetCore.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Url", "%baseurl%/api/" },
                { "DefaultConnection", "Server=*****;Database=*****;User Id=%SQLUSR%;Password=%SQLPWD%;MultipleActiveResultSets=true;" },
                { "Fake1", null },
                { "Fake2", "" },
                { "Fake3", "a" },
                { "Fake4", "prueba" },
                { "Fake5", "%" },
                { "Fake6", "User Id:%SQLUSR%;Password=%SQLPWD%;Other=%SQLOTHER%;" },
                { "BASEURL", "https://midns-dev.midominio.com" },
                { "SQLUSR", "myuser" },
                { "SQLPWD", "mypwd" }
            };
            var expected = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Url", "https://midns-dev.midominio.com/api/" },
                { "DefaultConnection", "Server=*****;Database=*****;User Id=myuser;Password=mypwd;MultipleActiveResultSets=true;" },
                { "Fake1", null },
                { "Fake2", "" },
                { "Fake3", "a" },
                { "Fake4", "prueba" },
                { "Fake5", "%" },
                { "Fake6", "User Id:myuser;Password=mypwd;Other=%SQLOTHER%;" },
                { "BASEURL", "https://midns-dev.midominio.com" },
                { "SQLUSR", "myuser" },
                { "SQLPWD", "mypwd" }
            };

            ConfigurationHelper.ApplyEnvironmentVariables(dictionary, (key, value) => dictionary[key] = value);
            foreach (var pair in dictionary)
            {
                Assert.AreEqual(expected[pair.Key], pair.Value, $"Key: {pair.Key}");
            }
        }
    }
}
