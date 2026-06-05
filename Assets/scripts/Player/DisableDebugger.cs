using UnityEngine;
using UnityEngine.Rendering;

public class DisableDebugger : MonoBehaviour
{
    void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }
}