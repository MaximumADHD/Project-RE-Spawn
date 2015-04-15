using UnityEngine;
using System.Collections;

public abstract class Invokable : MonoBehaviour 
{
    // Base class which serves as a base for objects that can be toggled.
    public bool isActive = false;
    abstract public void OnActiveChanged(bool newState);

    public void ToggleActive()
    {
        // if its active, then its set inactive, otherwise its set active.
        isActive = !isActive;
        OnActiveChanged(isActive);
    }

    public void SetActive(bool newState)
    {
        if (!isActive == newState)
        {
            isActive = newState;
            OnActiveChanged(newState);
        }
    }
}
