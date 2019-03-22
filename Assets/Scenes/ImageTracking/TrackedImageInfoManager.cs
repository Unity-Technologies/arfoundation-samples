using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// Each XRReferenceImage only stores a Guid, not a Texture2D.
/// At build time, generate a list of source Texture2Ds and store references
/// so that we will have them at runtime.
/// </summary>
class TrackedImageInfoManagerBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        var infoManagers = UnityEngine.Object.FindObjectsOfType<TrackedImageInfoManager>();
        if (infoManagers == null)
            return;

        foreach (var infoManager in infoManagers)
            infoManager.RebuildDictionary();

        AssetDatabase.Refresh();
    }
}
#endif

/// <summary>
/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
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

    /// <summary>
    /// A serializable container for Texture2D and XRReferenceImage pairs.
    /// This is used to associate a reference image with the source texture.
    /// </summary>
    [Serializable]
    struct TextureReferenceImagePair
    {
        [SerializeField]
        public Texture2D texture;

        [SerializeField]
        public XRReferenceImage referenceImage;

        public TextureReferenceImagePair(Texture2D texture, XRReferenceImage referenceImage)
        {
            this.texture = texture;
            this.referenceImage = referenceImage;
        }
    }

    /// <summary>
    /// A serializable list of Texture2D-ReferenceImage pairs used to lookup
    /// a reference image's source Texture2D at runtime.
    /// </summary>
    [SerializeField, HideInInspector]
    List<TextureReferenceImagePair> m_TextureReferenceImagePairs = new List<TextureReferenceImagePair>();

    ARTrackedImageManager m_TrackedImageManager;

    Dictionary<Guid, Texture2D> m_Textures;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        // Build a dictionary of Guid to Texture2D
        m_Textures = new Dictionary<Guid, Texture2D>();
        foreach (var pair in m_TextureReferenceImagePairs)
            m_Textures[pair.referenceImage.guid] = pair.texture;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Rebuilds a serializable list of Texture2D-XRReferenceImage pairs.
    /// At runtime, this List is used to populate a dictionary.
    /// </summary>
    internal void RebuildDictionary()
    {
        m_TextureReferenceImagePairs = new List<TextureReferenceImagePair>();
        var trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (trackedImageManager != null && trackedImageManager.referenceLibrary != null)
        {
            foreach (var referenceImage in trackedImageManager.referenceLibrary)
            {
                var guid = referenceImage.guid;
                var texturePath = AssetDatabase.GUIDToAssetPath(guid.ToString("N"));
                if (string.IsNullOrEmpty(texturePath))
                {
                    Debug.LogWarningFormat("Null or empty texturePath for image {0}", guid);
                    continue;
                }

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                m_TextureReferenceImagePairs.Add(new TextureReferenceImagePair(texture, referenceImage));
            }
        }
    }
#endif

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        // Update information about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
            "trackableId\n{0}\ntrackingState: {1}\nGUID: {2}\nReference width: {3}\nDetected size: {4:0.0000}x{5:0.0000}",
            trackedImage.trackableId,
            trackedImage.trackingState,
            trackedImage.referenceImage.guid,
            trackedImage.referenceImage.width,
            trackedImage.size.x,
            trackedImage.size.y);

        var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
        var planeGo = planeParentGo.transform.GetChild(0).gameObject;

        // Disable the visual plane if it is not being tracked
        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGo.SetActive(true);

            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);

            // Set the texture
            var meshRenderer = planeGo.GetComponentInChildren<MeshRenderer>();

            // Look up the texture by Guid
            Texture2D texture;
            if (!m_Textures.TryGetValue(trackedImage.referenceImage.guid, out texture))
                texture = defaultTexture;

            meshRenderer.material.mainTexture = texture;
        }
        else
        {
            planeGo.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
            UpdateInfo(trackedImage);
    }

    void Update()
    {
#if UNITY_IOS && !UNITY_EDITOR
        var subsystem = m_TrackedImageManager.subsystem as ARKitImageTrackingSubsystem;
        if (subsystem != null)
        {
            subsystem.maximumNumberOfTrackedImages = m_TrackedImageManager.referenceLibrary.count;
        }
#endif
    }
}
