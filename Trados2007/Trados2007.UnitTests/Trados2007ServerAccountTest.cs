namespace Trados2007.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sdl.TranslationStudio.Plugins.Trados2007;
    using System;

    [TestClass()]
    public class Trados2007ServerAccountTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Trados2007ServerAccount_Constructor_ArgNullServer()
        {
            string translationServer = null;
            string login = "Andy";
            string password = "Andy";
            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.Fail("Should not pass");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Trados2007ServerAccount_Constructor_ArgNullLogin()
        {
            string translationServer = "10.109.4.4";
            string login = string.Empty;
            string password = "Andy";
            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.Fail("Should not pass");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Trados2007ServerAccount_Constructor_ArgNullPassword()
        {
            string translationServer = "10.109.5.114";
            string login = "Andy";
            string password = string.Empty;
            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.Fail("Should not pass");
        }

        [TestMethod()]
        public void Trados2007ServerAccount_Constructor_OKForValidIp()
        {
            string translationServer = @"10.27.100.148";
            string login = "Andy";
            string password = "Andy";

            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.IsTrue(target.IsServerUp);
        }

        [TestMethod()]
        public void Trados2007ServerAccount_Constructor_OKForInvalidIp()
        {
            string translationServer = @"10.207.100.148";
            string login = "Andy";
            string password = "Andy";

            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.IsFalse(target.IsServerUp);
        }

        [TestMethod()]
        public void Trados2007ServerAccount_Constructor_OKForValidHostname()
        {
            string translationServer = @"VQ01EN-S3E-08";
            string login = "Andy";
            string password = "Andy";

            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.IsTrue(target.IsServerUp);
        }

        [TestMethod()]
        public void Trados2007ServerAccount_Constructor_OKForInvalidHostname()
        {
            string translationServer = @"SOMEIDIOTICNAME";
            string login = "Andy";
            string password = "Andy";

            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.Fail("Should not pass");
        }
        
        [TestMethod()]
        [ExpectedException(typeof(TypeInitializationException))]
        public void Trados2007ServerAccount_Constructor_OKForValidHostnameWithSuffixes()
        {
            string translationServer = @"VQ01EN-S3E-08.development.sheffield.sdl.corp";
            string login = "Andy";
            string password = "Andy";

            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            Assert.IsTrue(target.IsServerUp);
        }

        [TestMethod()]
        public void Trados2007ServerAccount_GetTMs_NotEmptyList()
        {
            string translationServer = @"10.27.100.148";
            string login = "Andy";
            string password = "Andy";
            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);
            
            var result = target.GetTranslationMemories();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            // minor checks for validness
            Assert.IsTrue(result[0].SupportsSourceConcordanceSearch);
            Assert.IsNotNull(result[0].Uri);
            Assert.IsNotNull(result[0].LanguageDirection);
        }

        [TestMethod()]
        public void Trados2007ServerAccount_TMExists_OKForNonExisting()
        {
            string translationServer = @"10.27.100.148";
            string login = "Andy";
            string password = "Andy";
            Trados2007ServerAccount target = new Trados2007ServerAccount(translationServer, login, password);

            var result = target.CheckTranslationMemoryExists("some imaginary non-existing name");

            Assert.IsFalse((bool)result);
        }
    }
}
