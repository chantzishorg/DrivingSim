using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

// class that caluclate the dimentions of the image
public class PNGImage
{
    // Tuple<uint, uint>
    public static (int, int) getPngDims(byte[] image)
    {
        /*
         * 0x89 0x50 0x4e 0x47 0x0d 0x0a 0x1a 0x0a - constant 8 byte png header
         * chunk length - 4 bytes in big endian order
         * chunk name - 4 bytes - first chunk must be "IHDR" (0x49 0x48 0x44 0x52)
         * chunk data - 'length' bytes - IHDR chunk contains:
         * - width (4 bytes)
         * - height (4 bytes)
         * - ...
         * CRC - 4 bytes
         * ... (more chunks)
         */
        if (image[0] == 0x89 && image[1] == 0x50 && image[2] == 0x4e && image[3] == 0x47 &&
            image[4] == 0x0d && image[5] == 0x0a && image[6] == 0x1a && image[7] == 0x0a &&
            image[12] == 0x49 && image[13] == 0x48 && image[14] == 0x44 && image[15] == 0x52)
        {
            //uint width = (uint)image[16] * (uint)16777216 + (uint)image[17] * (uint)65536 + (uint)image[18] * (uint)256 + (uint)image[19];
            //uint height = (uint)image[20] * (uint)16777216 + (uint)image[21] * (uint)65536 + (uint)image[22] * (uint)256 + (uint)image[23];

            // apparently PNG uses only 31 bit ints allowing use of singed ints (https://stackoverflow.com/a/4109505/5222357 - first comment):
            int width = (int)image[16] * (int)16777216 + (int)image[17] * (int)65536 + (int)image[18] * (int)256 + (int)image[19];
            int height = (int)image[20] * (int)16777216 + (int)image[21] * (int)65536 + (int)image[22] * (int)256 + (int)image[23];
            // Tuple.Create<uint, uint>(width, height)
            return (width, height);
        }
        Debug.LogError("Invalid PNG file");
        return (0, 0);
    }
}

// class that represents the last sign
public class LastSign : MonoBehaviour
{
    public static void loadImage(string fileName, bool isInstruction)
    {
        if (isInstruction)
        {
            loadInstructionImage(fileName);
        }
        else
        {
            loadSignImage(fileName);
        }
    }
    public static void loadInstructionImage(string fileName)
    {
        var myImage = GameObject.Find("Canvas").transform.Find("imageInstruction").GetComponent<RectTransform>();
        myImage.gameObject.SetActive(true);
        switch (fileName.ToLower())
        {
            case "right":
                myImage.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
            case "left":
                myImage.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
        }
    }
    public static void loadSignImage(string fileName)
    {
        var myImage = GameObject.Find("Canvas").transform.Find("imageSign").GetComponent<RawImage>();
        if (File.Exists(fileName))
        {
            clearImage(false);
            myImage.gameObject.SetActive(true);
            byte[] imageData = File.ReadAllBytes(fileName);
            (int width, int height) dims = PNGImage.getPngDims(imageData);
            //Debug.Log(dims);
            Texture2D tex = new Texture2D(dims.width, dims.height);
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
        if (!isInstruction) myImage.GetComponent<RawImage>().texture = null;
        myImage.gameObject.SetActive(false);
    }
}
