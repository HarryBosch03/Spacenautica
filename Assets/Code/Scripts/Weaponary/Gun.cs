using System.Collections;
using System.Collections.Generic;
using BoschingMachine.Weaponary.Modules;
using BoschingMachine.Bipedal;
using UnityEngine;

namespace BoschingMachine.Weaponary
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Gun : Weapon
    {
        [SerializeField] ProjectileWeaponModules projectileModule;
        [SerializeField] MagazineWeaponModule magazineModule;
        [SerializeField] TriggerWeaponModule[] fireModes;
        [SerializeField] GunAnimator animator;
        [SerializeField] int fireModeIndex;
        [SerializeField] Transform muzzle;
        [SerializeField] Transform root;
        [SerializeField] Transform slide;

        [Space]
        [SerializeField] float equipTime;
        [SerializeField] float unequipTime;

        bool fireState;

        public Biped User { get; private set; }
        public ProjectileWeaponModules ProjectileModule => projectileModule;
        public MagazineWeaponModule MagazineModule => magazineModule;
        public bool Equiped { get; private set; }
        public bool Aiming { get; set; }

        public event System.Action FireEvent;

        private void Awake()
        {
            User = GetComponentInParent<Biped>();
        }

        private void Start()
        {
            animator.Setup(this);
        }

        private void Update()
        {
            TriggerWeaponModule trigger = fireModes[fireModeIndex];

            trigger.Process(fireState, Fire);
        }

        private void LateUpdate()
        {
            animator.Animate(this, root, slide);
        }

        public bool Fire()
        {
            return magazineModule.TryFire(() =>
            {
                projectileModule.Fire(muzzle, User);
                FireEvent?.Invoke();
            });
        }

        public override void PrimaryFireDown()
        {
            fireState = true;
        }

        public override void PrimaryFireUp()
        {
            fireState = false;
        }

        public override void SeccondaryFireDown()
        {
            Aiming = true;
        }

        public override void SeccondaryFireUp()
        {
            Aiming = false;
        }

        public override void Reload()
        {
            magazineModule.Reload(this, null);
        }

        public void SwitchFireMode ()
        {
            fireModeIndex = (fireModeIndex + 1) % fireModes.Length;
        }

        public override IEnumerator Equip()
        {
            Equiped = true;
            yield return new WaitForSeconds(equipTime);
        }

        public override IEnumerator Unequip()
        {
            Equiped = false;
            yield return new WaitForSeconds(unequipTime);
        }
    }
}
