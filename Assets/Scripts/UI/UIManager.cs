using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
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
    void Awake() => Initialize();
    void Initialize()
    {
        score[0] = score[1] = 0;
        scoreText[0].text = scoreText[1].text = score[0].ToString();
    }
    //
    public void SetBulletCount(int playerIndex, Weapon weapon)
    {
        var array = playerIndex == 0 ? playerOneBullets : playerTwoBullets;
        for (int i = 0; i < array.Length; i++)
        {
            if (i < weapon.BulletsLeft()) array[i].enabled = true;
            else array[i].enabled = false;
        }
        //
        bulletsLeft[playerIndex] = weapon.BulletsLeft();
    }
    //
    public void SetPlayerWeapon(int playerIndex, Weapon weapon, int weaponIndex)
    {
        playerWeaponIcon[playerIndex].sprite = weaponIcons[weaponIndex];
        SetBulletCount(playerIndex, weapon);
    }
    //
    public void UpdateBulletDisplay(int playerIndex)
    {
        if (bulletsLeft[playerIndex] <= 0) return;
        bulletsLeft[playerIndex]--;
        //
        var array = playerIndex == 0 ? playerOneBullets : playerTwoBullets;
        array[bulletsLeft[playerIndex]].enabled = false;
    }
    //
    public void UpdateHealthDisplay(int playerNumber, int healthAmount)
    {
        healthText[playerNumber].text = $"{healthAmount} / {maxHealth}";
        healthBar[playerNumber].fillAmount = (float)healthAmount / maxHealth;
    }
    //
    public void IncreaseScore(int playerNumber)
    {
        score[playerNumber]++;
        scoreText[playerNumber].text = score[playerNumber].ToString();
    }
    //
    public IEnumerator ReloadBullets(int playerIndex, Weapon weapon)
    {
        WaitForSeconds delay = new WaitForSeconds(weapon.reloadTime / weapon.magazineSize);
        var array = playerIndex == 0 ? playerOneBullets : playerTwoBullets;
        //
        for (int i = 0; i < weapon.magazineSize; i++)
        {
            yield return delay;
            array[i].enabled = true;
        }
        //
        bulletsLeft[playerIndex] = weapon.magazineSize;
    }
}
