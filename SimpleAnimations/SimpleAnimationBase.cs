using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using UnityTime = UnityEngine.Time;

namespace DCFApixels
{
    public abstract class SimpleAnimationBase : MonoBehaviour
    {
#if UNITY_EDITOR
        protected void OnValidate()
        {
            if(_duration < float.Epsilon)
            {
                _duration = float.Epsilon;
            }
        }
#endif

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
                    StartAnimatiion();
                }
            }
        }
        [SerializeField]
        public bool IsRelative { get => _isRelative; set => _isRelative = value; }
        public bool RandomStartTime { get => _randomStartTime; set => _randomStartTime = value; }
        public float StartTime { get => _startTime; set => _startTime = value <= 1f ? value : value % 1f; }
        public eUpdateMode UpdateMode { get => _updateMode; set => _updateMode = value; }
        public float Duration { get => _duration; set => _duration = value; }
        public int Count { get => _count; set => _count = value; }
        public eLoopMode LoopMode { get => _loopMode; set => _loopMode = value; }
        public AnimationCurve Curve { get => _curve; set => _curve = value; }
        public bool YoyoCurve { get => _yoyoCurve; set => _yoyoCurve = value; }
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;
        public float Time => _time;
        public int LoopNumber => _loopNumber;
        public TransformState RelativeTransform => _relativeTransform;

        private void Start()
        {
            _relativeTransform = new TransformState(transform);
            Init();
        }
        #endregion

        private void OnEnable()
        {
            if(_isRunning == false && _playOnEnable)
            {
                StartAnimatiion();
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
        [ContextMenu("StartAnimatiion")]
        public void StartAnimatiion()
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
            if (_isPaused)
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
    }
}