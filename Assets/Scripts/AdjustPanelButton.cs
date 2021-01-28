using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AdjustPanelButton : MonoBehaviour 
{
	public Image Image;
	public Text Text;
	public UnityEvent onPressLeft = new UnityEvent();
	public UnityEvent onPressRight = new UnityEvent();


	void Awake () 
	{
		Image = GetComponent<Image>();
		Text = transform.GetChild(0).GetComponent<Text>();
	}

    public void PressLeft()
	{
		onPressLeft.Invoke();
    }

    public void PressRight()
	{
		onPressRight.Invoke();
    }
}
