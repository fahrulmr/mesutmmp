using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpecificClothesContainer
{
	public int CategoryTypeIndex;
   	//public ClothesType ClothesType;
    public List<ClothesItem> Values;
}

public class SpecificClothesContainerBehaviour : MonoBehaviour
{
	public List<SpecificClothesContainer> SpecificClothesContainers;
	public CarouselImageSetter CarouselClothes;
	public CarouselImageSetter CarouselCategory;


	private ImageIOUtility imageIOUtility;

	private int categoryIndex;
	private int clothesIndex;

	void Awake()
	{
		imageIOUtility = new ImageIOUtility();
		if (EditorSettings.EditorInstance.Сategories != null)
		{
			for (int i = 0; i < EditorSettings.EditorInstance.Сategories.Count; i++)
			{
				var specificClothesContainer = new SpecificClothesContainer();
				specificClothesContainer.CategoryTypeIndex = i;
				var сlothesItemList = new List<ClothesItem>();
				for (int j = 0; j < EditorSettings.EditorInstance.AddPanel.Count; j++)
				{
					if (i == EditorSettings.EditorInstance.AddPanel[j].CategoryIndex)
					{
						var clothesItem = new ClothesItem();
						clothesItem.Name = EditorSettings.EditorInstance.AddPanel[j].Title;
						clothesItem.Price = EditorSettings.EditorInstance.AddPanel[j].Description;
						clothesItem.URL = EditorSettings.EditorInstance.AddPanel[j].URL;
						clothesItem.ClothingSize = (ClothingSize)EditorSettings.EditorInstance.AddPanel[j].SelectedSizeButtons;
						imageIOUtility.LoadImage(UnityEngine.Application.dataPath + EditorSettings.EditorInstance.AddPanel[j].SpritePath);
						clothesItem.Texture = imageIOUtility.ImageWithPath.Texture;
						clothesItem.Position = EditorSettings.EditorInstance.AddPanel[j].Position;
						clothesItem.Offset = EditorSettings.EditorInstance.AddPanel[j].Offset;

						clothesItem.Scale = EditorSettings.EditorInstance.AddPanel[j].Scale;
						clothesItem.JointType = EditorSettings.EditorInstance.AddPanel[j].JointType;
						сlothesItemList.Add(clothesItem);
						specificClothesContainer.Values = сlothesItemList;
						if (PlayerPrefs.GetInt("LastSelectClothes") == j)
						{
							clothesIndex = сlothesItemList.Count - 1;
							categoryIndex = i;
						}
					}
				}
				SpecificClothesContainers.Add(specificClothesContainer);
			}
			CarouselCategory.SelectLastCategoryIndex(categoryIndex);
			CarouselClothes.SelectLastClothesIndex(clothesIndex);
		}
	}

	public void ResetTextures(int closeType)
	{
		var texturesContainer = SpecificClothesContainers.Find(c => c.CategoryTypeIndex == closeType).Values;
//		var clothesItems = texturesContainer.ConvertAll(t => new ClothesItem(closeType, t));
		if (texturesContainer != null)
		{
			for (int i = 0; i < texturesContainer.Count; i++)
			{
				var temp = texturesContainer[i];
				temp.ClothesTypeIndex = closeType;
				texturesContainer[i] = temp;
			}
		

			CarouselClothes.CloseItems = texturesContainer;
			CarouselClothes.UpdateImages();
		}
	}
}
