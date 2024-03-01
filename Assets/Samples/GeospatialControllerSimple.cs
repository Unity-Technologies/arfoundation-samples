// <copyright file="GeospatialController.cs" company="Google LLC">
//
// Copyright 2022 Google LLC
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

namespace Google.XR.ARCoreExtensions.Samples.Geospatial
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;
    using Unity.XR.CoreUtils;
    using ARFoundationReplay;

#if UNITY_ANDROID
    using UnityEngine.Android;
#endif

    /// <summary>
    /// Controller for Geospatial sample.
    /// </summary>
    public class GeospatialControllerSimple : MonoBehaviour
    {
        private AREarthManager _earthManager;
        private ARStreetscapeGeometryManager _streetScapeGeometryManager;


        [Header("UI Elements")]
        [SerializeField] Text DebugText;

        [SerializeField]
        private Material streetscapeMaterial;
        [SerializeField]
        private Color[] streetscapeColors;

        private int _buildingMatIndex = 0;
        private Dictionary<TrackableId, GameObject> _streetScapeGeometries = new();

        private bool _isInitialized = false;
        private readonly StringBuilder _sb = new();

        private MaterialPropertyBlock[] _propertyBlocks;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Application.targetFrameRate = 60;

            var origin = FindObjectOfType<XROrigin>();
            _earthManager = origin.GetComponent<AREarthManager>();
            _streetScapeGeometryManager = origin.GetComponent<ARStreetscapeGeometryManager>();
            Debug.Log("Geospatial sample initialized.");

            _propertyBlocks = streetscapeColors.Select(color =>
            {
                var mpb = new MaterialPropertyBlock();
                mpb.SetColor("_BaseColor", color);
                return mpb;
            }).ToArray();
        }

        /// <summary>
        /// Unity's OnEnable() method.
        /// </summary>
        public void OnEnable()
        {
            _streetScapeGeometryManager.StreetscapeGeometriesChanged += GetStreetscapeGeometry;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Debug.Log("Stop location services.");
            Input.location.Stop();
            _streetScapeGeometryManager.StreetscapeGeometriesChanged -= GetStreetscapeGeometry;
        }

        private IEnumerator Start()
        {
            yield return StartLocationService();
            Debug.Log("Location services started.");

            yield return new WaitUntil(() =>
                ARSession.state != ARSessionState.SessionInitializing);

            Debug.Log("ARSession state: " + ARSession.state);

            var featureSupport = _earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            if (featureSupport != FeatureSupported.Supported)
            {
                Debug.LogWarning("Geospatial mode is not supported.");
            }

            yield return new WaitUntil(() =>
                _earthManager.EarthState != EarthState.ErrorEarthNotReady);

            var earthState = _earthManager.EarthState;
            if (earthState != EarthState.Enabled)
            {
                Debug.LogWarning($"Geospatial sample encountered an EarthState error: {earthState}");
                yield break;
            }
            _isInitialized = true;
        }


        private void Update()
        {
            UpdateDebugInfo();
        }


        private void GetStreetscapeGeometry(ARStreetscapeGeometriesChangedEventArgs eventArgs)
        {
            foreach (var added in eventArgs.Added)
            {
                AddOrUpdateRenderObject(added);
            }
            foreach (var updated in eventArgs.Updated)
            {
                AddOrUpdateRenderObject(updated);
            }
            foreach (var removed in eventArgs.Removed)
            {
                DestroyRenderObject(removed);
            }
        }

        private void AddOrUpdateRenderObject(ARStreetscapeGeometry geometry)
        {
            if (geometry.mesh == null)
            {
                return;
            }

            Pose pose = geometry.pose;

            // Check if a render object already exists for this streetscapegeometry and
            // create one if not.
            if (_streetScapeGeometries.TryGetValue(geometry.trackableId, out GameObject go))
            {
                // Just update the pose.
                go.transform.SetPositionAndRotation(pose.position, pose.rotation);
                return;
            }

            go = new GameObject("StreetscapeGeometryMesh");
            go.AddComponent<MeshFilter>().sharedMesh = geometry.mesh;

            // Set materials
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = streetscapeMaterial;
            if (geometry.streetscapeGeometryType == StreetscapeGeometryType.Building)
            {
                int length = _propertyBlocks.Length - 1;
                renderer.SetPropertyBlock(_propertyBlocks[1 + _buildingMatIndex % length]);
                _buildingMatIndex++;
            }
            else
            {
                renderer.SetPropertyBlock(_propertyBlocks[0]);
            }

            go.transform.SetPositionAndRotation(pose.position, pose.rotation);
            _streetScapeGeometries.Add(geometry.trackableId, go);
        }

        private void DestroyRenderObject(ARStreetscapeGeometry geometry)
        {
            if (_streetScapeGeometries.TryGetValue(geometry.trackableId, out GameObject go))
            {
                Destroy(go);
                _streetScapeGeometries.Remove(geometry.trackableId);
            }
        }

        private IEnumerator StartLocationService()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Requesting the fine location permission.");
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(3.0f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location service is disabled by the user.");
                yield break;
            }

            Debug.Log("Starting location service.");
            Input.location.Start();

            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                yield return null;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarningFormat(
                    "Location service ended with {0} status.", Input.location.status);
                Input.location.Stop();
            }
        }


        private void UpdateDebugInfo()
        {
            bool isPoseAvailable = _earthManager.EarthState == EarthState.Enabled
                && _earthManager.EarthTrackingState == TrackingState.Tracking;
            var pose = isPoseAvailable
                ? _earthManager.CameraGeospatialPose
                : new GeospatialPose();
            var supported = _earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);

            _sb.Clear();
            _sb.AppendLine($"IsInitialized: {_isInitialized}");
            _sb.AppendLine($"SessionState: {ARSession.state}");
            _sb.AppendLine($"LocationServiceStatus: {Input.location.status}");
            _sb.AppendLine($"FeatureSupported: {supported}");
            _sb.AppendLine($"EarthState: {_earthManager.EarthState}");
            _sb.AppendLine($"EarthTrackingState: {_earthManager.EarthTrackingState}");
            _sb.AppendLine($"  LAT/LNG: {pose.Latitude:F6}, {pose.Longitude:F6}");
            _sb.AppendLine($"  HorizontalAcc: {pose.HorizontalAccuracy:F6}");
            _sb.AppendLine($"  ALT: {pose.Altitude:F2}");
            _sb.AppendLine($"  VerticalAcc: {pose.VerticalAccuracy:F2}");
            _sb.AppendLine($"  EunRotation: {pose.EunRotation:F2}");
            _sb.AppendLine($"  OrientationYawAcc: {pose.OrientationYawAccuracy:F2}");
            DebugText.text = _sb.ToString();

        }
    }
}
