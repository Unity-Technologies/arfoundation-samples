using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Component which reads input values and drives the thumbstick, trigger, and grip transforms
    /// to animate a controller model.
    /// </summary>
    public class ControllerAnimator : MonoBehaviour
    {
        [Header("Thumbstick")]
        [SerializeField]
        Transform m_ThumbstickTransform;

        [SerializeField]
        Vector2 m_StickRotationRange = new Vector2(30f, 30f);

        [SerializeField]
        XRInputValueReader<Vector2> m_StickInput = new XRInputValueReader<Vector2>("Thumbstick");

        [Header("Trigger")]
        [SerializeField]
        Transform m_TriggerTransform;

        [SerializeField]
        Vector2 m_TriggerXAxisRotationRange = new Vector2(0f, -15f);

        [SerializeField]
        XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");

        [Header("Grip")]
        [SerializeField]
        Transform m_GripTransform;

        [SerializeField]
        Vector2 m_GripRightRange = new Vector2(-0.0125f, -0.011f);

        [SerializeField]
        XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");

        void OnEnable()
        {
            if (m_ThumbstickTransform == null || m_GripTransform == null || m_TriggerTransform == null)
            {
                enabled = false;
                Debug.LogWarning($"Controller Animator component missing references on {gameObject.name}", this);
                return;
            }

            m_StickInput?.EnableDirectActionIfModeUsed();
            m_TriggerInput?.EnableDirectActionIfModeUsed();
            m_GripInput?.EnableDirectActionIfModeUsed();
        }

        void OnDisable()
        {
            m_StickInput?.DisableDirectActionIfModeUsed();
            m_TriggerInput?.DisableDirectActionIfModeUsed();
            m_GripInput?.DisableDirectActionIfModeUsed();
        }

        void Update()
        {
            if (m_StickInput != null)
            {
                var stickVal = m_StickInput.ReadValue();
                m_ThumbstickTransform.localRotation = Quaternion.Euler(-stickVal.y * m_StickRotationRange.x, 0f, -stickVal.x * m_StickRotationRange.y);
            }

            if (m_TriggerInput != null)
            {
                var triggerVal = m_TriggerInput.ReadValue();
                m_TriggerTransform.localRotation = Quaternion.Euler(Mathf.Lerp(m_TriggerXAxisRotationRange.x, m_TriggerXAxisRotationRange.y, triggerVal), 0f, 0f);
            }

            if (m_GripInput != null)
            {
                var gripVal = m_GripInput.ReadValue();
                var currentPos = m_GripTransform.localPosition;
                m_GripTransform.localPosition = new Vector3(Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, gripVal), currentPos.y, currentPos.z);
            }
        }
    }
}
