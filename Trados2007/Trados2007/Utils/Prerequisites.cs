// -----------------------------------------------------------------------
// <copyright file="Prerequisites.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    using Sdl.TranslationStudio.TranslationMemoryMigration.UI.References;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Prerequisites
    {
        private static readonly TranslationMemoryTypeProduct _tradosServerManager2007Product = new TradosServerManagerProduct();

        public static string WarnMessage 
        { 
            get 
            {
                return PluginResources.Trados2007_ServerManager2007NotInstalledMessage;
            }
        }

        internal static bool WarnIfServerManager2007NotInstalled()
        {
            var notInstalled = !_tradosServerManager2007Product.IsInstalled;

            if (notInstalled)
            {
                MessagingHelpers.ShowWarning(null, Prerequisites.WarnMessage);
            }

            return notInstalled;
        }
    }
}
