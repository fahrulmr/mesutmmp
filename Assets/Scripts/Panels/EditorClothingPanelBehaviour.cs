using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorClothingPanelBehaviour : GenericPanelBehaviour
{
	public GameObject ClothingPanelItem;
	public GameObject SelectClothesButton;
	public Transform ContentPanel;
	public CarouselBehaviour CarouselBehaviour;
	public List<GameObject> AllClothesButton;

	private CategoryPanelBehaviour categoryPanelBehaviour;
	private ImageIOUtility imageIOUtility;

	void Awake ()
	{
		imageIOUtility = new ImageIOUtility();
		ReloadCarousel();
	}
 
	[ContextMenu("ReloadCarousel")]
	public void ReloadCarousel()
	{
		AllClothesButton.Clear();
		if (ContentPanel.childCount > 0)
		{
			foreach (Transform contentPanelchild in ContentPanel)
			{
				Destroy(contentPanelchild.gameObject);
			}
		}
		ContentPanel.DetachChildren();
		CarouselBehaviour.RebuildPath();

		if (EditorSettings.EditorInstance.Сategories != null)
		{
			for (int i = 0; i < EditorSettings.EditorInstance.Сategories.Count; i++)
			{
				var clothingPanelItem = Instantiate(ClothingPanelItem, new Vector2(0.0f, 0.0f), Quaternion.identity);
				clothingPanelItem.transform.SetParent(ContentPanel, false);
				clothingPanelItem.GetComponent<EditorClothesCategoryScrollViewBehaviour>().CategoryTitle.text = EditorSettings.EditorInstance.Сategories[i];
				if (EditorSettings.EditorInstance.AddPanel != null)
				{
					for (int j = 0; j < EditorSettings.EditorInstance.AddPanel.Count; j++)
					{
						if (EditorSettings.EditorInstance.AddPanel[j].CategoryIndex == i)
						{
							var selectClothesButton = Instantiate(SelectClothesButton, new Vector2(0.0f, 0.0f), Quaternion.identity);
							selectClothesButton.transform.SetParent(clothingPanelItem.GetComponent<EditorClothesCategoryScrollViewBehaviour>().Content, false);
							var editorSelectClothesButton = selectClothesButton.GetComponent<EditorSelectClothesButton>();
							editorSelectClothesButton.Title.text = EditorSettings.EditorInstance.AddPanel[j].Title;
							imageIOUtility.LoadImage(UnityEngine.Application.dataPath + EditorSettings.EditorInstance.AddPanel[j].SpritePath);
							editorSelectClothesButton.Image.texture = imageIOUtility.ImageWithPath.Texture;
							editorSelectClothesButton.Index = j;
							AllClothesButton.Add(selectClothesButton);
						}
					}
				}
			}
			CarouselBehaviour.RebuildPath();
		}
	}
}
