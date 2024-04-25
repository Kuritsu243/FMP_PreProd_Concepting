using System.Collections;
using UnityEngine;

namespace Tutorial
{
    public class HighlightWeapon : MonoBehaviour
    {
        [SerializeField] private float targetOutlineWidth;
        [SerializeField] private Color outlineColor;
        [SerializeField] private float timeToTake;
        [SerializeField] private float pulseTime;
        
        
        private Outline _weaponOutline;
        

        private void Start()
        {
            _weaponOutline = GetComponent<Outline>();
            _weaponOutline.OutlineWidth = 0;
            _weaponOutline.enabled = false;
            _weaponOutline.OutlineColor = outlineColor;
        }

        public void OutlineWeapon()
        {
            _weaponOutline.enabled = true;
            LeanTween.value(_weaponOutline.gameObject, _weaponOutline.OutlineWidth, targetOutlineWidth, 2f)
                .setOnUpdate(
                    f =>
                    {
                        _weaponOutline.OutlineWidth = f;
                    })
                .setLoopPingPong();
        }

        private IEnumerator LerpHighlight()
        {
            var elapsedTime = 0f;
            while (_weaponOutline.OutlineWidth < targetOutlineWidth)
            {
                _weaponOutline.OutlineWidth = Mathf.Lerp(2f, targetOutlineWidth, elapsedTime / timeToTake);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(PulseOutline());
            yield return null;
        }

        private IEnumerator PulseOutline()
        {
            var timeTaken = 0f;
            while (_weaponOutline.OutlineWidth > 0f)
            {
                _weaponOutline.OutlineWidth = Mathf.Lerp(_weaponOutline.OutlineWidth, 0f, timeTaken / pulseTime);
                timeTaken += Time.deltaTime;
                yield return null;
            }
            StartCoroutine(LerpHighlight());
            yield return null;

        }
    }
}
