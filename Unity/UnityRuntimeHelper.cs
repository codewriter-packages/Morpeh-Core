namespace Scellecs.Morpeh {
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System;
    using Collections;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal class UnityRuntimeHelper : MonoBehaviour {
        internal static Action             onApplicationFocusLost = () => {};
        internal static UnityRuntimeHelper instance;

#if UNITY_EDITOR
        private void OnEnable() {
            if (instance == null) {
                instance = this;

                EditorApplication.playModeStateChanged += this.OnEditorApplicationOnplayModeStateChanged;
            }
            else {
                Destroy(this);
            }
        }

        private void OnDisable() {
            if (instance == this) {
                instance = null;
            }
        }

        private void OnEditorApplicationOnplayModeStateChanged(PlayModeStateChange state) {
            //todo: check for fastmode
            if (state == PlayModeStateChange.EnteredEditMode) {
                for (var i = World.worlds.length - 1; i >= 0; i--) {
                    var world = World.worlds.data[i];
                    world?.Dispose();
                }

                World.worlds.Clear();
                World.worlds.Add(null);

                if (this != null && this.gameObject != null) {
                    DestroyImmediate(this.gameObject);
                }

                EditorApplication.playModeStateChanged -= this.OnEditorApplicationOnplayModeStateChanged;
            }
        }
#endif

        private void Update() => WorldExtensions.GlobalUpdate(Time.deltaTime);

        private void FixedUpdate() => WorldExtensions.GlobalFixedUpdate(Time.fixedDeltaTime);
        private void LateUpdate()  => WorldExtensions.GlobalLateUpdate(Time.deltaTime);

        internal void OnApplicationPause(bool pauseStatus) {
            if (pauseStatus) {
                onApplicationFocusLost.Invoke();
                GC.Collect();
            }
        }

        internal void OnApplicationFocus(bool hasFocus) {
            if (!hasFocus) {
                onApplicationFocusLost.Invoke();
                GC.Collect();
            }
        }

        internal void OnApplicationQuit() {
            onApplicationFocusLost.Invoke();
        }
    }
}
