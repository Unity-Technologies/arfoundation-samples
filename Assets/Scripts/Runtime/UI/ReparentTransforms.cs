#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ReparentTransforms : MonoBehaviour
    {
        [SerializeField]
        Transform m_NewParent;

        [SerializeField]
        Transform m_CurrentParent;

        [SerializeField]
        Transform[] m_ElementsToReparent;

        public Transform newParent
        {
            get => m_NewParent;
            set => m_NewParent = value;
        }

        public Transform currentParent
        {
            get => m_CurrentParent;
            set => m_CurrentParent = value;
        }

        public Transform[] elementsToReparent
        {
            get => m_ElementsToReparent;
            set => m_ElementsToReparent = value;
        }

        public void SwapTransformParents()
        {
            if (m_ElementsToReparent.Length == 0)
                return;

            if (m_ElementsToReparent[0].parent == m_CurrentParent.transform)
                SetElementsParentTo(m_NewParent.transform);
            else
                SetElementsParentTo(m_CurrentParent.transform);
        }

        void SetElementsParentTo(Transform newParent)
        {
            for (int i = 0; i < m_ElementsToReparent.Length; i++)
            {
                m_ElementsToReparent[i].SetParent(newParent, false);
            }
        }

#if UNITY_EDITOR
        public void SwapTransformParentsInEditor()
        {
            if (m_ElementsToReparent.Length == 0)
                return;

            if (elementsToReparent[0].parent == currentParent.transform)
                SetElementsParentInEditorTo(m_NewParent.transform);
            else
                SetElementsParentInEditorTo(m_CurrentParent.transform);
        }

        void SetElementsParentInEditorTo(Transform newParent)
        {
            Undo.IncrementCurrentGroup();
            var undoIndexCheckpoint = Undo.GetCurrentGroup();
            for (int i = 0; i < elementsToReparent.Length; i++)
            {
                Undo.SetTransformParent(elementsToReparent[i], newParent, false, "Swap Transform Parents");
            }
            Undo.CollapseUndoOperations(undoIndexCheckpoint);
        }
#endif

        void Reset()
        {
            m_CurrentParent = GetComponent<Transform>();
        }
    }
}
