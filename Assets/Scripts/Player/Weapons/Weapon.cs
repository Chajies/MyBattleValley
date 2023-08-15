using System.Collections;
using UnityEngine;

public class Weapon
{
    // cached WaitForSeconds to improve performance
    protected WaitForSeconds timeToReload;
    protected WeaponManager manager;
    protected int bulletsLeft;
    //
    public int range;
    public int damage;
    public int magazineSize;
    public bool isAutomatic;
    public float reloadTime;
    public float timeBetweenShots;

    public IEnumerator Reload()
    {
        yield return timeToReload;
        //
        manager.PlaySFX(manager.reloadSFX);
        bulletsLeft = magazineSize;
        manager.reloading = false;
    }

    //

    public int BulletsLeft() => bulletsLeft;
    public void UseBullet() => bulletsLeft--;
}
