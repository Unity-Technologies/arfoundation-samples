using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public enum MenuType
    {
        Main,
        FaceTracking,
        PlaneDetection,
        HumanSegmentation,
        Meshing
    }

    public static class ActiveMenu
    {
        public static MenuType currentMenu { get; set; }

    }
}
