//-----------------------------------------------------------------------
// <copyright file="CircleRenderer.cs" company="Google LLC">
//
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Google.XR.ARCoreExtensions.Samples.PersistentCloudAnchors
{
    using UnityEngine;

    /// <summary>
    /// A circle renderer, draws an arc using a given range.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class CircleRenderer : MonoBehaviour
    {
        /// <summary>
        /// The count of the segments that represents a full circle.
        /// </summary>
        [Range(4, 720)]
        public int Segment = 72;

        // Variable name string used for changing particle material color.
        private const string _varColor = "_TintColor";

        private LineRenderer _lineRenderer;

        private float _alpha = 1.0f;

        /// <summary>
        /// Set the alpha value for this circle.
        /// </summary>
        /// <param name="alpha">Alpha value of this circle.</param>
        public void SetAlpha(float alpha)
        {
            _alpha = alpha;
        }

        /// <summary>
        /// Draws an arc based on given properties and sets it to visible.
        /// </summary>
        /// <param name="centerDir">The center direction of the arc in world space,
        /// it's always perpendicular to <see cref="Vector3.up"/> direction,
        /// so that the arc is drawn on a horizontal plane.</param>
        /// <param name="radius">The radius of the arc.</param>
        /// <param name="range"><The range of the arc in degrees./param>
        public void DrawArc(Vector3 centerDir, float radius, float range)
        {
            range = Mathf.Clamp(range, 0.0f, 360.0f);
            if (range == 0.0f)
            {
                gameObject.SetActive(false);
                return;
            }

            int count = (int)(range * Segment / 360.0f);
            float pointSpacing = range / count;

            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.GetComponent<LineRenderer>();
            }

            _lineRenderer.positionCount = count + 1;
            _lineRenderer.useWorldSpace = false;
            for (int i = 0; i < count + 1; ++i)
            {
                float deltaAngle = (-range / 2) + (pointSpacing * i);
                var rotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
                var position = (rotation * centerDir * radius) + transform.position;
                _lineRenderer.SetPosition(i, transform.InverseTransformPoint(position));
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity's Update() method.
        /// </summary>
        public void Update()
        {
            if (_lineRenderer == null)
            {
                return;
            }

            var renderer = _lineRenderer.GetComponent<Renderer>();
            var color = renderer.material.GetColor(_varColor);
            color.a = _alpha;
            renderer.material.SetColor(_varColor, color);
        }
    }
}
