using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;

namespace Gemmob.Common.UI {
    /**<summary> Head Up Display </summary>  */
    public abstract class HUD<T> : MonoBehaviour, IFrameListener, IPointerDownHandler {
        [SerializeField] private Image background;
        private Dictionary<Type, Frame> type2Frame = new Dictionary<Type, Frame>();

        protected Stack<IFrame> frames = new Stack<IFrame>();

        /**<summary> Use for HUD update priority. </summary> */
        public bool Updated { get; private set; }

        public static T Instance { get; private set; }

        void Awake() {
            Instance = gameObject.GetComponent<T>();
            if (Instance == null) Debug.LogError(string.Format("[HUD] {0} Not found", typeof(T).ToString()));
            OnAwake();
            ShowBackground(false);
            var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var p in props) {
                if (!typeof(Frame).IsAssignableFrom(p.PropertyType)) continue;
                var o = p.GetValue(this, null) as Frame;
                if (o != null) {
                    type2Frame[p.PropertyType] = o;
                }
            }
        }

        protected virtual void OnAwake() {

        }

        void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)/*&& Global.Instance.GameMode != GameMode.Tutorial*/) {
                if (CurrentFrame != null) {
                    CurrentFrame.Back(true);
                    Updated = true;
                }
                else {
                    Updated = false;
                    OnUpdate();
                }
            }
        }

        protected virtual void OnUpdate() {

        }

        #region Show
        public void ShowBackground(bool show) {
            if (background) background.gameObject.SetActive(show);
        }

        public bool Contains(Frame frame) {
            return frames.Contains(frame);
        }

        public Stack<IFrame> GetFrames() {
            return frames;
        }

        /** <summary> Return multi instance of type T </summary>*/
        public Frame GetFrame<T>(object data = null, bool animated = true, bool dismissCurrent = true, bool pauseCurrent = true) where T : Frame {
            Frame frame = null;
            if (type2Frame.TryGetValue(typeof(T), out frame)) {
                if (frames.Contains(frame)) {
                    frame = Instantiate(frame, transform);
                    frame.IsInstance = true;
                }
            }

            return frame;
        }

        /** <summary> Show multi instance of type T </summary>*/
        public void Show<T>(object data = null, bool animated = true, bool dismissCurrent = true, bool pauseCurrent = true) where T : Frame {
            Frame frame = GetFrame<T>(data, animated, dismissCurrent, pauseCurrent);
            if (frame != null) {
                Show(frame, data, animated, dismissCurrent, pauseCurrent);
            }
        }

        /**<summary> Show only one instance </summary> */
        public virtual void Show(Frame frame, object data = null, bool animated = true, bool dismissCurrent = true, bool pauseCurrent = true) {
            if (frame == null) {
                Debug.LogError("[HUD] Frame is null!");
                return;
            }
            //SwitchAllScroll(frame, false);
            //ResetAllScroll(frame);
            if (frames.Contains(frame)) {
#if UNITY_EDITOR
                Logs.LogWarning("Frame already shown");
#endif
                return;
            }

            if (CurrentFrame != null && CurrentFrame.IsTopFrame) {
#if UNITY_EDITOR
                Logs.LogWarning("Cannot push on top frame");
#endif
                return;
            }

            ShowBackground(true);

            if (dismissCurrent && CurrentFrame != null) {
                var current = frames.Pop();
                current.Listener = null;
                current.Hide(true);
            }
            else {
                var current = CurrentFrame;
                if (current != null && pauseCurrent) {
                    current.Pause(true);
                }
            }
            frames.Push(frame);
            frame.Listener = this;
            frame.Show(data, animated);
        }
        #endregion

        protected void Pop() {
            if (CurrentFrame != null) {
                CurrentFrame.Hide(true);
            }
        }

        public void HideAll() {
            while (frames.Count > 0) {
                var frame = frames.Pop();
                frame.Listener = null;
                frame.Hide(false);
            }

            ShowBackground(false);
        }

        public IFrame CurrentFrame {
            get { return frames.Count > 0 ? frames.Peek() : null; }
        }

        #region IFrameListener
        public void OnShown(Frame frame) {
            //SwitchAllScroll(frame, true);
        }

        //private void ResetAllScroll(MonoFrame frame) {
        //    if (!frame) return;
        //    var cpns = frame.GetComponentsInChildren<ScrollRect>();
        //    foreach (var item in cpns) {
        //        if (item.content == null) continue;
        //        item.verticalNormalizedPosition = 1;
        //    }
        //}

        //private void SwitchAllScroll(MonoFrame frame, bool enable) {
        //    if (!frame) return;
        //    var cpns = frame.GetComponentsInChildren<ScrollRect>();
        //    foreach (var item in cpns) {
        //        item.enabled = enable;
        //    }
        //}

        public virtual void OnHide(Frame frame) {
            frame.Listener = null;
            if (CurrentFrame == frame) {
                frames.Pop();
                if (frame.IsInstance) {
                    Destroy(frame.gameObject);
                }
            }

            if (CurrentFrame != null) {
                CurrentFrame.Resume(true);
            }

            if (frames.Count == 0) {
                ShowBackground(false);
            }
        }

        public void OnPaused(Frame frame) {

        }

        public void OnResumed(Frame frane) {
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!background) return;
            var current = CurrentFrame as Frame;
            if (current == null || !current.tapGroundToHide) return;

            if (eventData.pointerCurrentRaycast.gameObject.name == background.gameObject.name) {
                current.Hide(true);
            }
        }

        #endregion
    }
}
