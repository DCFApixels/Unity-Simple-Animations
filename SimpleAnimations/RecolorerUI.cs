using UnityEngine;
using UnityEngine.UI;

namespace DCFApixels
{
    [AddComponentMenu("SimpleAnimations/" + nameof(RecolorerUI), 11)]
    [RequireComponent(typeof(Image))]
    public class RecolorerUI : SimpleAnimationBase
    {
#if UNITY_EDITOR
        protected override void OnValidateEvent()
        {
            _image = GetComponent<Image>();
        }
#endif

        [Header(nameof(RecolorerUI))]
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Color _initialColor = Color.white;
        [SerializeField]
        private Color _finishColor = Color.white;

        protected override void Do(float t)
        {
            if (_image == null)
                return;

            _image.color = Color.Lerp(_initialColor, _finishColor, t);
        }
    }
}