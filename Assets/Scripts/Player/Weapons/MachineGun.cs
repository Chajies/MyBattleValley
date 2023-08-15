using System.Collections;
using UnityEngine;

public class MachineGun : Weapon
{
    public MachineGun(WeaponManager m)
    {
        range = 10;
        damage = 5;
        magazineSize = 30;
        isAutomatic = true;
        //
        manager = m;
        reloadTime = 1.2f;
        timeBetweenShots = 0.15f;
        bulletsLeft = magazineSize;
        timeToReload = new WaitForSeconds(reloadTime);
    }
}
