using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerManager : NetworkBehaviour
{
    [HideInInspector] public Events events;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool inAir;
    Collisions collisions;
    WeaponManager weapon;
    PlayerInput input;
    Movement movement;
    Actions actions;

    //

    void Awake() => Initialize();
    void Update()
    {
        if (!IsOwner) return;
        //
        input.Tick(isDead);
        collisions.CheckForGround();
        actions.CalculateVerticalMovement();
        movement.CalculateHorizontalMovement();
    }
    //
    void Initialize()
    {
        events = new Events();
        input = new PlayerInput(events);
        actions = GetComponent<Actions>();
        movement = GetComponent<Movement>();
        weapon = GetComponent<WeaponManager>();
        collisions = GetComponent<Collisions>();
        //
        movement.Injectmanager(this);
        weapon.InjectEvents(events);
    }
    //
    public float VerticalSpeed() => actions.VerticalSpeed();
    public float GetApexPoint() => actions.apexPoint;
    public void HandleJump()
    {
        collisions.HandleJump();
        inAir = true;
    }
}
