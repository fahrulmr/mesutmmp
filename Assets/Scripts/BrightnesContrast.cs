using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnesContrast : MonoBehaviour {
	private Material BrightnessContrast;

	void Awake(){
		BrightnessContrast = GetComponent<RawImage>().material;
		BrightnessContrast.SetFloat("_Brightness", Settings.Instance.Brightness);
		BrightnessContrast.SetFloat("_Contrast", Settings.Instance.Contrast);
	}

	public void UpdateBrightnessContrast () {
		BrightnessContrast.SetFloat("_Brightness", Settings.Instance.Brightness);
		BrightnessContrast.SetFloat("_Contrast", Settings.Instance.Contrast);
	} 
}
