using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NavigationBehaviour : MonoBehaviour
{
	private Color colorSelectButton = new Color(0.8f, 0.8f, 0.8f, 1.0f);
	private Color colorNormalButton = new Color(0.5f, 0.5f, 0.5f, 1.0f);

	public int CurrentSelected = 0; 
	public AdjustPanelButton[] AdjustPanelButtons;
	public Transform Arrows;

	public void PressUp()
	{
		AdjustPanelButtons[CurrentSelected].Image.color = colorNormalButton;
		CurrentSelected = GetPrevIndex (CurrentSelected, AdjustPanelButtons.Length);
		AdjustPanelButtons[CurrentSelected].Image.color = colorSelectButton;
		Arrows.position = AdjustPanelButtons[CurrentSelected].transform.position;
	}

	public void PressDown()
	{
		AdjustPanelButtons[CurrentSelected].Image.color = colorNormalButton;
		CurrentSelected = GetNextIndex (CurrentSelected, AdjustPanelButtons.Length);
		AdjustPanelButtons[CurrentSelected].Image.color = colorSelectButton;
		Arrows.position = AdjustPanelButtons[CurrentSelected].transform.position;
	}

	private int GetNextIndex(int current, int count)
	{
		return current == count - 1 ? 0 : current + 1;
	}

	private int GetPrevIndex(int current, int count)
	{
		return current == 0 ? count - 1 : current - 1;
	}

	public void PressLeft()
	{
		AdjustPanelButtons[CurrentSelected].PressLeft();
	}

	public void PressRight()
	{
		AdjustPanelButtons[CurrentSelected].PressRight();
	}

	void OnOpen ()
	{
		CurrentSelected = 0;
		AdjustPanelButtons[CurrentSelected].Image.color = colorSelectButton;
		Arrows.position = AdjustPanelButtons[CurrentSelected].transform.position;
	}

	void OnClose ()
	{
		AdjustPanelButtons[CurrentSelected].Image.color = colorNormalButton;
	}


	void Update(){
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{ 
			PressUp();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			PressDown();
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			PressLeft();
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			PressRight();
		}
	}
}



