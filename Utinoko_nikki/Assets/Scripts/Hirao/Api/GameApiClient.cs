
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AquaGarden.Api
{
    public sealed class GameApiClient : MonoBehaviour
    {
        [Header("Laravel API")]
        [SerializeField]
        private string baseUrl = "https://example.com/api";

        [SerializeField, Min(1)]
        private int timeoutSeconds = 15;

        public string BaseUrl => baseUrl.TrimEnd('/');

        public void Get(
            string path,
            Action<bool, string, long> onComplete)
        {
            Send("GET", path, null, onComplete);
        }

        public void Post(
            string path,
            string jsonBody,
            Action<bool, string, long> onComplete)
        {
            Send("POST", path, jsonBody, onComplete);
        }

        public void Put(
            string path,
            string jsonBody,
            Action<bool, string, long> onComplete)
        {
            Send("PUT", path, jsonBody, onComplete);
        }

        public void Delete(
            string path,
            Action<bool, string, long> onComplete)
        {
            Send("DELETE", path, null, onComplete);
        }

        private void Send(
            string method,
            string path,
            string jsonBody,
            Action<bool, string, long> onComplete)
        {
            if (!isActiveAndEnabled)
            {
                onComplete?.Invoke(false, "GameApiClient が無効です。", 0);
                return;
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                onComplete?.Invoke(false, "API の Base URL が未設定です。", 0);
                return;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                onComplete?.Invoke(false, "API のパスが未設定です。", 0);
                return;
            }

            string url = BaseUrl + "/" + path.TrimStart('/');
            StartCoroutine(SendCoroutine(
                method, url, jsonBody, onComplete));
        }

        private IEnumerator SendCoroutine(
            string method,
            string url,
            string jsonBody,
            Action<bool, string, long> onComplete)
        {
            using (var request = new UnityWebRequest(url, method))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = timeoutSeconds;

                if (method == "POST" || method == "PUT" ||
                    method == "PATCH")
                {
                    byte[] body = Encoding.UTF8.GetBytes(
                        string.IsNullOrEmpty(jsonBody) ? "{}" : jsonBody);

                    request.uploadHandler = new UploadHandlerRaw(body);
                    request.SetRequestHeader(
                        "Content-Type", "application/json");
                }

                request.SetRequestHeader(
                    "Accept", "application/json");

                yield return request.SendWebRequest();

                long statusCode = request.responseCode;
                string responseBody = request.downloadHandler != null
                    ? request.downloadHandler.text
                    : "";

                bool success =
                    request.result == UnityWebRequest.Result.Success;

                if (success)
                {
                    onComplete?.Invoke(
                        true, responseBody, statusCode);
                }
                else
                {
                    string error =
                        $"HTTP {statusCode}: {request.error}\n" +
                        responseBody;

                    Debug.LogWarning(
                        $"API request failed: {method} {url}\n{error}",
                        this);

                    onComplete?.Invoke(
                        false, error, statusCode);
                }
            }
        }
    }
}
