using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroudImageSetting : MonoBehaviour
{

    public enum Type
    {
        BACKGROUND,
        LOGO
    }

    private ImageIOUtility _imageIOUtility;
    private Image _image;
    public Type ImageType;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Start()
    {
        _imageIOUtility = new ImageIOUtility();

        string path = "";
        if (ImageType == Type.BACKGROUND)
        {
            path = Application.dataPath +  Settings.Instance.BackgroundImagePath;
        }
        else if (ImageType == Type.LOGO)
        {
            path = Application.dataPath + Settings.Instance.LogoImagePath;
        }


        if (path.Trim() != "")
        {
            string pathDebug = UnityEngine.Application.dataPath + path;
            _imageIOUtility.LoadImage(UnityEngine.Application.dataPath + path);
            if (_imageIOUtility.ImageWithPath.Texture == null)
            {
                _imageIOUtility.LoadImage(path);
                pathDebug =  path;
            }
            if (_imageIOUtility.ImageWithPath.Texture == null)
            {
                _imageIOUtility.LoadImage(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + path);
                pathDebug = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + path;
            }
            Debug.Log("Setting the backgroud image of: " + ImageType + " with the image from this path: " + pathDebug);
            _image.sprite = Sprite.Create(_imageIOUtility.ImageWithPath.Texture, new Rect(0,0, _imageIOUtility.ImageWithPath.Texture.width, _imageIOUtility.ImageWithPath.Texture.height), new Vector2(0.5f, 0.5f));
        }
    }


}
