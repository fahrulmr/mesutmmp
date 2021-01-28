using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuyInfoPanelBehaviour : GenericPanelBehaviour
{
	public Text ClothesName;
	public Text ClothesPrice;
    private string URL;
 
    public void UpdateBuyInfoPanel(ClothesItem clothesItem){
        ClothesName.text = clothesItem.Name;
        ClothesPrice.text = clothesItem.Price;
        URL = clothesItem.URL;
    }

    public void FolowTheLink(){
        Application.OpenURL(URL);
    }
}
