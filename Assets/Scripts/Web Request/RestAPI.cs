using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace VRInnocent.RestAPI
{
    public class RestAPI : MonoBehaviour
    {
        public TargetAPIConfig targetAPIConfig;
        private string endpointTitle = "";

        string tempDataJson;

        #region RestAPI Services
        public void PostAction(Dictionary<string, string> _data, Action<JObject> success, Action<JObject> err, Action<JObject> dataErr, string _endpointTitle, string uniqValue = "")
        {
            endpointTitle = _endpointTitle;

            var jsonDataToSend = JsonConvert.SerializeObject(_data);

            tempDataJson = jsonDataToSend;

            object[] callbacks = new object[4] { success, err, dataErr, uniqValue };

            StartCoroutine(nameof(Post), callbacks);
        }

        public void PostActionCustom(Dictionary<string, int> _data, Action<JObject> success, Action<JObject> err, Action<JObject> dataErr, string _endpointTitle, string uniqValue = "")
        {
            endpointTitle = _endpointTitle;

            var jsonDataToSend = JsonConvert.SerializeObject(_data);

            tempDataJson = jsonDataToSend;

            object[] callbacks = new object[4] { success, err, dataErr, uniqValue };

            StartCoroutine(nameof(Post), callbacks);
        }

        public void GetActionByID(string _endpointTitle, string userId, Action<JObject> success, Action<JObject> err)
        {
            object[] callbacks = new object[3] { targetAPIConfig.url + targetAPIConfig.GetEndpoint(_endpointTitle) + userId, err, success };
            StartCoroutine(nameof(Get), callbacks);
        }

        public void PatchAction(string _endpointTitle, string patchData, Action<JObject> success, Action<JObject> err)
        {
            //Debug.Log(targetAPIConfig.url + targetAPIConfig.GetEndpoint(_endpointTitle) + patchData);
            object[] callbacks = new object[3] { targetAPIConfig.url + targetAPIConfig.GetEndpoint(_endpointTitle) + patchData, success, err };
            StartCoroutine(nameof(Patch), callbacks);
        }

        public void PatchActionWithBody(Dictionary<string, string> _data, string _endpointTitle, string patchData, Action<JObject> success, Action<JObject> err)
        {
            var jsonDataToSend = JsonConvert.SerializeObject(_data);
            tempDataJson = jsonDataToSend;

            // Debug.Log(tempDataJson);

            object[] callbacks = new object[3] { targetAPIConfig.url + targetAPIConfig.GetEndpoint(_endpointTitle) + patchData, success, err };
            StartCoroutine(nameof(PatchWithBody), callbacks);
        }

        #endregion


        IEnumerator Get(object[] callback)
        {
            string uri = (string)callback[0];
            Action<JObject> err = (Action<JObject>)callback[1];
            Action<JObject> success = (Action<JObject>)callback[2];

            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    string responseDataProcessingError = request.downloadHandler.text;
                    JObject dataDataProcessingError = JObject.Parse(responseDataProcessingError);
                    // Debug.LogError(dataDataProcessingError);

                    err?.Invoke(dataDataProcessingError);
                }
                else
                {
                    string json = request.downloadHandler.text;
                    // Debug.Log(json); 
                    JObject dataSuccess = JObject.Parse(json);
                    success?.Invoke(dataSuccess);
                }
            }
        }

        IEnumerator Post(object[] callback)
        {
            Action<JObject> successCallback = (Action<JObject>)callback[0];
            Action<JObject> errCallback = (Action<JObject>)callback[1];
            Action<JObject> dataErrCallback = (Action<JObject>)callback[2];
            string uniqVal = (string)callback[3];

            string uri = targetAPIConfig.url + targetAPIConfig.GetEndpoint(endpointTitle) + uniqVal;
            Debug.Log($"Start post request to: {uri}");

            using UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            byte[] rowData = Encoding.UTF8.GetBytes(tempDataJson);
            webRequest.uploadHandler = new UploadHandlerRaw(rowData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseSuccess = webRequest.downloadHandler.text;
                JObject dataSuccess = JObject.Parse(responseSuccess);
                successCallback?.Invoke(dataSuccess);
            }
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                int responseCode = (int)webRequest.responseCode;
                string responseError = webRequest.downloadHandler.text;

                try
                {
                    JObject dataError = JObject.Parse(responseError);

                    // Handle specific HTTP error codes
                    if (responseCode == 500)
                    {
                        Debug.LogError("Internal Server Error (500): " + dataError);
                        errCallback?.Invoke(dataError);
                    }
                    else
                    {
                        Debug.LogError("Protocol or Data Processing Error: " + dataError);
                        if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                        {
                            errCallback?.Invoke(dataError);
                        }
                        else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
                        {
                            dataErrCallback?.Invoke(dataError);
                        }
                    }
                }
                catch (JsonReaderException e)
                {
                    Debug.LogError("Failed to parse JSON error response: " + e.Message);
                    JObject dataError = new JObject
                    {
                        ["error"] = "Internal Server Error",
                        ["message"] = "An unexpected error occurred on the server.",
                        ["details"] = new JObject
                        {
                            ["timestamp"] = "2024-07-09T12:34:56Z",
                            ["path"] = "/api/example-endpoint",
                            ["status"] = 500,
                            ["errorId"] = "1234567890abcdef"
                        }
                    };

                    if (responseCode == 500)
                    {
                        errCallback?.Invoke(dataError);
                    }
                    else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                    {
                        errCallback?.Invoke(dataError);
                    }
                    else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
                    {
                        dataErrCallback?.Invoke(dataError);
                    }
                }
            }
            else
            {
                Debug.LogError("Unexpected Error: " + webRequest.error);
            }
        }

        private IEnumerator Patch(object[] callback)
        {
            string url = (string)callback[0];
            Action<JObject> successCallback = (Action<JObject>)callback[1];
            Action<JObject> errCallback = (Action<JObject>)callback[2];

            // Buat UnityWebRequest dengan method PUT
            UnityWebRequest webRequest = new UnityWebRequest(url, "PATCH");

            // Kirim request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                //string response = webRequest.downloadHandler.text;
                string response = "{\"Pesan\":\"success\"}";

                JObject dataDataProcessing = JObject.Parse(response);

                successCallback?.Invoke(dataDataProcessing);
            }
            else
            {
                //string response = webRequest.downloadHandler.text;
                string response = "{\"Pesan\":\"error\"}";
                JObject dataDataProcessing = JObject.Parse(response);

                errCallback?.Invoke(dataDataProcessing);
            }
        }

        private IEnumerator PatchWithBody(object[] callback)
        {
            string url = (string)callback[0];
            Action<JObject> successCallback = (Action<JObject>)callback[1];
            Action<JObject> errCallback = (Action<JObject>)callback[2];

            // Buat UnityWebRequest dengan method PUT
            UnityWebRequest webRequest = new UnityWebRequest(url, "PATCH");

            // Set data yang ingin Anda kirim
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(tempDataJson);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Kirim request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                //string response = webRequest.downloadHandler.text;
                string response = "{\"Pesan\":\"success\"}";

                JObject dataDataProcessing = JObject.Parse(response);

                successCallback?.Invoke(dataDataProcessing);
            }
            else
            {
                //string response = webRequest.downloadHandler.text;
                string response = "{\"Pesan\":\"error\"}";
                JObject dataDataProcessing = JObject.Parse(response);

                errCallback?.Invoke(dataDataProcessing);
            }
        }
    }
}