using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
    /// and overlays some information as well as the source Texture2D on top of the
    /// detected image.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class MultiTrackedImageInfoManager : MonoBehaviour
    {

        [Serializable]
        /// <summary>
        /// It has an image guid and a GameObject for prefab.
        /// </summary>
        public struct NamedPrefab
        {
            Guid m_ImageGuid;

            public Guid imageGuid
            {
                get => m_ImageGuid; 
            }

            [SerializeField]
            GameObject m_Prefab;

            public GameObject prefab
            {
                get => m_Prefab; 
                set => m_Prefab = value;
            }

            public NamedPrefab(Guid imageGuid, GameObject prefab)
            {
                m_ImageGuid = imageGuid;
                m_Prefab = prefab;
            }
        }

        [SerializeField]
        [Tooltip("Each prefab corresponds to each image in the Image Library.")]
        List<NamedPrefab> m_PrefabList;

        /// <summary>
        /// Each prefab corresponds to the each image in the Image Library in the same order.
        /// </summary>
        public List<NamedPrefab> prefabList
        {
            get => m_PrefabList;
            set => m_PrefabList = value;
        }

        Dictionary<Guid, GameObject> m_PrefabsDictionary = new Dictionary<Guid, GameObject>();
        ARTrackedImageManager m_TrackedImageManager;
        
        [SerializeField]
        [Tooltip("Reference Image Library")]
        XRReferenceImageLibrary m_ImageLibrary;

        /// <summary>
        /// Get the <c>XRReferenceImageLibrary</c>
        /// </summary>
        public XRReferenceImageLibrary ImageLibrary
        {
            get => m_ImageLibrary;
            set => m_ImageLibrary = value;
        }

        void Awake()
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
            
            for (int i = 0; i < prefabList.Count; i++)
            {                
                m_PrefabsDictionary.Add(ImageLibrary[i].guid, prefabList[i].prefab);
            }
        }

        void OnEnable()
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable()
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        internal void OnLibraryChanged(XRReferenceImageLibrary library)
        {
            UpdatePrefabList();
        }

        void UpdatePrefabList()
        {
            if (m_ImageLibrary != null)
            {
                if (m_PrefabList == null)
                {
                    m_PrefabList = new List<NamedPrefab>();
                    for (int i = 0; i < m_ImageLibrary.count; i++)
                    {
                        m_PrefabList.Add(new NamedPrefab(m_ImageLibrary[i].guid, null));
                    }
                }
                else
                {
                    var tempList = new List<NamedPrefab>();
                    for (int i = 0; i < m_ImageLibrary.count; i++)
                    {
                        var idx = m_PrefabList.FindIndex(item => item.imageGuid == m_ImageLibrary[i].guid);
                        tempList.Add(new NamedPrefab(m_ImageLibrary[i].guid, (idx != -1) ? m_PrefabList[idx].prefab : null));
                    }
                    m_PrefabList = tempList;
                }
            }  
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // Give the initial image a reasonable default scale
                trackedImage.transform.localScale = new Vector3(trackedImage.size.x/2, trackedImage.size.x/2, trackedImage.size.x/2);

                AssignPrefab(trackedImage);
            }
        }

        void AssignPrefab(ARTrackedImage trackedImage)
        {
            if (m_PrefabsDictionary.TryGetValue(trackedImage.referenceImage.guid, out GameObject prefab)){
                Instantiate(prefab, trackedImage.transform);
            }
        }

        #if UNITY_EDITOR

        [CustomEditor(typeof(MultiTrackedImageInfoManager))]
        public class MultiTrackedImageInfoManagerInspector : Editor 
        {
            List<XRReferenceImage> m_ReferenceImages = new List<XRReferenceImage>();

            bool HasLibraryChanged(XRReferenceImageLibrary library)
            {
                if (library == null)
                    return m_ReferenceImages.Count == 0;

                if (m_ReferenceImages.Count != library.count)
                    return true;

                for (int i = 0; i < library.count; i++)
                {
                    if (m_ReferenceImages[i] != library[i])
                        return true;
                }

                return false;
            }
            
            public override void OnInspectorGUI () 
            {
                var behaviour = serializedObject.targetObject as MultiTrackedImageInfoManager;
                var library = serializedObject.FindProperty("m_ImageLibrary").objectReferenceValue as XRReferenceImageLibrary;

                serializedObject.Update();
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                GUI.enabled = true;
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ImageLibrary"));
                ShowPrefabList(serializedObject.FindProperty("m_PrefabList"), library);
                serializedObject.ApplyModifiedProperties();

                EditorGUI.BeginChangeCheck();

                if (EditorGUI.EndChangeCheck())
                    behaviour.OnLibraryChanged(library);
                else if (HasLibraryChanged(library))
                    behaviour.OnLibraryChanged(library);

                // Update current
                m_ReferenceImages.Clear();
                if (library)
                {
                    foreach (var referenceImage in library)
                    {
                        m_ReferenceImages.Add(referenceImage);
                    }
                }
	        }

            public static void ShowPrefabList (SerializedProperty list, XRReferenceImageLibrary library) 
            {
                EditorGUILayout.PropertyField(list, false);
                EditorGUI.indentLevel += 1;
                if (list.isExpanded)
                {
                    for (int i = 0; i < list.arraySize; i++) {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("m_Prefab"), new GUIContent(library[i].name));
                    }
                }
                EditorGUI.indentLevel -= 1;
	        }
        }
        #endif
    }
}
