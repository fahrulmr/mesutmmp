using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class webcam : MonoBehaviour
{
    
    void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture.deviceName = devices[0].name;
        Debug.Log(webcamTexture.deviceName);
        this.GetComponent<MeshRenderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();
        
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // Start is called before the first frame update
   /* void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        Debug.Log(webcamTexture.deviceName);

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
*/
    
}
