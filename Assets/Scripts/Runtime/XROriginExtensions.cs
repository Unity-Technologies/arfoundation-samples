using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// This class contains extension methods suitable for replacing the <c>ARSessionOrigin.MakeContentAppearAt</c>
    /// API as existed in AR Foundation 4.2, allowing users to upgrade projects from
    /// <see cref="ARSessionOrigin"/> to <see cref="XROrigin"/> with continued access to this API. 
    /// </summary>
    public static class XROriginExtensions
    {
        /// <summary>
        /// Makes <paramref name="content"/> appear to be placed at <paramref name="position"/> with orientation <paramref name="rotation"/>.
        /// </summary>
        /// <param name="origin">The <c>XROrigin</c> in the Scene.</param>
        /// <param name="content">The <c>Transform</c> of the content you wish to affect.</param>
        /// <param name="position">The position you wish the content to appear at. This could be
        /// a position on a detected plane, for example.</param>
        /// <param name="rotation">The rotation the content should appear to be in, relative
        /// to the <c>Camera</c>.</param>
        /// <remarks>
        /// This method does not actually change the <c>Transform</c> of content; instead,
        /// it updates the <c>XROrigin</c>'s <c>Transform</c> so the content
        /// appears to be at the given position and rotation. This is useful for placing AR
        /// content onto surfaces when the content itself cannot be moved at runtime.
        /// For example, if your content includes terrain or a NavMesh, it cannot
        /// be moved or rotated dynamically.
        /// </remarks>
        public static void MakeContentAppearAt(this XROrigin origin, Transform content, Vector3 position, Quaternion rotation)
        {
            MakeContentAppearAt(origin, content, position);
            MakeContentAppearAt(origin, content, rotation);
        }

        /// <summary>
        /// Makes <paramref name="content"/> appear to be placed at <paramref name="position"/>.
        /// </summary>
        /// <param name="origin">The <c>XROrigin</c> in the Scene.</param>
        /// <param name="content">The <c>Transform</c> of the content you wish to affect.</param>
        /// <param name="position">The position you wish the content to appear at. This could be
        /// a position on a detected plane, for example.</param>
        /// <remarks>
        /// This method does not actually change the <c>Transform</c> of content; instead,
        /// it updates the <c>XROrigin</c>'s <c>Transform</c> so the content
        /// appears to be at the given position.
        /// </remarks>
        public static void MakeContentAppearAt(this XROrigin origin, Transform content, Vector3 position)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var originTransform = origin.transform;
            
            // Adjust the Camera Offset transform to account
            // for the actual position we want the content to appear at.
            origin.CameraFloorOffsetObject.transform.position += originTransform.position - position;

            // The XROrigin's position needs to match the content's pivot. This is so
            // the entire XROrigin rotates around the content (so the impression is that
            // the content is rotating, not the rig).
            originTransform.position = content.position;
        }

        /// <summary>
        /// Makes <paramref name="content"/> appear to have orientation <paramref name="rotation"/> relative to the <c>Camera</c>.
        /// </summary>
        /// <param name="origin">The <c>XROrigin</c> in the Scene.</param>
        /// <param name="content">The <c>Transform</c> of the content you wish to affect.</param>
        /// <param name="rotation">The rotation the content should appear to be in, relative
        /// to the <c>Camera</c>.</param>
        /// <remarks>
        /// This method does not actually change the <c>Transform</c> of content; instead,
        /// it updates the <c>XROrigin</c>'s <c>Transform</c> so that the content
        /// appears to be in the requested orientation.
        /// </remarks>
        public static void MakeContentAppearAt(this XROrigin origin, Transform content, Quaternion rotation)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            // Since we aren't rotating the content, we need to perform the inverse
            // operation on the XROrigin. For example, if we want the
            // content to appear to be rotated 90 degrees on the Y axis, we should
            // rotate our rig -90 degrees on the Y axis.
            origin.transform.rotation = Quaternion.Inverse(rotation) * content.rotation;
        }
    }
}
