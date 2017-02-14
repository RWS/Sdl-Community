using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Community.TM.Database
{
    public class Helper
    {

        public static void InitializeDatabases(string databasePath)
        {

            var databaseSettingsOut = Path.Combine(databasePath, "Settings.sqlite");
            const string databaseSettings = "TM.Database.New.Settings.sqlite";
            var databaseProjectsOut = Path.Combine(databasePath, "Projects.sqlite");
            const string databaseProjects = "TM.Database.New.Projects.sqlite";

            if (!File.Exists(databaseSettingsOut))
                InitializeDatabasesFirst(databaseSettings, databaseSettingsOut);

            if (!File.Exists(databaseProjectsOut))
                InitializeDatabasesFirst(databaseProjects, databaseProjectsOut);
        }

        internal static void InitializeDatabasesFirst(string database_from, string database_to)
        {
            var _asb = Assembly.GetExecutingAssembly();

            using (var _inputStream = _asb.GetManifestResourceStream(database_from))
            {
                Stream _outputStream = File.Open(database_to, FileMode.Create);

                var _bsInput = new BufferedStream(_inputStream);
                var _bsOutput = new BufferedStream(_outputStream);

                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = _bsInput.Read(buffer, 0, 1024)) > 0)
                    _bsOutput.Write(buffer, 0, bytesRead);


                _bsInput.Flush();
                _bsOutput.Flush();
                _bsInput.Close();
                _bsOutput.Close();
            }
            
        }

        public static string DateTimeToSQLite(DateTime? datetime)
        {
            if (datetime.HasValue)
            {
                var dateTimeFormat = "{0}-{1}-{2}T{3}:{4}:{5}.{6}";
                return string.Format(dateTimeFormat,
                    datetime.Value.Year
                    , datetime.Value.Month.ToString().PadLeft(2, '0')
                    , datetime.Value.Day.ToString().PadLeft(2, '0')
                    , datetime.Value.Hour.ToString().PadLeft(2, '0')
                    , datetime.Value.Minute.ToString().PadLeft(2, '0')
                    , datetime.Value.Second.ToString().PadLeft(2, '0')
                    , datetime.Value.Millisecond.ToString().PadLeft(3, '0').Substring(0, 3));
            }
            return string.Empty;
        }
        public static DateTime? DateTimeFromSQLite(string strDateTime)
        {
            //"2015-05-01T09:20:05.213"
            DateTime? dt = null;

            try
            {
                if (strDateTime.Trim() != string.Empty)
                    dt = DateTime.ParseExact(strDateTime, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
            }
            catch { }
            return dt;
        }


        public static bool AreObjectsEqual(object objectA, object objectB, params string[] ignoreList)
        {
            bool result;

            if (objectA != null && objectB != null)
            {
                Type objectType;

                objectType = objectA.GetType();

                result = true; // assume by default they are equal

                foreach (var propertyInfo in objectType.GetProperties(
                  BindingFlags.Public | BindingFlags.Instance).Where(
                  p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    object valueA;
                    object valueB;

                    valueA = propertyInfo.GetValue(objectA, null);
                    valueB = propertyInfo.GetValue(objectB, null);

                    // if it is a primative type, value type or implements
                    // IComparable, just directly try and compare the value
                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.",
                                        objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IEnumerable<object> collectionItems1;
                        IEnumerable<object> collectionItems2;
                        int collectionItemsCount1;
                        int collectionItemsCount2;

                        // null check
                        if (valueA == null && valueB != null || valueA != null && valueB == null)
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.",
                                                     objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                        else if (valueA != null && valueB != null)
                        {
                            collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                            collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                            collectionItemsCount1 = collectionItems1.Count();
                            collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                Console.WriteLine("Collection counts for property '{0}.{1}' do not match.",
                                                    objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                            // and if they do, compare each item...
                            // this assumes both collections have the same order
                            else
                            {
                                for (var i = 0; i < collectionItemsCount1; i++)
                                {
                                    object collectionItem1;
                                    object collectionItem2;
                                    Type collectionItemType;

                                    collectionItem1 = collectionItems1.ElementAt(i);
                                    collectionItem2 = collectionItems2.ElementAt(i);
                                    collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.",
                                                       i, objectType.FullName, propertyInfo.Name);
                                            result = false;
                                        }
                                    }
                                    else if (!AreObjectsEqual(collectionItem1, collectionItem2, ignoreList))
                                    {
                                        Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.",
                                                            i, objectType.FullName, propertyInfo.Name);
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!AreObjectsEqual(propertyInfo.GetValue(objectA, null),
                                                 propertyInfo.GetValue(objectB, null), ignoreList))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.",
                                                    objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot compare property '{0}.{1}'.",
                                                  objectType.FullName, propertyInfo.Name);
                        result = false;
                    }
                }
            }
            else
                result = Equals(objectA, objectB);

            return result;
        }

        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }
    }
}
