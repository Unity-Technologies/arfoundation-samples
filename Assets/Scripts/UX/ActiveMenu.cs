using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MenuType
{
    Main,
    FaceTracking,
    PlaneDetection,
    HumanSegmentation,
    LightEstimation
}

public static class ActiveMenu
{
    public static MenuType currentMenu { get; set; }

}
