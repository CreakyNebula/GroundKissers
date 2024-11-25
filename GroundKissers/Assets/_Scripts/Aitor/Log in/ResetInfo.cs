using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResetInfo : MonoBehaviour
{
    public TMP_InputField userIF;
    public TMP_InputField passIF;

    public void ClearInputFields()
    {
        if (userIF != null)
        {
            userIF.text = string.Empty;
        }

        if (passIF != null)
        {
            passIF.text = string.Empty;
        }
    }
}
