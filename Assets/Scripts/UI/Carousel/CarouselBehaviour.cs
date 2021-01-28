using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum CarouselState
{
	Idle,
	Grabbed,
	Released,
	Transitioning,
	Repelling
}

public enum ControlType
{
	Horizontal,
	Vertical,
	AlongPath
}

public enum CarouselType
{
	OneByOne,
	FreeScrolling
}

public class CarouselPath
{
	public List<PathPoint> Points;

	public List<Segment> Segments;

	public float Magnitude;

	public CarouselPath(List<Transform> transformPoints, float distanceBetweenFirstAndLast = -1.0f, bool avarage = true, bool elementsRetainPositions = true)
	{
		SetPath(transformPoints, distanceBetweenFirstAndLast, avarage, elementsRetainPositions);
	}

	public List<Vector3> GetVector3ArrayFromTransforms(List<Transform> transforms)
	{
		List<Vector3> result = new List<Vector3>();

		for (int i = 0; i < transforms.Count; i++)
		{
			result.Add(transforms[i].transform.localPosition);
		}

		return result;
	}

	public List<Vector3> AddFirstAndLastPoint(List<Vector3> Points, float distanceBetweenFirstAndLast)
	{
		List<Vector3> result = new List<Vector3>();


		var oldMagnitude = Magnitude;
		Magnitude += (distanceBetweenFirstAndLast * Magnitude);

		for (int i = 0; i < Points.Count + 2; i++)
		{
			if (i == 0)
			{
				result.Add(Points[0] + (Points[0] - Points[1]).normalized * distanceBetweenFirstAndLast * oldMagnitude / 2.0f);

			}
			else if (i == Points.Count + 1)
			{
				result.Add(Points[Points.Count - 1] + ((Points[Points.Count - 1] - Points[Points.Count - 2]).normalized * distanceBetweenFirstAndLast * oldMagnitude / 2.0f));

			}
			else
			{
				result.Add(Points[i-1]);
			}

		}

		return result;
	}

	public void SetPath(List<Transform> transforms, float distanceBetweenFirstAndLast = -1.0f, bool avarage = true, bool elementsRetainPositions = true)
	{
		Points = new List<PathPoint>();
		Segments = new List<Segment>();

		Magnitude = 0.0f;

		List<Vector3> transformPoints = GetVector3ArrayFromTransforms(transforms);
		for (int i = 0; i < transformPoints.Count - 1; i++)
		{
			Magnitude += (transformPoints[i + 1] - transformPoints[i]).magnitude;
		}


		var pointsAdded = false;
		if (distanceBetweenFirstAndLast >= 0.0f)
		{
			if (avarage)
			{
				distanceBetweenFirstAndLast = 1.0f / ((float)transformPoints.Count - 1);
			}

			pointsAdded = true;
			transformPoints = AddFirstAndLastPoint(transformPoints, distanceBetweenFirstAndLast);

			if (avarage)
			{
				distanceBetweenFirstAndLast = 1.0f / ((float)transformPoints.Count - 2);
			}
		}

		var currentLength = 0.0f;
		var paramPositionAccum = 0.0f;
		for (int i = 0; i < transformPoints.Count; i++)
		{
			Points.Add(new PathPoint());

			if (elementsRetainPositions)
			{
				Points[i].ParametricPosition = paramPositionAccum;

				if (pointsAdded)
				{
					if (i == 0 || i == transformPoints.Count - 2)
					{
						paramPositionAccum += distanceBetweenFirstAndLast / 2.0f;
					}
					else
					{
						paramPositionAccum += (1.0f - distanceBetweenFirstAndLast) / (transformPoints.Count - 3);
					}
				}
				else
				{
					
					Points[i].ParametricPosition = (float)i / (transformPoints.Count - 1);

				}
			}
			else
			{
				Points[i].ParametricPosition = currentLength / Magnitude;

				if (i < transformPoints.Count - 1)
				{
					currentLength += (transformPoints[i + 1] - transformPoints[i]).magnitude;
				}
			}

			Points[i].Position = transformPoints[i];
		}

		for (int i = 0; i < Points.Count; i++)
		{
			if (i < Points.Count - 1)
			{
				Segments.Add(new Segment(Points[i], Points[i + 1]));
			}
			if (i == 0)
			{
				Segments[i].IsFirst = true;
			}
			if (i == Points.Count - 2)
			{
				Segments[i].IsLast = true;
			}
		}

	}

	public Vector3 GetPosition(float parametricPosition)
	{
		var segment = GetPathSegment(parametricPosition);

		var parametricPositionOnSegment = parametricPosition;
		if (segment.End.ParametricPosition - segment.Start.ParametricPosition > Mathf.Epsilon)
		{
			parametricPositionOnSegment = (parametricPosition - segment.Start.ParametricPosition) / (segment.End.ParametricPosition - segment.Start.ParametricPosition);
		}

		return segment.GetPositionOnSegment(parametricPositionOnSegment);
	}

	private Segment GetPathSegment(float parametricPosition)
	{
		var segment = new Segment();

		for (int i = 0; i < Points.Count; i++)
		{
			if (i < Points.Count - 1)
			{
				if (Points[i].ParametricPosition.Equals(parametricPosition) || parametricPosition > Points[i].ParametricPosition && parametricPosition < Points[i + 1].ParametricPosition)
				{
					segment.Start = Points[i];
					segment.End = Points[i + 1];
					break;
				}
			}

			if (i == Points.Count - 1)
			{
				if (Points[i].ParametricPosition.Equals(parametricPosition) || parametricPosition > Points[i].ParametricPosition)
				{
					segment.Start = Points[i];
					segment.End = new PathPoint();
					segment.End.ParametricPosition = parametricPosition;
					segment.End.Position = Points[i].Position + (Points[i].Position - Points[i - 1].Position).normalized * Mathf.Abs(parametricPosition - Points[i].ParametricPosition) * Magnitude;
					break;
				}
			}

			if (i == 0)
			{
				if (parametricPosition < Points[i].ParametricPosition)
				{
					segment.Start = new PathPoint();
					segment.Start.parametricPosition = parametricPosition;
					segment.Start.Position = Points[0].Position + (Points[0].Position - Points[1].Position).normalized * Mathf.Abs(parametricPosition) * Magnitude;

					segment.End = Points[i];
					break;
				}
			}
		}

		return segment;
	}
	public PathPoint GetPathPoint(Vector3 point)
	{
		var minimumDistance = Mathf.Infinity;

		PathPoint ClosestPoint = null;
		Segment closestSegment = null;

		for (int i = 0; i < Segments.Count; i++)
		{
			float distance;
			var pointOnSegment = Segments[i].GetPointOnSegment(point, out distance);
			if (distance < minimumDistance)
			{
				closestSegment = Segments[i];
				ClosestPoint = pointOnSegment;
				minimumDistance = distance;
			}
		}

		return ClosestPoint;
	}

	public float GetParametricPosition(Vector3 point)
	{
		return GetPathPoint(point).ParametricPosition;
	}
}

public class Segment
{
	public PathPoint Start;
	public PathPoint End;
	public bool IsFirst;
	public bool IsLast;

	public Segment()
	{

	}

	public Segment(PathPoint start, PathPoint end)
	{
		Start = start;
		End = end;
	}

	public Vector3 GetPositionOnSegment(float parametricPosition)
	{
		var direction = End.Position - Start.Position;
		return Start.Position + direction * parametricPosition;
	}

	public PathPoint GetPointOnSegment(Vector3 pnt, out float distance)
	{
		Vector3 lineDir = End.Position - Start.Position;
		lineDir.Normalize();

		var result = new PathPoint();
		Vector3 firstOffset = Vector3.zero;
		if (IsFirst)
		{
			firstOffset = (lineDir * -100000.0f);
		}
		Vector3 lastOffset = Vector3.zero;
		if (IsLast)
		{
			lastOffset = (lineDir * 100000.0f);
		} 

		result.Position = ProjectPointLine(pnt, Start.Position + firstOffset, End.Position + lastOffset);

		var resultStartDistance = (result.Position - Start.Position).magnitude;
		var resultEndDistance = (result.Position - End.Position).magnitude;
		var resultPointDistance = (result.Position - pnt).magnitude;
		var startEndDistance = (Start.Position - End.Position).magnitude;

		var angle = Angle360(End.Position - Start.Position, result.Position - Start.Position);
		var sign = angle > 90.0f && angle < 270.0f ? -1.0f : 1.0f;

		result.ParametricPosition = Start.ParametricPosition + sign * resultStartDistance * (Mathf.Abs((End.ParametricPosition - Start.ParametricPosition)) / startEndDistance);

		distance = resultPointDistance;

		return result;
	}

	public Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 rhs = point - lineStart;
		Vector3 vector2 = lineEnd - lineStart;
		float magnitude = vector2.magnitude;
		Vector3 lhs = vector2;
		if (magnitude > 1E-06f)
		{
			lhs = (Vector3)(lhs / magnitude);
		}
		float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
		return (lineStart + ((Vector3)(lhs * num2)));
	}

	float Angle360(Vector3 from, Vector3 to)
	{
		Vector3 right = Vector3.right;
		float angle = Vector3.Angle(from, to);
		return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;            
	}
}

public class PathPoint
{
	public float parametricPosition;

	public float ParametricPosition
	{
		get
		{
			return parametricPosition;
		}
		set
		{
			parametricPosition = value;
		}
	}

	public Vector3 position;

	public Vector3 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public CarouselPath path;
}

public class CarouselBehaviour : CaptureControlItemBehaviour, ISelectionHandler
{
	public ControlType ControlType = ControlType.Horizontal;
	public CarouselType CarouselType = CarouselType.OneByOne;

	public Transform Content;
	public Transform Selector;

	public bool ElementsRetainPositions = true;

	//public bool AbsoluteSpeed;
	[Range (0.0f, 2.0f)]
	public float OverallSpeed = 1.0f;

	[Range(0.0f, 1.0f)]
	public float GrabDrag = 1.0f;

	[Range(0.0f, 1.0f)]
	public float ItemGravity = 0.5f;

	[Range(0.0f, 1.0f)]
	public float ItemDrag = 0.5f;

	[Range(0.0f, 1.0f)]
	public float BoundsForce = 0.5f;

	[Range(0.0f, 1.0f)]
	public float BoundsDrag = 0.5f;

	[Range(0.0f, 1.0f)]
	public float SnapReaction = 0.5f;

	[Range(0.0f, 1.0f)]
	public float ScrollingDrag = 0.5f;

	[Range(0.0f, 1.0f)]
	public float DistanceBetweenFirstAndLast = 0.5f;

	public bool IsInfinite = false;

	public bool Avarage = true;
	public bool AllowBounceFromSides = false;

	public Transform SelectedTransform;

	public UnityEvent OnCarouselItemChange;

	private CarouselState _state = CarouselState.Released;

	public CarouselState state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
		}
	}

	private Vector3 startMousePosition;
	private Animator selectedAnimator;

	private Transform rememberedSelection;
	public Transform pendingTransformToTransitionTo;

	private const float DragMultiplier = 35.0f;
	private bool repelling = false;
	private bool shouldExecuteControlLostOnLateUpdate = false;
	private bool shouldSwipe = false;
	public float Velocity = 1.0f;
	public float Position = 0.0f;
	private float physicalMouseParametricPosition;
	private CarouselPath path;

	private float RepellMultiplier = 1.0f;

	void OnEnable() 
	{
		
	}

	void OnDisable() {
		
	}

	private void Awake()
	{
		base.Awake();

		RebuildPath();
	}

	public void RebuildPath()
	{
		//Debug.Log("Content.childCount " + Content.childCount);
		LayoutRebuilder.ForceRebuildLayoutImmediate(Content as RectTransform);

		List<Transform> children = new List<Transform>();
		for (int i = 0; i < Content.childCount; i++)
		{
			children.Add(Content.GetChild(i));
		}

		if (children.Count < 2)
		{
			return;
		}

		path = new CarouselPath(children, IsInfinite ? DistanceBetweenFirstAndLast : -1.0f, Avarage, ElementsRetainPositions);

		if (path.Magnitude == 0.0f)
		{
			path = null;
			return;
		}

		for (int i = 0; i < Content.childCount; i++)
		{
			var carouselItem = Content.GetChild(i).GetComponent<CarouselItemBehaviour>();
			if (carouselItem == null)
			{
				carouselItem = Content.GetChild(i).gameObject.AddComponent<CarouselItemBehaviour>();
			}
			UnselectItemLogic(carouselItem.transform);
			carouselItem.ParametricPosition = path.GetParametricPosition(Content.GetChild(i).localPosition);
		}


		Position = path.GetParametricPosition(Content.InverseTransformPoint(Selector.position));
		SelectedTransform = GetClosestToSelectorTransform();
	}

	public void OnSelectionAttemptBegin()
	{
		startMousePosition = this.GetWorldMousePos();	
	}

	public void OnSelectionAttemptEnd()
	{
		if ((startMousePosition - this.GetWorldMousePos()).magnitude < 0.2f)
		{
			var pointer = new PointerEventData(EventSystem.current);

			pointer.position = Input.mousePosition;
			var raycastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointer, raycastResults);
			foreach (var raycastResult in raycastResults)
			{
				var captureControlItem = raycastResult.gameObject.GetComponent<CaptureControlItemBehaviour>();
				Transform item;

				if (CheckIfContentIsAnyOfParents(raycastResult.gameObject.transform, out item))
				{
					SelectItem(item);
					break;
				}
			}
		}
	}

	private bool CheckIfContentIsAnyOfParents(Transform transform, out Transform result)
	{
		var pointer = transform;
		result = null;
		while (pointer.parent != null)
		{
			if (pointer.parent == Content)
			{
				result = pointer;
				return true;
			}
			pointer = pointer.parent;
		}

		return false;
	}

	public override void OnControlCaptured()
	{
		OnSelectionAttemptBegin();
		state = CarouselState.Grabbed;

		physicalMouseParametricPosition = GetUniversalGrabPosition();

	}

	public override void OnControlLost()
	{
		shouldSwipe = this.gestureState == GestureState.Active;

		shouldExecuteControlLostOnLateUpdate = true;
	}

	private void ProcessSwipe()
	{
		var distance = (GetUniversalGrabPosition() - physicalMouseParametricPosition);

		if (distance > 0.0001f)
		{
			SelectPreviousItem();
		}
		else if (distance < -0.0001f)
		{
			SelectNextItem();
		}
	}

	public void SelectNextItem()
	{
		SelectItem(GetNextItem());
	}

	private List<CarouselItemBehaviour> GetSortedCarouselItemsList()
	{
		List<CarouselItemBehaviour> carouselItems = new List<CarouselItemBehaviour>();

		for (int i = 0; i < Content.childCount; i++)
		{
			var childTransform = Content.GetChild(i);
			carouselItems.Add(childTransform.GetComponent<CarouselItemBehaviour>());
		}

		carouselItems.Sort((item1, item2) => item1.ParametricPosition.CompareTo(item2.ParametricPosition));

		return carouselItems;
	}

	private Transform GetPreviousItem()
	{
		CarouselItemBehaviour selectedCarouselItem = SelectedTransform.GetComponent<CarouselItemBehaviour>();

		Transform result = SelectedTransform;
		List<CarouselItemBehaviour> carouselItems = GetSortedCarouselItemsList();

		var thisIndex = carouselItems.FindIndex((item) => item == selectedCarouselItem);
		var resultIndex = thisIndex - 1;
		if (resultIndex < 0)
		{
			if (IsInfinite)
			{
				resultIndex += carouselItems.Count;
			}
			else
			{
				resultIndex += 1;
			}

		}
		result = carouselItems[resultIndex].transform;

		return result;
	}

	private Transform GetNextItem()
	{
		CarouselItemBehaviour selectedCarouselItem = SelectedTransform.GetComponent<CarouselItemBehaviour>();

		Transform result = SelectedTransform;
		List<CarouselItemBehaviour> carouselItems = GetSortedCarouselItemsList();

		var thisIndex = carouselItems.FindIndex((item) => item == selectedCarouselItem);
		var resultIndex = thisIndex + 1;
		if (resultIndex > carouselItems.Count - 1)
		{
			if (IsInfinite)
			{
				resultIndex -= carouselItems.Count;
			}
			else
			{
				resultIndex -= 1;
			}
		}
		result = carouselItems[resultIndex].transform;

		return result;
	}

	public void SelectPreviousItem()
	{
		SelectItem(GetPreviousItem());
	}

	public void SelectItem(Transform item)
	{
		if (EditorSettings.EditorInstance.Сategories != null)
		{
			if (EditorSettings.EditorInstance.Сategories.Count > 0)
			{
				pendingTransformToTransitionTo = item;
				state = CarouselState.Transitioning;
			}
		}
	}

	private void FixedOnControlLost()
	{
		if (shouldExecuteControlLostOnLateUpdate)
		{
			state = CarouselState.Released;
			OnSelectionAttemptEnd();
			shouldExecuteControlLostOnLateUpdate = false;

			if (shouldSwipe && this.CarouselType == CarouselType.OneByOne)
				ProcessSwipe();
			shouldSwipe = false;
		}
	}

	private void LateUpdate()
	{
		base.LateUpdate();

		this.CaptureType = ControlType == ControlType.Horizontal ? CaptureType.Horizontal : CaptureType;
		this.CaptureType = ControlType == ControlType.Vertical ? CaptureType.Vertical : CaptureType;
		this.CaptureType = ControlType == ControlType.AlongPath ? CaptureType.Any : CaptureType;

		if (path == null)
		{
			RebuildPath();
		}
		if (path == null)
		{
			return;
		}

		FixedOnControlLost();
		NewProcessAnimations();
		NewModifyVelocityBasedOnSelected();
		NewApplyDragToVelocity();
		NewProcessGrab();
		NewProcessVelocityForChildren();
		ChangeStateToIdleWhenMovingSlowly();
	}

	private float NewGetRepellingForce()
	{
		var selectorPos = path.GetParametricPosition(Content.InverseTransformPoint(Selector.position));
		var minPos = float.MaxValue;
		var maxPos = float.MinValue;

		for (int i = 0; i < Content.childCount; i++)
		{
			var carouselItem = Content.GetChild(i).GetComponent<CarouselItemBehaviour>();

			if (carouselItem == null)
			{
				continue;
			}

			if (carouselItem.ParametricPosition + Velocity < minPos)
			{
				minPos = carouselItem.ParametricPosition + Velocity;
			}
			if (carouselItem.ParametricPosition + Velocity > maxPos)
			{
				maxPos = carouselItem.ParametricPosition + Velocity;
			}	

		}

		repelling = selectorPos < minPos - 0.01f|| selectorPos > maxPos + 0.01f;

		if (selectorPos < minPos)
		{
			return - (minPos - selectorPos);
		}
		if (selectorPos > maxPos)
		{
			return - (maxPos - selectorPos);
		}

		return 0.0f;
	}

	private void NewProcessAnimations()
	{
		var prevSelected = SelectedTransform;

		var closestToSelectorTransform = GetClosestToSelectorTransform();

		if (selectedAnimator != null)
		{
			selectedAnimator.logWarnings = false;
			selectedAnimator.SetBool("IsSelected", true);
		}

		if (SelectedTransform != null && SelectedTransform != closestToSelectorTransform && state == CarouselState.Grabbed)
		{
			UnselectItemLogic(SelectedTransform);
			SelectedTransform = null;
			selectedAnimator = null;
		}

		if (TryToGetNewSelectedItem())
		{
			if (prevSelected != null)
			{
				UnselectItemLogic(prevSelected);
			}
			SelectItemLogic(SelectedTransform);
		}
	}

	private void NewProcessGrab()
	{
		if (state == CarouselState.Grabbed)
		{
			NewProcessGrabbingForChildren();
		}
	}

	private void ChangeStateToIdleWhenMovingSlowly()
	{
		if ((state == CarouselState.Released) && Mathf.Abs(Velocity) < float.Epsilon)
		{
			state = CarouselState.Idle;
		}
		if (state == CarouselState.Transitioning && Mathf.Abs(Velocity) < float.Epsilon && GetClosestToSelectorTransform() == SelectedTransform)
		{
			state = CarouselState.Idle;
		}
	}

	private void NewProcessVelocityForChildren()
	{
		var repellingDistance = NewGetRepellingForce();
		var repellingForce = repellingDistance * (1.0f - BoundsForce) * (state == CarouselState.Grabbed ? 1.0f : (1.0f - BoundsDrag) / 50.0f) ;

		Velocity += repellingForce;

		physicalMouseParametricPosition += Velocity;
		Position -= Velocity;
		var selectorParamPos = path.GetParametricPosition(Content.InverseTransformPoint(Selector.position));

		if (Position < 0.0f)
		{
			Position += 1.0f;
		}
		else if (Position > 1.0f)
		{
			Position -= 1.0f;
		}
		for (int i = 0; i < Content.childCount; i++)
		{
			var carouselItem = Content.GetChild(i).GetComponent<CarouselItemBehaviour>();

			if (carouselItem == null)
			{
				return;
			}

			carouselItem.ParametricPosition += Velocity;

			if (IsInfinite)
			{
				if (carouselItem.ParametricPosition > selectorParamPos + 0.5f)
				{
					carouselItem.ParametricPosition -= 1.0f;
				}
				else if (carouselItem.ParametricPosition < selectorParamPos - 0.5f)
				{
					carouselItem.ParametricPosition += 1.0f;
				}
			}

			carouselItem.SelectorParametricPosition = selectorParamPos;
			carouselItem.transform.localPosition = path.GetPosition(carouselItem.ParametricPosition);
		}
	}

	private void NewProcessGrabbingForChildren()
	{
		Velocity = (GetUniversalGrabPosition() - physicalMouseParametricPosition);
	}

	private Vector3 GetGrabPosition()
	{
		return this.GetWorldMousePos();
	}

	private float GetUniversalGrabPosition()
	{
		var magnitude = path != null ? path.Magnitude : 1.0f;
	
		var newWorldPos = this.GetGrabPosition();
		float result = 0.0f;
		newWorldPos = Content.InverseTransformPoint(newWorldPos);

		if (this.ControlType == ControlType.AlongPath)
		{
			result = path.GetParametricPosition(newWorldPos);
		}
		if (this.ControlType == ControlType.Horizontal)
		{
			result = newWorldPos.x / magnitude;
		}
		if (this.ControlType == ControlType.Vertical)
		{
			result = newWorldPos.y / magnitude;
		}

		return result;
	}

	private bool TryToGetNewSelectedItem()
	{
		var changed = false;
		var prevSelected = SelectedTransform;

		if (state != CarouselState.Transitioning /*&& state != CarouselState.Grabbed*/)
		{
			SelectedTransform = GetClosestToSelectorTransform();
			selectedAnimator = SelectedTransform.GetComponent<Animator>();
		}
		else if (state == CarouselState.Transitioning && pendingTransformToTransitionTo != null)
		{
			rememberedSelection = SelectedTransform;
			SelectedTransform = pendingTransformToTransitionTo;
			selectedAnimator = SelectedTransform.GetComponent<Animator>();
			pendingTransformToTransitionTo = null;
		}

		if (prevSelected != SelectedTransform)
		{
			changed = true;
			OnCarouselItemChange.Invoke();
		}

		if ((state == CarouselState.Released && CarouselType == CarouselType.OneByOne) || (state == CarouselState.Released) && (!repelling || !AllowBounceFromSides) && (Mathf.Abs(Velocity) < SnapReaction * 0.05f))
		{
			state = CarouselState.Transitioning;
		}
		return changed;
	}

	private void NewModifyVelocityBasedOnSelected()
	{
		if (SelectedTransform != null && state == CarouselState.Transitioning)
		{
			var selectedParametricPosition = SelectedTransform.GetComponent<CarouselItemBehaviour>().ParametricPosition;
			var selectorParametricPosition = path.GetParametricPosition(Content.InverseTransformPoint(Selector.position));

			Velocity = Mathf.Lerp(Velocity, selectorParametricPosition - selectedParametricPosition, ItemGravity * 0.15f);
		}
	}

	private void NewApplyDragToVelocity()
	{
		if (state == CarouselState.Grabbed)
		{
			Velocity *= Mathf.Clamp01(1.0f - GrabDrag * DragMultiplier * 0.017f);
		}
		if (state != CarouselState.Grabbed)
		{
			Velocity *= Mathf.Clamp01(1.0f - ScrollingDrag * DragMultiplier * 0.006f);
		}
		if (state == CarouselState.Transitioning)
		{
			Velocity *= Mathf.Clamp01(1.0f - ItemDrag * DragMultiplier * 0.017f);
		}
		if (repelling)
		{
			Velocity *= Mathf.Clamp01(1.0f - BoundsDrag * DragMultiplier * 0.017f);
		}
	}

	private Transform GetClosestToSelectorTransform()
	{
		Transform result = null;

		Vector3 selectorPosition = Selector.position;
		float minD = float.MaxValue;

		for (int i = 0; i < Content.childCount; i++)
		{
			var childPos = Content.GetChild(i).position;
			var d = Vector3.SqrMagnitude(childPos - selectorPosition);
			if (minD > d)
			{
				if (result == null)
				{
					result = Content.GetChild(i);
				}
				else
				{
					result = Content.GetChild(i);
				}
				minD = d;
			}
		}

		return result;
	}

	private void UnselectItemLogic(Transform item)
	{
		if (item != null)
		{
			var animator = item.GetComponent<Animator>();

			if (animator != null)
			{
				animator.logWarnings = false;
				var prevTime = 0.0f;
				if (animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Carousel")).IsName("ItemSelected"))
				{
					prevTime = Mathf.Min(1.0f, animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Carousel")).normalizedTime);
				}
				animator.SetBool("IsSelected", false);

				animator.Play("ItemUnselected", animator.GetLayerIndex("Carousel"), 1.0f - prevTime);
			}

			item.SendMessage("OnUnselected", SendMessageOptions.DontRequireReceiver);
			item.SendMessage("OnUnselectedBase", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SelectItemLogic(Transform item, bool sendMessage = true)
	{
		if (item != null)
		{
			var animator = item.GetComponent<Animator>();

			if (animator != null)
			{
				animator.logWarnings = false;
				var prevTime = 1.0f;
				if (animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Carousel")).IsName("ItemUnselected"))
				{
					prevTime = Mathf.Min(1.0f, animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Carousel")).normalizedTime);
				}
				animator.SetBool("IsSelected", true);

				animator.Play("ItemSelected", animator.GetLayerIndex("Carousel"), 1.0f - prevTime);
			}

			if (sendMessage)
			{
				item.SendMessage("OnSelected", SendMessageOptions.DontRequireReceiver);
				item.SendMessage("OnSelectedBase", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
