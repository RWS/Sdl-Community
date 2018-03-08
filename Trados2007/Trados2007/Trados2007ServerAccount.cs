// ---------------------------------
// <copyright file="ServerBasedPresenter.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-30</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using Trados.Interop.TMAccess;
    using System.Collections;

    /// <summary>
    /// Encapsulates logic for managing single server account
    /// </summary>
    public class Trados2007ServerAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trados2007ServerAccount"/> class.
        /// </summary>
        /// <param name="translationServer">The tm server adress. Could be ip adress or host name.</param>
        /// <param name="login">The login. Empty value with the same for password means windows authentication.</param>
        /// <param name="password">The password.</param>
        /// <remarks>
        /// Hostnames with suffixes are not supported (don't know why).
        /// </remarks>
        public Trados2007ServerAccount(string translationServer, string login = "", string password = "")
        {
            this.Login = login;
            this.Password = password;
            this.TranslationServer = translationServer;
        }

        public bool Valid
        {
            get
            {
                try
                {
                    var server = this.TranslationServer;

                    TmContainers containers = this.Browser.GetContainers(server, this.Login, this.Password);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private TmBrowserClass Browser
        {
            get
            {
                return new TmBrowserClass(); // creates each time new in order to get it worked on any thread
            }
        }

        /// <summary>
        /// Gets a value indicating whether this server is up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this server is up; otherwise, <c>false</c>.
        /// </value>
        public bool IsServerUp
        {
            get
            {
                var ping = new Ping();
                var reply = ping.Send(this.TranslationServer);

                return reply.Status == IPStatus.Success;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.TranslationServer;
        }

        private struct MyTmDescriptor
        {
            public TmDescriptor tmDescriptor;
            public ITmContainer container;
        }

        /// <summary>
        /// Gets the translation memories availible for specified user account.
        /// </summary>
        /// <returns>List of availible translation memories.</returns>
        public IList<ServerBasedTrados2007TranslationMemory> GetTranslationMemories(bool cache = true)
        {
            if (this.translationMemories != null && cache)
            {
                return this.translationMemories;
            }

            var result = new List<ServerBasedTrados2007TranslationMemory>();

            if (this.IsServerUp)
            {
                foreach (MyTmDescriptor myDescriptor in GetTmDescriptors())
                {
                    var tm = new ServerBasedTrados2007TranslationMemory(
                                    this.TranslationServer, myDescriptor.tmDescriptor.Name, myDescriptor.tmDescriptor.Container,
                                    myDescriptor.container.User, myDescriptor.container.Password, TranslationMemoryAccessMode.Maintenance);
                    result.Add(tm);
                }
            }

            this.translationMemories = result;

            return result;
        }

        private IEnumerable GetTmDescriptors()
        {
            //Don't use this method with server IP instead of server name.
            //I could return not all tms in such case.
            
            List<MyTmDescriptor> tmDescriptors = new List<MyTmDescriptor>();

            var tmContainers = this.Browser.GetContainers(this.TranslationServer, this.Login, this.Password);
            
            foreach (ITmContainer container in tmContainers)
            {
                var descriptors  = this.Browser.GetTMsInContainer(this.TranslationServer, container.Name, container.User, container.Password);

                foreach (TmDescriptor desc in descriptors)
                {
                    MyTmDescriptor myDesc = new MyTmDescriptor { tmDescriptor = desc, container = container };

                    tmDescriptors.Add(myDesc);
                }
            }

            return tmDescriptors;
        }

        /// <summary>
        /// Checks if the translation memory exists on server.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if exists; otherwise - <c>false</c>. Returns null if server is down.</returns>
        [Obsolete("Method is not implemented in COM wrapper.")]
        public bool? CheckTranslationMemoryExists(string name)
        {
            if (this.IsServerUp)
            {
                return this.Browser.TMExists(this.TranslationServer, name, this.Login, this.Password);
            }
            else
            {
                return null;
            }
        }

        // have to use IdentityInfoCache instead of sharing of this fields

        public string Login { get; private set; }

        public string Password { get; private set; }

        public bool IsWindowsAuthentication
        {
            get
            {
                return string.IsNullOrEmpty(Login) && string.IsNullOrEmpty(Password);
            }
        }

        private List<ServerBasedTrados2007TranslationMemory> translationMemories;

        /// <summary>
        /// Gets the connection point URI. Could be the same as ServerUri.
        /// </summary>
        public Uri ConnectionPointUri
        {
            get
            {
                string serverString = string.Format(
                    "{0}://{1}",
                    "http", this.TranslationServer);

                return new Uri(serverString);
            }
        }

        public string TranslationServer { get; private set; }
    }
}
