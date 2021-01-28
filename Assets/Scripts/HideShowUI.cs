using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShowUI : MonoBehaviour
{
	public GameObject UI;

	public void Update()
	{
		if (UI.activeSelf != Settings.Instance.HideShowControls)
		{
			UI.SetActive(Settings.Instance.HideShowControls);
		}
	}
}
