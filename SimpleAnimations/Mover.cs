using UnityEngine;

namespace DCFApixels
{
    [AddComponentMenu("SimpleAnimations/Mover", 11)]

    public class Mover : SimpleAnimationBase
    {
        [Header(nameof(Mover))]

        [SerializeField]
        private bool _isLocal = true;

        [SerializeField]
        private Vector3 _initialPosition;
        [SerializeField]
        private Vector3 _finishPosition;

        protected override void Do(float t)
        {
            Vector3 newPosition = Vector3.Lerp(_initialPosition, _finishPosition, t);
            if (_isLocal)
            {
                transform.localPosition = !IsRelative ? newPosition : newPosition + RelativeTransform.localPosition;
            }
            else
            {
                transform.position = !IsRelative ? newPosition : newPosition + RelativeTransform.Position;
            }
        }
    }
}