using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    // in the following order  "TrafficLightRL", "TrafficLightLR", "TrafficLightRR", "TrafficLightLL"
    public GameObject firstLightPre;
    public GameObject firstLightPass;
    public GameObject secondLightPre;
    public GameObject secondLightPass;
    public GameObject thirdLightPre;
    public GameObject thirdLightPass;
    public GameObject fourthLightPre;
    public GameObject fourthLightPass;

    private int activeIndex = 0;
    private bool active = false;
    //private static Light nullLight = new Light();
    private List<(Light green, Light red, Light yellow, MeshRenderer MR)> trafficLights = new List<(Light green, Light red, Light yellow, MeshRenderer MR)>();
    //private int currentRoad = 0;

    private (Vector2 middle, float width, Vector2 vec) GetParameters(GameObject line)
    {
        if (line == null) {
            return (new Vector2(float.NaN, float.NaN), float.NaN, new Vector2(float.NaN, float.NaN));
        }
        float middle_x = line.transform.position[0];
        float middle_z = line.transform.position[2];
        float direction_x = line.transform.GetChild(0).position[0];
        float direction_z = line.transform.GetChild(0).position[2];
        float vector_x = direction_x - middle_x;
        float vector_z = direction_z - middle_z;
        return (new Vector2(middle_x, middle_z), line.transform.localScale[2], new Vector2(vector_x, vector_z));
    }
    // Start is called before the first frame update
    void Start()
    {
        string[] trafficLightsNames = { "TrafficLightRL", "TrafficLightLR", "TrafficLightRR", "TrafficLightLL" };
        foreach (var trafficLightName in trafficLightsNames){
            var temp = transform.Find(trafficLightName)?.Find("Light0")?.Find("Lights");
            var templights = temp?.GetComponentsInChildren<Light>();
            //Debug.Log($"{templights.Length} {templights[0].enabled},{templights[1].enabled}, {templights[2].enabled}");
            if (temp != null)
                trafficLights.Add((templights[0], templights[1], templights[2], temp.GetComponent<MeshRenderer>()));
            else trafficLights.Add((null, null, null, null));
        }

        App.AddTrafficLights(
        (GetParameters(firstLightPre).middle, GetParameters(firstLightPre).width, GetParameters(firstLightPre).vec,
         GetParameters(firstLightPass).middle, GetParameters(firstLightPass).width, GetParameters(firstLightPass).vec),
        (GetParameters(secondLightPre).middle, GetParameters(secondLightPre).width, GetParameters(secondLightPre).vec,
         GetParameters(secondLightPass).middle, GetParameters(secondLightPass).width, GetParameters(secondLightPass).vec),
        (GetParameters(thirdLightPre).middle, GetParameters(thirdLightPre).width, GetParameters(thirdLightPre).vec,
         GetParameters(thirdLightPass).middle, GetParameters(thirdLightPass).width, GetParameters(thirdLightPass).vec),
        (GetParameters(fourthLightPre).middle, GetParameters(fourthLightPre).width, GetParameters(fourthLightPre).vec,
         GetParameters(fourthLightPass).middle, GetParameters(fourthLightPass).width, GetParameters(fourthLightPass).vec),
        onLightChange,
        setActive
        );
    //Invoke("flipRoadGreen", 2f);
}
    /*
    void flipRoadGreen()
    {
        //Debug.Log("flipRoadGreen");
        //var temp = trafficLights[greenRoad];
        //Debug.Log($"{temp.green.enabled},{temp.yellow.enabled}, {temp.red.enabled}");
        setGreen(currentRoad);
        setGreen(currentRoad + 1);
        currentRoad = (currentRoad + 2) % 4;
        setRed(currentRoad);
        setRed(currentRoad + 1);
        Invoke("flipRoadYellow", 10f);
    }
    void flipRoadYellow()
    {
        //Debug.Log("flipRoadYellow");
        currentRoad = (currentRoad + 2) % 4;
        setYellow(currentRoad);
        setYellow(currentRoad + 1);
        Invoke("flipRoadRed", 1f);
    }
    void flipRoadRed()
    {
        setRed(currentRoad);
        setRed(currentRoad + 1);
        currentRoad = (currentRoad + 2) % 4;
        Invoke("flipRoadGreen", 1f);
    }*/

    public void setGreen(int greenRoad)
    {
        if (trafficLights[greenRoad].green != null)
        {
            trafficLights[greenRoad].green.enabled = true;
            trafficLights[greenRoad].red.enabled = false;
            trafficLights[greenRoad].yellow.enabled = false;
            trafficLights[greenRoad].MR.material.mainTextureOffset = new Vector2(0.667f, 0f);
        }
        if (active && greenRoad == activeIndex)
        {
            viewModel.loadImage("Assets/moreSigns/green.png", false);
        }
    }
    public void setRed(int redRoad)
    {
        if (trafficLights[redRoad].green != null)
        {
            trafficLights[redRoad].green.enabled = false;
            trafficLights[redRoad].red.enabled = true;
            trafficLights[redRoad].yellow.enabled = false;
            trafficLights[redRoad].MR.material.mainTextureOffset = new Vector2(0f, 0f);
        }
        if (active && redRoad == activeIndex)
        {
            viewModel.loadImage("Assets/moreSigns/red.png", false);
        }
    }
    public void setYellow(int yellowRoad)
    {
        if (trafficLights[yellowRoad].green != null)
        {
            trafficLights[yellowRoad].green.enabled = false;
            trafficLights[yellowRoad].red.enabled = false;
            trafficLights[yellowRoad].yellow.enabled = true;
            trafficLights[yellowRoad].MR.material.mainTextureOffset = new Vector2(0.334f, 0f);
        }
        if (active && yellowRoad == activeIndex)
        {
            viewModel.loadImage("Assets/moreSigns/yellow.png", false);
        }
    }

    public void onLightChange(int index, LightColor color)
    {
        switch (color)
        {
            case LightColor.Green:
                setGreen(index);
                break;
            case LightColor.Yellow:
                setYellow(index);
                break;
            case LightColor.Red:
                setRed(index);
                break;
            default:
                // not reachable
                break;
        }
    }

    public void setActive(bool active, int index = 0)
    {
        if (active)
        {
            activeIndex = index;
            this.active = active;
            viewModel.loadImage(GetLight(index), false);
        }
        else
        {
            this.active = active;
            viewModel.clearImage(false);
        }
    }
    private string GetLight(int idx)
    {
        if (trafficLights[idx].MR.material.mainTextureOffset.x == 0.667f /*.green.enabled*/)
        {
            return "Assets/moreSigns/green.png";
        }
        if (trafficLights[idx].MR.material.mainTextureOffset.x == 0f /*.red.enabled*/)
        {
            return "Assets/moreSigns/red.png";
        }
        if (trafficLights[idx].MR.material.mainTextureOffset.x == 0.334f /*.yellow.enabled*/)
        {
            return "Assets/moreSigns/yellow.png";
        }
        return "red";
    }
}
