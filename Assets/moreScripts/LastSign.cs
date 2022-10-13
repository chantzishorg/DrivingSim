using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LastSign : MonoBehaviour
{
    // Start is called before the first frame update
    public static void loadImage(string fileName)
    {
        string fullFileName= Path.Combine("Assets/moreSigns/", fileName);
        Debug.Log(fullFileName);
        var myImage = GameObject.Find("Canvas").transform.Find("imageSign").GetComponent<RawImage>();
        Debug.Log(myImage.texture);
        if (File.Exists(fullFileName))
        {
            byte[] imageData = File.ReadAllBytes(fullFileName);
            Debug.Log($"{imageData.Length}");
            Texture2D tex = new Texture2D(512, 512);
            tex.LoadImage(imageData);
            myImage.texture = tex;
        }
    }
}
