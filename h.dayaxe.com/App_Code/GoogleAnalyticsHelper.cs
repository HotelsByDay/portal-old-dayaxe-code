using DayaxeDal.GoogleAnalytics;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace h.dayaxe.com
{
    public static class GoogleAnalyticsHelper
    {
        public static List<GoogleAnalyticsSearchResult> GetData(GoogleAnalyticsSearchParams param)
        {
            var credential = GetCredential(param.IsStageSite);
            using (var svc = new AnalyticsReportingService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Analytics API Console",
                }))
            {
                var dateRange = new DateRange
                {
                    StartDate = param.FromDate.ToString("yyyy-MM-dd"),
                    EndDate = param.ToDate.ToString("yyyy-MM-dd")
                };
                var sessions = new Metric
                {
                    Expression = "ga:sessions",
                    Alias = "Sessions"
                };

                var reportRequest = new ReportRequest
                {
                    DateRanges = new List<DateRange> { dateRange },
                    Dimensions = param.GoogleAnalyticsDimensions,
                    Metrics = new List<Metric> { sessions },
                    ViewId = param.ViewId
                };
                var getReportsRequest = new GetReportsRequest
                {
                    ReportRequests = new List<ReportRequest> { reportRequest }
                };
                var batchRequest = svc.Reports.BatchGet(getReportsRequest);
                var response = batchRequest.Execute();
                var result = new List<GoogleAnalyticsSearchResult>();

                if (response != null)
                {
                    var report = response.Reports.FirstOrDefault();

                    if (report != null && report.Data != null && report.Data.Rows != null && report.Data.Rows.Any())
                    {
                        foreach (var x in report.Data.Rows)
                        {
                            result.Add(new GoogleAnalyticsSearchResult
                            {
                                Dimensions = x.Dimensions,
                                Values = x.Metrics.First().Values
                            });
                        }
                    }
                }

                return result;
            }
        }

        //static async Task<UserCredential> GetCredential(string emailAddress, bool isStage)
        static ServiceAccountCredential GetCredential(bool isStage)
        {
            //var clientSecrets = "\\App_Data\\client_secret.json";
            //if (isStage)
            //{
            //    clientSecrets = "\\App_Data\\stage_client_secret.json";
            //}
            //using (var stream = new FileStream(HttpContext.Current.Server.MapPath(clientSecrets), FileMode.Open, FileAccess.Read))
            //{
            //    return await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        new[] { AnalyticsReportingService.Scope.Analytics },
            //        emailAddress,
            //        CancellationToken.None,
            //        GetFileDataStore());
            //}

            var analiticsCred = "\\App_Data\\analytics.json";
            if (isStage)
            {
                analiticsCred = "\\App_Data\\stage-analytics.json";
            }

            var file = HttpContext.Current.Server.MapPath(analiticsCred);

            var json = File.ReadAllText(file);

            var creds = JsonConvert.DeserializeObject<GoogleAnalyticsServiceAccount>(json);

            return new ServiceAccountCredential(new ServiceAccountCredential.Initializer(creds.client_email, creds.token_uri)
            {
                Scopes = new[] {
                    AnalyticsReportingService.Scope.Analytics
                }
            }.FromPrivateKey(creds.private_key));
        }

        //static FileDataStore GetFileDataStore()
        //{
        //    var path = HttpContext.Current.Server.MapPath("~/App_Data/Drive.Api.Auth.Store");
        //    var store = new FileDataStore(path, true);
        //    return store;
        //}
    }
}
