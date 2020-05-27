﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public Text nameText;
    public Text attributesText;
    public Text descriptionText;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void Show(ItemInformation itemInfo)
    {
        canvasGroup.alpha = 1;
        nameText.text = itemInfo.Name;
        attributesText.text = itemInfo.Attributes;
        descriptionText.text = itemInfo.Description;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }
}
