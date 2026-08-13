namespace UnityEngine.XR.ARFoundation.Samples
{
    public class HMDMenuController : MenuController
    {
        protected override void BuildMenuLayout()
        {
            foreach (var category in categoryOrder)
            {
                if (!scenesByCategory.TryGetValue(category, out var entries))
                    continue;

                foreach (var entry in entries)
                    CreateSceneButton(category, entry, contentParent);
            }
        }
    }
}
