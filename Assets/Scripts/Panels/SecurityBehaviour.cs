using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Microsoft.Win32;

public class SecurityBehaviour : MonoBehaviour
{
//	public InputField KinectID;
//	public InputField SecurityCode;
//	public InputField SecurityTrialCode;
//	public InputField RequestCode;
//	public RawImage Black;
//	public string SecretKey = "1100";
//	public string FirstTrialKey = "FirstTrialKey";

//	const string userRoot = "HKEY_CURRENT_USER";
//	const string subkey = "RegistrySetValueExample";
//	const string keyName = userRoot + "\\" + subkey;

//	private string kinectIDmd5;
//	private string appSecret;
//	private string pass;
//	private bool isAvailable = false;
//	private string AllSumbols = "abcdefghijklmnopqrstuvwxyz0123456789";

//	private Windows.Kinect.KinectSensor kinectSensor;

//	void OnInit()
//	{
//		//PlayerPrefs.SetInt("FirstLoad", 0);
//		//Registry.SetValue(keyName, FirstTrialKey, 5);
//		//Registry.SetValue(keyName, "777", -1);

//		Cursor.visible = true;
//		if (PlayerPrefs.GetInt("FirstLoad") == 0 && !System.IO.File.Exists(UnityEngine.Application.dataPath + "/Key.key"))
//		{
//			PlayerPrefs.SetInt("FirstLoad", 1);
//			int rValue = (int)Registry.GetValue(keyName, FirstTrialKey, -1);
//			if (rValue < 0)
//			{
//				Registry.SetValue(keyName, FirstTrialKey, 5);
//				PlayerPrefs.SetString("RequestCode", FirstTrialKey);
//				Application.LoadLevel("Main");
//			}
//			else if (rValue > 0)
//			{
////				rValue--;
////				Registry.SetValue(keyName, FirstTrialKey, rValue);
//				PlayerPrefs.SetString("RequestCode", FirstTrialKey);
//				Application.LoadLevel("Main");
//			}
//			else
//			{
//				Initialization();
//			}
//		}
//		else
//		{
//			Initialization();
//		}
//	}

//	private void Initialization()
//	{
//		kinectSensor = Windows.Kinect.KinectSensor.GetDefault();
//		kinectSensor.Open();
//		this.Invoke(new WaitUntil(() => kinectSensor.IsAvailable), () =>
//			{
//				if (System.IO.File.Exists(UnityEngine.Application.dataPath + "/Key.key"))
//				{
//					Black.enabled = false;
//					PanelManager.Instance.GetPanel<LoadingPanelBehaviour>().Open();
//					kinectIDmd5 = Md5Sum(kinectSensor.UniqueKinectId);
//					KinectID.text = kinectIDmd5;
//					Validate(System.IO.File.ReadAllText(UnityEngine.Application.dataPath + "/Key.key"), false);
//				}
//				else
//				{
//					Black.enabled = false;
//				}
//			});
//		appSecret = Md5Sum(SecretKey);
//	}

//	public void ValidateButton()
//	{
//		kinectIDmd5 = Md5Sum(kinectSensor.UniqueKinectId);
//		Validate(SecurityCode.text, false);
//	}

//	public void ValidateTrialButton()
//	{
//		kinectIDmd5 = Md5Sum(Md5Sum(kinectSensor.UniqueKinectId) + RequestCode.text);
//		Validate(SecurityTrialCode.text, true);
//	}

//	private void Validate(string securityCode, bool trial)
//	{
//		if (kinectSensor.IsAvailable)
//		{
//			pass = "";
//			for (int i = 0; i < kinectIDmd5.Length; i++)
//			{
//				if (kinectIDmd5.Length == securityCode.Length)
//				{
//					if ((int)appSecret[i] < (int)kinectIDmd5[i])
//					{
//						var p = (int)kinectIDmd5[i] - ((int)securityCode[i] - 48);//kinectmd5
//						pass += (char)p;
//					}
//					else
//					{
//						var p = (int)kinectIDmd5[i] + ((int)securityCode[i] - 48);
//						pass += (char)p;
//					}
//				}
//			}
//			SecurityCode.text = securityCode;
//			if (appSecret == pass)
//			{
//				if (!trial)
//				{
//					System.IO.File.WriteAllText(UnityEngine.Application.dataPath + "/Key.key", securityCode);
//					PlayerPrefs.SetString("RequestCode", securityCode);
//					Application.LoadLevel("Main");
//				}
//				else
//				{
//					if ((int)Registry.GetValue(keyName, RequestCode.text, -1) == 0)
//					{
//						PanelManager.Instance.GetPanel<WarningBehaviour>().WarningText.text = "Unsuitable Trial Security Code";
//						PanelManager.Instance.OpenPanel<WarningBehaviour>();
//					}
//					else if ((int)Registry.GetValue(keyName, RequestCode.text, -1) < 0)
//					{
//						Registry.SetValue(keyName, FirstTrialKey, 0);
//						Registry.SetValue(keyName, RequestCode.text, 10);
//						PlayerPrefs.SetString("RequestCode", RequestCode.text);
//						Application.LoadLevel("Main");
//					}

//					if ((int)Registry.GetValue(keyName, RequestCode.text, -1) > 0)
//					{
//						PlayerPrefs.SetString("RequestCode", RequestCode.text);
//						Application.LoadLevel("Main");
//					}
//				}
//			}
//			else
//			{
////				SecurityCode.text = securityCode;
//				//	PanelManager.Instance.GetPanel<WarningBehaviour>().WarningText.text = "Unsuitable Security Code";
//				PanelManager.Instance.OpenPanel<WarningBehaviour>();
//			}
//		}
//	}

//	public void GenerateNewRequestCode()
//	{
//		string requestCode = "";
//		for (int i = 0; i < 10; i++)
//		{
//			requestCode += AllSumbols[UnityEngine.Random.Range(0, AllSumbols.Length)];
//		}
//		RequestCode.text = requestCode;
////		if (PlayerPrefs.GetInt("FirstStart") == 0)
////		{
////			PlayerPrefs.SetInt("FirstStart", 1);
////			if (KinectID.text.Length > 0)
////			{
////				string newID = KinectID.text;
////
////				if (RequestCode.text != "")
////				{
////					newID = KinectID.text + requestCode;
////					newID = Md5Sum(newID);
////					Debug.Log(KinectID.text + "  " + requestCode);
////					Debug.Log("New ID " + newID);
////				}
////				for (int i = 0; i < newID.Length; i++)
////				{
////					var p = (int)newID[i] - (int)appSecret[i];
////					p = p < 0 ? -p : p;
////					p += 48;
////					pass += (char)p;
////				}
////				SecurityTrialCode.text = pass;
////			}
////		}
//	}

//	public  string Md5Sum(string strToEncrypt)
//	{
//		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
//		byte[] bytes = ue.GetBytes(strToEncrypt);
//		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
//		byte[] hashBytes = md5.ComputeHash(bytes);
//		string hashString = "";
//		for (int i = 0; i < hashBytes.Length; i++)
//		{
//			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
//		}
//		return hashString.PadLeft(32, '0');
//	}

//	void PanelUpdate()
//	{
//		if (kinectSensor != null)
//		{
//			if (!isAvailable && kinectSensor.IsAvailable)
//			{
//				isAvailable = true;
//				kinectIDmd5 = Md5Sum(kinectSensor.UniqueKinectId);
//				KinectID.text = kinectIDmd5;
//			}
//		}
//		else if (isAvailable && !kinectSensor.IsAvailable)
//		{
//			isAvailable = false;
//			KinectID.text = "Kinect not found";
//		}
//		if (Input.GetKeyDown(KeyCode.Escape))
//		{
//			if (PlayerPrefs.GetInt("FirstLoad") == 1)
//			{
//				int requestCode = (int)Registry.GetValue(keyName, PlayerPrefs.GetString("RequestCode"), -1);
//				if (requestCode > 0)
//				{
//					Registry.SetValue(keyName, PlayerPrefs.GetString("RequestCode"), requestCode - 1);
//				}
//				var rc = (int)Registry.GetValue(keyName, RequestCode.text, -1) - 1;
//				if (requestCode > 0)
//				{
//					Registry.SetValue(keyName, RequestCode.text, rc);
//				}
//				PlayerPrefs.SetInt("FirstLoad", 0);
//			}
//			Application.Quit();
//		}
//	}
}
