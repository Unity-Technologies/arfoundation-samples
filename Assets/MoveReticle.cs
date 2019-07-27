using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveReticle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 ancvecc = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(ancvecc.x + 1, ancvecc.y + 1);
        }
    }
}
