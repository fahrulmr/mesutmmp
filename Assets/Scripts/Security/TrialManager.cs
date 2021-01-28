using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TrialManager : MonoBehaviour
{
    public GameObject errorTextGO;
    public TMP_Text errortext;
    public Button trial;


    void Start()
    {
        trial.onClick.AddListener(ActivateTrial);
    }

    private void ActivateTrial()
    {
        if (PlayerPrefs.GetString("RequestCode") == "FirstTrialKey")
        {
            if (PlayerPrefs.GetInt("TrialNumberValue") <= 0)
            {
                errortext.text = "Trial Expired";
                errorTextGO.SetActive(true);
            }
            else if (PlayerPrefs.GetInt("TrialNumberValue") > 0)
            {
                PlayerPrefs.SetInt("TrialNumberValue", PlayerPrefs.GetInt("TrialNumberValue") - 1);
                SceneManager.LoadSceneAsync("Main");
            }
        }
        else if (PlayerPrefs.GetString("TrialActivated") == "true")
        {
            errortext.text = "Trial Expired";
            errorTextGO.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetString("RequestCode", "FirstTrialKey");
            PlayerPrefs.SetInt("TrialNumberValue", 5);
            PlayerPrefs.SetString("TrialActivated", "true");
            SceneManager.LoadSceneAsync("Main");
        }
    }

}
