using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private bool isPaused = false;
    public TextMeshProUGUI pauseTextObject;
    public AudioManager audioManager;

    private int coinCount = 0;
    public int coinsToWin = 10;
    public TextMeshProUGUI countText;

    // 胜利相关
    public TextMeshProUGUI winText;
    public GameObject restartButtonWin;

    // 失败相关
    public TextMeshProUGUI loseText;
    public GameObject restartButtonLose;

    public GameObject settingPanel;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 5f;
    private Camera mainCam;

    public float jumpForce = 5f;
    private bool isGrounded = true;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        ResumeGame();
        UpdateCoinUI();

   
        HideAllUI();
    }

    // 开局统一隐藏所有胜利/失败UI
    void HideAllUI()
    {
        if (winText != null) winText.gameObject.SetActive(false);
        if (restartButtonWin != null) restartButtonWin.SetActive(false);
        if (loseText != null) loseText.gameObject.SetActive(false);
        if (restartButtonLose != null) restartButtonLose.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        if (isGameOver || isPaused) return;
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump()
    {
        if (isGrounded && !isGameOver && !isPaused)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (isGameOver || isPaused) return;

        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 movement = camForward * movementY + camRight * movementX;
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isGameOver || isPaused) return;

        if (other.gameObject.CompareTag("Danger"))
        {
            GameOver();
            return;
        }

        if (other.gameObject.CompareTag("PickUp"))
        {
            if (audioManager != null)
                audioManager.PlayHitSound();

            other.gameObject.SetActive(false);
            coinCount++;
            UpdateCoinUI();
            CheckWin();
        }
    }

    void UpdateCoinUI()
    {
        if (countText != null)
            countText.text = "Coins: " + coinCount.ToString();
    }

    void CheckWin()
    {
        if (coinCount >= coinsToWin)
            WinGame();
    }

    void WinGame()
    {
        isGameOver = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        speed = 0;

        // 先隐藏失败，再显示胜利
        HideAllUI();
        if (winText != null) winText.gameObject.SetActive(true);
        if (restartButtonWin != null) restartButtonWin.SetActive(true);
    }

    void GameOver()
    {
        isGameOver = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        speed = 0;

        // 先隐藏胜利，再显示失败
        HideAllUI();
        if (loseText != null) loseText.gameObject.SetActive(true);
        if (restartButtonLose != null) restartButtonLose.SetActive(true);
    }

    void PauseGame()
    {
        if (isGameOver) return;
        Time.timeScale = 0f;
        isPaused = true;
        pauseTextObject.gameObject.SetActive(true);
        settingPanel.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseTextObject.gameObject.SetActive(false);
        settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
