using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    readonly int deathEffect = Animator.StringToHash("DeathEffect");
    const string uiTag = "UI";
    const int maxHealth = 100;
    int currentHealth;
    bool isDead;
    //
    WaitForSeconds respawnTime = new WaitForSeconds(3f);
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Animator anim;
    PlayerManager manager;
    UIManager ui;

    //

    void Awake() => manager = GetComponent<PlayerManager>();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ui = GameObject.FindGameObjectWithTag(uiTag).GetComponent<UIManager>();
    }
    //
    void Start() => currentHealth = maxHealth;
    public bool TakeDamage(int damage)
    {
        if (isDead) return false;
        //
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            KillPlayer();
            return false;
        }
        //
        ui.UpdateHealthDisplay(manager.PlayerNumber(), currentHealth);
        return true;
    }
    //
    public void KillPlayer()
    {
        isDead = true;
        currentHealth = 0;
        StartCoroutine(HandlePlayerDied());
        if (manager)
        {
            manager.isDead = true;
            manager.events.Died();
        }
        //
        ui.IncreaseScore(manager.PlayerNumber() == 0 ? 1 : 0);
        ui.UpdateHealthDisplay(manager.PlayerNumber(), currentHealth);
    }
    //
    IEnumerator HandlePlayerDied()
    {
        anim.SetTrigger(deathEffect);
        rend.enabled = false;
        yield return respawnTime;
        //
        isDead = false;
        rend.enabled = true;
        currentHealth = maxHealth;
        if (manager)
        {
            manager.isDead = false;
            manager.events.Resurrected();
        }
        //
        ui.UpdateHealthDisplay(manager.PlayerNumber(), currentHealth);
        transform.position = Vector3.zero;
    }
}
