using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    readonly int deathEffect = Animator.StringToHash("DeathEffect");
    const int maxHealth = 100;
    int currentHealth;
    bool isDead;
    //
    WaitForSeconds respawnTime = new WaitForSeconds(3f);
    [SerializeField] Transform[] respawnLocations;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Animator anim;
    [SerializeField] UIManager ui;
    public int playerNumber;
    PlayerManager manager;

    //

    void Awake() => manager = GetComponent<PlayerManager>();
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
        ui.UpdateHealthDisplay(playerNumber, currentHealth);
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
        ui.IncreaseScore(playerNumber == 0 ? 1 : 0);
        ui.UpdateHealthDisplay(playerNumber, currentHealth);
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
        transform.position = respawnLocations[Random.Range(0, respawnLocations.Length)].position;
        ui.UpdateHealthDisplay(playerNumber, currentHealth);
    }
}
