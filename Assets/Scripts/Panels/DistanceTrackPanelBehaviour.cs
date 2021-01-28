using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DistanceTrackPanelBehaviour : GenericPanelBehaviour
{
	public Text DistanceText;
	public RawImage BodyDistanceTrack;

	private PlayerTrackerBehaviour playerTracker;

	void OnInit ()
	{
		playerTracker = FindObjectOfType<PlayerTrackerBehaviour>();
	}
 
	void Update ()
	{
		HandleIcons();
		HandleDistanceText();
	}

	private void HandleDistanceText()
	{
		if (Settings.Instance.HideShowInfoPos)
		{
			DistanceText.gameObject.SetActive(true);

			if (playerTracker != null && playerTracker.IsUserTracked)
			{
				DistanceText.text = playerTracker.DistanceToUser.ToString("F");
			}
			else
			{
				DistanceText.text = "Not In Range";
			}
		}
		else
		{
			if(DistanceText.isActiveAndEnabled)
				DistanceText.gameObject.SetActive(false);
		}
	}

	private void HandleIcons()
	{
		if (playerTracker != null && playerTracker.IsUserTracked)
		{
			BodyDistanceTrack.material.color=new Color(0.0f, 0.5f, 1.0f, 0.8f);
		}
		else
		{
			BodyDistanceTrack.material.color=new Color(0.8f, 0.8f, 0.8f, 0.8f);
		}
	}

}
