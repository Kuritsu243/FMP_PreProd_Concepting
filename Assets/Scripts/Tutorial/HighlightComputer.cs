using UnityEngine;

namespace Tutorial
{
    public class HighlightComputer : MonoBehaviour
    {
        [SerializeField] private float targetOutlineWidth;
        [SerializeField] private Color outLineColor;
        [SerializeField] private float timeToTake;

        private Outline _computerOutline;

        private void Start()
        {
            _computerOutline = GetComponent<Outline>();
            _computerOutline.OutlineWidth = 0;
            _computerOutline.enabled = false;
            _computerOutline.OutlineColor = outLineColor;
        }

        
        
        public void OutlineComputer()
        {
            _computerOutline.enabled = true;
            LeanTween.value(_computerOutline.gameObject, _computerOutline.OutlineWidth, targetOutlineWidth, timeToTake)
                .setOnUpdate(
                    f =>
                    {
                        _computerOutline.OutlineWidth = f;
                    })
                .setLoopPingPong();
        }

        public void StopOutline()
        {
            LeanTween.cancel(_computerOutline.gameObject);
            _computerOutline.enabled = false;
        }
        
    }
}
