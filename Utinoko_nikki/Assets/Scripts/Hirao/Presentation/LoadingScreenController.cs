using System.Collections;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private CanvasGroup panelGroup;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    private bool initializationCompleted;
    private bool isFading;

    private void Awake()
    {
        // 起動時は必ず黒いパネルを表示
        loadingPanel.SetActive(true);
        panelGroup.alpha = 1f;
    }

    /// <summary>
    /// DB読み込み・ペット情報・UI配置など、
    /// 起動時の初期化がすべて完了したときに呼び出す
    /// </summary>
    public void NotifyInitializationCompleted()
    {
        if (initializationCompleted)
            return;

        initializationCompleted = true;

        TryHidePanel();
    }

    private void TryHidePanel()
    {
        if (!initializationCompleted || isFading)
            return;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            panelGroup.alpha =
                1f - Mathf.Clamp01(elapsed / fadeDuration);

            yield return null;
        }

        panelGroup.alpha = 0f;
        loadingPanel.SetActive(false);

        isFading = false;
    }
}