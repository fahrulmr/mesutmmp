using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialWatermark : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("RequestCode") != "FirstTrialKey" && PlayerPrefs.GetString("RequestCode") != "")
        {
            this.gameObject.SetActive(false);
        }
    }


}
