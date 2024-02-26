using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Cameras
{
    public static class CinemachineExtensions
    {

        public static void LerpFirstDutch(this CinemachineCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DutchLerp());
            IEnumerator DutchLerp()
            {
                var start = vcam.Lens.Dutch;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var dutchValue = Mathf.Lerp(start, endValue, timeElapsed);
                    vcam.Lens.Dutch = dutchValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }


        public static void LerpThirdDutch(this CinemachineCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DutchLerp());
            IEnumerator DutchLerp()
            {
                var start = vcam.Lens.Dutch;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var dutchValue = Mathf.Lerp(start, endValue, timeElapsed);
                    vcam.Lens.Dutch = dutchValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        public static void LerpFirstFOV(this CinemachineCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DoLerp());
            IEnumerator DoLerp()
            {
                var start = vcam.Lens.FieldOfView;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var fovValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.Lens.FieldOfView = fovValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        public static void LerpThirdFOV(this CinemachineCamera vcam, float endValue, float timeToTake)
        {
            vcam.StartCoroutine(DoLerp());
            IEnumerator DoLerp()
            {
                var start = vcam.Lens.FieldOfView;
                var timeElapsed = 0f;
                while (timeElapsed < timeToTake)
                {
                    var fovValue = Mathf.Lerp(start, endValue, timeElapsed / timeToTake);
                    vcam.Lens.FieldOfView = fovValue;
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

    }
}
