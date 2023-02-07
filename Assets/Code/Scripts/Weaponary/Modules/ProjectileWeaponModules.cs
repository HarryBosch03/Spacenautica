using UnityEngine;
using BoschingMachine.Bipedal;

namespace BoschingMachine.Weaponary.Modules
{
    [System.Serializable]
    public sealed class ProjectileWeaponModules
    {
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] float damage;
        [SerializeField] float muzzleSpeed;

        public void Fire (Transform origin, Biped shooter)
        {
            projectilePrefab.Spawn(origin, shooter, damage, muzzleSpeed);
        }
    }
}
