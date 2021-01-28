using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Astra;
//using Windows.Kinect;


public class JointScreenOverlayer : MonoBehaviour
{

    private AstraManager manager;
    public float Scale;


    //SHOULDERS TRACKING

    //Offset for fix the Orbbec Left Shouder X position.
    [Range(-1.0f, +1.0f)]
    public float LeftShouderOffsetX = 0.0f;

    //Offset for fix the Orbbec Rigth Shouder X position.
    [Range(-1.0f, +1.0f)]
    public float RightShouderOffsetX = 0.0f;

    //Offset for fix the Orbbec Left Shouder Y position.
    [Range(-1.0f, +1.0f)]
    public float LeftShouderOffsetY = 0.0f;

    //Offset for fix the Orbbec Rigth Shouder Y position.
    [Range(-1.0f, +1.0f)]
    public float RightShouderOffsetY = 0.0f;

    //HAND TRACKING 
    //Offset for fix the Orbbec Left Shouder X position.
    [Range(-1.0f, +1.0f)]
    public float LeftHandOffsetX = 0.0f;

    //Offset for fix the Orbbec Rigth Shouder X position.
    [Range(-1.0f, +1.0f)]
    public float RightHandOffsetX = 0.0f;

    //Offset for fix the Orbbec Left Shouder Y position.
    [Range(-1.0f, +1.0f)]
    public float LeftHandOffsetY = 0.0f;

    //Offset for fix the Orbbec Rigth Shouder Y position.
    [Range(-1.0f, +1.0f)]
    public float RightHandOffsetY = 0.0f;

    void Start()
    {
        manager = AstraManager.Instance;
    }

    public Vector3 GetScreenPosition(long userId, Astra.Joint joint, Vector3 prev)
    {

        Vector3 pos = prev;
        if (manager == null)
        {
            manager = AstraManager.Instance;
        }

        if (joint.Status == JointStatus.Tracked)
        {
            Vector3D posJoint = joint.WorldPosition;

            if (posJoint.X != 0 && posJoint.Y != 0 && posJoint.Z != 0)
            {
                Vector2D posDepth = joint.DepthPosition;
                float depthValue = joint.WorldPosition.Z;
                float xOffset = (Scale - 1) * (float)manager.DepthTexture.width / (2.0f * Scale);
                float height = (float)Screen.height * (float)manager.DepthTexture.width / Scale / (float)Screen.width;
                float yOffset = ((float)manager.DepthTexture.height - height) * 0.5f;


                if (depthValue > 0)
                {
                    float xNorm = 0;
                    if (joint.Type == Astra.JointType.LeftShoulder)
                    {
                        xNorm = Mathf.Clamp01((posDepth.X - xOffset) / ((float)manager.DepthTexture.width - 2.0f * xOffset)) + LeftShouderOffsetX;
                    }
                    else if (joint.Type == Astra.JointType.RightShoulder)
                    {
                        xNorm = Mathf.Clamp01((posDepth.X - xOffset) / ((float)manager.DepthTexture.width - 2.0f * xOffset)) + RightShouderOffsetX;
                    }else if (joint.Type == Astra.JointType.LeftHand)
                    {
                        xNorm = Mathf.Clamp01((posDepth.X - xOffset) / ((float)manager.DepthTexture.width - 2.0f * xOffset)) + LeftHandOffsetX;
                    }
                    else if (joint.Type == Astra.JointType.RightHand)
                    {
                        xNorm = Mathf.Clamp01((posDepth.X - xOffset) / ((float)manager.DepthTexture.width - 2.0f * xOffset)) + RightHandOffsetX;
                    }
                    else
                    {
                        xNorm = Mathf.Clamp01((posDepth.X - xOffset) / ((float)manager.DepthTexture.width - 2.0f * xOffset));
                    }

                    float yNorm = 0;

                    if (joint.Type == Astra.JointType.LeftShoulder)
                    {
                        yNorm = Mathf.Clamp01(1.0f - (posDepth.Y - yOffset) / height) + LeftShouderOffsetY;
                    }
                    else if (joint.Type == Astra.JointType.RightShoulder)
                    {
                        yNorm = Mathf.Clamp01(1.0f - (posDepth.Y - yOffset) / height) + RightShouderOffsetY;
                    } else if (joint.Type == Astra.JointType.LeftHand)
                    {
                        yNorm = Mathf.Clamp01(1.0f - (posDepth.Y - yOffset) / height) + LeftHandOffsetY;
                    }
                    else if (joint.Type == Astra.JointType.RightHand)
                    {
                        yNorm = Mathf.Clamp01(1.0f - (posDepth.Y - yOffset) / height) + RightHandOffsetY;
                    }
                    else
                    {
                        yNorm = Mathf.Clamp01(1.0f - (posDepth.Y - yOffset) / height);
                    }

                    pos = new Vector3(xNorm, yNorm, depthValue);

                }
            }
        }
        return pos;
    }

}
