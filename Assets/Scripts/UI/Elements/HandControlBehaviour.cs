using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControlBehaviour : MonoBehaviour
{
    public GameObject LeftHand;
    public GameObject RightHand;
    public float IgnoreRadius = 30.0f;
    public int SmoothFactor = 5;
    public float CoordinateScaler = 1.2f;

    public AstraManager kinectManager;
    public JointScreenOverlayer overlayer;
    public PlayerTrackerBehaviour playerTracker;
    public List<Vector2> leftHandLastPositions = new List<Vector2>();
    public List<Vector2> rightHandlastPositions = new List<Vector2>();

    void Start()
    {
        kinectManager = AstraManager.Instance;
        overlayer = FindObjectOfType<JointScreenOverlayer>();
        playerTracker = FindObjectOfType<PlayerTrackerBehaviour>();
        IgnoreRadius = PanelManager.Instance.GetPanel<SettingsPanelBehaviour>().Settings.IgnoreRadius;
    }

    void Update()
    {
        if (playerTracker != null && !playerTracker.IsUserTracked)
        {
            //            LeftHand.transform.position = Vector2.one * 100;
            //            RightHand.transform.position = Vector2.one * 100;
        }
        else
        {
            Vector2 leftHandPosition = LeftHand.transform.position;
            Vector2 newLeftHandPosition = GetSmoothedPosition(GetPosition(leftHandPosition, Astra.JointType.LeftHand), leftHandLastPositions);
            Vector2 rightHandPosition = RightHand.transform.position;
            Vector2 newRightHandPosition = GetSmoothedPosition(GetPosition(rightHandPosition, Astra.JointType.RightHand), rightHandlastPositions);

            //DEBUG START
            //Debug.Log("HANDCONTROLBEHAVIOUR - Left Hand position: "+ leftHandPosition+ " - NewLeftHandPosition: "+ newLeftHandPosition +" - Right hand Position: "+ rightHandPosition + " New Right Hand Position: "+ newRightHandPosition);
            //DEBUG LOG END

            if (Vector3.Distance(newLeftHandPosition, leftHandPosition) > IgnoreRadius)
            {
                leftHandPosition = newLeftHandPosition - (newLeftHandPosition - leftHandPosition).normalized * IgnoreRadius;
            }
            if (Vector3.Distance(newRightHandPosition, rightHandPosition) > IgnoreRadius)
            {
                rightHandPosition = newRightHandPosition - (newRightHandPosition - rightHandPosition).normalized * IgnoreRadius;
            }

            LeftHand.transform.position = leftHandPosition;
            RightHand.transform.position = rightHandPosition;
        }
    }

    private Vector3 GetPosition(Vector3 pos, Astra.JointType type)
    {

        Vector3 result = pos;
        if (kinectManager && kinectManager.isActiveAndEnabled)
        {
            var playerUserID = playerTracker.PlayerUserID;

            if (playerUserID != -1)
            {
                if (overlayer != null)
                {
                    Vector3 head = overlayer.GetScreenPosition(playerUserID, kinectManager.GetJointByType(kinectManager.Bodies[playerUserID], type), pos);
                    //DEBUG START
                    //Debug.Log("HANDCONTROLBEHAVIOUR - Player User ID: " + playerUserID + " - GetScreenPosition: " + head);
                    //DEBUG LOG END
                    Vector2 head2D = head;
                    if (head != pos)
                    {
                        Vector2 center = new Vector2(0.5f, 0.5f);
                        CoordinateScaler = Settings.Instance.ViewScale * 2.2f;
                        head2D = (head2D - center) * CoordinateScaler + center;
                        Vector2 newPos = new Vector2(head2D.x * Screen.width, head2D.y * Screen.height) + Settings.Instance.SkeletonShift + new Vector2(Settings.Instance.ViewPositionX, Settings.Instance.ViewPositionY);
                        result = newPos;
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

    private Vector2 GetSmoothedPosition(Vector2 pos, List<Vector2> lastPositions)
    {
        Vector2 smoothedPos = Vector2.zero;
        lastPositions.Add(pos);
        if (lastPositions.Count > SmoothFactor)
        {
            lastPositions.RemoveAt(0);
        }

        lastPositions.ForEach(p => smoothedPos += p);
        smoothedPos /= lastPositions.Count;
        return smoothedPos;
    }
}
