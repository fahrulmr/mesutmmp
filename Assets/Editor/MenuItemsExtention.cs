using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class MenuItemsExtention 
{
	#region Unity Methods
	#endregion

	#region Public Methods

	#endregion

	#region Private Methods
	[MenuItem("GameObject/ActivatePanelWithoutClosingOthers #e")]
	private static void ActivatePanelWithoutClosingOthers()
	{
		var go = Selection.activeObject as GameObject;

		bool valid = (go != null) && (go.GetComponent<CanvasGroup>() != null) && (go.GetComponent<Animator>() != null);
		if(valid)
		{
			var thisCg = go.GetComponent<CanvasGroup>();

			if (thisCg != null)
			{
				if (thisCg.alpha == 0.0f)
				{
					thisCg.alpha = 1.0f;
					thisCg.interactable = true;
				}
				else
				{
					thisCg.alpha = 0.0f;
					thisCg.interactable = false;
				}
			}
		}
	}

	[MenuItem("GameObject/ActivatePanel #w")]
	private static void ActivatePanel()
	{
		var go = Selection.activeObject as GameObject;

		bool valid = (go != null) && (go.GetComponent<CanvasGroup>() != null) && (go.GetComponent<Animator>() != null);
		if(valid)
		{
			var thisCg = go.GetComponent<CanvasGroup>();
			Transform parent = go.transform.parent;
			CanvasGroup cg;
			foreach (Transform child in parent )
			{
				cg = child.gameObject.GetComponent<CanvasGroup>();
				if (cg != null && thisCg != cg)
				{
					cg.alpha = 0.0f;
					cg.interactable = false;
				}
			}

			thisCg = go.GetComponent<CanvasGroup>();
			if (thisCg != null)
			{
				if (thisCg.alpha == 0.0f)
				{
					thisCg.alpha = 1.0f;
					thisCg.interactable = true;
				}
				else
				{
					thisCg.alpha = 0.0f;
					thisCg.interactable = false;
				}
			}
		}
	}

	[ MenuItem( "GameObject/Create Other/Create Panel #n" ) ]
	private static void CreatePanel()
	{
		var go = new GameObject();
		var selectedGo = Selection.activeObject as GameObject;

		go.transform.parent = selectedGo.transform;
		go.AddComponent<GenericPanelBehaviour>();
		var panelRectTransform = go.AddComponent<RectTransform>();

		panelRectTransform.anchorMin = new Vector2(0, 0);
		panelRectTransform.anchorMax = new Vector2(1, 1);
		panelRectTransform.sizeDelta = new Vector2(0.0f, 0.0f);

		panelRectTransform.pivot = new Vector2(0.5f, 0.5f);
		panelRectTransform.localScale = Vector3.one;
		panelRectTransform.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

		go.name = "New Panel";
		Selection.activeGameObject = go;
		if (GameObject.FindObjectOfType<PanelManager>() == null)
		{
			var panelManager = new GameObject();
			panelManager.AddComponent<PanelManager>();
			panelManager.name = "Panel Manager";
		}
	}
	#endregion
}