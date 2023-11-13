using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Camera
{
    public static class CinemachineExtensions
    {

        public static void LerpFirstDutch(this CinemachineVirtualCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DutchLerp());
            IEnumerator DutchLerp()
            {
                var start = vcam.m_Lens.Dutch;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var dutchValue = Mathf.Lerp(start, endValue, timeElapsed);
                    vcam.m_Lens.Dutch = dutchValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }


        public static void LerpThirdDutch(this CinemachineFreeLook vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DutchLerp());
            IEnumerator DutchLerp()
            {
                var start = vcam.m_Lens.Dutch;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var dutchValue = Mathf.Lerp(start, endValue, timeElapsed);
                    vcam.m_Lens.Dutch = dutchValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        public static void LerpFirstFOV(this CinemachineVirtualCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DoLerp());
            IEnumerator DoLerp()
            {
                var start = vcam.m_Lens.FieldOfView;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var fovValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.m_Lens.FieldOfView = fovValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        public static void LerpThirdFOV(this CinemachineFreeLook vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DoLerp());
            IEnumerator DoLerp()
            {
                var start = vcam.m_Lens.FieldOfView;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var fovValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.m_Lens.FieldOfView = fovValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

    }
}
