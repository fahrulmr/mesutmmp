using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClothesOffsetSetterPanelBehaviour : GenericPanelBehaviour
{
    public float TimeInterval = 6.5f;
	public float MoveOffsetValue = 50;
	public AdjusttheCameraBehaviour adjusttheCameraBehaviour;


    private BodyPartTrackerBehaviour bodyPartTrackerBehaviour;
    private float startTime;
	 

	void OnInit ()
	{
        bodyPartTrackerBehaviour = FindObjectOfType<BodyPartTrackerBehaviour>();
	}
 
	void OnOpen ()
	{
        startTime = Time.time;
	}

	public void ClotnesOffssetMoveLeft()
	{
		Settings.Instance.ClothesOffset = new Vector2(Settings.Instance.ClothesOffset.x - MoveOffsetValue, Settings.Instance.ClothesOffset.y);
		UpdateButtons();
		startTime = Time.time;
	}

	public void ClotnesOffssetMoveRight(){
		Settings.Instance.ClothesOffset = new Vector2(Settings.Instance.ClothesOffset.x + MoveOffsetValue, Settings.Instance.ClothesOffset.y);
		UpdateButtons();
		startTime = Time.time;
	}

	public void ClotnesOffssetMoveUp(){
		Settings.Instance.ClothesOffset = new Vector2(Settings.Instance.ClothesOffset.x, Settings.Instance.ClothesOffset.y + MoveOffsetValue);
		UpdateButtons();
		startTime = Time.time;
	}

	public void ClotnesOffssetMoveDown(){
		Settings.Instance.ClothesOffset = new Vector2(Settings.Instance.ClothesOffset.x, Settings.Instance.ClothesOffset.y - MoveOffsetValue);
		UpdateButtons();
		startTime = Time.time;
	}

//	public void ReloadOffsset(){
//		Settings.Instance.ClothesOffset = new Vector2(0.0f, 0.0f);
//		UpdateButtons();
//		startTime = Time.time;
//	}

	private void UpdateButtons()
	{
		if (adjusttheCameraBehaviour.transform.gameObject.activeSelf)
		{
			adjusttheCameraBehaviour.UpdateButtonText();
		}
	}

    void Update()
    {
        if (Time.time - startTime > TimeInterval)
        {
            Close();
        }
    }
}
