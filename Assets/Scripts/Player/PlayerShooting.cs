using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [Header("Fists Class for game start or when no weapon")]
        [SerializeField] private BaseWeapon fists;
        [SerializeField] private TextMeshProUGUI ammoReporter;
        [SerializeField] private GameObject ammoPanel;
        [SerializeField] private Image reloadBar;
        private BaseWeapon _previousWeapon;
        public BaseWeapon CurrentWeapon { get; private set; }
        public bool HasWeapon()
        {
            return CurrentWeapon;
        }
        

        public void EquipWeapon(BaseWeapon newWeapon)
        {
            CurrentWeapon = newWeapon;
            CurrentWeapon.CurrentPrimaryAmmo = CurrentWeapon.maxPrimaryAmmo;
            CurrentWeapon.CurrentSecondaryAmmo = CurrentWeapon.maxSecondaryAmmo;
        }

        public void Reload()
        {
            if (!CurrentWeapon) return;
            CurrentWeapon.Reload();
        }

        public void Fire()
        {
            if (!CurrentWeapon || CurrentWeapon.CurrentPrimaryAmmo <= 0) return;
            CurrentWeapon.Fire();
        }
        
    }
}