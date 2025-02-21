using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

using TMProvider.MemoQServerTypes;

namespace TMProvider
{
    public delegate bool AskToAcceptCertificateDelegate(TMProviderMQServer provider, X509Certificate2 certificate);

    public class TMProviderMQServer : TMProviderBase
    {
        #region Properties and Constructors

        /// <summary>
        /// How many times we've already tried login/resend. Reset it when the response comes back OK.
        /// </summary>
        private int resendCount = 0;

        private volatile LoginResult currentLoginData;
        private string apiURL;

        private ConcurrentQueue<Exception> exceptionList = new ConcurrentQueue<Exception>();
        public override List<Exception> ExceptionList { get { return exceptionList.ToList(); } }

        #region Error codes

        // error codes
        private const string AuthenticationFailed = "AuthenticationFailed";
        private const string InvalidOrExpiredToken = "InvalidOrExpiredToken";
        private const string TooFrequentLogin = "TooFrequentLogin";
        private const string NoLicenseAvailable = "NoLicenseAvailable";
        private const string ReverseLookupNotSupported = "ReverseLookupNotSupported";
        private const string ResourceNotFound = "ResourceNotFound";
        private const string EntryNotFound = "EntryNotFound";
        private const string OptimisticConcurrencyError = "OptimisticConcurrencyError";
        private const string TBError = "TBError";
        private const string InvalidArgument = "InvalidArgument";
        private const string Unauthorized = "Unauthorized";
        private const string InternalServerError = "InternalError";

        #endregion

        /// <summary>
        /// Use this constructor when you don't know which TMs to use.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password">In the form it needs to be stored (encrypted).</param>
        public TMProviderMQServer(string url, string username, string password, LoginTypes loginType)
            : this(new MemoQTMSettings(MemoQTMProviderTypes.MemoQServer, url, username, password, loginType))
        {

        }

        /// <summary>
        /// Use this consructor when you already know the settings.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password">In the form it needs to be stored (encrypted).</param>
        public TMProviderMQServer(MemoQTMSettings providerSettings)
        {
            this.usedTMs = new List<TMInfo>();
            this.providerName = providerSettings.URL;
            this.providerType = MemoQTMProviderTypes.MemoQServer;
            this.settings = providerSettings;
            apiURL = settings.URL + AppData.ServerURLCommon;

            if (settings.LoginType == LoginTypes.Undefined) settings.LoginType = LoginTypes.MemoQServer;

            this.CertificateData = null;
            // check if this provider has a stored self-signed certificate
            if (CertificateStore.Instance.ProviderHasStoredCertificate(this.providerName))
            {
                this.CertificateAccepted = true;
                this.CertificateIsSelfSigned = true;
            }
            else
            {
                this.CertificateAccepted = false;
                this.CertificateIsSelfSigned = false;
            }

  
        }

        #endregion

        #region Certificates

        /// <summary>
        /// The certificate was rejected by the user.
        /// </summary>
        public bool CertificateAccepted { get; private set; }
        /// <summary>
        /// The certificate is self-signed or something else (the user has to accept the certificate).
        /// </summary>
        public bool CertificateIsSelfSigned { get; private set; }

        /// <summary>
        /// To display the certificate information to the user. Null if everything's OK with
        /// </summary>
        public X509Certificate2 CertificateData { get; private set; }

        public void AcceptCertificate(string thumbprint)
        {
            CertificateStore.Instance.StoreThumbprint(this.ProviderName, thumbprint);
            this.CertificateAccepted = true;
            this.CertificateIsSelfSigned = true;
        }

        /// <summary>
        /// Overrides certification authorization decision.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool onValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {           
            // we validate every time
            this.CertificateAccepted = true;
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                // the coming certificate is OK, but the previous one was not
                // delete the previous one from the store
                if (CertificateIsSelfSigned && CertificateAccepted) CertificateStore.Instance.DeleteThumbprint(this.providerName);
                return true;
            } 

            // Create an X509Certificate2 instance from X509Certificate so we can access the thumbprint easily.
            X509Certificate2 certificate2 = new X509Certificate2();
            certificate2.Import(certificate.GetRawCertData());

            this.CertificateAccepted = false;
            this.CertificateData = null;

            if (certificate2.NotAfter < DateTime.Now)
            {
                collectException(new ExpiredCertificateException(String.Format("The certificate for {0} has expired.", providerName)));
                return false;
            }
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                // the server's actual name doesn't match the name the certificate was issued for
                collectException(new NameMismatchCertificateException("The name of the server and the name in the certificate don't match."));
                return false;
            }
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                collectException(new NoCertificateException(String.Format("{0} has no valid certificate.", providerName)));
                return false;
            }

            // don't worry if it's self-signed or anything else, just check if it's stored and ask the user
            string thumbprint = certificate2.Thumbprint;

            // Check whether we have a stored thumbprint already.
            bool isThumbprintSaved = !String.IsNullOrEmpty(CertificateStore.Instance.IsThumbprintStored(this.ProviderName, thumbprint));
            if (isThumbprintSaved)
            {
                this.CertificateIsSelfSigned = true;
                this.CertificateAccepted = true;
                return true;
            }
            else
            {
                // In case we do not have a stored thumbprint yet, this may be the first time the user runs the
                // application, so she has to be asked here if she accepts the provided certificate. If so, the value
                // of the thumbprint variable must be persisted for later use and the method should return true.

                // save data so we can ask the user to accept
                this.CertificateAccepted = false;
                this.CertificateIsSelfSigned = true;
                this.CertificateData = certificate2;
                return false;
            }


 
        }

 
        #endregion

        #region Login

        /// <summary>
        /// The password should come as it is stored with the settings (encrypted).
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password">In the form it is stored with the settings (encrypted).</param>
        /// <param name="loginType"></param>
        public override void Login(string userName, string password, LoginTypes loginType)
        {
            try
            {
                if (this.settings == null) throw new InvalidOperationException("The memoQ server TM provider has no settings.");

                LoginMode lmode = getLoginMode(settings.LoginType);
                string ltst = null;
                
                LoginParam data = new LoginParam(userName, StringEncoder.DecryptText(password), ltst, lmode);


                Task<LoginResult> t = request<LoginResult>(HttpMethod.Post, apiURL + AppData.ServerURLLogin, data);
                t.Wait();
                currentLoginData = t.Result;

                if (currentLoginData == null) isLoggedIn = false;
                else
                {
                    // refresh settings if login was successful
                    this.settings.UserName = userName;
                    this.settings.Password = password;
                    isLoggedIn = true;
                    isActive = true;
                    // if TMs haven't been filled yet, fill them up now from settings
                    if (this.usedTMs == null || this.usedTMs.Count == 0) fillUsedTMsFromSettings();
                }
            }
            catch (Exception e)
            {
                isLoggedIn = false;
                collectException(e);
            }
        }

        public void Logout()
        {
            try
            {
                Task t = request(HttpMethod.Post, apiURL + AppData.ServerURLLogout, null);
                t.Wait();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Tries to login again, but only if not logged in.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="loginType"></param>
        protected override void RetryLogin(string username, string pwd, LoginTypes loginType)
        {
            // if already logged in -> don't do anything
            if (isLoggedIn) return;
            // if not logged in and not active (already tried login enough times) -> don't do anything
            if (!isLoggedIn && !isActive) return;

            Login(username, pwd, loginType);
        }

        private LoginMode getLoginMode(LoginTypes loginType)
        {
            switch (settings.LoginType)
            {
                case LoginTypes.Undefined:
                    return LoginMode.MemoQServerUser;
                case LoginTypes.MemoQServer:
                    return LoginMode.MemoQServerUser;
                case LoginTypes.Windows:
                    return LoginMode.WindowsUser;
                default:
                    throw new InvalidOperationException("Invalid login type for a memoQ server provider.");
            }
        }



        #endregion // Login

        #region TM Operations

        public override TMInfo[] ListTMs()
        {
            if (!isActive) return new TMInfo[0];
            if (!isLoggedIn) Login(this.settings.UserName, this.settings.Password, this.settings.LoginType);

            MemoQServerTypes.TMModel[] t = null;
            try
            {
                t = request<MemoQServerTypes.TMModel[]>(HttpMethod.Get, apiURL + AppData.ServerURLListTMs, null).Result;
                // exceptions are added to the exception list
            }
            catch (Exception e)
            {
                // if there is some communication error, the task is null, and can't access the Result property -> throws NullReferenceEx.
                // don't collect this, the com. error is already there
                if (!(e is NullReferenceException)) collectException(e);
                return new TMInfo[0];
            }
            if (t != null)
            {
                TMInfo[] tms = t.Select(tmmodel => new TMInfo(tmmodel)).ToArray();
                return tms;
            }
            else return new TMInfo[0];
        }


        private async Task<TMHit[]> lookupSegmentAsync(QuerySegment segmentToLookup, TMProvider.LookupSegmentRequest options, TMInfo TM)
        {
            // try to log in again - when we send the request, and the token has expired, we try to login again anyway
            if (!isActive) return new TMHit[0];
            if (!isLoggedIn) Login(this.settings.UserName, this.settings.Password, this.settings.LoginType);

            LookupSegmentsParam param = new LookupSegmentsParam(new QuerySegment[] { segmentToLookup }, options);
            LookupSegmentsResult lookupResult = null;
            try
            {
                //lookupResult = await request<LookupSegmentsResult>(HttpMethod.Post, apiURL + String.Format(AppData.ServerURLLookup, TM.TMGuid.ToString()) + "?authToken=" + currentLoginData.AccessToken, param);
                lookupResult = await request<LookupSegmentsResult>(HttpMethod.Post, apiURL + String.Format(AppData.ServerURLLookup, TM.TMGuid.ToString()), param);
            }
            catch (Exception e)
            {
                string tmName = TM.FriendlyName;// GetTMName(TM);
                collectException(e, TM.TMGuid.ToString(), tmName);
                return new TMHit[0];
            }
            if (lookupResult == null || lookupResult.Result.Length == 0) return new TMHit[] { };

            // we only have one segment to look up
            TMHit[] hits = new TMHit[lookupResult.Result[0].TMHits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i] = new TMHit(lookupResult.Result[0].TMHits[i], TM.SourceLanguageCode, TM.TargetLanguageCode);
            }
            return hits;
        }

        private async Task<TMHit[][]> lookupSegmentsAsync(QuerySegment[] segments, TMProvider.LookupSegmentRequest options, TMInfo TM, int matchThreshold)
        {
            if (!isActive) return new TMHit[0][];
            if (!isLoggedIn) Login(this.settings.UserName, this.settings.Password, this.settings.LoginType);

            LookupSegmentsParam param = new LookupSegmentsParam(segments, options);
            LookupSegmentsResult lookupResult = null;
            try
            {
                lookupResult = await request<LookupSegmentsResult>(HttpMethod.Post, apiURL + String.Format(AppData.ServerURLLookup, TM.TMGuid.ToString()), param);
            }
            catch (Exception e)
            {
                string tmName = TM.FriendlyName;
                collectException(e, TM.TMGuid.ToString(), tmName);
                return new TMHit[0][];
            }

            if (lookupResult == null) return new TMHit[][] { };

            TMHitsPerSegment[] tmHitsPersegment = lookupResult.Result;
            if (tmHitsPersegment == null || tmHitsPersegment.Length == 0) return new TMHit[][] { };

            TMHit[][] hits = new TMHit[tmHitsPersegment.Length][];
            for (int j = 0; j < tmHitsPersegment.Length; j++)
            {
                if (tmHitsPersegment[j].TMHits == null) continue;
                hits[j] = new TMHit[tmHitsPersegment[j].TMHits.Length];
                for (int i = 0; i < tmHitsPersegment[j].TMHits.Length; i++)
                {
                    hits[j][i] = new TMHit(tmHitsPersegment[j].TMHits[i], TM.SourceLanguageCode, TM.TargetLanguageCode);
                }
            }
            return hits;
        }

        private async Task<MemoQServerTypes.ConcordanceResult> concordanceLookupAsync(string tmGuid, List<string> expressions, ConcordanceRequest options)
        {
            if (!isActive) return null;
            if (!isLoggedIn) Login(this.settings.UserName, this.settings.Password, this.settings.LoginType);

            ConcordanceParam param = new ConcordanceParam(expressions, options.ToMemoQServerConcOptions());
            MemoQServerTypes.ConcordanceResult result = null;
            try
            {
                result = await request<MemoQServerTypes.ConcordanceResult>(HttpMethod.Post, apiURL + String.Format(AppData.ServerURLConcordance, tmGuid.ToString()), param);
            }
            catch (Exception e)
            {
                string tmName = GetTMName(new Guid(tmGuid));
                collectException(e, tmGuid, tmName);
                return null;
            }
            return result;
        }

        private async Task addTMEntryAsync(string tmGuid, TranslationUnit tmEntry)
        {
            if (!isActive) return;
            if (!isLoggedIn) Login(this.settings.UserName, this.settings.Password, this.settings.LoginType);

            TMEntryModel entry = tmEntry.ToMemoQServerTMEntryModel();
            try
            {
                await request(HttpMethod.Post, apiURL + String.Format(AppData.ServerURLAddEntry, tmGuid.ToString()), entry);
            }
            catch (Exception e)
            {
                string tmName = GetTMName(new Guid(tmGuid));
                collectException(e, tmGuid, tmName);
            }
        }

        private async Task updateEntryAsync(string tmGuid, TranslationUnit tmEntry, int tuId)
        {
            if (!isActive) return;
            if (!isLoggedIn) Login(this.settings.UserName, this.settings.Password, this.settings.LoginType);

            TMEntryModel entry = tmEntry.ToMemoQServerTMEntryModel();

            try
            {
                await request(HttpMethod.Post, apiURL + String.Format(AppData.ServerURLUpdateEntry, tmGuid, tuId), entry);
            }
            catch (Exception e)
            {
                string tmName = GetTMName(new Guid(tmGuid));
                collectException(e, tmGuid, tmName);
            }
        }

        private async Task updateOrAddEntryAsync(TMInfo tm, TranslationUnit tu, LookupSegmentRequest options)
        {
            try
            {
                // disable context for now
                QuerySegment qs = new QuerySegment(tu.ContextID, tu.SourceSegment, null, null); //tu.PrecedingSegment, tu.FollowingSegment);
                // this has to be done first in order to know if it's
                TMProvider.TMHit[] hits = await lookupSegmentAsync(qs, options, tm);

                // the tu should contain the data for update: key, modified and tm guid
                if (hits != null && hits.Length > 0)
                {
                    // sort by match rate
                    Array.Sort(hits);
                    tu.Key = hits[0].TranslationUnit.Key;
                    await updateEntryAsync(tm.TMGuid.ToString(), tu, tu.Key);
                }
                else
                {
                    // if there's no result froom the lookup: add entry
                    await addTMEntryAsync(tm.TMGuid.ToString(), tu);
                }


            }
            catch (Exception e)
            {
                tmErrors[tm.FriendlyName] = 0;
                collectException(e);
            }
        }

        #endregion // TM Operations

        #region TM operations from base class

        protected override List<TMHit>[] onLookup(LookupData lookupData)
        {
            // do the lookup for the segments in each TM
            List<TMHit>[] results = new List<TMHit>[lookupData.Segments.Count];
            if (!isActive) return results;

            TMProvider.TMInfo[] lookupTMs = GetLookupTMs(lookupData.SourceLangCode, lookupData.TargetLangCode, lookupData.CanReverseLangs, lookupData.AllowSubLangs);
            if (lookupTMs.Length == 0) return results;

            List<Task<TMHit[][]>> lookupTasks = new List<Task<TMHit[][]>>();

            for (int i = 0; i < lookupTMs.Length; i++)
            {
                TMInfo tmInfo = lookupTMs[i];
                // do the lookup in one TM
                bool doReverse = false;
                // if languages don't match do a reverse lookup
                if (lookupData.CanReverseLangs && !tmInfo.SourceLanguageCode.StartsWith(lookupData.SourceLangCode.Substring(0, 3))) doReverse = true;
                lookupData.LookupSettings.SetReverseLookup(doReverse);
                try
                {
                    Task<TMHit[][]> t = lookupSegmentsAsync(lookupData.Segments.ToArray(), lookupData.LookupSettings, tmInfo, lookupData.MatchThreshold);
                    // we'll wait for the first TM to finish, because if the token has expired, all the tasks try to login almost simultaneously
                    // we'll wait for the first task to finish re-login
                    if (i == 0)
                    {
                        t.Wait();
                        TMHit[][] oneTMHits = t.Result;
                    }
                    lookupTasks.Add(t);
                }
                catch (Exception e)
                {
                    tmErrors[tmInfo.FriendlyName] = 0;
                    collectException(e);
                }
            }
            // wait for all tasks to finish, and then process the results (add them to the collection and set TM name)
            Task.WaitAll(lookupTasks.ToArray());
            try
            {
                for (int j = 0; j < lookupTasks.Count; j++)
                {
                    TMHit[][] oneTMHits = lookupTasks[j].Result;
                    int i = 0;
                    foreach (TMHit[] hits in oneTMHits)
                    {
                        if (hits == null || hits.Length == 0)
                        {
                            i++;
                            continue;
                        }
                        foreach (TMHit hit in hits)
                        {
                            hit.TMName = lookupTMs[j].FriendlyName;
                            hit.TMGuid = lookupTMs[j].TMGuid;
                        }
                        if (results[i] == null) results[i] = new List<TMHit>();
                        results[i].AddRange(hits);
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                collectException(e);
            }
            return results;
        }

        protected override void onAdd(LookupData data)
        {
            if (!isActive) return;

            TMProvider.TMInfo[] updateTMs = GetUpdateTMs(data.SourceLangCode, data.TargetLangCode, !data.AllowSubLangs);
            if (updateTMs.Length == 0) return;

            List<Task> updateTasks = new List<Task>();

            foreach (TMProvider.TMInfo tm in updateTMs)
            {
                int i = 0;
                foreach (TMHit hit in data.UpdateTUs)
                {
                    try
                    {
                        Task t = addTMEntryAsync(tm.TMGuid.ToString(), hit.TranslationUnit);
                        updateTasks.Add(t);
                        // we'll wait for the first TM to finish, because if the token has expired, all the tasks try to login almost simultaneously
                        // we'll wait for the first task to finish re-login
                        if (i == 0) t.Wait();
                    }
                    catch (Exception e)
                    {
                        tmErrors[tm.FriendlyName] = 0;
                        collectException(e);
                    }
                    i++;
                }
            }

            Task.WaitAll(updateTasks.ToArray());
        }

        protected override List<ConcordanceResult> onConcordance(LookupData data)
        {
            List<ConcordanceResult> result = new List<ConcordanceResult>();
            if (!isActive) return result;

            TMProvider.TMInfo[] lookupTMs = GetLookupTMs(data.SourceLangCode, data.TargetLangCode, data.CanReverseLangs, !data.AllowSubLangs);
            if (lookupTMs.Length == 0) return result;

            List<Task<MemoQServerTypes.ConcordanceResult>> concTasks = new List<Task<MemoQServerTypes.ConcordanceResult>>();

            if (data.Segments == null || data.Segments.Count == 0) return result;
            int i = 0;
            foreach (TMProvider.TMInfo tm in lookupTMs)
            {
                try
                {
                    Task<MemoQServerTypes.ConcordanceResult> t = concordanceLookupAsync(tm.TMGuid.ToString(), new List<string> { data.Segments[0].Segment }, data.ConcordanceSettings);
                    concTasks.Add(t);
                    // we'll wait for the first TM to finish, because if the token has expired, all the tasks try to login almost simultaneously
                    // we'll wait for the first task to finish re-login
                    if (i == 0) t.Wait();
                }
                catch (Exception e)
                {
                    tmErrors[tm.FriendlyName] = 0;
                    collectException(e);
                }
                // wait for all tasks and collect results
                Task.WaitAll(concTasks.ToArray());
                foreach (Task<MemoQServerTypes.ConcordanceResult> t in concTasks)
                {
                    if (t.Result != null) result.Add(new TMProvider.ConcordanceResult(t.Result, tm.SourceLanguageCode, tm.TargetLanguageCode));
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// Updates all translation units in the lookupdata. Remove the masked ones before calling this function!
        /// </summary>
        /// <param name="data"></param>
        protected override void onUpdate(LookupData data)
        {
            if (!isActive) return;

            TMProvider.TMInfo[] updateTMs = GetUpdateTMs(data.SourceLangCode, data.TargetLangCode, false);
            if (updateTMs.Length == 0) return;

            List<Task> updateTasks = new List<Task>();
            // only update in one TM, not in all of them
            // TM guid should be in the tmhit from the lookupdata

            int i = 0;
            // update or add each entry
            foreach (TMHit hit in data.UpdateTUs)
            {
                try
                {
                    // no TM information
                    if (hit.TMGuid == Guid.Empty) continue;
                    // no such TM
                    if (!updateTMs.Any(tm => tm.TMGuid == hit.TMGuid)) continue;

                    // add entry: if duplicates are allowed, the entry is added, if not, updated
                    Task t = updateEntryAsync(hit.TMGuid.ToString(), hit.TranslationUnit, hit.TranslationUnit.Key);
                    //Task t = updateOrAddEntryAsync(tm, hit.TranslationUnit, options);
                    updateTasks.Add(t);
                    // we'll wait for the first TM to finish, because if the token has expired, all the tasks try to login almost simultaneously
                    // we'll wait for the first task to finish re-login
                    if (i == 0) t.Wait();
                }
                catch (Exception e)
                {
                    tmErrors[hit.TMName] = 0;
                    collectException(e);
                }
                i++;
            }

            Task.WaitAll(updateTasks.ToArray());
        }

        #endregion

        #region Clone

        public override TMProviderBase Clone()
        {
            TMProviderBase p = new TMProviderMQServer(settings.Clone());
            return p;
        }

        #endregion

        #region Server communication

        private async Task<T> request<T>(HttpMethod method, string url, object param)
        {
            HttpResponseMessage responseMessage = await getResponseMessage(method, url, param).ConfigureAwait(false);

            if (responseMessage == null) return default(T);
            T result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                bool resendRequest = await handleHttpError(responseMessage, getOperationFromURL(url)).ConfigureAwait(false);
                if (resendRequest)
                {
                    result = await request<T>(method, url, param).ConfigureAwait(false);
                    return result;
                }
                if (param != null && !(param is LoginParam)) resendCount = 0;
                return default(T);
            }

            result = await deserializeResponse<T>(responseMessage).ConfigureAwait(false);
            if (param == null || !(param is LoginParam)) resendCount = 0;
            return result;
        }

        /// <summary>
        /// Request without a return value.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task request(HttpMethod method, string url, object param)
        {
            HttpResponseMessage responseMessage = await getResponseMessage(method, url, param).ConfigureAwait(false);

            if (responseMessage == null) return;

            //  TODO: error handling (responseMessage.IsSuccessStatusCode is false)
            if (!responseMessage.IsSuccessStatusCode)
            {
                bool resendRequest = await handleHttpError(responseMessage, getOperationFromURL(url));
                if (resendRequest)
                {
                    await request(method, url, param).ConfigureAwait(false);
                }
            }
            if (param == null || !(param is LoginParam)) resendCount = 0;
        }

        private Operations getOperationFromURL(string url)
        {
            if (url.Contains("lookup")) return Operations.Lookup;
            if (url.Contains("concordance")) return Operations.Concordance;
            if (url.Contains("create")) return Operations.Add;
            if (url.Contains("update")) return Operations.Update;
            return Operations.None;
        }

        private async Task<HttpResponseMessage> getResponseMessage(HttpMethod method, string uri, object param)
        {
            // WebRequestHandler is only available in .NET 4.5 and above. In the older versions of the framework, the
            // validation callback has to be set on AppDomain level through the "ServerCertificateValidationCallback"
            // member of "System.Net.ServicePointManager".
            using (WebRequestHandler webRequestHandler = new WebRequestHandler())
            {
                webRequestHandler.ServerCertificateValidationCallback = onValidateCertificate;
                using (var client = new HttpClient(webRequestHandler)) // new HttpClientHandler { UseDefaultCredentials = true }))
                {
                    var requestMessage = new HttpRequestMessage(method, uri);
                    requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (this.currentLoginData != null) requestMessage.Headers.Add("Authorization", "MQS-API " + this.currentLoginData.AccessToken);
                    if (method == HttpMethod.Post && param != null)
                    {
                        var jsonStr = JsonConvert.SerializeObject(param);
                        requestMessage.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                    }
                    // http://stackoverflow.com/questions/10343632/httpclient-getasync-never-returns-when-using-await-async/10351400#10351400
                    HttpResponseMessage responseMessage = null;
                    try
                    {
                        responseMessage = await client.SendAsync(requestMessage).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        // eg. if server is not running
                        //isActive = false; - keep trying
                        isLoggedIn = false;
                        collectException(e);
                        return default(HttpResponseMessage);
                    }
                    return responseMessage;
                }
            }
        }

        private async Task<T> deserializeResponse<T>(HttpResponseMessage responseMessage)
        {
            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var readponseReader = new StreamReader(responseStream))
            {
                var jsonMessage = readponseReader.ReadToEnd();
                return (T)JsonConvert.DeserializeObject(jsonMessage, typeof(T));
            }
        }

        private async Task<bool> handleHttpError(HttpResponseMessage responseMessage, Operations operation)
        {
            try
            {
                using (var responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var readponseReader = new StreamReader(responseStream))
                {
                    var jsonMessage = readponseReader.ReadToEnd();
                    RequestError err = (RequestError)JsonConvert.DeserializeObject(jsonMessage, typeof(RequestError));
                    bool resendRequest;
                    if (collectException(err, operation, out resendRequest)) return resendRequest;
                }
            }
            catch (JsonException e)
            {
                // deserialization didn't succeed
                collectException(new ServerException(responseMessage.ReasonPhrase + "\n" + e.StackTrace));
                if (responseMessage.StatusCode == HttpStatusCode.Forbidden || responseMessage.StatusCode == HttpStatusCode.NotFound) isActive = false;
            }
            return false;
            //collectException(responseMessage);
        }

        #endregion

        #region Exceptions

        protected override bool collectException(Exception e, string tmGuid = "", string tmName = "")
        {
            // aggregate exception has to be separated into individual exceptions
            if (e is AggregateException)
            {
                AggregateException ae = e as AggregateException;
                foreach (var innerException in ae.InnerExceptions)
                {
                    collectException(innerException, tmGuid, tmName);
                }
            }
            else if (e.InnerException != null)
            {
                collectException(e.InnerException);
            }
            // for certain types of exceptions
            // if the tm doesn't exist: remove it from the list
            // if there's no lookup right: remove it from the list - currently this cannot be differentiated: it's just not in the list
            // if there's no update right: replace it with new tminfo with only lookup rights and purpose
            else if (e is TMNotFoundException)
            {
                // remove TM from the user's local list
                if (tmGuid != "") RemoveUsedTM(new Guid(tmGuid));
                e.Data.Add("tm", tmName);
                e.Data.Add("stack", e.ToString());
                exceptionList.Enqueue(e);
            }
            else if (e is UnauthorizedTMReadException)
            {
                // change rights to only lookup
                if (tmGuid != "") SetTMPurpose(new Guid(tmGuid), TMPurposes.None);
                e.Data.Add("tm", tmName);
                e.Data.Add("stack", e.ToString());
                exceptionList.Enqueue(e);
            }
            else if (e is UnauthorizedTMWriteException)
            {
                // change rights to only lookup
                if (tmGuid != "") SetTMPurpose(new Guid(tmGuid), TMPurposes.Lookup);
                e.Data.Add("tm", tmName);
                e.Data.Add("stack", e.ToString());
                exceptionList.Enqueue(e);
            }
            else if (e is System.Security.Authentication.AuthenticationException)
            {
                if (!e.Message.Contains("certificate"))
                {
                    e.Data.Add("stack", e.ToString());
                    exceptionList.Enqueue(e);
                }
            }
            else
            {
                e.Data.Add("stack", e.ToString());
                exceptionList.Enqueue(e);
            }

            Log.WriteToLog(e.ToString());
            return true;
        }

        /// <summary>
        /// Checks the error code, creates a custom exception, handles the error and adds the exception to the list if necessary.
        /// Rethrows TM specific exceptions.
        /// </summary>
        /// <param name="requestError"></param>
        /// <returns>True if the exception was handled (added or not added to the list).</returns>
        private bool collectException(RequestError requestError, Operations operation, out bool resendRequest)
        {
            Exception e = null;
            resendRequest = false;
            if (requestError.ErrorCode == InvalidOrExpiredToken)
            {
                isLoggedIn = false;
                currentLoginData = null;
                // try login again
                RetryLogin(settings.UserName, settings.Password, settings.LoginType);
                // if still not logged in, provider is deactivated
                if (!isLoggedIn || resendCount > 0) e = new AuthenticationException(requestError.Message);
                // if logged in, resend the request
                else
                {
                    resendCount++;
                    resendRequest = true;
                    // don't add this exception
                    e = null;
                }
            }
            else if (requestError.ErrorCode == AuthenticationFailed)
            {
                e = new AuthenticationException(requestError.Message);
                // we won't try login again, just deactivate provider
                isLoggedIn = false;
                isActive = false;
            }
            else if (requestError.ErrorCode == TooFrequentLogin)
            {
                // seems like illegal use
                e = new TooFrequentLoginException(requestError.Message);
                // deactivate provider
                isLoggedIn = false;
                isActive = false;
            }
            else if (requestError.ErrorCode == NoLicenseAvailable)
            {
                // try login again
                // only resend the request
                if (!isLoggedIn || resendCount > 0) e = new NoLicenseException(requestError.Message);
                // if logged in, resend the request
                else
                {
                    resendRequest = true;
                    resendCount++;
                }
            }
            else if (requestError.ErrorCode == ReverseLookupNotSupported)
            {
                // don't do anything, silently swallow it
                // but we'll add it to the list and decide about it later
                e = new ReverseLookupException(requestError.Message);
            }
            else if (requestError.ErrorCode == InternalServerError)
            {
                // just collect the error
                e = new GeneralServerException(requestError.Message + "\n" + Environment.StackTrace);
            }
            // rethrow TM-specific errors
            else if (requestError.ErrorCode == ResourceNotFound)
            {
                // just throw the error, and then collect at the operation itself
                throw new TMNotFoundException(requestError.Message);
            }
            else if (requestError.ErrorCode == OptimisticConcurrencyError)
            {
                // just throw the error, and then collect at the operation itself
                e = new ServerException(requestError.Message + "\n" + Environment.StackTrace);
            }
            else if (requestError.ErrorCode == Unauthorized && (operation == Operations.Add || operation == Operations.Update))
            {
                // just throw the error, and then collect at the operation itself
                throw new UnauthorizedTMWriteException(requestError.Message);
            }
            else if (requestError.ErrorCode == Unauthorized && (operation == Operations.Lookup || operation == Operations.Concordance))
            {
                // just throw the error, and then collect at the operation itself
                throw new UnauthorizedTMReadException(requestError.Message);
            }
            else e = new Exception(requestError.Message);

            // we collect the exceptions and display a summarized message later
            if (e != null)
            {
                e.Data.Add("stack", requestError.Message);
                exceptionList.Enqueue(e);
            }

            return true;
        }

        public override void ClearExceptions()
        {
            exceptionList = new ConcurrentQueue<Exception>();
        }

        #endregion
    }
}
