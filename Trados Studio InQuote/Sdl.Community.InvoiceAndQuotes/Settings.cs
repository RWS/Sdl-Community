using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes
{
    public static class Settings
    {
        public static String SettingsFile
        {
            get
            {
                String settingsFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"SDL\OpenExchange\Invoices And Quotes\InvoicesAndQuotes.xml");
                if (!Directory.Exists(Path.GetDirectoryName(settingsFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
                if (!File.Exists(settingsFilePath))
                {
                    using (StreamWriter sw = new StreamWriter(settingsFilePath, true))
                    {
                        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                        sw.WriteLine("<Settings></Settings>");
                    }
                }
                return settingsFilePath;
            }
        }
        private const string SettingsPath = "Settings";
        private const string LanguageCulturePath = "LanguageCulture";
        private const string DefaultUser = "DefaultUser";
        private const string DefaultCustomer = "DefaultCustomer";
        private const string RatesState = "RatesState";

        public static String GetXMLPath(String element)
        {
            return String.Format(@"//{0}", element);
        }

        public static void SaveSelectedCulture(String culture)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            XmlNode xmlNodeLanguageCuture = xmlNodeSettings.SelectSingleNode(GetXMLPath(LanguageCulturePath));
            if (xmlNodeLanguageCuture != null)
                xmlNodeLanguageCuture.InnerText = culture;
            else
            {
                xmlNodeLanguageCuture = xmlDoc.CreateElement(LanguageCulturePath);
                xmlNodeLanguageCuture.InnerText = culture;
                xmlNodeSettings.AppendChild(xmlNodeLanguageCuture);
            }

            xmlDoc.Save(SettingsFile);
        }

        public static String GetSavedCulture()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            XmlNode xmlNodeLanguageCuture = xmlNodeSettings.SelectSingleNode(GetXMLPath(LanguageCulturePath));
            return xmlNodeLanguageCuture == null ? "en-US" : xmlNodeLanguageCuture.InnerText;
        }

        public static void SaveCurrentUser(User user)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            if (xmlNodeSettings != null)
            {
                XmlNode xmlNodeDefaultUser = xmlNodeSettings.SelectSingleNode(GetXMLPath(DefaultUser));
                bool addElement = false;
                if (xmlNodeDefaultUser == null)
                {
                    xmlNodeDefaultUser = xmlDoc.CreateElement(DefaultUser);
                    addElement = true;
                }

                ((XmlElement)xmlNodeDefaultUser).SetAttribute("name", user.Name);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("street", user.Street);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("city", user.City);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("state", user.State);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("twitter", user.Twitter);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("webaddress", user.WebAddress);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("skype", user.Skype);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("email", user.Email);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("mobile", user.Mobile);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("phone", user.Phone);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("country", user.Country);
                ((XmlElement)xmlNodeDefaultUser).SetAttribute("zip", user.Zip);

                if (addElement)
                    xmlNodeSettings.AppendChild(xmlNodeDefaultUser);

                xmlDoc.Save(SettingsFile);
            }
        }

        public static User GetCurrentUser()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            if (xmlNodeSettings != null)
            {
                XmlNode xmlNodeDefaultUser = xmlNodeSettings.SelectSingleNode(GetXMLPath(DefaultUser));
                if (xmlNodeDefaultUser != null && xmlNodeDefaultUser.Attributes != null)
                {
                    User user = new User();

                    user.Name = xmlNodeDefaultUser.Attributes["name"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["name"].Value;
                    user.Street = xmlNodeDefaultUser.Attributes["street"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["street"].Value;
                    user.City = xmlNodeDefaultUser.Attributes["city"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["city"].Value;
                    user.State = xmlNodeDefaultUser.Attributes["state"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["state"].Value;
                    user.Zip = xmlNodeDefaultUser.Attributes["zip"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["zip"].Value;
                    user.Country = xmlNodeDefaultUser.Attributes["country"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["country"].Value;
                    user.Phone = xmlNodeDefaultUser.Attributes["phone"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["phone"].Value;
                    user.Mobile = xmlNodeDefaultUser.Attributes["mobile"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["mobile"].Value;
                    user.Email = xmlNodeDefaultUser.Attributes["email"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["email"].Value;
                    user.Skype = xmlNodeDefaultUser.Attributes["skype"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["skype"].Value;
                    user.WebAddress = xmlNodeDefaultUser.Attributes["webaddress"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["webaddress"].Value;
                    user.Twitter = xmlNodeDefaultUser.Attributes["twitter"] == null ? String.Empty : xmlNodeDefaultUser.Attributes["twitter"].Value;

                    return user;
                }
            }
            return null;    
        }

        public static void SaveCurrentCustomer(Customer customer)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            if (xmlNodeSettings != null)
            {
                XmlNode xmlNodeDefaultCustomer = xmlNodeSettings.SelectSingleNode(GetXMLPath(DefaultCustomer));
                bool addElement = false;
                if (xmlNodeDefaultCustomer == null)
                {
                    xmlNodeDefaultCustomer = xmlDoc.CreateElement(DefaultCustomer);
                    addElement = true;
                }

                ((XmlElement)xmlNodeDefaultCustomer).SetAttribute("name", customer.Name);
                ((XmlElement)xmlNodeDefaultCustomer).SetAttribute("street", customer.Street);
                ((XmlElement)xmlNodeDefaultCustomer).SetAttribute("city", customer.City);
                ((XmlElement)xmlNodeDefaultCustomer).SetAttribute("state", customer.State);
                ((XmlElement)xmlNodeDefaultCustomer).SetAttribute("country", customer.Country);
                ((XmlElement)xmlNodeDefaultCustomer).SetAttribute("zip", customer.Zip);

                if (addElement)
                    xmlNodeSettings.AppendChild(xmlNodeDefaultCustomer);

                xmlDoc.Save(SettingsFile);
            }
        }

        public static Customer GetCurrentCustomer()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            if (xmlNodeSettings != null)
            {
                XmlNode xmlNodeDefaultCustomer = xmlNodeSettings.SelectSingleNode(GetXMLPath(DefaultCustomer));
                if (xmlNodeDefaultCustomer != null && xmlNodeDefaultCustomer.Attributes != null)
                {
                    Customer customer = new Customer();

                    customer.Name = xmlNodeDefaultCustomer.Attributes["name"] == null ? String.Empty : xmlNodeDefaultCustomer.Attributes["name"].Value;
                    customer.Street = xmlNodeDefaultCustomer.Attributes["street"] == null ? String.Empty : xmlNodeDefaultCustomer.Attributes["street"].Value;
                    customer.City = xmlNodeDefaultCustomer.Attributes["city"] == null ? String.Empty : xmlNodeDefaultCustomer.Attributes["city"].Value;
                    customer.State = xmlNodeDefaultCustomer.Attributes["state"] == null ? String.Empty : xmlNodeDefaultCustomer.Attributes["state"].Value;
                    customer.Zip = xmlNodeDefaultCustomer.Attributes["zip"] == null ? String.Empty : xmlNodeDefaultCustomer.Attributes["zip"].Value;
                    customer.Country = xmlNodeDefaultCustomer.Attributes["country"] == null ? String.Empty : xmlNodeDefaultCustomer.Attributes["country"].Value;
                    
                    return customer;
                }
            }
            return null;
        }

        public static void SaveRatesState(List<TemplateRatesBase> state)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            var serializedList = SerializeObject(state);

            XmlNode xmlNodeLanguageCuture = xmlNodeSettings.SelectSingleNode(GetXMLPath(RatesState));
            if (xmlNodeLanguageCuture != null)
                xmlNodeLanguageCuture.InnerText = serializedList;
            else
            {
                xmlNodeLanguageCuture = xmlDoc.CreateElement(RatesState);
                xmlNodeLanguageCuture.InnerText = serializedList;
                xmlNodeSettings.AppendChild(xmlNodeLanguageCuture);
            }

            xmlDoc.Save(SettingsFile);
        }

        public static string SerializeObject<T>(T source)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw))
            {
                serializer.Serialize(writer, source);
                return sw.ToString();
            }
        }

        public static T DeserializeObject<T>(String serializedObject)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(serializedObject))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static List<TemplateRatesBase> GetSavedRatesState()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            XmlNode xmlNodeLanguageCuture = xmlNodeSettings.SelectSingleNode(GetXMLPath(RatesState));
            return xmlNodeLanguageCuture == null ? null : DeserializeObject<List<TemplateRatesBase>>(xmlNodeLanguageCuture.InnerText);
        }
    }
}
