using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClothingsSizesPanelBehaviour : GenericPanelBehaviour
{
    private ButtonSizes[] clothingSizeButton;

    void OnInit()
    {
        clothingSizeButton = FindObjectsOfType<ButtonSizes>();

    }

	public void UpdateSizePanelButton(ClothingSize clothesSize)
    {
        foreach (ButtonSizes button in clothingSizeButton)
        {
			button.gameObject.SetActive((clothesSize & button.ClothingSize) == button.ClothingSize);
        }
    }

}
