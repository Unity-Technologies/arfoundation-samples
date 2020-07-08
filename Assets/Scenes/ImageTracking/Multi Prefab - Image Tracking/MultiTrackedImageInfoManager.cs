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
        [SerializeField]
        [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
        Camera m_WorldSpaceCanvasCamera;

        /// <summary>
        /// The prefab has a world space UI canvas,
        /// which requires a camera to function properly.
        /// </summary>
        public Camera worldSpaceCanvasCamera
        {
            get { return m_WorldSpaceCanvasCamera; }
            set { m_WorldSpaceCanvasCamera = value; }
        }

        [SerializeField]
        [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
        Texture2D m_DefaultTexture;

        /// <summary>
        /// If an image is detected but no source texture can be found,
        /// this texture is used instead.
        /// </summary>
        public Texture2D defaultTexture
        {
            get { return m_DefaultTexture; }
            set { m_DefaultTexture = value; }
        }

        [Serializable]
        /// <summary>
        /// It has a string for name and a GameObject for prefab.
        /// </summary>
        public struct NamedPrefab
        {
            [SerializeField]
            string m_PrefabName;
            [SerializeField]
            GameObject m_Prefab;
            
            public string prefabName
            {
                get { return m_PrefabName; }
                set { m_PrefabName = value; }
            }

            public GameObject prefab
            {
                get { return m_Prefab; }
                set { m_Prefab = value; }
            }

            public NamedPrefab(string prefabName, GameObject prefab)
            {
                m_PrefabName = prefabName;
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
            get { return m_PrefabList; }
            set { m_PrefabList = value; }
        }

        Dictionary<string, GameObject> m_PrefabsDictionary = new Dictionary<string, GameObject>();
        ARTrackedImageManager m_TrackedImageManager;
        
        [SerializeField, ReferenceImageLibraryChanged]
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
                m_PrefabsDictionary.Add(prefabList[i].prefabName, prefabList[i].prefab);
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
            Debug.Log("OnLibraryChanged");
            UpdatePrefabList();
        }

        void UpdatePrefabList()
        {
            if (m_PrefabList.Count == m_ImageLibrary.count)
            {
                for (int i = 0; i < m_PrefabList.Count; i++)
                {
                    var pref = m_PrefabList[i];
                    pref.prefabName = m_ImageLibrary[i].name;
                    m_PrefabList[i] = pref;
                }
            }
            else
            {
                m_PrefabList = new List<NamedPrefab>();

                for (int i = 0; i < m_ImageLibrary.count; i++)
                {
                    NamedPrefab pref = new NamedPrefab(m_ImageLibrary[i].name, null);
                    m_PrefabList.Add(pref);
                }
            }
        }

        void AssignPrefab(ARTrackedImage trackedImage)
        {
            GameObject prefab;
            if (m_PrefabsDictionary.TryGetValue(trackedImage.referenceImage.name, out prefab)){
                Instantiate(prefab, trackedImage.transform);
            }
        }

        void UpdateInfo(ARTrackedImage trackedImage)
        {
            switch(trackedImage.referenceImage.name)
            {

                case "Rafflesia":

                    // Disable the visual plane if it is not being tracked
                    if (trackedImage.trackingState != TrackingState.None)
                    {
                        // The image extents is only valid when the image is being tracked
                        trackedImage.transform.localScale = new Vector3(trackedImage.size.x/10, trackedImage.size.x/10, trackedImage.size.x/10);
                    }
                    break;

                //  case "Logo":

                case "QRCode":

                    // Set canvas camera
                    var canvas = trackedImage.GetComponentInChildren<Canvas>();
                    canvas.worldCamera = worldSpaceCanvasCamera;

                    // Update information about the tracked image
                    var text = canvas.GetComponentInChildren<Text>();
                    text.text = string.Format(
                        "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3} cm\nDetected size: {4} cm",
                        trackedImage.referenceImage.name,
                        trackedImage.trackingState,
                        trackedImage.referenceImage.guid,
                        trackedImage.referenceImage.size * 100f,
                        trackedImage.size * 100f);

                    var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
                    var planeGo = planeParentGo.transform.GetChild(0).gameObject;

                    // Disable the visual plane if it is not being tracked
                    if (trackedImage.trackingState != TrackingState.None)
                    {
                        planeGo.SetActive(true);

                        // The image extents is only valid when the image is being tracked
                        trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);

                        // Set the texture
                        var material = planeGo.GetComponentInChildren<MeshRenderer>().material;
                        material.mainTexture = (trackedImage.referenceImage.texture == null) ? defaultTexture : trackedImage.referenceImage.texture;
                    }
                    else
                    {
                        planeGo.SetActive(false);
                    }

                    break;
            }
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // Give the initial image a reasonable default scale
                trackedImage.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

                AssignPrefab(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
                UpdateInfo(trackedImage);

        }

        public class ReferenceImageLibraryChangedAttribute : PropertyAttribute { }

        #if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(ReferenceImageLibraryChangedAttribute))]
        public class ReferenceImageLibraryChangedAttributePropertyDrawer : PropertyDrawer
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

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var behaviour = property.serializedObject.targetObject as MultiTrackedImageInfoManager;
                var library = property.objectReferenceValue as XRReferenceImageLibrary;

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(position, property);
                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log($"Library field changed");
                    behaviour.OnLibraryChanged(library);
                }
                else if (HasLibraryChanged(library))
                {
                    Debug.Log("Library changed (items added or removed)");
                    behaviour.OnLibraryChanged(library);
                }

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
        }

        #endif
    }
}