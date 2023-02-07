using System.Collections;
using UnityEngine;

namespace BoschingMachine.Weaponary
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Weapon : MonoBehaviour
    {
        public abstract void PrimaryFireDown();        
        public abstract void PrimaryFireUp();        
        public abstract void SeccondaryFireDown();        
        public abstract void SeccondaryFireUp();        
        public abstract void Reload();        
        public abstract IEnumerator Equip();        
        public abstract IEnumerator Unequip();        
    }
}
