using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AdjusttheCameraBehaviour : GenericPanelBehaviour
{
	private PositionMenu positionMenu;
	private BodyAnchoredClothesBehaviour bodyAnchoredClothesBehaviour;
	private NavigationBehaviour navigationBehaviour;
	private BrightnesContrast brightnesContrast;
	private TopAutoPositionPanelBehaviour topAutoPositionPanelBehaviour;

	void Awake()
	{
		positionMenu = FindObjectOfType<PositionMenu>();
		topAutoPositionPanelBehaviour = FindObjectOfType<TopAutoPositionPanelBehaviour>();
		bodyAnchoredClothesBehaviour = FindObjectOfType<BodyAnchoredClothesBehaviour>();
		brightnesContrast = FindObjectOfType<BrightnesContrast>();
		navigationBehaviour = transform.GetComponent<NavigationBehaviour>();
		//UpdateButtonText();
	}

	void OnOpen()
	{
		PanelManager.Instance.GetPanel<AdjustControlsAndBackgroundBehaviour>().Close();
		positionMenu.UpdatePositionMenu();
		UpdateButtonText();
	}

	public void UpdateButtonText()
	{
		navigationBehaviour.AdjustPanelButtons[0].Text.text = "Brightness: " + Settings.Instance.Brightness.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[1].Text.text = "Contrast: " + Settings.Instance.Contrast.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[2].Text.text = "View Position X: " + Settings.Instance.ViewPositionX.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[3].Text.text = "View Position Y: " + Settings.Instance.ViewPositionY.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[4].Text.text = "View Scale: " + Settings.Instance.ViewScale.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[5].Text.text = "Auto Menu Position: " + Settings.Instance.AutoMenuPosition;
		navigationBehaviour.AdjustPanelButtons[6].Text.text = "Skeleton Shift X: " + Settings.Instance.SkeletonShift.x;
		navigationBehaviour.AdjustPanelButtons[7].Text.text = "Skeleton Shift Y: " + Settings.Instance.SkeletonShift.y;
		navigationBehaviour.AdjustPanelButtons[8].Text.text = "Skeleton Scale: " + Settings.Instance.SkeletonScale.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[9].Text.text = "Tracking Start Left: " + Settings.Instance.TrackingStartLeft.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[10].Text.text = "Tracking Start Right: " + Settings.Instance.TrackingStartRight.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[11].Text.text = "Tracking Min Position: " + Settings.Instance.TrackingMinPosition.ToString("F2");
		navigationBehaviour.AdjustPanelButtons[12].Text.text = "Tracking Max Position: " + Settings.Instance.TrackingMaxPosition.ToString("F2");
	}
		
	public void Brightness(float Value)
	{
		Settings.Instance.Brightness += Value;
		brightnesContrast.UpdateBrightnessContrast();
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Brightness: " + Settings.Instance.Brightness.ToString("F2");
	}

	public void Contrast(float Value)
	{
		Settings.Instance.Contrast += Value;
		brightnesContrast.UpdateBrightnessContrast();
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Contrast: " + Settings.Instance.Contrast.ToString("F2");
	}

	public void ViewPositionX(float ValueX)
	{
		Settings.Instance.ViewPositionX += ValueX;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "View Position X: " + Settings.Instance.ViewPositionX.ToString("F2");
	}

	public void ViewPositionY(float ValueY)
	{
		Settings.Instance.ViewPositionY += ValueY;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "View Position Y: " + Settings.Instance.ViewPositionY.ToString("F2");
	}

	public void ViewScale(float Value)
	{
		Settings.Instance.ViewScale += Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "View Scale: " + Settings.Instance.ViewScale.ToString("F2");
	}

	public void AutoMenuPosition(bool Value)
	{
		Settings.Instance.AutoMenuPosition = Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Auto Menu Position: " + Settings.Instance.AutoMenuPosition;
		if (!Value)
		{
			topAutoPositionPanelBehaviour.ResetTopMenuPosition();
		}
	}

	public void SkeletonShiftX(float ValueX)
	{
		Settings.Instance.SkeletonShift = new Vector2(Settings.Instance.SkeletonShift.x + ValueX, Settings.Instance.SkeletonShift.y);
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Skeleton Shift X: " + Settings.Instance.SkeletonShift.x;
	}

	public void SkeletonShiftY(float ValueY)
	{
		Settings.Instance.SkeletonShift = new Vector2(Settings.Instance.SkeletonShift.x, Settings.Instance.SkeletonShift.y + ValueY);
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Skeleton Shift Y: " + Settings.Instance.SkeletonShift.y;
	}

	public void SkeletonScalePlus(float ValueX)
	{
		Settings.Instance.SkeletonScale += ValueX;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Skeleton Scale: " + Settings.Instance.SkeletonScale.ToString("F2");
		bodyAnchoredClothesBehaviour.ValidateAndSetScaler();
	}

	public void SkeletonScaleMinus(float ValueX)
	{
		Settings.Instance.SkeletonScale -= ValueX;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Skeleton Scale: " + Settings.Instance.SkeletonScale.ToString("F2");
		bodyAnchoredClothesBehaviour.ValidateAndSetScaler();
	}

	public void TrackingStartLeft(float Value)
	{
		Settings.Instance.TrackingStartLeft += Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Tracking Start Left: " + Settings.Instance.TrackingStartLeft.ToString("F2");
	}

	public void TrackingStartRight(float Value)
	{
		Settings.Instance.TrackingStartRight += Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Tracking Start Right: " + Settings.Instance.TrackingStartRight.ToString("F2");
	}

	public void TrackingMinPosition(float Value)
	{
		Settings.Instance.TrackingMinPosition += Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Tracking Min Position: " + Settings.Instance.TrackingMinPosition.ToString("F2");
	}

	public void TrackingMaxPosition(float Value)
	{
		Settings.Instance.TrackingMaxPosition += Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Tracking Max Position: " + Settings.Instance.TrackingMaxPosition.ToString("F2");
	}
}



