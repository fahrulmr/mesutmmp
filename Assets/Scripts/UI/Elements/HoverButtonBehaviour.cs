using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public float InitialDelay
	{
		get
		{
			return PanelManager.Instance.GetPanel<SettingsPanelBehaviour>().Settings.ButtonPressTime;
		}
	}

	public float RepeatDelay;
	public bool ReadFromSettings = true;

//	public float RepeatDelay
//	{
//		get
//		{
//			return PanelManager.Instance.GetPanel<SettingsPanelBehaviour>().Settings.ButtonNextClickWaitTime;
//		}
//	}

	public Color TargetColor;
	public bool IsActive = true;

	private bool isInside = false;
	private bool hasDelayPassed = false;
	private bool cancelStarted = false;
	private bool isHovering = false;

	private GameObject currentOverlapping = null;

	void Awake()
	{
		if (ReadFromSettings)
		{
			RepeatDelay = PanelManager.Instance.GetPanel<SettingsPanelBehaviour>().Settings.ButtonNextClickWaitTime;
		}
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter(PointerEventData eventData)
	{
//		isHovering = true;
//		this.BroadcastMessage("Hover", TargetColor, SendMessageOptions.DontRequireReceiver);
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit(PointerEventData eventData)
	{
//		isHovering = false;
	}

	#endregion

	void OnTriggerEnter2D(Collider2D other)
	{
		if (currentOverlapping == null && other.tag == "Input")
		{
			currentOverlapping = other.gameObject;
			isHovering = true;
			this.BroadcastMessage("Hover", TargetColor, SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject == currentOverlapping && other.tag == "Input")
		{
			isHovering = false;
			currentOverlapping = null;
		}
	}

	public void OnPointerHover(PointerEventData eventData)
	{
		if (cancelStarted)
		{
			cancelStarted = false;
			StopCoroutine("UnClickCoroutine");
		}

		if (isInside)
		{
			return;
		}

		isInside = true;

		StartCoroutine("ClickCoroutine");
	}

	public void OnPointerNotHover(PointerEventData eventData)
	{
		if (cancelStarted)
		{
			return;
		}

		cancelStarted = true;
		StartCoroutine("UnClickCoroutine");

	}

	private void Update()
	{
		if (!isHovering)
		{
			OnPointerNotHover(new PointerEventData(EventSystem.current));
		}

		if (!IsActive)
		{
			return;
		}

		if (isHovering)
		{
			OnPointerHover(new PointerEventData(EventSystem.current));
		}
	}

	IEnumerator UnClickCoroutine()
	{
		this.BroadcastMessage("UnHover", TargetColor, SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds(InitialDelay - 0.1f);

		StopCoroutine("ClickCoroutine");
		this.BroadcastMessage("OnClickCancel", SendMessageOptions.DontRequireReceiver);

		if (hasDelayPassed)
		{
			SendUpEvent();
		}

		hasDelayPassed = false;
		isInside = false;
		cancelStarted = false;
	}

	IEnumerator ClickCoroutine()
	{
		yield return new WaitForSeconds(InitialDelay);

		if (!isHovering)
		{
			yield break;
		}

		hasDelayPassed = true;

		SendDownEvent();
		SendClickEvent();
		if (this.GetComponent<Animator>() != null)
			this.GetComponent<Animator>().Play("Pressed", 0, 0.0f);
		while (true)
		{
			yield return new WaitForSeconds(RepeatDelay);
			if (InteractionManager.Instance == null || (InteractionManager.Instance != null && !InteractionManager.Instance.IsCursorHidden()))
			{
				SendClickEvent();
			}
		}
	}

	private void SendClickEvent()
	{
		if (this.GetComponent<Button>() != null && this.GetComponent<Button>().interactable && this.GetComponent<Button>().enabled)
		{
			this.GetComponent<Button>().OnPointerClick(new PointerEventData(EventSystem.current));
		}
	}

	private void SendDownEvent()
	{
		if (this.GetComponent<EventTrigger>() != null && this.GetComponent<EventTrigger>().enabled)
		{
			this.GetComponent<EventTrigger>().OnPointerDown(new PointerEventData(EventSystem.current));
		}
	}

	private void SendUpEvent()
	{
		if (this.GetComponent<EventTrigger>() != null && this.GetComponent<EventTrigger>().enabled)
		{
			this.GetComponent<EventTrigger>().OnPointerUp(new PointerEventData(EventSystem.current));
		}
	}
}
