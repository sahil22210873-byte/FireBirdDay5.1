using UnityEngine;
using System.Collections;

public class IntroSoundDelay : MonoBehaviour
{
    public AudioSource introVoice;
    public float delayTime = 2f; // 2 seconds

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delayTime);
        introVoice.Play();
    }
}
