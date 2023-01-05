using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartDots.Model;
using SmartDots.Model.Security;
using SmartDots.Model.Smartdots;

namespace SmartDots.Helpers
{
    public class WebAPI : ISmartDotsAPI
    {
        /// <summary>
        /// Duration in seconds after which WebAPI methods will automatically be stopped.
        /// </summary>
        private const int TimeOutAfter = 500;

        private HttpClient client;

        public bool IsAuthenticated { get; set; }
        public DtoSmartdotsSettings Settings { get; set; }
        public string Connection { get; set; }
        public User CurrentUser { get; set; }

        public delegate void WebApiEventHandler(object sender, WebApiEventArgs e);
        public event WebApiEventHandler OnError;

        private void ReportError(string cmd, object obj, Exception ex)
        {
            var e = new WebApiEventArgs();
            e.Command = cmd;
            e.Object = obj;
            e.Error = ex;
            Helper.Log("errors.txt","Web API connection error", ex);
            OnError?.Invoke(null, e);
        }

        public WebApiResult<LoginToken> EstablishConnection(string connStr)
        {
            //TODO: actually retrieve and store login token
            try
            {
                if (string.IsNullOrWhiteSpace(connStr)) throw new Exception("No API provided.");
                connStr = connStr.Trim();
                if (!connStr.EndsWith("/")) connStr += "/";

                Reset();
                client = new HttpClient { BaseAddress = new Uri(connStr) };
                Connection = connStr;

                return new WebApiResult<LoginToken>();
            }
            catch (Exception e)
            {
                ReportError("connect", null, e);
                return new WebApiResult<LoginToken> { ErrorMessage = e.Message };
            }
        }

        WebApiResult<LoginToken> ISmartDotsAPI.EstablishConnection(string connStr)
        {
            return EstablishConnection(connStr);
        }



        private static WebApiResult<t> ParseResult<t>(string unparsedResult)
        {
            //Helper.LogWebAPIResult(unparsedResult);
            if (string.IsNullOrWhiteSpace(unparsedResult)) return new WebApiResult<t>();

            dynamic result = null;
            if (typeof(t) == typeof(Guid)) result = Guid.Parse(unparsedResult);
            else if (typeof(t) == typeof(bool)) result = bool.Parse(unparsedResult);
            else if (typeof(t) != typeof(string)) result = Newtonsoft.Json.JsonConvert.DeserializeObject<t>(unparsedResult);
            else if (typeof(t) == typeof(string)) result = unparsedResult;

            return new WebApiResult<t> { Result = result};
        }

        private async Task<WebApiResult<t>> PerformCallAsync<t>(string path)
        {
            //initialize timeout variables
            var timespan = TimeSpan.FromSeconds(TimeOutAfter);
            var cancelToken = new CancellationTokenSource();
            try
            {
                Task<HttpResponseMessage> task;

                if (Settings?.MaturityAPI != null && (path.ToLower().Contains("maturity") || path.ToLower().Contains("vocab")))
                {
                    var maturityClient = new HttpClient { BaseAddress = new Uri(Settings.MaturityAPI) };
                    task = maturityClient.GetAsync(path, cancelToken.Token);
                }
                else
                {
                    task = client.GetAsync(path, cancelToken.Token);
                }
                
                if (await Task.WhenAny(task, Task.Delay(timespan)) != task)
                {
                    cancelToken.Cancel(true);
                    throw new Exception("Connection to API timed out.");
                }

                //retrieve data async
                var response = task.Result;
                if (response.IsSuccessStatusCode)
                {
                    var apiTask = response.Content.ReadAsAsync<WebApiResult>();
                    if (await Task.WhenAny(apiTask, Task.Delay(timespan)) != apiTask)
                    {
                        cancelToken.Cancel(true);
                        throw new Exception("the API did not respond in time.");
                    }

                    //parse received data
                    var webapiresult = apiTask.Result;
                    if (!string.IsNullOrWhiteSpace(webapiresult.ErrorMessage))
                        throw new Exception(webapiresult.ErrorMessage);

                    var res = ParseResult<t>(webapiresult.Result?.ToString());
                    res.WarningMessage = webapiresult.WarningMessage;
                    return res;
                }
                else if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Not supported on this Web API");
                }
                throw new Exception($"Error posting {typeof(t).Name} to WebAPI");
            }
            catch (Exception e)
            {
                cancelToken.Cancel(false);
                ReportError(path, null, e);
                return new WebApiResult<t> { ErrorMessage = e.Message };
            }
        }

        private WebApiResult<t> PerformCall<t>(string path)
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                //return Task.Run(() => PerformCallAsync<t>(path)).Result;
                var temp = Task.Run(() => PerformCallAsync<t>(path)).Result;
                timer.Stop();
                //Helper.Log("webapi.txt", path + Environment.NewLine + JsonConvert.SerializeObject(temp));
                //Helper.Log("timer.txt", $"{path}: {timer.ElapsedMilliseconds} ms" + Environment.NewLine);
                return temp;
            }
            catch (Exception ex)
            {
                ReportError(path, null, ex);
                return new WebApiResult<t> { ErrorMessage = ex.Message };
            }
        }

        private WebApiResult<t> PerformPost<t, u>(string path, u obj)
        {
            try
            {
                Task<HttpResponseMessage> task;

                Stopwatch timer = new Stopwatch();
                timer.Start();

                if (Settings != null && Settings.MaturityAPI != null && path.ToLower().Contains("maturity"))
                {
                    var maturityClient = new HttpClient { BaseAddress = new Uri(Settings.MaturityAPI) };
                    task = maturityClient.PostAsJsonAsync(path, obj);
                }
                else
                {
                    task = client.PostAsJsonAsync(path, obj);
                }


                var response = task.Result;

                //Helper.Log("timer.txt", $"{path}: {timer.ElapsedMilliseconds} ms" + Environment.NewLine);


                //Helper.Log("webapi.txt", path + Environment.NewLine + JsonConvert.SerializeObject(response));
                if (response.IsSuccessStatusCode)
                {
                    var apiPost = response.Content.ReadAsAsync<WebApiResult>().Result;
                    if (apiPost.Succeeded)
                    {
                        var res = ParseResult<t>(apiPost.Result?.ToString());
                        res.WarningMessage = apiPost.WarningMessage;
                        return res;
                    }
                    else
                    {
                        throw new Exception(apiPost.ErrorMessage);
                    }
                }
                //Helper.Log("api-log.txt", JsonConvert.SerializeObject(response));
                throw new Exception($"Error posting {typeof(t).Name} to WebAPI");
            }
            catch (Exception e)
            {
                ReportError(path, obj, e);
                return new WebApiResult<t> { ErrorMessage = e.Message };
            }
        }

        public void Reset()
        {
            IsAuthenticated = false;
            Connection = null;
            CurrentUser = null;
            client?.Dispose();
        }

        void ISmartDotsAPI.Reset()
        {
            Reset();
        }

        WebApiResult<DtoSmartdotsSettings> ISmartDotsAPI.GetSettings() //server dependent
        {
            return PerformCall<DtoSmartdotsSettings>("getsettings?token=" + CurrentUser.Token);
        }

        WebApiResult<List<DtoReadabilityQuality>> ISmartDotsAPI.GetQualities() //server dependent
        {
            return PerformCall<List<DtoReadabilityQuality>>("getreadabilityqualities?token=" + CurrentUser.Token);
        }

        WebApiResult<string> ISmartDotsAPI.GetGuestToken()
        {
            return PerformCall<string>("getguesttoken");
        }

        WebApiResult<DtoUser> ISmartDotsAPI.Authenticate(DtoUserAuthentication userauthentication)
        {
            var result = PerformPost<DtoUser, DtoUserAuthentication>("authenticate", userauthentication);
            if (string.IsNullOrWhiteSpace(result.ErrorMessage)) CurrentUser = (User)Helper.ConvertType(result.Result, typeof(User));
            return result;
        }

        WebApiResult<DtoAnalysis> ISmartDotsAPI.GetAnalysis(Guid id)
        {
            return PerformCall<DtoAnalysis>("getanalysis?token=" + CurrentUser.Token + "&id=" + id);
        }

        WebApiResult<List<dynamic>> ISmartDotsAPI.GetAnalysesDynamic()
        {
            return PerformCall<List<dynamic>>("getanalysesdynamic?token=" + CurrentUser.Token );
        }

        DtoFolder ISmartDotsAPI.GetFolder(string path, Guid? samplesetid)
        {
            return PerformCall<DtoFolder>("getfolder?path=" + path + "&samplesetid=" + samplesetid).Result;
        }

        WebApiResult<DtoFile> ISmartDotsAPI.GetFile(Guid id, bool withAnnotations, bool withSample)
        {
            return PerformCall<DtoFile>("getfilewithsampleandannotations?token=" + CurrentUser.Token + "&id=" + id + 
                "&withAnnotations=" + withAnnotations + "&withSample=" + withSample);
        }

        WebApiResult<List<DtoFile>> ISmartDotsAPI.GetFiles(Guid analysisid, List<string> imagenames)
        {
            return PerformPost<List<DtoFile>, List<string>>("getfiles?token=" + CurrentUser.Token + "&analysisid=" + analysisid, imagenames);
        }

        //public static Dictionary<string, string> GetSampleProperties(Guid sampleid)
        //{
        //    return PerformCall<Dictionary<string, string>>("getsampleproperties?token=" + CurrentUser.Token + "&sampleid=" + sampleid).Result;
        //}

        WebApiResult<bool> ISmartDotsAPI.UpdateAnnotations(List<DtoAnnotation> annotations)
        {
            return PerformPost<bool, List<DtoAnnotation>>("updateannotations?token=" + CurrentUser.Token, annotations);
        }

        WebApiResult<bool> ISmartDotsAPI.UpdateFile(DtoFile file)
        {
            return PerformPost<bool, DtoFile>("updatefile?token=" + CurrentUser.Token, file);
        }

        WebApiResult<bool> ISmartDotsAPI.AddAnnotation(DtoAnnotation annotation)
        {
            return PerformPost<bool, DtoAnnotation>("addannotation?token=" + CurrentUser.Token, annotation);
        }

        WebApiResult<bool> ISmartDotsAPI.DeleteAnnotations(List<Guid> ids)
        {
            return PerformPost<bool, List<Guid>>("deleteannotations?token=" + CurrentUser.Token, ids);
        }

        WebApiResult<List<AnalysisSample>> ISmartDotsAPI.GetAnalysisSamples(Guid id)
        {
            return PerformCall<List<AnalysisSample>>("getanalysissamples?token=" + CurrentUser.Token + "&id=" + id);
        }
        WebApiResult<bool> ISmartDotsAPI.UpdateAnalysisFolder(Guid analysisid, string folderpath)
        {
            string folder = System.Net.WebUtility.UrlEncode(folderpath);
            return PerformPost<bool, Guid>("updateanalysisfolder?token=" + CurrentUser.Token + "&folderpath=" + folder, analysisid);
        }
        WebApiResult<bool> ISmartDotsAPI.ToggleAnalysisUserProgress(Guid analysisid)
        {
            return PerformPost<bool, Guid>("toggleanalysisuserprogress?token=" + CurrentUser.Token, analysisid);
        }

        //WebApiResult<DtoSmartdotsSettings> ISmartDotsAPI.GetSettings() //server dependent
        //{
        //    return WebAPI.GetSettings();
        //}

        WebApiResult<DtoMaturityAnalysis> ISmartDotsAPI.GetMaturityAnalysis(Guid id)
        {
            return PerformCall<DtoMaturityAnalysis>("getmaturityanalysis?token=" + CurrentUser.Token + "&id=" + id);
        }

        public WebApiResult<DtoMaturitySample> GetMaturitySample(Guid id)
        {
            return PerformCall<DtoMaturitySample>("getmaturitysample?token=" + CurrentUser.Token + "&id=" + id);
        }

        public WebApiResult<DtoMaturitySample> SaveMaturityAnnotation(DtoMaturityAnnotation annotation)
        {
            return PerformPost<DtoMaturitySample, DtoMaturityAnnotation>("savematurityannotation?token=" + CurrentUser.Token, annotation);
        }

        public WebApiResult<bool> UpdateMaturityFile(DtoMaturityFile file)
        {
            return PerformPost<bool, DtoMaturityFile>("updatematurityfile?token=" + CurrentUser.Token, file);
        }
        public WebApiResult<bool> ToggleMaturityAnalysisUserProgress(Guid analysisid)
        {
            return PerformPost<bool, Guid>("togglematurityanalysisuserprogress?token=" + CurrentUser.Token, analysisid);
        }

        public WebApiResult<List<DtoMaturityLookupItem>> GetVocab(Guid analysisid, string code)
        {
            return PerformCall<List<DtoMaturityLookupItem>>("getvocab?token=" + CurrentUser.Token + "&codeType=" + code);
        }

        WebApiResult<DtoLarvaeAnalysis> ISmartDotsAPI.GetLarvaeAnalysis(Guid id)
        {
            return PerformCall<DtoLarvaeAnalysis>("getlarvaeanalysis?token=" + CurrentUser.Token + "&id=" + id);
        }

        public WebApiResult<DtoLarvaeSample> GetLarvaeSample(Guid id)
        {
            return PerformCall<DtoLarvaeSample>("getlarvaesample?token=" + CurrentUser.Token + "&id=" + id);
        }

        public WebApiResult<DtoLarvaeSample> SaveLarvaeAnnotation(DtoLarvaeAnnotation annotation)
        {
            return PerformPost<DtoLarvaeSample, DtoLarvaeAnnotation>("savelarvaeannotation?token=" + CurrentUser.Token, annotation);
        }

        public WebApiResult<bool> UpdateLarvaeFile(DtoLarvaeFile file)
        {
            return PerformPost<bool, DtoLarvaeFile>("updatelarvaefile?token=" + CurrentUser.Token, file);
        }
        public WebApiResult<bool> ToggleLarvaeAnalysisUserProgress(Guid analysisid)
        {
            return PerformPost<bool, Guid>("togglelarvaeanalysisuserprogress?token=" + CurrentUser.Token, analysisid);
        }
    }
}
