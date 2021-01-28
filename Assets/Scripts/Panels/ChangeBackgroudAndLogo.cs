using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeBackgroudAndLogo : MonoBehaviour
{


    public GameObject PanelContainer;

    public InputField BackgroundPath;
    public InputField LogoPath;

    private string _backgroudPath;
    private string _logoPath;

    ImageIOUtility imageIOUtility;

    ImageWithPath background ;
    ImageWithPath icon ;


    void Awake()
    {
        imageIOUtility = new ImageIOUtility();
    }


    public void OpenEdit()
    {
        if (!PanelContainer.activeSelf)
        {
            BackgroundPath.text = Settings.Instance.BackgroundImagePath;
            LogoPath.text = Settings.Instance.LogoImagePath;
            PanelContainer.SetActive(true);
        } else
        {
            PanelContainer.SetActive(false);
        }
    }

    public void CloseEdit()
    {
        Debug.Log("Closing edit.");
        PanelContainer.SetActive(false);
    }

    public void SaveAndClose()
    {
        Debug.Log("Saving and closing edit.");
        Settings.Instance.BackgroundImagePath = BackgroundPath.text;
        Settings.Instance.LogoImagePath = LogoPath.text;
        Settings.Instance.Save();

        if (icon.Texture != null)
            imageIOUtility.SaveTexture(icon);

        if (background.Texture != null)
            imageIOUtility.SaveTexture(background);

        PanelContainer.SetActive(false);
    }

    public void FindBackgroundImage()
    {
        Debug.Log("Opening FileSystem for loading the new Logo.");
        imageIOUtility.LoadImageFromFileSystem();
        if (imageIOUtility.IsValid())
        {
            _backgroudPath = imageIOUtility.ImageWithPath.Path;
            BackgroundPath.text = _backgroudPath;
            background = imageIOUtility.ImageWithPath;
        }
    }

    public void FindLogoImage()
    {
        Debug.Log("Opening FileSystem for loading the new Logo.");
        imageIOUtility.LoadImageFromFileSystem();
        if (imageIOUtility.IsValid())
        {
            _logoPath = imageIOUtility.ImageWithPath.Path;
            LogoPath.text = _logoPath;
            icon = imageIOUtility.ImageWithPath;
        }
    }


   
}
