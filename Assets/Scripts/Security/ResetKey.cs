using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetKey : MonoBehaviour
{
    public Button resetKey;


    void Start()
    {
        resetKey.onClick.AddListener(CleanKey);
    }

    protected void CleanKey()
    {
        PlayerPrefs.SetString("RequestCode", "");
        PlayerPrefs.SetString("MachineID", "");
    }

  }
