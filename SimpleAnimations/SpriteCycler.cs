using UnityEngine;

namespace DCFApixels
{
    [RequireComponent(typeof(SpriteRenderer))]
    [AddComponentMenu("SimpleAnimations/SpriteCycler", 11)]
    public class SpriteCycler : SimpleAnimationBase
    {
#if UNITY_EDITOR
        private new void OnValidate()
        {
            base.OnValidate();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif

        [Header("SpriteCycler")]
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