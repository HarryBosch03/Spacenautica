using BoschingMachine.Bipedal;
using UnityEditor;
using UnityEngine;

namespace BoschingMachine.Weaponary
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSize;
        [SerializeField] LayerMask collisionMask;

        [Space]
        [SerializeField] GameObject projectileImpactPrefab;

        Biped shooter;

        new Rigidbody rigidbody;
        float damage;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void FixedUpdate()
        {
            float speed = rigidbody.velocity.magnitude;
            Ray ray = new Ray(rigidbody.position, rigidbody.velocity);

            if (Physics.SphereCast(ray, projectileSize, out var hit, speed * Time.deltaTime + 0.01f, collisionMask))
            {
                if (projectileImpactPrefab)
                {
                    Instantiate(projectileImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
                }
                print($"Hit {hit.transform.name}");

                Destroy(gameObject);
            }
        }

        public void Spawn (Transform origin, Biped shooter, float damage, float muzzleSpeed)
        {
            Projectile instance = Instantiate(this, origin.position, origin.rotation);
            instance.shooter = shooter;
            instance.damage = damage;

            instance.rigidbody.velocity = origin.forward * muzzleSpeed;
        }
    }
}
