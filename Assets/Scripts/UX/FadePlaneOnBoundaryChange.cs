using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlane))]
[RequireComponent(typeof(Animator))]
public class FadePlaneOnBoundaryChange : MonoBehaviour
{
    const string k_FadeOffAnim = "FadeOff";
    const string k_FadeOnAnim = "FadeOn";
    const float k_TimeOut = 2.0f;
    
    Animator m_Animator;
    ARPlane m_Plane;

    float m_ShowTime = 0;
    bool m_UpdatingPlane = false;

    void OnEnable()
    {
        m_Plane = GetComponent<ARPlane>();
        m_Animator = GetComponent<Animator>();
        
        m_Plane.boundaryChanged += PlaneOnBoundaryChanged;
    }

    void OnDisable()
    {
        m_Plane.boundaryChanged -= PlaneOnBoundaryChanged;
    }

    void Update()
    {
        if (m_UpdatingPlane)
        {
            m_ShowTime -= Time.deltaTime;

            if (m_ShowTime <= 0)
            {
                m_UpdatingPlane = false;
                m_Animator.SetBool(k_FadeOffAnim, true);
                m_Animator.SetBool(k_FadeOnAnim, false);
            }
        }
    }

    void PlaneOnBoundaryChanged(ARPlaneBoundaryChangedEventArgs obj)
    {
        m_Animator.SetBool(k_FadeOffAnim, false);
        m_Animator.SetBool(k_FadeOnAnim, true);
        m_UpdatingPlane = true;
        m_ShowTime = k_TimeOut;
    }
}