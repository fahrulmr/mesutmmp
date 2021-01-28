using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using System.Collections;
using UnityEngine.EventSystems;

public enum PanelType
{
	Panel,
	NotDependentPanel
}

public enum PanelState
{
	NotDefined,
	Opened,
	Closed,
	IsOpening,
	IsClosing
}

[System.Serializable]
public class PanelProperties
{
	[Header("When To Open")]
	public bool OnAwake = false;
	public bool WithParent = false;
	public bool WithChild = false;

	[Header("Before Opening")]
	public bool WaitForSameLevelToClose = true;
	[Header("Before Closing")]
	public bool WaitForChildenToClose = false;
}

[RequireComponent(typeof(CanvasGroup), typeof(Animator))]
public class GenericPanelBehaviour : MonoBehaviour
{
	public PanelType PanelType = PanelType.Panel;

	public PanelProperties PanelProperties;

	public UnityEvent OnPanelOpen;
	public UnityEvent OnPanelClose;

	public UnityEvent OnPanelOpenEnd;
	public UnityEvent OnPanelCloseEnd;


	private const string IS_OPENED = "IsOpened";
	private const string ON_OPEN = "OnOpen";
	private const string ON_CLOSE = "OnClose";
	private const string ON_OPEN_END = "OnOpenEnd";
	private const string ON_CLOSE_END = "OnCloseEnd";
	private const string ON_INIT = "OnInit";

	private const string PANEL_UPDATE = "PanelUpdate";
	private const string DEFAULT_ANIMATOR_NAME = "Animators/Default Panel Controller";

	private Animator animator;

	private PanelState panelState = PanelState.NotDefined;

	private Coroutine waitingForOthersToCloseBeforeClosingCoroutine;
	private Coroutine closingCoroutine;
	private Coroutine waitingForOthersToCloseBeforeOpeningCoroutine;
	private Coroutine openingCoroutine;

	private void Reset()
	{
		var defaultAnimator = Resources.Load(DEFAULT_ANIMATOR_NAME) as RuntimeAnimatorController;
		this.GetComponent<Animator>().runtimeAnimatorController = defaultAnimator;
	}

	protected void Awake()
	{
		if (RegisterIfNecessary())
		{
			Init();
		}
	}

	public int Init()
	{
		SendMessage(ON_INIT, SendMessageOptions.DontRequireReceiver);

		if (panelState != PanelState.NotDefined)
		{
			return 0;
		}

		animator = this.GetComponent<Animator>();

		if (PanelProperties.OnAwake)
		{
			Open();
		}

		if (panelState == PanelState.NotDefined)
		{
			CloseImmediately();
		}

		ForEachChild((childPanel) =>
			{
				childPanel.Init();
			});

		return 1;
	}

	void Start()
	{

	}

	void OnDisable()
	{
		panelState = PanelState.Closed;
	}

	private bool RegisterIfNecessary()
	{
		if (PanelManager.Instance != null)
		{
			return PanelManager.Instance.RegisterPanel(this);
		}

		return false;
	}

	private void OnDestroy()
	{
		if (PanelManager.Instance != null)
		{
			PanelManager.Instance.UnRegisterPanel(this);
		}
	}

	public Animator GetAnimator()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
		return animator;
	}

	public void Trigger()
	{
		if (!IsOpenedOrOpening())
		{
			Open();
		}
		else if (!IsClosedOrClosing())
		{
			CloseWithTime();
		}
	}

	private void StopCoroutines()
	{
		this.StopCoroutineIfNotNull(closingCoroutine);
		this.StopCoroutineIfNotNull(waitingForOthersToCloseBeforeOpeningCoroutine);
		this.StopCoroutineIfNotNull(waitingForOthersToCloseBeforeClosingCoroutine);
		this.StopCoroutineIfNotNull(openingCoroutine);
	}

	private void StopCoroutineIfNotNull(Coroutine coroutine)
	{
		if (coroutine != null)
		{
			this.StopCoroutine(coroutine);
		}
	}

	public void Open()
	{
		this.gameObject.SetActive(true);

		StopCoroutines();

		var maxTimeForOthersToClose = 0.0f;
		ForEachSameLevelPanel((sameLevelPanel) =>
			{
				if (sameLevelPanel.IsOpenedOrOpening() && (PanelType == sameLevelPanel.PanelType && PanelType != PanelType.NotDependentPanel) && sameLevelPanel != this)
				{
					var timeToClose = sameLevelPanel.CloseWithTime();
					maxTimeForOthersToClose = Mathf.Max(timeToClose, maxTimeForOthersToClose);
				}
			});

		maxTimeForOthersToClose = PanelProperties.WaitForSameLevelToClose ? maxTimeForOthersToClose : 0.0f;

		this.OnBaseOpen();

		waitingForOthersToCloseBeforeOpeningCoroutine = this.Invoke(maxTimeForOthersToClose, () =>
			{
				PerformOpening();
			});
	}

	private void PerformOpening()
	{
		if (!IsOpened())
		{	
			var prevTime = 1.0f;
			if (GetAnimator().GetCurrentAnimatorStateInfo(GetAnimator().GetLayerIndex("Panel")).IsName("PanelClosed"))
			{
				prevTime = Mathf.Min(1.0f, GetAnimator().GetCurrentAnimatorStateInfo(GetAnimator().GetLayerIndex("Panel")).normalizedTime);
			}
			GetAnimator().SetBool(IS_OPENED, true);

			GetAnimator().Play("PanelOpened", GetAnimator().GetLayerIndex("Panel"), 1.0f - prevTime);

			SendMessage(ON_OPEN, SendMessageOptions.DontRequireReceiver);
			OnPanelOpen.Invoke();

			openingCoroutine = this.Invoke(prevTime * GetOpeningTime(), () =>
				{
					SendMessage(ON_OPEN_END, SendMessageOptions.DontRequireReceiver);
					panelState = PanelState.Opened;
					OnPanelOpenEnd.Invoke();
				});
		}
	}

	public float GetMaxClosingTimeOfChildren()
	{
		GenericPanelBehaviour[] childPanels = transform.GetComponentsInChildren<GenericPanelBehaviour>();
		var maxClosingTime = 0.0f;
		for (int i = 0; i < childPanels.Length; i++)
		{
			var closingTime = childPanels[i].GetClosingTime();
			maxClosingTime = Mathf.Max(maxClosingTime, closingTime);
		}

		return maxClosingTime;
	}

	public float GetClosingTime()
	{
		float closingTime = GetAnimationLength(this.animator, "PanelClosed");

		// This means that the panel has mirrored opening and closed animations
		if (closingTime == -1.0f)
		{
			closingTime = GetOpeningTime();
		}

		return closingTime;
	}

	public float GetOpeningTime()
	{
		return GetAnimationLength(this.animator, "PanelOpened");
	}

	public int OnBaseOpen()
	{
		panelState = PanelState.IsOpening;

		var cursor = this.transform.parent;

		while (cursor != null && cursor.parent != null)
		{
			GenericPanelBehaviour[] parentPanels = cursor.parent.GetComponentsInChildren<GenericPanelBehaviour>(true);

			ForEachParent(cursor.parent, (parentPanel) =>
				{
					var childPanelAnimator = parentPanel.GetAnimator();

					if (parentPanel.IsClosed() && parentPanel.PanelProperties.WithChild)
					{
						parentPanel.Open();
					}
				});

			cursor = cursor.parent;
		}

		ForEachChild((childPanel) =>
			{
				var childPanelAnimator = childPanel.GetAnimator();

				if (childPanel.IsClosed() && childPanel.PanelProperties.WithParent)
				{
					childPanel.Open();
				}
			});

		return 1;
	}

	public float CloseWithTime()
	{
		float timeForOthersToClose = 0.0f;
		float timeToClose = 0.0f;
		if (IsOpenedOrOpening())
		{
			timeForOthersToClose = PanelProperties.WaitForChildenToClose ? this.OnBaseClose() : 0.0f;

			panelState = PanelState.IsClosing;
			timeToClose = GetClosingTime();

			waitingForOthersToCloseBeforeClosingCoroutine = this.Invoke(timeForOthersToClose, () =>
				{
					PerformClosing();
				});

		}

		return timeToClose + timeForOthersToClose;
	}

	public void Close()
	{
		CloseWithTime();
	}

	private void PerformClosing()
	{
		var prevTime = Mathf.Min(1.0f, GetAnimator().GetCurrentAnimatorStateInfo(GetAnimator().GetLayerIndex("Panel")).normalizedTime);
		GetAnimator().SetBool(IS_OPENED, false);
		GetAnimator().Play("PanelClosed", GetAnimator().GetLayerIndex("Panel"), 1.0f - prevTime);

		SendMessage(ON_CLOSE, SendMessageOptions.DontRequireReceiver);
		OnPanelClose.Invoke();

		closingCoroutine = this.Invoke(new WaitForSeconds(prevTime * GetClosingTime()), () =>
			{
				GenericPanelBehaviour[] sameLevelPanels = transform.GetComponentsInChildren<GenericPanelBehaviour>(true);

				SendMessage(ON_CLOSE_END, SendMessageOptions.DontRequireReceiver);
				OnPanelCloseEnd.Invoke();
				foreach (GenericPanelBehaviour sameLevelPanel in sameLevelPanels)
				{
					sameLevelPanel.gameObject.SetActive(false);
				}
				panelState = PanelState.Closed;
				this.gameObject.SetActive(false);
			});
	}

	private void CloseImmediately()
	{
		var panelAnimator = GetAnimator();
		panelAnimator.SetBool(IS_OPENED, false);

		this.GetComponent<CanvasGroup>().alpha = 0.0f;
		this.GetComponent<CanvasGroup>().interactable = false;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;

		this.gameObject.SetActive(false);
		panelState = PanelState.Closed;

		this.OnBaseCloseImmediately();
	}

	public bool IsOpened()
	{
		return panelState == PanelState.Opened;
	}

	public bool IsClosed()
	{
		return panelState == PanelState.Closed || panelState == PanelState.NotDefined;
	}

	public bool IsOpenedOrOpening()
	{
		return panelState == PanelState.Opened || panelState == PanelState.IsOpening;
	}

	public bool IsClosedOrClosing()
	{
		return panelState == PanelState.Closed || panelState == PanelState.IsClosing;
	}

	public int OnBaseCloseImmediately()
	{
		SendMessage(ON_CLOSE, SendMessageOptions.DontRequireReceiver);
		OnPanelClose.Invoke();
		ForEachChild((childPanel) =>
			{
				childPanel.CloseImmediately();
			});

		return 1;
	}

	public float OnBaseClose()
	{
		SendMessage(ON_CLOSE, SendMessageOptions.DontRequireReceiver);
		OnPanelClose.Invoke();
		float maxChildClosingTime = 0.0f;

		ForEachChild((childPanel) =>
			{
				var childClosingTime = childPanel.CloseWithTime();

				maxChildClosingTime = Mathf.Max(maxChildClosingTime, childClosingTime);
			});

		return maxChildClosingTime;
	}

	void ForEachSameLevelPanel(UnityAction<GenericPanelBehaviour> action)
	{
		if (transform.parent == null)
		{
			return;
		}

		GenericPanelBehaviour[] sameLevelPanels = transform.parent.GetComponentsInChildren<GenericPanelBehaviour>(true);

		foreach (GenericPanelBehaviour sameLevelPanel in sameLevelPanels)
		{
			if (sameLevelPanel.transform.parent != this.transform.parent)
			{
				continue;
			}

			action.Invoke(sameLevelPanel);
		}
	}

	void ForEachParent(Transform transform, UnityAction<GenericPanelBehaviour> action)
	{
		GenericPanelBehaviour[] parentPanels = transform.GetComponentsInChildren<GenericPanelBehaviour>(true);

		foreach (GenericPanelBehaviour parentPanel in parentPanels)
		{
			if (!CheckIfParentPanel(parentPanel))
			{
				continue;
			}

			action.Invoke(parentPanel);
		}
	}

	void ForEachChild(UnityAction<GenericPanelBehaviour> action)
	{
		GenericPanelBehaviour[] childPanels = transform.GetComponentsInChildren<GenericPanelBehaviour>(true);

		foreach (GenericPanelBehaviour childPanel in childPanels)
		{
			if (!CheckIfChildPanel(childPanel))
			{
				continue;
			}

			action.Invoke(childPanel);
		}
	}

	bool CheckIfParentPanel(GenericPanelBehaviour panel)
	{
		var parent = this.transform.parent;

		while (parent != null)
		{
			if (parent != panel.transform && parent.GetComponent<GenericPanelBehaviour>() == null)
			{
				parent = parent.transform.parent;
			}
			else if (parent != panel.transform && parent.GetComponent<GenericPanelBehaviour>() != null)
			{
				return false;
			}
			else if (parent == panel.transform)
			{
				return true;
			}
		}

		return false;
	}

	bool CheckIfChildPanel(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;

		while (parent != null)
		{
			if (parent != this.transform && parent.GetComponent<GenericPanelBehaviour>() == null)
			{
				parent = parent.transform.parent;
			}
			else if (parent != this.transform && parent.GetComponent<GenericPanelBehaviour>() != null)
			{
				return false;
			}
			else if (parent == this.transform)
			{
				return true;
			}
		}

		return false;
	}

	void BaseUpdate()
	{
		if (IsOpened())
		{
			SendMessage(PANEL_UPDATE, SendMessageOptions.DontRequireReceiver);
		}
	}

	private float GetAnimationLength(Animator anim, string track)
	{
		float length = -1.0f;
		if (anim != null && anim.runtimeAnimatorController != null)
		{
			for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
			{
				var animationClip = anim.runtimeAnimatorController.animationClips[i];
				if (animationClip.name == track)
				{
					length = animationClip.length;
				}
			}
		}
		return length;
	}

	public Coroutine Invoke(float delay, System.Action action)
	{
		if (delay == 0.0f)
		{
			action.Invoke();
			return null;
		}

		return StartCoroutine(InvokeHelper(new WaitForSeconds(delay), action));
	}

	public Coroutine Invoke(YieldInstruction instruction, System.Action action)
	{
		return StartCoroutine(InvokeHelper(instruction, action));
	}

	private IEnumerator InvokeHelper(YieldInstruction instruction, System.Action action)
	{
		yield return instruction;
		action.Invoke();
	}
}
