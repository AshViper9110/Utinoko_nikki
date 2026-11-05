using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace VisualProgramming
{
    /// <summary>
    /// Add this component to an empty GameObject to create the editor UI at runtime.
    /// Supports Unity's new Input System.
    /// </summary>
    public class VisualProgrammingBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            EnsureEventSystem();

            GameObject editor = new GameObject("VisualProgrammingEditor");
            editor.AddComponent<VisualProgramRunner>();
            editor.AddComponent<VisualProgramEditor>();
        }

        private void EnsureEventSystem()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject(
                    "EventSystem",
                    typeof(EventSystem)
                );
                eventSystem = eventSystemObject.GetComponent<EventSystem>();
            }

            // Disable and remove the legacy module if the scene already has one.
            StandaloneInputModule legacyModule =
                eventSystem.GetComponent<StandaloneInputModule>();

            if (legacyModule != null)
            {
                legacyModule.enabled = false;
                Destroy(legacyModule);
            }

            // uGUI must receive pointer/navigation events through the Input System UI module.
            InputSystemUIInputModule inputSystemModule =
                eventSystem.GetComponent<InputSystemUIInputModule>();

            if (inputSystemModule == null)
            {
                inputSystemModule =
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            // Assign the package's default Point/Click/Move/Submit/Cancel actions.
            inputSystemModule.AssignDefaultActions();
        }
    }
}
