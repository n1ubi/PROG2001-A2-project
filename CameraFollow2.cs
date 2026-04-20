using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform target; // 要跟随的目标物体
    public Vector3 offset = new Vector3(0f, 5f, -10f); // 相机相对于目标的偏移
    public float smoothSpeed = 0.125f; // 平滑跟随的速度
    public bool lookAtTarget = true; // 是否让相机始终看向目标

    private void LateUpdate()
    {
        if (target == null)
            return;

        // 计算相机的目标位置
        Vector3 desiredPosition = target.position + offset;
        
        // 使用平滑插值计算相机的新位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 如果需要看向目标
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}
