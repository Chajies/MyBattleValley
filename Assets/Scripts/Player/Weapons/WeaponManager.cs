using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    const string poolTag = "Pool";
    const string uiTag = "UI";
    //
    [SerializeField] GameObject impactEffect;
    [SerializeField] AudioClip[] weaponSFX;
    [SerializeField] AudioClip cycleSFX;
    public AudioClip reloadSFX;
    //
    [SerializeField] LineRenderer shotTrail;
    [SerializeField] LineRenderer redDot;
    //
    [HideInInspector] public bool reloading;
    const int maxWeapons = 3;
    float lastTimeShot;
    int currentWeapon;
    bool playerDied;
    float timeShot;
    bool canShoot;
    //
    NetworkVariable<int> playerNumber = new NetworkVariable<int>();
    WaitForSeconds shotTrailTimer = new WaitForSeconds(0.05f);
    RaycastHit2D[] hitObject = new RaycastHit2D[2];
    ReloadBar reloadBar;
    UIManager uiManager;
    AudioSource source;
    Weapon[] weapons;
    Transform form;
    Camera mainCam;
    Events events;
    //
    Vector3 mouseWorldCoordinates;
    Vector3 bulletEndPosition;
    Vector3 mouseInput;
    Vector3 direction;

    //

    void Awake()
    {
        playerNumber.OnValueChanged += OnPlayerNumberChanged;
        reloadBar = GetComponentInChildren<ReloadBar>();
        source = GetComponent<AudioSource>();
        mainCam = Camera.main;
        form = transform;
    }
    //
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        uiManager = GameObject.FindGameObjectWithTag(uiTag).GetComponent<UIManager>();
        Initialize();
    }
    //
    void Start()
    {
        events.OnShot += Shoot;
        events.OnDied += HandleDeath;
        events.OnReloaded += HandleReload;
        events.OnHeldTrigger += HandleTriggerHeld;
        events.OnResurrected += HandleResurrection;
        events.OnCycledWeapon += HandleWeaponCycle;
    }
    //
    // Handle red dot sight to follow mouse and turn off when the player can't shoot or has died
    void LateUpdate()
    {
        if (!IsOwner || !Application.isFocused) return;
        if (playerDied)
        {
            redDot.enabled = false;
            return;
        }
        //
        SetMousePosition();
        if (!CanShoot()) redDot.enabled = false;
        else
        {
            redDot.SetPosition(0, form.position);
            redDot.SetPosition(1, bulletEndPosition);
            //
            redDot.enabled = true;
        }
    }
    //
    public void InjectEvents(Events e) => events = e;
    public void Shoot()
    {
        if (!canShoot) return;
        //
        SetMousePosition();
        PlaySFX(weaponSFX[currentWeapon]);
        uiManager.RequestUpdateBulletDisplayServerRpc(playerNumber.Value);
        //
        // Cached ray results to improve performance. As no bullets pierce there is no need to check for multiple hits
        int results = Physics2D.RaycastNonAlloc(form.position, direction, hitObject, weapons[currentWeapon].range);
        StartCoroutine(HandleHitResults(results));
        weapons[currentWeapon].UseBullet();
        //
        lastTimeShot = Time.time;
        if (OutOfBullets()) HandleReload();
    }
    //
    void SetMousePosition()
    {
        mouseInput.x = Input.mousePosition.x;
        mouseInput.y = Input.mousePosition.y;
        mouseInput.z = mainCam.nearClipPlane;
        //
        // mouseWorldCoordinates set to 0 as we are in 2D
        mouseWorldCoordinates = mainCam.ScreenToWorldPoint(mouseInput);
        mouseWorldCoordinates.z = 0;
        //
        direction = (mouseWorldCoordinates - form.position).normalized;
        bulletEndPosition = form.position + direction * weapons[currentWeapon].range;
    }
    //
    IEnumerator HandleHitResults(int results)
    {
        shotTrail.SetPosition(0, form.position);
        // As the shooter will always appear in result, there needs to be at least 2 to have hit something
        if (results < 2)
        {
            shotTrail.SetPosition(1, bulletEndPosition);
            //
            // Request hit to execute on all clients
            RequestHitServerRpc(bulletEndPosition);
            // Hit locally immediately
            SpawnHitEffect(bulletEndPosition);
        }
        else
        {
            bool alive = false;
            RaycastHit2D hit = hitObject[1];
            // check if hit player is alive. If no player is hit, treat platforms as being alive
            if (hit.transform.gameObject.layer == 6)
            {
                alive = hit.transform.GetComponent<Health>().TakeDamage(weapons[currentWeapon].damage);
            }
            else alive = true;
            //
            Vector3 hitPosition = alive ? hit.point : bulletEndPosition;
            shotTrail.SetPosition(1, hitPosition);
            RequestHitServerRpc(hitPosition);
            SpawnHitEffect(hitPosition);
        }
        //
        shotTrail.enabled = true;
        yield return shotTrailTimer;
        shotTrail.enabled = false;
    }
    //
    [ServerRpc]
    void RequestHitServerRpc(Vector3 position) => SpawnHitClientRpc(position);
    [ClientRpc]
    void SpawnHitClientRpc(Vector3 position)
    {
        if (!IsOwner) SpawnHitEffect(position);
    }
    //
    void SpawnHitEffect(Vector3 position)  => Instantiate(impactEffect, position, Quaternion.identity);
    bool CanShoot() => canShoot = Time.time - lastTimeShot > weapons[currentWeapon].timeBetweenShots && !reloading;
    bool OutOfBullets() => weapons[currentWeapon].BulletsLeft() == 0;
    //
    // For automatic weapons enable the player to hold the shoot button
    void HandleTriggerHeld()
    {
        if (!weapons[currentWeapon].isAutomatic) return;
        Shoot();
    }
    //
    void HandleReload()
    {
        if (reloading) return;
        reloading = true;
        //
        PlaySFX(reloadSFX);
        StartCoroutine(weapons[currentWeapon].Reload());
        StartCoroutine(reloadBar.LerpReloadBarSize(weapons[currentWeapon].reloadTime));
        uiManager.ReloadBulletsServerRpc(playerNumber.Value, weapons[currentWeapon].reloadTime, weapons[currentWeapon].magazineSize);
    }
    //
    void HandleWeaponCycle()
    {
        if (reloading) return;
        //
        currentWeapon++;
        currentWeapon = currentWeapon % maxWeapons;
        uiManager.SetPlayerWeaponServerRpc(playerNumber.Value, weapons[currentWeapon].BulletsLeft(), currentWeapon);
        //
        PlaySFX(cycleSFX);
    }
    //
    public void PlaySFX(AudioClip clip)
    {
        source.pitch = Random.Range(0.85f, 1f);
        source.PlayOneShot(clip);
    }
    //
    void Initialize()
    {
        if (IsOwner) CommitPlayerNumberToNetworkServerRpc((int)OwnerClientId);
        else playerNumber.Value = (int)OwnerClientId;
        //
        weapons = new Weapon[maxWeapons];
        weapons[0] = new HandGun(this);
        weapons[1] = new LongGun(this);
        weapons[2] = new MachineGun(this);
        //
        uiManager.SetPlayerWeaponServerRpc(playerNumber.Value, weapons[0].BulletsLeft(), 0);
    }
    //
    [ServerRpc]
    void CommitPlayerNumberToNetworkServerRpc(int id) => playerNumber.Value = (int)OwnerClientId;
    void OnPlayerNumberChanged(int prevId, int newId) => playerNumber.Value = newId;
    public int GetPlayerNumber() => playerNumber.Value;
    void HandleResurrection() => playerDied = false;
    void HandleDeath() => playerDied = true;
    public override void OnNetworkDespawn()
    {
        events.OnShot -= Shoot;
        events.OnDied -= HandleDeath;
        events.OnReloaded -= HandleReload;
        events.OnHeldTrigger -= HandleTriggerHeld;
        events.OnCycledWeapon -= HandleWeaponCycle;
        events.OnResurrected -= HandleResurrection;
        playerNumber.OnValueChanged -= OnPlayerNumberChanged;
    }
}
