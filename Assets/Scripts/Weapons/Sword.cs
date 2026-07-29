using UnityEngine;

public class Sword : AWeapon
{
    private MeshSlicer slicer;

    void Awake()
    {
        slicer = FindFirstObjectByType<MeshSlicer>();
    }

    public void Attack()
    {
        return;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cuttable")) {
            if (slicer != null) {
                EntityController entityController = other.GetComponentInParent<EntityController>();
                if (entityController != null)
                    entityController.Kill();

                slicer.CutObject(other.gameObject, transform.position, transform.up);
            }
        }
    }
}
