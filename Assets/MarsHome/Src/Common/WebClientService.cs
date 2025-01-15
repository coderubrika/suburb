using Newtonsoft.Json;
using Proyecto26;
using Suburb.Utils;
using System;
using System.Collections.Generic;
using UniRx;

namespace Suburb.Common
{
    public class WebClientService
    {
        public IObservable<T> GetViaJson<T>(
            string url,
            string token,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            bool isIgnoreNetworkError)
        {
            return GetText(url, token, headers, parameters, isIgnoreNetworkError)
                .Select(JsonConvert.DeserializeObject<T>);
        }

        private IObservable<string> GetText(
            string url,
            string token,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters,
            bool isIgnoreNetworkError)
        {
            var request = GetRequestHelper(url, token, headers);
            request.Params = parameters;

            return Get(request, isIgnoreNetworkError)
                .Select(response => response.Text);
        }

        private IObservable<ResponseHelper> Get(RequestHelper requestHelper, bool isIgnoreNetworkError)
        {
            this.Log($"GET {requestHelper.Uri}");
            return PerformRequest(requestHelper, RestClient.Get, isIgnoreNetworkError);
        }

        private IObservable<ResponseHelper> Post(RequestHelper requestHelper, bool isIgnoreNetworkError)
        {
            this.Log($"POST {requestHelper.Uri}");
            return PerformRequest(requestHelper, RestClient.Post, isIgnoreNetworkError);
        }

        private IObservable<ResponseHelper> Delete(RequestHelper requestHelper, bool isIgnoreNetworkError)
        {
            this.Log($"DELETE {requestHelper.Uri}");
            return PerformRequest(requestHelper, RestClient.Delete, isIgnoreNetworkError);
        }

        private IObservable<ResponseHelper> PerformRequest(
            RequestHelper requestHelper,
            Action<RequestHelper, Action<RequestException, ResponseHelper>> requestAction,
            bool isIgnoreNetworkError)
        {
            //check progress and abort
            /*requestHelper.ProgressCallback = loadAssistant.Report;
            var cancelDisposable = loadAssistant.OnCancel
                .Subscribe(_ => requestHelper.Abort());*/

            Subject<ResponseHelper> resultSubject = new Subject<ResponseHelper>();
            requestAction(requestHelper,
                (requestException, responseHelper) =>
                {
                    //cancelDisposable?.Dispose();
                    HandleRequest(requestException, responseHelper, resultSubject, isIgnoreNetworkError);
                });

            return resultSubject;
        }

        private void HandleRequest(
            RequestException requestException,
            ResponseHelper responseHelper,
            Subject<ResponseHelper> resultSubject,
            bool isIgnoreNetworkError)
        {
            if (requestException != null)
            {
                string errorMessage =
                    $"Network error {responseHelper.Request.url}: {requestException.Message}; {requestException.InnerException?.Message}";

                if (isIgnoreNetworkError)
                    this.Log(errorMessage);
                else
                    this.LogError(errorMessage);

                resultSubject.OnError(requestException);
            }
            else
            {
                this.Log($"RESPONSE {responseHelper.Request?.url}: {responseHelper.StatusCode}");
                resultSubject.OnNext(responseHelper);
                resultSubject.OnCompleted();
            }
        }

        private RequestHelper GetRequestHelper(string url, string token, Dictionary<string, string> headers)
        {
            var requestHelper = new RequestHelper();
            requestHelper.Uri = url;
            requestHelper.Timeout = 25;
            requestHelper.RetryCallbackOnlyOnNetworkErrors = false;
            requestHelper.RetryCallback =
                (exception, tryNumber) => this.Log($"Request retry: {url}; {exception?.Message}; {tryNumber}");

            if (!string.IsNullOrEmpty(token))
                requestHelper.Headers.Add("Authorization", $"Bearer {token}");

            if (headers != null)
            {
                foreach (var headerPair in headers)
                    requestHelper.Headers.Add(headerPair.Key, headerPair.Value);
            }

            return requestHelper;
        }
    }
}
