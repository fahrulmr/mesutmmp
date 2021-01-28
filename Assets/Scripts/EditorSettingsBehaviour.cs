using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorSettings
{
	private const string fileName = "SettingsEditor.json";
	private static EditorSettings editorInstance = null;

	public static EditorSettings EditorInstance
	{
		get
		{
			if (editorInstance == null)
			{
				editorInstance = FileUtility.Load<EditorSettings>(fileName);
			}
			return editorInstance;
		}
	}

	public List<string> Сategories;
	public List<string> СategoriesIcons;
	public List<ClothesInfo> AddPanel;

	public void Save()
	{
		FileUtility.Save(fileName, this);
	}

	public void Reload()
	{
		editorInstance = FileUtility.Load<EditorSettings>(fileName);
	}
}

public class EditorSettingsBehaviour : MonoBehaviour {

	public EditorSettings EditorSettings;

	void Awake()
	{
		EditorSettings = EditorSettings.EditorInstance;
		//EditorSettings.EditorInstance.Save();
	}
}
