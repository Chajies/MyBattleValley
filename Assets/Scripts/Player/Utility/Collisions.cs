using UnityEngine;

// custom ground collision checking
public class Collisions : MonoBehaviour
{
    // used three raycasts to balance performance and accuracy
    [SerializeField] Transform groundCheckerMiddle;
    [SerializeField] Transform groundCheckerRight;
    [SerializeField] Transform groundCheckerLeft;
    [SerializeField] LayerMask groundLayer;
    RaycastHit2D hitMiddle;
    RaycastHit2D hitRight;
    RaycastHit2D hitLeft;
    //
    const float minDistance = 0.3f;
    const float rayBuffer = 0.15f;
    const int maxHealth = 100;
    PlayerManager manager;
    bool landedMiddle;
    bool landedRight;
    bool landedLeft;
    float airTimer;
    bool jumped;

    //

    void Awake() => manager = GetComponent<PlayerManager>();
    public void CheckForGround()
    {
        if (manager.inAir && jumped)
        {
            airTimer += Time.deltaTime;
            if (airTimer > rayBuffer) RaycastGround();
        }
        else RaycastGround();
    }
    //
    void RaycastGround()
    {
        hitMiddle = Physics2D.Raycast(groundCheckerMiddle.position, Vector2.down, rayBuffer, groundLayer);
        hitRight = Physics2D.Raycast(groundCheckerRight.position, Vector2.down, rayBuffer, groundLayer);
        hitLeft = Physics2D.Raycast(groundCheckerLeft.position, Vector2.down, rayBuffer, groundLayer);
        //
        // if any collider hits the ground we flag that collider as being landed
        if (hitRight.collider && hitRight.distance < minDistance)
        {
            if (airTimer == 0) manager.events.Landed();
            landedRight = true;
            jumped = false;
            airTimer = 0;
        }
        else landedRight = false;
        //
        if (hitLeft.collider && hitLeft.distance < minDistance)
        {
            if (airTimer == 0) manager.events.Landed();
            landedLeft = true;
            jumped = false;
            airTimer = 0;
        }
        else landedLeft = false;
        //
        if (hitMiddle.collider && hitMiddle.distance < minDistance)
        {
            if (airTimer == 0) manager.events.Landed();
            landedMiddle = true;
            jumped = false;
            airTimer = 0;
        }
        else landedRight = false;
        //
        // if the middle collider and at least one side collider hits the ground, we are grounded 
        if (landedMiddle && (landedLeft || landedRight)) manager.inAir = false;
        else manager.inAir = true;
    }
    //
    public void HandleJump() => jumped = true;
}
