using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using System.IO;

namespace TMProvider
{
    public class CertificateStore
    {
        // stores the thumbprints of the certificates on the memoQ servers
        private Dictionary<string, CertificateData> certificates = new Dictionary<string, CertificateData>();
        public string IsThumbprintStored(string serverName, string thumbprint)
        {
            CertificateData t;
            // no thumbprint for that server
            if (!certificates.TryGetValue(serverName.ToLower(), out t)) return null;
            // thumbprints don't match
            else if (String.Compare(thumbprint, t.Thumbprint, true) == 0) return t.Thumbprint;
            else return null;
        }

        public bool ProviderHasStoredCertificate(string providername)
        { 
            if(certificates.ContainsKey(providername.ToLower())) return true;
            else return false;
        }

        /// <summary>
        /// Adds a thumbprint to the store, and saves it to the file.
        /// </summary>
        /// <param name="thumbprint"></param>
        public void StoreThumbprint(string servername, string thumbprint)
        {
            AddCertificateData(new CertificateData(servername, thumbprint));
            Save();
        }

        /// <summary>
        /// Deletes the certificate from the store (for example the server's certificate changed to a new, valid one).
        /// </summary>
        /// <param name="servername"></param>
        public void DeleteThumbprint(string servername)
        { 
            string s = servername.ToLower();
            if(certificates.ContainsKey(s)) certificates.Remove(s);
        }

        /// <summary>
        /// Just adds a thumbprint to the store, but doesn't save it.
        /// </summary>
        /// <param name="thumbprint"></param>
        public void AddCertificateData(CertificateData certData)
        {
            // one certificate for one server only
            string s = certData.ServerName.ToLower();
            if (certificates.ContainsKey(s)) certificates[s] = certData;
            else certificates.Add(s, certData);
        }

        #region Singleton management and serialization

        // all the settings are in the same file, in an xml element

        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static CertificateStore instance = null;

        /// <summary>
        /// Lock object for thread-safe singleton initialization.
        /// </summary>
        private static object classSyncRoot = new object();

        /// <summary>
        /// Gets the singleton settings instance.
        /// </summary>
        public static CertificateStore Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (classSyncRoot)
                    {
                        if (instance == null)
                            instance = createInstance();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Creates a new object instance.
        /// </summary>
        private static CertificateStore createInstance()
        {
            var settings = load();
            if (settings == null)
                settings = new CertificateStore();
            return settings;
        }

        /// <summary>
        /// DO NOT USE: Public ctor is needed for XML serialization
        /// </summary>
        public CertificateStore()
        {
            // Nothing to do here.
        }

        /// <summary>
        /// Loads data from XML.
        /// </summary>
        private static CertificateStore load()
        {
            XDocument doc;
            try
            {
                // open file and read the certificates section
                doc = XDocument.Load(AppData.ConfigPath);
            }
            catch
            {
                return new CertificateStore();
            }
            IEnumerable<XElement> certElements = doc.Descendants("Certificates");

            if (certElements == null || certElements.Count() == 0) return new CertificateStore();

            XElement certElement = certElements.ElementAt(0);

            CertificateStore store = new CertificateStore();
            foreach (XElement element in certElement.Elements())
            {
                CertificateData c = element.FromXElement<CertificateData>();
                store.AddCertificateData(c);
            }
            return store;

        }

        /// <summary>
        /// Saves the settings object to XML.
        /// </summary>
        public void Save()
        {
            // delete the certificates section and save the thumbprints
            XDocument doc;
            try
            {
                if (!File.Exists(AppData.ConfigPath)) doc = new XDocument(new XElement("memoQTMPluginSettings"));
                else doc = XDocument.Load(AppData.ConfigPath);
            }
            catch (Exception ex)
            {
                doc = new XDocument(new XElement("memoQTMPluginSettings"));
            }

            IEnumerable<XElement> certElements = doc.Descendants("Certificates");

            // remove existing element
            if (certElements != null && certElements.Count() != 0)
            {
                doc.Descendants("Certificates").Remove();
            }

            // add the thumbprints
            XElement certs = new XElement("Certificates");
            foreach (KeyValuePair<string, CertificateData> kvp in certificates)
            {
                certs.Add(kvp.Value.ToXElement<CertificateData>());
            }

            doc.Root.Add(certs);

            if (Directory.Exists(Path.GetDirectoryName(AppData.ConfigPath))) doc.Save(AppData.ConfigPath);
        }

        #endregion
    }

    public class CertificateData
    {
        public string ServerName { get; set; }
        public string Thumbprint { get; set; }

        public CertificateData()
        {
            // for serialization
        }

        public CertificateData(string server, string thumbprint)
        {
            this.ServerName = server;
            this.Thumbprint = thumbprint;
        }
    }
}
