using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Sdl.Community.InvoiceAndQuotes.Customers
{
    public static class Customers
    {

        public static void SaveCustomer(String name, Customer customer)
        {
            if (String.IsNullOrEmpty(name))
                return;
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Settings.SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode("//Settings");
            if (xmlNodeSettings != null)
            {
                XmlNode xmlNodeCustomers = xmlNodeSettings.SelectSingleNode("//Customers");

                if (xmlNodeCustomers != null)
                {
                    XmlNodeList customerNodes = ((XmlElement) xmlNodeCustomers).GetElementsByTagName("Customer");
                    XmlElement xmlNodeCustomer = customerNodes.Cast<XmlElement>().FirstOrDefault(node => node.HasAttribute("Name") && node.Attributes["Name"].Value == name);
                    if (xmlNodeCustomer != null)
                        xmlNodeCustomers.RemoveChild(xmlNodeCustomer);
                }

                if (xmlNodeCustomers != null)
                    xmlNodeCustomers.AppendChild(CreateCustomerElement(xmlDoc, customer));
                else
                {
                    xmlNodeCustomers = xmlDoc.CreateElement("Customers");
                    xmlNodeSettings.AppendChild(xmlNodeCustomers);
                    xmlNodeCustomers.AppendChild(CreateCustomerElement(xmlDoc, customer));
                }
            }

            xmlDoc.Save(Settings.SettingsFile);
        }

        private static XmlNode CreateCustomerElement(XmlDocument xmlDoc, Customer customer)
        {
            var xmlNodeCustomer = xmlDoc.CreateElement("Customer");

            SetInnerTextCustomerElement(customer, xmlNodeCustomer);

            return xmlNodeCustomer;
        }

        private static void SetInnerTextCustomerElement(Customer customer, XmlElement xmlNodeCustomer)
        {
            var serializedCustomer = Settings.SerializeObject(customer);

            var bytes = new byte[serializedCustomer.Length * sizeof(char)];
            Buffer.BlockCopy(serializedCustomer.ToCharArray(), 0, bytes, 0, bytes.Length);
            xmlNodeCustomer.SetAttribute("Name", customer.Name);
            xmlNodeCustomer.InnerText = Convert.ToBase64String(bytes);
        }

        public static void DeleteCustomer(Customer customer)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Settings.SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode("//Settings");
            if (xmlNodeSettings != null)
            {
                XmlNode xmlNodeCustomers = xmlNodeSettings.SelectSingleNode("//Customers");

                if (xmlNodeCustomers != null)
                {
                    XmlNodeList customerNodes = ((XmlElement)xmlNodeCustomers).GetElementsByTagName("Customer");
                    XmlElement xmlNodeCustomer = customerNodes.Cast<XmlElement>().FirstOrDefault(node => node.HasAttribute("Name") && node.Attributes["Name"].Value == customer.Name);
                    if (xmlNodeCustomer != null)
                        xmlNodeCustomers.RemoveChild(xmlNodeCustomer);
                }
            }

            xmlDoc.Save(Settings.SettingsFile);
        }

        private static XPathNavigator GetCustomerNode(String name)
        {
            var projectsFile = new XPathDocument(Settings.SettingsFile);
            var nav = projectsFile.CreateNavigator();
            string expression = String.Format("Settings/Customers/Customer[@Name='{0}']", name);
            return nav.SelectSingleNode(expression);
        }

        public static Customer GetCustomer(String name)
        {
            var customerNode = GetCustomerNode(name);
            return customerNode == null ? null : CreateCustomerFromNode(customerNode);
        }

        private static Customer CreateCustomerFromNode(XPathNavigator customerNode)
        {
            var serializedCustomer = customerNode.Value;
            var bytes = Convert.FromBase64String(serializedCustomer);

            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            var sCustomer = new string(chars);

            var customer = Settings.DeserializeObject<Customer>(sCustomer);
            return customer;
        }

        public static List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();

            var settingsFile = new XPathDocument(Settings.SettingsFile);
            var nav = settingsFile.CreateNavigator();
            const string expression = "Settings/Customers/Customer";
            var customerNodes = nav.Select(expression);
            while (customerNodes.MoveNext())
            {
                customers.Add(CreateCustomerFromNode(customerNodes.Current));
            }

            return customers;
        }
    }
}
