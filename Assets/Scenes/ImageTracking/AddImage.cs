using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class AddImage : MonoBehaviour
{
    public XRReferenceImageLibrary referenceImageLibrary;

    public Texture2D imageToAdd;

    public string imageName;

    public float widthInMeters;

    JobHandle? m_JobHandle;

    public void AddImageToLibrary()
    {
        var manager = GetComponent<ARTrackedImageManager>();

        Debug.Log($"Creating library from {referenceImageLibrary.name}");
        var library = manager.CreateRuntimeLibrary(referenceImageLibrary);
        if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            Debug.Log($"Before addition, the library has {library.count} images in it.");
            m_JobHandle = mutableLibrary.ScheduleAddImageJob(imageToAdd, imageName, widthInMeters);
            manager.referenceLibrary = mutableLibrary;
            manager.enabled = true;
        }
        else if (library == null)
        {
            Debug.LogError("Image library is null.");
        }
        else
        {
            Debug.LogError($"Mutable libraries are not supported. Type of image library is {library.GetType().Name})");
        }
    }

    void Update()
    {
        if (m_JobHandle.HasValue && m_JobHandle.Value.IsCompleted)
        {
            Debug.Log($"Image \"{imageName}\" added. The reference library now has {GetComponent<ARTrackedImageManager>().referenceLibrary.count} images in it.");
            m_JobHandle = null;
        }
    }
}
