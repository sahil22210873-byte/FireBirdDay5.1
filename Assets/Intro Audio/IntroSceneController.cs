using System.Collections;
using UnityEngine;

public class IntroSceneController : MonoBehaviour
{
    [Header("References")]
    public AudioSource introAudio;
    public GameObject canvas;
    public Animator gateAnimator;
    public Transform player;

    [Header("Trigger")]
    public Collider doorTriggerCollider;

    bool gateOpened = false;
    bool gateClosed = false;

    void Start()
    {
        if (canvas != null) canvas.SetActive(false);

        if (introAudio != null)
        {
            introAudio.Play();
            StartCoroutine(ShowCanvasAfterAudio());
        }
        else
        {
            if (canvas != null) canvas.SetActive(true);
        }
    }

    IEnumerator ShowCanvasAfterAudio()
    {
        yield return new WaitWhile(() => introAudio != null && introAudio.isPlaying);
        yield return new WaitForSeconds(0.25f);

        if (canvas != null) canvas.SetActive(true);
    }

    public void OnLetsGoClicked()
    {
        if (gateAnimator != null)
        {
            gateAnimator.SetTrigger("Open");
            gateOpened = true;
        }

        if (canvas != null) canvas.SetActive(false);
    }

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
