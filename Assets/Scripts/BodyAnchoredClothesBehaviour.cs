using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyAnchoredClothesBehaviour : MonoBehaviour
{
	public RawImage Image;
	public float MinMaxStepScale;
	public AdjusttheCameraBehaviour adjusttheCameraBehaviour;

	private BodyPartTrackerBehaviour bodyPartTracker;
	private float initScale;
	//private float scaler = 1.0f;
	private float clothesScale = 1.0f;
	private Vector2 ClothesPosition;
	private Vector2 Pivot;

	void Awake()
	{
		Settings.Instance.ClothesSkeletonScale = 0.0f;
		bodyPartTracker = FindObjectOfType<BodyPartTrackerBehaviour>();
		initScale = bodyPartTracker.UIScaler;
		ValidateAndSetScaler();
	}

	public void IncreaseScale()
	{
		Settings.Instance.ClothesSkeletonScale += MinMaxStepScale;
		ValidateAndSetScaler();
	}

	public void DecreaseScale()
	{
		if (Settings.Instance.SkeletonScale - MinMaxStepScale > 0)
		{
			Settings.Instance.ClothesSkeletonScale -= MinMaxStepScale;
			ValidateAndSetScaler();
		}
	}

	public void ValidateAndSetScaler()
	{
		//scaler = Mathf.Clamp(Settings.Instance.SkeletonScale, MinMaxStepScale.x, MinMaxStepScale.y);
		//bodyPartTracker.UIScaler = initScale * scaler;

		bodyPartTracker.UIScaler = (Settings.Instance.SkeletonScale + Settings.Instance.ClothesSkeletonScale) * clothesScale;
		bodyPartTracker.ClothesOfset = ClothesPosition;
		bodyPartTracker.Pivot = Pivot;
		if (adjusttheCameraBehaviour.transform.gameObject.activeSelf)
		{
			adjusttheCameraBehaviour.UpdateButtonText();
		}
	}

	public void SetNewCothes(ClothesItem clothesItem)
	{
		Image.SetAndFitTexture(clothesItem.Texture, false);
		clothesScale = clothesItem.Scale;
		Pivot = clothesItem.Position;
		ClothesPosition = clothesItem.Offset;
		ValidateAndSetScaler();
	}
}
