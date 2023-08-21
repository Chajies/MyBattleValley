using System.Collections;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class UIManager : NetworkBehaviour
{
    const int maxHealth = 100;
    //
    public TextMeshProUGUI[] healthText;
    public TextMeshProUGUI[] scoreText;
    //
    public Image[] playerWeaponIcon;
    public Sprite[] weaponIcons;
    public Image[] healthBar;
    //
    public Image[] playerOneBullets;
    public Image[] playerTwoBullets;
    //
    int[] bulletsLeft = new int[2];
    int[] score = new int[2];

    //

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }
    //
    void Initialize()
    {
        score[0] = score[1] = 0;
        scoreText[0].text = scoreText[1].text = score[0].ToString();
    }

    //

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerWeaponServerRpc(int playerIndex, int bullets, int weaponIndex) => SetPlayerWeaponClientRpc(playerIndex, bullets, weaponIndex);
    [ClientRpc]
    void SetPlayerWeaponClientRpc(int playerIndex, int bullets, int weaponIndex) => SetPlayerWeapon(playerIndex, bullets, weaponIndex);
    void SetPlayerWeapon(int playerIndex, int bullets, int weaponIndex)
    {
        playerWeaponIcon[playerIndex].sprite = weaponIcons[weaponIndex];
        SetBulletCount(playerIndex, bullets);
    }
    //
    void SetBulletCount(int playerIndex, int bullets)
    {
        var array = playerIndex == 0 ? playerOneBullets : playerTwoBullets;
        for (int i = 0; i < array.Length; i++)
        {
            if (i < bullets) array[i].enabled = true;
            else array[i].enabled = false;
        }
        //
        bulletsLeft[playerIndex] = bullets;
    }

    //

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateBulletDisplayServerRpc(int playerIndex) => UpdateBulletDisplayClientRpc(playerIndex);
    [ClientRpc]
    void UpdateBulletDisplayClientRpc(int playerIndex) => UpdateBulletDisplay(playerIndex);
    void UpdateBulletDisplay(int playerIndex)
    {
        if (bulletsLeft[playerIndex] <= 0) return;
        bulletsLeft[playerIndex]--;
        //
        var array = playerIndex == 0 ? playerOneBullets : playerTwoBullets;
        array[bulletsLeft[playerIndex]].enabled = false;
    }

    //

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthDisplayServerRpc(int playerNumber, int healthAmount) => UpdateHealthDisplayClientRpc(playerNumber, healthAmount);
    [ClientRpc]
    void UpdateHealthDisplayClientRpc(int playerNumber, int healthAmount) => UpdateHealthDisplay(playerNumber, healthAmount);
    void UpdateHealthDisplay(int playerNumber, int healthAmount)
    {
        healthText[playerNumber].text = $"{healthAmount} / {maxHealth}";
        healthBar[playerNumber].fillAmount = (float)healthAmount / maxHealth;
    }

    //

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseScoreServerRpc(int playerNumber) => IncreaseScoreClientRpc(playerNumber);
    [ClientRpc]
    void IncreaseScoreClientRpc(int playerNumber) => IncreaseScore(playerNumber);
    void IncreaseScore(int playerNumber)
    {
        score[playerNumber]++;
        scoreText[playerNumber].text = score[playerNumber].ToString();
    }

    //
    
    [ServerRpc(RequireOwnership = false)]
    public void ReloadBulletsServerRpc(int playerIndex, float reloadTime, int magazineSize) => ReloadBulletsClientRpc(playerIndex, reloadTime, magazineSize);
    [ClientRpc]
    void ReloadBulletsClientRpc(int playerIndex, float reloadTime, int magazineSize) => StartCoroutine(ReloadBulletsRoutine(playerIndex, reloadTime, magazineSize));
    IEnumerator ReloadBulletsRoutine(int playerIndex, float reloadTime, int magazineSize)
    {
        WaitForSeconds delay = new WaitForSeconds(reloadTime / magazineSize);
        var array = playerIndex == 0 ? playerOneBullets : playerTwoBullets;
        //
        for (int i = 0; i < magazineSize; i++)
        {
            yield return delay;
            array[i].enabled = true;
        }
        //
        bulletsLeft[playerIndex] = magazineSize;
    }
}
