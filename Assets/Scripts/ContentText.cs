using UnityEngine.UI;
using UnityEngine;

public class ContentText : MonoBehaviour
{
    private Text text;
    public void SetElement(string msg)
    {
        if(text == null) text = GetComponent<Text>();
        
        text.text = string.Format("{0}", msg);
    }
}
