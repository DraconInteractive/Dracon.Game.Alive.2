using System.Collections;
using UnityEngine;

/*Sub-component of the main player interaction script, 
  needed for collision detection and playback drop sound*/

namespace Suntail
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class PhysicsObject : MonoBehaviour
    {
        public PhysicsObjectPreset preset;
        
        [Tooltip("Array drop sounds")]
        [HideInInspector] public bool pickedUp = false;
        [HideInInspector] public bool wasPickedUp = false;
        [HideInInspector] public PlayerInteractions playerInteraction;
        private AudioSource _objectAudioSource;

        private void Awake()
        {
            _objectAudioSource = gameObject.GetComponent<AudioSource>();
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        //Breaking connection if break force be lower magnitude
        private void OnCollisionEnter(Collision collision)
        {
            if (pickedUp)
            {
                if (collision.relativeVelocity.magnitude > preset.BreakForce)
                {
                    playerInteraction.BreakConnection();
                }

            }
            else if (wasPickedUp) //Check if the item has been picked up
            {
                PlayDropSound(); //Play sound if we drop an object and it hits the ground.
            }

        }

        //Prevent the connection from breaking when you just picked up the object as it sometimes fires a collision with the ground or whatever it is touching
        public IEnumerator PickUp()
        {
            yield return new WaitForSeconds(preset.WaitOnPickup);
            pickedUp = true;
            wasPickedUp = true;
        }

        //Playing drop sound on item collision
        private void PlayDropSound()
        {
            var clips = preset.DropClips;
            _objectAudioSource.clip = clips[Random.Range(0, clips.Length)];
            _objectAudioSource.Play();
        }
    }
}