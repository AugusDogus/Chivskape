using UnityEngine;
using System.Collections.Generic;
using SocketIO;
using System;

public class Network : MonoBehaviour {

    static SocketIOComponent socket;
    public GameObject playerPrefab;
    public GameObject myPlayer;
    Dictionary<string, GameObject> players;

    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("spawn", OnSpawn);
        socket.On("move", OnMove);
        socket.On("disconnected", OnDisconnected);
        socket.On("requestPosition", onRequestPosition);
        socket.On("updatePosition", onUpdatePosition);

        players = new Dictionary<string, GameObject>();
    }

    void OnConnected(SocketIOEvent e)
    {
        Debug.Log("connected");
    }

    void onRequestPosition(SocketIOEvent e)
    {
        Debug.Log("server requested my position");
        socket.Emit("updatePosition", new JSONObject(VectorToJson(myPlayer.transform.position)));

    }

    void onUpdatePosition(SocketIOEvent e)
    {
        Debug.Log("updating position: " + e.data);

        var position = new Vector3(GetFloatFromJson(e.data, "x"), 0, GetFloatFromJson(e.data, "y"));

        var player = players[e.data["id"].ToString()];

        player.transform.position = position;
    }

    void OnDisconnected(SocketIOEvent e)
    {
        Debug.Log("disconnected with id: " + e.data);

        var id = e.data["id"].ToString();

        var player = players[id];
        Destroy(player);
        players.Remove(id);
    }

    void OnSpawn(SocketIOEvent e)
    {
        Debug.Log("spawned" + e.data);
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (e.data["x"]) {

            var movePosition = new Vector3(GetFloatFromJson(e.data, "x"), 0, GetFloatFromJson(e.data, "y"));

            var navigatePos = player.GetComponent<NavigatePosition>();

            navigatePos.NavigateTo(movePosition);
        }

        players.Add(e.data["id"].ToString(), player);
        Debug.Log("count: " + players.Count);
    }

    void OnMove(SocketIOEvent e)
    {
        Debug.Log("player moved" + e.data);

        var position = new Vector3(GetFloatFromJson(e.data, "x"), 0, GetFloatFromJson(e.data, "y"));

        var player = players[e.data["id"].ToString()];

        var navigatePos = player.GetComponent<NavigatePosition>();

        navigatePos.NavigateTo(position);
        
    }

    float GetFloatFromJson(JSONObject data, string key)
    {
        return float.Parse(data[key].ToString().Replace("\"", ""));
    }

    public static string VectorToJson(Vector3 vector)
    {
        return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", vector.x, vector.z);
    }
}
