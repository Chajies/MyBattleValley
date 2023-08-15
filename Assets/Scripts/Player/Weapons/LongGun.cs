using System.Collections;
using UnityEngine;

public class LongGun : Weapon
{
    public LongGun(WeaponManager m)
    {
        range = 13;
        damage = 15;
        magazineSize = 12;
        isAutomatic = false;
        //
        manager = m;
        reloadTime = 1.75f;
        timeBetweenShots = 0.65f;
        bulletsLeft = magazineSize;
        timeToReload = new WaitForSeconds(reloadTime);
    }
}
