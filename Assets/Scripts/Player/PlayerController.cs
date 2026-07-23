using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float topClamp = 85f;
    [SerializeField] private float bottomClamp = -85f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float cameraAngle = 0f;
    private InputManager inputManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.instance;
    }

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

    void Update()
    {
        // Vector3 move = new(moveInput.x, 0, moveInput.y);
        // // Vector3 movement = inputManager.GetPlayerMovement();
        // // Vector3 move = new(movement.x, 0f, movement.y);


        // controller.Move(speed * Time.deltaTime * move);

        // if (move != Vector3.zero)
        //     gameObject.transform.forward = move;

        // velocity.y += gravity * Time.deltaTime;
        // controller.Move(velocity * Time.deltaTime);
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        if (lookInput.sqrMagnitude < 0.01f)
            return;

        cameraAngle -= lookInput.y * mouseSensitivity;
        cameraAngle = Mathf.Clamp(cameraAngle, bottomClamp, topClamp);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(cameraAngle, 0f, 0f);

        transform.Rotate(Vector3.up * (lookInput.x * mouseSensitivity));
    }

    private void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
