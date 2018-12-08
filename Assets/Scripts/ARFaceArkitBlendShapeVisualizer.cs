using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;

/// <summary>
/// Populates the action unit coefficients for an <see cref="ARFace"/>.
/// </summary>
/// <remarks>
/// If this <c>GameObject</c> has a <c>SkinnedMeshRenderer</c>,
/// this component will generate the blend shape coefficients from the underlying <c>ARFace</c>.
/// 
/// </remarks>
[RequireComponent(typeof(ARFace))]
public class ARFaceArkitBlendShapeVisualizer : MonoBehaviour
{
    [SerializeField]
    float coefficientScale = 100.0f;
    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;
    
    ARFace m_Face;
    ARKitFaceSubsystem arkitFaceSubsystem;

    static List<XRFaceArkitBlendShapeCoefficient> s_FaceArkitBlendShapeCoefficients;

    Dictionary<XRArkitBlendShapeLocation, int> m_FaceArkitBlendShapeIndexMap;
    
    void Awake()
    {
        s_FaceArkitBlendShapeCoefficients = new List<XRFaceArkitBlendShapeCoefficient>();
        m_Face = GetComponent<ARFace>();
        CreateFeatureBlendMapping();
    }

    void CreateFeatureBlendMapping()
    {
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
        {
            return;
        }
 
        const string strPrefix = "blendShape2.";
        m_FaceArkitBlendShapeIndexMap = new Dictionary<XRArkitBlendShapeLocation, int>();
        
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.BrowDownLeft        ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browDown_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.BrowDownRight       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browDown_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.BrowInnerUp         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browInnerUp");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.BrowOuterUpLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browOuterUp_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.BrowOuterUpRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browOuterUp_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.CheekPuff           ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekPuff");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.CheekSquintLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekSquint_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.CheekSquintRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekSquint_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeBlinkLeft        ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeBlink_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeBlinkRight       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeBlink_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookDownLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookDown_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookDownRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookDown_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookInLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookIn_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookInRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookIn_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookOutLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookOut_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookOutRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookOut_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookUpLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookUp_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeLookUpRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookUp_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeSquintLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeSquint_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeSquintRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeSquint_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeWideLeft         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeWide_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.EyeWideRight        ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeWide_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.JawForward          ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawForward");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.JawLeft             ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawLeft");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.JawOpen             ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawOpen");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.JawRight            ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawRight");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthClose          ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthClose");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthDimpleLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthDimple_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthDimpleRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthDimple_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthFrownLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFrown_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthFrownRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFrown_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthFunnel         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFunnel");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthLeft           ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLeft");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthLowerDownLeft  ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLowerDown_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthLowerDownRight ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLowerDown_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthPressLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPress_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthPressRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPress_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthPucker         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPucker");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthRight          ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRight");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthRollLower      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRollLower");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthRollUpper      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRollUpper");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthShrugLower     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthShrugLower");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthShrugUpper     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthShrugUpper");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthSmileLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthSmile_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthSmileRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthSmile_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthStretchLeft    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthStretch_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthStretchRight   ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthStretch_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthUpperUpLeft    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthUpperUp_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.MouthUpperUpRight   ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthUpperUp_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.NoseSneerLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "noseSneer_L");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.NoseSneerRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "noseSneer_R");
        m_FaceArkitBlendShapeIndexMap[XRArkitBlendShapeLocation.TongueOut           ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "tongueOut");
        
    }

    void SetVisible(bool visible)
    {
        if (skinnedMeshRenderer == null) return;
                    
        skinnedMeshRenderer.enabled = visible;
    }

    void UpdateVisibility()
    {
        var visible = enabled &&
                      (m_Face.trackingState != TrackingState.Unavailable) &&
                      (ARSubsystemManager.systemState > ARSystemState.Ready);

        SetVisible(visible);
    }

    void OnEnable()
    {
        arkitFaceSubsystem = (ARKitFaceSubsystem) ARSubsystemManager.faceSubsystem;
        if (arkitFaceSubsystem == null )
        {
            return;
        }
        m_Face.updated += OnUpdated;
        ARSubsystemManager.systemStateChanged += OnSystemStateChanged;
        UpdateVisibility();
    }

    void OnDisable()
    {
        m_Face.updated -= OnUpdated;
        ARSubsystemManager.systemStateChanged -= OnSystemStateChanged;
    }

    void OnSystemStateChanged(ARSystemStateChangedEventArgs eventArgs)
    {
        UpdateVisibility();
    }

    void OnUpdated(ARFace face)
    {
        UpdateVisibility();
        UpdateFaceFeatures();
    }
    
    void UpdateFaceFeatures()
    {
        if (skinnedMeshRenderer == null || !skinnedMeshRenderer.enabled)
        {
            return;
        }

        if (!arkitFaceSubsystem.TryGetFaceARKitBlendShapeCoefficients(m_Face.xrFace.trackableId,s_FaceArkitBlendShapeCoefficients))
        {
            return;
        }

        foreach (var xrFaceFeatureCoefficient in s_FaceArkitBlendShapeCoefficients)
        {
            int mappedBlendShapeIndex;
            if (m_FaceArkitBlendShapeIndexMap.TryGetValue(xrFaceFeatureCoefficient.arkitBlendShapeLocation, out mappedBlendShapeIndex))
            {
                if (mappedBlendShapeIndex >= 0 ) {
                    skinnedMeshRenderer.SetBlendShapeWeight (mappedBlendShapeIndex, xrFaceFeatureCoefficient.coefficient * coefficientScale);
                }
            }
        }
    }
}
