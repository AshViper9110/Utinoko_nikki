using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }

    private async void Start()
    {
        await InitializeFirebase();
    }
    //FierBase初期化
    private async Task InitializeFirebase()
    {
        try
        {
            // Firebaseに必要な依存関係を確認・修復
            DependencyStatus status =
                await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status == DependencyStatus.Available)
            {
                // Firebase Authenticationを初期化
                Auth = FirebaseAuth.DefaultInstance;

                Instance = this;

                Debug.Log("Firebaseの初期化に成功しました！");
            }
            else
            {
                Debug.LogError(
                    $"Firebaseの依存関係エラー: {status}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase初期化エラー: {e}");
        }
    }
    //新規登録(メールパスワード)
    public async Task<bool> RegisterUser(string email, string password)
    {
        if (Auth == null)
        {
            Debug.LogError("Firebaseが初期化されていません");
            return false;
        }

        try
        {
            // Firebase Authenticationにユーザーを登録
            FirebaseUser user =
                (await Auth.CreateUserWithEmailAndPasswordAsync(
                    email, password)).User;

            Debug.Log($"新規登録成功 UID: {user.UserId}");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"新規登録失敗: {e.Message}");
            return false;
        }
    }

    //ログイン機能
    public async Task<bool> LoginUser(string email, string password)
    {
        if (Auth == null)
        {
            Debug.LogError("Firebaseが初期化されていません");
            return false;
        }

        try
        {
            // メールアドレスとパスワードでログイン
            FirebaseUser user =
                (await Auth.SignInWithEmailAndPasswordAsync(
                    email, password)).User;

            Debug.Log($"ログイン成功 UID: {user.UserId}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ログイン失敗: {e.Message}");
            return false;
        }
    }

    //ログアウト
    public void LogoutUser()
    {
        if (Auth == null)
        {
            Debug.LogError("Firebaseが初期化されていません");
            return;
        }

        // Firebaseからログアウト
        Auth.SignOut();

        Debug.Log("ログアウトしました");
    }


    //ログイン状態取得
    public bool IsLoggedIn()
    {
        return Auth != null && Auth.CurrentUser != null;
    }

    //googleログイン
    public async Task<bool> LoginWithGoogle(string googleIdToken)
    {
        if (Auth == null)
        {
            Debug.LogError("Firebaseが初期化されていません");
            return false;
        }

        if (string.IsNullOrEmpty(googleIdToken))
        {
            Debug.LogError("Google IDトークンがありません");
            return false;
        }

        try
        {
            // GoogleのIDトークンから認証情報を生成
            Firebase.Auth.Credential credential =
                Firebase.Auth.GoogleAuthProvider.GetCredential(
                    googleIdToken, null);

            // Firebase Authenticationにログイン
            Firebase.Auth.FirebaseUser user =
                await Auth.SignInWithCredentialAsync(credential);

            Debug.Log($"Googleログイン成功 UID: {user.UserId}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Googleログイン失敗: {e.Message}");
            return false;
        }
    }

}
