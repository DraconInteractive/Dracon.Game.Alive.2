using UnityEngine;

[CreateAssetMenu(fileName = "Physics Object Preset", menuName = "Interactables/Physics Obj Preset")]
public class PhysicsObjectPreset : ScriptableObject
{
    public float WaitOnPickup;
    public float BreakForce;
    public AudioClip[] DropClips;
}
