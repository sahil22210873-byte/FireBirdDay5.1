using UnityEngine;

public class VoiceoverTrigger : MonoBehaviour
{
    public NPCMovementController npcController;
    public int triggerNumber = 2;
    public AudioClip voiceoverClip;
    public AudioSource audioSource;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
        {
            return;
        }

        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            Debug.Log($"Player entered voiceover trigger {triggerNumber}");
            hasTriggered = true;

            if (audioSource != null && voiceoverClip != null)
            {
                audioSource.clip = voiceoverClip;
                audioSource.Play();
                Debug.Log($"Playing voiceover clip for trigger {triggerNumber}");
            }

            if (npcController != null)
            {
                switch (triggerNumber)
                {
                    case 2:
                        npcController.OnPlayerEnteredSecondTrigger();
                        break;
                    case 3:
                        npcController.OnPlayerEnteredThirdTrigger();
                        break;
                }
            }
        }
    }
}
