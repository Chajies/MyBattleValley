using System.Collections;
using UnityEngine;

public class PooledEffect : PoolObject
{
    readonly int impact = Animator.StringToHash("Impact");
    WaitForSeconds timeToDisable = new WaitForSeconds(1);
    public Animator anim;

    //

    public override void OnObjectReuse()
    {
        StartCoroutine(WaitToDisable());
        anim.SetTrigger(impact);
    }
    //
    IEnumerator WaitToDisable()
    {
        yield return timeToDisable;
        gameObject.SetActive(false);
    }
}
