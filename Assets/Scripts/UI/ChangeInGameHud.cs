using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeInGameHud : MonoBehaviour
{
    private Image imageComp;
    public Sprite deafult;
    [SerializeField]BeaconSO beacon;
    private void Awake()
    {
        imageComp = GetComponent<Image>();
        imageComp.sprite = deafult;
        beacon.uiChannel.OnHudChange += ChangeHud;
    }

    private void ChangeHud(Sprite newSprite)
    {
        if(newSprite == null)
        {
            imageComp.sprite = deafult;
            return;
        }
        imageComp.sprite = newSprite;
    }
}
