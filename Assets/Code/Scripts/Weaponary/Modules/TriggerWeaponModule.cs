using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Weaponary.Modules
{
    [System.Serializable]
    public sealed class TriggerWeaponModule
    {
        [SerializeField] float fireRate;
        [SerializeField] bool fullAuto;

        float lastFireTime;
        bool lastFireState;

        public void Process(bool fireState, System.Action callback)
        {
            Process(fireState, () => { callback?.Invoke(); return true; });
        }
        public void Process(bool fireState, System.Func<bool> callback)
        {
            if (fireState && Time.time > lastFireTime + 60.0f / fireRate)
            {
                if (!lastFireState || fullAuto)
                {
                    if (callback())
                    {
                        lastFireTime = Time.time;
                    }
                }
            }

            lastFireState = fireState;
        }
    }
}
