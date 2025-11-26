using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public IntroSceneController introController;
    public Animator characterAnimator;
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
                StartCoroutine(StartCharacterAnimationAfterDelay());
            }
        }
    }

    IEnumerator StartCharacterAnimationAfterDelay()
    {
        Debug.Log($"Waiting {characterAnimDelay} seconds before starting character animation");
        yield return new WaitForSeconds(characterAnimDelay);

        if (characterAnimator != null)
        {
            Debug.Log("Starting character animation NOW");
            characterAnimator.SetTrigger("StartAnimation");
        }
    }
}
