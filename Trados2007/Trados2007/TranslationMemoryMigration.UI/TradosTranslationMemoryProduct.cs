using System;
using Microsoft.Win32;

using Trados.Interop.TMAccess;

namespace Sdl.TranslationStudio.TranslationMemoryMigration.UI.References
{
    /// <summary>
    /// TradosTranslationMemoryProduct class represents the trados translation memory product - SDL Trados 2007.
    /// </summary>
    public class TradosTranslationMemoryProduct : TranslationMemoryTypeProduct
    {
        const string TradosOptionsRegistryKey = @"Software\Trados\TW4Win\Options";
        const string TradosUserIdValueName = "Userid";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TradosTranslationMemoryProduct() : base("SDL Trados 2007")
        {
        }

        /// <summary>
        /// IsInstalled property determines whether the product is installed.
        /// </summary>
        public override bool IsInstalled
        {
            get
            {
                bool isInstalled = base.IsInstalled;

                if (!isInstalled)
                {
                    // can't find "SDL Trados 2007" in the registry

                    // try and instantiate a Trados COM component
                    try
                    {
                        new TranslationMemoryClass();

                        // can instantiate a Trados COM component so deemed installed
                        isInstalled = true;
                    }
                    catch (Exception exception)
                    {
                        // can't instantiate a Trados COM component so deemed not installed
                        //GlobalServices.LoggingService.GetLogger(typeof(TradosTranslationMemoryProduct).FullName).
                        //    Warn("Cannot instantiate Trados COM component during product check", exception);
                    }
                }

                return isInstalled;
            }
        }

        /// <summary>
        /// IsInitialized property determines whether the product is initialized.
        /// </summary>
        /// <remarks>
        /// Trados is initialized if the 'Userid' registry entry exists. Creates entry if it doesn't exist.
        /// </remarks>
        public override bool IsInitialized
        {
            get
            {
                try
                {
                    return CheckUserId();
                }
                catch (Exception exception)
                {
                    //ILog log = GlobalServices.LoggingService.GetLogger(this.GetType().FullName);
                    //log.Warn("Cannot determine whether Trados is initialized or not.", exception);

                    return true;
                }
            }
        }

        private bool CheckUserId()
        {
            bool createdKey = false;

            RegistryKey key = null;

            try
            {
                key = Registry.CurrentUser.OpenSubKey(TradosOptionsRegistryKey);

                // If the key doesn't exist then create it.
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey(TradosOptionsRegistryKey);
                    createdKey = true;
                }

                // It does exist and it can't be created so exit.
                if (key == null)
                {
                    return false;
                }

                string userId = key.GetValue(TradosUserIdValueName) as string;

                // If it already exists then exit now.
                if (!String.IsNullOrEmpty(userId))
                {
                    return true;
                }

                // If the registry key was opened in 'Read Only' mode then open the key again with write access 
                if (!createdKey)
                {
                    key.Close();
                    key = Registry.CurrentUser.OpenSubKey(TradosOptionsRegistryKey, true);

                    if (key == null)
                    {
                        return false;
                    }
                }

                // Create the required Userid entry using current windows user name
                // Note: Trados requires Userid to be uppercase.

                userId = Environment.UserName;
                userId = userId.ToUpperInvariant();

                key.SetValue(TradosUserIdValueName, userId, RegistryValueKind.String);

                return true;
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
        }


    }
}
