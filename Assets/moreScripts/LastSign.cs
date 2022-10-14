using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LastSign : MonoBehaviour
{
    public static void loadImage(string fileName, bool isInstruction)
    {

        string nameImageObject;
        if (isInstruction == true)
        {
            nameImageObject= "imageInstruction";
        }
        else
        {
            nameImageObject= "imageSign";
        }
        
        string fullFileName= Path.Combine("Assets/moreSigns/", fileName);
        var myImage = GameObject.Find("Canvas").transform.Find(nameImageObject).GetComponent<RawImage>();
        myImage.gameObject.SetActive(true);
        if (File.Exists(fullFileName))
        {
            byte[] imageData = File.ReadAllBytes(fullFileName);
            Texture2D tex = new Texture2D(512, 512);
            tex.LoadImage(imageData);
            myImage.texture = tex;
        }
    }
    public static void clearImage()
    {
        var myImage = GameObject.Find("Canvas").transform.Find("imageSign");
        myImage.GetComponent<RawImage>().texture = null;
        myImage.gameObject.SetActive(false);
    }
}
