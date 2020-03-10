using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public GameObject backButton;
    // Start is called before the first frame update
    void Start()
    {
         if (Application.CanStreamedLevelBeLoaded("Menu"))
         {
            backButton.SetActive(true);
         }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButtonPressed()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
