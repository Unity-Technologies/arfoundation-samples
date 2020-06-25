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
        BodyTracking,
        Meshing,
        Depth
    }

    public static class ActiveMenu
    {
        public static MenuType currentMenu { get; set; }

    }
}
