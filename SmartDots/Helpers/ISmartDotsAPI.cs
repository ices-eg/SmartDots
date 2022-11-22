using System;
using System.Collections.Generic;
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
    public interface ISmartDotsAPI
    {
        bool IsAuthenticated { get; set; }
        DtoSmartdotsSettings Settings { get; set; }
        User CurrentUser { get; set; }
        string Connection { get; set; }


        void Reset();
        WebApiResult<DtoSmartdotsSettings> GetSettings();
        WebApiResult<List<DtoReadabilityQuality>> GetQualities();
        WebApiResult<string> GetGuestToken();
        WebApiResult<DtoUser> Authenticate(DtoUserAuthentication userauthentication);
        WebApiResult<DtoAnalysis> GetAnalysis(Guid id);
        WebApiResult<List<dynamic>> GetAnalysesDynamic();
        DtoFolder GetFolder(string path, Guid? samplesetid);
        WebApiResult<DtoFile> GetFile(Guid id, bool withAnnotations, bool withSample);
        WebApiResult<List<DtoFile>> GetFiles(Guid analysisid, List<string> imagenames);
        WebApiResult<bool> UpdateAnnotations(List<DtoAnnotation> annotations);
        WebApiResult<bool> UpdateFile(DtoFile file);
        WebApiResult<bool> AddAnnotation(DtoAnnotation annotation);
        WebApiResult<bool> DeleteAnnotations(List<Guid> ids);
        WebApiResult<List<AnalysisSample>> GetAnalysisSamples(Guid id);
        WebApiResult<bool> UpdateAnalysisFolder(Guid analysisid, string folderpath);
        WebApiResult<bool> ToggleAnalysisUserProgress(Guid analysisid);
        WebApiResult<LoginToken> EstablishConnection(string connStr);
        WebApiResult<DtoMaturityAnalysis> GetMaturityAnalysis(Guid id);
        WebApiResult<DtoMaturitySample> GetMaturitySample(Guid id);
        WebApiResult<List<DtoMaturityLookupItem>> GetVocab(Guid analysisid, string code);
        WebApiResult<DtoMaturitySample> SaveMaturityAnnotation(DtoMaturityAnnotation annotation);
        WebApiResult<bool> UpdateMaturityFile(DtoMaturityFile file);
        WebApiResult<bool> ToggleMaturityAnalysisUserProgress(Guid analysisid);

        WebApiResult<DtoLarvaeAnalysis> GetLarvaeAnalysis(Guid id);
        WebApiResult<DtoLarvaeSample> GetLarvaeSample(Guid id);
        WebApiResult<DtoLarvaeSample> SaveLarvaeAnnotation(DtoLarvaeAnnotation annotation);
        WebApiResult<bool> UpdateLarvaeFile(DtoLarvaeFile file);
        WebApiResult<bool> ToggleLarvaeAnalysisUserProgress(Guid analysisid);

    }
}
