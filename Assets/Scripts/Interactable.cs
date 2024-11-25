using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string DisplayName;
    private void Reset()
    {
        DisplayName = gameObject.name;
    }

    [Button]
    public virtual void Interact()
    {
        
    }

    public virtual bool CanInteract()
    {
        return true;
    }

    public virtual void Focus()
    {
        
    }

    public virtual void Defocus()
    {
        
    }
}
