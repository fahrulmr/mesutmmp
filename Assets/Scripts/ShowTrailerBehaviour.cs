using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class ShowTrailerBehaviour : MonoBehaviour {
    private VideoPlayer videoPlayer;
    private RawImage ShowVideoImage;
    private PlayerTrackerBehaviour playerTracker;
    private bool firstPlay = true;

    void Awake () {
        videoPlayer = transform.GetComponent<VideoPlayer>();
        ShowVideoImage = transform.GetComponent<RawImage>();
        playerTracker = FindObjectOfType<PlayerTrackerBehaviour>();
        videoPlayer.url = "file:///" + Application.dataPath + "/IntroVideo.mp4";
    }

    void Update () {
        if (playerTracker != null && playerTracker.IsUserTracked && Settings.Instance.ShowTrailer)
        {
            if (!videoPlayer.isPlaying )
            {
                if (firstPlay)
                {
                    ShowVideoImage.enabled = true;
                    videoPlayer.Play();
                    firstPlay = false;
                }else
                {
                    videoPlayer.Stop();
                    ShowVideoImage.enabled = false;
                }
            }
            ShowVideoImage.texture = videoPlayer.texture;
        }else if (videoPlayer.isPlaying || !firstPlay)
            {
                videoPlayer.Stop();
                ShowVideoImage.enabled = false;
                firstPlay = true;
            }
    }
}
