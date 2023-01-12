﻿using UnityEngine;

namespace DCFApixels
{
    [AddComponentMenu("SimpleAnimations/Scaler", 11)]
    public class Scaler : SimpleAnimationBase
    {
        [Header("Scaler")]
        [SerializeField]
        private Vector3 _initialScale = Vector3.one;
        [SerializeField]
        private Vector3 _finishScale = Vector3.one;

        protected override void Do(float t)
        {
            transform.localScale = Vector3.Lerp(_initialScale, _finishScale, t);
        }
    }
}