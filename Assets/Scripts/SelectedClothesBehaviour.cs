using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedClothesBehaviour : MonoBehaviour
{
	public CarouselBehaviour SpecificClothesCarousel;

	public ClothesItem GetSelectedClose()
	{
		return  SpecificClothesCarousel.SelectedTransform.GetComponent<ClothesItemBehaviour>().ClothesItem;
	}
}
