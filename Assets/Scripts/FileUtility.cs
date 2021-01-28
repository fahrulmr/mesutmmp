using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileUtility
{
    public static T Load<T>(string fileName) where T : class, new()
    {
        T file;
        var path = Application.dataPath;
        path = System.IO.Path.Combine(path, fileName);
        //	Debug.Log("****Settings Path: "+path);
        if (System.IO.File.Exists(path))
        {
            var json = System.IO.File.ReadAllText(path);
            file = JsonUtility.FromJson<T>(json);
        }
        else
        {
            file = new T();
        }
        return file;
    }

    public static void Save(string fileName, object file)
    {
        var json = JsonUtility.ToJson(file);
        var path = Application.dataPath;
        path = System.IO.Path.Combine(path, fileName);
        System.IO.File.WriteAllText(path, json);
    }

}
