using Assets.Scripts.Network;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerNetworkManager))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerNetworkManager player = (PlayerNetworkManager)target;

        GUI.enabled = Application.isPlaying && PhotonNetwork.CurrentRoom != null;

        if (GUILayout.Button("Kick"))
            player.view.RPC("Kick", RpcTarget.All);
    }
}
