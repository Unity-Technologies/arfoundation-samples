using System;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ScreenSpaceJointVisualizer : MonoBehaviour
{
    // 2D joint skeleton
    enum JointIndices
    {
        Invalid = -1,
        head_joint = 0, // parent: neck_1_joint [1]
        neck_1_joint = 1, // parent: root [16]
        right_shoulder_1_joint = 2, // parent: neck_1_joint [1]
        right_forearm_joint = 3, // parent: right_shoulder_1_joint [2]
        right_hand_joint = 4, // parent: right_forearm_joint [3]
        left_shoulder_1_joint = 5, // parent: neck_1_joint [1]
        left_forearm_joint = 6, // parent: left_shoulder_1_joint [5]
        left_hand_joint = 7, // parent: left_forearm_joint [6]
        right_upLeg_joint = 8, // parent: root [16]
        right_leg_joint = 9, // parent: right_upLeg_joint [8]
        right_foot_joint = 10, // parent: right_leg_joint [9]
        left_upLeg_joint = 11, // parent: root [16]
        left_leg_joint = 12, // parent: left_upLeg_joint [11]
        left_foot_joint = 13, // parent: left_leg_joint [12]
        right_eye_joint = 14, // parent: head_joint [0]
        left_eye_joint = 15, // parent: head_joint [0]
        root = 16, // parent: <none> [-1]
    }

    [SerializeField]
    [Tooltip("The AR camera being used in the scene.")]
    Camera m_ARCamera;

    /// <summary>
    /// Get or set the <c>Camera</c>.
    /// </summary>
    public Camera arCamera
    {
        get { return m_ARCamera; }
        set { m_ARCamera = value; }
    }

    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce human body anchors.")]
    ARHumanBodyManager m_HumanBodyManager;

    /// <summary>
    /// Get or set the <c>ARHumanBodyManager</c>.
    /// </summary>
    public ARHumanBodyManager humanBodyManager
    {
        get { return m_HumanBodyManager; }
        set { m_HumanBodyManager = value; }
    }

    [SerializeField]
    [Tooltip("A prefab that contains a LineRenderer component that will be used for rendering lines, representing the skeleton joints.")]
    GameObject m_LineRendererPrefab;

    /// <summary>
    /// Get or set the Line Renderer prefab.
    /// </summary>
    public GameObject lineRendererPrefab
    {
        get { return m_LineRendererPrefab; }
        set { m_LineRendererPrefab = value; }
    }

    Dictionary<int, GameObject> m_LineRenderers;
    static HashSet<int> s_JointSet = new HashSet<int>();

    void Awake()
    {
        m_LineRenderers = new Dictionary<int, GameObject>();
    }

    void UpdateRenderer(NativeArray<XRHumanBodyPose2DJoint> joints, int index)
    {
        GameObject lineRendererGO;
        if (!m_LineRenderers.TryGetValue(index, out lineRendererGO))
        {
            lineRendererGO = Instantiate(m_LineRendererPrefab, transform);
            m_LineRenderers.Add(index, lineRendererGO);
        }

        var lineRenderer = lineRendererGO.GetComponent<LineRenderer>();

        // Traverse hierarchy to determine the longest line set that needs to be drawn.
        var positions = new NativeArray<Vector2>(joints.Length, Allocator.Temp);
        try
        {
            var boneIndex = index;
            int jointCount = 0;
            while (boneIndex >= 0)
            {
                var joint = joints[boneIndex];
                if (joint.tracked)
                {
                    positions[jointCount++] = joint.position;
                    if (!s_JointSet.Add(boneIndex))
                        break;
                }
                else
                    break;

                boneIndex = joint.parentIndex;
            }

            // Render the joints as lines on the camera's near clip plane.
            lineRenderer.positionCount = jointCount;
            lineRenderer.startWidth = 0.001f;
            lineRenderer.endWidth = 0.001f;
            for (int i = 0; i < jointCount; ++i)
            {
                var position = positions[i];
                var worldPosition = m_ARCamera.ViewportToWorldPoint(
                    new Vector3(position.x, position.y, m_ARCamera.nearClipPlane));
                lineRenderer.SetPosition(i, worldPosition);
            }
            lineRendererGO.SetActive(true);
        }
        finally
        {
            positions.Dispose();
        }
    }

    void Update()
    {
        Debug.Assert(m_HumanBodyManager != null, "Human body manager cannot be null");
        var joints = m_HumanBodyManager.GetHumanBodyPose2DJoints(Allocator.Temp);
        if (!joints.IsCreated)
        {
            HideJointLines();
            return;
        }

        using (joints)
        {
            s_JointSet.Clear();
            for (int i = joints.Length - 1; i >= 0; --i)
            {
                if (joints[i].parentIndex != -1)
                    UpdateRenderer(joints, i);
            }
        }
    }

    void HideJointLines()
    {
        foreach (var lineRenderer in m_LineRenderers)
        {
            lineRenderer.Value.SetActive(false);
        }
    }
}
