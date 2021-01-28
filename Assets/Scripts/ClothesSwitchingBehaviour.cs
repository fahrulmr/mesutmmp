using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesSwitchingBehaviour : MonoBehaviour
{
	public CarouselBehaviour SpecificClothesCarousel;
	public CarouselBehaviour ClothesTypeCarousel;
	public SpecificClothesContainerBehaviour SpecificCloseContainer;

	void OnEnable()
	{
		ClothesTypeCarousel.OnCarouselItemChange.AddListener(OnClothesTypeChange);
	}

	void Start()
	{
		ClothesTypeCarousel.GetComponent<CarouselImageSetter>().UpdateImages();
		OnClothesTypeChange();
	}

	void OnDisable()
	{
		ClothesTypeCarousel.OnCarouselItemChange.RemoveListener(OnClothesTypeChange);
	}

	void OnClothesTypeChange()
	{
		var closeType = ClothesTypeCarousel.SelectedTransform.GetComponent<ClothesItemBehaviour>().ClothesItem.ClothesTypeIndex;//
		SpecificCloseContainer.ResetTextures(closeType);
	}
}
