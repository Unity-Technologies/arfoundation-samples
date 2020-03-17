using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public GameObject backButton;
    
    void Start()
    {
         if (Application.CanStreamedLevelBeLoaded("Menu"))
         {
            backButton.SetActive(true);
         }
    }

    public void BackButtonPressed()
    {   
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
