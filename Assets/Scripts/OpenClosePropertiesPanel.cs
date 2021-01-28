using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenClosePropertiesPanel : MonoBehaviour {
	private AdjusttheCameraBehaviour AdjusttheCameraBehaviourCamera;
	private AdjustControlsAndBackgroundBehaviour AdjusttheCameraBehaviourCotrolsAndBackground;


    void Start () {
		AdjusttheCameraBehaviourCamera = PanelManager.Instance.GetPanel<AdjusttheCameraBehaviour>();
		AdjusttheCameraBehaviourCotrolsAndBackground = PanelManager.Instance.GetPanel<AdjustControlsAndBackgroundBehaviour>();
	}


	void Update () {
        if(Input.GetKeyDown(KeyCode.F7)){
			if (AdjusttheCameraBehaviourCamera.IsClosed())
			{
				AdjusttheCameraBehaviourCamera.Open();
			}
			else
				AdjusttheCameraBehaviourCamera.Close();
        }else if(Input.GetKeyDown(KeyCode.F8)){
			if (AdjusttheCameraBehaviourCotrolsAndBackground.IsClosed())
			{
				AdjusttheCameraBehaviourCotrolsAndBackground.Open();
			}
			else
				AdjusttheCameraBehaviourCotrolsAndBackground.Close();
        }
	}
}
