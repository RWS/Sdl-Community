using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Send_feedback;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using LanguageWeaverProvider.ViewModel;
using NLog;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LanguageWeaverProvider.Studio.FeedbackController.ViewModel;

public class FeedbackViewModel : BaseViewModel
{
    private readonly EditorController _editController;
    private ITranslationOptions _selectedProvider;
    private TranslationErrors _translationErrors;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private ISegmentPair _activeSegment;
    private QualityEstimations _originalQE;
    private QualityEstimations _selectedQE;
    private string _previousFeedbackmessage;
    private string _feedbackMessage;
    private string _mtTranslation;
    private bool _canSendFeedback;
    private bool _isMetadataSet;
    private bool _isQeEnabled;

    private bool _isNotificationVisible;
    private string _notificationMessage;

    public FeedbackViewModel()
    {
        _editController = SdlTradosStudio.Application.GetController<EditorController>();
        _editController.ActiveDocumentChanged += ActiveDocumentChanged;
        SendFeedbackCommand = new RelayCommand(SendFeedback);

        InitializeControl();
    }

    public bool IsCloudServiceSelected => SelectedProvider?.PluginVersion == PluginVersion.LanguageWeaverCloud;

    public ITranslationOptions SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            _selectedProvider = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCloudServiceSelected));
        }
    }

    public TranslationErrors TranslationErrors
    {
        get => _translationErrors ??= new();
        set
        {
            _translationErrors = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<QualityEstimations> QualityEstimations => Enum.GetValues(typeof(QualityEstimations)).Cast<QualityEstimations>().Skip(1);

    public QualityEstimations OriginalQE
    {
        get => _originalQE;
        set
        {
            _originalQE = value;
            OnPropertyChanged();
        }
    }

    public QualityEstimations SelectedQE
    {
        get => _selectedQE;
        set
        {
            _selectedQE = value;
            OnPropertyChanged();
        }
    }

    public string FeedbackMessage
    {
        get => _feedbackMessage;
        set
        {
            _feedbackMessage = value;
            OnPropertyChanged();
        }
    }

    public bool CanSendFeedback
    {
        get => _canSendFeedback;
        set
        {
            _canSendFeedback = value;
            OnPropertyChanged();
            IsMetadataSet = value;
        }
    }

    public string MTTranslation
    {
        get => _mtTranslation;
        set
        {
            _mtTranslation = value;
            OnPropertyChanged();
        }
    }

    public bool IsQeEnabled
    {
        get => _isQeEnabled;
        set
        {
            _isQeEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool IsMetadataSet
    {
        get => _isMetadataSet;
        set => SetIsMetadataSetValueAsync(value);
    }

    public bool IsNotificationVisible
    {
        get => _isNotificationVisible;
        set
        {
            _isNotificationVisible = value;
            OnPropertyChanged();
        }
    }

    public string NotificationMessage
    {
        get => _notificationMessage;
        set
        {
            _notificationMessage = value;
            OnPropertyChanged();
        }
    }

    private async void SetIsMetadataSetValueAsync(bool value)
    {
        if (value)
        {
            _isMetadataSet = value;
            OnPropertyChanged(nameof(IsMetadataSet));
            return;
        }

        if (value || !_isMetadataSet)
        {
            return;
        }

        await Task.Delay(4000);
        if (CanSendFeedback)
        {
            return;
        }

        _isMetadataSet = value;
        OnPropertyChanged(nameof(IsMetadataSet));
    }

    public ICommand SendFeedbackCommand { get; private set; }

    private void InitializeControl()
    {
        if (_editController.ActiveDocument is null)
        {
            return;
        }

        _editController.ActiveDocument.SegmentsConfirmationLevelChanged += SegmentConfirmationLevelChanged;
        _editController.ActiveDocument.ActiveSegmentChanged += ActiveSegmentChanged;
        _editController.ActiveDocument.SegmentsTranslationOriginChanged += ActiveSegmentMetadataUpdated;
        _activeSegment = _editController.ActiveDocument.GetActiveSegmentPair();
        GetSegmentMetadata(_activeSegment);
    }

    private void ActiveDocumentChanged(object sender, DocumentEventArgs e)
    {
        InitializeControl();
        ResetView();
    }

    private void ActiveSegmentChanged(object sender, EventArgs e)
    {
        if ((sender as Document).ActiveSegmentPair is not ISegmentPair segmentPair)
        {
            return;
        }

        _activeSegment = segmentPair;
        ResetView();
        GetSegmentMetadata(_activeSegment);
    }

    private void ActiveSegmentMetadataUpdated(object sender, EventArgs e)
    {
        if (_activeSegment?.Properties is null)
        {
            return;
        }

        var segmentObj = GetPropertyValue(sender, "Segment");
        var segmentPropertiesObj = GetPropertyValue(segmentObj, "Properties");
        var segmentProperties = segmentPropertiesObj as ISegmentPairProperties;
        if (IsCurrentTranslationLanguageWeaverSource(segmentProperties)
         && segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation) is null
         && _activeSegment is not null)
        {
            SetSavedSegmentMetadata(segmentProperties);
        }

        GetSegmentMetadata(segmentProperties);
    }

    private async void SegmentConfirmationLevelChanged(object sender, EventArgs e)
    {
        var segmentObj = GetPropertyValue(sender, "Segment");
        var segmentPropertiesObj = GetPropertyValue(segmentObj, "Properties");
        var segmentProperties = segmentPropertiesObj as ISegmentPairProperties;
        if (segmentProperties.ConfirmationLevel != ConfirmationLevel.Translated
         || !IsLanguageWeaverSource(segmentProperties))
        {
            return;
        }

        var currentSegment = _editController.ActiveDocument.SegmentPairs.FirstOrDefault(x => x.Properties.Id.Id.Equals(segmentProperties.Id.Id));
        if (!bool.TryParse(currentSegment.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Feedback), out var autosendFeedback)
         || !autosendFeedback)
        {
            return;
        }

        GetSegmentMetadata(currentSegment.Properties);

        var feedbackSent = await SendFeedback(currentSegment, false);

        if (feedbackSent)
        {
            ResetView();
        }
    }

    private object GetPropertyValue(object sender, string propertyName)
    {
        if (sender is null)
        {
            return null;
        }

        var propertyObject = sender.GetType().GetProperty(propertyName);
        return propertyObject?.GetValue(sender, null);
    }

    private void GetSegmentMetadata(ISegmentPair segment)
    {
        if (segment is null)
        {
            return;
        }

        var segmentProperties = segment.Properties;
        GetSegmentMetadata(segmentProperties);
    }

    private void GetSegmentMetadata(ISegmentPairProperties segmentProperties)
    {
        SetProviderState(segmentProperties);
        CanSendFeedback = segmentProperties is not null && SelectedProvider is not null && IsLanguageWeaverSource(segmentProperties) && segmentProperties.TranslationOrigin?.GetMetaData(Constants.SegmentMetadata_Translation) is not null;

        if (!CanSendFeedback)
        {
            MTTranslation = null;
            OriginalQE = LanguageWeaverProvider.QualityEstimations.None;
            return;
        }

        var qualityEstimation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_QE);

        IsQeEnabled = Enum.TryParse(qualityEstimation, true, out QualityEstimations currentQE);
        OriginalQE = currentQE;
        SelectedQE = QualityEstimations.FirstOrDefault(qe => qe.Equals(currentQE));
        MTTranslation = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);
    }

    private void SetSavedSegmentMetadata(ISegmentPairProperties segmentProperties)
    {
        var currentSegmentId = segmentProperties.Id;
        var documentSegmentPair = _editController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id == currentSegmentId);
        var ratedSegment = ApplicationInitializer.RatedSegments.FirstOrDefault(rs => rs.SegmentId.Equals(currentSegmentId));

        if (documentSegmentPair is null || ratedSegment is null)
        {
            return;
        }

        var translationOrigin = documentSegmentPair.Properties.TranslationOrigin;
        var translationData = new TranslationData
        {
            QualityEstimation = ratedSegment.QualityEstimation,
            Translation = ratedSegment.Translation,
            Model = ratedSegment.Model,
            ModelName = ratedSegment.ModelName,
            AutoSendFeedback = ratedSegment.AutosendFeedback,
            Index = translationOrigin.GetLastTqeIndex() + 1
        };

        translationOrigin.SetMetaData(translationData);
        UpdateSegmentPairProperties(documentSegmentPair);
        ApplicationInitializer.RatedSegments.Remove(ratedSegment);
    }

    private void SetProviderState(ISegmentPairProperties segmentProperties)
    {
        var segmentModel = segmentProperties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
        var pluginVersion = GetPluginVersion(segmentProperties);

        if (segmentModel is null || pluginVersion == PluginVersion.None)
        {
            return;
        }

        var translationOptions = new TranslationOptions { PluginVersion = pluginVersion };
        CredentialManager.GetCredentials(translationOptions, true);
        SelectedProvider = translationOptions;
    }

    private async void SendFeedback(object parameter)
    {
        _activeSegment = _editController?.ActiveDocument?.ActiveSegmentPair;
        SetProviderState(_activeSegment.Properties);

        var feedbackSent = await SendFeedback(_activeSegment);

        if (!feedbackSent) return;

        ResetView();
        ToggleSuccesfullNotification();
    }

    private async Task<bool> SendFeedback(ISegmentPair segmentPair, bool showErrors = true)
    {
        var feedbackSent = false;
        var feedback = LanguageWeaverFeedbackFactory.Create(segmentPair);

        switch (feedback)
        {
            case CloudFeedback cloudFeedback:
                var feedbackMessage = string.IsNullOrEmpty(FeedbackMessage) ? _previousFeedbackmessage : FeedbackMessage;
                var rating = new Rating(feedbackMessage, TranslationErrors);
                cloudFeedback.Rating = rating;
                cloudFeedback.OriginalQe = IsQeEnabled ? OriginalQE.ToString() : null;
                cloudFeedback.Qe = IsQeEnabled ? SelectedQE.ToString() : null;
                break;
            case EdgeFeedback edgeFeedback:
                var comment = !string.IsNullOrEmpty(_previousFeedbackmessage) ? _previousFeedbackmessage : !string.IsNullOrEmpty(FeedbackMessage) ? FeedbackMessage : default;
                edgeFeedback.Comment = comment;
                break;
        }

        if (feedback is null)
        {
            return false;
        }

        try
        {
            feedbackSent = await feedback.Send();
            UpdateSegmentPairProperties(_activeSegment);
        }
        catch (Exception ex)
        {
            if (showErrors) ex.ShowDialog("Feedback", ex.Message, true);
            Logger.Log(LogLevel.Warn, ex.Message);
        }

        return feedbackSent;
    }

    private async void ToggleSuccesfullNotification()
    {
        IsNotificationVisible = true;
        NotificationMessage = "Feedback sent";
        await Task.Delay(3500);
        IsNotificationVisible = false;
        await Task.Delay(1500);
        NotificationMessage = null;
    }



    private void UpdateSegmentPairProperties(ISegmentPair segmentPair)
    {
        _editController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPair.Properties);
    }

    private bool IsLanguageWeaverSource(ISegmentPairProperties segmentProperties)
    {
        return IsCurrentTranslationLanguageWeaverSource(segmentProperties)
            || IsAdapted(segmentProperties);
    }

    private bool IsAdapted(ISegmentPairProperties segmentProperties)
    {
        return segmentProperties.TranslationOrigin?.OriginBeforeAdaptation?.OriginSystem?.StartsWith(Constants.PluginShortName) == true;
    }

    private bool IsCurrentTranslationLanguageWeaverSource(ISegmentPairProperties segmentProperties)
    {
        return segmentProperties.TranslationOrigin?.OriginSystem?.StartsWith(Constants.PluginShortName) == true;
    }

    private PluginVersion GetPluginVersion(ISegmentPairProperties segmentProperties)
    {
        var origin = true switch
        {
            _ when IsCurrentTranslationLanguageWeaverSource(segmentProperties) => segmentProperties.TranslationOrigin.OriginSystem,
            _ when IsAdapted(segmentProperties) => segmentProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem,
            _ => string.Empty,
        };

        return origin switch
        {
            Constants.PluginNameCloud => PluginVersion.LanguageWeaverCloud,
            Constants.PluginNameEdge => PluginVersion.LanguageWeaverEdge,
            _ => PluginVersion.None,
        };
    }

    private void ResetView()
    {
        _previousFeedbackmessage = FeedbackMessage;
        FeedbackMessage = string.Empty;
        TranslationErrors.ResetValues();
    }
}