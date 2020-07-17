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
    /// <summary>
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
            [SerializeField]
            string m_ImageGuid;

            public string imageGuid => m_ImageGuid;

            [SerializeField]
            GameObject m_Prefab;

            public GameObject prefab
            {
                get => m_Prefab;
                set => m_Prefab = value;
            }

            public NamedPrefab(XRReferenceImage image, GameObject prefab)
            {
                m_ImageGuid = image.guid.ToString();
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

        void OnLibraryChanged(XRReferenceImageLibrary library)
        {
            if (library)
            {
                if (prefabList == null)
                {
                    prefabList = new List<NamedPrefab>();
                    for (int i = 0; i < library.count; i++)
                    {
                        prefabList.Add(new NamedPrefab(library[i], null));
                    }
                }
                else
                {
                    var tempList = new List<NamedPrefab>();
                    for (int i = 0; i < library.count; i++)
                    {
                        var listIndex = prefabList.FindIndex(item => item.imageGuid == library[i].guid.ToString());
                        tempList.Add(new NamedPrefab(library[i], (listIndex != -1) ? prefabList[listIndex].prefab : null));
                    }
                    prefabList = tempList;
                }
            }
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // Give the initial image a reasonable default scale
                trackedImage.transform.localScale = new Vector3(trackedImage.size.x / 2, trackedImage.size.x / 2, trackedImage.size.x / 2);

                AssignPrefab(trackedImage);
            }
        }

        void AssignPrefab(ARTrackedImage trackedImage)
        {
            if (m_PrefabsDictionary.TryGetValue(trackedImage.referenceImage.guid, out GameObject prefab))
            {
                Instantiate(prefab, trackedImage.transform);
            }
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(MultiTrackedImageInfoManager))]
        class MultiTrackedImageInfoManagerInspector : Editor 
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
                //customized inspector
                var behaviour = serializedObject.targetObject as MultiTrackedImageInfoManager;
                var library = serializedObject.FindProperty("m_ImageLibrary").objectReferenceValue as XRReferenceImageLibrary;
                var list = serializedObject.FindProperty("m_PrefabList");

                serializedObject.Update();
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                GUI.enabled = true;
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ImageLibrary"));

                //show prefab list
                EditorGUILayout.PropertyField(list, false);
                EditorGUI.indentLevel += 1;
                if (list.isExpanded)
                {
                    for (int i = 0; i < list.arraySize; i++) {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i).FindPropertyRelative("m_Prefab"), new GUIContent(library[i].name));
                    }
                }
                EditorGUI.indentLevel -= 1;

                //check library changes
                EditorGUI.BeginChangeCheck();

                if (EditorGUI.EndChangeCheck())
                    behaviour.OnLibraryChanged(library);
                else if (HasLibraryChanged(library))
                    behaviour.OnLibraryChanged(library);     

                // update current
                m_ReferenceImages.Clear();
                if (library)
                {
                    foreach (var referenceImage in library)
                    {
                        m_ReferenceImages.Add(referenceImage);
                    }
                }

                serializedObject.ApplyModifiedProperties();
	        }
        }
#endif
    }
}
