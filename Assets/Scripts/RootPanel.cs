using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootPanel : MonoBehaviour
{
	public void Update()
	{
		transform.localPosition = new Vector3(Settings.Instance.ViewPositionX, Settings.Instance.ViewPositionY);
		transform.localScale = new Vector3(Settings.Instance.ViewScale, Settings.Instance.ViewScale, transform.localScale.z);
	}
}
