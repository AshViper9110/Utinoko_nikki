
using UnityEngine;

public class FirebaseRegisterTest : MonoBehaviour
{
    [SerializeField] private string email;
    [SerializeField] private string password;

    //登録
    public async void Register()
    {
        bool success = await FirebaseManager.Instance
            .RegisterUser(email, password);

        if (success)
        {
            Debug.Log("登録テスト完了！");
        }
    }

    //ログイン
    public async void Login()
    {
        bool success = await FirebaseManager.Instance
            .LoginUser(email, password);

        if (success)
        {
            Debug.Log("ログインテスト完了！");
        }
    }

    // ログアウト
    public void Logout()
    {
        FirebaseManager.Instance.LogoutUser();

        if (!FirebaseManager.Instance.IsLoggedIn())
        {
            Debug.Log("ログアウトテスト完了！");
        }
        else
        {
            Debug.LogError("ログアウトに失敗しました");
        }

    }

    // ログイン状態確認
    public void CheckLoginStatus()
    {
        if (FirebaseManager.Instance == null ||
            FirebaseManager.Instance.Auth == null)
        {
            Debug.LogWarning("Firebaseが初期化されていません");
            return;
        }

        if (FirebaseManager.Instance.IsLoggedIn())
        {
            Debug.Log("ログイン状態：ログイン済み");

            // ログイン中のユーザーのUIDを取得
            string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;
            Debug.Log($"UID: {uid}");
        }
        else
        {
            Debug.Log("ログイン状態：未ログイン");
        }
    }

}
