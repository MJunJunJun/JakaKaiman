using UnityEngine;

public class CharacterActor : MonoBehaviour
{
    [Header("Character")]
    public string characterID;

    [Header("Animator")]
    public Animator animator;

    [Header("Animation States")]
    public string idleState = "Idle";
    public string talkState = "Talk";

    public void PlayIdle()
    {
        if (animator != null)
        {
            animator.Play(idleState);
        }
    }

    public void PlayTalk()
    {
        if (animator != null)
        {
            animator.Play(talkState);
        }
    }
}