//-----------------------------------------------------------------------
// <copyright file="DoubleLabelsItem.cs" company="Google LLC">
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
    using UnityEngine.UI;

    /// <summary>
    /// A helper component used by <see cref="MultiselectionDropdown"/> on displaying option data.
    /// </summary>
    public class DoubleLabelsItem : MonoBehaviour
    {
        /// <summary>
        /// The Text component of the first label, used to display the
        /// <see cref="MultiselectionDropdown.OptionData.MajorInfo"/>.
        /// </summary>
        public Text FirstLabel;

        /// <summary>
        /// The Text component for the second label, used to display the
        /// <see cref="MultiselectionDropdown.OptionData.MinorInfo"/>.
        /// </summary>
        public Text SecondLabel;

        /// <summary>
        /// Set the contents of two labels.
        /// </summary>
        /// <param name="first">The content for first label.</param>
        /// <param name="second">The content for second label.</param>
        public void SetLabels(string first, string second)
        {
            if (FirstLabel != null)
            {
                FirstLabel.text = first;
            }

            if (SecondLabel != null)
            {
                SecondLabel.text = second;
            }
        }
    }
}
