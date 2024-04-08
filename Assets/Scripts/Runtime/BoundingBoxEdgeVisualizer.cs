using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARBoundingBox))]
public class BoundingBoxEdgeVisualizer : MonoBehaviour
{
    const float k_EdgeThickness = 0.01f;

    [Header("Top Edges")]
    [SerializeField]
    Renderer m_TopEdge1;

    [SerializeField]
    Renderer m_TopEdge2;

    [SerializeField]
    Renderer m_TopEdge3;

    [SerializeField]
    Renderer m_TopEdge4;

    [Header("Vertical Edges")]
    [SerializeField]
    Renderer m_VerticalEdge1;

    [SerializeField]
    Renderer m_VerticalEdge2;

    [SerializeField]
    Renderer m_VerticalEdge3;

    [SerializeField]
    Renderer m_VerticalEdge4;

    [SerializeField, HideInInspector]
    ARBoundingBox m_ARBoundingBox;

    [Header("BottomEdges")]
    [SerializeField]
    Renderer m_BottomEdge1;

    [SerializeField]
    Renderer m_BottomEdge2;

    [SerializeField]
    Renderer m_BottomEdge3;

    [SerializeField]
    Renderer m_BottomEdge4;

    Vector3 m_PreviouseSize;

    public void SetGradient(Gradient gradient)
    {
        // top edges
        m_TopEdge1.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_TopEdge1.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_TopEdge2.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_TopEdge2.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_TopEdge3.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_TopEdge3.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_TopEdge4.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_TopEdge4.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        // vertical edges
        m_VerticalEdge1.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_VerticalEdge1.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_VerticalEdge2.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_VerticalEdge2.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_VerticalEdge3.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_VerticalEdge3.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_VerticalEdge4.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_VerticalEdge4.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        // bottom edges
        m_BottomEdge1.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_BottomEdge1.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_BottomEdge2.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_BottomEdge2.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_BottomEdge3.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_BottomEdge3.material.SetColor("_ColorB", gradient.colorKeys[1].color);

        m_BottomEdge4.material.SetColor("_ColorA", gradient.colorKeys[0].color);
        m_BottomEdge4.material.SetColor("_ColorB", gradient.colorKeys[1].color);
    }

    void Reset()
    {
        m_ARBoundingBox = GetComponent<ARBoundingBox>();
    }

    void Awake()
    {
        if (m_ARBoundingBox == null)
            m_ARBoundingBox = GetComponent<ARBoundingBox>();
    }

    void Update()
    {
        if (m_ARBoundingBox.size == m_PreviouseSize)
            return;

        m_PreviouseSize = m_ARBoundingBox.size;
        var size = m_ARBoundingBox.size;
        var halfSize = size * 0.5f;
        
        UpdateTopEdges(size, halfSize);
        UpdateVerticalEdges(size, halfSize);
        UpdateBottomEdges(size, halfSize);
    }
    
    void UpdateTopEdges(Vector3 size, Vector3 halfSize)
    {
        // update sizes
        m_TopEdge1.transform.localScale = new(k_EdgeThickness, k_EdgeThickness, size.z);
        m_TopEdge2.transform.localScale = new(size.x, k_EdgeThickness, k_EdgeThickness);
        m_TopEdge3.transform.localScale = new(k_EdgeThickness, k_EdgeThickness, size.z);
        m_TopEdge4.transform.localScale = new(size.x, k_EdgeThickness, k_EdgeThickness);

        // update positions
        m_TopEdge1.transform.localPosition = new(halfSize.x, halfSize.y, 0);
        m_TopEdge2.transform.localPosition = new(0, halfSize.y, halfSize.z);
        m_TopEdge3.transform.localPosition = new(-halfSize.x, halfSize.y, 0);
        m_TopEdge4.transform.localPosition = new(0, halfSize.y, -halfSize.z);
    }

    void UpdateVerticalEdges(Vector3 size, Vector3 halfSize)
    {
        // update sizes
        m_VerticalEdge1.transform.localScale = new(k_EdgeThickness, size.y, k_EdgeThickness);
        m_VerticalEdge2.transform.localScale = new(k_EdgeThickness, size.y, k_EdgeThickness);
        m_VerticalEdge3.transform.localScale = new(k_EdgeThickness, size.y, k_EdgeThickness);
        m_VerticalEdge4.transform.localScale = new(k_EdgeThickness, size.y, k_EdgeThickness);

        // update positions
        m_VerticalEdge1.transform.localPosition = new(halfSize.x, 0, -halfSize.z);
        m_VerticalEdge2.transform.localPosition = new(halfSize.x, 0, halfSize.z);
        m_VerticalEdge3.transform.localPosition = new(-halfSize.x, 0, halfSize.z);
        m_VerticalEdge4.transform.localPosition = new(-halfSize.x, 0, -halfSize.z);
    }

    void UpdateBottomEdges(Vector3 size, Vector3 halfSize)
    {
        // update sizes
        m_BottomEdge1.transform.localScale = new(k_EdgeThickness, k_EdgeThickness, size.z);
        m_BottomEdge2.transform.localScale = new(size.x, k_EdgeThickness, k_EdgeThickness);
        m_BottomEdge3.transform.localScale = new(k_EdgeThickness, k_EdgeThickness, size.z);
        m_BottomEdge4.transform.localScale = new(size.x, k_EdgeThickness, k_EdgeThickness);

        // update positions
        m_BottomEdge1.transform.localPosition = new(halfSize.x, -halfSize.y, 0);
        m_BottomEdge2.transform.localPosition = new(0, -halfSize.y, halfSize.z);
        m_BottomEdge3.transform.localPosition = new(-halfSize.x, -halfSize.y, 0);
        m_BottomEdge4.transform.localPosition = new(0, -halfSize.y, -halfSize.z);
    }
}
