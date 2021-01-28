using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AdjustControlsAndBackgroundBehaviour : GenericPanelBehaviour
{
	private PositionMenu positionMenu;
	private NavigationBehaviour navigationBehaviour;
	private ShowHideBackground showHideBackground;

	void OnInit()
	{
		positionMenu = FindObjectOfType<PositionMenu>();
		navigationBehaviour = transform.GetComponent<NavigationBehaviour>();
		showHideBackground = FindObjectOfType<ShowHideBackground>();
		showHideBackground.UpdateHideShowBackground();
	}

	void OnOpen()
	{
		PanelManager.Instance.GetPanel<AdjusttheCameraBehaviour>().Close();
		positionMenu.UpdatePositionMenu();
		UpdateButtonText();
    }

	private void UpdateButtonText()
	{
		navigationBehaviour.AdjustPanelButtons[0].Text.text = "Menu Position: " + Settings.Instance.MenuPosition.ToString();
		navigationBehaviour.AdjustPanelButtons[1].Text.text = "Show Background: " + Settings.Instance.ShowBackground;
		navigationBehaviour.AdjustPanelButtons[2].Text.text = "Show/Hide Joints: " + Settings.Instance.HideShowJoints;
		navigationBehaviour.AdjustPanelButtons[3].Text.text = "Show/Hide Controls: " + Settings.Instance.HideShowControls;
		navigationBehaviour.AdjustPanelButtons[4].Text.text = "Show/Hide Info Pos: " + Settings.Instance.HideShowInfoPos;
		navigationBehaviour.AdjustPanelButtons[5].Text.text = "Show Trailer: " + Settings.Instance.ShowTrailer;
	}

    public void MenuPosition (int Value)
	{
		if (Settings.Instance.MenuPosition + Value >= 0 && Settings.Instance.MenuPosition + Value <= 8)
		{
			Settings.Instance.MenuPosition += Value;
			positionMenu.UpdatePositionMenu();
			navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Menu Position: " + Settings.Instance.MenuPosition.ToString();
		}
	}
 
	public void HideShowJoints (bool Value)
	{
		Settings.Instance.HideShowJoints = Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Show/Hide Joints: " + Settings.Instance.HideShowJoints;
	}

	public void HideShowControls (bool Value)
	{
		Settings.Instance.HideShowControls = Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Show/Hide Controls: " + Settings.Instance.HideShowControls;
	}

	public void ShowBackground (bool Value)
	{
		Settings.Instance.ShowBackground = Value;
		showHideBackground.UpdateHideShowBackground();
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Show Background: " + Settings.Instance.ShowBackground;
	}

	public void HideShowInfoPos (bool Value)
	{
		Settings.Instance.HideShowInfoPos = Value;
		navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Show/Hide Info Pos: " + Settings.Instance.HideShowInfoPos;
	}

    public void ShowTrailer(bool Value)
    {
        Settings.Instance.ShowTrailer = Value;
        navigationBehaviour.AdjustPanelButtons[navigationBehaviour.CurrentSelected].Text.text = "Show Trailer: " + Settings.Instance.ShowTrailer;
    }
}
