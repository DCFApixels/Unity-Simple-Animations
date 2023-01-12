using UnityEngine;
using UnityRandom = UnityEngine.Random;
using UnityTime = UnityEngine.Time;

namespace DCFApixels
{
    public abstract class SimpleAnimationBase : MonoBehaviour
    {
        #region Editor
#if UNITY_EDITOR
        protected void OnValidate()
        {
            if(_duration < float.Epsilon)
            {
                _duration = float.Epsilon;
            }
        }
#endif
        #endregion

        [Header("Time")]
        [SerializeField]
        private bool _randomStartTime;
        [SerializeField, Range(0f, 1f)]
        private float _startTime;
        [SerializeField]
        private float _duration = 1f;

        [Header("Other")]
        [SerializeField]
        private bool _playOnEnable = true;
        [SerializeField]
        private bool _isRelative = true;
        [SerializeField]
        private eUpdateMode _updateMode;
        [SerializeField]
        private int _count = -1;
        [SerializeField]
        private eLoopMode _loopMode = eLoopMode.Yoyo;
        [SerializeField]
        private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField]
        private bool _yoyoCurve = true;

        //[SerializeField]
        //private bool _isTimeScaled = true; //TODO

        private bool _isRunning = false;
        private bool _isPaused = true;
        private float _time;
        private int _loopNumber;
        private TransformState _relativeTransform;

        #region Properties
        public bool PlayOnEnable
        {
            get => _playOnEnable; set
            {
                _playOnEnable = value;
                if(!_isRunning && enabled)
                {
                    StartAnimation();
                }
            }
        }
        [SerializeField]
        public bool IsRelative { get => _isRelative; set => _isRelative = value; }
        public bool RandomStartTime { get => _randomStartTime; set => _randomStartTime = value; }
        public float StartTime { get => _startTime; set => _startTime = value <= 1f ? value : value % 1f; }
        public float Duration
        {
            get => _duration; set
            {
                if (value < float.Epsilon)
                {
                    value = float.Epsilon;
                }
                _duration = value;
            }
        }
        public eUpdateMode UpdateMode { get => _updateMode; set => _updateMode = value; }
        public int Count { get => _count; set => _count = value; }
        public eLoopMode LoopMode { get => _loopMode; set => _loopMode = value; }
        public AnimationCurve Curve { get => _curve; set => _curve = value; }
        public bool YoyoCurve { get => _yoyoCurve; set => _yoyoCurve = value; }
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;
        public float Time => _time;
        public int LoopNumber => _loopNumber;
        public TransformState RelativeTransform => _relativeTransform;
        #endregion
        private void Start()
        {
            _relativeTransform = new TransformState(transform);
            Init();
        }

        private void OnEnable()
        {
            if(_isRunning == false && _playOnEnable)
            {
                StartAnimation();
            }
        }

        #region Updates
        private void Update()
        {
            if (_updateMode != eUpdateMode.Default)
                return;

            Apply(UnityTime.deltaTime);
        }
        private void LateUpdate()
        {
            if (_updateMode != eUpdateMode.Late)
                return;

            Apply(UnityTime.deltaTime);
        }
        private void FixedUpdate()
        {
            if (_updateMode != eUpdateMode.Fixed)
                return;

            Apply(UnityTime.deltaTime);
        }
        public void ManualUpdate(float deltaTime)
        {
            if (_updateMode != eUpdateMode.Manual)
                return;

            Apply(deltaTime);
        }

        #endregion

        #region Controls
        [ContextMenu("StartAnimation")]
        public void StartAnimation()
        {
            _loopNumber = 0;
            if(_randomStartTime)
            {
                _time = UnityRandom.Range(0f, _duration);
            }
            else
            {
                _time = _duration * _startTime;
            }
            _isRunning = true;
            _isPaused = false;
        }
        [ContextMenu("StopAnimation")]
        public void StopAnimation()
        {
            _isRunning = false;
            _isPaused = true;
        }

        [ContextMenu("ResumeAnimation")]
        public void ResumeAnimation()
        {
            _isPaused = false;
        }
        [ContextMenu("PauseAnimation")]
        public void PauseAnimation()
        {
            _isPaused = true;
        }

        #endregion

        private void Apply(float deltaTime)
        {
            if (_isPaused || !_isRunning)
                return;

            if (_count >= 0 && _loopNumber >= _count)
                return;

            _time += deltaTime;
            if (_time >= _duration)
            {
                _loopNumber++;
                _time = 0f;
            }

            float t = _time / _duration;
            if(!_yoyoCurve)
                t = _curve.Evaluate(t);

            if (_loopMode == eLoopMode.Yoyo && _loopNumber % 2 == 1)
            {
                t = 1f - t;
            }


            if (_yoyoCurve)
                t = _curve.Evaluate(t);
            Do(t);
        }

        protected virtual void Init() { }
        protected abstract void Do(float t);

        #region Utils
        public enum eLoopMode
        {
            Restart,
            Yoyo,
        }
        public enum eUpdateMode
        {
            Default,
            Late,
            Fixed,
            Manual,
        }

        [System.Serializable]
        public struct TransformState
        {
            public Transform parent;
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;

            public Vector3 Position => parent == null ? localPosition : parent.TransformPoint(localPosition);
            public Quaternion Rotation => parent == null ? localRotation : parent.rotation * localRotation;


            public TransformState(Transform transform)
            {
                parent = transform.parent;
                localPosition = transform.localPosition;
                localRotation = transform.localRotation;
                localScale = transform.localScale;
            }
        }
        #endregion
    }
}

#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    using System;
    using System.Reflection;
    using UnityEditor;

    [CustomEditor(typeof(SimpleAnimationBase), true)]
    public class SimpleAnimationBaseEditor : Editor
    {
        private static Type _baseType = typeof(SimpleAnimationBase);
        private static MethodInfo _startMethodInfo   = _baseType.GetMethod("StartAnimation", BindingFlags.Instance | BindingFlags.Public);
        private static MethodInfo _stopMethodInfo    = _baseType.GetMethod("StopAnimation"    , BindingFlags.Instance | BindingFlags.Public);
        private static MethodInfo _pauseMethodInfo   = _baseType.GetMethod("PauseAnimation"   , BindingFlags.Instance | BindingFlags.Public);
        private static MethodInfo _resumeMethodInfo  = _baseType.GetMethod("ResumeAnimation"  , BindingFlags.Instance | BindingFlags.Public);

        private static Color _timeBarBackgroundColor = new Color(0, 0, 0, 0.34f);
        private static Color _timeBarForegroundColor = new Color(111f / 255f, 186f / 255f, 1f, 0.37f);

        private const float TIMEBAR_HEIGHT = 4f;

        public override void OnInspectorGUI()
        {
            SimpleAnimationBase target = this.target as SimpleAnimationBase;

            Rect timebarRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, TIMEBAR_HEIGHT);
            Rect rect = timebarRect;
            rect.width = rect.width * (target.Time / target.Duration);

            EditorGUI.DrawRect(timebarRect, _timeBarBackgroundColor);
            EditorGUI.DrawRect(rect, _timeBarForegroundColor);

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            base.OnInspectorGUI();

            if(!target.IsRunning)
            {
                if (GUILayout.Button("Start"))
                {
                    _startMethodInfo.Invoke(target, null);
                }
            }
            else
            {
                if (GUILayout.Button("Stop"))
                {
                    _stopMethodInfo.Invoke(target, null);
                }
            }

            if(!target.IsPaused)
            {
                if (GUILayout.Button("Pause"))
                {
                    _pauseMethodInfo.Invoke(target, null);
                }
            }
            else
            {
                if (GUILayout.Button("Resume"))
                {
                    _resumeMethodInfo.Invoke(target, null);
                }
            }
        }
    }
}
#endif
