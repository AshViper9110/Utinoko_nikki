
using UnityEngine;
using AquaGarden.Systems;

namespace AquaGarden.Presentation
{
    public class DailyCareCompleteButton : MonoBehaviour
    {
        private bool isProcessing;

        public void OnClickComplete()
        {
            if (isProcessing)
                return;

            if (DailyCareManager.Instance == null)
            {
                Debug.LogError("DailyCareManager‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñB");
                return;
            }

            isProcessing = true;

            DailyCareManager.Instance.CompleteAndReturn();
        }
    }
}
