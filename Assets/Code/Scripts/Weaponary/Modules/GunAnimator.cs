using System.Collections;
using System.Collections.Generic;
using BoschingMachine.Weaponary;
using BoschingMachine.Bipedal;
using UnityEngine;
using BoschingMachine.Player;

namespace BoschingMachine.Weaponary.Modules
{
    [System.Serializable]
    public sealed class GunAnimator
    {
        [SerializeField] Vector3 homeOffset;
        [SerializeField] Vector3 homeRot;
        [SerializeField] SeccondOrderDynamicsV3 posSpring;
        [SerializeField] SeccondOrderDynamicsV3 rotSpring;
        [SerializeField] SeccondOrderDynamicsF fovSpring;

        [Space]
        [SerializeField] float swayMagnitude;

        [Space]
        [SerializeField] Vector3 holsterPosition;
        [SerializeField] Vector3 holsterRotation;

        [Space]
        [SerializeField] Vector3 reloadPosition;
        [SerializeField] Vector3 reloadRotation;

        [Space]
        [SerializeField] Vector3 aimPosition;
        [SerializeField] Vector3 aimRotation;

        [Space]
        [SerializeField] Vector3 fireForce;
        [SerializeField] Vector3 fireForceVariance;
        [SerializeField] Vector3 fireTorque;
        [SerializeField] Vector3 fireTorqueVariance;

        [Space]
        [SerializeField] float aimFOV;
        [SerializeField] float viewmodelFOV;
        [SerializeField] float viewmodelAimFOV;

        Vector2 lastCamRot;
        
        public void Setup(Gun gun)
        {
            gun.FireEvent += OnGunFire;
        }

        private void OnGunFire()
        {
            posSpring.Velocity += GetFromPointAndVariance(fireForce, fireForceVariance);
            rotSpring.Velocity += GetFromPointAndVariance(fireTorque, fireTorqueVariance);
        }

        public void Animate(Gun gun, Transform root, Transform slide)
        {
            Vector3 targetPos = homeOffset;
            Vector3 targetRot = homeRot;

            if (gun.Aiming)
            {
                targetPos = aimPosition;
                targetRot = aimRotation;
            }

            if (gun.MagazineModule.IsReloading)
            {
                targetPos = reloadPosition;
                targetRot = reloadRotation;
            }

            if (!gun.Equiped)
            {
                targetPos = holsterPosition;
                targetRot = holsterRotation;
            }

            Vector2 lookDelta = gun.User.LookRotation - lastCamRot;
            rotSpring.Velocity += (Vector3)lookDelta * swayMagnitude;

            lastCamRot = gun.User.LookRotation;

            rotSpring.ProcessOperation = SeccondOrderDynamics.ProcessRotation;

            posSpring.Process(targetPos, null, Time.deltaTime);
            rotSpring.Process(targetRot, null, Time.deltaTime);

            root.localPosition = posSpring.Position;
            root.localEulerAngles = new Vector3(rotSpring.Position.y, rotSpring.Position.z, -rotSpring.Position.x);

            if (gun.User is PlayerBiped)
            {
                PlayerBiped player = gun.User as PlayerBiped;

                fovSpring.Loop(gun.Aiming ? 1.0f : 0.0f, null, Time.deltaTime);
                float t = fovSpring.Position;
                player.FOV = Mathf.Lerp(player.FOV, aimFOV, t);
                player.ViewmodelFOV = Mathf.Lerp(viewmodelFOV, viewmodelAimFOV, t);
            }
        }

        public float GetFromPointAndVariance(float point, float variance)
        {
            return point + Random.Range(-variance, variance);
        }

        public Vector3 GetFromPointAndVariance(Vector3 point, Vector3 variance)
        {
            return new Vector3(
                GetFromPointAndVariance(point.x, variance.x),
                GetFromPointAndVariance(point.y, variance.y),
                GetFromPointAndVariance(point.z, variance.z)
                );
        }
    }
}
