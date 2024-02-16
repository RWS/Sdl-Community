using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMProvider
{
    public enum TMPurposes
    {
        Undefined = 0,
        None,
        Lookup,
        Update,
        LookupUpdate
    }

    public enum ResourceAccessLevel
    {
        ReadOnly = 0,
        ReadWrite = 1,
        Admin = 2
    }

    public class TMInfo
    {
        public ResourceAccessLevel AccessLevel { get; private set; }

        public string Client { get; private set; }

        public string Domain { get; private set; }

        public string FriendlyName { get; private set; }

        public int NumEntries { get; private set; }

        public string Project { get; private set; }

        public string SourceLanguageCode { get; private set; }

        public string Subject { get; private set; }

        public Guid TMGuid { get; private set; }

        public string TMOwner { get; private set; }

        public string TargetLanguageCode { get; private set; }

        public TMPurposes Purpose { get; set; }

        public TMInfo(Guid guid, string name, string owner, string sourceLang, string targetLang, int entryCount,
            ResourceAccessLevel permissions, TMPurposes sdlPurpose, string pr, string cl, string sub, string dom)
        {
            this.AccessLevel = permissions;
            this.Client = cl;
            this.Domain = dom;
            this.FriendlyName = name;
            this.NumEntries = entryCount;
            this.Project = pr;
            this.SourceLanguageCode = sourceLang;
            this.Subject = sub;
            this.TMGuid = guid;
            this.TMOwner = owner;
            this.TargetLanguageCode = targetLang;
            this.Purpose = sdlPurpose;
        }

        internal TMInfo(MemoQServerTypes.TMModel memoQServerTMModel)
        {
            this.AccessLevel = memoQServerTMModel.AccessLevel;
            this.Client = memoQServerTMModel.Client;
            this.Domain = memoQServerTMModel.Domain;
            this.FriendlyName = memoQServerTMModel.FriendlyName;
            this.NumEntries = memoQServerTMModel.NumEntries;
            this.Project = memoQServerTMModel.Project;
            this.SourceLanguageCode = memoQServerTMModel.SourceLangCode == null ? "" : memoQServerTMModel.SourceLangCode;
            this.Subject = memoQServerTMModel.Subject;
            this.TargetLanguageCode = memoQServerTMModel.TargetLangCode == null ? "" : memoQServerTMModel.TargetLangCode;
            this.TMGuid = new Guid(memoQServerTMModel.TMGuid);
            this.TMOwner = memoQServerTMModel.TMOwner;
            this.Purpose = TMPurposes.Undefined;
        }

        public TMInfo Clone()
        {
            return new TMInfo(this.TMGuid, this.FriendlyName, this.TMOwner, this.SourceLanguageCode, this.TargetLanguageCode, this.NumEntries, this.AccessLevel,
                this.Purpose, this.Project, this.Client, this.Subject, this.Domain);
        }
    }
}
