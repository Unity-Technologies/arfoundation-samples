using UnityEngine.XR.ARFoundation;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System;

public class BoneController : MonoBehaviour
{
    // Joint indices of the 3D skeleton.
    enum JointIndices
    {
        Invalid = -1,
        Root = 0, // parent: Invalid
        Hips = 1, // parent: Root
        LeftUpLeg = 2, // parent: Hips
        LeftLeg = 3, // parent: LeftUpLeg
        LeftFoot = 4, // parent: LeftLeg
        LeftToes = 5, // parent: LeftFoot
        LeftToesEnd = 6, // parent: LeftToes
        RightUpLeg = 7, // parent: Hips
        RightLeg = 8, // parent: RightUpLeg
        RightFoot = 9, // parent: RightLeg
        RightToes = 10, // parent: RightFoot
        RightToesEnd = 11, // parent: RightToes
        Spine1 = 12, // parent: Hips
        Spine2 = 13, // parent: Spine1
        Spine3 = 14, // parent: Spine2
        Spine4 = 15, // parent: Spine3
        Spine5 = 16, // parent: Spine4
        Spine6 = 17, // parent: Spine5
        Spine7 = 18, // parent: Spine6
        RightShoulder1 = 19, // parent: Spine7
        RightShoulder2 = 20, // parent: RightShoulder1
        RightArm = 21, // parent: RightShoulder2
        RightForearm = 22, // parent: RightArm
        RightHand = 23, // parent: RightForearm
        RightHandThumbStart = 24, // parent: RightHand
        RightHandThumb1 = 25, // parent: RightHandThumbStart
        RightHandThumb2 = 26, // parent: RightHandThumb1
        RightHandThumbEnd = 27, // parent: RightHandThumb2
        RightHandIndexStart = 28, // parent: RightHand
        RightHandIndex1 = 29, // parent: RightHandIndexStart
        RightHandIndex2 = 30, // parent: RightHandIndex1
        RightHandIndex3 = 31, // parent: RightHandIndex2
        RightHandIndexEnd = 32, // parent: RightHandIndex3
        RightHandMidStart = 33, // parent: RightHand
        RightHandMid1 = 34, // parent: RightHandMidStart
        RightHandMid2 = 35, // parent: RightHandMid1
        RightHandMid3 = 36, // parent: RightHandMid2
        RightHandMidEnd = 37, // parent: RightHandMid3
        RightHandRingStart = 38, // parent: RightHand
        RightHandRing1 = 39, // parent: RightHandRingStart
        RightHandRing2 = 40, // parent: RightHandRing1
        RightHandRing3 = 41, // parent: RightHandRing2
        RightHandRingEnd = 42, // parent: RightHandRing3
        RightHandPinkyStart = 43, // parent: RightHand
        RightHandPinky1 = 44, // parent: RightHandPinkyStart
        RightHandPinky2 = 45, // parent: RightHandPinky1
        RightHandPinky3 = 46, // parent: RightHandPinky2
        RightHandPinkyEnd = 47, // parent: RightHandPinky3
        LeftShoulder1 = 48, // parent: Spine7
        LeftShoulder2 = 49, // parent: LeftShoulder1
        LeftArm = 50, // parent: LeftShoulder2
        LeftForearm = 51, // parent: LeftArm
        LeftHand = 52, // parent: LeftForearm
        LeftHandThumbStart = 53, // parent: LeftHand
        LeftHandThumb1 = 54, // parent: LeftHandThumbStart
        LeftHandThumb2 = 55, // parent: LeftHandThumb1
        LeftHandThumbEnd = 56, // parent: LeftHandThumb2
        LeftHandIndexStart = 57, // parent: LeftHand
        LeftHandIndex1 = 58, // parent: LeftHandIndexStart
        LeftHandIndex2 = 59, // parent: LeftHandIndex1
        LeftHandIndex3 = 60, // parent: LeftHandIndex2
        LeftHandIndexEnd = 61, // parent: LeftHandIndex3
        LeftHandMidStart = 62, // parent: LeftHand
        LeftHandMid1 = 63, // parent: LeftHandMidStart
        LeftHandMid2 = 64, // parent: LeftHandMid1
        LeftHandMid3 = 65, // parent: LeftHandMid2
        LeftHandMidEnd = 66, // parent: LeftHandMid3
        LeftHandRingStart = 67, // parent: LeftHand
        LeftHandRing1 = 68, // parent: LeftHandRingStart
        LeftHandRing2 = 69, // parent: LeftHandRing1
        LeftHandRing3 = 70, // parent: LeftHandRing2
        LeftHandRingEnd = 71, // parent: LeftHandRing3
        LeftHandPinkyStart = 72, // parent: LeftHand
        LeftHandPinky1 = 73, // parent: LeftHandPinkyStart
        LeftHandPinky2 = 74, // parent: LeftHandPinky1
        LeftHandPinky3 = 75, // parent: LeftHandPinky2
        LeftHandPinkyEnd = 76, // parent: LeftHandPinky3
        Neck1 = 77, // parent: Spine7
        Neck2 = 78, // parent: Neck1
        Neck3 = 79, // parent: Neck2
        Neck4 = 80, // parent: Neck3
        Head = 81, // parent: Neck4
        Jaw = 82, // parent: Head
        Chin = 83, // parent: Jaw
        Nose = 84, // parent: Head
        RightEye = 85, // parent: Head
        RightEyeUpperLid = 86, // parent: RightEye
        RightEyeLowerLid = 87, // parent: RightEye
        RightEyeBall = 88, // parent: RightEye
        LeftEye = 89, // parent: Head
        LeftEyeUpperLid = 90, // parent: LeftEye
        LeftEyeLowerLid = 91, // parent: LeftEye
        LeftEyeBall = 92, // parent: LeftEye
    }
    const int k_NumSkeletonJoints = 93;

    [SerializeField]
    [Tooltip("The root bone of the skeleton.")]
    Transform m_SkeletonRoot;

    /// <summary>
    /// Get/Set the root bone of the skeleton.
    /// </summary>
    public Transform skeletonRoot
    {
        get
        {
            return m_SkeletonRoot;
        }
        set
        {
            m_SkeletonRoot = value;
        }
    }

    Transform[] m_BoneMapping = new Transform[k_NumSkeletonJoints];

    public void InitializeSkeletonJoints()
    {
        // Walk through all the child joints in the skeleton and
        // store the skeleton joints at the corresponding index in the m_BoneMapping array.
        // This assumes that the bones in the skeleton are named as per the
        // JointIndices enum above.
        Queue<Transform> nodes = new Queue<Transform>();
        nodes.Enqueue(m_SkeletonRoot);
        while (nodes.Count > 0)
        {
            Transform next = nodes.Dequeue();
            for (int i = 0; i < next.childCount; ++i)
            {
                nodes.Enqueue(next.GetChild(i));
            }
            ProcessJoint(next);
        }
    }

    public void ApplyBodyPose(ARHumanBody body)
    {
        var joints = body.joints;
        if (!joints.IsCreated)
            return;

        for (int i = 0; i < k_NumSkeletonJoints; ++i)
        {
            XRHumanBodyJoint joint = joints[i];
            var bone = m_BoneMapping[i];
            if (bone != null)
            {
                bone.transform.localPosition = joint.localPose.position;
                bone.transform.localRotation = joint.localPose.rotation;
            }
        }
    }

    void ProcessJoint(Transform joint)
    {
        int index = GetJointIndex(joint.name);
        if (index >= 0 && index < k_NumSkeletonJoints)
        {
            m_BoneMapping[index] = joint;
        }
        else
        {
            Debug.LogWarning($"{joint.name} was not found.");
        }
    }

    // Returns the integer value corresponding to the JointIndices enum value
    // passed in as a string.
    int GetJointIndex(string jointName)
    {
        JointIndices val;
        if (Enum.TryParse(jointName, out val))
        {
            return (int)val;
        }
        return -1;
    }
}
