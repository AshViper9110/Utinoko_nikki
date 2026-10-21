
#if UNITY_EDITOR
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleOAuthEditor : MonoBehaviour
{
    [SerializeField] private string clientId;
    private bool isLoggingIn = false;

    private const string RedirectUri = "http://127.0.0.1:8765/";

    // Windowsの環境変数から取得
    private string ClientSecret =>
        Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET");

    [Serializable]
    private class TokenResponse
    {
        public string id_token;
        public string access_token;
        public string error;
    }

    public async void GoogleLogin()
    {

       
    
        if (isLoggingIn)
        {
            Debug.LogWarning("Googleログイン処理中です");
            return;
        }

        isLoggingIn = true;

        
            // 既存のGoogleログイン処理
            try
            {


            if (string.IsNullOrWhiteSpace(clientId))
            {
                Debug.LogError("Google Client IDが未設定です");
                return;
            }

            // PKCE用の検証文字列を作成
            byte[] randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            string verifier = Base64Url(randomBytes);

            byte[] hash;
            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(
                    Encoding.ASCII.GetBytes(verifier));
            }

            string challenge = Base64Url(hash);
            string state = Guid.NewGuid().ToString("N");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(RedirectUri);
                listener.Start();

                string url =
                    "https://accounts.google.com/o/oauth2/v2/auth" +
                    "?client_id=" + Uri.EscapeDataString(clientId) +
                    "&redirect_uri=" + Uri.EscapeDataString(RedirectUri) +
                    "&response_type=code" +
                    "&scope=" + Uri.EscapeDataString("openid email profile") +
                    "&code_challenge=" + challenge +
                    "&code_challenge_method=S256" +
                    "&state=" + state;

                // ブラウザでGoogle認証画面を開く
                Application.OpenURL(url);

                // Googleからのリダイレクトを受信
                var context = await listener.GetContextAsync();

                string returnedState =
                    context.Request.QueryString["state"];

                string code =
                    context.Request.QueryString["code"];

                string error =
                    context.Request.QueryString["error"];

                string message =
                    "認証処理が完了しました。Unityに戻ってください。";

                byte[] responseBytes = Encoding.UTF8.GetBytes(message);
                context.Response.ContentType =
                    "text/plain; charset=utf-8";
                context.Response.ContentLength64 = responseBytes.Length;

                await context.Response.OutputStream.WriteAsync(
                    responseBytes, 0, responseBytes.Length);

                context.Response.Close();

                if (returnedState != state ||
                    !string.IsNullOrEmpty(error) ||
                    string.IsNullOrEmpty(code))
                {
                    Debug.LogError("Google認証がキャンセルまたは失敗しました");
                    return;
                }


                // 認可コードをIDトークンに交換
                string secret = ClientSecret;

                if (string.IsNullOrWhiteSpace(secret))
                {
                    Debug.LogError(
                        "GOOGLE_OAUTH_CLIENT_SECRET が設定されていません");
                    return;
                }

                using (var http = new HttpClient())
                using (var content = new FormUrlEncodedContent(
                    new System.Collections.Generic.Dictionary<string, string>
                    {
        { "client_id", clientId },
        { "client_secret", secret },
        { "code", code },
        { "code_verifier", verifier },
        { "redirect_uri", RedirectUri },
        { "grant_type", "authorization_code" }
                    }))
                {
                    var response = await http.PostAsync(
                        "https://oauth2.googleapis.com/token",
                        content);

                    string json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.LogError(
                            $"Googleトークン取得失敗\n" +
                            $"HTTPステータス: {(int)response.StatusCode}\n" +
                            $"エラー詳細: {json}");
                        return;
                    }

                    TokenResponse token =
                        JsonUtility.FromJson<TokenResponse>(json);

                    if (string.IsNullOrEmpty(token.id_token))
                    {
                        Debug.LogError("IDトークンを取得できませんでした");
                        return;
                    }

                    bool success = await FirebaseManager.Instance
                        .LoginWithGoogle(token.id_token);

                    if (success)
                    {
                        Debug.Log("Googleログインテスト完了！");
                    }
                }

            }
        }

        catch (Exception e)
        {
            Debug.LogError($"Googleログインエラー: {e.Message}");
        }
        finally
        {
            isLoggingIn = false;
        }
    }

    private static string Base64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
#endif
