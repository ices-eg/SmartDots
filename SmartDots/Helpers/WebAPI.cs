using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SmartDots.Model;
using SmartDots.Model.Security;
using SmartDots.Model.Smartdots;

namespace SmartDots.Helpers
{
    public static class WebAPI
    {
        /// <summary>
        /// Duration in seconds after which WebAPI methods will automatically be stopped.
        /// </summary>
        private const int TimeOutAfter = 500;

        private static HttpClient client;

        public static bool IsAuthenticated { get; set; }
        public static DtoSmartdotsSettings Settings { get; set; }
        public static string Connection { get; private set; }
        public static User CurrentUser { get; private set; }

        public delegate void WebApiEventHandler(object sender, WebApiEventArgs e);
        public static event WebApiEventHandler OnError;

        private static void ReportError(string cmd, object obj, Exception ex)
        {
            var e = new WebApiEventArgs();
            e.Command = cmd;
            e.Object = obj;
            e.Error = ex;
            Helper.Log("Web API connection error", ex);
            OnError?.Invoke(null, e);
        }

        public static WebApiResult<LoginToken> EstablishConnection(string connStr)
        {
            //TODO: actually retrieve and store login token
            try
            {
                if (string.IsNullOrWhiteSpace(connStr)) throw new Exception("No API provided.");
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

        public static void Reset()
        {
            IsAuthenticated = false;
            Connection = null;
            CurrentUser = null;
            client?.Dispose();
        }

        private static WebApiResult<t> ParseResult<t>(string unparsedResult)
        {
            if (string.IsNullOrWhiteSpace(unparsedResult)) return new WebApiResult<t>();

            dynamic result = null;
            if (typeof(t) == typeof(Guid)) result = Guid.Parse(unparsedResult);
            else if (typeof(t) == typeof(bool)) result = bool.Parse(unparsedResult);
            else if (typeof(t) != typeof(string)) result = Newtonsoft.Json.JsonConvert.DeserializeObject<t>(unparsedResult);
            else if (typeof(t) == typeof(string)) result = unparsedResult;

            return new WebApiResult<t> { Result = result };
        }

        private static async Task<WebApiResult<t>> PerformCallAsync<t>(string path)
        {
            //initialize timeout variables
            var timespan = TimeSpan.FromSeconds(TimeOutAfter);
            var cancelToken = new CancellationTokenSource();
            try
            {
                //connect to client async
                var task = client.GetAsync(path, cancelToken.Token);
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

                    return ParseResult<t>(webapiresult.Result?.ToString());
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

        private static WebApiResult<t> PerformCall<t>(string path)
        {
            try
            {
                return Task.Run(() => PerformCallAsync<t>(path)).Result;
            }
            catch (Exception ex)
            {
                ReportError(path, null, ex);
                return new WebApiResult<t> { ErrorMessage = ex.Message };
            }
        }

        private static WebApiResult<t> PerformPost<t, u>(string path, u obj)
        {
            try
            {
                var response = client.PostAsJsonAsync(path, obj).Result;
                if (response.IsSuccessStatusCode)
                {
                    var apiPost = response.Content.ReadAsAsync<WebApiResult>().Result;
                    if (apiPost.Succeeded) return ParseResult<t>(apiPost.Result?.ToString());
                    else
                    {
                        throw new Exception(apiPost.ErrorMessage);
                    }
                }
                throw new Exception($"Error posting {typeof(t).Name} to WebAPI");
            }
            catch (Exception e)
            {
                ReportError(path, obj, e);
                return new WebApiResult<t> { ErrorMessage = e.Message };
            }
        }

        public static WebApiResult<DtoSmartdotsSettings> GetSettings() //server dependent
        {
            return PerformCall<DtoSmartdotsSettings>("getsettings?token=" + CurrentUser.Token);
        }

        public static WebApiResult<List<DtoReadabilityQuality>> GetQualities() //server dependent
        {
            return PerformCall<List<DtoReadabilityQuality>>("getreadabilityqualities?token=" + CurrentUser.Token);
        }

        public static WebApiResult<string> GetGuestToken()
        {
            return PerformCall<string>("getguesttoken");
        }

        public static WebApiResult<DtoUser> Authenticate(DtoUserAuthentication userauthentication)
        {
            var result = PerformPost<DtoUser, DtoUserAuthentication>("authenticate", userauthentication);
            if (string.IsNullOrWhiteSpace(result.ErrorMessage)) CurrentUser = (User)Helper.ConvertType(result.Result, typeof(User));
            return result;
        }

        public static WebApiResult<DtoAnalysis> GetAnalysis(Guid id)
        {
            return PerformCall<DtoAnalysis>("getanalysis?token=" + CurrentUser.Token + "&id=" + id);
        }

        public static WebApiResult<List<dynamic>> GetAnalysesDynamic()
        {
            return PerformCall<List<dynamic>>("getanalysesdynamic?token=" + CurrentUser.Token );
        }

        public static DtoFolder GetFolder(string path, Guid? samplesetid)
        {
            return PerformCall<DtoFolder>("getfolder?path=" + path + "&samplesetid=" + samplesetid).Result;
        }

        public static WebApiResult<DtoFile> GetFile(Guid id, bool withAnnotations, bool withSample)
        {
            return PerformCall<DtoFile>("getfilewithsampleandannotations?token=" + CurrentUser.Token + "&id=" + id + 
                "&withAnnotations=" + withAnnotations + "&withSample=" + withSample);
        }

        public static WebApiResult<List<DtoFile>> GetFiles(Guid analysisid, List<string> imagenames)
        {
            return PerformPost<List<DtoFile>, List<string>>("getfiles?token=" + CurrentUser.Token + "&analysisid=" + analysisid, imagenames);
        }

        //public static Dictionary<string, string> GetSampleProperties(Guid sampleid)
        //{
        //    return PerformCall<Dictionary<string, string>>("getsampleproperties?token=" + CurrentUser.Token + "&sampleid=" + sampleid).Result;
        //}

        public static WebApiResult<bool> UpdateAnnotations(List<DtoAnnotation> annotations)
        {
            return PerformPost<bool, List<DtoAnnotation>>("updateannotations?token=" + CurrentUser.Token, annotations);
        }

        public static WebApiResult<bool> UpdateFile(DtoFile file)
        {
            return PerformPost<bool, DtoFile>("updatefile?token=" + CurrentUser.Token, file);
        }

        public static WebApiResult<bool> AddAnnotation(DtoAnnotation annotation)
        {
            return PerformPost<bool, DtoAnnotation>("addannotation?token=" + CurrentUser.Token, annotation);
        }

        public static WebApiResult<bool> DeleteAnnotations(List<Guid> ids)
        {
            return PerformPost<bool, List<Guid>>("deleteannotations?token=" + CurrentUser.Token, ids);
        }

        public static WebApiResult<List<AnalysisSample>> GetAnalysisSamples(Guid id)
        {
            return PerformCall<List<AnalysisSample>>("getanalysissamples?token=" + CurrentUser.Token + "&id=" + id);
        }
        public static WebApiResult<bool> UpdateAnalysisFolder(Guid analysisid, string folderpath)
        {
            string folder = System.Net.WebUtility.UrlEncode(folderpath);
            return PerformPost<bool, Guid>("updateanalysisfolder?token=" + CurrentUser.Token + "&folderpath=" + folder, analysisid);
        }
    }
}
