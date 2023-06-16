using UnityEngine.InputSystem;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [CreateAssetMenu(menuName = "XR/AR Foundation/Input Action References")]
    public class InputActionReferences : ScriptableObject
    {
        [SerializeField]
        InputActionProperty m_ScreenTap;

        [SerializeField]
        InputActionProperty m_ScreenTapPosition;

        [SerializeField]
        InputActionProperty m_LeftTriggerPressed;

        [SerializeField]
        InputActionProperty m_LeftHandPosition;

        [SerializeField]
        InputActionProperty m_LeftHandRotation;

        [SerializeField]
        InputActionProperty m_RightTriggerPressed;

        [SerializeField]
        InputActionProperty m_RightHandPosition;

        [SerializeField]
        InputActionProperty m_RightHandRotation;

        public InputActionProperty screenTap
        {
            get => m_ScreenTap;
            set => m_ScreenTap = value;
        }

        public InputActionProperty screenTapPosition
        {
            get => m_ScreenTapPosition;
            set => m_ScreenTapPosition = value;
        }

        public InputActionProperty leftTriggerPressed
        {
            get => m_LeftTriggerPressed;
            set => m_LeftTriggerPressed = value;
        }

        public InputActionProperty leftHandPosition
        {
            get => m_LeftHandPosition;
            set => m_LeftHandPosition = value;
        }

        public InputActionProperty leftHandRotation
        {
            get => m_LeftHandRotation;
            set => m_LeftHandRotation = value;
        }

        public InputActionProperty rightTriggerPressed
        {
            get => m_RightTriggerPressed;
            set => m_RightTriggerPressed = value;
        }

        public InputActionProperty rightHandPosition
        {
            get => m_RightHandPosition;
            set => m_RightHandPosition = value;
        }

        public InputActionProperty rightHandRotation
        {
            get => m_RightHandRotation;
            set => m_RightHandRotation = value;
        }
    }
}
