using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EditorMainPanelBehaviour : GenericPanelBehaviour
{
	public int SelectClothesIndex = -1;
	public Astra.JointType JointType; 
	public Transform Clothes;
	public Slider Slider;
	public Button[] JointButtons;
	public Transform PivotJointButton;

	private EditorClothingPanelBehaviour editorClothingPanelBehaviour;
	private RawImage clothes;
	private Vector3 BeginClothesDragMousePosition;
	private Vector3 BeginPivotDragMousePosition;

	private Vector3 point;
	private RectTransform SelectJointTransform;

	void OnInit()
	{
		editorClothingPanelBehaviour = FindObjectOfType<EditorClothingPanelBehaviour>();
		clothes = Clothes.GetComponent<RawImage>();
		Cursor.visible = true;
		JointType = Astra.JointType.BaseSpine;
	}

	void Start()
	{
		SelectJointType(0);
	}
		
	public void RemoveClothes()
	{
		if (SelectClothesIndex >= 0)
		{
			Destroy(editorClothingPanelBehaviour.AllClothesButton[SelectClothesIndex]);
			EditorSettings.EditorInstance.AddPanel.RemoveAt(SelectClothesIndex);
			EditorSettings.EditorInstance.Save();
			clothes.enabled = false;
			PivotJointButton.gameObject.SetActive(false);
			SelectClothesIndex = -1;
		}
		editorClothingPanelBehaviour.ReloadCarousel();
		SelectJointType(0);
	}

	public void SetClothesIcon(Texture tex)
	{
		clothes.SetAndFitTexture((Texture2D) tex, false);
		clothes.enabled = true;
		PivotJointButton.gameObject.SetActive(true);
		if (SelectClothesIndex >= 0)
		{
			Slider.value = EditorSettings.EditorInstance.AddPanel[SelectClothesIndex].Scale;
			Clothes.localScale = new Vector2(EditorSettings.EditorInstance.AddPanel[SelectClothesIndex].Scale, EditorSettings.EditorInstance.AddPanel[SelectClothesIndex].Scale);
			PivotJointButton.localScale = new Vector2(1.0f / Clothes.transform.localScale.x, 1.0f / Clothes.transform.localScale.y);
			for(int i = 0; i < JointButtons.Length; i++)
			{
				if (EditorSettings.EditorInstance.AddPanel[SelectClothesIndex].JointType == JointButtons[i].GetComponent<JointsButtonBehaviour>().JointType)
				{
					SelectJointType(i);
				}
			}
			EditorSettings.EditorInstance.Save();
		}
	}

	public void SelectJointType(int index)
	{
		for(int i = 0; i < JointButtons.Length; i++)
		{
			if (i != index)
			{
				JointButtons[i].GetComponent<JointsButtonBehaviour>().Deselect();
			}
			else if (i == index)
			{
				JointButtons[i].GetComponent<JointsButtonBehaviour>().Select();
				if (SelectClothesIndex >= 0)
				{
					Clothes.GetComponent<RectTransform>().pivot = EditorSettings.EditorInstance.AddPanel[SelectClothesIndex].Position;
					Clothes.GetComponent<RectTransform>().transform.position = EditorSettings.EditorInstance.AddPanel[SelectClothesIndex].Offset + (Vector2)JointButtons[i].transform.position; 
					PivotJointButton.transform.position = Clothes.transform.position;
					SelectJointTransform = JointButtons[i].GetComponent<RectTransform>();
				}
				JointType = JointButtons[i].GetComponent<JointsButtonBehaviour>().JointType;
			}
		}
	}

	public void Left()
	{
		if(SelectClothesIndex >= 0)
			Clothes.transform.localPosition = new Vector2(Clothes.transform.localPosition.x - 20f, Clothes.transform.localPosition.y);
	}

	public void Right()
	{
		if(SelectClothesIndex >= 0)
			Clothes.transform.localPosition = new Vector2(Clothes.transform.localPosition.x + 20f, Clothes.transform.localPosition.y);
	}

	public void Up()
	{
		if(SelectClothesIndex >= 0)
			Clothes.transform.localPosition = new Vector2(Clothes.transform.localPosition.x, Clothes.transform.localPosition.y + 20f);
	}

	public void Down()
	{
		if(SelectClothesIndex >= 0)
			Clothes.transform.localPosition = new Vector2(Clothes.transform.localPosition.x, Clothes.transform.localPosition.y - 20f);
	}

	public void ScalePlus()
	{
		if (SelectClothesIndex >= 0)
		{
			Clothes.transform.localScale = new Vector2(Clothes.transform.localScale.x + 0.1f, Clothes.transform.localScale.y + 0.1f);
			PivotJointButton.localScale = new Vector2(1.0f / Clothes.transform.localScale.x, 1.0f / Clothes.transform.localScale.y);
			Slider.value += 0.1f;
		}
	}

	public void ScaleMinus()
	{
		if (SelectClothesIndex >= 0)
		{
			Clothes.transform.localScale = new Vector2(Clothes.transform.localScale.x - 0.1f, Clothes.transform.localScale.y - 0.1f);
			PivotJointButton.localScale = new Vector2(1.0f / Clothes.transform.localScale.x, 1.0f / Clothes.transform.localScale.y);
			Slider.value -= 0.1f;
		}
	}

	public void OnSliderChange()
	{
		if(SelectClothesIndex >= 0)
			Clothes.transform.localScale = new Vector2(Slider.value, Slider.value);
	}

	public void SaveParameters()
	{
		if (SelectClothesIndex >= 0)
		{
			var addpanel = EditorSettings.EditorInstance.AddPanel[SelectClothesIndex];
			addpanel.JointType = JointType;
			addpanel.Scale = Clothes.localScale.x;
			var jointPos = new List<Button>(JointButtons).Find(b => b.GetComponent<JointsButtonBehaviour>().JointType == JointType).transform.position;
			addpanel.Offset = Clothes.transform.position - jointPos;
			addpanel.Position = Clothes.GetComponent<RectTransform>().pivot;
			EditorSettings.EditorInstance.AddPanel[SelectClothesIndex] = addpanel;
			EditorSettings.EditorInstance.Save();
		}
	}

	private bool Destroy(int i)
	{
			for (int j = 0; j < EditorSettings.EditorInstance.AddPanel.Count; j++)
			{
				if (i == EditorSettings.EditorInstance.AddPanel[j].CategoryIndex)
				{
				return false;
				}
			}
		return true;
	}

	void PanelUpdate ()
	{
		if (Input.GetKeyDown(KeyCode.F12))
		{
			if (EditorSettings.EditorInstance.Сategories != null)
			{
				for (int i = 0; i < EditorSettings.EditorInstance.Сategories.Count; i++)
				{
					if (Destroy(i))
					{
						EditorSettings.EditorInstance.Сategories.RemoveAt(i);
						EditorSettings.EditorInstance.СategoriesIcons.RemoveAt(i);	
						EditorSettings.EditorInstance.Save();
					}
				}
			}
			PlayerPrefs.SetInt("LastSelectClothes", SelectClothesIndex);
			SceneManager.LoadSceneAsync("Main");
		}
	}

	public void OnDragBegin()
	{
		BeginClothesDragMousePosition = Input.mousePosition - Clothes.transform.position;
	}



	public void Drag()
	{
		Clothes.transform.position = Input.mousePosition - BeginClothesDragMousePosition;
	}

	public void OnDragEnd()
	{
		SetPivotPoint();
	}



	private void SetPivotPoint()
	{
		Vector2 dir = SelectJointTransform.position - Clothes.transform.position;
		dir = new Vector2(dir.x / Clothes.transform.localScale.x, dir.y / Clothes.transform.localScale.y);
		Clothes.transform.position = SelectJointTransform.position;
		Clothes.GetComponent<RectTransform>().pivot += new Vector2(dir.x / Clothes.GetComponent<RectTransform>().rect.width, 
			dir.y / Clothes.GetComponent<RectTransform>().rect.height); 
	}

	public void Exit()
	{
		PlayerPrefs.SetInt("LastSelectClothes", SelectClothesIndex);
		EditorSettings.EditorInstance.Save();
		SceneManager.LoadSceneAsync("Main");
	}


	public void SaveToS3()
	{
		S3Manager.Instance.SaveAllToS3();
	}

	public void DownloadFromS3()
	{
		S3Manager.Instance.DownloadAllFromS3();
	}
}
