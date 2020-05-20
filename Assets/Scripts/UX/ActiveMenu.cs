using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MenuType
{
    Main,
    FaceTracking,
    PlaneDetection,
    HumanSegmentation,
}

public static class ActiveMenu
{
    public static MenuType currentMenu { get; set; }

}
