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
    public class MultiTrackedImageInfoManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Serializable]
        /// <summary>
        /// Used to associate an `XRReferenceImage` with a Prefab by using the `XRReferenceImage`'s guid as a unique identifier for a particular reference image.
        /// </summary>
        struct NamedPrefab
        {
            // System.Guid isn't serializable, so we store the Guid as a string. At runtime, this is converted back to a System.Guid
            [SerializeField]
            public readonly string m_ImageGuid;

            [SerializeField]
            public GameObject m_Prefab;

            public NamedPrefab(XRReferenceImage image, GameObject prefab)
            {
                m_ImageGuid = image.guid.ToString();
                m_Prefab = prefab;
            }

            public NamedPrefab(Guid guid, GameObject prefab)
            {
                m_ImageGuid = guid.ToString();
                m_Prefab = prefab;
            }
        }

        [SerializeField]
        [Tooltip("Each prefab corresponds to each image in the Image Library.")]
        List<NamedPrefab> m_PrefabsList = new List<NamedPrefab>();

        Dictionary<Guid, GameObject> m_PrefabsDictionary = new Dictionary<Guid, GameObject>();
        Dictionary<Guid, GameObject> m_InstantiatedPrefabsDictionary = new Dictionary<Guid, GameObject>();
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

        public void OnBeforeSerialize()
        {
            m_PrefabsList.Clear();
            foreach (var kvp in m_PrefabsDictionary)
            {
                m_PrefabsList.Add(new NamedPrefab(kvp.Key, kvp.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            if (ImageLibrary.count != 0)
            {
                m_PrefabsDictionary = new Dictionary<Guid, GameObject>();
                for (int i = 0; i < m_PrefabsList.Count; i++)
                {
                    m_PrefabsDictionary.Add(ImageLibrary[i].guid, m_PrefabsList[i].m_Prefab);
                }
            }
        }

        void Awake()
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
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
                var tempDictionary = new Dictionary<Guid, GameObject>();
                foreach (var referenceImage in library)
                {
                    tempDictionary.Add(referenceImage.guid, (m_PrefabsDictionary.TryGetValue(referenceImage.guid, out GameObject prefab)) ? prefab : null);
                }
                m_PrefabsDictionary = tempDictionary;
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

            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking)
                    RemoveInstantiatedPrefab(trackedImage);
                else if (trackedImage.trackingState == TrackingState.Tracking)
                    AssignPrefab(trackedImage);
            }
        }

        void AssignPrefab(ARTrackedImage trackedImage)
        {
            if (m_PrefabsDictionary.TryGetValue(trackedImage.referenceImage.guid, out GameObject prefab))
            {
                if (!m_InstantiatedPrefabsDictionary.ContainsKey(trackedImage.referenceImage.guid))
                {
                    var instantiatedPrefab = Instantiate(prefab, trackedImage.transform);
                    m_InstantiatedPrefabsDictionary.Add(trackedImage.referenceImage.guid, instantiatedPrefab);
                }
            }
        }

        void RemoveInstantiatedPrefab(ARTrackedImage trackedImage)
        {
            if (m_InstantiatedPrefabsDictionary.TryGetValue(trackedImage.referenceImage.guid, out GameObject instantiatedPrefab))
            {
                Destroy(instantiatedPrefab);
                m_InstantiatedPrefabsDictionary.Remove(trackedImage.referenceImage.guid);
            }
        }

        void RemoveInstantiatedPrefab(XRReferenceImage referenceImage)
        {
            if (m_InstantiatedPrefabsDictionary.TryGetValue(referenceImage.guid, out GameObject instantiatedPrefab))
            {
                Destroy(instantiatedPrefab);
                m_InstantiatedPrefabsDictionary.Remove(referenceImage.guid);
            }
        }

        public GameObject GetPrefabForReferenceImage(XRReferenceImage referenceImage)
            => m_PrefabsDictionary.TryGetValue(referenceImage.guid, out var prefab) ? prefab : null;

        public void SetPrefabForReferenceImage(XRReferenceImage referenceImage, GameObject alternativePrefab)
        {
            if (m_PrefabsDictionary.TryGetValue(referenceImage.guid, out GameObject targetPrefabInDictionary))
            {
                m_PrefabsDictionary[referenceImage.guid] = alternativePrefab;
                RemoveInstantiatedPrefab(referenceImage);
            }
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(MultiTrackedImageInfoManager))]
        class MultiTrackedImageInfoManagerInspector : Editor 
        {
            List<XRReferenceImage> m_ReferenceImages = new List<XRReferenceImage>();
            bool isExpanded = true;

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

                serializedObject.Update();
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                }
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ImageLibrary"));

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

                //show prefab list
                isExpanded = EditorGUILayout.Foldout(isExpanded, "Prefab List");
                if (isExpanded)
                {
                    EditorGUI.indentLevel += 1;
                    foreach (var image in library)
                    {
                        if (behaviour.m_PrefabsDictionary.TryGetValue(image.guid, out GameObject prefab))
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(image.name, GUILayout.Width(200f));
                            EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }

                serializedObject.ApplyModifiedProperties();
	        }
        }
#endif
    }
}
