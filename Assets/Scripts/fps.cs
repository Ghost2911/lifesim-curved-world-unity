using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class fps : MonoBehaviour
{
    Text txtFPS;
    void Start()
    {
        txtFPS = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        txtFPS.text = (1.0f / Time.deltaTime).ToString()+" fps";
    }
}
