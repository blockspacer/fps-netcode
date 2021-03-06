﻿using UnityEngine;
using LiteNetLib;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Net;
using KinematicCharacterController;

/// Base component for primary logic and dependencies needed by both client and server.
[RequireComponent(typeof(NetworkObjectManager))]
public abstract class BaseLogicController : MonoBehaviour {
  public NetMonitor netMonitor;
  public DebugNetworkSettings debugNetworkSettings;

  // Delegates.
  protected NetworkObjectManager networkObjectManager;
  protected NetChannel netChannel;
  protected PlayerManager playerManager;

  protected virtual void Awake() {
    networkObjectManager = GetComponent<NetworkObjectManager>();
    playerManager = new PlayerManager();
    netChannel = new NetChannel(debugNetworkSettings);
    if (netMonitor != null) {
      netChannel.SetNetMonitor(netMonitor);
    }

    netChannel.PeerConnectedHandler += OnPeerConnected;
    netChannel.PeerDisconnectedHandler += OnPeerDisconnected;
  }

  protected virtual void Start() { }

  protected virtual void Update() {
    netChannel.Update();

    if (Input.GetKeyDown(KeyCode.F1)) {
      TearDownGameScene();
      LoadLobbyScene();
    }
  }

  private void OnApplicationQuit() {
    TearDownGameScene();
  }

  public void PingServer(IPEndPoint endpoint, Action<int> callback) {
    netChannel.PingServer(endpoint, callback);
  }

  protected void LoadGameScene() {
    SceneManager.LoadScene("Game");
  }

  protected virtual void TearDownGameScene() {
    Debug.Log("Stopping network stack.");
    Cursor.lockState = CursorLockMode.None;
    playerManager.Clear();
    networkObjectManager.Clear();
    netChannel.Stop();
  }

  protected void LoadLobbyScene() {
    SceneManager.LoadScene("Lobby");
  }

  protected abstract void OnPeerConnected(NetPeer peer);

  protected abstract void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);
}
