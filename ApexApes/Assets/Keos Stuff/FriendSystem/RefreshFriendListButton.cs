using UnityEngine;

public class RefreshFriendListButton : MonoBehaviour
{
    public string HandTag = "HandTag";
    public FriendingManager Manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            Manager.RefreshFriendList();
        }
    }
}
