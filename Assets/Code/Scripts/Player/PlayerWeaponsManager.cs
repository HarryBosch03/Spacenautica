using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Weaponary;
using System;

namespace BoschingMachine.Player
{
    [System.Serializable]
    public sealed class PlayerWeaponsManager
    {
        [SerializeField] Transform weaponContainer;

        public int WeaponIndex { get; private set; }
        public Weapon CurrentWeapon
        {
            get
            {
                if (WeaponIndex < 0 || WeaponIndex >= weaponContainer.childCount) return null;

                else return weaponContainer.GetChild(WeaponIndex).GetComponent<Weapon>();
            }
        }

        public void PrimaryFireDown() => WeaponCallback(w => w.PrimaryFireDown());
        public void PrimaryFireUp() => WeaponCallback(w => w.PrimaryFireUp());
        public void SeccondaryFireDown() => WeaponCallback(w => w.SeccondaryFireDown());
        public void SeccondaryFireUp() => WeaponCallback(w => w.SeccondaryFireUp());
        public void Reload() => WeaponCallback(w => w.Reload());

        public void Setup (PlayerBiped player, int startingIndex)
        {
            foreach (Transform child in weaponContainer)
            {
                child.gameObject.SetActive(false);
            }

            WeaponIndex = -1;
            SetWeaponIndex(player, startingIndex);
        }

        public void WeaponCallback (System.Action<Weapon> callback)
        {
            Weapon weapon = CurrentWeapon;
            if (!CurrentWeapon) return;

            callback(weapon);
        }

        public void SetWeaponIndex (PlayerBiped player, int index)
        {
            player.StartCoroutine(SwitchWeaponRoutine(player, index));
        }

        private IEnumerator SwitchWeaponRoutine(PlayerBiped player, int index)
        {
            Weapon weapon = CurrentWeapon;
            if (CurrentWeapon)
            {
                yield return player.StartCoroutine(weapon.Unequip());
                weapon.gameObject.SetActive(false);
            }

            WeaponIndex = index;
            weapon = CurrentWeapon;
            if (CurrentWeapon)
            {
                weapon.gameObject.SetActive(true);
                yield return player.StartCoroutine(weapon.Equip());
            }
        }
    }
}
