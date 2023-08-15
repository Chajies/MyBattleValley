using System.Collections;
using UnityEngine;

public class ReloadBar : MonoBehaviour
{
    Vector3 startScale = new Vector3(0, 1, 1);
    Vector3 localScale;
    //
    public GameObject barHolder;
    public Transform bar;
    float elapsedTime;
    float xValue;

    //

    public IEnumerator LerpReloadBarSize(float reloadTime)
    {
        barHolder.SetActive(true);
        while (elapsedTime < reloadTime)
        {
            localScale = bar.localScale;
            elapsedTime += Time.deltaTime;
            xValue = Mathf.Lerp(0, 1, elapsedTime / reloadTime);
            //
            localScale.x = xValue;
            bar.localScale = localScale;
            yield return null;
        }
        //
        ResetBar();
    }
    //
    void ResetBar()
    {
        bar.localScale = startScale;
        barHolder.SetActive(false);
        elapsedTime = 0;
    }
}
