using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CopyToClipboard : MonoBehaviour
{
    private void OnMouseDown()
    {
        GUIUtility.systemCopyBuffer = GetComponent<TextMeshProUGUI>().text;
    }
}
