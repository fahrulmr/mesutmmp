using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class JointsButtonBehaviour : MonoBehaviour {
	public Button Button;
	public Astra.JointType JointType; 
	public bool ChildImage;

	private ColorBlock colorBlock;
	public Component[] ChildButtons;


	void Awake () {
		Button = transform.GetComponent<Button>();
		colorBlock = Button.colors;
		ChildButtons = GetComponentsInChildren(typeof(Image));
	}

	public void Select()
	{
		colorBlock.normalColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		Press();
	}

	public void Deselect()
	{
		colorBlock.normalColor = new Color(1.0f, 0.0f, 0.0f, 0.4f);
		Press();
	}

	private void Press()
	{
		if (!ChildImage)
		{
			Button.colors = colorBlock;
		}
		else
		{
			foreach (Image child in ChildButtons)
				child.color = colorBlock.normalColor;
		}
	}
}
