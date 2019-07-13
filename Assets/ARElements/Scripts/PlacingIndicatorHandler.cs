using UnityEngine;

public class PlacingIndicatorHandler : MonoBehaviour
{
    public Material FeatheredPlaneMaterial;
    private Vector3 posOnMat, sphereMaskSize;
    private GameObject plane;

    Bounds GetMaxBounds(GameObject g)
    {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        plane = GameObject.FindWithTag("ARPlane");
        sphereMaskSize = GetMaxBounds(gameObject).size;
        FeatheredPlaneMaterial.SetVector("_SphereMask", sphereMaskSize);
    }

    [ExecuteAlways]
    // Update is called once per frame
    void FixedUpdate()
    {
        posOnMat = new Vector3(transform.position.x - plane.transform.position.x, 0, transform.position.z - plane.transform.position.z);
        FeatheredPlaneMaterial.SetVector("_RingPos", posOnMat);

        sphereMaskSize = GetMaxBounds(gameObject).size;

        sphereMaskSize = new Vector3(0, Mathf.Max(sphereMaskSize.x, sphereMaskSize.z) * 0.5f, 0.9f);
        FeatheredPlaneMaterial.SetVector("_SphereMask", sphereMaskSize);
        Debug.Log("Bounds changed");
    }
}
