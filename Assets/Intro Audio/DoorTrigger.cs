using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public IntroSceneController introController;
    public Animator characterAnimator;
    public NPCMovementController npcController;
    public AudioClip voiceoverClip;
    public AudioSource audioSource;
    public float characterAnimDelay = 2f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}, Tag: {other.tag}");

        if (hasTriggered)
        {
            Debug.Log("Already triggered, ignoring");
            return;
        }

        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            Debug.Log("Player detected! Closing gate and starting character animation sequence");
            hasTriggered = true;

            if (introController != null)
            {
                introController.PlayerEnteredDoorway();
            }

            if (audioSource != null && voiceoverClip != null)
            {
                audioSource.clip = voiceoverClip;
                audioSource.Play();
                Debug.Log("Playing voiceover clip for first trigger");
            }

            StartCoroutine(StartCharacterAnimationAfterDelay());
        }
    }

    IEnumerator StartCharacterAnimationAfterDelay()
    {
        Debug.Log($"Waiting {characterAnimDelay} seconds before starting character animation");
        yield return new WaitForSeconds(characterAnimDelay);

        if (npcController != null)
        {
            Debug.Log("Starting NPC animation sequence NOW");
            npcController.OnPlayerEnteredFirstTrigger();
        }
        else if (characterAnimator != null)
        {
            Debug.Log("Starting character animation NOW (legacy mode)");
            characterAnimator.SetTrigger("StartAnimation");
        }
    }
}
