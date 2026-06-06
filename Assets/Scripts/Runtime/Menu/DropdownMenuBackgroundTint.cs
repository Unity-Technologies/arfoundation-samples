using System;
using System.Threading;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DropdownMenuBackgroundTint : MonoBehaviour
    {
        [SerializeField]
        CustomTMPDropdown m_Dropdown;

        [SerializeField]
        Image m_BackgroundTint;

        CancellationTokenSource m_Cts;

        void Start()
        {
            m_BackgroundTint.CrossFadeAlpha(0f, 0f, true);
        }

        void OnEnable()
        {
            m_Dropdown.dropdownOpened += OnDropdownOpened;
            m_Dropdown.dropdownClosed += OnDropdownClosed;
        }

        void OnDisable()
        {
            m_Dropdown.dropdownOpened -= OnDropdownOpened;
            m_Dropdown.dropdownClosed -= OnDropdownClosed;
            Cancel();
        }

        void OnDropdownOpened()
        {
            Cancel();
            m_BackgroundTint.gameObject.SetActive(true);
            m_BackgroundTint.CrossFadeAlpha(1f, m_Dropdown.alphaFadeSpeed, true);
        }

        async void OnDropdownClosed()
        {

            try
            {
                Cancel();
                m_Cts = new();
                m_BackgroundTint.CrossFadeAlpha(0f, m_Dropdown.alphaFadeSpeed, true);

                await Awaitable.WaitForSecondsAsync(m_Dropdown.alphaFadeSpeed, m_Cts.Token);

                if (this != null && gameObject.activeInHierarchy)
                    m_BackgroundTint.gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
                // Gracefully exit
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        void Cancel()
        {
            if (m_Cts == null)
                return;

            m_Cts.Cancel();
            m_Cts.Dispose();
            m_Cts = null;
        }
    }
}
