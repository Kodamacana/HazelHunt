using System;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class FriendObject : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public Image selectedUI;
    [SerializeField] private Image background;
    [SerializeField] private Image Avatar;
    [SerializeField] private Image AvatarCanvas;
    [SerializeField] private Image score_ico;
    [SerializeField] private Image nuts_ico;
    [SerializeField] private TextMeshProUGUI username_txt;
    [SerializeField] private TextMeshProUGUI score_txt;
    [SerializeField] private TextMeshProUGUI nuts_txt;

    [Header("Materials")]
    [SerializeField] private Material blackSpriteMaterial;
    [SerializeField] private TMP_FontAsset blackFontMaterial;

    [Space(10)]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private TMP_FontAsset normalFontMaterial;

    public string UID;
    private bool isSelectable = false;
    private InvitesManager InvitesManager;
    private FriendshipManager friendshipManager;

    private void Start()
    {
        InvitesManager = InvitesManager.Instance;
        friendshipManager = FriendshipManager.Instance;
    }
    internal void Init(string username_txt, string score_txt, string nuts_txt, bool isOnline, string UID)
    {
        //Avatar = avatar;
        this.username_txt.text = username_txt;
        this.score_txt.text = score_txt;
        this.nuts_txt.text = nuts_txt;
        this.UID = UID;

        if (isOnline)
            OnlineFriend();
        else
            OfflineFriend();
    }

    private void OfflineFriend()
    {
        isSelectable = false;
        background.material = blackSpriteMaterial;
        Avatar.material = blackSpriteMaterial;
        AvatarCanvas.material = blackSpriteMaterial;
        score_ico.material = blackSpriteMaterial;
        nuts_ico.material = blackSpriteMaterial;

        username_txt.font = blackFontMaterial;
        score_txt.font = blackFontMaterial;
        nuts_txt.font = blackFontMaterial;
    }

    private void OnlineFriend()
    {
        isSelectable = true;
        background.material = normalMaterial;
        Avatar.material = normalMaterial;
        AvatarCanvas.material = normalMaterial;
        score_ico.material = normalMaterial;
        nuts_ico.material = normalMaterial;

        username_txt.font = normalFontMaterial;
        score_txt.font = normalFontMaterial;
        nuts_txt.font = normalFontMaterial;
    }

    private void OnMouseDown()
    {
        if (!isSelectable)
            return;

        friendshipManager.SelectFriendObject(this, UID);
        InvitesManager.SelectFriendObject(this, UID);

        GetComponent<BoxCollider2D>().enabled = false;
        selectedUI.gameObject.SetActive(true);
    }
}