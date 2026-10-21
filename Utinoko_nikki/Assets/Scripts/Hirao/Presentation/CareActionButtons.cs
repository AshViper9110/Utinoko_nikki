
using UnityEngine;
using AquaGarden.Systems;

namespace AquaGarden.Presentation
{
    public class PetCareButtonActions : MonoBehaviour
    {
        [SerializeField] private PetCareSystem petCareSystem;
        [SerializeField] private CatAnimationController petController;

        private void Awake()
        {
            if (petCareSystem == null)
                petCareSystem = FindFirstObjectByType<PetCareSystem>();

            if (petController == null)
                petController = FindFirstObjectByType<CatAnimationController>();
        }

        // ごはんボタン
        public void Feed()
        {
            if (petCareSystem == null)
            {
                Debug.LogError("PetCareSystem が見つかりません。", this);
                return;
            }

            if (!petCareSystem.Feed())
            {
                Debug.LogWarning("ごはんをあげられませんでした。PetCareSystem の状態を確認してください。", this);
                return;
            }

            Debug.Log("ごはんをあげました！", this);
        }

        // なでるボタン
        public void Pet()
        {
            if (petCareSystem == null)
            {
                Debug.LogError("PetCareSystem が見つかりません。", this);
                return;
            }

            if (!petCareSystem.Pet())
            {
                Debug.LogWarning("なでることができませんでした。PetCareSystem の状態を確認してください。", this);
                return;
            }

            if (petController != null)
                petController.ReactToPet();

            Debug.Log("なでました！", this);
        }

        // 一緒に遊ぶボタン
        public void Play()
        {
            if (petCareSystem == null)
            {
                Debug.LogError("PetCareSystem が見つかりません。", this);
                return;
            }

            if (!petCareSystem.Play())
            {
                Debug.LogWarning("一緒に遊べませんでした。PetCareSystem の状態を確認してください。", this);
                return;
            }

            if (petController != null)
                petController.PlayAction(CatAnimationController.PetAction.Crouch);

            Debug.Log("一緒に遊びました！", this);
        }
    }
}
