using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Text;

public class KeyGenerator : MonoBehaviour
{

    public TMP_Text Error;
    public TMP_InputField BusinessName;
    public TMP_InputField MaxNumberOfActivations;
    public TMP_InputField GeneratedKey;
    public Toggle LimitedActivations;
    public Toggle UnlimitedActivations;
    public Button CreateKey;

    public GameObject ErrorGO;
    public GameObject WaitGO;
    public GameObject MaxNumberOfActivationsGO;

    void Start()
    {
        LimitedActivations.onValueChanged.AddListener(delegate {
            LimitedActivationChanged();
        });
        UnlimitedActivations.onValueChanged.AddListener(delegate {
            UnlimitedActivationChanged();
        });

        CreateKey.onClick.AddListener(CreateCloudKey);

    }

    private void UnlimitedActivationChanged()
    {
        if (UnlimitedActivations.isOn)
        {
            LimitedActivations.isOn = false;
            MaxNumberOfActivationsGO.SetActive(false);
        }
        else
        {
            MaxNumberOfActivationsGO.SetActive(true);
            LimitedActivations.isOn = true;
        }
    }

    private void LimitedActivationChanged()
    {
        if (LimitedActivations.isOn)
        {
            UnlimitedActivations.isOn = false;
            MaxNumberOfActivationsGO.SetActive(true);
        } else
        {
            MaxNumberOfActivationsGO.SetActive(false);
            UnlimitedActivations.isOn = true;
        }
    }

    void CreateCloudKey()
    {
        Debug.Log("Creating Key!");
        WaitGO.SetActive(true);
        ErrorGO.SetActive(false);
        string businessName = BusinessName.text;

        int maxActivations = -1;

        if (LimitedActivations.isOn)
        {
            if (!Int32.TryParse(MaxNumberOfActivations.text, out maxActivations))
            {
                ErrorGO.SetActive(true);
                WaitGO.SetActive(false);
                Error.text = "Max number of activations must be an integer number.";
                return;
            }       

        }

        if (businessName.Trim() == "")
        {
            ErrorGO.SetActive(true);
            WaitGO.SetActive(false);
            Error.text = "Business Name could not be empty";
            return;
        }

        StartCoroutine(KeyCreation(businessName, maxActivations));
    }

    IEnumerator KeyCreation(string businessName, int maxActivations)
    {
        string jsonData = "{\"body\":{ \"Name\": \"" + businessName.Trim() + "\", \"NumberOfMaxActivations\": \"" + maxActivations.ToString()+ "\" ,\"AuthKey\": \"9a97a824609d71c59fe5188e67fb0098b635b08739a3b41fc4184d3abeb7ece3\"}}";

        UnityWebRequest www = UnityWebRequest.Put("https://l07umvpzaj.execute-api.us-east-2.amazonaws.com/Prod/addkey", jsonData);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("accept", "application/json; charset=UTF-8");

        Debug.Log("Creating Key!");

        yield return www.SendWebRequest();

        Debug.Log("Returned Key creation");
        WaitGO.SetActive(false);
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error");
            Debug.Log(www.error);
            StringBuilder sb = new StringBuilder();
            foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
            {
                sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
            }

            Debug.Log("Body: " + www.downloadHandler.text);

            if (www.downloadHandler.text.Contains("Error"))
            {
                ErrorGO.SetActive(true);
                Error.text = "Server side error try again.";
            }                      
            else
            {
                ErrorGO.SetActive(true);
                Error.text = "Error while handling your request. Check your connection and try again.";              
            }
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            if (www.downloadHandler.text.Contains("GeneratedKey"))
            {
                string generatedKey = www.downloadHandler.text.Replace("GeneratedKey", "").Replace(":", "").Replace("{", "").Replace("}", "").Replace("\"", "");
                GeneratedKey.text = generatedKey;
            }
          
        }
    }

}
