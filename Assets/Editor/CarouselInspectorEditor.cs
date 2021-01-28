using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;

[CustomEditor(typeof(CarouselBehaviour), true)]
public class CarouselInspectorEditor : Editor 
{
	private string scriptName = "";
	private bool waitForCompile = false;
	private string componentName;
	private GameObject targetGameObject;

	private bool wasEdited;
	private bool toggleValue;

	private bool additionalFoldout;
	private bool events;

	public void OnEnable()
	{
		scriptName = target.name;

	}

	public override void OnInspectorGUI()
	{
		var carousel = target as CarouselBehaviour;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlType"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("CarouselType"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Content"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Selector"));



		EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemGravity"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemDrag"));

		if (carousel.CarouselType == CarouselType.FreeScrolling)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ScrollingDrag"));
			if (!carousel.IsInfinite)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowBounceFromSides"));
			}
		}
		else
		{
			carousel.ScrollingDrag = 0.0f;
		}

		if (!carousel.IsInfinite || !Application.isPlaying || carousel.CarouselType == CarouselType.FreeScrolling)
		{
			additionalFoldout = EditorGUILayout.Foldout(additionalFoldout, "Additional", true);
			EditorGUI.indentLevel++;
			if (additionalFoldout)
			{
				if (!Application.isPlaying)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ElementsRetainPositions"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("IsInfinite"));
					if (carousel.IsInfinite)
					{
						EditorGUI.indentLevel++;
						if (!carousel.ElementsRetainPositions)
						{
							if (!carousel.Avarage)
							{
								EditorGUILayout.PropertyField(serializedObject.FindProperty("DistanceBetweenFirstAndLast"));
							}
							EditorGUILayout.PropertyField(serializedObject.FindProperty("Avarage"));
						}
						EditorGUI.indentLevel--;
					}
				}
				else
				{
					if (carousel.IsInfinite)
					{
						EditorGUILayout.TextArea("Infinite carousel");
						EditorGUILayout.Space();
					}
					else
					{
						EditorGUILayout.TextArea("Not Infinite carousel");
						EditorGUILayout.Space();
					}

				}

				if (!carousel.IsInfinite)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(serializedObject.FindProperty("BoundsForce"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("BoundsDrag"));
					EditorGUI.indentLevel--;
				}

				if (carousel.CarouselType == CarouselType.FreeScrolling)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SnapReaction"));
				}
			}
			EditorGUI.indentLevel--;
		}




		events = EditorGUILayout.Foldout(events, "Events", true);

		if (events)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnCarouselItemChange"));
		}


		serializedObject.ApplyModifiedProperties();
	}

	[ MenuItem( "GameObject/Create Other/Create Carousel" ) ]
	private static void CreateCarousel()
	{
		var selectedGo = Selection.activeObject as GameObject;

		var carouselGameObject = CreateGameObjectWithParent(selectedGo.transform, "Carousel");
		var carouselBehaviour = carouselGameObject.AddComponent<CarouselBehaviour>();
		var selectorGameObject = CreateGameObjectWithParent(carouselGameObject.transform, "Selector");
		var viewportGameObject = CreateGameObjectWithParent(carouselGameObject.transform, "Viewport");
		var viewportRectTransform = viewportGameObject.GetComponent<RectTransform>();
		viewportRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
		viewportRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
		viewportRectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
		viewportGameObject.AddComponent<RectMask2D>();

		var contentGameObject = CreateGameObjectWithParent(viewportGameObject.transform, "Content");
		var emptyImage = contentGameObject.AddComponent<RawImage>();
		emptyImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		var contentRectTransform = contentGameObject.GetComponent<RectTransform>();
		contentRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
		contentRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
		contentRectTransform.sizeDelta = new Vector2(0.0f, 0.0f);

		carouselBehaviour.Content = contentGameObject.transform;
		carouselBehaviour.Selector = selectorGameObject.transform;

		Selection.activeGameObject = carouselGameObject;
	}

	[ MenuItem( "GameObject/Create Other/Create Carousel Item" ) ]
	private static void CreateCarouselItem()
	{
		var selectedGo = Selection.activeObject as GameObject;

		var carouselGameObject = CreateGameObjectWithParent(selectedGo.transform, "Item");
		var carouselBehaviour = carouselGameObject.AddComponent<CarouselItemBehaviour>();
		carouselGameObject.AddComponent<RectMask2D>();

		Selection.activeGameObject = carouselGameObject;
	}

	private static GameObject CreateGameObjectWithParent(Transform parent, string name)
	{
		var go = new GameObject();
		go.transform.parent = parent;
		var rectTransform = go.AddComponent<RectTransform>();
		rectTransform.localScale = Vector3.one;
		rectTransform.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		go.name = name;

		return go;
	}
}
