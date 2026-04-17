using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Reference to the player GameObject.
    public GameObject player;

    // The distance between the camera and the player.
    private Vector3 offset;

    // 鼠标控制视角
    public float mouseSensitivity = 2f;
    public float minYAngle = -20f;   // 向下看限制
    public float maxYAngle = 60f;    // 向上看限制

    private float rotationX = 0f;
    private float rotationY = 0f;

    // Start is called before the first frame update.
    void Start()
    {
        // Calculate the initial offset between the camera's position and the player's position.
        offset = transform.position - player.transform.position;

        // 记录初始角度
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    // LateUpdate is called once per frame after all Update functions have been completed.
    void LateUpdate()
    {
        // 鼠标控制旋转
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 限制上下角度，防止镜头翻转
        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

        // 计算新的相机位置
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Maintain the same offset between the camera and player throughout the game.
        transform.position = player.transform.position + rotation * offset;

        // 镜头始终看着玩家
        transform.LookAt(player.transform.position);
    }
}