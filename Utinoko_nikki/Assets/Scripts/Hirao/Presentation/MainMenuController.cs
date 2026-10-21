
using UnityEngine;
using UnityEngine.SceneManagement;
using AquaGarden.Systems;

namespace AquaGarden.Presentation
{
    /// <summary>
    /// Aqua Garden のメニュー画面を管理する。
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("メニュー")]
        [SerializeField] private GameObject menuPanel;

        [Header("各画面")]
        [SerializeField] private GameObject petInfoPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject encyclopediaPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("タイトルへ戻る確認画面（任意）")]
        [SerializeField] private GameObject titleConfirmPanel;

        [Header("システム")]
        [SerializeField] private SaveManager saveManager;

        [Header("タイトル画面")]
        [SerializeField] private string titleSceneName = "Title";

        private void Awake()
        {
            if (saveManager == null)
                saveManager = FindFirstObjectByType<SaveManager>();

            CloseAllSubPanels();

            if (titleConfirmPanel != null)
                titleConfirmPanel.SetActive(false);
        }

        // メニューを開く
        public void OpenMenu()
        {
            if (menuPanel != null)
                menuPanel.SetActive(true);
        }

        // メニューを閉じる
        public void CloseMenu()
        {
            if (menuPanel != null)
                menuPanel.SetActive(false);
        }

        // メニューの開閉を切り替える
        public void ToggleMenu()
        {
            if (menuPanel != null)
                menuPanel.SetActive(!menuPanel.activeSelf);
        }

        // ペット情報
        public void OpenPetInfo()
        {
            OpenSubPanel(petInfoPanel);
        }

        // もちもの
        public void OpenInventory()
        {
            OpenSubPanel(inventoryPanel);
        }

        // 図鑑
        public void OpenEncyclopedia()
        {
            OpenSubPanel(encyclopediaPanel);
        }

        // 設定
        public void OpenSettings()
        {
            OpenSubPanel(settingsPanel);
        }

        // サブ画面を開く
        private void OpenSubPanel(GameObject target)
        {
            if (target == null)
            {
                Debug.LogWarning(
                    "表示先の画面が Inspector に設定されていません。",
                    this);
                return;
            }

            CloseAllSubPanels();
            target.SetActive(true);
            CloseMenu();
        }

        // サブ画面をすべて閉じる
        public void CloseAllSubPanels()
        {
            SetActiveIfAssigned(petInfoPanel, false);
            SetActiveIfAssigned(inventoryPanel, false);
            SetActiveIfAssigned(encyclopediaPanel, false);
            SetActiveIfAssigned(settingsPanel, false);
        }

        // サブ画面からメニューに戻る
        public void ReturnToMenu()
        {
            CloseAllSubPanels();

            if (titleConfirmPanel != null)
                titleConfirmPanel.SetActive(false);

            OpenMenu();
        }

        // セーブ
        public void SaveGame()
        {
            if (saveManager == null)
            {
                Debug.LogError(
                    "SaveManager が見つかりません。",
                    this);
                return;
            }

            if (saveManager.SaveNow())
                Debug.Log("セーブしました。", this);
            else
                Debug.LogError(
                    "セーブに失敗しました: " + saveManager.LastError,
                    this);
        }

        // タイトルへ戻る確認画面を開く
        public void ShowTitleConfirmation()
        {
            if (titleConfirmPanel == null)
            {
                Debug.LogWarning(
                    "確認画面が未設定のため、タイトルへ直接戻ります。",
                    this);

                ReturnToTitle();
                return;
            }

            titleConfirmPanel.SetActive(true);
        }

        // タイトルへ戻るのをキャンセル
        public void CancelReturnToTitle()
        {
            if (titleConfirmPanel != null)
                titleConfirmPanel.SetActive(false);
        }

        // タイトルへ戻る
        public void ReturnToTitle()
        {
            if (string.IsNullOrWhiteSpace(titleSceneName))
            {
                Debug.LogError(
                    "タイトルシーン名が設定されていません。",
                    this);
                return;
            }

            // シーン切り替え前に保存
            if (saveManager != null && !saveManager.SaveNow())
            {
                Debug.LogError(
                    "セーブに失敗したため、タイトルへの移動を中止しました: "
                    + saveManager.LastError,
                    this);
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(titleSceneName))
            {
                Debug.LogError(
                    "タイトルシーンが見つからないか、Build Settings に登録されていません: "
                    + titleSceneName,
                    this);
                return;
            }

            SceneManager.LoadScene(titleSceneName);
        }

        private void SetActiveIfAssigned(
            GameObject target,
            bool active)
        {
            if (target != null)
                target.SetActive(active);
        }
    }
}
