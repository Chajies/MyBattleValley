using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    readonly int groundedTrigger = Animator.StringToHash("Grounded");
    readonly int idleTrigger = Animator.StringToHash("IdleSpeed");
    readonly int jumpTrigger = Animator.StringToHash("Jump");
    const int tiltAmount = 5;
    //
    PlayerManager manager;
    Transform form;
    Animator anim;

    //

    void Awake()
    {
        form = transform;
        anim = form.GetChild(0).GetComponent<Animator>();
        manager = GetComponentInParent<PlayerManager>();
    }
    //
    void Start()
    {
        manager.events.OnStopped += ResetTilt;
        manager.events.OnJumped += HandleJump;
        manager.events.OnLanded += HandleLanded;
        manager.events.OnDirectionChanged += RotateSprite;
    }
    //
    void RotateSprite(int direction)
    {
        if (direction == 1) form.rotation = Quaternion.Euler(0, 0, tiltAmount);
        else form.rotation = Quaternion.Euler(0, 180, tiltAmount);
    }
    // if player stops moving stop player tilt
    void ResetTilt() => form.rotation = Quaternion.Euler(0, form.rotation.y > 0 ? 180 : 0, 0);
    void HandleLanded() => anim.SetTrigger(groundedTrigger);
    void HandleJump()
    {
        anim.SetTrigger(jumpTrigger);
        anim.ResetTrigger(groundedTrigger);
    }
    //
    void OnDisable()
    {
        manager.events.OnDirectionChanged -= RotateSprite;
        manager.events.OnLanded -= HandleLanded;
        manager.events.OnJumped -= HandleJump;
        manager.events.OnStopped -= ResetTilt;
    }
}
