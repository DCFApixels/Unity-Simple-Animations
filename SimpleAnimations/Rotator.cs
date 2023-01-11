using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DCFApixels
{
    public class Rotator : SimpleAnimationBase
    {
        [Header("Rotator")]
        [SerializeField]
        private bool _isLocal = true;

        [SerializeField]
        private Vector3 _initialRotation;
        [SerializeField]
        private Vector3 _finishRotation;
        protected override void Do(float t)
        {
            Quaternion newRotation = Quaternion.Euler(Vector3.Lerp(_initialRotation, _finishRotation, t));
            if (_isLocal)
            {
                transform.localRotation = !IsRelative ? newRotation : newRotation * RelativeTransform.localRotation;
            }
            else
            {
                transform.rotation = !IsRelative ? newRotation : newRotation * RelativeTransform.Rotation;
            }
        }
    }
}