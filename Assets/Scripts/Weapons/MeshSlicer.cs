using UnityEngine;
using EzySlice;

public class MeshSlicer : MonoBehaviour
{
    [Header("Slice Settings")]
    [SerializeField] private Material cutMaterial;

    public void CutObject(GameObject target, Vector3 pointOnPlane, Vector3 planeNormal)
    {
        SlicedHull hull = target.Slice(pointOnPlane, planeNormal, cutMaterial);

        if (hull != null) {
            GameObject upperHull = hull.CreateUpperHull(target, cutMaterial);
            GameObject lowerHull = hull.CreateLowerHull(target, cutMaterial);

            SetupPiece(upperHull, target);
            SetupPiece(lowerHull, target);

            Destroy(target);
        }
    }

    private void SetupPiece(GameObject piece, GameObject originalTarget)
    {
        // piece.tag = originalTarget.tag;
        // piece.layer = originalTarget.layer;

        MeshFilter mf = piece.GetComponent<MeshFilter>();

        if (mf != null && mf.sharedMesh != null) {
            if (mf.sharedMesh.vertexCount > 256)
                piece.AddComponent<BoxCollider>();
            else {
                MeshCollider mc = piece.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                
                mc.convex = true; 
            }
        } else
            piece.AddComponent<BoxCollider>();

        Rigidbody rb = piece.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.AddExplosionForce(120f, piece.transform.position, 1.5f);
    }
}
