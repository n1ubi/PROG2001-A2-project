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
    public GameObject losePanel; // 失败面板

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
    public Transform groundCheck; // 地面检测点
    public float groundCheckRadius = 0.2f; // 检测范围
    public LayerMask groundLayer; // 地面层级

    // 掉落失败逻辑
    public float fallThreshold = -10.5f; // 低于这个高度判定为失败
    private bool isGameOver = false;

    // 动画控制
    public Animator snakeAnimator; 
    public float eatAnimDuration = 0.5f; // 吃动画持续时间

    // 旋转速度
    public float turnSpeed = 15f;

    private bool isEating = false;

    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();

        mainCam = Camera.main; // 自动获取主相机

        ResumeGame();

        // 自动获取蛇身上的Animator
        if (snakeAnimator == null)
            snakeAnimator = GetComponent<Animator>();
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

    // 跳跃输入检测
    void OnJump()
    {
        // 防止空中无限跳
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // FixedUpdate is called once per fixed frame-rate frame.
    void FixedUpdate()
    {
        // 地面检测
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

        // 切换Walk/Idle动画
        bool isMoving = movement.magnitude > 0.1f;
        if (!isEating)
        {
            snakeAnimator.SetBool("IsWalking", isMoving);
        }

        // 让蛇朝向移动方向
        if (movement.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
        }
    }

    // 吃字母动画
    IEnumerator PlayEatAnimation()
    {
        isEating = true;

        snakeAnimator.SetBool("IsWalking", false);
        snakeAnimator.SetBool("IsEating", true);

        yield return new WaitForSeconds(eatAnimDuration);
        snakeAnimator.SetBool("IsEating", false);

        isEating = false;
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

                // 收集字母播放Eat动画
                StartCoroutine(PlayEatAnimation());
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

    // 游戏失败逻辑
    void GameOver()
    {
        isGameOver = true;
        countText.text = "YOU FELL IN THE WATER!";
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        speed = 0;

        if (losePanel != null)
            losePanel.SetActive(true);
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

        // 掉落检测
        if (transform.position.y < fallThreshold)
        {
            GameOver();
        }
    }
}