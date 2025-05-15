using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEyeScript : MonoBehaviour
{
    [SerializeField]private Sprite eyeSprite;
    [SerializeField]private Sprite deadEyeSprite;
    [SerializeField]private Image image;

    public void setImage(bool value)
    {
        if(value)
            image.sprite = eyeSprite;
        else
        {
            image.sprite = deadEyeSprite;
        }
    }
    
}
