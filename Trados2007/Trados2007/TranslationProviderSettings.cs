// ---------------------------------
// <copyright file="TranslationProviderSettings.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// Trados 2007 Translation Provider Settings
    /// </summary>
    public class TranslationProviderSettings
    {
        #region Fields

        /// <summary>
        /// Uri builder
        /// </summary>
        private TranslationProviderUriBuilder uriBuilder;

        #endregion // Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationProviderSettings"/> class.
        /// </summary>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        public TranslationProviderSettings(Uri translationProviderUri)
        {
            this.uriBuilder = new TranslationProviderUriBuilder(translationProviderUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationProviderSettings"/> class with empty data fields.
        /// </summary>
        public TranslationProviderSettings()
        {
            this.uriBuilder = new TranslationProviderUriBuilder(AbstractTrados2007TranslationProvider.UriScheme);
        }

        /// <summary>
        /// Gets the translation Provider URI.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Gets or sets the path to filebased trados 2007 translation provider.
        /// For server-based provider contains only server path. 
        /// Translation memory container and name are stored separetely.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path
        {
            get
            {
                return this.uriBuilder["path"];
            }

            set
            {
                this.uriBuilder["path"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the host. Setting this field is mandatory.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host
        {
            get
            {
                return this.uriBuilder.HostName;
            }

            set
            {
                this.uriBuilder.HostName = value;
            }
        }

        /// <summary>
        /// Gets the connection point URI. Could be the same as ServerUri.
        /// </summary>
        public Uri ConnectionPointUri
        {
            get
            {
                string serverString = string.Format(
                    "{0}://{1}.{2}",
                    "http", this.Path, this.TranslationMemoryName);

                return new Uri(serverString);
            }
        }

        /// <summary>
        /// Gets or sets the password for server-based translation memory. The value is ignored for file-based TMs.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get
            {
                return this.uriBuilder["password"];
            }

            set
            {
                this.uriBuilder["password"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get
            {
                return this.uriBuilder.UserName;
            }

            set
            {
                this.uriBuilder.UserName = value;
            }
        }

        /// <summary>
        /// Gets or sets the descriptive name of the translation memory.
        /// </summary>
        public string TranslationMemoryName
        {
            get
            {
                return this.uriBuilder["translationMemoryName"];
            }

            set
            {
                this.uriBuilder["translationMemoryName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the container. Valid only for server-based translation memories.
        /// </summary>
        /// <value>
        /// The name of the container.
        /// </value>
        public string Container
        {
            get
            {
                return this.uriBuilder["container"];
            }
            set
            {
                this.uriBuilder["container"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the access mode.
        /// </summary>
        /// <value>
        /// The access mode.
        /// </value>
        public TranslationMemoryAccessMode AccessMode
        {
            get
            {
                return Extensions.FromString(this.uriBuilder["accessMode"]);
            }

            set
            {
                this.uriBuilder["accessMode"] = value.ToString();
            }
        }
    }
}