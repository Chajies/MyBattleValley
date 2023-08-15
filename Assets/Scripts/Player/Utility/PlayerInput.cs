using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerInput
{
    int movedLeft = -1;
    int movedRight = 1;
    int lastDirection;
    int direction;
    Events events;

    //
    public PlayerInput(Events e) => events = e;
    public void Tick(bool isDead)
    {
        if (Input.GetKeyDown(KeyCode.P)) SceneManager.LoadScene(0);
        if (isDead) return;
        direction = 0;
        //
        if (Input.GetKeyDown(KeyCode.A)) events.DirectionChanged(movedLeft);
        else if (Input.GetKeyDown(KeyCode.D)) events.DirectionChanged(movedRight);
        //
        if (Input.GetKey(KeyCode.A)) direction = movedLeft;
        else if (Input.GetKey(KeyCode.D)) direction = movedRight;
        //
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) direction = 0;
        if (direction == 0 && lastDirection == 0) events.Stopped();
        if (Input.GetKeyDown(KeyCode.Space)) events.Jumped();
        //
        if (Input.GetKeyDown(KeyCode.Mouse0)) events.Shot();
        else if (Input.GetKey(KeyCode.Mouse0)) events.HeldTrigger();
        if (Input.GetKeyDown(KeyCode.Q)) events.CycledWeapon();
        //
        if (Input.GetKeyDown(KeyCode.R)) events.Reloaded();
        //
        events.Moved(direction);
        lastDirection = direction;
    }
}
