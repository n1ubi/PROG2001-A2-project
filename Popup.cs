using UnityEngine;

public class Popup : MonoBehaviour
{
    // œ‘ æµØ¥∞
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // πÿ±’µØ¥∞
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}