using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gemmob.Common.UI {
    public class Frame : MonoBehaviour, IFrame {
        /** <summary>mainFrame: used for scale animation</summary> */
        [SerializeField] protected RectTransform mainFrame;
        [SerializeField] protected ButtonBase closeButton;

        public bool tapGroundToHide = true;
        public bool physicBackButtonEnable = true;

        protected float targetScale = 1f;

        #region IFrame
        public bool IsInstance { get; set; }
        public bool IsTopFrame { get; set; }
        public bool IsActive { get { return gameObject.activeSelf; } }

        protected virtual void Awake() {
            if (mainFrame == null) mainFrame = transform as RectTransform;
        }

        protected virtual void Start() {
            if (closeButton) {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
        }

        public virtual void Show(object data = null, bool animated = false) {
            gameObject.SetActive(true);
            if (animated) {
                AnimateShow(() => { if (Listener != null) Listener.OnShown(this); });
                return;
            }

            if (Listener != null) Listener.OnShown(this);
        }

        public virtual void Hide(bool animated = true) {
            if (animated) {
                AnimateHide(() => { if (Listener != null) Listener.OnHide(this); });
            }

            if (Listener != null) Listener.OnHide(this);
            gameObject.SetActive(false);
        }

        public virtual void Back(bool animate = true) {
            if (physicBackButtonEnable)
                Hide(animate);
        }

        public virtual void Pause(bool animated) {
            if (animated) {
                AnimateHide(() => { if (Listener != null) Listener.OnPaused(this); });
                return;
            }

            if (Listener != null) Listener.OnPaused(this);
            gameObject.SetActive(false);
        }

        public virtual void Resume(bool animated) {
            if (gameObject.activeSelf) return;

            gameObject.SetActive(true);
            if (animated) {
                AnimateShow(() => { if (Listener != null) Listener.OnResumed(this); });
                return;
            }
            if (Listener != null) Listener.OnResumed(this);
        }

        public IFrameListener Listener { get; set; }
        #endregion

        #region Animation
        void AnimateShow(Action callback = null) {
            if (!mainFrame) {
                if (callback != null) callback.Invoke();
                return;
            }

            StartCoroutine(IEScale(mainFrame.transform, targetScale - 0.2f, targetScale, 0.1f, callback));
        }

        void AnimateHide(Action callback = null) {
            if (callback != null) { callback.Invoke(); }
            gameObject.SetActive(false);
        }

        System.Collections.IEnumerator IEScale(Transform target, float fro, float to, float duration, System.Action callback) {
            if (duration > 0) {
                float elapsed = 0;
                target.localScale = Vector3.one * fro;
                while (elapsed < duration) {
                    float scale = Mathf.Lerp(fro, to, elapsed / duration);
                    target.localScale = Vector3.one * scale;
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            target.localScale = Vector3.one * to;
            if (callback != null) callback.Invoke();
            yield return null;
        }
        #endregion

        #region Button Callback
        public virtual void OnCloseButtonClicked() {
            Hide(true);
        }
        #endregion
    }

    public interface IFrame {
        void Show(object data, bool animated = true);
        void Hide(bool animated = true);
        void Back(bool animate = true);
        void Pause(bool animated = true);
        void Resume(bool animated = true);

        //bool Dismissable { get; set; }
        bool IsTopFrame { get; set; }
        bool IsInstance { get; set; }
        bool IsActive { get; }
        IFrameListener Listener { get; set; }
    }

    public interface IFrameListener {
        void OnShown(Frame frame);
        void OnHide(Frame frame);
        void OnPaused(Frame frame);
        void OnResumed(Frame frane);
    }
}