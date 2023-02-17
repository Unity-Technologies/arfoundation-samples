using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This example shows how to check for AR support before the ARSession is enabled.
    /// For ARCore in particular, it is possible for a device to support ARCore but not
    /// have it installed. This example will detect this case and prompt the user to install ARCore.
    /// To test this feature yourself, use a supported device and uninstall ARCore.
    /// (Settings > Search for "ARCore" and uninstall or disable it.)
    /// </summary>
    public class SupportChecker : MonoBehaviour
    {
        [SerializeField]
        ARSession m_Session;

        public ARSession session
        {
            get { return m_Session; }
            set { m_Session = value; }
        }

        [SerializeField]
        Text m_LogText;

        public Text logText
        {
            get { return m_LogText; }
            set { m_LogText = value; }
        }

        [SerializeField]
        Button m_InstallButton;

        public Button installButton
        {
            get { return m_InstallButton; }
            set { m_InstallButton = value; }
        }

        void Log(string message)
        {
            m_LogText.text += $"{message}\n";
        }

        IEnumerator CheckSupport()
        {
            SetInstallButtonActive(false);

            Log("Checking for AR support...");

            yield return ARSession.CheckAvailability();

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                Log("Your device supports AR, but requires a software update.");
                Log("Attempting install...");
                yield return ARSession.Install();
            }

            if (ARSession.state == ARSessionState.Ready)
            {
                Log("Your device supports AR!");
                Log("Starting AR session...");

                // To start the ARSession, we just need to enable it.
                m_Session.enabled = true;
            }
            else
            {
                switch (ARSession.state)
                {
                    case ARSessionState.Unsupported:
                        Log("Your device does not support AR.");
                        break;
                    case ARSessionState.NeedsInstall:
                        Log("The software update failed, or you declined the update.");

                        // In this case, we enable a button which allows the user
                        // to try again in the event they decline the update the first time.
                        SetInstallButtonActive(true);
                        break;
                }

                Log("\n[Start non-AR experience instead]");

                //
                // Start a non-AR fallback experience here...
                //
            }
        }

        void SetInstallButtonActive(bool active)
        {
            if (m_InstallButton != null)
                m_InstallButton.gameObject.SetActive(active);
        }

        IEnumerator Install()
        {
            SetInstallButtonActive(false);

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                Log("Attempting install...");
                yield return ARSession.Install();

                if (ARSession.state == ARSessionState.NeedsInstall)
                {
                    Log("The software update failed, or you declined the update.");
                    SetInstallButtonActive(true);
                }
                else if (ARSession.state == ARSessionState.Ready)
                {
                    Log("Success! Starting AR session...");
                    m_Session.enabled = true;
                }
            }
            else
            {
                Log("Error: ARSession does not require install.");
            }
        }

        public void OnInstallButtonPressed()
        {
            StartCoroutine(Install());
        }

        void OnEnable()
        {
            StartCoroutine(CheckSupport());
        }
    }
}