using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class BaseManager : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI connectionStateText;
    [SerializeField] protected TMP_InputField ipField;
    [SerializeField] protected TMP_InputField portField;
    [SerializeField] protected TMP_InputField messageField;

    [SerializeField] protected TextMeshProUGUI webSocketMessageText;
    [SerializeField] protected TextMeshProUGUI udpMessageText;


    [SerializeField] protected Button webServerButton;
    [SerializeField] protected Button udpServerButton;

    [SerializeField] protected Button webClientButton;
    [SerializeField] protected Button udpClientButton;


    [SerializeField] protected Button udpSendButton;
    [SerializeField] protected Button webSendButton;
}