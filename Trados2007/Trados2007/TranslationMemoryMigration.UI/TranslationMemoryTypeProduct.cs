using Microsoft.Win32;

namespace Sdl.TranslationStudio.TranslationMemoryMigration.UI.References
{
    /// <summary>
    /// TranslationMemoryTypeProduct class represents a translation memory type product using information in the registry.
    /// </summary>
    public abstract class TranslationMemoryTypeProduct
    {
        private readonly string _productDisplayName;

        private RegistryKey _productRegistryKey;
        private bool _productRegistryKeyInitialized;

        /// <summary>
        /// Constructor that takes the given product display name.
        /// </summary>
        /// <param name="productDisplayName">product display name</param>
        protected TranslationMemoryTypeProduct(string productDisplayName)
        {
            _productDisplayName = productDisplayName;
        }

        /// <summary>
        /// IsInstalled property determines whether the product is installed.
        /// </summary>
        public virtual bool IsInstalled
        {
            get
            {
                return (ProductRegistryKey != null);
            }
        }

        /// <summary>
        /// ProductRegistryKey property represents the product registry key.
        /// </summary>
        private RegistryKey ProductRegistryKey
        {
            get
            {
                if (!_productRegistryKeyInitialized)
                {
                    _productRegistryKeyInitialized = true;

                    _productRegistryKey = GetProductRegistryKey(_productDisplayName, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall") ??
                                          GetProductRegistryKey(_productDisplayName, "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"); // 64-bit windows
                }

                return _productRegistryKey;
            }
        }

        /// <summary>
        /// Gets the product registry key with the given product display name searching the given uninstall key.
        /// </summary>
        /// <param name="productDisplayName">product display name</param>
        /// <param name="uninstallKeyName">uninstall key name</param>
        /// <returns>product registry key</returns>
        private RegistryKey GetProductRegistryKey(string productDisplayName, string uninstallKeyName)
        {
            RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey(uninstallKeyName);
            if (uninstallKey != null)
            {
                foreach (string uninstallSubKeyName in uninstallKey.GetSubKeyNames())
                {
                    RegistryKey uninstallSubKey = uninstallKey.OpenSubKey(uninstallSubKeyName);
                    if (uninstallSubKey != null)
                    {
                        string uninstallSubKeyDisplayName = (string)uninstallSubKey.GetValue("DisplayName");
                        if ((uninstallSubKeyDisplayName != null) && (uninstallSubKeyDisplayName.StartsWith(productDisplayName)))
                        {
                            object systemComponentObject = uninstallSubKey.GetValue("SystemComponent");
                            if ((systemComponentObject == null) || (((int) systemComponentObject) == 0))
                            {
                                return uninstallSubKey;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// IsInitialized property determines whether the product is initialized.
        /// </summary>
        /// <remarks>
        /// For example, some products may need to be run once to be initialized.
        /// </remarks>
        public virtual bool IsInitialized
        {
            get
            {
                return true;
            }
        }
    }
}

