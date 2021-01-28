using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartTrackerBehaviour : MonoBehaviour
{
	public Astra.JointType JointType;
	public int playerIndex = 0;
	public int SmoothFactor = 5;
	public float UIScaler = 1500f;
	public float UIOffsetScaler = 1500f;
	public float CoordinateScaler = 1.2f;
	public float ScaleFactor = 2.2f;
	public float ScaleGap = 0.04f;
	public Vector2 ClothesOfset = Vector2.zero;
	public Vector2 Pivot;
	[Range(0.0f, 0.4f)]
	public float ShoulderScaler = 0.0f;

	public List<Vector2> lastPositions = new List<Vector2>();
	public JointScreenOverlayer overlayer;
	public Vector2 initPosition;
	public PlayerTrackerBehaviour playerTracker;
	public Vector2 SkeletonShift = Vector2.zero;
	public Vector2 ClothesShift = Vector2.zero;

	//	private Vector2 clothesPosition = new Vector2(0.0f, 0.0f);
	public float heightOffset;

	void Start()
	{
		initPosition = transform.position;
		overlayer = FindObjectOfType<JointScreenOverlayer>();
		playerTracker = FindObjectOfType<PlayerTrackerBehaviour>();
		
	}
	
	// Update is called once per frame
	void Update()
	{
		AstraManager manager = AstraManager.Instance;
		if (playerTracker != null && !playerTracker.IsUserTracked)
		{
			
//			transform.position = Vector3.Lerp(transform.position, initPosition, 1.5f * Time.deltaTime);
//			transform.localScale = Vector3.one / 1500 * UIScaler;
		}
		else
		{
			// update Kinect interaction
			if (manager && manager.isActiveAndEnabled)
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
							Vector3 shoulderLeft = overlayer.GetScreenPosition(playerUserID, manager.GetJointByType(manager.Bodies[playerUserID], Astra.JointType.LeftShoulder), transform.position);
							Vector3 shoulderRight = overlayer.GetScreenPosition(playerUserID, manager.GetJointByType(manager.Bodies[playerUserID], Astra.JointType.RightShoulder), transform.position);
							float distanceBetweenShoulders;
								distanceBetweenShoulders = Vector2.Distance(shoulderLeft, shoulderRight) * Settings.Instance.ViewScale;
							CoordinateScaler = Settings.Instance.ViewScale * ScaleFactor;
							head2D = (head2D - center) * CoordinateScaler + center;

							SkeletonShift = Settings.Instance.SkeletonShift;
							ClothesShift = Settings.Instance.ClothesOffset;

							GetComponent<RectTransform>().pivot = Pivot;
							Vector2 jointPos = new Vector2(head2D.x * Screen.width, head2D.y * Screen.height);
							Vector2 newPos = jointPos + ClothesOfset + SkeletonShift + ClothesShift + new Vector2(Settings.Instance.ViewPositionX, Settings.Instance.ViewPositionY);

							transform.position = GetSmoothedPosition(newPos);

							if (shoulderRight.z > 25.0f && shoulderLeft.z > 25.0f)
							{
								if (shoulderLeft.z - shoulderRight.z < 250.0f && shoulderRight.z - shoulderLeft.z < 250.0f)
								{		
									float newScale = distanceBetweenShoulders / ShoulderScaler * UIScaler;
									float scale = transform.localScale.x;
									float diff = newScale - scale;
									if (Mathf.Abs(diff) >= ScaleGap)
									{
										scale = newScale - Mathf.Sign(diff) * ScaleGap;
									}
									transform.localScale = Vector3.one * scale;
								}
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

	private float HeightOffset(float x)
	{
		return (x - 840.0f) * (-40.0f - 60.0f) / (1425.0f - 840.0f) + 60.0f;
	}

	Vector2 GetSmoothedPosition(Vector2 pos)
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
