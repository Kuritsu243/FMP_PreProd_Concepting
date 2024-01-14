using System;
using UnityEngine;

namespace Weapons
{
    public class Pistol : BaseWeapon
    {
        public override void Fire()
        {
            Debug.Log("pistol firing");
            base.Fire();
        }
    }
    
}
