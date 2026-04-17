using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class PlayerController : MonoBehaviour
{
    private bool isPaused = false;
    public TextMeshProUGUI pauseTextObject; //UI of 'pause' text

    public AudioManager audioManager;

    //private int count;

    // 拼单词核心
    private string collectedLetters = "";
    public string targetWord = "APPLE";
    public TextMeshProUGUI countText; // the score TMS object
    public GameObject winPanel; // 胜利面板

    public GameObject settingPanel;

    // Rigidbody of the player.
    private Rigidbody rb;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;

    // Speed at which the player moves.
    public float speed = 0;

    // 用来获取主相机方向
    private Camera mainCam;

    // 跳跃相关参数
    public float jumpForce = 5f; // 跳跃力度，可在Inspector调整
    private bool isGrounded = true; // 是否在地面上
    public Transform groundCheck; // 地面检测点（需要在Inspector拖入）
    public float groundCheckRadius = 0.2f; // 检测范围
    public LayerMask groundLayer; // 地面层级（需要在Inspector选择）

    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();

        mainCam = Camera.main; // 自动获取主相机

        ResumeGame();
    }

    // This function is called when a move input is detected.
    void OnMove(InputValue movementValue)
    {
        // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();

        // Store the X and Y components of the movement.
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // 跳跃输入检测（Input System自动识别空格）
    void OnJump()
    {
        // 只有在地面上才能跳，防止空中无限跳
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // FixedUpdate is called once per fixed frame-rate frame.
    void FixedUpdate()
    {
        // 地面检测（每帧判断是否落地）
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // 移动方向跟随镜头
        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;

        // 消除Y轴影响，保证水平移动
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 计算相对镜头的移动方向
        Vector3 movement = camForward * movementY + camRight * movementX;

        // Create a 3D movement vector using the X and Y inputs.
        //Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        // Apply force to the Rigidbody to move the player.
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with has the "PickUp" tag.

        /*if (other.gameObject.CompareTag("PickUp"))
        {
            audioManager.PlayHitSound();

            // Deactivate the collided object (making it disappear).
            other.gameObject.SetActive(false);
            // Update the score
            count++;
            countText.text = "Count: " + count.ToString();
        }

        else if (other.gameObject.CompareTag("Bonus"))
        {
            audioManager.PlayHitSound();

            // Deactivate the collided object (making it disappear).
            other.gameObject.SetActive(false);
            // Update the score
            count=count+5;
            countText.text = "Count: " + count.ToString();
        }*/

        if (other.gameObject.CompareTag("Letter"))
        {
            Letter letterComp = other.GetComponent<Letter>();
            if (letterComp == null) return;

            char nextExpected = GetNextExpectedLetter();
            char got = char.ToUpper(letterComp.letter);

            // 只有对的才收，错的不收
            if (got == nextExpected)
            {
                audioManager.PlayHitSound();
                collectedLetters += got;
                other.gameObject.SetActive(false);
                UpdateCollectedUI();
                CheckWin();
            }
        }
    }

    // 获取下一个应该收集的字母
    char GetNextExpectedLetter()
    {
        if (collectedLetters.Length >= targetWord.Length)
            return ' ';

        return targetWord[collectedLetters.Length];
    }


    // 更新已收集字母显示
    void UpdateCollectedUI()
    {
        countText.text = "Letters: " + collectedLetters;
    }

    // 检查是否拼出 APPLE
    void CheckWin()
    {
        if (collectedLetters == targetWord)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        countText.text = "YOU WIN! WORD: " + targetWord;
        // 停止移动
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        speed = 0;

        // 胜利面板打开
        if (winPanel != null)
            winPanel.SetActive(true);

    }

    void PauseGame()
    {
        Time.timeScale = 0f;//pause the time
        isPaused = true;
        pauseTextObject.gameObject.SetActive(true);
        settingPanel.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;//pause the time
        isPaused = false;
        pauseTextObject.gameObject.SetActive(false);
        settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(isPaused == true)
                ResumeGame();
            else
                PauseGame();
        }
    }
}