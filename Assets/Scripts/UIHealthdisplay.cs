using UnityEngine;
using TMPro;

public class UIHealthdisplay : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    
    
    private int maxHealth;
    private int currentHealth;
    [SerializeField]private Sprite eyeSprite;
    [SerializeField]private Sprite deadeyeSprite;
    private BeaconSO beacon;
    
    void Awake()
    {
        beacon.uiChannel.OnChangeHealth += updateText;
        healthText = GetComponent<TextMeshProUGUI>();
    }
    

    void updateText(int health)
    {
        currentHealth = health;
        if(healthText != null)
        {
            healthText.text = $"{health}";
        }
    }

    private void OnDestroy()
    {
        if (beacon != null)
        {
            beacon.uiChannel.OnChangeHealth -= updateText;
        }
    }

}
