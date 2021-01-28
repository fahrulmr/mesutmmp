using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateWithPanelBehaviour : MonoBehaviour 
{
	public CarouselItemBehaviour PanelItem;
	public GenericPanelBehaviour Panel;

	private void Awake()
	{
		
		if (PanelItem != null)
		{
			var prevColor = this.GetComponent<RawImage>().color;

			this.GetComponent<RawImage>().color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.3f);

			PanelItem.OnItemSelected.AddListener(() => this.GetComponent<RawImage>().color = new Color(prevColor.r, prevColor.g, prevColor.b, 1.0f));
			PanelItem.OnItemUnselected.AddListener(() => this.GetComponent<RawImage>().color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.3f));	
		}
		else if (Panel != null)
		{
			var prevColor = this.GetComponent<Image>().color;

			//this.GetComponent<Image>().color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.3f);

			Panel.OnPanelOpen.AddListener(() => this.GetComponent<Image>().color = new Color(prevColor.r, prevColor.g, prevColor.b, 1.0f));
			Panel.OnPanelClose.AddListener(() => this.GetComponent<Image>().color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.3f));	
		}
	}	


	public void Highlight()
	{
		this.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}
	public void Unhighlight()
	{
		this.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
	}
}
