using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Actions : NetworkBehaviour
{
    const int jumpApexThreshold = 5;
    const float jumpBuffer = 0.1f;
    const int maxFallSpeed = 120;
    const int minFallSpeed = 80;
    const int jumpHeight = 13;
    //
    float previousYPosition;
    float verticalSpeed;
    float yVelocity;
    float apexPoint;
    float fallSpeed;
    bool jumped;
    //
    PlayerManager manager;
    Transform form;
    Events events;



    void Awake()
    {
        manager = GetComponent<PlayerManager>();
        form = transform;
    }
    //
    void Start() => manager.events.OnJumped += HandleJump;
    public void CalculateVerticalMovement()
    {
        // if the player is on the floor the jump apex is always 0
        if (!manager.inAir) apexPoint = 0;
        else
        {
            yVelocity = (form.position.y - previousYPosition) / Time.deltaTime;
            previousYPosition = form.position.y;
            //
            // the jump apex becomes 1 at the apex
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(yVelocity));
            fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);
        }
        //
        CalculateGravity();
    }
    //
    void CalculateGravity()
    {
        // if we are through the ground, move player back to be in line with the ground
        if (!manager.inAir || manager.isDead) verticalSpeed = verticalSpeed < 0 ? 0 : verticalSpeed;
        else verticalSpeed -= fallSpeed * Time.deltaTime;
        if (jumped && !manager.inAir) Jump();
    }
    //
    void Jump()
    {
        verticalSpeed = jumpHeight;
        manager.HandleJump();
        jumped = false;
    }
    //
    void HandleJump() => jumped = true;
    public float ApexPoint() => apexPoint;
    public float VerticalSpeed() => verticalSpeed;
    void OnDisable() => manager.events.OnJumped -= HandleJump;
}
