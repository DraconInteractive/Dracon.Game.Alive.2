using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// Doors currently only open and shut from one side, which traps the player if they shut it
// TODO: Implement backside detection. PA: Two box colliders, and reverse behaviour if approaching from back
public class Door : Interactable
{
    public float openRot;

    [ReadOnly]
    public bool open;
    
    [ReadOnly]
    public bool interactionLocked;

    public GameObject promptUI;
    
    public override void Interact()
    {
        base.Interact();

        StartCoroutine(open ? CloseRoutine() : OpenRoutine());
    }

    public override bool CanInteract()
    {
        return !interactionLocked;
    }

    public override void Focus()
    {
        promptUI.SetActive(true);
    }

    public override void Defocus()
    {
        promptUI.SetActive(false);
    }

    private IEnumerator OpenRoutine()
    {
        interactionLocked = true;
        var startRot = transform.rotation;
        var targetRot = startRot * Quaternion.AngleAxis(openRot, transform.up);
        for (float f = 0; f < 1; f += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRot, targetRot, f);
            yield return null;
        }

        open = true;
        interactionLocked = false;
    }
    
    private IEnumerator CloseRoutine()
    {
        interactionLocked = true;
        var startRot = transform.rotation;
        var targetRot = startRot * Quaternion.AngleAxis(-openRot, transform.up);
        for (float f = 0; f < 1; f += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRot, targetRot, f);
            yield return null;
        }

        open = false;
        interactionLocked = false;
    }
}


