using System;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    // Simple flags for other systems to read.
    // Only one action type should be active at a time in normal use.
    public bool IsDoingAITask { get; private set; }
    public bool IsDoingCreativeTask { get; private set; }

    // Events in case other systems prefer subscribing
    public event Action<bool> OnAITaskChanged;
    public event Action<bool> OnCreativeTaskChanged;

    /// <summary>
    /// Use these to set states. Other scripts should call these when tasks begin/end.
    /// </summary>
    public void SetDoingAITask(bool value)
    {
        IsDoingAITask = value;
        if (value) IsDoingCreativeTask = false; // ensure no overlap
        OnAITaskChanged?.Invoke(IsDoingAITask);
    }

    public void SetDoingCreativeTask(bool value)
    {
        IsDoingCreativeTask = value;
        if (value) IsDoingAITask = false;
        OnCreativeTaskChanged?.Invoke(IsDoingCreativeTask);
    }
}
