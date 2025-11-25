using System.Collections;
using UnityEngine;

public class IntroSceneController : MonoBehaviour
{
    [Header("References")]
    public AudioSource introAudio;       // IntroAudio object
    public GameObject letsGoButton;      // UI Button gameObject (set inactive in inspector)
    public Animator gateAnimator;        // Animator controlling the gate (or parent)
    public Transform player;             // Player or XR Rig transform

    [Header("Trigger")]
    public Collider doorTriggerCollider; // assign DoorCloseTrigger's collider (optional)

    bool gateOpened = false;
    bool gateClosed = false;

    void Start()
    {
        // ensure button hidden
        if (letsGoButton != null) letsGoButton.SetActive(false);

        // Play intro audio (if not already playing)
        if (introAudio != null)
        {
            introAudio.Play();
            StartCoroutine(ShowButtonAfterAudio());
        }
        else
        {
            // fallback: show immediately
            if (letsGoButton != null) letsGoButton.SetActive(true);
        }
    }

    IEnumerator ShowButtonAfterAudio()
    {
        // Wait until audio finished (handles clip length accurately)
        yield return new WaitWhile(() => introAudio != null && introAudio.isPlaying);

        // slight delay to make it feel polished
        yield return new WaitForSeconds(0.25f);

        if (letsGoButton != null) letsGoButton.SetActive(true);
    }

    // Called by the button's OnClick event
    public void OnLetsGoClicked()
    {
        if (gateAnimator != null)
        {
            gateAnimator.SetTrigger("Open");
            gateOpened = true;
        }

        // Hide button after clicked
        if (letsGoButton != null) letsGoButton.SetActive(false);
    }

    // This function will be called by DoorCloseTrigger script when player enters
    public void PlayerEnteredDoorway()
    {
        if (gateOpened && !gateClosed)
        {
            if (gateAnimator != null)
            {
                gateAnimator.SetTrigger("Close");
                gateClosed = true;
            }
        }
    }
}
