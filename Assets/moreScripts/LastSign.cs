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
        
        var myImage = GameObject.Find("Canvas").transform.Find(nameImageObject).GetComponent<RawImage>();
        myImage.gameObject.SetActive(true);
        if (File.Exists(fileName))
        {
            //System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            //MessageBox.Show("Width: " + img.Width + ", Height: " + img.Height);
            byte[] imageData = File.ReadAllBytes(fileName);
            Texture2D tex = new Texture2D(512, 512);
            tex.LoadImage(imageData);
            myImage.texture = tex;
        }
    }
    public static void clearImage(bool isInstruction)
    {
        string nameImageObject;
        if (isInstruction == true)
        {
            nameImageObject = "imageInstruction";
        }
        else
        {
            nameImageObject = "imageSign";
        }
        var myImage = GameObject.Find("Canvas").transform.Find(nameImageObject);
        myImage.GetComponent<RawImage>().texture = null;
        myImage.gameObject.SetActive(false);
    }
}
