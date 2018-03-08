// ---------------------------------
// <copyright file="Trados2007TranslationProviderSerializationHelper.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-12-01</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Sdl.TranslationStudio.Plugins.Trados2007.UI;
    using System.Xml.Serialization;

    /// <summary>
    /// Serialization helper
    /// </summary>
    public class TP2007ServerBasedDialogSerializationHelper
    {
        public bool Dummy { get; set; }

        public TP2007ServerBasedDialogSerializationHelper()
        {
        }
        
        public TP2007ServerBasedDialogSerializationHelper(ServerBasedModel model)
        {
            int cnt = model.Servers.Count;
            this.Servers = new string[cnt];
            for (int i = 0; i < cnt; i++)
            {
                this.Servers[i] = model.Servers[i].ToString();
            }
        }

        [XmlArray()]
        public string[] Servers
        {
            get;
            set;
        }
    }
}
