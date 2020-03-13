using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public GameObject backButton;
    
    /*
    public ARSceneSelectUI menuManager;
    public bool faceMenu;
    public bool lightMenu;
    public bool humanSegmentationMenu;
    public bool planeMenu;*/
    
    // Start is called before the first frame update
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
