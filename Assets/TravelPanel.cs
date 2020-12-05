using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TravelPanel : MonoBehaviour
{
    public GameObject panel;
    
    private void Start()
    {
        panel = this.gameObject;
        
    }

    public void LoadScene(string locationName)
    {
        SceneManager.LoadScene(locationName);
    }

    public void ClosePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
