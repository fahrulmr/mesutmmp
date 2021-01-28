using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using DG.Tweening;

[System.Serializable]
public class Settings
{
	private const string fileName = "Settings.json";
	private static Settings instance = null;

	public static Settings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FileUtility.Load<Settings>(fileName);
			}
			return instance;
		}
	}

	public void Reload()
	{
		instance = FileUtility.Load<Settings>(fileName);
	}

	public int MaximumDistance = 1700;
	public float ButtonPressTime = 0.55f;
	public float ButtonNextClickWaitTime = 2f;
	public float SmoothFactor = 5.0f;
	public float IgnoreRadius = 30.0f;
	public float DisplayScale = 1.3f;
	public int Quality = 3;
	public float DisplayEdgeOffset = 0.0f;
	public float HandScale = 1.0f;
	public float CursorSpeed = 1.0f;
	public float SensorAngle = 0.0f;
	public float SensorHeight = 1.0f;
	public Vector3 KinectImageOffset = Vector3.zero;
	//public Vector2 offset = Vector2.zero;

	public float Brightness = 0.5f;
	public float Contrast = 0.5f;
	public float ViewPositionX = 0.0f;
	public float ViewPositionY = 0.0f;
	public float ViewScale = 1.0f;
	public bool AutoMenuPosition = false;
//	public float SkeletonShiftX = 0.0f;
//	public float SkeletonShiftY = 0.0f;
	public Vector2 SkeletonShift = new Vector2(0.0f, 0.0f);
	public Vector2 ClothesOffset = new Vector2(0.0f, 0.0f);
	public float ClothesSkeletonScale = 0.0f;
	public float SkeletonScale = 1.0f;
	public float TrackingStartLeft = 1000.0f;
	public float TrackingStartRight = 2300.0f;
	public float TrackingMinPosition = 1000.0f;
	public float TrackingMaxPosition = 1600.0f;
	public string BackgroundImagePath = "";
	public string LogoImagePath = "";

	public int MenuPosition = 4;
	public bool HideShowJoints=false;
	public bool HideShowControls=true;
	public bool ShowBackground = true;
	public bool HideShowInfoPos = false;
    public bool ShowTrailer = true;

	public void Save()
	{
		FileUtility.Save(fileName, this);
	}
}

public class SettingsPanelBehaviour : GenericPanelBehaviour
{
	public Text DistanceText;

	[HideInInspector]
	public Settings Settings;

	void Awake()
	{
//		var path = Application.dataPath;
//		path = System.IO.Path.Combine(path, "Settings.json");
//
//		if (System.IO.File.Exists(path))
//		{
//			var json = System.IO.File.ReadAllText(path);
//			Settings = JsonUtility.FromJson<Settings>(json);
//		}
//		else
//		{
//			Settings = new Settings();
//		}

		Settings = Settings.Instance;

		base.Awake();

		Settings.Quality = Mathf.Clamp(Settings.Quality, 0, 5);
		Settings.DisplayEdgeOffset = Mathf.Clamp(Settings.DisplayEdgeOffset, 0.0f, 0.4f);
		Settings.IgnoreRadius = Mathf.Max(Settings.IgnoreRadius, 0.0f);
		Settings.Instance.ClothesOffset = new Vector2(0.0f, 0.0f);
		QualitySettings.SetQualityLevel(Settings.Quality);
	}

//	void OnApplicationQuit()
//	{
////		var json = JsonUtility.ToJson(Settings);
////		var path = Application.dataPath;
////
////		path = System.IO.Path.Combine(path, "Settings.json");
////
////		System.IO.File.WriteAllText(path, json);
//		Settings.Instance.Save();
//	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//Settings.Instance.Save();
			//Application.Quit();
		}

		if (Input.GetKeyUp(KeyCode.F2))
		{
			Settings.MaximumDistance += 50;
			TextToDisp = "Distance: " + (Settings.MaximumDistance / 1000.0f).ToString() + "m";
			UpdateLayout();
		}
		else if (Input.GetKeyUp(KeyCode.F1))
		{
			Settings.MaximumDistance -= 50;
			TextToDisp = "Distance: " + (Settings.MaximumDistance / 1000.0f).ToString() + "m";
			UpdateLayout();
		}

		if (Input.GetKeyUp(KeyCode.F5))
		{
			Settings.DisplayScale += 0.05f;
			TextToDisp = "Scale: " + Settings.DisplayScale.ToString("F2");
			UpdateLayout();
		}
		else if (Input.GetKeyUp(KeyCode.F6))
		{
			Settings.DisplayScale -= 0.05f;
			Settings.DisplayScale = Mathf.Max(Settings.DisplayScale, 1.0f);
			TextToDisp = "Scale: " + Settings.DisplayScale.ToString("F2");
			UpdateLayout();
		}

//		if (Input.GetKeyUp(KeyCode.F8))
//		{
//			Settings.KinectImageOffset.x += 10f;
//			TextToDisp = "Offset X: " + Settings.KinectImageOffset.x.ToString("F2");
//			UpdateLayout();
//		}
		else if (Input.GetKeyUp(KeyCode.F9))
		{
			Settings.KinectImageOffset.x -= 10f;
			TextToDisp = "Offset X: " + Settings.KinectImageOffset.x.ToString("F2");
			UpdateLayout();
		}

		if (Input.GetKeyUp(KeyCode.F10))
		{
			Settings.KinectImageOffset.y += 10f;
			TextToDisp = "Offset Y: " + Settings.KinectImageOffset.y.ToString("F2");
			UpdateLayout();
		}
		else if (Input.GetKeyUp(KeyCode.F11))
		{
			Settings.KinectImageOffset.y -= 10f;
			TextToDisp = "Offset Y: " + Settings.KinectImageOffset.y.ToString("F2");
			UpdateLayout();
		}

//		if (Input.GetKeyUp(KeyCode.F7))
//		{
//			int next = Settings.Quality == 5 ? 0 : Settings.Quality + 1;
//			Settings.Quality = next;
//			TextToDisp = "QualityLevel: " + next.ToString("F2");
//			UpdateLayout();
//			QualitySettings.SetQualityLevel(next);
//		}
	}

	private string TextToDisp;

	void UpdateLayout()
	{
		StopCoroutine("ShowText");
		StartCoroutine("ShowText");
	}

	IEnumerator ShowText()
	{
//		DistanceText.text = TextToDisp;
//		DistanceText.text = "Distance: " + (Settings.MaximumDistance / 1000.0f).ToString() + "m";
//		DistanceText.DOFade(1.0f, 0.3f);
		yield return new WaitForSeconds(1.0f);
//		DistanceText.DOFade(0.0f, 0.3f);
	}
}
