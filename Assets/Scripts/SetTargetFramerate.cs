using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple MonoBehaviour to set the Application's targetFrameRate
/// </summary>
public class SetTargetFramerate : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Sets the application's target frame rate.")]
    int m_TargetFrameRate = 60;

    /// <summary>
    /// Get or set the application's target frame rate.
    /// </summary>
    public int targetFrameRate
    {
        get { return m_TargetFrameRate; }
        set
        {
            m_TargetFrameRate = value;
            SetFrameRate();
        }
    }

    void SetFrameRate()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    void Start()
    {
        SetFrameRate();
    }
}
