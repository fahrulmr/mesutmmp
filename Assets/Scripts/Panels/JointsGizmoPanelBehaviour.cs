using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct JointAnchoredTransform
{
    public Transform Transform;
    public Astra.JointType JointType;
}

public class JointsGizmoPanelBehaviour : GenericPanelBehaviour
{
    public float CoordinateScaler = 2.2f;
    public Vector2 offset;
    public List<JointAnchoredTransform> anchoredTransforms;

    private PlayerTrackerBehaviour playerTracker;
    private JointScreenOverlayer overlayer;

    void OnInit()
    {
        playerTracker = FindObjectOfType<PlayerTrackerBehaviour>();
        overlayer = FindObjectOfType<JointScreenOverlayer>();
    }


    void Update()
    {
        if (playerTracker != null && playerTracker.IsUserTracked && Settings.Instance.HideShowJoints && playerTracker.PlayerUserID != -1)
        {
            anchoredTransforms.ForEach(at =>
                {
                    at.Transform.gameObject.SetActive(true);
                    at.Transform.position = GetPosition(new Vector2(-100.0f, -100.0f), at.JointType);
                });
        }
        else
        {
            anchoredTransforms.ForEach(at => at.Transform.gameObject.SetActive(false));
        }
    }

    private Vector3 GetPosition(Vector3 pos, Astra.JointType type)
    {
        AstraManager manager = AstraManager.Instance;
        Vector3 result = pos;
        if (playerTracker != null)
        {
            var playerUserID = playerTracker.PlayerUserID;

            if (playerUserID != -1)
            {
                if (overlayer != null)
                {
                    Vector3 head = overlayer.GetScreenPosition(playerUserID, manager.GetJointByType(manager.Bodies[playerUserID], type), pos);
                    Vector2 head2D = head;

                    if (head != pos && head != null)
                    {
                        Vector2 center = new Vector2(0.5f, 0.5f);
                        CoordinateScaler = Settings.Instance.ViewScale * 2.2f;
                        head2D = (head2D - center) * CoordinateScaler + center;
                        offset = Settings.Instance.SkeletonShift + new Vector2(Settings.Instance.ViewPositionX, Settings.Instance.ViewPositionY);
                        Vector2 newPos = new Vector2(head2D.x * Screen.width, head2D.y * Screen.height) + offset;
                        result = newPos;
                        //Debug.Log("JointsGizmo "+newPos+" - type"+type);
                    }
                }
                else
                {
                    overlayer = FindObjectOfType<JointScreenOverlayer>();
                }
            }
        }
        return result;
    }
}
