//-----------------------------------------------------------------------
// <copyright file="MapQualityIndicator.cs" company="Google LLC">
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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR.ARSubsystems;

    /// <summary>
    /// An animated indicator used to guide better hosting quality.
    /// </summary>
    public class MapQualityIndicator : MonoBehaviour
    {
        /// <summary>
        /// The map quality bar game object.
        /// </summary>
        public GameObject MapQualityBarPrefab;

        /// <summary>
        /// The line renderer that draws an arc at the bottom of the indicator.
        /// </summary>
        public CircleRenderer CircleRenderer;

        /// <summary>
        /// The range of this indicator.
        /// In this sample, it will be set to 150 degrees on a vertical surface,
        /// or 180 degrees on a horizontal surface.
        /// Adjust the value to match the actual use case.
        /// </summary>
        [Range(0, 360)]
        public float Range = 150.0f;

        /// <summary>
        /// The radius of this indicator.
        /// </summary>
        public float Radius = 0.1f;

        /// <summary>
        /// The indication range for vertical surface.
        /// </summary>
        private const float _verticalRange = 150.0f;

        /// <summary>
        /// The indication range for horizontal surface.
        /// </summary>
        private const float _horizontalRange = 180.0f;

        /// <summary>
        /// The quality threshold which indicates it's sufficient to host an anchor.
        /// </summary>
        private const float _qualityThreshold = 0.6f;

        /// <summary>
        /// The angle threshold that indicates the camera is looking from the top view of
        /// this indicator.
        /// </summary>
        private const float _topviewThreshold = 15.0f;

        private const float _disappearDuration = 0.5f;
        private const float _fadingDuration = 3.0f;
        private const float _barSpacing = 7.5f;
        private const float _circleFadingRange = 10.0f;

        private int _currentQualityState = 0;
        private Camera _mainCamera = null;
        private Vector3 _centerDir;
        private float _fadingTimer = -1.0f;
        private float _disappearTimer = -1.0f;
        private List<MapQualityBar> _mapQualityBars = new List<MapQualityBar>();

        /// <summary>
        /// Gets a value indicating whether the map quality reaches the threshold
        /// and is ready for hosting.
        /// </summary>
        public bool ReachQualityThreshold
        {
            get
            {
                float currentQuality = 0.0f;
                foreach (var bar in _mapQualityBars)
                {
                    currentQuality += bar.Weight;
                }

                return (currentQuality / _mapQualityBars.Count) >= _qualityThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the camera is at the top view position.
        /// </summary>
        public bool ReachTopviewAngle
        {
            get
            {
                var cameraDir = _mainCamera.transform.position - transform.position;
                return Vector3.Angle(cameraDir, Vector3.up) < _topviewThreshold;
            }
        }

        /// <summary>
        /// Update the quality state for current frame.
        /// </summary>
        /// <param name="quality">The quality state.</param>
        public void UpdateQualityState(int quality)
        {
            _currentQualityState = quality;
        }

        /// <summary>
        /// Draw a map quality indicator at the given position and enable it.
        /// </summary>
        /// <param name="planeAlignment">The alignment of the plane where the anchor is placed.
        /// </param>
        /// <param name="camera">The main camera.</param>
        public void DrawIndicator(PlaneAlignment planeAlignment, Camera camera)
        {
            // To use customized value, remove this line and set the desired range in inspector.
            Range = planeAlignment == PlaneAlignment.Vertical ?
                _verticalRange : _horizontalRange;

            _mainCamera = camera;

            // Get the direction from the center of the circle to the center of the arc
            // in world space.
            _centerDir = planeAlignment == PlaneAlignment.Vertical ?
                transform.TransformVector(Vector3.up) :
                transform.TransformVector(-Vector3.forward);

            DrawBars();
            DrawRing();

            gameObject.SetActive(true);
        }

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Default map quality indicator to disable.
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity's Update().
        /// </summary>
        public void Update()
        {
            // Play fading animation.
            if (ReachTopviewAngle)
            {
                // Fading animation finished.
                if (_fadingTimer >= _fadingDuration)
                {
                    return;
                }

                // Start fading animation.
                if (_fadingTimer < 0)
                {
                    _fadingTimer = 0.0f;
                }

                _fadingTimer += Time.deltaTime;
                float alpha = Mathf.Clamp(1 - (_fadingTimer / _fadingDuration), 0, 1);
                CircleRenderer.SetAlpha(alpha);
                foreach (var bar in _mapQualityBars)
                {
                    bar.SetAlpha(alpha);
                }

                return;
            }
            else if (_fadingTimer > 0)
            {
                _fadingTimer -= Time.deltaTime;
                float alpha = Mathf.Clamp(1 - (_fadingTimer / _fadingDuration), 0, 1);
                CircleRenderer.SetAlpha(alpha);
                foreach (var bar in _mapQualityBars)
                {
                    bar.SetAlpha(alpha);
                }
            }

            // Update visited bar.
            foreach (MapQualityBar bar in _mapQualityBars)
            {
                if (IsLookingAtBar(bar))
                {
                    bar.IsVisited = true;
                    bar.QualityState = _currentQualityState;
                }
            }

            PlayDisappearAnimation();
        }

        private void DrawRing()
        {
            CircleRenderer.DrawArc(_centerDir, Radius, Range + (2 * _circleFadingRange));
        }

        private void DrawBars()
        {
            // Place a quality bar at the center position.
            Vector3 basePos = transform.position;
            Vector3 position = _centerDir * Radius;
            var rotation = Quaternion.AngleAxis(0, Vector3.up);
            var qualityBar =
                Instantiate(MapQualityBarPrefab, basePos + position, rotation, transform);
            _mapQualityBars.Add(qualityBar.GetComponent<MapQualityBar>());

            for (float deltaAngle = _barSpacing; deltaAngle < Range / 2;
                deltaAngle += _barSpacing)
            {
                // Place a quality bar at right side.
                rotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
                position = (rotation * _centerDir) * Radius;
                qualityBar =
                    Instantiate(MapQualityBarPrefab, basePos + position, rotation, transform);
                _mapQualityBars.Add(qualityBar.GetComponent<MapQualityBar>());

                // Place a quality bar at left side.
                rotation = Quaternion.AngleAxis(-deltaAngle, Vector3.up);
                position = (rotation * _centerDir) * Radius;
                qualityBar =
                    Instantiate(MapQualityBarPrefab, basePos + position, rotation, transform);
                _mapQualityBars.Add(qualityBar.GetComponent<MapQualityBar>());
            }
        }

        private bool IsLookingAtBar(MapQualityBar bar)
        {
            // Check whether the bar is inside camera's view:
            var screenPoint = _mainCamera.WorldToViewportPoint(bar.transform.position);
            if (screenPoint.z <= 0 || screenPoint.x <= 0 || screenPoint.x >= 1 ||
                screenPoint.y <= 0 || screenPoint.y >= 1)
            {
                return false;
            }

            // Check the distance between the indicator and the camera.
            float distance = (transform.position - _mainCamera.transform.position).magnitude;
            if (distance <= Radius)
            {
                return false;
            }

            var cameraDir = Vector3.ProjectOnPlane(
                _mainCamera.transform.position - transform.position, Vector3.up);
            var barDir = Vector3.ProjectOnPlane(
                bar.transform.position - transform.position, Vector3.up);
            return Vector3.Angle(cameraDir, barDir) < _barSpacing;
        }

        private void PlayDisappearAnimation()
        {
            if (_disappearTimer < 0.0f && ReachQualityThreshold)
            {
                _disappearTimer = 0.0f;
            }

            if (_disappearTimer >= 0.0f && _disappearTimer < _disappearDuration)
            {
                _disappearTimer += Time.deltaTime;
                float scale =
                    Mathf.Max(0.0f, (_disappearDuration - _disappearTimer) / _disappearDuration);
                transform.localScale = new Vector3(scale, scale, scale);
            }

            if (_disappearTimer >= _disappearDuration)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
