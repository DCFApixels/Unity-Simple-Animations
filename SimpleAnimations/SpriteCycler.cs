using UnityEngine;

namespace DCFApixels
{
    [AddComponentMenu("SimpleAnimations/" + nameof(SpriteCycler), 11)]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteCycler : SimpleAnimationBase
    {
#if UNITY_EDITOR
        protected override void OnValidateEvent()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif

        [Header(nameof(SpriteCycler))]
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Sprite[] _spriteSequence;
    
        protected override void Do(float t)
        {
            int index = Mathf.Min((int)(t * _spriteSequence.Length), _spriteSequence.Length - 1);
            _spriteRenderer.sprite = _spriteSequence[index];
        }
    }
}