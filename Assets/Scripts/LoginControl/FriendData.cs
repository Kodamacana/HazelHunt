using UnityEngine;

public struct FriendData {

    private string username;
    private string photoUrl;
    private int score;
    private int nut;
    private bool onlineStatus;
    private string userId;

    public string Username { get => username; }
    public string PhotoUrl { get => photoUrl; }
    public int Score { get => score; }
    public int Nut { get => nut; }
    public bool OnlineStatus { get => onlineStatus; }
    public string UserId { get => userId; }

    public FriendData(string username, string photoUrl, int score, int nut, bool onlineStatus, string userId)
    {
        this.username = username;
        this.photoUrl = photoUrl;
        this.score = score;
        this.nut = nut;
        this.onlineStatus = onlineStatus;
        this.userId = userId;
    }
}