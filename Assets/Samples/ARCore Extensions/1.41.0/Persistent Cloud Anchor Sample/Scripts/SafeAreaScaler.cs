// <copyright file="SafeAreaScaler.cs" company="Google LLC">
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
    /// A helper component that scale the UI rect to the same size as the safe area.
    /// </summary>
    public class SafeAreaScaler : MonoBehaviour
    {
        private Rect _screenSafeArea = new Rect(0, 0, 0, 0);

        /// <summary>
        /// Unity's Awake() method.
        /// </summary>
        public void Update()
        {
            Rect safeArea;
            safeArea = Screen.safeArea;

            if (_screenSafeArea != safeArea)
            {
                _screenSafeArea = safeArea;
                MatchRectTransformToSafeArea();
            }
        }

        private void MatchRectTransformToSafeArea()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            // lower left corner offset
            Vector2 offsetMin = new Vector2(_screenSafeArea.xMin,
                Screen.height - _screenSafeArea.yMax);

            // upper right corner offset
            Vector2 offsetMax = new Vector2(
                _screenSafeArea.xMax - Screen.width,
                -_screenSafeArea.yMin);

            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }
    }
}
