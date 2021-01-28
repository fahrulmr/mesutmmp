using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif

public struct ImageWithPath
{
    public Texture2D Texture;
    public string Path;

    public ImageWithPath(Texture2D texture, string path)
    {
        Texture = texture;
        Path = path;
    }
}

public class ImageIOUtility
{
    private ImageWithPath imageWithPath;

    public ImageWithPath ImageWithPath
    {
        get
        {
            return imageWithPath;
        }
    }

    public ImageIOUtility()
    {
        imageWithPath = new ImageWithPath(null, null);
    }

    public void LoadImageFromFileSystem()
    {
#if UNITY_STANDALONE_WIN
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.InitialDirectory = UnityEngine.Application.dataPath;
		

		ofd.Filter = "png files (*.png)|*.png";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			LoadImage(ofd.FileName);
			imageWithPath.Path = "/Images/" + ofd.SafeFileName;
		}
#endif

    }


    public bool IsValid()
    {
        return imageWithPath.Texture != null;
    }

    public void LoadImage(string filePath)
    {
        byte[] fileData;
        Debug.Log("trying to load: " + UnityEngine.Application.dataPath);
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            imageWithPath.Texture = new Texture2D(2, 2);
            imageWithPath.Texture.LoadImage(fileData);
        }
    }

    public void SaveTexture(ImageWithPath imageWithPath)
    {
        if (imageWithPath.Texture != null)
        {
            var bytes = imageWithPath.Texture.EncodeToPNG();
            File.WriteAllBytes(UnityEngine.Application.dataPath + imageWithPath.Path, bytes);
        }
    }
}
