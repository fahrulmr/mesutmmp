using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class S3Manager : MonoBehaviour
{
    private static S3Manager instance = null;
    public GameObject ErrorSettings;
    public GameObject ErrorEditorSettings;
    public GameObject UploadInfo;
    public Text UploadInfotext;

    public GameObject DownloadInfo;
    public Text DownloadInfotext;

    // Game Instance Singleton
    public static S3Manager Instance
    {
        get
        {
            return instance;
        }
    }


    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;
    public string IdentityPoolId = "us-east-2:e9a36d43-5858-43e8-9605-43720865febd";
    public string CognitoIdentityRegion = RegionEndpoint.USEast2.SystemName;
    public string S3BucketName = "magicmirrorunity";

    public int totalRequested = 0;
    public int totalUpdloaded = 0;
    public bool isUploading = false;

    public int totalDWRequested = 0;
    public int totalDownloaded = 0;
    public bool isDownloading = false;

    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }
    public string S3Region = RegionEndpoint.USEast2.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }


    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    }

    private void Update()
    {
        if (isUploading)
        {
            UploadInfo.SetActive(true);
            if (totalRequested - 2 == totalUpdloaded)
            {
                isUploading = false;
                totalUpdloaded = 0;
                totalRequested = 0;
                UploadInfo.SetActive(false);
            }
            UploadInfotext.text = "UPLOADING STATUS \n Total to be uploaded: " + totalRequested + "\n Total uploaded: " + totalUpdloaded + " \n Status: " + (((float)totalUpdloaded / (float)totalRequested) * 100.0f).ToString("n2") + "%";
        }

        if (isDownloading)
        {
            DownloadInfo.SetActive(true);
            if (totalDWRequested == totalDownloaded && totalDWRequested != 0)
            {
                isDownloading = false;
                totalDownloaded = 0;
                totalDWRequested = 0;
                DownloadInfo.SetActive(false);
            }
            DownloadInfotext.text = "DOWNLOADING STATUS \n Total to be Downloaded: " + totalDWRequested + "\n Total downloaded: " + totalDownloaded + " \n Status: " + (((float)totalDownloaded / (float)totalDWRequested) * 100.0f).ToString("n2") + "%";
        }

    }


    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, _S3Region);
            }
            //test comment
            return _s3Client;
        }
    }


    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveAllToS3()
    {
        totalRequested = EditorSettings.EditorInstance.AddPanel.Count + 4;

        foreach (ClothesInfo panel in EditorSettings.EditorInstance.AddPanel)
        {
            Debug.Log("Saving " + panel.SpritePath + " On S3");
            PostImage(Application.dataPath + panel.SpritePath);
        }
        Debug.Log("Saving settings.json On S3");
        PostJSON(Application.dataPath + "\\Settings.json");
        Debug.Log("Saving settingsEditor.json On S3");
        PostJSON(Application.dataPath + "\\SettingsEditor.json");
        Debug.Log("Saving background On S3");
        PostImage(Application.dataPath + Settings.Instance.BackgroundImagePath);
        Debug.Log("Saving logo On S3");
        PostImage(Application.dataPath + Settings.Instance.LogoImagePath);
        isUploading = true;
    }

    public void DownloadAllFromS3()
    {
        string key = PlayerPrefs.GetString("RequestCode");
        GetObject(key + "/Settings.json", Application.dataPath + "\\Settings.json");

        GetObject(key + "/SettingsEditor.json", Application.dataPath + "\\SettingsEditor.json");


        isDownloading = true;
    }

    private void DownloadAllPictures()
    {
        string key = PlayerPrefs.GetString("RequestCode");
        foreach (ClothesInfo clothes in EditorSettings.EditorInstance.AddPanel)
        {
            GetObject(key + "/images/" + Path.GetFileName(clothes.SpritePath), Application.dataPath + clothes.SpritePath);
        }
    }

    private void GetObject(string fileKey, string path)
    {

        Client.GetObjectAsync(S3BucketName, fileKey, (responseObj) =>
        {
            Debug.Log("Downloading: " + fileKey);
            var response = responseObj.Response;
            if (response.ResponseStream != null)
            {
                using (var fs = System.IO.File.Create(path))
                {
                    byte[] buffer = new byte[81920];
                    int count;
                    while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
                        fs.Write(buffer, 0, count);
                    fs.Flush();
                    Debug.Log("Downloaded: " + fileKey);

                }

                if (fileKey.Contains("SettingsEditor.json"))
                {
                    EditorSettings.EditorInstance.Reload();
                    totalDWRequested = EditorSettings.EditorInstance.AddPanel.Count+2;
                    if (!Directory.Exists(Application.dataPath + "\\Images"))
                    {
                        Directory.CreateDirectory(Application.dataPath + "\\Images");
                    }
                    DownloadAllPictures();
                }
                else if (fileKey.Contains("Settings.json"))
                {
                    Settings.Instance.Reload();
                    string key = PlayerPrefs.GetString("RequestCode");
                    Debug.LogWarning("After downloaded: "+ Path.GetFileName(Settings.Instance.BackgroundImagePath) + " as background path and "+ Path.GetFileName(Settings.Instance.LogoImagePath) + "As logo path" );
                    GetObject(key + "/images/" + Path.GetFileName(Settings.Instance.BackgroundImagePath), Application.dataPath + Settings.Instance.BackgroundImagePath);
                    GetObject(key + "/images/" + Path.GetFileName(Settings.Instance.LogoImagePath), Application.dataPath + Settings.Instance.LogoImagePath);
                }
                else
                {
                    totalDownloaded++;
                }
            }

            if (responseObj.Exception != null)
            {
                Debug.LogError(responseObj.Exception);
                if (fileKey.Contains("SettingsEditor.json"))
                {
                    ErrorEditorSettings.SetActive(true);
                }
                else if (fileKey.Contains("Settings.json"))
                {
                    ErrorSettings.SetActive(true);
                }
                else
                {
                    totalDownloaded++;
                }
            }

        });

    }


    public void PostImage(string path)
    {
        string key = PlayerPrefs.GetString("RequestCode");

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        var request = new PostObjectRequest()
        {
            Bucket = S3BucketName,
            Key = key + "/images/" + Path.GetFileName(path),
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };


        Debug.Log(string.Format(" Uploading:  {0} ", request.Key));

        Client.PostObjectAsync(request, (responseObj) =>
        {
            Debug.Log(string.Format("Uploaded:  {0} status: {1}", request.Key, responseObj.Exception));
            totalUpdloaded++;
            if (responseObj.Exception == null)
            {
                Debug.Log(string.Format("\n object {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
            }
            else
            {
                Debug.Log(string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString()));
                Debug.LogError(responseObj.Exception);
            }
        });
    }


    public void PostJSON(string path)
    {
        string key = PlayerPrefs.GetString("RequestCode");

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        var request = new PostObjectRequest()
        {
            Bucket = S3BucketName,
            Key = key + "/" + Path.GetFileName(path),
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        Debug.Log(string.Format(" Uploading:  {0} ", request.Key));

        Client.PostObjectAsync(request, (responseObj) =>
        {
            Debug.Log(string.Format("Uploaded:  {0} ", request.Key));

            if (responseObj.Exception == null)
            {
                Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
            }
            else
            {
                Debug.Log(string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString()));
                Debug.LogError(responseObj.Exception);
            }
        });
    }



}
