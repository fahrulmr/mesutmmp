using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedClothesSetterBehaviour : MonoBehaviour
{
	public CarouselBehaviour SpecificClothesCarousel;
	public CarouselBehaviour ClothesTypeCarousel;

	private BodyPartTrackerBehaviour bodyPartTrackerBehaviour;
    private ClothingsSizesPanelBehaviour clothingsSizesPanelBehaviour;
	private BodyAnchoredClothesBehaviour bodyAnchoredClothes;
    private BuyInfoPanelBehaviour buyInfoPanel;

	void Awake()
	{
        clothingsSizesPanelBehaviour = PanelManager.Instance.GetPanel<ClothingsSizesPanelBehaviour>();
        bodyAnchoredClothes = FindObjectOfType<BodyAnchoredClothesBehaviour>();
        buyInfoPanel = FindObjectOfType<BuyInfoPanelBehaviour>();
		bodyPartTrackerBehaviour = FindObjectOfType<BodyPartTrackerBehaviour>();
        OnSelectedClothesChange();
	}

	void OnEnable()
	{
		SpecificClothesCarousel.OnCarouselItemChange.AddListener(OnSelectedClothesChange);
		ClothesTypeCarousel.OnCarouselItemChange.AddListener(OnSelectedClothesChange);
	}

	void OnDisable()
	{
		SpecificClothesCarousel.OnCarouselItemChange.RemoveListener(OnSelectedClothesChange);
		ClothesTypeCarousel.OnCarouselItemChange.RemoveListener(OnSelectedClothesChange);
	}

	public void OnSelectedClothesChange()
	{
		this.Invoke(new WaitForEndOfFrame(), () =>
			{
				var selected = SpecificClothesCarousel.SelectedTransform.GetComponent<ClothesItemBehaviour>().ClothesItem;
				bodyAnchoredClothes.SetNewCothes(selected);
                clothingsSizesPanelBehaviour.UpdateSizePanelButton(selected.ClothingSize);
				bodyPartTrackerBehaviour.JointType = selected.JointType;
                buyInfoPanel.UpdateBuyInfoPanel(selected);
				//Debug.Log("SelectedClothesSetterBehaviour - New clothes! Type: "+ selected.JointType+" - Clothing size: "+ selected.ClothingSize+" - Name: "+ selected.Name);
			});
	}
}
