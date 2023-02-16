using UnityEngine;
using UnityEngine.UI;

namespace DCFApixels
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("SimpleAnimations/SpriteCyclerUI", 11)]
    public class SpriteCyclerUI : SimpleAnimationBase
    {
#if UNITY_EDITOR
        private new void OnValidate()
        {
            base.OnValidate();
            _spriteRenderer = GetComponent<Image>();
        }
#endif

        [Header("SpriteCycler")]
        [SerializeField]
        private Image _spriteRenderer;
        [SerializeField]
        private Sprite[] _spriteSequence;
    
        protected override void Do(float t)
        {
            int index = Mathf.Min((int)(t * _spriteSequence.Length), _spriteSequence.Length - 1);
            _spriteRenderer.sprite = _spriteSequence[index];
        }
    }
}