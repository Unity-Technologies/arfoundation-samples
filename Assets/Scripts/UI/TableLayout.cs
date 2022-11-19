using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(RectTransform))]
    public class TableLayout : MonoBehaviour
    {
        enum Alignment { Left, Center }

        RectTransform m_RectTransform;

        [SerializeField]
        List<RectTransform> m_Cells = new();

        [SerializeField]
        Alignment m_Alignment = Alignment.Center;

        [SerializeField, Range(1, 5)]
        int m_Columns = 3;

        [SerializeField, Range(1, 1250)]
        int m_CellWidth = 340;

        [SerializeField, Range(1, 1250)]
        int m_CellHeight = 115;

        [SerializeField, Range(1, 400)]
        int m_CellPadding = 30;

        void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += Refresh;
#endif
        }

        void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += Refresh;
#endif
        }

        void Awake()
        {
            Refresh();
        }

        void Refresh()
        {
            // Can happen if this GameObject was marked for destroy before the Editor delay call executes
            if (this == null)
                return;

            if (m_RectTransform == null)
               m_RectTransform = GetComponent<RectTransform>();

            ResetCells();
            RegenerateLayout();
        }

        void ResetCells()
        {
            foreach (RectTransform cell in m_Cells.Where(cell => cell != null))
            {
                cell.localScale = Vector3.one;
                cell.SetWidth(m_CellWidth);
                cell.SetHeight(m_CellHeight);
            }
        }

        void RegenerateLayout()
        {
            var rect = m_RectTransform.rect;
            var xCenter = rect.xMin + rect.width / 2;
            for (var i = 0; i < Mathf.CeilToInt((float)m_Cells.Count / m_Columns); i++)
            {
                for (var j = 0; j < m_Columns; j++)
                {
                    var index = i * m_Columns + j;
                    if (index > m_Cells.Count - 1)
                        break;

                    var cell = m_Cells[index];

                    float xLeft = 0;
                    switch (m_Alignment)
                    {
                        case Alignment.Left:
                            xLeft = rect.xMin;
                            break;
                        case Alignment.Center when m_Columns % 2 == 0:
                            xLeft =
                                xCenter
                                - (m_Columns / 2 * m_CellWidth) // subtract cells left of center
                                - Mathf.FloorToInt((m_Columns / 2 - .5f) * m_CellPadding); // subtract padding
                            break;
                        case Alignment.Center:
                            xLeft =
                                xCenter
                                - (m_Columns / 2 * m_CellPadding) // subtract padding left of center
                                - Mathf.FloorToInt((m_Columns / 2 + .5f) * m_CellWidth); // subtract cells
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    cell.AnchorToTopCenter();
                    cell.SetTopLeftPosition(new Vector2(
                        xLeft + j * (m_CellWidth + m_CellPadding), 
                        rect.yMax - i * (m_CellHeight + m_CellPadding)));
                }
            }
        }
    }
}
