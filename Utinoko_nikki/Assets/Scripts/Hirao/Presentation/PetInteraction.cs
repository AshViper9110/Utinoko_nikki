using AquaGarden.Systems;
using UnityEngine;

namespace AquaGarden.Presentation
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class PetInteraction : MonoBehaviour
    {
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField, Min(0f)] private float affectionPerClick = 5f;

        private CatAnimationController catAnimationController;

        private void Awake()
        {
            catAnimationController = GetComponent<CatAnimationController>();
        }

        private void OnMouseDown()
        {
            if (petRegistry == null)
            {
                Debug.LogWarning(
                    "PetInteraction: PetRegistry is not assigned.", this);
                return;
            }

            var pet = petRegistry.CurrentPet;
            if (pet == null)
                return;

            pet.affection = Mathf.Clamp(
                pet.affection + affectionPerClick,
                0f,
                100f);

            petRegistry.NotifyChanged();

            // Smile is shown only in response to petting.
            if (catAnimationController == null)
                catAnimationController = GetComponent<CatAnimationController>();

            if (catAnimationController != null)
                catAnimationController.ReactToPet();

            Debug.Log(
                $"{pet.petName} ÇÇ»Ç≈ÇΩÅI çDä¥ìx: {pet.affection}",
                this);
        }
    }
}
