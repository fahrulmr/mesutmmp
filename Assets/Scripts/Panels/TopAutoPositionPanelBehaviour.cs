using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TopAutoPositionPanelBehaviour : GenericPanelBehaviour
{
	public Astra.JointType JointType;
	public int DistanceToHead = 350;
	public float CoordinateScaler = 2.2f;
	public Transform MaxPositionPanelY;
	public Transform MinPositionPanelY;


	private PlayerTrackerBehaviour playerTracker;
	private JointScreenOverlayer overlayer;

	void OnInit ()
	{
		playerTracker = FindObjectOfType<PlayerTrackerBehaviour>();
		overlayer = FindObjectOfType<JointScreenOverlayer>();
	}

	public void ResetTopMenuPosition()
	{
		transform.position = new Vector2(transform.position.x, MaxPositionPanelY.position.y);
	}
 
	void PanelUpdate ()
	{
		AstraManager manager = AstraManager.Instance;
		if (Settings.Instance.AutoMenuPosition){
	        if (playerTracker != null && playerTracker.IsUserTracked)
	        {
	            var playerUserID = playerTracker.PlayerUserID;
	            if (playerUserID != -1)
	            {
	                if (overlayer != null)
	                {
	                    Vector3 head = overlayer.GetScreenPosition(playerUserID, manager.GetJointByType(manager.Bodies[playerUserID], JointType), transform.position);
	                    Vector2 head2D = head;
	                    if (head != transform.position)
	                    {
	                        Vector2 center = new Vector2(0.5f, 0.5f);
							CoordinateScaler = Settings.Instance.ViewScale * 2.2f;
	                        head2D = (head2D - center) * CoordinateScaler + center;
	                        Vector2 newPos = new Vector2(head2D.x * Screen.width, head2D.y * Screen.height);
	                        if (newPos.y + DistanceToHead > MinPositionPanelY.position.y && newPos.y + DistanceToHead < MaxPositionPanelY.position.y)
	                        {
	                            transform.position = new Vector2(transform.position.x, newPos.y + DistanceToHead);
	                        }
	                    }
	                }
	                else
	                {
	                    overlayer = FindObjectOfType<JointScreenOverlayer>();
	                }
	            }
	            
	        }
		}
    }
}
