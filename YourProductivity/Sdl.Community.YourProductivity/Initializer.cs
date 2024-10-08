﻿using NLog;
using NLog.Config;
using NLog.Targets;
using Sdl.Community.YourProductivity.Persistance;
using Sdl.Community.YourProductivity.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.YourProductivity
{
    [ApplicationInitializer]
    public class Initializer : IApplicationInitializer
    {
        private EditorController _editorController;
        private Logger _logger;
        private LoggingConfiguration _loggingConfiguration;
        private KeyboardTrackingService _service;

        public void Execute()
        {
            _logger = LogManager.GetLogger("log");

            try
            {
                RavenContext.Current.Initialize();
                InitializeLoggingConfiguration();

                Application.AddMessageFilter(KeyboardTracking.Instance);
                SdlTradosStudio.Application.Closing += Application_Closing;

                _service = new KeyboardTrackingService(_logger);

                SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
                    .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

                _logger.Info(string.Format("Started productivity plugin version {0}", VersioningService.GetPluginVersion()));
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unexpected exception when intializing the app");
            }
        }

        private void _editorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            _service.ActiveDocument = e.Document;
        }

        private void _editorController_Closing(object sender, CancelDocumentEventArgs e)
        {
            try
            {
                if (e.Document.Mode != EditingMode.Translation)
                    return;
                _service.UnregisterDocument(e.Document);

                foreach (var file in e.Document.Files)
                {
                    _logger.Info(string.Format("Closed document: {0}", file.Name));
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unexpected exception when closing the editor");
            }
        }

        private void _editorController_Opened(object sender, DocumentEventArgs e)
        {
            try
            {
                if (e.Document.Mode != EditingMode.Translation)
                    return;

                _service.RegisterDocument(e.Document);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unexpected exception when changing the active document");
            }
        }

        private void Application_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.RemoveMessageFilter(KeyboardTracking.Instance);
            RavenContext.Current.CurrentSession.SaveChanges();
            RavenContext.Current.Dispose();
        }

        private void InitializeLoggingConfiguration()
        {
            _loggingConfiguration = new LoggingConfiguration();
            var fileTarget = new FileTarget();
            _loggingConfiguration.AddTarget("file", fileTarget);
            fileTarget.CreateDirs = true;
            fileTarget.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Trados AppStore\Productivity\Log\community-productivity.log");
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}";

            var rule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            _loggingConfiguration.LoggingRules.Add(rule);

            LogManager.Configuration = _loggingConfiguration;
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent e)
        {
            _editorController = SdlTradosStudio.Application.GetController<EditorController>();
            _editorController.Opened += _editorController_Opened;
            _editorController.ActiveDocumentChanged += _editorController_ActiveDocumentChanged;
            _editorController.Closing += _editorController_Closing;
        }
    }
}