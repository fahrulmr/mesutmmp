using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainBehaviour : GenericPanelBehaviour
{
	public bool CursorVisible = false;

	void OnInit ()
	{
		Cursor.visible = CursorVisible;
	}
 
	void OnOpen ()
	{
 
	}
 
	void OnClose ()
	{
 
	}
 
	void OnApplicationQuit()
	{
		Settings.Instance.Save();
		PlayerPrefs.SetInt("FirstLoad", 0);
		PlayerPrefs.SetInt("LastSelectClothes", 0);

	}

	void PanelUpdate ()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
			Settings.Instance.Save();
            Application.Quit();
		}
		else if(Input.GetKeyDown(KeyCode.F10))
		{
			Application.LoadLevel("Security");
		}
		else if(Input.GetKeyDown(KeyCode.F12))
		{
			Settings.Instance.Save();
			Application.LoadLevel("Editor");
		}
	}

}
