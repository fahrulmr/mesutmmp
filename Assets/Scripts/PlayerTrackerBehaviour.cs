using Astra;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerTrackerBehaviour : MonoBehaviour
{
    public float trackDistance = 2000;
    private int playerIndex = 0;

    public int PlayerIndex
    {
        get
        {
            return playerIndex;
        }
    }

    public float DistanceToUser;
    public bool IsUserToClose = false;

    private long playerUserID = 0;
    public long PlayerUserID
    {
        get
        {
            return playerUserID;
        }
    }

    public bool IsUserTracked
    {
        get
        {
            return playerUserID != -1;
        }
    }

    private AstraManager manager;
    private List<int> trackedUserIndexes = new List<int>();
    private JointScreenOverlayer overlayer;

    //DEBUG Obecjts ot be deleted.
    private int dimesionOfBodies = 0;


    void Start()
    {
        manager = AstraManager.Instance;
        overlayer = FindObjectOfType<JointScreenOverlayer>();
    }


    // Update is called once per frame
    void Update()
    {


        if (manager != null)
        {
            if (manager && manager.isActiveAndEnabled)
            {

                List<byte> ids = new List<byte>();
                byte position = 0;
                foreach (Body body in manager.Bodies)
                {
                    ids.Add(position);
                    position++;
                }


                ids.RemoveAll(id =>
                {
                    bool remove = false;

                    if (manager.GetJointByType(manager.Bodies[id], JointType.Head) != null)
                    {
                        Vector3 spine = overlayer.GetScreenPosition(id, manager.GetJointByType(manager.Bodies[id], JointType.Head), Vector3.zero);

                        if (spine.z > Settings.Instance.TrackingMaxPosition
                            || spine.x < Settings.Instance.TrackingStartLeft * 0.01 || spine.x > 1.0f - Settings.Instance.TrackingStartRight * 0.01f || spine.z < Settings.Instance.TrackingMinPosition)
                        {
                            remove = true;

                        }

                    }
                    else
                    {
                        remove = true;

                    }
                    return remove;
                });


                if (ids.Count == 0)
                {
                    playerUserID = -1;
                }
                else
                {
                    float NearestCameraDistance = 20000;
                    foreach(byte id in ids)
                    {
                        float distance = overlayer.GetScreenPosition(id, manager.GetJointByType(manager.Bodies[id], JointType.BaseSpine), Vector3.zero).z;
                        if (distance < NearestCameraDistance && distance >= Settings.Instance.TrackingMinPosition)
                        {
                            NearestCameraDistance = distance;
                            playerUserID = id;
                        }
                      //  Debug.Log("PlayerTrackerBehaviour - Found new user: " + id + " with a distance of: " + distance + " the current min distance is: " + NearestCameraDistance + " - chosen player is: " + playerUserID);
                    }

                    playerUserID = 0;
                    DistanceToUser = overlayer.GetScreenPosition(playerUserID, manager.GetJointByType(manager.Bodies[playerUserID], JointType.BaseSpine), Vector3.zero).z;
                    IsUserToClose = DistanceToUser < Settings.Instance.TrackingMinPosition;
                }
            }
        }
        else
        {
            manager = AstraManager.Instance;
        }
    }
}
