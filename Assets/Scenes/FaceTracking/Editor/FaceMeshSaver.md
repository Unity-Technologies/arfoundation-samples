FaceMeshSaver sample usage

The FaceMeshSaver sample consists of two steps: 
1. An ARFoundation Sample app that can be built to any device that supports Face Tracking and will save the face mesh to a json file on the device.
2. A Unity Editor tool that can read the json file from disk on your PC or Mac and write the corresponding mesh out to a Mesh asset that you had selected.

There will need to be a transfer of data from device to PC/Mac between steps 1 and 2.  Once you have saved a Mesh asset in Unity, you can then proceed to use it for anything you would use a Mesh in Unity for.  In our case, we can use it as a model to keep track of landmarks on the face.

Usage:

Build the FaceMeshSaver scene file as an app to a platform that supports Face Tracking under ARFoundation (at time of writing ARKit/iOS and ARCore/Android).  Install the application on a device that supports Face Tracking and run it there.  When your face appears on the device, press the button to save it to a json file in the persistent storage of the device.

Transfer the json file from the device to your personal computer (PC or Mac).  Open this same project on that machine.  Go to the "SavedAsset" folder and click on the mesh asset there so that it is selected.  Now go to the top menu and click on the entry "Window/JSONMeshLoader/WriteJSONToMesh", then in the file chooser panel, select the json file that you've transferred above.