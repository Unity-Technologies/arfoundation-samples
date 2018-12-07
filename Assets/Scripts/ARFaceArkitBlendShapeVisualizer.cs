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

    Dictionary<XRArkitBlendShapeLocation, string> m_FaceArkitBlendShapeNameMap;
    
    void Awake()
    {
        s_FaceArkitBlendShapeCoefficients = new List<XRFaceArkitBlendShapeCoefficient>();
        m_Face = GetComponent<ARFace>();
        CreateFeatureBlendMapping();
    }

    void CreateFeatureBlendMapping()
    {
        m_FaceArkitBlendShapeNameMap = new Dictionary<XRArkitBlendShapeLocation, string>();
        
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.BrowDownLeft        ]   =   "browDown_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.BrowDownRight       ]   =   "browDown_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.BrowInnerUp         ]   =   "browInnerUp";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.BrowOuterUpLeft     ]   =   "browOuterUp_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.BrowOuterUpRight    ]   =   "browOuterUp_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.CheekPuff           ]   =   "cheekPuff";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.CheekSquintLeft     ]   =   "cheekSquint_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.CheekSquintRight    ]   =   "cheekSquint_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeBlinkLeft        ]   =   "eyeBlink_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeBlinkRight       ]   =   "eyeBlink_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookDownLeft     ]   =   "eyeLookDown_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookDownRight    ]   =   "eyeLookDown_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookInLeft       ]   =   "eyeLookIn_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookInRight      ]   =   "eyeLookIn_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookOutLeft      ]   =   "eyeLookOut_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookOutRight     ]   =   "eyeLookOut_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookUpLeft       ]   =   "eyeLookUp_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeLookUpRight      ]   =   "eyeLookUp_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeSquintLeft       ]   =   "eyeSquint_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeSquintRight      ]   =   "eyeSquint_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeWideLeft         ]   =   "eyeWide_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.EyeWideRight        ]   =   "eyeWide_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.JawForward          ]   =   "jawForward";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.JawLeft             ]   =   "jawLeft";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.JawOpen             ]   =   "jawOpen";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.JawRight            ]   =   "jawRight";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthClose          ]   =   "mouthClose";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthDimpleLeft     ]   =   "mouthDimple_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthDimpleRight    ]   =   "mouthDimple_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthFrownLeft      ]   =   "mouthFrown_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthFrownRight     ]   =   "mouthFrown_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthFunnel         ]   =   "mouthFunnel";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthLeft           ]   =   "mouthLeft";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthLowerDownLeft  ]   =   "mouthLowerDown_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthLowerDownRight ]   =   "mouthLowerDown_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthPressLeft      ]   =   "mouthPress_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthPressRight     ]   =   "mouthPress_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthPucker         ]   =   "mouthPucker";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthRight          ]   =   "mouthRight";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthRollLower      ]   =   "mouthRollLower";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthRollUpper      ]   =   "mouthRollUpper";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthShrugLower     ]   =   "mouthShrugLower";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthShrugUpper     ]   =   "mouthShrugUpper";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthSmileLeft      ]   =   "mouthSmile_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthSmileRight     ]   =   "mouthSmile_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthStretchLeft    ]   =   "mouthStretch_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthStretchRight   ]   =   "mouthStretch_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthUpperUpLeft    ]   =   "mouthUpperUp_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.MouthUpperUpRight   ]   =   "mouthUpperUp_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.NoseSneerLeft       ]   =   "noseSneer_L";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.NoseSneerRight      ]   =   "noseSneer_R";
        m_FaceArkitBlendShapeNameMap[XRArkitBlendShapeLocation.TongueOut           ]   =   "tongueOut";
        
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
            const string strPrefix = "blendShape2.";
            string mappedBlendShapeName;
            if (m_FaceArkitBlendShapeNameMap.TryGetValue(xrFaceFeatureCoefficient.arkitBlendShapeLocation, out mappedBlendShapeName))
            {
                int blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex (strPrefix + mappedBlendShapeName);
                if (blendShapeIndex >= 0 ) {
                    skinnedMeshRenderer.SetBlendShapeWeight (blendShapeIndex, xrFaceFeatureCoefficient.coefficient * coefficientScale);
                }
            }
        }
    }
}
