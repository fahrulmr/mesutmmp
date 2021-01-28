using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ForegroundToImage : MonoBehaviour 
{
	RawImage guiImageTexture;
	private void Start()
	{
		guiImageTexture = GetComponent<RawImage>();
	}

	void Update () 
	{
		BackgroundRemovalManager backManager = BackgroundRemovalManager.Instance;

		if(backManager && backManager.IsBackgroundRemovalInitialized())
		{
			if(guiImageTexture.texture && guiImageTexture.texture == null)
			{
				guiImageTexture.texture = backManager.GetForegroundTex();
			}
		}
	}

}
