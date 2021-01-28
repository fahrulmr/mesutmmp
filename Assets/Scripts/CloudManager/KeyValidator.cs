using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft;
using System;
using UnityEngine.SceneManagement;

public class KeyValidator : MonoBehaviour
{
    public TMP_InputField keyInputField;
    public Button validate;
    public TMP_Text errorText;
    public GameObject errorTextGO;

    void Start()
    {
        validate.onClick.AddListener(ValidateKey);
    }


    void ValidateKey()
    {
        Debug.Log("Validating Key!");

        errorTextGO.SetActive(false);
        string keyText = keyInputField.text;

        if (keyText == "")
        {
            errorTextGO.SetActive(true);
            errorText.text = "Key could not be empty";
            return;
        }

        StartCoroutine(KeyCheck(keyText));
    }


    IEnumerator KeyCheck(string key)
    {
        string jsonData = "{\"body\":{ \"Key\": \"" + key.Trim() + "\",\"AuthKey\": \"64a0b0656e4f2b59996d24bb221f2f6a3c97f7c3404c9703520e617585ef1e20\"}}";

        UnityWebRequest www = UnityWebRequest.Put("https://l07umvpzaj.execute-api.us-east-2.amazonaws.com/Prod/keyvalidation", jsonData);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("accept", "application/json; charset=UTF-8");
        
        Debug.Log("Chacking Key!");

        yield return www.SendWebRequest();

        Debug.Log("Returned Key check");

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

            if (www.downloadHandler.text.Contains("Error during the key updating process"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Server Error, please contact support.";
            }
            else if (www.downloadHandler.text.Contains("The key parameter is not configured"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Empty Key.";
            }
            else if (www.downloadHandler.text.Contains("Not Existing"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Invalid Key.";
            }
            else if (www.downloadHandler.text.Contains("NoAvailableKeyActivation"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Expired Key.";
            }
            else
            {
                errorTextGO.SetActive(true);
                errorText.text = "Error while handling your request. Check your connection and try again.";
            }
        }
        else
        {

            if (www.downloadHandler.text.Contains("Activable"))
            {
                ActivateAndLoad(key);
            }
            else if (www.downloadHandler.text.Contains("The key parameter is not configured"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Empty Key.";
            }
            else if (www.downloadHandler.text.Contains("Not Existing"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Invalid Key.";
            }
            else if (www.downloadHandler.text.Contains("NoAvailableKeyActivation"))
            {
                errorTextGO.SetActive(true);
                errorText.text = "Expired Key.";
            }
        }
    }



    private void ActivateAndLoad(string key)
    {
        Debug.Log("Activated!");
        PlayerPrefs.SetString("RequestCode", key);
        PlayerPrefs.SetString("MachineID", SystemInfo.deviceUniqueIdentifier);
        SceneManager.LoadSceneAsync("Main");
    }
}
