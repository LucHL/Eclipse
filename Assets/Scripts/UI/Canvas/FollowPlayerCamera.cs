using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform nPC;
    private new Camera camera;

    void Awake()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(camera.transform);
        // transform.position = nPC.position;
    }
}
