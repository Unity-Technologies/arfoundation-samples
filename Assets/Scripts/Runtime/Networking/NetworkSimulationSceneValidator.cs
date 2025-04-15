using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Handles
    /// <a href="https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/using-networkscenemanager/#scene-validation">Scene Validation</a>
    /// for the simulation scene that loads at runtime when using the xr environment simulation in the editor.
    /// It prevents the server/host from synchronizing that scene as there is no network code that lives in that scene.
    /// </summary>
    public class NetworkSimulationSceneValidator : NetworkBehaviour
    {
#if UNITY_EDITOR
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.SceneManager.DisableValidationWarnings(true);
                NetworkManager.SceneManager.VerifySceneBeforeLoading = ServerSideSceneValidation;
            }
        }

        bool ServerSideSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
        {
            // The XR Simulation scene that gets added at runtime defines the name
            // of the scene in UnityEngine.XR.Simulation.BaseSimulationSceneManager.
            return !sceneName.Contains("Simulated Environment Scene");
        }
#endif // UNITY_EDITOR
    }
}
