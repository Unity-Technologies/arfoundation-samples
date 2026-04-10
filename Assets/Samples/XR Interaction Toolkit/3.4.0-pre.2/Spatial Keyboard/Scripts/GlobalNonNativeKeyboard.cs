#if TEXT_MESH_PRO_PRESENT || (UGUI_2_0_PRESENT && UNITY_6000_0_OR_NEWER)
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// Manages spawning and positioning of the global keyboard.
    /// </summary>
    public class GlobalNonNativeKeyboard : MonoBehaviour
    {
        public static GlobalNonNativeKeyboard instance { get; private set; }

        [SerializeField, Tooltip("The prefab with the XR Keyboard component to automatically instantiate.")]
        GameObject m_KeyboardPrefab;

        /// <summary>
        /// The prefab with the XR Keyboard component to automatically instantiate.
        /// </summary>
        public GameObject keyboardPrefab
        {
            get => m_KeyboardPrefab;
            set => m_KeyboardPrefab = value;
        }

        [SerializeField, Tooltip("The parent Transform to instantiate the Keyboard Prefab under.")]
        Transform m_PlayerRoot;

        /// <summary>
        /// The parent Transform to instantiate the Keyboard Prefab under.
        /// </summary>
        public Transform playerRoot
        {
            get => m_PlayerRoot;
            set => m_PlayerRoot = value;
        }

        [HideInInspector]
        [SerializeField]
        XRKeyboard m_Keyboard;

        /// <summary>
        /// Global keyboard instance.
        /// </summary>
        public XRKeyboard keyboard
        {
            get => m_Keyboard;
            set => m_Keyboard = value;
        }

        [SerializeField, Tooltip("Position offset from the camera to place the keyboard.")]
        Vector3 m_KeyboardOffset;

        /// <summary>
        /// Position offset from the camera to place the keyboard.
        /// </summary>
        public Vector3 keyboardOffset
        {
            get => m_KeyboardOffset;
            set => m_KeyboardOffset = value;
        }

        [SerializeField, Tooltip("Transform of the camera. If left empty, this will default to Camera.main.")]
        Transform m_CameraTransform;

        /// <summary>
        /// Transform of the camera. If left empty, this will default to Camera.main.
        /// </summary>
        public Transform cameraTransform
        {
            get => m_CameraTransform;
            set => m_CameraTransform = value;
        }

        [SerializeField, Tooltip("If true, the keyboard will be repositioned to the starting position if it is out of view when Show Keyboard is called.")]
        bool m_RepositionOutOfViewKeyboardOnOpen = true;

        /// <summary>
        /// If true, the keyboard will be repositioned to the starting position if it is out of view when Show Keyboard is called.
        /// </summary>
        public bool repositionOutOfViewKeyboardOnOpen
        {
            get => m_RepositionOutOfViewKeyboardOnOpen;
            set => m_RepositionOutOfViewKeyboardOnOpen = value;
        }

        [SerializeField, Tooltip("Threshold for the dot product when determining if the keyboard is out of view and should be repositioned. The lower the threshold, the wider the field of view."), Range(0f, 1f)]
        float m_FacingKeyboardThreshold = 0.15f;

        /// <summary>
        /// Threshold for the dot product when determining if the keyboard is out of view and should be repositioned. The lower the threshold, the wider the field of view.
        /// </summary>
        public float facingKeyboardThreshold
        {
            get => m_FacingKeyboardThreshold;
            set => m_FacingKeyboardThreshold = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;

            if (m_CameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                    m_CameraTransform = mainCamera.transform;
                else
                    Debug.LogWarning("Could not find main camera to assign the missing Camera Transform property.", this);
            }

            if (m_KeyboardPrefab != null)
            {
                keyboard = Instantiate(m_KeyboardPrefab, m_PlayerRoot).GetComponent<XRKeyboard>();
                keyboard.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Opens the global keyboard with a <see cref="TMP_InputField"/> to monitor.
        /// </summary>
        /// <remarks>This will update the keyboard with <see cref="TMP_InputField.text"/> as the existing string for the keyboard.</remarks>
        /// <param name="inputField">The input field for the global keyboard to monitor</param>
        /// <param name="observeCharacterLimit">If true, the global keyboard will respect the character limit of the
        /// <see cref="inputField"/>. This is false by default.</param>
        public virtual void ShowKeyboard(TMP_InputField inputField, bool observeCharacterLimit = false)
        {
            if (keyboard == null)
                return;

            // Check if keyboard is already open or should be repositioned
            var shouldPositionKeyboard = !keyboard.isOpen || (m_RepositionOutOfViewKeyboardOnOpen && IsKeyboardOutOfView());

            // Open keyboard
            keyboard.Open(inputField, observeCharacterLimit);

            // Position keyboard in front of user if the keyboard is closed
            if (shouldPositionKeyboard)
                PositionKeyboard(m_CameraTransform);
        }

        /// <summary>
        /// Opens the global keyboard with the option to populate it with existing text.
        /// </summary>
        /// <remarks>This will update the keyboard with <see cref="text"/> as the existing string for the keyboard.</remarks>
        /// <param name="text">The existing text string to populate the keyboard with on open.</param>
        public virtual void ShowKeyboard(string text)
        {
            if (keyboard == null)
                return;

            // Check if keyboard is already open or should be repositioned
            var shouldPositionKeyboard = !keyboard.isOpen || (m_RepositionOutOfViewKeyboardOnOpen && IsKeyboardOutOfView());

            // Open keyboard
            keyboard.Open(text);

            // Position keyboard in front of user if the keyboard is closed
            if (shouldPositionKeyboard)
                PositionKeyboard(m_CameraTransform);
        }

        /// <summary>
        /// Opens the global keyboard with the option to clear any existing keyboard text.
        /// </summary>
        /// <param name="clearKeyboardText">If true, the keyboard will open with no string populated in the keyboard. If false,
        /// the existing text will be maintained. This is false by default.</param>
        public void ShowKeyboard(bool clearKeyboardText = false)
        {
            if (keyboard == null)
                return;

            ShowKeyboard(clearKeyboardText ? string.Empty : keyboard.text);
        }

        /// <summary>
        /// Closes the global keyboard.
        /// </summary>
        public virtual void HideKeyboard()
        {
            if (keyboard == null)
                return;

            keyboard.Close();
        }

        /// <summary>
        /// Reposition <see cref="keyboard"/> to starting position if it is out of view. Keyboard will only reposition if is active and enabled.
        /// </summary>
        /// <remarks>
        /// Field if view is defined by the <see cref="facingKeyboardThreshold"/>, and the starting position
        /// is defined by the <see cref="keyboardOffset"/> in relation to the camera.
        /// </remarks>
        public void RepositionKeyboardIfOutOfView()
        {
            if (IsKeyboardOutOfView())
            {
                if (keyboard.isOpen)
                    PositionKeyboard(m_CameraTransform);
            }
        }

        void PositionKeyboard(Transform target)
        {
            var position = target.position +
                target.right * m_KeyboardOffset.x +
                target.forward * m_KeyboardOffset.z +
                Vector3.up * m_KeyboardOffset.y;
            keyboard.transform.position = position;
            FaceKeyboardAtTarget(m_CameraTransform);
        }

        void FaceKeyboardAtTarget(Transform target)
        {
            var forward = (keyboard.transform.position - target.position).normalized;
            BurstMathUtility.OrthogonalLookRotation(forward, Vector3.up, out var newTarget);
            keyboard.transform.rotation = newTarget;
        }

        bool IsKeyboardOutOfView()
        {
            if (m_CameraTransform == null || keyboard == null)
            {
                Debug.LogWarning("Camera or keyboard reference is null. Unable to determine if keyboard is out of view.", this);
                return false;
            }

            var dotProduct = Vector3.Dot(m_CameraTransform.forward, (keyboard.transform.position - m_CameraTransform.position).normalized);
            return dotProduct < m_FacingKeyboardThreshold;
        }
    }
}
#endif
