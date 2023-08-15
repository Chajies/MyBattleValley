using UnityEngine;

public class Movement : MonoBehaviour
{
    const int deceleration = 60;
    const int acceleration = 50;
    const int maxSpeed = 6;
    //
    PlayerManager manager;
    Vector3 positionToMoveTo;
    Transform form;
    //
    float horizontalSpeed;
    int xDirection;

    //

    void Awake() => form = transform;
    void Start() => manager.events.OnMoved += HandleDirection;
    public void Injectmanager(PlayerManager c) => manager = c;
    public void CalculateHorizontalMovement()
    {
        if (xDirection != 0)
        {
            horizontalSpeed += xDirection * acceleration * Time.deltaTime;
            horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeed, maxSpeed);
            //
            float apexBonus = Mathf.Sign(xDirection) * manager.GetApexPoint();
            horizontalSpeed += apexBonus * Time.deltaTime;
        }
        // if stopped moving slow the player down to 0
        else horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, deceleration * Time.deltaTime);
        //
        MoveCharacter();
    }
    //
    void MoveCharacter()
    {
        // take the players vertical speed into consideration
        positionToMoveTo.x = horizontalSpeed;
        positionToMoveTo.y = manager.VerticalSpeed();
        form.position += positionToMoveTo * Time.deltaTime;
    }
    //
    void HandleDirection(int direction) => xDirection = direction;
    void OnDisable() => manager.events.OnMoved -= HandleDirection;
}
