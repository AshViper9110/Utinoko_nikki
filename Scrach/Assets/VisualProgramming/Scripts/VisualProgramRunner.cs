using System.Collections;
using UnityEngine;

namespace VisualProgramming
{
    public class VisualProgramRunner : MonoBehaviour
    {
        private Coroutine runningRoutine;

        public bool IsRunning => runningRoutine != null;

        public void Run(VisualProgramData program, Transform target)
        {
            Stop();
            if (program == null || target == null)
            {
                Debug.LogWarning("[VisualProgramming] Program or target is missing.");
                return;
            }

            runningRoutine = StartCoroutine(Execute(program, target));
        }

        public void Stop()
        {
            if (runningRoutine == null) return;
            StopCoroutine(runningRoutine);
            runningRoutine = null;
        }

        private IEnumerator Execute(VisualProgramData program, Transform target)
        {
            for (int i = 0; i < program.blocks.Count; i++)
            {
                if (target == null) break;

                VisualBlockData block = program.blocks[i];
                if (block == null) continue;

                switch (block.type)
                {
                    case VisualBlockType.Move:
                        yield return MoveBy(target, new Vector3(block.x, block.y, block.z),
                            Mathf.Max(0.01f, block.duration));
                        break;

                    case VisualBlockType.Rotate:
                        yield return RotateBy(target, new Vector3(block.x, block.y, block.z),
                            Mathf.Max(0.01f, block.duration));
                        break;

                    case VisualBlockType.SetPosition:
                        target.position = new Vector3(block.x, block.y, block.z);
                        break;

                    case VisualBlockType.SetVisible:
                    {
                        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
                        foreach (Renderer renderer in renderers)
                            renderer.enabled = block.visible;
                        break;
                    }

                    case VisualBlockType.SetScale:
                        target.localScale = new Vector3(block.x, block.y, block.z);
                        break;

                    case VisualBlockType.Wait:
                        yield return new WaitForSeconds(Mathf.Max(0f, block.duration));
                        break;

                    case VisualBlockType.DebugLog:
                        Debug.Log("[VisualProgramming] " + block.message);
                        break;
                }
            }

            runningRoutine = null;
        }

        private IEnumerator MoveBy(Transform target, Vector3 offset, float duration)
        {
            Vector3 start = target.position;
            Vector3 end = start + offset;
            float elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.deltaTime;
                target.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            if (target != null) target.position = end;
        }

        private IEnumerator RotateBy(Transform target, Vector3 eulerOffset, float duration)
        {
            Quaternion start = target.rotation;
            Quaternion end = start * Quaternion.Euler(eulerOffset);
            float elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.deltaTime;
                target.rotation = Quaternion.Slerp(start, end, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            if (target != null) target.rotation = end;
        }

        private void OnDisable()
        {
            Stop();
        }
    }
}
