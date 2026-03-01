using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputRouter : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionsAsset;

    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }
    public bool InteractPressedThisFrame { get; private set; }
    public bool BuildModePressedThisFrame { get; private set; }

    private InputActionAsset _inputActionsInstance;
    private InputActionMap _gameplayMap;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _crouchAction;
    private InputAction _interactAction;
    private InputAction _buildModeToggleAction;
    private bool _isInitialized;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (_gameplayMap == null)
        {
            Initialize();
        }

        _gameplayMap?.Enable();
    }

    private void OnDisable()
    {
        _gameplayMap?.Disable();
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            Move = Vector2.zero;
            Look = Vector2.zero;
            JumpPressedThisFrame = false;
            SprintHeld = false;
            CrouchHeld = false;
            InteractPressedThisFrame = false;
            BuildModePressedThisFrame = false;
            return;
        }

        Move = _moveAction.ReadValue<Vector2>();
        Look = _lookAction.ReadValue<Vector2>();
        JumpPressedThisFrame = _jumpAction.WasPressedThisFrame();
        SprintHeld = _sprintAction.IsPressed();
        CrouchHeld = _crouchAction.IsPressed();
        InteractPressedThisFrame = _interactAction.WasPressedThisFrame();
        BuildModePressedThisFrame = _buildModeToggleAction.WasPressedThisFrame();

        if (JumpPressedThisFrame)
        {
            Debug.Log("Jump pressed");
        }
    }

    private void OnDestroy()
    {
        if (_inputActionsInstance != null)
        {
            Destroy(_inputActionsInstance);
            _inputActionsInstance = null;
        }
    }

    private void Initialize()
    {
        if (_inputActionsInstance != null || inputActionsAsset == null)
        {
            return;
        }

        _inputActionsInstance = Instantiate(inputActionsAsset);
        _gameplayMap = _inputActionsInstance.FindActionMap("Gameplay", false);
        if (_gameplayMap == null)
        {
            return;
        }

        _moveAction = _gameplayMap.FindAction("Move", false);
        _lookAction = _gameplayMap.FindAction("Look", false);
        _jumpAction = _gameplayMap.FindAction("Jump", false);
        _sprintAction = _gameplayMap.FindAction("Sprint", false);
        _crouchAction = _gameplayMap.FindAction("Crouch", false);
        _interactAction = _gameplayMap.FindAction("Interact", false);
        _buildModeToggleAction = _gameplayMap.FindAction("BuildModeToggle", false);

        _isInitialized = _moveAction != null
            && _lookAction != null
            && _jumpAction != null
            && _sprintAction != null
            && _crouchAction != null
            && _interactAction != null
            && _buildModeToggleAction != null;
    }
}
