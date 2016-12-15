using UnityEngine;
using System.Collections;

public class ClickMove : MonoBehaviour {

    public GameObject player;

	public void onClick (Vector3 position) {
        var navPos = player.GetComponent<NavigatePosition>();
        var netMove = player.GetComponent<NetworkMove>();
        navPos.NavigateTo(position);
        netMove.OnMove(position);
	}
}
