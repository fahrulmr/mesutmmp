using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarouselItemBehaviour : MonoBehaviour 
{
	public UnityEvent OnItemSelected;
	public UnityEvent OnItemUnselected;

	public float ParametricPosition;
	public float SelectorParametricPosition;
	public float Velocity;

	void OnSelectedBase()
	{
		if (OnItemSelected != null)
		{
			OnItemSelected.Invoke();
		}
	}

	void OnUnselectedBase()
	{
		if (OnItemUnselected != null)
		{
			OnItemUnselected.Invoke();
		}
	}

	void Update()
	{

	}
}
