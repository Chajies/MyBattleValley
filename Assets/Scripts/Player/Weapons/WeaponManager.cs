using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] AudioClip[] weaponSFX;
    [SerializeField] AudioClip cycleSFX;
    public AudioClip reloadSFX;
    public int playerNumber;
    //
    [SerializeField] ObjectPoolManager poolManager;
    [SerializeField] UIManager uiManager;
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
    WaitForSeconds shotTrailTimer = new WaitForSeconds(0.05f);
    RaycastHit2D[] hitObject = new RaycastHit2D[2];
    ReloadBar reloadBar;
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
        reloadBar = GetComponentInChildren<ReloadBar>();
        source = GetComponent<AudioSource>();
        mainCam = Camera.main;
        form = transform;
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
    void LateUpdate()
    {
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
        int results = Physics2D.RaycastNonAlloc(form.position, direction, hitObject, weapons[currentWeapon].range);
        //
        uiManager.UpdateBulletDisplay(playerNumber);
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
        if (results < 2)
        {
            poolManager.ReuseObject(poolManager.bulletID, bulletEndPosition);
            shotTrail.SetPosition(1, bulletEndPosition);
        }
        else
        {
            bool alive = false;
            RaycastHit2D hit = hitObject[1];
            if (hit.transform.gameObject.layer == 6)
            {
                alive = hit.transform.GetComponent<Health>().TakeDamage(weapons[currentWeapon].damage);
            }
            else alive = true;
            //
            poolManager.ReuseObject(poolManager.bulletID, alive ? hit.point : bulletEndPosition);
            shotTrail.SetPosition(1, alive ? hit.point : bulletEndPosition);
        }
        //
        shotTrail.enabled = true;
        yield return shotTrailTimer;
        shotTrail.enabled = false;
    }
    //
    bool OutOfBullets() => weapons[currentWeapon].BulletsLeft() == 0;
    bool CanShoot() => canShoot = Time.time - lastTimeShot > weapons[currentWeapon].timeBetweenShots && !reloading;
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
        StartCoroutine(uiManager.ReloadBullets(playerNumber, weapons[currentWeapon]));
        StartCoroutine(reloadBar.LerpReloadBarSize(weapons[currentWeapon].reloadTime));
    }
    void HandleWeaponCycle()
    {
        if (reloading) return;
        //
        currentWeapon++;
        currentWeapon = currentWeapon % maxWeapons;
        uiManager.SetPlayerWeapon(playerNumber, weapons[currentWeapon], currentWeapon);
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
        weapons = new Weapon[maxWeapons];
        //
        weapons[0] = new HandGun(this);
        weapons[1] = new LongGun(this);
        weapons[2] = new MachineGun(this);
        //
        uiManager.SetPlayerWeapon(playerNumber, weapons[0], 0);
    }
    //
    void HandleDeath() => playerDied = true;
    void HandleResurrection() => playerDied = false;
    void OnDisable()
    {
        events.OnShot -= Shoot;
        events.OnDied -= HandleDeath;
        events.OnReloaded -= HandleReload;
        events.OnHeldTrigger -= HandleTriggerHeld;
        events.OnCycledWeapon -= HandleWeaponCycle;
        events.OnResurrected -= HandleResurrection;
    }
}
