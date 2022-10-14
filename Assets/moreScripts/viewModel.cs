using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewModel
{
    public static void Reportfailure(string failure)
    {
        Failure.Reportfailure(failure);
    }
   
    public static void loadImage(string fileName,bool isInstruction)
    {
        LastSign.loadImage(fileName, isInstruction);
    }
    public static void clearImage()
    {
        LastSign.clearImage();
    }
}
