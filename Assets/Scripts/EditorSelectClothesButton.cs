using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EditorSelectClothesButton : MonoBehaviour {
	public RawImage Image;
	public Text Title;
	//public int ButtonIndex;
	public int Index;

	private EditorMainPanelBehaviour editorMainPanelBehaviour;

	void Awake()
	{
		editorMainPanelBehaviour = FindObjectOfType<EditorMainPanelBehaviour>();
	}

	public void Press()
	{		
		editorMainPanelBehaviour.SelectClothesIndex = Index;
		editorMainPanelBehaviour.SetClothesIcon(Image.texture);
	}
}
