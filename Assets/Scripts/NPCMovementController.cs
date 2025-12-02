using System.Collections;
using UnityEngine;

public class NPCMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 1.5f;
    public float arrivalDistance = 0.1f;
    public float turnAngleThreshold = 30f;

    [Header("Player Tracking")]
    public Transform playerTransform;
    public float idleRotationSpeed = 2f;
    public bool facePlayerWhenIdle = true;

    [Header("Target Positions")]
    public Transform firstPosition;
    public Transform secondPosition;

    [Header("Animation Triggers")]
    public string waveGestureTrigger = "Wave";
    public string idleTrigger = "Idle";
    public string startWalkingTrigger = "StartWalking";
    public string stopWalkingTrigger = "StopWalking";
    public string leftTurnTrigger = "LeftTurn";
    public string rightTurnTrigger = "RightTurn";
    public string turn180Trigger = "Turn180";

    [Header("Animation Parameters")]
    public string isWalkingBool = "IsWalking";

    [Header("Voiceover Settings")]
    public AudioSource voiceoverAudioSource;
    public float voiceoverDuration1 = 5f;
    public float voiceoverDuration2 = 5f;
    public float voiceoverDuration3 = 5f;

    private Animator animator;
    private Transform currentTarget;
    private bool isMoving = false;
    private bool isTurning = false;
    private bool isWalking = false;
    private bool isGesturing = false;
    private bool isInIdleState = false;
    private int currentStage = 0;
    private float initialYPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialYPosition = transform.position.y;

        if (animator == null)
        {
            Debug.LogError("Animator component not found on NPC!");
        }

        if (playerTransform == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
            if (xrOrigin != null)
            {
                Transform cameraOffset = xrOrigin.transform.Find("Camera Offset");
                if (cameraOffset != null)
                {
                    playerTransform = cameraOffset.Find("Main Camera");
                }
            }

            if (playerTransform == null)
            {
                playerTransform = Camera.main?.transform;
            }

            if (playerTransform != null)
            {
                Debug.Log($"Auto-found player: {playerTransform.name}");
            }
            else
            {
                Debug.LogWarning("Player transform not found! Assign manually or ensure Camera is tagged as MainCamera");
            }
        }

        isInIdleState = true;
    }

    void Update()
    {
        if (isMoving && currentTarget != null && !isTurning)
        {
            MoveTowardsTarget();
        }

        if (facePlayerWhenIdle && isInIdleState && !isMoving && !isTurning && !isGesturing)
        {
            FacePlayer();
        }

        LockYPosition();
    }

    public void OnPlayerEnteredFirstTrigger()
    {
        if (currentStage != 0) return;

        Debug.Log("Stage 1: Player entered first trigger");
        currentStage = 1;
        StartCoroutine(ExecuteFirstSequence());
    }

    public void OnPlayerEnteredSecondTrigger()
    {
        if (currentStage != 1) return;

        Debug.Log("Stage 2: Player entered second trigger");
        currentStage = 2;
        StartCoroutine(ExecuteSecondSequence());
    }

    public void OnPlayerEnteredThirdTrigger()
    {
        if (currentStage != 2) return;

        Debug.Log("Stage 3: Player entered third trigger");
        currentStage = 3;
        StartCoroutine(ExecuteThirdSequence());
    }

    IEnumerator ExecuteFirstSequence()
    {
        PlayWaveGestureAnimation();

        yield return new WaitForSeconds(GetWaveAnimationLength());

        PlayIdleAnimation();

        float waitTime = voiceoverAudioSource != null && voiceoverAudioSource.isPlaying 
            ? voiceoverAudioSource.clip.length 
            : voiceoverDuration1;

        Debug.Log($"Waiting {waitTime} seconds for voiceover to finish");
        yield return new WaitForSeconds(waitTime);

        if (firstPosition != null)
        {
            Debug.Log("Moving to first position");
            yield return StartCoroutine(WalkToPosition(firstPosition));
        }
    }

    IEnumerator ExecuteSecondSequence()
    {
        PlayWaveGestureAnimation();

        yield return new WaitForSeconds(GetWaveAnimationLength());

        PlayIdleAnimation();

        float waitTime = voiceoverAudioSource != null && voiceoverAudioSource.isPlaying 
            ? voiceoverAudioSource.clip.length 
            : voiceoverDuration2;

        Debug.Log($"Waiting {waitTime} seconds for voiceover to finish");
        yield return new WaitForSeconds(waitTime);

        if (secondPosition != null)
        {
            Debug.Log("Moving to second position");
            yield return StartCoroutine(WalkToPosition(secondPosition));
        }
    }

    IEnumerator ExecuteThirdSequence()
    {
        PlayWaveGestureAnimation();

        yield return new WaitForSeconds(GetWaveAnimationLength());

        float waitTime = voiceoverAudioSource != null && voiceoverAudioSource.isPlaying 
            ? voiceoverAudioSource.clip.length 
            : voiceoverDuration3;

        Debug.Log($"Waiting {waitTime} seconds for voiceover to finish");
        yield return new WaitForSeconds(waitTime);

        PlayIdleAnimation();
        Debug.Log("NPC sequence complete, returning to idle");
    }

    void PlayWaveGestureAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Playing wave gesture animation");
            isGesturing = true;
            isInIdleState = false;
            animator.SetTrigger(waveGestureTrigger);
        }
    }

    void PlayIdleAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Playing idle animation");
            isWalking = false;
            isGesturing = false;
            isInIdleState = true;
            animator.SetBool(isWalkingBool, false);
            animator.SetTrigger(idleTrigger);
        }
    }

    void PlayStartWalkingAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Playing start walking animation");
            isWalking = true;
            isInIdleState = false;
            animator.SetBool(isWalkingBool, true);
            animator.SetTrigger(startWalkingTrigger);
        }
    }

    void PlayStopWalkingAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Playing stop walking animation");
            animator.SetTrigger(stopWalkingTrigger);
        }
    }

    IEnumerator WalkToPosition(Transform target)
    {
        currentTarget = target;

        yield return StartCoroutine(PerformTurnTowardsTarget());

        yield return new WaitForSeconds(0.2f);

        PlayStartWalkingAnimation();

        yield return new WaitForSeconds(0.5f);

        isMoving = true;

        while (isMoving && currentTarget != null)
        {
            yield return null;
        }

        PlayStopWalkingAnimation();

        yield return new WaitForSeconds(0.5f);

        PlayIdleAnimation();
    }

    IEnumerator PerformTurnTowardsTarget()
    {
        if (currentTarget == null) yield break;

        Vector3 directionToTarget = currentTarget.position - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget == Vector3.zero) yield break;

        float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);

        Debug.Log($"Angle to target: {angleToTarget} degrees");

        if (Mathf.Abs(angleToTarget) < turnAngleThreshold)
        {
            Debug.Log("Target within threshold, no turn needed");
            yield break;
        }

        isTurning = true;

        if (Mathf.Abs(angleToTarget) > 150f)
        {
            Debug.Log("Performing 180 turn");
            animator.SetTrigger(turn180Trigger);
            yield return new WaitForSeconds(GetTurnAnimationLength(turn180Trigger));
        }
        else if (angleToTarget > turnAngleThreshold)
        {
            Debug.Log("Performing left turn");
            animator.SetTrigger(leftTurnTrigger);
            yield return new WaitForSeconds(GetTurnAnimationLength(leftTurnTrigger));
        }
        else if (angleToTarget < -turnAngleThreshold)
        {
            Debug.Log("Performing right turn");
            animator.SetTrigger(rightTurnTrigger);
            yield return new WaitForSeconds(GetTurnAnimationLength(rightTurnTrigger));
        }

        isTurning = false;
    }

    void MoveTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 targetPosition = currentTarget.position;
        Vector3 currentPosition = transform.position;

        targetPosition.y = initialYPosition;
        currentPosition.y = initialYPosition;

        Vector3 direction = (targetPosition - currentPosition).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);
        }

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, movementSpeed * Time.deltaTime);
        newPosition.y = initialYPosition;
        transform.position = newPosition;

        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
                                         new Vector3(targetPosition.x, 0, targetPosition.z));

        if (distance <= arrivalDistance)
        {
            Debug.Log("Arrived at target position");
            isMoving = false;
            currentTarget = null;
        }
    }

    void FacePlayer()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, idleRotationSpeed * Time.deltaTime);
    }

    void LockYPosition()
    {
        Vector3 pos = transform.position;
        if (Mathf.Abs(pos.y - initialYPosition) > 0.01f)
        {
            pos.y = initialYPosition;
            transform.position = pos;
        }
    }

    float GetWaveAnimationLength()
    {
        return 2f;
    }

    float GetTurnAnimationLength(string triggerName)
    {
        if (triggerName == turn180Trigger)
            return 2f;
        else if (triggerName == leftTurnTrigger || triggerName == rightTurnTrigger)
            return 1.5f;
        
        return 1.5f;
    }
}
