using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PredefinedPositionHolder
{
	public Transform Transform;
	public List<Transform> PredefinedTransforms;
}

public class PositionMenu : MonoBehaviour
{
	public List<PredefinedPositionHolder> PredefinedPositionHolders;

	public void UpdatePositionMenu()
	{
		if (Settings.Instance.MenuPosition >= 0)
		{
			foreach (PredefinedPositionHolder transformPanel in PredefinedPositionHolders)
			{
				foreach (Transform transformPositionObject in  transformPanel.PredefinedTransforms)
				{
					transformPanel.Transform.SetParent(transformPanel.PredefinedTransforms[Settings.Instance.MenuPosition]);
					transformPanel.Transform.localPosition = Vector2.zero;
				}
			}
		}
	}
}
