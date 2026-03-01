using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMotorSimple : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6.5f;

    private CharacterController _characterController;
    private InputRouter _inputRouter;
    private float _verticalVelocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputRouter = GetComponentInParent<InputRouter>();

        if (_inputRouter == null)
        {
            _inputRouter = FindFirstObjectByType<InputRouter>();
        }
    }

    private void Update()
    {
        if (_characterController == null || _inputRouter == null)
        {
            return;
        }

        if (_characterController.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        Vector2 moveInput = _inputRouter.Move;
        Vector3 planarMove = new Vector3(moveInput.x, 0f, moveInput.y);
        if (planarMove.sqrMagnitude > 1f)
        {
            planarMove.Normalize();
        }

        float speed = _inputRouter.SprintHeld ? sprintSpeed : walkSpeed;

        _verticalVelocity += -9.81f * Time.deltaTime;

        Vector3 movement = planarMove * speed;
        movement.y = _verticalVelocity;

        _characterController.Move(movement * Time.deltaTime);
    }
}
