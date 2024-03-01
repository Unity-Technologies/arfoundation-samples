//-----------------------------------------------------------------------
// <copyright file="PersistentCloudAnchorsController.cs" company="Google LLC">
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


#if UNITY_2022_3_OR_NEWER && !ARCORE_USE_ARF_5
// For AR Foundation 5.X compatibility, define the ARCORE_USE_ARF_5
// symbol, see https://docs.unity3d.com/Manual/CustomScriptingSymbols.html
// You can define ARCORE_USE_ARF_5 for Unity 2021.x or higher but you have
// to define it after 2022.x
#warning For AR Foundation 5.X compatibility, define the ARCORE_USE_ARF_5 symbol
#endif

namespace Google.XR.ARCoreExtensions.Samples.PersistentCloudAnchors
{
    using System;
    using System.Collections.Generic;
#if ARCORE_USE_ARF_5 // use ARF 5
    using Unity.XR.CoreUtils;
#endif
    using UnityEngine;
    using UnityEngine.XR.ARFoundation;

    /// <summary>
    /// Controller for Persistent Cloud Anchors sample.
    /// </summary>
    public class PersistentCloudAnchorsController : MonoBehaviour
    {
        [Header("AR Foundation")]

#if ARCORE_USE_ARF_5 // use ARF 5
        /// <summary>
        /// The active XROrigin used in the example.
        /// </summary>
        public XROrigin Origin;
#else // use ARF 4
        /// <summary>
        /// The active ARSessionOrigin used in the example.
        /// </summary>
        public ARSessionOrigin SessionOrigin;
#endif

        /// <summary>
        /// The ARSession used in the example.
        /// </summary>
        public ARSession SessionCore;

        /// <summary>
        /// The ARCoreExtensions used in the example.
        /// </summary>
        public ARCoreExtensions Extensions;

        /// <summary>
        /// The active ARAnchorManager used in the example.
        /// </summary>
        public ARAnchorManager AnchorManager;

        /// <summary>
        /// The active ARPlaneManager used in the example.
        /// </summary>
        public ARPlaneManager PlaneManager;

        /// <summary>
        /// The active ARRaycastManager used in the example.
        /// </summary>
        public ARRaycastManager RaycastManager;

        [Header("UI")]

        /// <summary>
        /// The home page to choose entering hosting or resolving work flow.
        /// </summary>
        public GameObject HomePage;

        /// <summary>
        /// The resolve screen that provides the options on which Cloud Anchors to be resolved.
        /// </summary>
        public GameObject ResolveMenu;

        /// <summary>
        /// The information screen that displays useful information about privacy prompt.
        /// </summary>
        public GameObject PrivacyPrompt;

        /// <summary>
        /// The AR screen which displays the AR view, hosts or resolves cloud anchors,
        /// and returns to home page.
        /// </summary>
        public GameObject ARView;

        /// <summary>
        /// The current application mode.
        /// </summary>
        [HideInInspector]
        public ApplicationMode Mode = ApplicationMode.Ready;

        /// <summary>
        /// A list of Cloud Anchors that will be used in resolving.
        /// </summary>
        public HashSet<string> ResolvingSet = new HashSet<string>();

        /// <summary>
        /// The key name used in PlayerPrefs which indicates whether the start info has displayed
        /// at least one time.
        /// </summary>
        private const string _hasDisplayedStartInfoKey = "HasDisplayedStartInfo";

        /// <summary>
        /// The key name used in PlayerPrefs which stores persistent Cloud Anchors history data.
        /// Expired data will be cleared at runtime.
        /// </summary>
        private const string _persistentCloudAnchorsStorageKey = "PersistentCloudAnchors";

        /// <summary>
        /// The limitation of how many Cloud Anchors can be stored in local storage.
        /// </summary>
        private const int _storageLimit = 40;

        /// <summary>
        /// Sample application modes.
        /// </summary>
        public enum ApplicationMode
        {
            /// <summary>
            /// Ready to host or resolve.
            /// </summary>
            Ready,

            /// <summary>
            /// Hosting Cloud Anchors.
            /// </summary>
            Hosting,

            /// <summary>
            /// Resolving Cloud Anchors.
            /// </summary>
            Resolving,
        }

        /// <summary>
        /// Gets the current main camera.
        /// </summary>
        public Camera MainCamera
        {
            get
            {
#if ARCORE_USE_ARF_5 // use ARF 5
                return Origin.Camera;
#else // use ARF 4
                return SessionOrigin.camera;
#endif
            }
        }

        /// <summary>
        /// Callback handling "Begin to host" button click event in Home Page.
        /// </summary>
        public void OnHostButtonClicked()
        {
            Mode = ApplicationMode.Hosting;
            SwitchToPrivacyPrompt();
        }

        /// <summary>
        /// Callback handling "Begin to resolve" button click event in Home Page.
        /// </summary>
        public void OnResolveButtonClicked()
        {
            Mode = ApplicationMode.Resolving;
            SwitchToResolveMenu();
        }

        /// <summary>
        /// Callback handling "Learn More" Button click event in Privacy Prompt.
        /// </summary>
        public void OnLearnMoreButtonClicked()
        {
            Application.OpenURL(
                "https://developers.google.com/ar/data-privacy");
        }

        /// <summary>
        /// Switch to home page, and disable all other screens.
        /// </summary>
        public void SwitchToHomePage()
        {
            ResetAllViews();
            Mode = ApplicationMode.Ready;
            ResolvingSet.Clear();
            HomePage.SetActive(true);
        }

        /// <summary>
        /// Switch to resolve menu, and disable all other screens.
        /// </summary>
        public void SwitchToResolveMenu()
        {
            ResetAllViews();
            ResolveMenu.SetActive(true);
        }

        /// <summary>
        /// Switch to privacy prompt, and disable all other screens.
        /// </summary>
        public void SwitchToPrivacyPrompt()
        {
            if (PlayerPrefs.HasKey(_hasDisplayedStartInfoKey))
            {
                SwitchToARView();
                return;
            }

            ResetAllViews();
            PrivacyPrompt.SetActive(true);
        }

        /// <summary>
        /// Switch to AR view, and disable all other screens.
        /// </summary>
        public void SwitchToARView()
        {
            ResetAllViews();
            PlayerPrefs.SetInt(_hasDisplayedStartInfoKey, 1);
            ARView.SetActive(true);
            SetPlatformActive(true);
        }

        /// <summary>
        /// Load the persistent Cloud Anchors history from local storage,
        /// also remove outdated records and update local history data. 
        /// </summary>
        /// <returns>A collection of persistent Cloud Anchors history data.</returns>
        public CloudAnchorHistoryCollection LoadCloudAnchorHistory()
        {
            if (PlayerPrefs.HasKey(_persistentCloudAnchorsStorageKey))
            {
                var history = JsonUtility.FromJson<CloudAnchorHistoryCollection>(
                    PlayerPrefs.GetString(_persistentCloudAnchorsStorageKey));

                // Remove all records created more than 24 hours and update stored history.
                DateTime current = DateTime.Now;
                history.Collection.RemoveAll(
                    data => current.Subtract(data.CreatedTime).Days > 0);
                PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey,
                    JsonUtility.ToJson(history));
                return history;
            }

            return new CloudAnchorHistoryCollection();
        }

        /// <summary>
        /// Save the persistent Cloud Anchors history to local storage,
        /// also remove the oldest data if current storage has met maximal capacity.
        /// </summary>
        /// <param name="data">The Cloud Anchor history data needs to be stored.</param>
        public void SaveCloudAnchorHistory(CloudAnchorHistory data)
        {
            var history = LoadCloudAnchorHistory();

            // Sort the data from latest record to oldest record which affects the option order in
            // multiselection dropdown.
            history.Collection.Add(data);
            history.Collection.Sort((left, right) => right.CreatedTime.CompareTo(left.CreatedTime));

            // Remove the oldest data if the capacity exceeds storage limit.
            if (history.Collection.Count > _storageLimit)
            {
                history.Collection.RemoveRange(
                    _storageLimit, history.Collection.Count - _storageLimit);
            }

            PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
        }

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Lock screen to portrait.
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.Portrait;

            // Enable Persistent Cloud Anchors sample to target 60fps camera capture frame rate
            // on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
            SwitchToHomePage();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            // On home page, pressing 'back' button quits the app.
            // Otherwise, returns to home page.
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (HomePage.activeSelf)
                {
                    Application.Quit();
                }
                else
                {
                    SwitchToHomePage();
                }
            }
        }

        private void ResetAllViews()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            SetPlatformActive(false);
            ARView.SetActive(false);
            PrivacyPrompt.SetActive(false);
            ResolveMenu.SetActive(false);
            HomePage.SetActive(false);
        }

        private void SetPlatformActive(bool active)
        {
#if ARCORE_USE_ARF_5 // use ARF 5
            Origin.gameObject.SetActive(active);
#else // use ARF 4
            SessionOrigin.gameObject.SetActive(active);
#endif
            SessionCore.gameObject.SetActive(active);
            Extensions.gameObject.SetActive(active);
        }
    }
}
