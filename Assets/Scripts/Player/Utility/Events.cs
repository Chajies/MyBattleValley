using System;

public class Events
{
    public event Action OnShot = delegate{};
    public event Action OnDied = delegate{};
    public event Action OnJumped = delegate{};
    public event Action OnLanded = delegate{};
    public event Action OnStopped = delegate{};
    public event Action OnReloaded = delegate{};
    public event Action<int> OnMoved = delegate{};
    public event Action OnResurrected = delegate{};
    public event Action OnHeldTrigger = delegate{};
    public event Action OnCycledWeapon = delegate{};
    public event Action<int> OnDirectionChanged = delegate{};
    //
    public void Shot() => OnShot();
    public void Died() => OnDied();
    public void Landed() => OnLanded();
    public void Jumped() => OnJumped();
    public void Stopped() => OnStopped();
    public void Reloaded() => OnReloaded();
    public void Resurrected() => OnResurrected();
    public void HeldTrigger() => OnHeldTrigger();
    public void CycledWeapon() => OnCycledWeapon();
    public void Moved(int direction) => OnMoved(direction);
    public void DirectionChanged(int direction) => OnDirectionChanged(direction);
}
