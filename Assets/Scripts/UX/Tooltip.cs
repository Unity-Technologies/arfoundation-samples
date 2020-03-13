using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject toolTip;
    bool _enteredButton;
    Vector3 _toolTipOffset;
    // Start is called before the first frame update
    void Start()
    {
        _toolTipOffset = new Vector3(-50,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if(_enteredButton){
              toolTip.transform.position = Input.mousePosition + _toolTipOffset;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _enteredButton = true;
        if(!gameObject.GetComponent<Button>().interactable)
        {
             toolTip.SetActive(true);
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        _enteredButton = false;
        toolTip.SetActive(false);
    }
}
