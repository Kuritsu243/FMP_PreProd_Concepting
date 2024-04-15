using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Lighting
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private Color lightFlickerColour;
        [SerializeField] private float minIntensity;
        [SerializeField] private float maxIntensity;
        [SerializeField] private bool enableFlicker;
        [SerializeField] private int lightSmoothing = 8;


        private float _lastSum;
        private Light _light;
        private Queue<float> _lightQueue;
        
        private void Start()
        {
            _light = GetComponentInChildren<Light>();

            if (enableFlicker && lightSmoothing > 0)
                _lightQueue = new Queue<float>(lightSmoothing);
        }

        private void FixedUpdate()
        {
            if (!enableFlicker && lightSmoothing > 0) return;

            while (_lightQueue.Count >= lightSmoothing)
            {
                _lastSum -= _lightQueue.Dequeue();
            }

            var newVal = Random.Range(minIntensity, maxIntensity);
            _lightQueue.Enqueue(newVal);
            _lastSum += newVal;
            _light.intensity = _lastSum / (float)_lightQueue.Count;
        }

        private void Reset()
        {
            _lightQueue.Clear();
            _lastSum = 0;
        }
    }
}
