using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum GestureState
{
	Possible,
	Active,
	NotPossible
}

public enum CaptureType
{
	Horizontal,
	Vertical,
	Any,
	DontCapture
}

public interface ISelectionHandler : IEventSystemHandler
{
	void OnSelectionAttemptBegin();
	void OnSelectionAttemptEnd();
}

public interface ISelectionClick : IEventSystemHandler
{
	void OnSelectionClick();
}

public class CaptureControlItemBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, ISelectionClick, ISelectionHandler, IInitializePotentialDragHandler
{
	public CaptureType CaptureType;

	protected GestureState gestureState = GestureState.NotPossible;
	private Vector3 startDragPosition;
	private Vector3 startLocalPosition;
	private Vector3 offset = Vector3.zero;

	private float dAngle = 40.0f;
	private float captureMagnitude = 0.1f;
	private bool isClickPossible = false;
	private const float	 MIN_CLICK_DISTANCE = 0.2f;
	public UnityEvent OnClick;

	private Canvas parentCanvas;

	protected void Awake()
	{
		startLocalPosition = this.transform.localPosition;
		var parentCanvases = this.GetComponentsInParent<Canvas>();

		parentCanvas = (parentCanvases != null && parentCanvases.Length > 0) ? parentCanvases[parentCanvases.Length - 1] : null;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		startDragPosition = GetWorldMousePos();
		isClickPossible = true;
		PropagateSelectionAttemptBegin();

		if (CaptureType == CaptureType.DontCapture)
		{
			PropagateMouseDown();
		}
			
		offset = Vector3.zero;
		if (CaptureType != CaptureType.DontCapture)
		{
			OnControlCaptured();
		
			gestureState = GestureState.Possible;
			startDragPosition = GetWorldMousePos();
			startLocalPosition = this.transform.localPosition;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (CaptureType == CaptureType.DontCapture)
		{
			PropagateBeginDrag(eventData);
		}
		else if (gestureState == GestureState.NotPossible)
		{
			PropagateBeginDrag(eventData);
		}
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		if (CaptureType == CaptureType.DontCapture)
		{
			PropagateEndDrag(eventData);
		}
		else if (gestureState == GestureState.NotPossible)
		{
			PropagateEndDrag(eventData);
		}
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (CaptureType == CaptureType.DontCapture)
		{
			PropagateInitializePotentialDrag(eventData);
		}
		else if (gestureState == GestureState.NotPossible)
		{
			PropagateInitializePotentialDrag(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (CaptureType == CaptureType.DontCapture)
		{
			PropagateDrag(eventData);
		}
		else if (gestureState == GestureState.NotPossible)
		{
			PropagateDrag(eventData);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (CaptureType == CaptureType.DontCapture)
		{
			PropagateMouseUp(eventData);
		}
		else if (gestureState != GestureState.Active)
		{
			PropagateMouseUp(eventData);
		}

		if (gestureState != GestureState.NotPossible)
		{
			OnControlLost();
			PropagateSelectionAttemptEnd();
			gestureState = GestureState.NotPossible;
		}
		else
		{
			PropagateSelectionAttemptEnd();
		}

		if ((startDragPosition - GetWorldMousePos()).magnitude < MIN_CLICK_DISTANCE && isClickPossible)
		{
			OnSelectionClick();
		}
	}

	public void OnSelectionClick()
	{
		OnClick.Invoke();
	}

	public virtual void OnControlCaptured()
	{

	}

	public virtual void OnControlLost()
	{

	}

	public void LateUpdate()
	{
		ProcessClick();
		if (CaptureType != CaptureType.DontCapture)
		{
			ProcessDragging();
		}
	}

	private void ProcessClick()
	{
		var dir = GetWorldMousePos() - startDragPosition;
		if (dir.magnitude > MIN_CLICK_DISTANCE)
		{
			isClickPossible = false;
		}
	}
	private void ProcessDragging()
	{
		if (gestureState == GestureState.Possible || gestureState == GestureState.Active)
		{
			TryGesture();
		}
	}

	private void TryGesture()
	{
		var dir = GetWorldMousePos() - startDragPosition;
		var minAgle = 90.0f - dAngle;
		var maxAngle = 90.0f + dAngle;
		var axis = CaptureType == CaptureType.Horizontal ? Vector3.up : Vector3.right;
		var angle = Vector3.Angle(dir, axis);

		offset = dir;
		if (gestureState == GestureState.Active || (angle > minAgle && angle < maxAngle) || this.CaptureType == CaptureType.Any)
		{
			if (gestureState != GestureState.Active && (offset.magnitude > captureMagnitude  || this.CaptureType == CaptureType.Any))
			{
				gestureState = GestureState.Active;
				PropagateMouseUp();
				PropagateEndDrag();
			}
		}
		else if (gestureState == GestureState.Possible)
		{
			gestureState = GestureState.NotPossible;
			PropagateMouseDown();
			PropagateInitializePotentialDrag();
			PropagateBeginDrag();
			OnControlLost();
		}
	}

	protected Vector3 GetWorldMousePos()
	{
		var mousePos = Input.mousePosition;

		if (parentCanvas != null)
		{
			if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay && parentCanvas.worldCamera != null)
			{
				mousePos = parentCanvas.worldCamera.ScreenToWorldPoint(Input.mousePosition);
			}
		}

		return mousePos;
	}


	private void PropagateBeginDrag(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}
		ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
	}

	private void PropagateEndDrag(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}
		ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
	}

	private void PropagateInitializePotentialDrag(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
		eventData = new PointerEventData(EventSystem.current);
		}
		ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
	}

	private void PropagateDrag(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}
		ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
	}

	private void PropagateMouseDown(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}
		ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
	}

	private void PropagateMouseUp(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}

		ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);
	}

	private void PropagateSelectionAttemptBegin(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}

		ExecuteEvents.ExecuteHierarchy<ISelectionHandler>(transform.parent.gameObject, eventData, (x, y) => x.OnSelectionAttemptBegin());
	}

	private void PropagateSelectionAttemptEnd(PointerEventData eventData = null)
	{
		if (eventData == null)
		{
			eventData = new PointerEventData(EventSystem.current);
		}

		ExecuteEvents.ExecuteHierarchy<ISelectionHandler>(transform.parent.gameObject, eventData, (x, y) => x.OnSelectionAttemptEnd());
	}

	public void OnSelectionAttemptBegin()
	{
		
	}

	public void OnSelectionAttemptEnd()
	{

	}
}
