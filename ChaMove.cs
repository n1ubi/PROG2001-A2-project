using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChaMove : MonoBehaviour
{
    public float speed = 3;
    private int count; // 变量用于存储当前分数
    public TextMeshProUGUI countText; // 分数显示的TextMeshProUGUI对象

    Animator anim;
    Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        count = 0; // 初始化分数
        countText.text = "Count: " + count.ToString(); // 初始化文本显示
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = new Vector3(x, 0, z);

        transform.LookAt(transform.position + new Vector3(x, 0, z));
        transform.position += new Vector3(x, 0, z) * speed * Time.deltaTime;

        UpdateAnim();
    }

    void UpdateAnim()
    {
        anim.SetFloat("Speed", move.magnitude);
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查玩家碰撞的对象是否有“PickUp”标签
        if (other.gameObject.CompareTag("PickUp"))
        {
            // 禁用碰撞的对象（使其消失）
            other.gameObject.SetActive(false);
            // 更新分数
            count++;
            countText.text = "Count: " + count.ToString();
        }
        else if (other.gameObject.CompareTag("Bonus"))
        {
            // 禁用碰撞的对象（使其消失）
            other.gameObject.SetActive(false);
            // 更新分数
            count += 5;
            countText.text = "Count: " + count.ToString();
        }
    }
}
