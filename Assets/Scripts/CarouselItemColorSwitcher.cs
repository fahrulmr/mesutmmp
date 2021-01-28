using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class CarouselItemColorSwitcher : MonoBehaviour
{
	public Color SelectedColor;
	public Color DefaultColor;
	[Range(0.0f, 2.0f)]
	public float TweenTime = 0.2f;

	public CarouselBehaviour Carousel;

	private RawImage image;
	private bool isSelected = false;

	void Awake()
	{
		image = GetComponent<RawImage>();
	}

	void OnEnable()
	{
		Carousel.OnCarouselItemChange.AddListener(OnCarouselItemChange);
	}

	void Start()
	{
		Carousel.OnCarouselItemChange.AddListener(OnCarouselItemChange);
		if (Carousel.SelectedTransform == this.transform)
		{
			isSelected = true;
		}
	}

	void OnDisable()
	{
		Carousel.OnCarouselItemChange.RemoveListener(OnCarouselItemChange);
	}

	void OnCarouselItemChange()
	{
		if (Carousel.SelectedTransform == this.transform)
		{
			isSelected = true;
			this.Tween(TweenTime, 0.0f, 1.0f, v => image.color = Color.Lerp(DefaultColor, SelectedColor, v));
		}
		else
		{
			if (isSelected)
			{
				isSelected = false;
				this.Tween(TweenTime, 0.0f, 1.0f, v => image.color = Color.Lerp(SelectedColor, DefaultColor, v));
			}
		}
	}
}
