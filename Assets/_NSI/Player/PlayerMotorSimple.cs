using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMotorSimple : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private Transform moveFrame;

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
        Vector3 frameForward = moveFrame != null ? moveFrame.forward : Vector3.forward;
        Vector3 frameRight = moveFrame != null ? moveFrame.right : Vector3.right;

        frameForward.y = 0f;
        frameRight.y = 0f;

        frameForward.Normalize();
        frameRight.Normalize();

        Vector3 planarMove = frameForward * moveInput.y + frameRight * moveInput.x;
        if (planarMove.sqrMagnitude > 1f)
        {
            planarMove.Normalize();
        }

        if (planarMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(planarMove, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        float speed = _inputRouter.SprintHeld ? sprintSpeed : walkSpeed;

        _verticalVelocity += -9.81f * Time.deltaTime;

        Vector3 movement = planarMove * speed;
        movement.y = _verticalVelocity;

        _characterController.Move(movement * Time.deltaTime);
    }
}
