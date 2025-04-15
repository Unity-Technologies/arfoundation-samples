using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This class handles shutting down the network session when leaving the scene.
    /// </summary>
    public class NetworkSessionController : MonoBehaviour
    {
        Scene m_MetaSharedAnchorsScene;

        void Awake()
        {
            m_MetaSharedAnchorsScene = SceneManager.GetActiveScene();
        }

        void Start()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDestroy()
        {
            ShutdownNetworkSession();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void OnSceneUnloaded(Scene scene)
        {
            if (scene != m_MetaSharedAnchorsScene)
                return;

            ShutdownNetworkSession();
        }

        void ShutdownNetworkSession()
        {
            if (NetworkManager.Singleton == null ||
                !NetworkManager.Singleton.IsConnectedClient ||
                NetworkManager.Singleton.ShutdownInProgress)
                return;

            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
