using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Button m_Template = null;
    
    [SerializeField]
    private Transform m_ButtonParent = null;
    
    [SerializeField]
    private Toggle m_MenuToggle = null;

    [SerializeField] [HideInInspector]
    private string[] m_Scenes = null;

    // make sure our scene list is up-to-date
    void OnValidate() {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
         
        m_Scenes = new string[sceneCount];

        // skip scene 0 - that's the SceneLoader scene
        for( int i = 1; i < sceneCount; i++ )
        {
            m_Scenes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
    }

    void OnEnable()
    {
        // make sure the menu stays around
        DontDestroyOnLoad(this.gameObject);

        m_Template.gameObject.SetActive(false);

        // create buttons for all scenes that we want to be able to load at runtime
        for(int i = 1; i < m_Scenes.Length; i++)
        {
            var sceneLoadButton = Instantiate(m_Template, m_ButtonParent);
            sceneLoadButton.gameObject.SetActive(true);
            sceneLoadButton.GetComponentInChildren<Text>().text = m_Scenes[i];
            sceneLoadButton.gameObject.name = m_Scenes[i];
            
            // when a scene is opened, we want to load it and disable the menu again
            var k = i;
            sceneLoadButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(k, LoadSceneMode.Single);
                m_MenuToggle.isOn = false;
            });
        }
    }
}
