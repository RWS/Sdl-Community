using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SDLCommunityCleanUpTasks.Utilities
{
	[ContractVerification(false)]
    public static class SimpleLog
    {
        #region Enum Severity

        /// <summary>
        /// Log severity
        /// </summary>
        public enum Severity
        {
            Info,
            Warning,
            Error,
            Exception
        }

        #endregion Enum Severity

        #region Fields

        /// <summary>
        /// Directory to log to
        /// </summary>
        /// <remarks>
        /// Default is the application's current working directory
        /// </remarks>
        private static DirectoryInfo _logDir = new DirectoryInfo(Directory.GetCurrentDirectory());

        /// <summary>
        /// Prefix to use in file name
        /// </summary>
        /// <remarks>
        /// Default is the empty string, i.e. no prefix.
        /// </remarks>
        private static string _prefix;

        /// <summary>
        /// Date format to use in file name
        /// </summary>
        /// <remarks>
        /// Default is "yyyy_MM_dd" (e.g. 2013_04_21), which leads to a daily change of the log file.
        /// </remarks>
        private static string _dateFormat;

        /// <summary>
        /// Suffix to use in file name
        /// </summary>
        /// <remarks>
        /// Default is the empty string, i.e. no suffix.
        /// </remarks>
        private static string _suffix;

        /// <summary>
        /// Extension to use in file name
        /// </summary>
        /// <remarks>
        /// Default is "log".
        /// </remarks>
        private static string _extension;

        /// <summary>
        /// Log level
        /// </summary>
        /// <remarks>
        /// Log all entries with <see cref="Severity"/> set here and above.
        /// For example, when log level is set to <see cref="Severity.Info"/>, incoming entries with severity
        /// <see cref="Severity.Info"/>, <see cref="Severity.Warning"/>, <see cref="Severity.Error"/> and <see cref="Severity.Exception"/>
        /// are actually written to the log file. When log level is set to e.g. <see cref="Severity.Error"/>, only
        /// entries with severity <see cref="Severity.Error"/> and <see cref="Severity.Exception"/> are actually written to the log file.
        /// Default is <see cref="Severity.Info"/>.
        /// </remarks>
        private static Severity _logLevel = Severity.Info;

        /// <summary>
        /// Log entry queue
        /// </summary>
        private static readonly Queue<XElement> _logEntryQueue = new Queue<XElement>();

        /// <summary>
        /// Background task to write log entries to disk
        /// </summary>
        private static Task _backgroundTask;

        /// <summary>
        /// Snyc root for the background task itself
        /// </summary>
        private static readonly object _backgroundTaskSyncRoot = new object();

        /// <summary>
        /// Snyc root for the log file
        /// </summary>
        private static readonly object _logFileSyncRoot = new object();

        /// <summary>
        /// Backing field for <see cref="TextSeparator"/>.
        /// </summary>
        private static string _textSeparator = " | ";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Directory to log to
        /// </summary>
        /// <remarks>
        /// Default is the application's current working directory. Can be set using <see cref="SetLogDir"/>.
        /// Log file is assembled in <see cref="GetFileName"/> using <code>string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension)</code>.
        /// </remarks>
        public static string LogDir
        {
            get
            {
                return _logDir.FullName;
            }
        }

        /// <summary>
        /// Prefix to use in file name
        /// </summary>
        /// <remarks>
        /// Default is the empty string, i.e. no prefix.
        /// Log file is assembled in <see cref="GetFileName"/> using <code>string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension)</code>.
        /// </remarks>
        public static string Prefix
        {
            get
            {
                return _prefix ?? string.Empty;
            }
            set
            {
                _prefix = value;
            }
        }

        /// <summary>
        /// Suffix to use in file name
        /// </summary>
        /// <remarks>
        /// Default is the empty string, i.e. no suffix.
        /// Log file is assembled in <see cref="GetFileName"/> using <code>string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension)</code>.
        /// </remarks>
        public static string Suffix
        {
            get
            {
                return _suffix ?? string.Empty;
            }
            set
            {
                _suffix = value;
            }
        }

        /// <summary>
        /// Extension to use in file name
        /// </summary>
        /// <remarks>
        /// Default is "log". Set to null to return to default.
        /// Log file is assembled in <see cref="GetFileName"/> using <code>string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension)</code>.
        /// </remarks>
        public static string Extension
        {
            get
            {
                return _extension ?? "log";
            }
            set
            {
                _extension = value;
            }
        }

        /// <summary>
        /// Date format to use in file name
        /// </summary>
        /// <remarks>
        /// Default is "yyyy_MM_dd" (e.g. 2013_04_21), which leads to a daily change of the log file. Set to null to return to default. Set to e.g. "yyyy_MM_dd_HH" to change log file hourly.
        /// Log file is assembled in <see cref="GetFileName"/> using <code>string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension)</code>.
        /// </remarks>
        public static string DateFormat
        {
            get
            {
                return _dateFormat ?? "yyyy_MM_dd";
            }
            set
            {
                _dateFormat = value;
            }
        }

        /// <summary>
        /// Log level
        /// </summary>
        /// <remarks>
        /// Log all entries with <see cref="Severity"/> set here and above. In other words, do not write entries to the log file with
        /// severity below the severity specified here.
        ///
        /// For example, when log level is set to <see cref="Severity.Info"/>, incoming entries with severity
        /// <see cref="Severity.Info"/>, <see cref="Severity.Warning"/>, <see cref="Severity.Error"/> and <see cref="Severity.Exception"/>
        /// are actually written to the log file. When log level is set to e.g. <see cref="Severity.Error"/>, only
        /// entries with severity <see cref="Severity.Error"/> and <see cref="Severity.Exception"/> are actually written to the log file.
        /// Default is <see cref="Severity.Info"/>. <see cref="Log(XElement, Severity, bool, int)"/> for details.
        /// </remarks>
        public static Severity LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
            }
        }

        /// <summary>
        /// Whether logging has to be started explicitly as opposed to start automatically on first log. Default is false.
        /// </summary>
        /// <remarks>
        /// Normally, logging starts automatically when the first log entry is enqueued, <see cref="Enqueue"/>. In some
        /// situations, it may be desired to start logging explicitly at a later time. In the meantime, logging
        /// entries are enqued and are processed (i.e. written to the log file) when logging is started.
        /// To start logging, use <see cref="StartLogging"/>
        /// </remarks>
        public static bool StartExplicitly
        {
            get;
            set;
        }

        /// <summary>
        /// Whether to write plain text instead of XML. Default is false.
        /// </summary>
        public static bool WriteText
        {
            get;
            set;
        }

        /// <summary>
        /// When <see cref="WriteText"/> is true, this is the separator text entries reperesenting attributes or values are separated with. Defaults to " | ".
        /// </summary>
        public static string TextSeparator
        {
            get
            {
                return _textSeparator;
            }
            set
            {
                _textSeparator = value ?? string.Empty;
            }
        }

        /// <summary>
        /// File to log in
        /// </summary>
        /// <remarks>
        /// Is assembled from <see cref="LogDir"/>, <see cref="Prefix"/>, the current date and time formatted in <see cref="DateFormat"/>,
        /// <see cref="Suffix"/>, "." and <see cref="Extension"/>. So, by default, the file is named e.g. "2013_04_21.log" and is written to the current working directory.
        /// It is assembled in <see cref="GetFileName"/> using <code>string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension)</code>.
        /// </remarks>
        public static string FileName
        {
            get
            {
                return GetFileName(DateTime.Now);
            }
        }

        /// <summary>
        /// Whether to stop logging background task is requested, i.e. to stop logging at all is requested.
        /// </summary>
        /// <remarks>
        /// Use <see cref="StopLogging"/> to stop logging and <see cref="StartLogging"/> to start logging.
        /// </remarks>
        public static bool StopLoggingRequested
        {
            get;
            private set;
        }

        /// <summary>
        /// Last exception that occurred in the background task when trying to write to the file.
        /// </summary>
        public static Exception LastExceptionInBackgroundTask
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of log entries waiting to be written to file
        /// </summary>
        /// <remarks>
        /// When this number is 1000 or more, there seems to be a permanent problem to wite
        /// to the file. See <see cref="LastExceptionInBackgroundTask"/> what it could be.
        /// </remarks>
        public static int NumberOfLogEntriesWaitingToBeWrittenToFile
        {
            get
            {
                return _logEntryQueue.Count;
            }
        }

        /// <summary>
        /// Whether logging background task currenty runs, i.e. log entries are written to disk.
        /// </summary>
        /// <remarks>
        /// If logging is not running (yet), log methods can be called anyway. Messages will
        /// be written to disk when logging is started. See <see cref="Enqueue"/> for details.
        /// </remarks>
        public static bool LoggingStarted
        {
            get
            {
                return _backgroundTask != null;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static SimpleLog()
        {
            // Attach to process exit event
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
        }

		/// <summary>
		/// Process is about to exit
		/// </summary>
		/// <remarks>
		/// This is some kind of static destructor used to flush unwritten log entries.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void CurrentDomainProcessExit(object sender, EventArgs e)
		{
			StopLogging();
		}

		#endregion Constructor

		#region Public Methods

		/// <summary>
		/// Set all log properties at once
		/// </summary>
		/// <remarks>
		/// Set all log customizing properties at once. This is a pure convenience function. All parameters are optional.
		/// When <see cref="logDir"/> is set and it cannot be created or writing a first entry fails, no exception is thrown, but the previous directory,
		/// respectively the default directory (the current working directory), is used instead.
		/// </remarks>
		/// <param name="logDir"><see cref="LogDir"/> for details. When null is passed here, <see cref="LogDir"/> is not set. Here, <see cref="LogDir"/> is created, when it does not exist.</param>
		/// <param name="prefix"><see cref="Prefix"/> for details. When null is passed here, <see cref="Prefix"/> is not set.</param>
		/// <param name="suffix"><see cref="Suffix"/> for details. When null is passed here, <see cref="Suffix"/> is not set.</param>
		/// <param name="extension"><see cref="Extension"/> for details. When null is passed here, <see cref="Extension"/> is not set.</param>
		/// <param name="dateFormat"><see cref="DateFormat"/> for details. When null is passed here, <see cref="DateFormat"/> is not set.</param>
		/// <param name="logLevel"><see cref="LogLevel"/> for details. When null is passed here, <see cref="LogLevel"/> is not set.</param>
		/// <param name="startExplicitly"><see cref="StartExplicitly"/> for details. When null is passed here, <see cref="StartExplicitly"/> is not set.</param>
		/// <param name="check">Whether to call <see cref="Check"/>, i.e. whether to write a test entry after setting the new log file. If true, the result of <see cref="Check"/> is returned.</param>
		/// <param name="writeText"><see cref="WriteText"/> for details. When null is passed here, <see cref="WriteText"/> is not set.</param>
		/// <param name="textSeparator"><see cref="TextSeparator"/> for details. When null is passed here, <see cref="TextSeparator"/> is not set.</param>
		/// <returns>Null on success, otherwise an exception with what went wrong.</returns>
		public static Exception SetLogFile(string logDir = null, string prefix = null, string suffix = null, string extension = null, string dateFormat = null, Severity? logLevel = null, bool? startExplicitly = null, bool check = true, bool? writeText = null, string textSeparator = null)
        {
            Exception result = null;

            try
            {
                if (writeText != null)
                    WriteText = writeText.Value;

                if (textSeparator != null)
                    TextSeparator = textSeparator;

                if (logLevel != null)
                    LogLevel = logLevel.Value;

                if (extension != null)
                    Extension = extension;

                if (suffix != null)
                    Suffix = suffix;

                if (dateFormat != null)
                    DateFormat = dateFormat;

                if (prefix != null)
                    Prefix = prefix;

                if (startExplicitly != null)
                    StartExplicitly = startExplicitly.Value;

                if (logDir != null)
                    result = SetLogDir(logDir, true);

                // Check if logging works with new settings
                if (result == null && check)
                    result = Check();
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }

        /// <summary>
        /// Set new logging directory
        /// </summary>
        /// <param name="logDir">The logging diretory to set. When passing null or the empty string, the current working directory is used.</param>
        /// <param name="createIfNotExisting">Try to create directory if not existing. Default is false.</param>
        /// <returns>Null if setting log directory was successful, otherwise an exception with what went wrong.</returns>
        public static Exception SetLogDir(string logDir, bool createIfNotExisting = false)
        {
            if (string.IsNullOrEmpty(logDir))
                logDir = Directory.GetCurrentDirectory();

            try
            {
                _logDir = new DirectoryInfo(logDir);

                if (!_logDir.Exists)
                {
                    if (createIfNotExisting)
                    {
                        _logDir.Create();
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(string.Format("Directory '{0}' does not exist!", _logDir.FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        /// <summary>
        /// Check if logging to <see cref="FileName"/> works
        /// </summary>
        /// <remarks>
        /// Writes a test entry directly to <see cref="FileName"/> without using the background task.
        /// When no exception is returned, logging to <see cref="FileName"/> works.
        /// </remarks>
        /// <param name="message">Test message to write to the log file</param>
        /// <returns>Null on success, otherwise an exception with what went wrong.</returns>
        public static Exception Check(string message = "Test entry to see if logging works.")
        {
            // Try to write directly to the file to see if it's working.
            return Log(message, Severity.Info, false);
        }

        /// <summary>
        /// Write info message to log
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="useBackgroundTask">Whether to use the background task (thread) to write messages to disk. Default is true. This is much faster than writing directly to disk in the main thread.</param>
        /// <returns>Null on success or the <see cref="Exception"/> that occurred when processing the message, i.e. when enqueuing the message (when <paramref name="useBackgroundTask"/> is true) or when writing the message to disk (when <paramref name="useBackgroundTask"/> is false).</returns>
        public static Exception Info(string message, bool useBackgroundTask = true)
        {
            return Log(message);
        }

        /// <summary>
        /// Write warning message to log
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="useBackgroundTask">Whether to use the background task (thread) to write messages to disk. Default is true. This is much faster than writing directly to disk in the main thread.</param>
        /// <returns>Null on success or the <see cref="Exception"/> that occurred when processing the message, i.e. when enqueuing the message (when <paramref name="useBackgroundTask"/> is true) or when writing the message to disk (when <paramref name="useBackgroundTask"/> is false).</returns>
        public static Exception Warning(string message, bool useBackgroundTask = true)
        {
            return Log(message, Severity.Warning);
        }

        /// <summary>
        /// Write error message to log
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="useBackgroundTask">Whether to use the background task (thread) to write messages to disk. Default is true. This is much faster than writing directly to disk in the main thread.</param>
        /// <returns>Null on success or the <see cref="Exception"/> that occurred when processing the message, i.e. when enqueuing the message (when <paramref name="useBackgroundTask"/> is true) or when writing the message to disk (when <paramref name="useBackgroundTask"/> is false).</returns>
        public static Exception Error(string message, bool useBackgroundTask = true)
        {
            return Log(message, Severity.Error);
        }

        /// <summary>
        /// Write exception to log
        /// </summary>
        /// <param name="ex">The exception to write to the log</param>
        /// <param name="useBackgroundTask">Whether to use the background task (thread) to write messages to disk. Default is true. This is much faster than writing directly to disk in the main thread.</param>
        /// <param name="framesToSkip">How many frames to skip when detecting the calling method, <see cref="GetCaller"/>. This is useful when log calls to <see cref="SimpleLog"/> are wrapped in an application. Default is 0.</param>
        /// <returns>Null on success or the <see cref="Exception"/> that occurred when processing the message, i.e. when enqueuing the message (when <paramref name="useBackgroundTask"/> is true) or when writing the message to disk (when <paramref name="useBackgroundTask"/> is false).</returns>
        public static Exception Log(Exception ex, bool useBackgroundTask = true, int framesToSkip = 0)
        {
            return ex == null ? null : Log(GetExceptionXElement(ex), Severity.Exception, useBackgroundTask, framesToSkip);
        }

        /// <summary>
        /// Gets an XML string with detailed information about an exception
        /// </summary>
        /// <remarks>
        /// Recursively adds elements for inner exceptions. For the most inner exception, the stack trace is added.
        /// Tags for <see cref="Exception.Data"/> are added. Specific properties of the exception types <see cref="SqlException"/>,
        /// <see cref="COMException"/> and <see cref="AggregateException"/> are recognized, too.
        /// </remarks>
        /// <param name="ex">The exception to get detailed information about</param>
        /// <returns>An XML string with detailed information about the passed exception</returns>
        public static string GetExceptionAsXmlString(Exception ex)
        {
            XElement xElement = GetExceptionXElement(ex);
            return xElement == null ? string.Empty : xElement.ToString();
        }

        /// <summary>
        /// Gets an XElement for an exception
        /// </summary>
        /// <remarks>
        /// Recursively adds elements for inner exceptions. For the most inner exception, the stack trace is added.
        /// Tags for <see cref="Exception.Data"/> are added. Specific properties of the exception types <see cref="SqlException"/>,
        /// <see cref="COMException"/> and <see cref="AggregateException"/> are recognized, too.
        /// </remarks>
        /// <param name="ex">The exception to get the XElement for</param>
        /// <returns>An XElement for the exception</returns>
        public static XElement GetExceptionXElement(Exception ex)
        {
            if (ex == null)
                return null;

            var xElement = new XElement("Exception");
            xElement.Add(new XAttribute("Type", ex.GetType().FullName));
            xElement.Add(new XAttribute("Source", ex.TargetSite == null || ex.TargetSite.DeclaringType == null ? ex.Source : string.Format("{0}.{1}", ex.TargetSite.DeclaringType.FullName, ex.TargetSite.Name)));
            xElement.Add(new XElement("Message", ex.Message));

            if (ex.Data.Count > 0)
            {
                var xDataElement = new XElement("Data");

                foreach (DictionaryEntry de in ex.Data)
                {
                    xDataElement.Add(new XElement("Entry", new XAttribute("Key", de.Key), new XAttribute("Value", de.Value)));
                }

                xElement.Add(xDataElement);
            }

            if (ex is SqlException)
            {
                var sqlEx = (SqlException)ex;
                var xSqlElement = new XElement("SqlException");
                xSqlElement.Add(new XAttribute("ErrorNumber", sqlEx.Number));

                if (!string.IsNullOrEmpty(sqlEx.Server))
                    xSqlElement.Add(new XAttribute("ServerName", sqlEx.Server));

                if (!string.IsNullOrEmpty(sqlEx.Procedure))
                    xSqlElement.Add(new XAttribute("Procedure", sqlEx.Procedure));

                xElement.Add(xSqlElement);
            }

            if (ex is COMException)
            {
                var comEx = (COMException)ex;
                var xComElement = new XElement("ComException");
                xComElement.Add(new XAttribute("ErrorCode", string.Format("0x{0:X8}", (uint)comEx.ErrorCode)));
                xElement.Add(xComElement);
            }

            if (ex is AggregateException)
            {
                var xAggElement = new XElement("AggregateException");
                foreach (Exception innerEx in ((AggregateException)ex).InnerExceptions)
                {
                    xAggElement.Add(GetExceptionXElement(innerEx));
                }
                xElement.Add(xAggElement);
            }

            xElement.Add(ex.InnerException == null ? new XElement("StackTrace", ex.StackTrace) : GetExceptionXElement(ex.InnerException));

            return xElement;
        }

        /// <summary>
        /// Write message to log
        /// </summary>
        /// <remarks>
        /// See <see cref="Log(XElement, Severity, bool, int)"/>.
        /// </remarks>
        /// <param name="message">The message to write to the log</param>
        /// <param name="severity">Log entry severity</param>
        /// <param name="useBackgroundTask">Whether to use the background task (thread) to write messages to disk. Default is true. This is much faster than writing directly to disk in the main thread.</param>
        /// <param name="framesToSkip">How many frames to skip when detecting the calling method, <see cref="GetCaller"/>. This is useful when log calls to <see cref="SimpleLog"/> are wrapped in an application. Default is 0.</param>
        /// <returns>Null on success or the <see cref="Exception"/> that occurred when processing the message, i.e. when enqueuing the message (when <paramref name="useBackgroundTask"/> is true) or when writing the message to disk (when <paramref name="useBackgroundTask"/> is false).</returns>
        public static Exception Log(string message, Severity severity = Severity.Info, bool useBackgroundTask = true, int framesToSkip = 0)
        {
            return string.IsNullOrEmpty(message) ? null : Log(new XElement("Info", message), severity, useBackgroundTask, framesToSkip);
        }

        /// <summary>
        /// Write XElement to log
        /// </summary>
        /// <remarks>
        /// Unless <paramref name="useBackgroundTask"/> is set to false (default is true), the XElement is not actually
        /// written to the file here, but enqueued to the log entry queue. It is dequeued by
        /// <see cref="WriteLogEntriesToFile"/> in a backround task and actually written to the file there.
        /// This is much faster than writing directly to disk in the main thread (what is done when
        /// <paramref name="useBackgroundTask"/> is set to false).
        ///
        /// However, writing to the file is synchronized between threads. I.e. writing directly can be done from multiple threads.
        /// Also, using the background task and writing directly to the file can be used both in parallel.
        ///
        /// When <see cref="StartExplicitly"/> is set to true (default is false), the background task must be started
        /// explicitly by calling <see cref="StartLogging"/>, to get messages actually written to the file. They get enqueued
        /// before the background task is started, though. I.e. they will get logged when the background task is started later.
        ///
        /// When <see cref="StartExplicitly"/> is set to false, which is the default, logging background task (thread) is
        /// started automatically when first calling this method with <paramref name="useBackgroundTask"/> set to true
        /// (which is the default).
        /// </remarks>
        /// <param name="xElement">The XElement to log</param>
        /// <param name="severity">Log entry severity, defaults to <see cref="Severity.Info"/></param>
        /// <param name="useBackgroundTask">Whether to use the background task (thread) to write messages to disk. Default is true. This is much faster than writing directly to disk in the main thread.</param>
        /// <param name="framesToSkip">How many frames to skip when detecting the calling method, <see cref="GetCaller"/>. This is useful when log calls to <see cref="SimpleLog"/> are wrapped in an application. Default is 0.</param>
        /// <returns>Null on success or the <see cref="Exception"/> that occurred when processing the message, i.e. when enqueuing the message (when <paramref name="useBackgroundTask"/> is true) or when writing the message to disk (when <paramref name="useBackgroundTask"/> is false).</returns>
        public static Exception Log(XElement xElement, Severity severity = Severity.Info, bool useBackgroundTask = true, int framesToSkip = 0)
        {
            // Filter entries below log level
            if (xElement == null || severity < LogLevel)
                return null;

            try
            {
                // Assemble XML log entry
                var logEntry = new XElement("LogEntry");
                logEntry.Add(new XAttribute("Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                // logEntry.Add(new XAttribute("Severity", severity));
                logEntry.Add(new XAttribute("Source", GetCaller(framesToSkip)));
                // logEntry.Add(new XAttribute("ThreadId", Thread.CurrentThread.ManagedThreadId));
                logEntry.Add(xElement);

                if (useBackgroundTask)
                {
                    // Enqueue log entry to be written to the file by background task
                    Enqueue(logEntry);
                }
                else
                {
                    // Write directly to the file. This is synchronized among threads within the method,
                    // so can be used in parallel with the above.
                    return WriteLogEntryToFile(logEntry);
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        /// <summary>
        /// Gets the log filename for the passed date
        /// </summary>
        /// <param name="dateTime">The date to get the log file name for</param>
        /// <returns>The log filename for the passed date</returns>
        public static string GetFileName(DateTime dateTime)
        {
            return string.Format("{0}\\{1}{2}{3}.{4}", LogDir, Prefix, dateTime.ToString(DateFormat), Suffix, Extension);
        }

        /// <summary>
        /// Check, whether there is a log file for the passed date
        /// </summary>
        /// <param name="dateTime">The date and time to check the existance of a log file for</param>
        /// <returns>True = log file exists, false otherwise</returns>
        public static bool LogFileExists(DateTime dateTime)
        {
            return File.Exists(GetFileName(dateTime));
        }

        /// <summary>
        /// Get the current log file as XML document
        /// </summary>
        /// <remarks>
        /// Does not throw an exception when the log file does not exist.
        /// </remarks>
        /// <returns>The log file as XML document or null when it does not exist.</returns>
        public static XDocument GetLogFileAsXml()
        {
            return GetLogFileAsXml(DateTime.Now);
        }

        /// <summary>
        /// Get the log file for the passed date as XML document
        /// </summary>
        /// <remarks>
        /// Does not throw an exception when the log file does not exist.
        /// </remarks>
        /// <param name="dateTime">The date and time to get the log file for. Use DateTime.Now to get the current log file.</param>
        /// <returns>The log file as XML document or null when it does not exist.</returns>
        public static XDocument GetLogFileAsXml(DateTime dateTime)
        {
            string fileName = GetFileName(dateTime);
            if (!File.Exists(fileName))
                return null;

            Flush();

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<LogEntries>");
            sb.AppendLine(File.ReadAllText(fileName));
            sb.AppendLine("</LogEntries>");

            return XDocument.Parse(sb.ToString());
        }

        /// <summary>
        /// Get the current log file as text document
        /// </summary>
        /// <remarks>
        /// Does not throw an exception when the log file does not exist.
        /// </remarks>
        /// <returns>The log file as text document or null when it does not exist.</returns>
        public static string GetLogFileAsText()
        {
            return GetLogFileAsText(DateTime.Now);
        }

        /// <summary>
        /// Get the log file for the passed date as text document
        /// </summary>
        /// <remarks>
        /// Does not throw an exception when the log file does not exist.
        /// </remarks>
        /// <param name="dateTime">The date and time to get the log file for. Use DateTime.Now to get the current log file.</param>
        /// <returns>The log file as text document or null when it does not exist.</returns>
        public static string GetLogFileAsText(DateTime dateTime)
        {
            string fileName = GetFileName(dateTime);
            if (!File.Exists(fileName))
                return null;

            Flush();

            return File.ReadAllText(fileName);
        }

        /// <summary>
        /// Shows the current log file
        /// </summary>
        /// <remarks>
        /// Opens the default program to show XML files and displays the requested file, if it exists. Does nothing otherwise.
        /// A temporary XML file is created and saved in the users's temporary path each time this method is called. So don't
        /// use it excessively.
        /// </remarks>
        public static void ShowLogFile()
        {
            ShowLogFile(DateTime.Now);
        }

        /// <summary>
        /// Show a log file for the passed date
        /// </summary>
        /// <remarks>
        /// Opens the default program to show text or XML files and displays the requested file, if it exists. Does nothing otherwise.
        /// When <see cref="WriteText"/> is false, a temporary XML file is created and saved in the users's temporary path each time this method is called.
        /// So don't use it excessively in that case. Otherwise, the log file itself is shown.
        /// </remarks>
        /// <param name="dateTime">The date and time to show the log file for.</param>
        public static void ShowLogFile(DateTime dateTime)
        {
            string fileName;

            if (WriteText)
            {
                Flush();
                fileName = GetFileName(dateTime);
            }
            else
            {
                fileName = string.Format("{0}Log_{1}.xml", Path.GetTempPath(), DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                XDocument logFileXml = GetLogFileAsXml(dateTime);
                if (logFileXml != null)
                    logFileXml.Save(fileName);
            }

            if (!File.Exists(fileName))
                return;

            // Let system choose application to start
            Process.Start(fileName);

            // Wait a little to give application time to open
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Start logging
        /// </summary>
        /// <remarks>
        /// Start background task pointing to <see cref="WriteLogEntriesToFile"/> to write log files to disk.
        /// Is called automatically by <see cref="Enqueue"/> when the first entry is logged, unless
        /// <see cref="StartExplicitly"/> is set to true (default is false). Then, this method has to be
        /// called explicitly to start logging.
        /// </remarks>
        public static void StartLogging()
        {
            // Task already started
            if (_backgroundTask != null || StopLoggingRequested)
                return;

            // Reset stopping flag
            StopLoggingRequested = false;

            lock (_backgroundTaskSyncRoot)
            {
                if (_backgroundTask != null)
                    return;

                // Reset last exception
                LastExceptionInBackgroundTask = null;

                // Create and start task
                _backgroundTask = new Task(WriteLogEntriesToFile, TaskCreationOptions.LongRunning);
                _backgroundTask.Start();
            }
        }

        /// <summary>
        /// Stop logging background task, i.e. logging at all.
        /// </summary>
        /// <remarks>
        /// Stop background task pointing to <see cref="WriteLogEntriesToFile"/> to write log files to disk.
        /// </remarks>
        /// <param name="flush">Whether to write all pending entries to disk before. Default is true.</param>
        public static void StopLogging(bool flush = true)
        {
            // Tell task to stop.
            StopLoggingRequested = true;
            if (_backgroundTask == null)
                return;

            // Write pending entries to disk
            if (flush)
                Flush();

            lock (_backgroundTaskSyncRoot)
            {
                if (_backgroundTask == null)
                    return;

                // Wait for task to finish and set null then
                _backgroundTask.Wait(1000);
                _backgroundTask = null;
            }
        }

        /// <summary>
        /// Wait for all entries having been written to the file
        /// </summary>
        public static void Flush()
        {
            // Background task not running? Nothing to do.
            if (!LoggingStarted)
                return;

            // Are there still items waiting to be written to disk?
            while (NumberOfLogEntriesWaitingToBeWrittenToFile > 0)
            {
                // Remember current number
                int lastNumber = NumberOfLogEntriesWaitingToBeWrittenToFile;

                // Wait some time to let background task do its work
                Thread.Sleep(222);

                // Didn't help? No log entries have been processed? We probably hang.
                // Let it be to avoid waiting eternally.
                if (lastNumber == NumberOfLogEntriesWaitingToBeWrittenToFile)
                    break;
            }
        }

        /// <summary>
        /// Clear background task's log entry queue. I.e. remove all log messages waiting to be written to <see cref="FileName"/> by the background task.
        /// </summary>
        public static void ClearQueue()
        {
            lock (_logEntryQueue)
            {
                _logEntryQueue.Clear();
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Enqueue log entry to be written to log file
        /// </summary>
        /// <remarks>
        /// When <see cref="StartExplicitly"/> is set to false (which is the default),
        /// logging is started automatically by calling <see cref="StartLogging"/> from
        /// inside this method when the first <paramref name="logEntry"/> is enqueued.
        ///
        /// When <see cref="StartExplicitly"/> is set to true, <paramref name="logEntry"/>
        /// is just enqueued, but not yet actually written to the log file.
        /// The latter will be done when <see cref="StartLogging"/> is called explicitly.
        /// </remarks>
        /// <param name="logEntry">The log entry to be enqueued</param>
        private static void Enqueue(XElement logEntry)
        {
            // Start logging if not already started, unless it is desired to start it explicitly
            if (!StartExplicitly)
                StartLogging();

            lock (_logEntryQueue)
            {
                // Stop enqueueing when stop request was set or when the queue gets too full.
                if (!StopLoggingRequested && _logEntryQueue.Count < 10000)
                    _logEntryQueue.Enqueue(logEntry);
            }
        }

        /// <summary>
        /// Get the next log entry from the queue, but do not dequeue it
        /// </summary>
        /// <returns>The next element or null when the queue is empty</returns>
        private static XElement Peek()
        {
            lock (_logEntryQueue)
            {
                return _logEntryQueue.Count == 0 ? null : _logEntryQueue.Peek();
            }
        }

        /// <summary>
        /// Dequeue log entry
        /// </summary>
        private static void Dequeue()
        {
            lock (_logEntryQueue)
            {
                if (_logEntryQueue.Count > 0)
                    _logEntryQueue.Dequeue();
            }
        }

        /// <summary>
        /// Write log entries to the file on disk
        /// </summary>
        /// <remarks>
        /// The thread looks every 100 milliseconds for new items in the queue.
        /// </remarks>
        private static void WriteLogEntriesToFile()
        {
            while (!StopLoggingRequested)
            {
                // Get next log entry from queue
                XElement xmlEntry = Peek();
                if (xmlEntry == null)
                {
                    // If queue is empty, sleep for a while and look again later.
                    Thread.Sleep(100);
                    continue;
                }

                // Try ten times to write the entry to the log file. Wait between tries, because the file could (hopefully) temporarily
                // be locked by another application. When it didn't work out after ten tries, dequeue the entry anyway, i.e. the entry is lost then.
                // This is necessary to ensure that the queue does not get too full and we run out of memory.
                for (int i = 0; i < 10; i++)
                {
                    // Actually write entry to log file.
                    LastExceptionInBackgroundTask = WriteLogEntryToFile(xmlEntry);

                    // When all is fine, we're done. Otherwise do not retry when queue is already getting full.
                    if (LastExceptionInBackgroundTask == null || NumberOfLogEntriesWaitingToBeWrittenToFile > 1000)
                        break;

                    // Only wait when queue is not already getting full.
                    Thread.Sleep(100);
                }

                // Dequeue entry from the queue
                Dequeue();
            }
        }

        /// <summary>
        /// Write one log entry to file
        /// </summary>
        /// <remarks>
        /// This method can be called from the logging background thread or directly
        /// from the main thread. Lock accordingly to avoid multiple threads concurrently
        /// accessing the file. When the lock can not be got within five seconds,
        /// <paramref name="xmlEntry" /> is not being written to the file, but a respective
        /// exception is returned, saying what went wrong.
        /// </remarks>
        /// <param name="xmlEntry">The entry to write</param>
        /// <returns>Null when all worked fine, an exception otherwise</returns>
        private static Exception WriteLogEntryToFile(XElement xmlEntry)
        {
            if (xmlEntry == null)
                return null;

            const int secondsToWaitForFile = 5;

            // This method can be called from the logging background thread or directly
            // from the main thread. Lock accordingly to avoid multiple threads concurrently
            // accessing the file.
            if (Monitor.TryEnter(_logFileSyncRoot, new TimeSpan(0, 0, 0, secondsToWaitForFile)))
            {
                try
                {
                    // Use filestream to be able to explicitly specify FileShare.None
                    using (var fileStream = new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        // Modified to UTF8
                        using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        {
                            if (WriteText)
                            {
                                // Write plain text
                                streamWriter.WriteLine(ConvertXmlToPlainText(xmlEntry));
                            }
                            else
                            {
                                // Write XML
                                streamWriter.WriteLine(xmlEntry);
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    try
                    {
                        ex.Data["Filename"] = FileName;
                    }
                    catch
                    {
                        return ex;
                    }

                    return ex;
                }
                finally
                {
                    Monitor.Exit(_logFileSyncRoot);
                }
            }

            try
            {
                return new Exception(string.Format("Could not write to file '{0}', because it was blocked by another thread for more than {1} seconds.", FileName, secondsToWaitForFile));
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Convert <paramref name="xmlEntry"/> to plain text to be written to a file.
        /// </summary>
        /// <remarks>
        /// A typical xml entry to be converted looks like this:
        /// <![CDATA[
        ///
        /// <LogEntry Date="2014-06-19 11:20:52" Severity="Info" Source="SimpleLogDemo.Program.DoSomethingElse" ThreadId="9">
        ///   <Message>Entering method. See Source which method is meant.</Message>
        /// </LogEntry>
        /// <LogEntry Date="2014-06-19 11:20:52" Severity="Exception" Source="SimpleLogDemo.Program.DoSomething" ThreadId="9">
        ///   <Exception Type="System.Exception" Source="SimpleLogDemo.Program.DoSomethingElse">
        ///     <Message>Something went wrong.</Message>
        ///     <Exception Type="System.NullReferenceException" Source="SimpleLogDemo.Program.DoSomethingElse">
        ///       <Message>Object reference not set to an instance of an object.</Message>
        ///       <StackTrace>   at SimpleLogDemo.Program.DoSomethingElse(String fred) in D:\Projekt\VisualStudio\SimpleLogDemo\SimpleLogDemo\Program.cs:line 91</StackTrace>
        ///     </Exception>
        ///   </Exception>
        /// </LogEntry>
        ///
        /// ]]>
        ///
        /// This is a basic implementation so far. Feel free to implement your own if you need something more sophisticated, e.g.
        /// nicer exception formatting.
        /// </remarks>
        /// <param name="xmlEntry">The XML entry to convert.</param>
        /// <returns><paramref name="xmlEntry"/> converted to plain text.</returns>
        private static string ConvertXmlToPlainText(XElement xmlEntry)
        {
            var sb = new StringBuilder();

            foreach (var element in xmlEntry.DescendantsAndSelf())
            {
                if (element.HasAttributes)
                {
                    foreach (var attribute in element.Attributes())
                    {
                        if (sb.Length > 0)
                            sb.Append(TextSeparator);

                        sb.Append(attribute.Name).Append(" = ").Append(attribute.Value);
                    }
                }
                else
                {
                    if (sb.Length > 0)
                        sb.Append(TextSeparator);

                    // Remove new lines to get all in one line.
                    string value = element.Value.Replace("\r\n", " ");
                    sb.Append(element.Name).Append(" = ").Append(value);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Detects the method that was calling the log method
        /// </summary>
        /// <remarks>
        /// The method is walking up the frames in the stack trace until the first method outside <see cref="SimpleLog"/> is reached.
        /// When log calls to <see cref="SimpleLog"/> are wrapped in an application, this may still not be the method where logging
        /// was called initially (e.g. when an exception occurred and has been logged). In that case set <paramref name="framesToSkip"/>
        /// accordingly to get outside the wrapper method(s).
        /// </remarks>
        /// <param name="framesToSkip">How many frames to skip when detecting the calling method. This is useful when log calls to <see cref="SimpleLog"/> are wrapped in an application. Default is 0.</param>
        /// <returns>Class and method that was calling the log method</returns>
        private static string GetCaller(int framesToSkip = 0)
        {
            string result = string.Empty;

            int i = 1;

            while (true)
            {
                // Walk up the stack trace ...
                var stackFrame = new StackFrame(i++);
                MethodBase methodBase = stackFrame.GetMethod();
                if (methodBase == null)
                    break;

                // Here we're at the end - nomally we should never get that far
                Type declaringType = methodBase.DeclaringType;
                if (declaringType == null)
                    break;

                // Get class name and method of the current stack frame
                result = string.Format("{0}.{1}", declaringType.FullName, methodBase.Name);

                // Here, we're at the first method outside of SimpleLog class.
                // This is the method that called the log method. We're done unless it is
                // specified to skip additional frames and go further up the stack trace.
                if (declaringType != typeof(XmlReportGenerator) && declaringType != typeof(SimpleLog) && --framesToSkip < 0)
                    break;
            }

            return result;
        }

        #endregion Private Methods
    }
}