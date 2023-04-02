using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogMessage : MonoBehaviour
{
    UnityAction confirmAction;
    UnityAction cancelAction;

    public void OpenMessage(string msg, UnityAction confirm = null, UnityAction cancel = null)
    {
        
    }
}
