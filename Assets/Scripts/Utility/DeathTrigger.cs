using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] UIManager ui;
    const int playerLayer = 6;

    //

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.layer != playerLayer) return;
        //
        Health player = hit.gameObject.GetComponent<Health>();
        player.KillPlayer();
    }
}
