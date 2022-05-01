using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public Weapon CurrentWeapon;
    [SerializeField]
    private Text _weaponText;

    public void ChangeWeapon(Weapon weapon)
    {
        foreach (GameObject obj in CurrentWeapon.Buttons) obj.SetActive(false);
        CurrentWeapon = weapon;
        foreach (GameObject obj in CurrentWeapon.Buttons) obj.SetActive(true);
        _weaponText.text = weapon.name;
    }
}
