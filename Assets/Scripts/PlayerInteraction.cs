using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform eyes;
    public LayerMask targetLayer;
    public float range;

    public KeyCode interactionKey;
    public KeyCode testKey;
    public KeyCode zoomKey;
    [ReadOnly]
    public Interactable currentTarget;
    
    [Header("UI")] 
    public TMP_Text interactableTxt;

    private Action onGainTarget, onLostTarget;
    public Action<bool> onZoomChange;
    public Action onTestKey;
    
    private void OnEnable()
    {
        onGainTarget += OnGainTarget;
        onLostTarget += OnLostTarget;
    }

    private void OnDisable()
    {
        onGainTarget -= OnGainTarget;
        onLostTarget -= OnLostTarget;
    }

    private void Update()
    {
        Interactable tempTarget = null;
        if (Physics.Raycast(eyes.position, eyes.forward, out var h, range, targetLayer.value))
        {
            if (h.transform.TryGetComponent<Interactable>(out var interactable))
            {
                tempTarget = interactable;
            }
        }

        if (tempTarget != currentTarget)
        {
            if (currentTarget != null)
            {
                currentTarget.Defocus();
            }
            
            currentTarget = tempTarget;
            
            if (tempTarget == null)
            {
                onLostTarget?.Invoke();
            }
            else
            {
                tempTarget.Focus();
                onGainTarget?.Invoke();
            }
        }

        if (currentTarget != null && currentTarget.CanInteract() && Input.GetKeyDown(interactionKey))
        {
            currentTarget.Interact();
        }

        if (Input.GetKeyDown(zoomKey))
        {
            onZoomChange?.Invoke(true);
        }
        else if (Input.GetKeyUp(zoomKey))
        {
            onZoomChange?.Invoke(false);
        }
        else if (Input.GetKeyDown(testKey))
        {
            onTestKey?.Invoke();
        }
    }

    private void OnGainTarget()
    {
        interactableTxt.text = currentTarget.DisplayName;
    }

    private void OnLostTarget()
    {
        interactableTxt.text = "";
    }

    private void OnDrawGizmos()
    {
        if (eyes != null)
        {
            Gizmos.DrawLine(eyes.position, eyes.position + eyes.forward * range);
        }
    }
}
