using UnityEngine;
using TMPro;

public class EntityInspectorUI : MonoBehaviour
{
    public static EntityInspectorUI instance;

    [Header("UI References")]
    [SerializeField] private GameObject windowPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text genderText;
    [SerializeField] private TMP_Text jobText;
    [SerializeField] private TMP_Text ageText;
    [SerializeField] private TMP_Text religionText;
    [SerializeField] private TMP_Text homeText;
    [SerializeField] private TMP_Text currentHomeText;
    [SerializeField] private TMP_Text raceText;
    [SerializeField] private TMP_Text factionText;

    [Header("3D Preview Studio")]
    [SerializeField] private Transform previewSpawnPoint;
    [SerializeField] private float rotationSpeed = 25f;

    [Tooltip("Use this to disable player movement when the Inspector is open")]
    public bool isOpen = false;

    private GameObject currentPreviewInstance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (windowPanel != null)
            windowPanel.SetActive(false);
    }

    private void Update()
    {
        if (currentPreviewInstance != null)
            currentPreviewInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    public void InspectNPC(NPCIdentity identity, GameObject originalNPC)
    {
        isOpen = true;
        SetAllText(identity);

        if (currentPreviewInstance != null)
            Destroy(currentPreviewInstance);

        // if (originalNPC != null && previewSpawnPoint != null) {
        //     currentPreviewInstance = Instantiate(originalNPC, previewSpawnPoint.position, previewSpawnPoint.rotation);
        //     currentPreviewInstance.transform.SetParent(previewSpawnPoint);
        //     currentPreviewInstance.transform.localScale = Vector3.one;

        //     foreach (var col in currentPreviewInstance.GetComponentsInChildren<Collider>())
        //         col.enabled = false;

        //     foreach (var mono in currentPreviewInstance.GetComponentsInChildren<MonoBehaviour>())
        //         mono.enabled = false;
        // }

        windowPanel.SetActive(true);
    }

    public void CloseWindow()
    {
        if (currentPreviewInstance != null)
            Destroy(currentPreviewInstance);

        if (windowPanel != null)
            windowPanel.SetActive(false);

        isOpen = false;
    }

    private void SetAllText(NPCIdentity identity)
    {
        if (nameText != null)
            nameText.text = $"{identity.firstName} {identity.familyName}";

        if (genderText != null)
            genderText.text = $"Gender: {identity.gender.ToString()}";

        if (jobText != null)
            jobText.text = $"Job: {identity.jobs.ToString()}";

        if (ageText != null)
            ageText.text = $"Age: {identity.age.ToString()}";

        if (religionText != null)
            religionText.text = $"Religion: {identity.religion}";

        if (homeText != null)
            homeText.text = $"Born in: {identity.homeVillage}";

        if (currentHomeText != null)
            currentHomeText.text = $"Living in: {identity.currentVillage}";

        if (raceText != null)
            raceText.text = $"Race: {identity.race.ToString()}";

        if (factionText != null)
            factionText.text = $"Faction: {identity.faction.ToString()}";
    }
}
