using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
#if UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif

[System.Serializable]
public struct ClothesInfo
{
	public string Title;
	public int CategoryIndex;
	public int SelectedSizeButtons;
	public string Description;
	public string URL;
	public string SpritePath;
	public Vector2 Position;
	public Vector2 Offset;
	public float Scale;
	public Astra.JointType JointType; 
}

public class AddEditClothesBehaviour : GenericPanelBehaviour
{
	public InputField Title;
	public Dropdown CategoryDropdown;
	public InputField Description;
	public InputField URL;
	public InputField SpritePathField;
	public EditorClothesSizeButton[] EditorClothesSizeButtons;
	public RawImage ClothesImage;

	private EditorClothingPanelBehaviour editorClothingPanelBehaviour;
	private EditorMainPanelBehaviour editorMainPanelBehaviour;
	private ClothesInfo addPanel;
	private bool Edit;
	private Astra.JointType jointType; 
	private int selectedSizeButtons;
	private ImageIOUtility imageIOUtility;
	private Texture2D image;
	private ImageWithPath imageWithPath;

	void OnInit()
	{
		editorClothingPanelBehaviour = FindObjectOfType<EditorClothingPanelBehaviour>();
		imageIOUtility = new ImageIOUtility();
		editorMainPanelBehaviour = FindObjectOfType<EditorMainPanelBehaviour>();
	}

	void OnOpen()
	{
		CategoryDropdown.ClearOptions();
		CategoryDropdown.AddOptions(EditorSettings.EditorInstance.Сategories);
		if (EditorSettings.EditorInstance.AddPanel == null)
			EditorSettings.EditorInstance.AddPanel = new List<ClothesInfo>();
	}

	public void PressAdd()
	{
		if (EditorSettings.EditorInstance.Сategories != null)
		{
			if (EditorSettings.EditorInstance.Сategories.Count > 0)
			{
				selectedSizeButtons = 0;
				Edit = false;
				ClothesImage.enabled = false;
				PanelManager.Instance.GetPanel<AddEditClothesBehaviour>().Open();		
				CategoryDropdown.value = 0;
				Title.text = "";
				Description.text = "";
				URL.text = "";
				SpritePathField.text = "";
				ClothesImage.transform.localScale = new Vector2(1.0f, 1.0f);
				ClothesImage.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
				ClothesImage.transform.localPosition = new Vector2(0.0f, 0.0f);
				ClothesImage.transform.GetChild(0).position = ClothesImage.transform.position;
				jointType = 0;
				editorMainPanelBehaviour.SelectClothesIndex = -1;
				foreach (EditorClothesSizeButton button in EditorClothesSizeButtons)
				{
					button.Deselect();
				}	
			}
		}
	}

	public void FindClothesIcon()
	{
		imageIOUtility.LoadImageFromFileSystem();
		if (imageIOUtility.IsValid())
		{
			imageWithPath = imageIOUtility.ImageWithPath;
			SpritePathField.text = imageWithPath.Path;
			image = imageWithPath.Texture;
//			ClothesImage.texture = image;
//			ClothesImage.SetNativeSize();
			editorMainPanelBehaviour.SetClothesIcon(image);
			//ClothesImage.enabled = true;
		}
	}

	public void PressEdit()
	{
		if (EditorSettings.EditorInstance.Сategories != null)
		{
			if (editorMainPanelBehaviour.SelectClothesIndex >= 0)
			{
				Edit = true;
				PanelManager.Instance.GetPanel<AddEditClothesBehaviour>().Open();
				selectedSizeButtons = EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].SelectedSizeButtons;
				Title.text = EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].Title;
				CategoryDropdown.value = EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].CategoryIndex;
				Description.text = EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].Description;
				URL.text = EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].URL;
				SpritePathField.text = EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].SpritePath;
				ClothesImage.enabled = true;
				foreach (EditorClothesSizeButton button in EditorClothesSizeButtons)
					if (((ClothingSize)EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex].SelectedSizeButtons & button.ClothingSize) == button.ClothingSize)
					{
						button.Select();
					}
					else
						button.Deselect();
			}
		}
	}

	public void AddApplyButton()
	{
		if (Title.text != "" && Description.text != "" && URL.text != "" && SpritePathField.text != "")
		{
			addPanel.Title = Title.text;
			addPanel.CategoryIndex = CategoryDropdown.value;
			addPanel.Description = Description.text;
			addPanel.URL = URL.text;
			addPanel.SpritePath = SpritePathField.text;
			addPanel.Position = ClothesImage.transform.localPosition;
			addPanel.Offset = Vector2.zero;

			addPanel.Scale = ClothesImage.transform.localScale.x;
			addPanel.SelectedSizeButtons = selectedSizeButtons;
			addPanel.JointType = editorMainPanelBehaviour.JointType;
			if (!Edit)
			{
				EditorSettings.EditorInstance.AddPanel.Add(addPanel);
				editorMainPanelBehaviour.SelectClothesIndex = EditorSettings.EditorInstance.AddPanel.Count - 1;
			}
			else if (Edit)
			{
				EditorSettings.EditorInstance.AddPanel[editorMainPanelBehaviour.SelectClothesIndex] = addPanel;
			}
			if (image != null)
			{
				imageIOUtility.SaveTexture(imageWithPath);
			}
			EditorSettings.EditorInstance.Save();
			PanelManager.Instance.GetPanel<AddEditClothesBehaviour>().Close();
			editorMainPanelBehaviour.JointType = Astra.JointType.BaseSpine;
			editorClothingPanelBehaviour.ReloadCarousel();
		}
	}

	public void PressCancel()
	{
		editorMainPanelBehaviour.SelectClothesIndex = -1;
		ClothesImage.enabled = false;
		//ClothesImage.texture = null;
		PanelManager.Instance.GetPanel<AddEditClothesBehaviour>().Close();
	}

	public void PressSizeButton(int index)
	{
		if (!EditorClothesSizeButtons[index].IsSelected)
		{
			selectedSizeButtons += (int)EditorClothesSizeButtons[index].ClothingSize;
			EditorClothesSizeButtons[index].Select();
		}
		else
		{
			selectedSizeButtons -= (int)EditorClothesSizeButtons[index].ClothingSize;
			EditorClothesSizeButtons[index].Deselect();
		}
	}
}
