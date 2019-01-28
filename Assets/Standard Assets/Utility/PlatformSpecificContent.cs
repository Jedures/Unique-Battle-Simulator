using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR

#endif

namespace Standard_Assets.Utility
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
#endif
    public class PlatformSpecificContent : MonoBehaviour
#if UNITY_EDITOR
        , UnityEditor.Build.IActiveBuildTargetChanged
#endif
    {
        private enum BuildTargetGroup
        {
            Standalone,
            Mobile
        }

        [FormerlySerializedAs("m_BuildTargetGroup")] [SerializeField]
        private BuildTargetGroup buildTargetGroup;
        [FormerlySerializedAs("m_Content")] [SerializeField]
        private GameObject[] content = new GameObject[0];
        [FormerlySerializedAs("m_MonoBehaviours")] [SerializeField]
        private MonoBehaviour[] monoBehaviours = new MonoBehaviour[0];
        [FormerlySerializedAs("m_ChildrenOfThisObject")] [SerializeField]
        private bool childrenOfThisObject;

#if !UNITY_EDITOR
	void OnEnable()
	{
		CheckEnableContent();
	}
#else
        public int callbackOrder => 1;
#endif

#if UNITY_EDITOR

        private void OnEnable()
        {
            EditorApplication.update += Update;
        }


        private void OnDisable()
        {
            // ReSharper disable once DelegateSubtraction
            if (EditorApplication.update != null) EditorApplication.update -= Update;
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            CheckEnableContent();
        }

        private void Update()
        {
            CheckEnableContent();
        }
#endif


        private void CheckEnableContent()
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_TIZEN)
            EnableContent(buildTargetGroup == BuildTargetGroup.Mobile);
#endif

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_TIZEN)
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
            if (content.Length > 0)
            {
                foreach (var g in content)
                {
                    if (g != null)
                    {
                        g.SetActive(enabled);
                    }
                }
            }
            if (childrenOfThisObject)
            {
                foreach (Transform t in transform)
                {
                    t.gameObject.SetActive(enabled);
                }
            }
            if (monoBehaviours.Length > 0)
            {
                foreach (var monoBehaviour in monoBehaviours)
                {
                    monoBehaviour.enabled = enabled;
                }
            }
        }
    }
}