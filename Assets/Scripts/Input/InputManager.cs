using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private PlayerControls playerControls;

    void Awake()
    {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        playerControls = new();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.GamePlay.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.GamePlay.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumped()
    {
        return playerControls.GamePlay.Jump.triggered;
    }
}
