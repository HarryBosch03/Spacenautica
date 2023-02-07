using System;
using System.Collections;
using UnityEngine;

namespace BoschingMachine.Weaponary.Modules
{
    [System.Serializable]
    public sealed class MagazineWeaponModule
    {
        [SerializeField] int magazineSize;
        [SerializeField] int currentMagazine;
        [SerializeField] float reloadTime;

        public bool IsReloading { get; private set; }

        MonoBehaviour routineRunner;
        Coroutine routine;

        public void Reload(MonoBehaviour context, System.Action finishCallback = null)
        {
            routineRunner = context;
            routine = context.StartCoroutine(ReloadRoutine(finishCallback));
        }

        public void StopReload()
        {
            routineRunner.StopCoroutine(routine);
            IsReloading = false;
        }

        private IEnumerator ReloadRoutine(System.Action finishCallback)
        {
            if (IsReloading) yield break;
            IsReloading = true;
            currentMagazine = 0;

            yield return new WaitForSeconds(reloadTime);

            IsReloading = false;
            currentMagazine = magazineSize;
            finishCallback?.Invoke();
        }

        public bool CanFire()
        {
            if (currentMagazine <= 0) return false;
            if (IsReloading) return false;

            return true;
        }

        public bool TryFire(System.Action callback)
        {
            if (CanFire())
            {
                callback();
                currentMagazine--;
                return true;
            }
            else return false;
        }
    }
}
