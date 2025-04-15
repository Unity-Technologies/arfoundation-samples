using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard
{
    /// <summary>
    /// This script is used to optimize the keyboard rendering performance by updating the canvas hierarchy
    /// into separate parent transforms based on UI Grouping. This will greatly reduce the number of draw calls.
    /// Optimization is only done at runtime to prevent breaking the prefab.
    /// </summary>
    public class KeyboardOptimizer : MonoBehaviour
    {
        [SerializeField]
        bool m_OptimizeOnStart = true;

        /// <summary>
        /// If enabled, the optimization will be called on <see cref="Start"/>.
        /// </summary>
        public bool optimizeOnStart
        {
            get => m_OptimizeOnStart;
            set => m_OptimizeOnStart = value;
        }

        [SerializeField]
        Transform m_BatchGroupParentTransform;

        /// <summary>
        /// The parent transform for batch groups.
        /// </summary>
        public Transform batchGroupParentTransform
        {
            get => m_BatchGroupParentTransform;
            set => m_BatchGroupParentTransform = value;
        }

        [SerializeField]
        Transform m_ButtonParentTransform;

        /// <summary>
        /// The parent transform for buttons.
        /// </summary>
        public Transform buttonParentTransform
        {
            get => m_ButtonParentTransform;
            set => m_ButtonParentTransform = value;
        }

        [SerializeField]
        Transform m_ImageParentTransform;

        /// <summary>
        /// The parent transform for images.
        /// </summary>
        public Transform imageParentTransform
        {
            get => m_ImageParentTransform;
            set => m_ImageParentTransform = value;
        }

        [SerializeField]
        Transform m_TextParentTransform;

        /// <summary>
        /// The parent transform for text elements.
        /// </summary>
        public Transform textParentTransform
        {
            get => m_TextParentTransform;
            set => m_TextParentTransform = value;
        }

        [SerializeField]
        Transform m_IconParentTransform;

        /// <summary>
        /// The parent transform for icons.
        /// </summary>
        public Transform iconParentTransform
        {
            get => m_IconParentTransform;
            set => m_IconParentTransform = value;
        }

        [SerializeField]
        Transform m_HighlightParentTransform;

        /// <summary>
        /// The parent transform for highlights.
        /// </summary>
        public Transform highlightParentTransform
        {
            get => m_HighlightParentTransform;
            set => m_HighlightParentTransform = value;
        }

        bool m_IsCurrentlyOptimized;

        /// <summary>
        /// Is the keyboard currently optimized?
        /// </summary>
        public bool isCurrentlyOptimized => m_IsCurrentlyOptimized;

        /// <summary>
        /// Horizontal layout groups need to be disabled when optimizing the keyboard
        /// otherwise the input field will not position correctly.
        /// </summary>
        HorizontalLayoutGroup[] m_LayoutGroups;

        /// <summary>
        /// List of key data. This is used to store information that allows us
        /// to revert the keyboard back to its original state (aka unoptimize).
        /// </summary>
        readonly List<KeyData> m_KeyData = new List<KeyData>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
            CheckReferences();
            Canvas.ForceUpdateCanvases();

            if (m_OptimizeOnStart)
                Optimize();
        }

        /// <summary>
        /// Check all the references needed for optimization.
        /// </summary>
        void CheckReferences()
        {
            if (!TryGetOrCreateTransformReferences())
            {
                Debug.LogError("Failed to get or create transform references. Optimization will not be possible.", this);
                return;
            }

            if (m_KeyData.Count == 0)
                GetKeys();

            if (m_LayoutGroups == null || m_LayoutGroups.Length == 0)
                GetLayoutGroups();
        }

        bool TryGetOrCreateTransformReferences()
        {
            if (m_BatchGroupParentTransform == null)
            {
                var canvasComponent = GetComponentInChildren<Canvas>(true);
                if (canvasComponent == null)
                {
                    Debug.LogError("No Canvas component found in hierarchy. Optimization will not be possible.", this);
                    return false;
                }

                m_BatchGroupParentTransform = CreateTransformAndSetParent("BatchingGroup", canvasComponent.transform);
            }

            if (m_ButtonParentTransform == null)
                m_ButtonParentTransform = CreateTransformAndSetParent("Buttons", m_BatchGroupParentTransform);

            if (m_ImageParentTransform == null)
                m_ImageParentTransform = CreateTransformAndSetParent("Images", m_BatchGroupParentTransform);

            if (m_TextParentTransform == null)
                m_TextParentTransform = CreateTransformAndSetParent("Text", m_BatchGroupParentTransform);

            if (m_IconParentTransform == null)
                m_IconParentTransform = CreateTransformAndSetParent("Icons", m_BatchGroupParentTransform);

            if (m_HighlightParentTransform == null)
                m_HighlightParentTransform = CreateTransformAndSetParent("Highlights", m_BatchGroupParentTransform);

            return true;
        }

        void GetKeys()
        {
            XRKeyboardKey[] keys = GetComponentsInChildren<XRKeyboardKey>();
            foreach (var keyboardKey in keys)
            {
                m_KeyData.Add(new KeyData
                {
                    key = keyboardKey,
                    batchFollow = keyboardKey.GetComponent<KeyboardBatchFollow>(),
                    parent = keyboardKey.transform.parent,
                    childPosition = keyboardKey.transform.GetSiblingIndex(),
                });
            }
        }

        void GetLayoutGroups()
        {
            m_LayoutGroups = GetComponentsInChildren<HorizontalLayoutGroup>();
        }

        static Transform CreateTransformAndSetParent(string name, Transform parent)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent);
            t.SetLocalPose(Pose.identity);
            t.localScale = Vector3.one;

            return t;
        }

        /// <summary>
        /// Optimize the keyboard. This will set all the different components of each keyboard key into separate parent transforms for batching.
        /// </summary>
        public void Optimize()
        {
            m_IsCurrentlyOptimized = true;
            foreach (var layoutGroup in m_LayoutGroups)
            {
                layoutGroup.enabled = false;
            }

            foreach (var keyData in m_KeyData)
            {
                var key = keyData.key;
                if (key == null)
                    continue;
                key.transform.SetParent(m_ButtonParentTransform);

                if (key.targetGraphic != null)
                    key.targetGraphic.transform.SetParent(m_ImageParentTransform);

                if (key.textComponent != null)
                    key.textComponent.transform.SetParent(m_TextParentTransform);

                if (key.iconComponent != null)
                    key.iconComponent.transform.SetParent(m_IconParentTransform);

                if (key.highlightComponent != null)
                    key.highlightComponent.transform.SetParent(m_HighlightParentTransform);

                if (keyData.batchFollow != null)
                    keyData.batchFollow.enabled = true;
            }
        }

        /// <summary>
        /// Unoptimize the keyboard. This will set the keyboard back to its original state.
        /// </summary>
        public void Unoptimize()
        {
            m_IsCurrentlyOptimized = false;
            foreach (var layoutGroup in GetComponentsInChildren<HorizontalLayoutGroup>())
            {
                layoutGroup.enabled = true;
            }

            foreach (var keyData in m_KeyData)
            {
                XRKeyboardKey key = keyData.key;
                if (key == null)
                    continue;

                // NOTE: Order of objects setting their parent is important for sorting order.
                key.transform.SetParent(keyData.parent);
                key.transform.SetSiblingIndex(keyData.childPosition);

                if (key.targetGraphic != null)
                    key.targetGraphic.transform.SetParent(key.transform);

                if (key.textComponent != null)
                    key.textComponent.transform.SetParent(key.targetGraphic.transform);

                if (key.iconComponent != null)
                    key.iconComponent.transform.SetParent(key.targetGraphic.transform);

                if (key.highlightComponent != null)
                    key.highlightComponent.transform.SetParent(key.targetGraphic.transform);

                if (keyData.batchFollow != null)
                    keyData.batchFollow.enabled = false;
            }
        }

        struct KeyData
        {
            public XRKeyboardKey key;
            public KeyboardBatchFollow batchFollow;
            public Transform parent;
            public int childPosition;
        }
    }
}
