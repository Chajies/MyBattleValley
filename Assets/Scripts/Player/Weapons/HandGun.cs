using System.Collections;
using UnityEngine;

public class HandGun : Weapon
{
    public HandGun(WeaponManager m)
    {
        range = 8;
        damage = 10;
        magazineSize = 6;
        isAutomatic = false;
        //
        manager = m;
        reloadTime = 0.75f;
        timeBetweenShots = 0.25f;
        bulletsLeft = magazineSize;
        timeToReload = new WaitForSeconds(reloadTime);
    }
}
