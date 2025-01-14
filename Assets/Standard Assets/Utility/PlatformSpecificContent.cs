using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace UnityStandardAssets.Utility
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class PlatformSpecificContent : MonoBehaviour
#if UNITY_EDITOR
        , IActiveBuildTargetChanged
#endif
    {
        private enum BuildTargetGroup
        {
            Standalone,
            Mobile
        }

        [SerializeField] private BuildTargetGroup m_BuildTargetGroup;
        [SerializeField] private GameObject[] m_Content = new GameObject[0];
        [SerializeField] private MonoBehaviour[] m_MonoBehaviours = new MonoBehaviour[0];
        [SerializeField] private bool m_ChildrenOfThisObject;

#if !UNITY_EDITOR
        void OnEnable()
        {
            CheckEnableContent();
        }
#endif

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
            CheckEnableContent();
        }
#endif

        private void CheckEnableContent()
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
            if (m_BuildTargetGroup == BuildTargetGroup.Mobile)
            {
                EnableContent(true);
            }
            else
            {
                EnableContent(false);
            }
#endif

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
            if (m_BuildTargetGroup == BuildTargetGroup.Mobile)
            {
                EnableContent(false);
            }
            else
            {
                EnableContent(true);
            }
#endif
        }

        private void EnableContent(bool enabled)
        {
            if (m_Content.Length > 0)
            {
                foreach (var g in m_Content)
                {
                    if (g != null)
                    {
                        g.SetActive(enabled);
                    }
                }
            }
            if (m_ChildrenOfThisObject)
            {
                foreach (Transform t in transform)
                {
                    t.gameObject.SetActive(enabled);
                }
            }
            if (m_MonoBehaviours.Length > 0)
            {
                foreach (var monoBehaviour in m_MonoBehaviours)
                {
                    monoBehaviour.enabled = enabled;
                }
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