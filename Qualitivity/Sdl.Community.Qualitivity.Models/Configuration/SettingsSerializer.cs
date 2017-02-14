using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace Structures.Configuration
{
    public class SettingsSerializer
    {
        #region  |  settings serialization  |

        //public static void SaveSettings(Settings settings)
        //{
        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        serializer = new XmlSerializer(typeof(Settings));
        //        stream = new FileStream(settings.applicationPaths.applicationSettingsFullPath, FileMode.Create, FileAccess.Write);
        //        serializer.Serialize(stream, settings);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}
        //public static Settings ReadSettings()
        //{
        //    Settings settings = new Settings();
        //    if (!File.Exists(settings.applicationPaths.applicationSettingsFullPath))
        //    {
        //        SaveSettings(settings);
        //    }

        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        serializer = new XmlSerializer(typeof(Settings));
        //        stream = new FileStream(settings.applicationPaths.applicationSettingsFullPath, FileMode.Open);
        //        settings = (Settings)serializer.Deserialize(stream);

        //        if (settings == null)
        //            settings = new Settings();


        //        return settings;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();


        //        #region  |  check/create backup  |

        //        //try
        //        //{

        //        //    DateTime dt_a = settings.backupSettings.backupLastDate;
        //        //    if (settings.backupSettings.backupEveryType == 0)
        //        //        dt_a = dt_a.AddDays(settings.backupSettings.backupEvery);
        //        //    else
        //        //        dt_a = dt_a.AddDays(settings.backupSettings.backupEvery * 7);


        //        //    if (dt_a < DateTime.Now)
        //        //    {
        //        //        if (settings.backupSettings.backupFolder.Trim() != string.Empty && Directory.Exists(settings.backupSettings.backupFolder))
        //        //        {
        //        //            string _backupFolder_year = Path.Combine(settings.backupSettings.backupFolder.Trim(), DateTime.Now.Year.ToString());
        //        //            if (!Directory.Exists(_backupFolder_year))
        //        //                Directory.CreateDirectory(_backupFolder_year);

        //        //            string _backupFolder_month = Path.Combine(_backupFolder_year, DateTime.Now.Month.ToString().PadLeft(2, '0'));
        //        //            if (!Directory.Exists(_backupFolder_month))
        //        //                Directory.CreateDirectory(_backupFolder_month);

        //        //            string newFilePath = Path.Combine(_backupFolder_month, "Studio.Time.Tracker.Professional.settings."
        //        //                + DateTime.Now.Year + "." + DateTime.Now.Month.ToString().PadLeft(2, '0') + "." + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".xml");

        //        //            settings.backupSettings.backupLastDate = DateTime.Now;

        //        //            File.Copy(settings.applicationPaths.applicationSettingsFullPath, newFilePath, true);

        //        //            SaveSettings(settings);
        //        //        }
        //        //        else
        //        //        {
        //        //            throw new Exception("Unable to locate the backup folder: '" + settings.backupSettings.backupFolder.Trim() + "' ");
        //        //        }
        //        //    }
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    throw new Exception("Error creating backup!\r\n\r\n" + ex.Message);
        //        //}
        //        #endregion
        //    }
        //}

        //public static void SaveTrackingProjects(TrackingProjects trackingProjects)
        //{
        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        serializer = new XmlSerializer(typeof(TrackingProjects));
        //        stream = new FileStream(trackingProjects.trackingProjectsFullFilePath, FileMode.Create, FileAccess.Write);
        //        serializer.Serialize(stream, trackingProjects);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}
        //public static TrackingProjects ReadTrackingProjects()
        //{
        //    TrackingProjects trackingProjects = new TrackingProjects();
        //    if (!File.Exists(trackingProjects.trackingProjectsFullFilePath))
        //    {
        //        SaveTrackingProjects(trackingProjects);
        //    }

        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        serializer = new XmlSerializer(typeof(TrackingProjects));
        //        stream = new FileStream(trackingProjects.trackingProjectsFullFilePath, FileMode.Open);
        //        trackingProjects = (TrackingProjects)serializer.Deserialize(stream);

        //        if (trackingProjects == null)
        //            trackingProjects = new TrackingProjects();


        //        return trackingProjects;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}

        //public static void UpdateTrackChanges(string applicationTrackChangesPath, string projectId, string documentId, List<Structures.Documents.DocumentActivity> tcas)
        //{
        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        string path = Path.Combine(applicationTrackChangesPath, projectId);
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        string fullPath = Path.Combine(path, documentId + ".xml");


        //        Structures.TrackChanges.DocumentActivities tcass = new TrackChanges.DocumentActivities();
        //        if (tcas.Count > 0)
        //        {
        //            tcass.documentId = tcas[0].documentId;
        //            tcass.documentName = tcas[0].documentName;
        //            tcass.documentPath = tcas[0].documentPath;
                   
        //            tcass.projectId = tcas[0].projectId;
        //            tcass.projectName = tcas[0].projectName;

        //            tcass.sourceLang = tcas[0].sourceLang;
        //            tcass.targetLang = tcas[0].targetLang;

        //            tcass.documentActivities = tcas;
        //        }


        //        serializer = new XmlSerializer(typeof(Structures.TrackChanges.DocumentActivities));
        //        stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //        serializer.Serialize(stream, tcass);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}
        //public static void SaveTrackChanges(string applicationTrackChangesPath, string projectId, string documentId, List<Structures.Documents.DocumentActivity> tcas_new)
        //{
        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        string path = Path.Combine(applicationTrackChangesPath, projectId);
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        string fullPath = Path.Combine(path, documentId + ".xml");

        //        List<Structures.TrackChanges.DocumentActivity> tcas = ReadTrackChanges(applicationTrackChangesPath, projectId, documentId);
        //        tcas.AddRange(tcas_new);

        //        Structures.TrackChanges.DocumentActivities tcass = new TrackChanges.DocumentActivities();
        //        if (tcas.Count > 0)
        //        {
        //            tcass.documentId = tcas[0].documentId;
        //            tcass.documentName = tcas[0].documentName;
        //            tcass.documentPath = tcas[0].documentPath;
                  
        //            tcass.projectId = tcas[0].projectId;
        //            tcass.projectName = tcas[0].projectName;

        //            tcass.sourceLang = tcas[0].sourceLang;
        //            tcass.targetLang = tcas[0].targetLang;

        //            tcass.documentActivities = tcas;
        //        }


        //        serializer = new XmlSerializer(typeof(Structures.TrackChanges.DocumentActivities));
        //        stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //        serializer.Serialize(stream, tcass);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}
        //public static List<Structures.Documents.DocumentActivity> ReadTrackChanges(string applicationTrackChangesPath, string projectId, string documentId)
        //{
        //    Structures.TrackChanges.DocumentActivities tcass = new TrackChanges.DocumentActivities();

        //    string path = Path.Combine(applicationTrackChangesPath, projectId);
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);

        //    string fullPath = Path.Combine(path, documentId + ".xml");

        //    if (File.Exists(fullPath))
        //    {

        //        XmlSerializer serializer = null;
        //        FileStream stream = null;
        //        try
        //        {


        //            serializer = new XmlSerializer(typeof(Structures.TrackChanges.DocumentActivities));
        //            stream = new FileStream(fullPath, FileMode.Open);
        //            tcass = (Structures.TrackChanges.DocumentActivities)serializer.Deserialize(stream);

        //            foreach (Structures.TrackChanges.DocumentActivity da in tcass.documentActivities)
        //            {
        //                da.documentId = tcass.documentId;
        //                da.documentName = tcass.documentName;
        //                da.documentPath = tcass.documentPath;
                       
        //                da.projectId = tcass.projectId;
        //                da.projectName = tcass.projectName;

        //                da.sourceLang = tcass.sourceLang;
        //                da.targetLang = tcass.targetLang;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (stream != null)
        //                stream.Close();

        //        }
        //    }
        //    return tcass.documentActivities;
        //}
        //public static void UpdateTrackChanges(string applicationTrackChangesPath, string projectId, string documentId, List<Structures.TrackChanges.DocumentActivity> tcas)
        //{
        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        string path = Path.Combine(applicationTrackChangesPath, projectId);
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        string fullPath = Path.Combine(path, documentId + ".xml");

        //        serializer = new XmlSerializer(typeof(List<Structures.TrackChanges.DocumentActivity>));
        //        stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //        serializer.Serialize(stream, tcas);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}
        //public static void SaveTrackChanges(string applicationTrackChangesPath, string projectId, string documentId, List<Structures.TrackChanges.DocumentActivity> tcas_new)
        //{
        //    XmlSerializer serializer = null;
        //    FileStream stream = null;
        //    try
        //    {
        //        string path = Path.Combine(applicationTrackChangesPath, projectId);
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        string fullPath = Path.Combine(path, documentId + ".xml");

        //        List<Structures.TrackChanges.DocumentActivity> tcas = ReadTrackChanges(applicationTrackChangesPath, projectId, documentId);
        //        tcas.AddRange(tcas_new);

        //        serializer = new XmlSerializer(typeof(List<Structures.TrackChanges.DocumentActivity>));                
        //        stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //        serializer.Serialize(stream, tcas);
        //    }
        //    catch (Exception ex)
        //    {                
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }
        //}
        //public static List<Structures.TrackChanges.DocumentActivity> ReadTrackChanges(string applicationTrackChangesPath, string projectId, string documentId)
        //{
        //    List<Structures.TrackChanges.DocumentActivity> tcas = new List<TC.DocumentActivity>();

        //    string path = Path.Combine(applicationTrackChangesPath, projectId);
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);

        //    string fullPath = Path.Combine(path, documentId + ".xml");

        //    if (File.Exists(fullPath))
        //    {

        //        XmlSerializer serializer = null;
        //        FileStream stream = null;
        //        try
        //        {
        //            serializer = new XmlSerializer(typeof(List<Structures.TrackChanges.DocumentActivity>));
        //            stream = new FileStream(fullPath, FileMode.Open);
        //            tcas = (List<Structures.TrackChanges.DocumentActivity>)serializer.Deserialize(stream);

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (stream != null)
        //                stream.Close();

        //        }
        //    }
        //    return tcas;
        //}


        #endregion
    }
}
