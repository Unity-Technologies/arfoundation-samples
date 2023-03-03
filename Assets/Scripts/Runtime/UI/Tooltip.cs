using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        GameObject m_Tooltip;
        public GameObject toolTip
        {
            get { return m_Tooltip; }
            set { m_Tooltip = value; }
        }
        bool m_EnteredButton;

        void Update()
        {
            if(m_EnteredButton)
            {
                m_Tooltip.transform.position = transform.position;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_EnteredButton = true;
            if(!gameObject.GetComponent<Button>().interactable)
            {
                m_Tooltip.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_EnteredButton = false;
            m_Tooltip.SetActive(false);
        }
    }
}
