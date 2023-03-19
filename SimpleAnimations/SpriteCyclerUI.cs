using UnityEngine;
using UnityEngine.UI;

namespace DCFApixels
{
    [AddComponentMenu("SimpleAnimations/" + nameof(SpriteCyclerUI), 11)]
    [RequireComponent(typeof(Image))]
    public class SpriteCyclerUI : SimpleAnimationBase
    {
#if UNITY_EDITOR
        protected override void OnValidateEvent()
        {
            _image = GetComponent<Image>();
        }
#endif

        [Header(nameof(SpriteCyclerUI))]
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Sprite[] _spriteSequence;
    
        protected override void Do(float t)
        {
            int index = Mathf.Min((int)(t * _spriteSequence.Length), _spriteSequence.Length - 1);
            _image.sprite = _spriteSequence[index];
        }


    }
}