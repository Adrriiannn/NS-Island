using UnityEngine;

[DefaultExecutionOrder(10000)]
public sealed class MoveFrameYawSync : MonoBehaviour
{
    [SerializeField] private Transform sourceTransform;

    private void LateUpdate()
    {
        if (sourceTransform == null)
        {
            return;
        }

        Vector3 euler = sourceTransform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
    }
}