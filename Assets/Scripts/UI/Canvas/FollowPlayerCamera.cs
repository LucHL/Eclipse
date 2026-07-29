using TMPro;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    [Header("TextMeshPro GUI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI jobText;

    private Transform cameraTransform;
    private NPCIdentity identity;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        identity = GetComponentInParent<EntityController>().identity;

        if (identity.firstName != "")
            nameText.text = $"{identity.firstName} {identity.familyName}";

        jobText.text = identity.jobs.ToString();
    }

    void LateUpdate()
    {
        if (identity.firstName != "")
            nameText.text = $"{identity.firstName} {identity.familyName}";
        else
            nameText.text = "Iconnu";

        if (cameraTransform == null)
            return;

        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
