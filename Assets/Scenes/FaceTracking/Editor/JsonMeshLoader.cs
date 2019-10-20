using System.IO;
using UnityEditor;
using UnityEngine;

public class JsonMeshLoader : EditorWindow
{
    //How to use this 
    [MenuItem("Window/JSONMeshLoader/WriteJSONToMesh")]
    static void Apply()
    {
        Mesh mesh = Selection.activeObject as Mesh;

        if (mesh == null)
        {
            EditorUtility.DisplayDialog("Select Mesh", "You must select a mesh first!", "OK");
            return;
        }

        string path = EditorUtility.OpenFilePanel("Convert json mesh", "", "json");
        if (path.Length != 0)
        { 
             var jsonMesh = File.ReadAllText(path);

             SerializableFaceGeometry serializableFaceGeometry = JsonUtility.FromJson<SerializableFaceGeometry>(jsonMesh);
             mesh.vertices = serializableFaceGeometry.vertices;
             mesh.uv = serializableFaceGeometry.texCoords;
             mesh.triangles = serializableFaceGeometry.triIndices;
             EditorUtility.SetDirty(mesh);
             
        }
    }
}
