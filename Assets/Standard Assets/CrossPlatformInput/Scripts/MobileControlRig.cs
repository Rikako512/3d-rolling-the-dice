using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    [ExecuteInEditMode]
    public class MobileControlRig : MonoBehaviour
#if UNITY_EDITOR
        , IActiveBuildTargetChanged
#endif
    {
        // this script enables or disables the child objects of a control rig
        // depending on whether the USE_MOBILE_INPUT define is declared.

        // This define is set or unset by a menu item that is included with
        // the Cross Platform Input package.

#if !UNITY_EDITOR
        void OnEnable()
        {
            CheckEnableControlRig();
        }
#endif

        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) //if in the editor, need to check if we are playing, as start is also called just after exiting play
#endif
            {
                UnityEngine.EventSystems.EventSystem system = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

                if (system == null)
                {//the scene have no event system, spawn one
                    GameObject o = new GameObject("EventSystem");

                    o.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    o.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }
        }

#if UNITY_EDITOR
        public int callbackOrder => 0;

        private void OnEnable()
        {
            EditorApplication.update += Update;
            BuildTargetEvents.Register(this);
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
            BuildTargetEvents.Unregister(this);
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Update();
        }

        private void Update()
        {
            CheckEnableControlRig();
        }
#endif

        private void CheckEnableControlRig()
        {
#if MOBILE_INPUT
            EnableControlRig(true);
#else
            EnableControlRig(false);
#endif
        }

        private void EnableControlRig(bool enabled)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(enabled);
            }
        }
    }

#if UNITY_EDITOR
    public static class BuildTargetEvents
    {
        private static readonly System.Collections.Generic.List<IActiveBuildTargetChanged> listeners = new System.Collections.Generic.List<IActiveBuildTargetChanged>();

        public static void Register(IActiveBuildTargetChanged listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public static void Unregister(IActiveBuildTargetChanged listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        static BuildTargetEvents()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            foreach (var listener in listeners)
            {
                listener.OnActiveBuildTargetChanged(EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.activeBuildTarget);
            }
        }
    }
#endif
}