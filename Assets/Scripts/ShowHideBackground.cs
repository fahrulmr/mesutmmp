using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideBackground : MonoBehaviour {
	public RawImage ShowBackgroundImage;
	public RawImage HideBackgroundImage;
	public RawImage DistanceImageLow;
	public RawImage DistanceImageHigh;


	public void UpdateHideShowBackground(){
		if (Settings.Instance.ShowBackground && !ShowBackgroundImage.enabled)
		{
			ShowBackgroundImage.enabled = true;
			HideBackgroundImage.enabled = false;
//			DistanceImageHigh.enabled = false;
//			DistanceImageLow.enabled = true;
		}else if (!Settings.Instance.ShowBackground && !HideBackgroundImage.enabled)
		{
			ShowBackgroundImage.enabled = false;
			HideBackgroundImage.enabled = true;
//			DistanceImageLow.enabled = false;
//			DistanceImageHigh.enabled = true;
		}
	}
}
