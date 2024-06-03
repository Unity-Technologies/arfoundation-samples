using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ARPlaceAnchor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The enabled Anchor Manager in the scene.")]
        ARAnchorManager m_AnchorManager;

        [SerializeField]
        [Tooltip("The Scriptable Object Asset that contains the ARRaycastHit event.")]
        ARRaycastHitEventAsset m_RaycastHitEvent;

        Dictionary<TrackableId, ARAnchor> m_AnchorsByTrackableId = new();

        public ARAnchorManager anchorManager
        {
            get => m_AnchorManager;
            set => m_AnchorManager = value;
        }

        public void RemoveAllAnchors()
        {
            foreach (var anchor in m_AnchorsByTrackableId.Values)
            {
                m_AnchorManager.TryRemoveAnchor(anchor);
            }
            m_AnchorsByTrackableId.Clear();
        }

        // Runs when the reset option is called in the context menu in-editor, or when first created.
        void Reset()
        {
            m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();
        }

        void OnEnable()
        {
            if (m_AnchorManager == null)
                m_AnchorManager = FindAnyObjectByType<ARAnchorManager>();

            if ((m_AnchorManager ? m_AnchorManager.subsystem : null) == null)
            {
                enabled = false;
                Debug.LogWarning($"No XRAnchorSubsystem was found in {nameof(ARPlaceAnchor)}'s {nameof(m_AnchorManager)}, so this script will be disabled.", this);
                return;
            }

            if (m_RaycastHitEvent == null)
            {
                enabled = false;
                Debug.LogWarning($"{nameof(m_RaycastHitEvent)} field on {nameof(ARPlaceAnchor)} component of {name} is not assigned.", this);
                return;
            }

            m_RaycastHitEvent.eventRaised += CreateAnchor;
            m_AnchorManager.trackablesChanged.AddListener(OnAnchorsChanged);
        }

        void OnAnchorsChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs)
        {
            // add any anchors that have been added outside our control, such as loading from storage
            foreach (var addedAnchor in eventArgs.added)
            {
                m_AnchorsByTrackableId.Add(addedAnchor.trackableId, addedAnchor);
            }

            // remove any anchors that have been removed outside our control, such as during a session reset
            foreach (var removedAnchor in eventArgs.removed)
            {
                if (removedAnchor.Value != null)
                {
                    m_AnchorsByTrackableId.Remove(removedAnchor.Key);
                    Destroy(removedAnchor.Value.gameObject);
                }
            }
        }

        void OnDisable()
        {
            if (m_RaycastHitEvent != null)
                m_RaycastHitEvent.eventRaised -= CreateAnchor;

            if (m_AnchorManager != null)
                m_AnchorManager.trackablesChanged.AddListener(OnAnchorsChanged);
        }

        /// <summary>
        /// Attempts to attach a new anchor to a hit `ARPlane` if supported.
        /// Otherwise, asynchronously creates a new anchor.
        /// </summary>
        void CreateAnchor(object sender, ARRaycastHit hit)
        {
            if (m_AnchorManager.descriptor.supportsTrackableAttachments && hit.trackable is ARPlane plane)
            {
                AttachAnchorToTrackable(plane, hit);
            }
            else
            {
                CreateAnchorAsync(hit);
            }
        }

        void AttachAnchorToTrackable(ARPlane plane, ARRaycastHit hit)
        {
            var anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
            var arAnchorDebugVisualizer = anchor.GetComponent<ARAnchorDebugVisualizer>();
            if (arAnchorDebugVisualizer != null)
            {
                arAnchorDebugVisualizer.IsAnchorAttachedToTrackable = true;
                arAnchorDebugVisualizer.SetAnchorCreationMethod(true, hit.hitType);
            }
        }

        async void CreateAnchorAsync(ARRaycastHit hit)
        {
            var result = await m_AnchorManager.TryAddAnchorAsync(hit.pose);
            if (result.status.IsSuccess())
            {
                var anchor = result.value;
                var arAnchorDebugVisualizer = anchor.GetComponent<ARAnchorDebugVisualizer>();
                if (arAnchorDebugVisualizer != null)
                    arAnchorDebugVisualizer.SetAnchorCreationMethod(true, hit.hitType);
            }
        }
    }
}
