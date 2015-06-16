using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sdl.Community.Productivity.Services;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Community.Productivity.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Productivity
{
    [ApplicationInitializer]
    public class Initializer:IApplicationInitializer
    {
        private EditorController _editorController;
        private LoggingConfiguration _loggingConfiguration;
        private Logger _logger;
        KeyboardTrackingService _service;

        public void Execute()
        {
            _logger = LogManager.GetLogger("log");

            try
            {

                InitializeLoggingConfiguration();

                Application.AddMessageFilter(KeyboardTracking.Instance);
                SdlTradosStudio.Application.Closing += Application_Closing;

                _service = new KeyboardTrackingService(_logger);
                _editorController = SdlTradosStudio.Application.GetController<EditorController>();
                _editorController.Opened += _editorController_Opened;
                _editorController.ActiveDocumentChanged += _editorController_ActiveDocumentChanged;
                _editorController.Closing += _editorController_Closing;

                var twitterPersistenceService = new TwitterPersistenceService(_logger);
                if (twitterPersistenceService.HasAccountConfigured) return;
                using (var tForm = new TwitterAccountSetup(twitterPersistenceService))
                {
                    tForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {

                _logger.Debug(ex,"Unexpected exception when intializing the app");
                throw;
            }
        }
        
        void Application_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.RemoveMessageFilter(KeyboardTracking.Instance);
        }

        private void InitializeLoggingConfiguration()
        {
            _loggingConfiguration = new LoggingConfiguration();
            var fileTarget = new FileTarget();
            _loggingConfiguration.AddTarget("file", fileTarget);
            fileTarget.CreateDirs = true;
            fileTarget.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL Community\Productivity\Log\community-productivity.log");
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}";
            
            var rule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            _loggingConfiguration.LoggingRules.Add(rule);

            LogManager.Configuration = _loggingConfiguration;
            
        }

        private void _editorController_Closing(object sender, CancelDocumentEventArgs e)
        {
            try
            {
                _logger.Info(string.Format("Closed document: {0}", e.Document.ActiveFile.Name));

                _service.UnregisterDocument(e.Document);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unexpected exception when closing the editor");
                throw;
            }

        }

        void _editorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            _service.ActiveDocument = e.Document;
        }

        private void _editorController_Opened(object sender, DocumentEventArgs e)
        {
            try
            {
                _logger.Info(string.Format("Opened document: {0}", e.Document.ActiveFile.Name));

                _service.RegisterDocument(e.Document);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unexpected exception when changing the active document");
                throw;
            }

        }

    }
}
