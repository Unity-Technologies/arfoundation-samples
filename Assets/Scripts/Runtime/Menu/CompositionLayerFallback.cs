using System;
using System.Threading;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Services;
using Unity.XR.CompositionLayers.UIInteraction;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class CompositionLayerFallback : MonoBehaviour
    {
        const int k_UILayer = 5;
        const int k_CompLayer = 10;

        [Header("XR Components to Disable")]
        [SerializeField]
        CompositionLayer m_CompositionLayerComponent;

        [SerializeField]
        InteractableUIMirror m_InteractableUIMirrorComponent;

        [Header("UI Hierarchy")]
        [SerializeField]
        Canvas m_CanvasRoot;

        [SerializeField]
        float m_FallbackScale = 1.0f;

        [SerializeField]
        bool m_ForceFallbackOnStart;

        float m_CanvasCompLayerScale;
        CancellationTokenSource m_DestroyCancellationTokenSource = new();

        async void Start()
        {
            try
            {
                // We must await 2 frame to let InteractableUIMirror and the comp layer system to finish
                // setting up.
                await Awaitable.NextFrameAsync(m_DestroyCancellationTokenSource.Token);
                await Awaitable.NextFrameAsync(m_DestroyCancellationTokenSource.Token);

                m_CanvasCompLayerScale =  m_CanvasRoot.transform.localScale.x;
                var shouldFallback = CompositionLayerManager.Instance?.LayerProvider == null;
                if (shouldFallback || m_ForceFallbackOnStart)
                    ToggleCompLayers(false);
            }
            catch (OperationCanceledException)
            {
                // Exit gracefully
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void OnDestroy()
        {
            m_DestroyCancellationTokenSource.Cancel();
            m_DestroyCancellationTokenSource.Dispose();
        }

        public void ToggleCompLayers(bool isOn)
        {
            if (m_CompositionLayerComponent != null)
            {
                m_CompositionLayerComponent.enabled = isOn;

                if (m_CompositionLayerComponent.TryGetComponent<Collider>(out var compCollider))
                    compCollider.enabled = isOn;
            }

            if (m_InteractableUIMirrorComponent != null)
                m_InteractableUIMirrorComponent.enabled = isOn;

            if (m_CanvasRoot != null)
            {
                var layer = isOn ? k_CompLayer : k_UILayer;
                SetLayersRecursively(m_CanvasRoot.gameObject, layer);

                var scaleFactor = isOn ? m_CanvasCompLayerScale : m_FallbackScale;
                m_CanvasRoot.transform.localScale = Vector3.one * scaleFactor;

                var canvasGroup = m_CanvasRoot.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                    canvasGroup.blocksRaycasts = !isOn;

                var wasActive = m_CanvasRoot.gameObject.activeSelf;
                m_CanvasRoot.gameObject.SetActive(false);
                if (wasActive)
                    m_CanvasRoot.gameObject.SetActive(true);
            }
        }

        static void SetLayersRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayersRecursively(child.gameObject, newLayer);
            }
        }
    }
}
