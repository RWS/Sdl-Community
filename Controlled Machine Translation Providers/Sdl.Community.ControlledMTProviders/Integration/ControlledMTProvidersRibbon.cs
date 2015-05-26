using Sdl.Community.ControlledMTProviders.Provider;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.ControlledMTProviders.Integration
{
    [RibbonGroup("SDL.Community.ManageMTRibbonGroup", Name = "Controlled MT Providers")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorAdvancedRibbonTabLocation))]
    class ControlledMTProvidersRibbon : AbstractRibbonGroup
    {
    }


    [Action("SDL.Community.ManageMT", Name = "Enable/Disable Machine Translation", Icon = "icon", Description = "Enable or disable configured machine translation providers")]
    [ActionLayout(typeof(ControlledMTProvidersRibbon), 60, DisplayType.Normal)]
    [Shortcut(Keys.Alt | Keys.Shift | Keys.M)]
    class ControlledMTViewPartAction : AbstractAction
    {
        public ControlledMTViewPartAction()
        {
        }

        public override void Initialize()
        {
            string text = CascadeLanguageDirection.DisableMt ? "Enable MT" : "Disable MT";
            this.Text = text;
        }

        protected override void Execute()
        {
            CascadeLanguageDirection.DisableMt = !CascadeLanguageDirection.DisableMt;
            string text = CascadeLanguageDirection.DisableMt ? "Enable MT" : "Disable MT";
            this.Text = text;
        }
    }

    [Action("SDL.Community.ManageMT.Translate", Name = "Translate", Icon = "icon", Description = "Translate current segment in the editor")]
    [ActionLayout(typeof(ControlledMTProvidersRibbon), 60, DisplayType.Normal)]
    [Shortcut(Keys.Alt | Keys.T)]
    class ControlledMtTranslateViewPartAction : AbstractAction
    {
        private EditorController _editorController;
        private bool _disableMt = CascadeLanguageDirection.DisableMt;

        public ControlledMtTranslateViewPartAction()
        {
        }

        public override void Initialize()
        {
            _editorController = SdlTradosStudio.Application.GetController<EditorController>();
        }

        protected override void Execute()
        {
            if (_editorController.ActiveDocument != null)
            {
                CascadeLanguageDirection.TranslationFinished += CascadeLanguageDirection_TranslationFinished;
                _disableMt = CascadeLanguageDirection.DisableMt;
                CascadeLanguageDirection.DisableMt = false;
                _editorController.ActiveDocument.TryTranslateActiveSegment();
            }
        }

        void CascadeLanguageDirection_TranslationFinished(object sender, EventArgs e)
        {
            CascadeLanguageDirection.DisableMt = _disableMt;
            CascadeLanguageDirection.TranslationFinished -= CascadeLanguageDirection_TranslationFinished;
        }
    }
}

