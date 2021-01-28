using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PanelManager : MonoBehaviour
{
	private static PanelManager  instance;

	public static PanelManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<PanelManager>();

				if (FindObjectsOfType<PanelManager>().Length > 1)
				{
					Debug.LogError("There should never be more than 1 Panel Manager!");

					return instance;
				}
			}

			return instance;
		}
	}

	[HideInInspector]
	public Animator OpenedPanelAnimator;
	[HideInInspector]
	public Animator OpenedPopupAnimator;

	private GenericPanelBehaviour previousPanel;

	private GenericPanelBehaviour[] panels;

	private const string DEFAULT_ANIMATOR_NAME = "Animators/Default Panel Controller";

	protected PanelManager () {} // guarantee this will be always a singleton only - can't use the constructor!

	private void Awake()
	{
		panels = GameObject.FindObjectsOfType<GenericPanelBehaviour>();

		var defaultAnimator = Resources.Load(DEFAULT_ANIMATOR_NAME) as RuntimeAnimatorController;

		foreach (var panel in panels)
		{
			if (panel.GetComponent<Animator>().runtimeAnimatorController == null)
			{
				panel.GetComponent<Animator>().runtimeAnimatorController = defaultAnimator;
			}
		}

		foreach (var panel in panels)
		{
			RegisterPanel(panel);
		}

		foreach (var panel in panels)
		{
			if (IsRootPanel(panel))
			{
				panel.Init();
			}
		}
	}

	public void UnRegisterPanel(GenericPanelBehaviour panelToUnRegister)
	{
		List<GenericPanelBehaviour> panelsList = panels.ToList();
		panelsList.Remove(panelToUnRegister);
		panels = panelsList.ToArray();
	}

	public bool RegisterPanel(GenericPanelBehaviour panelToRegister)
	{
		foreach (var panel in panels)
		{
			if (panel == panelToRegister)
			{
				return false;
			}
		}

		List<GenericPanelBehaviour> panelsList = panels.ToList();
		panelsList.Add(panelToRegister);
		panels = panelsList.ToArray();

		return true;
	}

	private int GetNumberOfOpenedParents(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;
		var result = 0;

		while (parent != null)
		{
			if (parent.GetComponent<GenericPanelBehaviour>() != null)
			{
				var parentPanel = parent.GetComponent<GenericPanelBehaviour>();
				result += (parentPanel.IsOpened() && parentPanel.GetType() != panel.GetType()) ? 1 : 0;
			}

			parent = parent.transform.parent;
		}

		return result;
	}

	private bool IsRootPanel(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;

		while (parent != null)
		{
			if (parent.GetComponent<GenericPanelBehaviour>())
			{
				return false;
			}

			parent = parent.transform.parent;
		}

		return true;
	}

	private int GetNumberOfParents(GenericPanelBehaviour panel)
	{
		var parent = panel.transform.parent;
		var result = 0;

		while (parent != null)
		{
			result++;
			parent = parent.transform.parent;
		}

		return result;
	}

	private bool HasParent(GenericPanelBehaviour panel, MonoBehaviour parentToCheck = null)
	{
		var parent = panel.transform.parent;
		if (parentToCheck == null)
		{
			return true;
		}

		while (parent != null)
		{
			if (parent == parentToCheck.transform)
			{
				return true;
			}

			parent = parent.transform.parent;
		}

		return false;
	}

	public T GetPanel<T>() where T : GenericPanelBehaviour
	{
		GenericPanelBehaviour panel = null;
		var maxOpenedParents = int.MinValue;
		var minParents = int.MaxValue;
		List<GenericPanelBehaviour> panelsWithMaxOpenedParents = new List<GenericPanelBehaviour>();

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && openedParents > maxOpenedParents)
			{
				maxOpenedParents = openedParents;
			}
		}

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && openedParents == maxOpenedParents)
			{
				panelsWithMaxOpenedParents.Add(panels[i]);
			}
		}


		for (int i = 0; i < panelsWithMaxOpenedParents.Count; i++)
		{
			var parents = GetNumberOfParents(panelsWithMaxOpenedParents[i]);
			if (panelsWithMaxOpenedParents[i] is T && parents < minParents)
			{
				panel = panelsWithMaxOpenedParents[i];
				minParents = parents;
			}
		}

		return panel as T;
	}

	public T GetChildPanel<T>(MonoBehaviour parent) where T : GenericPanelBehaviour
	{
		GenericPanelBehaviour panel = null;
		var maxOpenedParents = int.MinValue;
		var minParents = int.MaxValue;
		List<GenericPanelBehaviour> panelsWithMaxOpenedParents = new List<GenericPanelBehaviour>();

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && HasParent(panels[i], parent) && openedParents > maxOpenedParents)
			{
				maxOpenedParents = openedParents;
			}
		}

		for (int i = 0; i < panels.Length; i++)
		{
			var openedParents = GetNumberOfOpenedParents(panels[i]);

			if (panels[i] is T && HasParent(panels[i], parent) && openedParents == maxOpenedParents)
			{
				panelsWithMaxOpenedParents.Add(panels[i]);
			}
		}

		for (int i = 0; i < panelsWithMaxOpenedParents.Count; i++)
		{
			var parents = GetNumberOfParents(panelsWithMaxOpenedParents[i]);
			if (panelsWithMaxOpenedParents[i] is T && parents < minParents)
			{
				panel = panelsWithMaxOpenedParents[i];
				minParents = parents;
			}
		}

		return panel as T;
	}

	public Animator GetAnimator<T>() where T : GenericPanelBehaviour
	{
		for (int i = 0; i < panels.Length; i++)
		{
			if (panels[i] is T)
			{
				return panels[i].GetComponent<Animator>();
			}
		}

		return null;
	}

	private Animator GetAnimator(GenericPanelBehaviour panel)
	{
		for (int i = 0; i < panels.Length; i++)
		{
			if (panels[i] == panel)
			{
				return panels[i].GetComponent<Animator>();
			}
		}

		return null;
	}

	public bool IsOpen<T>() where T : GenericPanelBehaviour
	{
		var panel = GetPanel<T>();


		return panel != null && panel.IsOpened();
	}

	public void OnOpenPanelClick(GenericPanelBehaviour panel) 
	{
		panel.Open();
	}

	public void OnClosePanelClick(GenericPanelBehaviour panel) 
	{
		panel.CloseWithTime();
	}

	public void OnTriggerPanelClick(GenericPanelBehaviour panel) 
	{
		panel.Trigger();
	}

	public GenericPanelBehaviour TriggerPanel(GenericPanelBehaviour panel) 
	{
		panel.Trigger();

		return panel;
	}

	public T TriggerPanel<T>() where T : GenericPanelBehaviour
	{
		var panel = GetPanel<T>();

		if (panel != null)
		{
			panel.Trigger();
		}

		return panel;
	}

	public GenericPanelBehaviour OpenPanel(GenericPanelBehaviour panel) 
	{
		panel.Open();

		return panel;
	}

	public T OpenPanel<T>() where T : GenericPanelBehaviour
	{
		var panel = GetPanel<T>();
			
		if (panel != null)
		{
			panel.Open();
		}

		return panel;
	}

	public GenericPanelBehaviour ClosePanel(GenericPanelBehaviour panel) 
	{
		Animator panelAnimator = GetAnimator(panel);

		panel.CloseWithTime();

		return panel;
	}

	public T ClosePanel<T>() where T : GenericPanelBehaviour
	{
		var panel = GetPanel<T>();

		if (panel != null)
		{
			panel.CloseWithTime();
		}

		return panel;
	}

	void Update()
	{
		foreach (var panel in panels)
		{
			panel.SendMessage("BaseUpdate", SendMessageOptions.DontRequireReceiver);
		}
	}
}