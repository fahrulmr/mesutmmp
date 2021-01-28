using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class KeyEvaluationSystem : MonoBehaviour
{
    public GameObject errorTextGO;
    public TMP_Text errortext;
    public GameObject loading;


    private void Start()
    {
        if (PlayerPrefs.GetString("RequestCode") != null && PlayerPrefs.GetString("RequestCode") != "" && PlayerPrefs.GetString("MachineID") != null && PlayerPrefs.GetString("MachineID") != "" && PlayerPrefs.GetString("MachineID") == SystemInfo.deviceUniqueIdentifier)
        {
            loading.SetActive(true);
            SceneManager.LoadSceneAsync("Main");
        }
        else if (PlayerPrefs.GetString("RequestCode") == "FirstTrialKey")
        {

            if (PlayerPrefs.GetInt("TrialNumberValue") <= 0)
            {
                errortext.text = "Trial Expired";
                errorTextGO.SetActive(true);
            } else if (PlayerPrefs.GetInt("TrialNumberValue") > 0)
            {
                PlayerPrefs.SetInt("TrialNumberValue", PlayerPrefs.GetInt("TrialNumberValue")-1);
                loading.SetActive(true);
                SceneManager.LoadSceneAsync("Main");
            }
        }
    }

}
