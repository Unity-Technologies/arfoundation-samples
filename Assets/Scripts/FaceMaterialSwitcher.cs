using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FaceMaterialSwitcher : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Materials to use for face meshes.")]
    Material[] m_FaceMaterials;

    /// <summary>
    /// Getter/setter for the Face Materials.
    /// </summary>
    public Material[] faceMaterials
    {
        get { return m_FaceMaterials; }
        set { m_FaceMaterials = value; }
    }

    static int s_CurrentMaterialIndex;
    static Dictionary<TrackableId, Material> s_FaceTracker = new Dictionary<TrackableId, Material>();

    void Start()
    {
        ARFace face = GetComponent<ARFace>();
        Material mat;
        if (!s_FaceTracker.TryGetValue(face.trackableId, out mat))
        {
            s_FaceTracker.Add(face.trackableId, m_FaceMaterials[s_CurrentMaterialIndex]);
            GetComponent<MeshRenderer>().material = m_FaceMaterials[s_CurrentMaterialIndex];
            s_CurrentMaterialIndex = (s_CurrentMaterialIndex + 1) % m_FaceMaterials.Length;
        }
        else
        {
            // Assign the material that was already used for the face's unique id.
            GetComponent<MeshRenderer>().material = mat;
        }
    }
}
