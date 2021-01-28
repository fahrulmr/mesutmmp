using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarouselBehaviour))]
public class CarouselImageSetter : MonoBehaviour 
{
	public List<ClothesItemBehaviour> CarouselCloseItems;
	public List<ClothesItem> CloseItems;

	private CarouselBehaviour carousel;
	private ImageIOUtility imageIOUtility;

	private int LastIndex;
	void Awake()
	{
		carousel = GetComponent<CarouselBehaviour>();
		imageIOUtility = new ImageIOUtility();
		if (EditorSettings.EditorInstance.Сategories != null)
		{
			for (int i = 0; i < EditorSettings.EditorInstance.Сategories.Count; i++)
			{
				var clothesItem = new ClothesItem();
				clothesItem.ClothesTypeIndex = i;
				imageIOUtility.LoadImage(UnityEngine.Application.dataPath + EditorSettings.EditorInstance.СategoriesIcons[i]);
				clothesItem.Texture = imageIOUtility.ImageWithPath.Texture;
				CloseItems.Add(clothesItem);
			}
		}
	}

	public void OnEnable()
	{
		carousel.OnCarouselItemChange.AddListener(OnCarouselItemChange);
	}

	void OnDisable()
	{
		carousel.OnCarouselItemChange.RemoveListener(OnCarouselItemChange);
	}

	private void Init()
	{
		int closeItemsIndex = LastIndex;
		if (CloseItems.Count == 0)
			return;
		for (int i = 0; i < CarouselCloseItems.Count; i++)
		{
			CarouselCloseItems[i].ClothesItem = CloseItems[closeItemsIndex];
			closeItemsIndex = NextIndex(closeItemsIndex, CloseItems);
		}
	}

	public void SelectLastCategoryIndex(int index)
	{
		LastIndex = index;
	}

	public void SelectLastClothesIndex(int index)
	{
		LastIndex = index;
	}

	private void OnCarouselItemChange()
	{
		Settings.Instance.ClothesOffset = new Vector2(0.0f, 0.0f);
		Settings.Instance.ClothesSkeletonScale = 0.0f;
		var closeItem = carousel.SelectedTransform.GetComponent<ClothesItemBehaviour>().ClothesItem;
		var currentItemIndex = CarouselCloseItems.IndexOf(carousel.SelectedTransform.GetComponent<ClothesItemBehaviour>());
		var currentIndex = CloseItems.IndexOf(closeItem);
		currentItemIndex = PrevIndex(currentItemIndex, CarouselCloseItems, 2);
		currentIndex = PrevIndex(currentIndex, CloseItems, 2);
		for (int i = 0; i < 5; i++)
		{
			CarouselCloseItems[currentItemIndex].ClothesItem = CloseItems[currentIndex];
			currentItemIndex = NextIndex(currentItemIndex, CarouselCloseItems);
			currentIndex = NextIndex(currentIndex, CloseItems);
		}
	}

	private int PrevIndex(int i, IList list, int steps)
	{
		var res = i;
		for (int j = 0; j < steps; j++)
		{
			res = PrevIndex(res, list);
		}
		return res;
	}

	private int NextIndex(int i, IList list, int steps)
	{
		var res = i;
		for (int j = 0; j < steps; j++)
		{
			res = NextIndex(res, list);
		}
		return res;
	}

	private int NextIndex(int i, IList list)
	{
		return i == list.Count - 1 ? 0 : i + 1;
	}

	private int PrevIndex(int i, IList list)
	{
		return i == 0 ? list.Count - 1 : i - 1;
	}

	public void UpdateImages()
	{
//		if (carousel.SelectedTransform.GetComponent<CloseTypeItemBehaviour>().CloseItem.Texture == null)
//		{
//			Init();
//		}
//		else
//		{
//			for (int i = 0; i < CarouselCloseItems.Count; i++)
//			{
//				CarouselCloseItems[i].CloseItem = CloseItems[i];
//			}
//		}
		Init();
	}

}
