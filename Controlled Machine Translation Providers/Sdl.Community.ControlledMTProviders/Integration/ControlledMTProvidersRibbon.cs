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
            string text = CascadeLanguageDirection.DisableMT ? "Enable MT" : "Disable MT";
            this.Text = text;
        }

        protected override void Execute()
        {
            CascadeLanguageDirection.DisableMT = !CascadeLanguageDirection.DisableMT;
            string text = CascadeLanguageDirection.DisableMT ? "Enable MT" : "Disable MT";
            this.Text = text;
        }
    }

    [Action("SDL.Community.ManageMT.Translate", Name = "Translate", Icon = "icon", Description = "Translate current segment in the editor")]
    [ActionLayout(typeof(ControlledMTProvidersRibbon), 60, DisplayType.Normal)]
    [Shortcut(Keys.Alt | Keys.T)]
    class ControlledMtTranslateViewPartAction : AbstractAction
    {
        private EditorController _editorController;
        private bool _disableMt = CascadeLanguageDirection.DisableMT;

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
                _editorController.ActiveDocument.ContentChanged += ActiveDocument_ContentChanged;

                _disableMt = CascadeLanguageDirection.DisableMT;
                CascadeLanguageDirection.DisableMT = false;
                _editorController.ActiveDocument.TryTranslateActiveSegment();
            }
        }

        void ActiveDocument_ContentChanged(object sender, DocumentContentEventArgs e)
        {
            CascadeLanguageDirection.DisableMT = _disableMt;
            _editorController.ActiveDocument.ContentChanged -= ActiveDocument_ContentChanged;
        }
    }
}

