//-----------------------------------------------------------------------
// <copyright file="MapQualityBar.cs" company="Google LLC">
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
    /// An animated bar for visualizing the map quality status in real time.
    /// </summary>
    public class MapQualityBar : MonoBehaviour
    {
        /// <summary>
        /// The animatior of this map quality bar which is used for state update.
        /// </summary>
        public Animator Animator;

        /// <summary>
        /// The renderer of this map quality bar which is used for color changing animation.
        /// </summary>
        public Renderer Renderer;

        /// <summary>
        /// The color that indicates initial state.
        /// </summary>
        public Color InitialColor = Color.white;

        /// <summary>
        /// The color that indicates low quality state.
        /// </summary>
        public Color LowQualityColor = Color.red;

        /// <summary>
        /// The color that indicates medium quality state.
        /// </summary>
        public Color MediumQualityColor = Color.yellow;

        /// <summary>
        /// The color that indicates high quality state.
        /// </summary>
        public Color HighQualityColor = Color.green;

        // Variable name string used for changing material color.
        private const string _varColor = "_Color";

        // Hash values for variables defined by MapQualityBarAnimator.
        private static readonly int _paramQuality = Animator.StringToHash("Quality");
        private static readonly int _paramIsVisited = Animator.StringToHash("IsVisited");
        private static readonly int _paramColorCurve = Animator.StringToHash("ColorCurve");

        // Hash values for states defined by MapQualityBarAnimator.
        private static readonly int _stateLow = Animator.StringToHash("Base Layer.Low");
        private static readonly int _stateMedium = Animator.StringToHash("Base Layer.Medium");
        private static readonly int _stateHigh = Animator.StringToHash("Base Layer.High");

        private bool _isVisited = false;
        private int _state = 0;
        private float _alpha = 1.0f;

        /// <summary>
        /// Gets or sets a value indicating whether this map quality bar has been visited.
        /// </summary>
        public bool IsVisited
        {
            get
            {
                return _isVisited;
            }

            set
            {
                _isVisited = value;
                Animator.SetBool(_paramIsVisited, _isVisited);
            }
        }

        /// <summary>
        /// Gets or sets the quality state of this map quality bar.
        /// </summary>
        public int QualityState
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
                Animator.SetInteger(_paramQuality, _state);
            }
        }

        /// <summary>
        /// Gets the weight of this map quality bar based on it's quality state,
        /// range in [0.0f, 1.0f].
        /// </summary>
        public float Weight
        {
            get
            {
                if (IsVisited)
                {
                    switch (_state)
                    {
                        case 0: return 0.1f; // Insufficient quality
                        case 1: return 0.5f; // Sufficient quality
                        case 2: return 1.0f; // Good quality
                        default: return 0.0f;
                    }
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        /// <summary>
        /// Set the alpha value for this bar.
        /// </summary>
        /// <param name="alpha">Alpha value of this bar.</param>
        public void SetAlpha(float alpha)
        {
            _alpha = alpha;
        }

        /// <summary>
        /// Unity's Update().
        /// </summary>
        public void Update()
        {
            // Sync map quality bar color with current state animation.
            var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            float colorCurve = Animator.GetFloat(_paramColorCurve);
            Color color = InitialColor;
            if (stateInfo.fullPathHash == _stateLow)
            {
                color = Color.Lerp(InitialColor, LowQualityColor, colorCurve);
            }
            else if (stateInfo.fullPathHash == _stateMedium)
            {
                color = Color.Lerp(LowQualityColor, MediumQualityColor, colorCurve);
            }
            else if (stateInfo.fullPathHash == _stateHigh)
            {
                color = Color.Lerp(MediumQualityColor, HighQualityColor, colorCurve);
            }

            color.a = _alpha;
            Renderer.material.SetColor(_varColor, color);
        }
    }
}
