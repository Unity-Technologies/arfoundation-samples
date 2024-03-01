//-----------------------------------------------------------------------
// <copyright file="MultiselectionDropdown.cs" company="Google LLC">
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
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// A custom dropdown that presents a list of options when clicked,
    /// from which multiple items can be selected.
    /// Note: each item can have two labels at most, it uses <see cref="DoubleLabelsItem"/>
    /// to set the contents.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MultiselectionDropdown : Selectable,
        IPointerClickHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {
        /// <summary>
        /// The game object of the template for the multiselection dropdown list.
        /// The initial content height is used as the maximal height of the dropdown list.
        /// </summary>
        public GameObject OptionRect;

        /// <summary>
        /// The heading text prefab. If present, it will be placed at the top of the dropdown list.
        /// Note: If the click click event is not handled, clicking on this heading text will also
        /// close the dropdown list.
        /// </summary>
        public GameObject HeadingTextPrefab;

        /// <summary>
        /// The multiselection item prefab which is used to populate the dropdown options.
        /// <see cref="DoubleLabelsItem"/> and <see cref="Toggle"/> components should be used,
        /// so that each option item can be properly presented.
        /// </summary>
        public GameObject MultiselectionItemPrefab;

        /// <summary>
        /// The Text component to hold the text of the currently selected option.
        /// It automatically updates selected options, and displays "Select" when
        /// nothing is selected.
        /// </summary>
        public Text CaptionText;

        /// <summary>
        /// The text limit for caption text label. If selected options exceed this limit,
        /// it will show with ellipsis.
        /// </summary>
        public int TextLimit = 20;

        /// <summary>
        /// The callback action which is called when the selection is changed.
        /// </summary>
        public Action OnValueChanged;

        private float _itemHeight;
        private float _maxHeight;
        private bool _optionChanged = true;

        private List<OptionData> _options = new List<OptionData>();
        private List<Toggle> _optionToggles = new List<Toggle>();

        /// <summary>
        /// Gets or sets the list of multiselection dropdown options.
        /// </summary>
        public List<OptionData> Options
        {
            get
            {
                return _options;
            }

            set
            {
                if (CaptionText != null)
                {
                    CaptionText.text = "Select";
                }

                _optionToggles.Clear();
                _optionChanged = true;
                _options = value;
                if (_options.Count == 0)
                {
                    CaptionText.text = "No option available";
                }
            }
        }

        /// <summary>
        /// Gets the list of the index that represents current selected options.
        /// </summary>
        public List<int> SelectedValues
        {
            get
            {
                List<int> index = new List<int>();
                for (int i = 0; i < _optionToggles.Count; i++)
                {
                    if (_optionToggles[i].isOn)
                    {
                        index.Add(i);
                    }
                }

                return index;
            }
        }

        /// <summary>
        /// Deselect this multiselection dropdown.
        /// </summary>
        public void Deselect()
        {
            if (OptionRect.activeSelf)
            {
                OptionRect.SetActive(false);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            bool currentActive = OptionRect.activeSelf;
            if (!currentActive)
            {
                // Display the dropdown options.
                UpdateOptionRect();
                OptionRect.SetActive(!currentActive);
            }
            else
            {
                // Do not close the dropdown options if it clicks on a single option.
                if (eventData.pointerCurrentRaycast.gameObject.GetHashCode() !=
                    OptionRect.GetHashCode())
                {
                    OptionRect.SetActive(false);
                }
            }
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (OnValueChanged != null)
            {
                OnValueChanged();
            }

            Deselect();
        }

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            Deselect();
        }

        /// <summary>
        /// Unity's Awake() Method.
        /// </summary>
        protected override void Awake()
        {
            _maxHeight = OptionRect.GetComponent<RectTransform>().rect.height;
            _itemHeight = OptionRect.GetComponent<ScrollRect>()
                .content.GetComponent<RectTransform>().rect.height;
            _optionChanged = true;
        }

        private void OnSelectionChanged(bool isSelected)
        {
            // Fetch selected options.
            List<string> selectedOptions = new List<string>();
            foreach (var toggle in _optionToggles)
            {
                if (toggle.isOn)
                {
                    selectedOptions.Add(toggle.GetComponent<DoubleLabelsItem>().FirstLabel.text);
                }
            }

            // Update captain text.
            if (CaptionText != null && _options.Count > 0)
            {
                if (selectedOptions.Count == 0)
                {
                    CaptionText.text = "Select";
                }
                else
                {
                    string combined = string.Join(",", selectedOptions.ToArray());
                    if (TextLimit > 0 && combined.Length > TextLimit)
                    {
                        combined = combined.Substring(0, TextLimit) + "...";
                    }

                    CaptionText.text = combined;
                }
            }

            // Broadcast OnValueChanged event.
            if (OnValueChanged != null)
            {
                OnValueChanged();
            }
        }

        private void UpdateOptionRect()
        {
            if (!_optionChanged)
            {
                return;
            }

            RectTransform optionRect = OptionRect.GetComponent<RectTransform>();
            RectTransform contentRect = OptionRect.GetComponent<ScrollRect>().content;

            _optionToggles.Clear();
            contentRect.transform.DetachChildren();

            int count = 0;
            if (HeadingTextPrefab != null && _options.Count > 0)
            {
                GameObject headingText = Instantiate(HeadingTextPrefab);
                headingText.transform.SetParent(contentRect.transform, false);
                count++;
            }

            foreach (var optionData in Options)
            {
                GameObject selectableItem = Instantiate(MultiselectionItemPrefab);
                selectableItem.transform.SetParent(contentRect.transform, false);
                selectableItem.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, -(_itemHeight * count));

                selectableItem.GetComponent<DoubleLabelsItem>()
                    .SetLabels(optionData.MajorInfo, optionData.MinorInfo);

                var toggle = selectableItem.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnSelectionChanged);
                _optionToggles.Add(toggle);

                count++;
            }

            optionRect.sizeDelta =
                new Vector2(optionRect.sizeDelta.x, Mathf.Min(count * _itemHeight, _maxHeight));
            contentRect.sizeDelta =
                new Vector2(contentRect.sizeDelta.x, count * _itemHeight);
            CaptionText.text = _options.Count == 0 ? "No option available" : "Select";
            _optionChanged = false;
        }

        /// <summary>
        /// Class to store the text for a single option in the multiselection dropdown list.
        /// </summary>
        [Serializable]
        public class OptionData
        {
            /// <summary>
            /// The major information of this option which will be displayed in
            /// <see cref="DoubleLabelsItem.FirstLabel"/>.
            /// </summary>
            [SerializeField]
            public string MajorInfo;

            /// <summary>
            /// The minor information of this option which will be displayed in
            /// <see cref="DoubleLabelsItem.SecondLabel"/>.
            /// </summary>
            [SerializeField]
            public string MinorInfo;

            /// <summary>
            /// The constructor of an OptionData.
            /// </summary>
            /// <param name="major">The major information of this option.</param>
            /// <param name="minor">The minor information of this option.</param>
            public OptionData(string major, string minor)
            {
                MajorInfo = major;
                MinorInfo = minor;
            }
        }
    }
}
