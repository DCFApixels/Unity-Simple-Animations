using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DCFApixels
{
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