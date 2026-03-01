using Unity.Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class CameraDistanceCycler : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private InputActionAsset inputActionsAsset;
    [SerializeField] private string gameplayMapName = "Gameplay";
    [SerializeField] private string cameraZoomActionName = "CameraZoomCycle";
    [SerializeField] private float closeRadius = 7f;
    [SerializeField] private float farRadius = 10f;
    [SerializeField] private float extraFarRadius = 16f;
    [SerializeField] private float transitionDuration = 0.45f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private InputAction _cameraZoomCycleAction;
    private int _currentPresetIndex;
    private Coroutine _radiusTransitionCoroutine;

    private void Awake()
    {
        if (orbitalFollow == null)
        {
            orbitalFollow = GetComponentInChildren<CinemachineOrbitalFollow>(true);
        }

        if (inputActionsAsset != null)
        {
            InputActionMap gameplayMap = inputActionsAsset.FindActionMap(gameplayMapName, false);
            _cameraZoomCycleAction = gameplayMap?.FindAction(cameraZoomActionName, false);
        }

        _currentPresetIndex = 0;
        ApplyCurrentPreset();
    }

    private void OnEnable()
    {
        if (_cameraZoomCycleAction == null)
        {
            return;
        }

        _cameraZoomCycleAction.performed += OnCameraZoomCyclePerformed;
        _cameraZoomCycleAction.Enable();
    }

    private void OnDisable()
    {
        if (_cameraZoomCycleAction == null)
        {
            return;
        }

        _cameraZoomCycleAction.performed -= OnCameraZoomCyclePerformed;
        _cameraZoomCycleAction.Disable();

        if (_radiusTransitionCoroutine != null)
        {
            StopCoroutine(_radiusTransitionCoroutine);
            _radiusTransitionCoroutine = null;
        }
    }

    private void OnCameraZoomCyclePerformed(InputAction.CallbackContext _)
    {
        _currentPresetIndex = (_currentPresetIndex + 1) % 3;

        if (orbitalFollow == null)
        {
            return;
        }

        float fromRadius = orbitalFollow.Radius;
        float toRadius = GetRadiusForPreset(_currentPresetIndex);

        if (_radiusTransitionCoroutine != null)
        {
            StopCoroutine(_radiusTransitionCoroutine);
        }

        _radiusTransitionCoroutine = StartCoroutine(TransitionRadius(fromRadius, toRadius));
    }

    private void ApplyCurrentPreset()
    {
        if (orbitalFollow == null)
        {
            return;
        }

        orbitalFollow.Radius = GetRadiusForPreset(_currentPresetIndex);
    }

    private float GetRadiusForPreset(int presetIndex)
    {
        switch (presetIndex)
        {
            case 1:
                return farRadius;
            case 2:
                return extraFarRadius;
            default:
                return closeRadius;
        }
    }

    private IEnumerator TransitionRadius(float fromRadius, float toRadius)
    {
        if (transitionDuration <= 0f)
        {
            orbitalFollow.Radius = toRadius;
            _radiusTransitionCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float easedT = easeCurve != null ? easeCurve.Evaluate(t) : t;
            orbitalFollow.Radius = Mathf.Lerp(fromRadius, toRadius, easedT);
            yield return null;
        }

        orbitalFollow.Radius = toRadius;
        _radiusTransitionCoroutine = null;
    }
}
