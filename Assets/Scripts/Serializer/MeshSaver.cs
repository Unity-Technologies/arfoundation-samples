using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MeshSaver : MonoBehaviour
{
    
    public void SaveMesh()
    {
        ARFaceMeshVisualizer arFaceMeshVisualizer = FindObjectOfType<ARFaceMeshVisualizer>();
        if (!arFaceMeshVisualizer)
        {
            Debug.Log("Could not find face mesh visualizer");
            return;
        }
        SerializableFaceGeometry sfg = new SerializableFaceGeometry(arFaceMeshVisualizer.mesh);
        var jsonMesh = JsonUtility.ToJson(sfg);
        string filePath = Application.persistentDataPath + "/savedFaceMesh.json";
        File.WriteAllText (filePath, jsonMesh);
    }

}
