using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Close : MonoBehaviour
{

    private Button thisButton;
    void Start()
    {
        thisButton = gameObject.GetComponent<Button>();
        if(thisButton != null)
        {
            thisButton.onClick.AddListener(closeApp);
        }
    }

    void closeApp()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

}
