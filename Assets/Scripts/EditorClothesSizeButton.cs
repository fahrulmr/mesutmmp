using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class EditorClothesSizeButton : MonoBehaviour
{
	public Sprite SelectedSprite;
	public Sprite NotSelectedSprite;
	public ClothingSize ClothingSize;

	private bool isSelected = false;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
	}

	private Image buttonSprite;

	void Awake()
	{
		buttonSprite = transform.GetComponent<Image>();
	}

	public void Select()
	{
		buttonSprite.sprite = SelectedSprite;
		isSelected = true;
	}

	public void Deselect()
	{
		buttonSprite.sprite = NotSelectedSprite;
		isSelected = false;
	}
}
