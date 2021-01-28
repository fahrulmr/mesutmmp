using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif

public class CategoryPanelBehaviour : GenericPanelBehaviour
{
	public InputField CategoryNameField;
	public Dropdown Dropdown;
	public InputField SpritePathField;

	public InputField NewCategoryNameField;
	public InputField NewSpritePathField;

	private EditorClothingPanelBehaviour editorClothingPanelBehaviour;
	private ImageIOUtility imageIOUtility;

	private ImageWithPath imageWithPath;
	private ImageWithPath imageWithPathNew;
	private Texture2D image;
	private Texture2D newImage;

	void OnInit()
	{
		editorClothingPanelBehaviour = FindObjectOfType<EditorClothingPanelBehaviour>();
		imageIOUtility = new ImageIOUtility();
	}

	void OnOpen()
	{
		if (EditorSettings.EditorInstance.Сategories == null)
		{
			EditorSettings.EditorInstance.Сategories = new List<string>();
			EditorSettings.EditorInstance.СategoriesIcons = new List<string>();

		}
		ReloadDropdown();
		if (EditorSettings.EditorInstance.СategoriesIcons.Count > 0)
			SpritePathField.text = EditorSettings.EditorInstance.СategoriesIcons[Dropdown.value];
	}

	public void FindCategoryIcon()
	{
		if (EditorSettings.EditorInstance.Сategories.Count > 0)
		{
			imageIOUtility.LoadImageFromFileSystem();
			if (imageIOUtility.IsValid())
			{
				imageWithPath = imageIOUtility.ImageWithPath;
				SpritePathField.text = imageWithPath.Path;
				image = imageWithPath.Texture;
			}
		}
	}

	public void FindNewCategoryIcon()
	{
		imageIOUtility.LoadImageFromFileSystem();
		if (imageIOUtility.IsValid())
		{
			imageWithPathNew = imageIOUtility.ImageWithPath;
			NewSpritePathField.text = imageWithPathNew.Path;
			newImage = imageWithPath.Texture;
		}
	}

	public void ValueChanged()
	{
		SpritePathField.text = EditorSettings.EditorInstance.СategoriesIcons[Dropdown.value];
	}

	public void AddCategory()
	{ 
		if (NewCategoryNameField.textComponent.text != "" && NewSpritePathField.textComponent.text != "")
		{
			EditorSettings.EditorInstance.Сategories.Add(NewCategoryNameField.textComponent.text);
			EditorSettings.EditorInstance.СategoriesIcons.Add(imageWithPathNew.Path);
			ReloadDropdown();
			EditorSettings.EditorInstance.Save();
			editorClothingPanelBehaviour.ReloadCarousel();
			SpritePathField.text = EditorSettings.EditorInstance.СategoriesIcons[Dropdown.value];
			NewCategoryNameField.text = "";
			NewSpritePathField.text = "";
		}
	}

	public void Save()
	{
		if (EditorSettings.EditorInstance.Сategories.Count > 0)
		{
			if (CategoryNameField.textComponent.text != "" && SpritePathField.textComponent.text != "")
			{
				EditorSettings.EditorInstance.Сategories[Dropdown.value] = CategoryNameField.textComponent.text;
				EditorSettings.EditorInstance.СategoriesIcons[Dropdown.value] = SpritePathField.textComponent.text;
				EditorSettings.EditorInstance.Save();
				editorClothingPanelBehaviour.ReloadCarousel();
			}
			//if (openFileDialog != null)
			imageIOUtility.SaveTexture(imageWithPathNew);
		}
	}

	private void ReloadDropdown()
	{
		Dropdown.ClearOptions();
		Dropdown.AddOptions(EditorSettings.EditorInstance.Сategories);
		CategoryNameField.text = "";
	}

	public void Rename()
	{
		for (int i = 0; i < EditorSettings.EditorInstance.Сategories.Count; i++)
		{
			if (i == Dropdown.value)
			{
				EditorSettings.EditorInstance.Сategories[i] = CategoryNameField.text;
				ReloadDropdown();
				EditorSettings.EditorInstance.Save();
				editorClothingPanelBehaviour.ReloadCarousel();
			}
		}
	}

	public void Delete()
	{
		if (EditorSettings.EditorInstance.Сategories.Count >= 0)
		{
			EditorSettings.EditorInstance.Сategories.RemoveAt(Dropdown.value);
			EditorSettings.EditorInstance.СategoriesIcons.RemoveAt(Dropdown.value);	
			ReloadDropdown();
			EditorSettings.EditorInstance.Save();
			editorClothingPanelBehaviour.ReloadCarousel();
			Dropdown.value = 0;
			if (EditorSettings.EditorInstance.Сategories.Count == 0)
				SpritePathField.text = "";
		}
	}

	public void Done()
	{
		PanelManager.Instance.GetPanel<CategoryPanelBehaviour>().Close();
		imageIOUtility.SaveTexture(imageWithPathNew);
	}
}
