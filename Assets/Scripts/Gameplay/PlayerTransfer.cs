using UnityEngine;

public class PlayerTransfer : MonoBehaviour
{
    public int SelectedPlayerId = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
