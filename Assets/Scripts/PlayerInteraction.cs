using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Dracon.PlayerInput;

public class PlayerInteraction : SentientModule
{
    public Transform eyes;
    public LayerMask targetLayer;
    public float range;
    
    [ReadOnly]
    public Interactable currentTarget;
    
    [Header("UI")] 
    public TMP_Text interactableTxt;
    
    private Action onGainTarget, onLostTarget;

    public override void SentientAwake()
    {
        base.SentientAwake();
        onGainTarget += OnGainTarget;
        onLostTarget += OnLostTarget;
        Player.Instance.Input.keyEvents[PlayerInput.KeyMap.Interaction].onKeyDown += TryInteract;
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
    }

    private void TryInteract()
    {
        if (currentTarget != null && currentTarget.CanInteract())
        {
            currentTarget.Interact();
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
