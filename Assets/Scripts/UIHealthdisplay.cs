


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHealthdisplay : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    private MainCharacter MainCharacter;
    // Reference to the Text component
    // Start is called before the first frame update
    void Start()
    {
        
        MainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        if(!MainCharacter)
            Debug.LogError("MainCharacter not found");
        healthText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = $"{MainCharacter.CurrentHits}";
    }
}
