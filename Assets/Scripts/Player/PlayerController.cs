using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("Player Info")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float mouseSensitivity = 1f;

    [Header("Weapon")]
    [SerializeField] private AWeapon currentWeapon; 

    [Header("Raycast / NPC Inspection")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float interactRange = 5f;
    private bool isBlocking = false;
    private bool isInspecting = false;

    [Tooltip("New Input System")]
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private InputManager inputManager;

    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position;
    }

    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
    }

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.instance;
        WorldManager.instance.HideCursor();
    }

    #region New Input system Action

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && currentWeapon != null)
            currentWeapon.Attack();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started) {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, LayerMask.GetMask("Entity"))) {
                NPCControllers npcController = hit.collider.GetComponentInParent<NPCControllers>();

                if (npcController != null && npcController.identity != null) {
                    isInspecting = true;
                    EntityInspectorUI.instance.InspectNPC(npcController.identity, npcController.gameObject);
                    WorldManager.instance.DisplayCursor();
                    return;
                }
            }

            isInspecting = false;
            StartBlocking();
        }

        if (context.canceled) {
            if (isBlocking)
                StopBlocking();

            isInspecting = false;
        }
    }

    private void StartBlocking()
    {
        isBlocking = true;

        if (currentWeapon != null)
            currentWeapon.Blocking(isBlocking);
    }

    private void StopBlocking()
    {
        isBlocking = false;

        if (currentWeapon != null)
            currentWeapon.Blocking(isBlocking);
    }

    #endregion

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        if (lookInput.sqrMagnitude < 0.01f)
            return;

        transform.Rotate(Vector3.up * (lookInput.x * mouseSensitivity * Time.deltaTime));
    }

    private void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(speed * Time.deltaTime * move);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
