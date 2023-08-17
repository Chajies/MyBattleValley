using System.Collections;
using UnityEngine;

public class NonPooledEffect : MonoBehaviour
{
    readonly int impact = Animator.StringToHash("Impact");
    WaitForSeconds timeToDisable = new WaitForSeconds(1);
    public Animator anim;

    //

    void OnEnable()
    {
        StartCoroutine(WaitToDisable());
        anim.SetTrigger(impact);
    }
    //
    IEnumerator WaitToDisable()
    {
        yield return timeToDisable;
        Destroy(gameObject);
    }
}
