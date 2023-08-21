using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugPause : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.P;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(pauseKey))
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
#endif
    }
}