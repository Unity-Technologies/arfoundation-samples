//-----------------------------------------------------------------------
// <copyright file="ResolveMenuManager.cs" company="Google LLC">
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
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A manager component that helps to populate and handle the options of resolving anchors.
    /// </summary>
    public class ResolveMenuManager : MonoBehaviour
    {
        /// <summary>
        /// The main controller for Persistent Cloud Anchors sample.
        /// </summary>
        public PersistentCloudAnchorsController Controller;

        /// <summary>
        /// A multiselection dropdown component that contains all available resolving options.
        /// </summary>
        public MultiselectionDropdown Multiselection;

        /// <summary>
        /// An input field for manually typing Cloud Anchor Id(s).
        /// </summary>
        public InputField InputField;

        /// <summary>
        /// The warning text that appears when invalid characters are filled in.
        /// </summary>
        public GameObject InvalidInputWarning;

        /// <summary>
        /// The resolve button which leads to AR view screen.
        /// </summary>
        public Button ResolveButton;

        /// <summary>
        /// Cached Cloud Anchor history data used to fetch the Cloud Anchor Id using
        /// the index given by multi-selection dropdown.
        /// </summary>
        private CloudAnchorHistoryCollection _history = new CloudAnchorHistoryCollection();

        /// <summary>
        /// Cached active color for interactable buttons.
        /// </summary>
        private Color _activeColor;

        /// <summary>
        /// Callback handling the validation of the input field.
        /// </summary>
        /// <param name="inputString">Current value of the input field.</param>
        public void OnInputFieldValueChanged(string inputString)
        {
            // Input should only contain:
            // letters, numbers, hyphen(-), underscore(_), and comma(,).
            // Note: the valid character set is controlled by the validation rule of
            // the naming field in AR View.
            var regex = new Regex("^[a-zA-Z0-9-_,]*$");
            InvalidInputWarning.SetActive(!regex.IsMatch(inputString));
        }

        /// <summary>
        /// Callback handling the end edit event of the input field.
        /// </summary>
        /// <param name="inputString">The value of the input field.</param>
        public void OnInputFieldEndEdit(string inputString)
        {
            if (InvalidInputWarning.activeSelf)
            {
                return;
            }

            OnResolvingSelectionChanged();
        }

        /// <summary>
        /// Callback handling the selection values changed in multiselection dropdown and
        /// input field.
        /// </summary>
        public void OnResolvingSelectionChanged()
        {
            Controller.ResolvingSet.Clear();

            // Add Cloud Anchor Ids from multiselection dropdown.
            List<int> selectedIndex = Multiselection.SelectedValues;
            if (selectedIndex.Count > 0)
            {
                foreach (int index in selectedIndex)
                {
                    Controller.ResolvingSet.Add(_history.Collection[index].Id);
                }
            }

            // Add Cloud Anchor Ids from input field.
            if (!InvalidInputWarning.activeSelf && InputField.text.Length > 0)
            {
                string[] inputIds = InputField.text.Split(',');
                if (inputIds.Length > 0)
                {
                    Controller.ResolvingSet.UnionWith(inputIds);
                }
            }

            // Update resolve button.
            SetButtonActive(ResolveButton, Controller.ResolvingSet.Count > 0);
        }

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            _activeColor = ResolveButton.GetComponent<Image>().color;
        }

        /// <summary>
        /// The Unity OnEnable() method.
        /// </summary>
        public void OnEnable()
        {
            SetButtonActive(ResolveButton, false);
            InvalidInputWarning.SetActive(false);
            InputField.text = string.Empty;
            _history = Controller.LoadCloudAnchorHistory();

            Multiselection.OnValueChanged += OnResolvingSelectionChanged;
            var options = new List<MultiselectionDropdown.OptionData>();
            foreach (var data in _history.Collection)
            {
                options.Add(new MultiselectionDropdown.OptionData(
                    data.Name, FormatDateTime(data.CreatedTime)));
            }

            Multiselection.Options = options;
        }

        /// <summary>
        /// The Unity OnDisable() method.
        /// </summary>
        public void OnDisable()
        {
            Multiselection.OnValueChanged -= OnResolvingSelectionChanged;
            Multiselection.Deselect();
            Multiselection.Options.Clear();
            _history.Collection.Clear();
        }

        private string FormatDateTime(DateTime time)
        {
            TimeSpan span = DateTime.Now.Subtract(time);
            return span.Hours == 0 ? span.Minutes == 0 ? "Just now" :
                string.Format("{0}m ago", span.Minutes) : string.Format("{0}h ago", span.Hours);
        }

        private void SetButtonActive(Button button, bool active)
        {
            button.GetComponent<Image>().color = active ? _activeColor : Color.grey;
            button.enabled = active;
        }
    }
}
