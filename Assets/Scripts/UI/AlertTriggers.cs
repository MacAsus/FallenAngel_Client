using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertTriggers : MonoBehaviour
{
    public static void TriggerAlert(Alert alert)
    {
        FindObjectOfType<AlertManager>().StartAlert(alert);
    }
}