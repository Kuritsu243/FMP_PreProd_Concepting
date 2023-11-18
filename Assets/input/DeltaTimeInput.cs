using Cinemachine;
using UnityEngine;

namespace input
{
    public class DeltaTimeInput : CinemachineInputProvider
    {
        public override float GetAxisValue(int axis) => base.GetAxisValue(axis) / Time.deltaTime;
    }
}
