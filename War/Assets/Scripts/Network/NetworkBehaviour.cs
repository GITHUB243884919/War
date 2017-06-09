using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class NetworkBehaviour:TaskExecutor
{ 
	private bool isHoldNetwork = false;

	protected void Awake()
	{  
		base.Awake ();
		TCPHelper.WorldClient.OnStatusChanged += OnNetworkStatus;
		TCPHelper.GameClient.OnStatusChanged += OnNetworkStatus;

		DNSHelper.OnDNSReady += OnDNSServerReady;
		DNSHelper.OnDNSError += OnDNSServerError;

		if (TCPHelper.WorldClient.Status == NetworkStatus.Error) { 
			OnNetworkStatus(TCPHelper.WorldClient,NetworkStatus.Error);	
		}

		if (TCPHelper.GameClient.Status == NetworkStatus.Error) { 
			OnNetworkStatus(TCPHelper.GameClient,NetworkStatus.Error);	
		}

		GameObject.DontDestroyOnLoad (gameObject); 
	}

	protected void OnDestroy(){ 
		TCPHelper.WorldClient.OnStatusChanged -= OnNetworkStatus;
		TCPHelper.GameClient.OnStatusChanged -= OnNetworkStatus;

		TCPHelper.OnDestroy ();
	}

	protected void LateUpdate () {		
		TCPHelper.Tick ();
		base.Tick ();

		if (Input.GetKeyUp (KeyCode.Escape)) {
			OnBackClick();
		}
	}	

	protected virtual void OnBackClick(){
		
		MessageBox.Show ("是否退出游戏?", delegate (MessageBoxButton button){
			if(button == MessageBoxButton.ButtonOk){
				Application.Quit();
			}
		});
	}
 
	void OnApplicationPause(bool isPause){ 
#if !UNITY_EDITOR
		Debug.LogFormat ("OnApplicationPause {0}",isPause);

		if (isPause) {
			if (TCPHelper.WorldClient.IsConnected) {
				LoginManager.Disconnect ();
				isHoldNetwork = true;
			}
		} else {
			if (isHoldNetwork) {
				LoadingTips.Show();
				LoginManager.Reconnect ();
			}
		}
#endif

	}
	
	protected void OnNetworkStatus(TCPClient client,NetworkStatus status){
		
		Debug.LogFormat ("{0} OnNetworkStatus {1}",client.ClientType,status);
		if (status == NetworkStatus.Connected) {
			ScheduleTask (delegate { 
				//LoadingTips.Hide (); 
				OnConnected(client.ClientType); 
			});	
		} else if(status == NetworkStatus.Fail){
			ScheduleTask (delegate {
				LoadingTips.Hide (); 
				LoginManager.NotifyLoginError();
				GameEventCenter.getEventCenter ().DispatchEvent (new GameEvent((int)GameEventCode.EVENT_CONNECT_ERROR));
			});
		}else if(status == NetworkStatus.Timeout){
			ScheduleTask (delegate {
				LoadingTips.Hide (); 
				LoginManager.NotifyLoginTimeout();
				GameEventCenter.getEventCenter ().DispatchEvent (new GameEvent((int)GameEventCode.EVENT_CONNECT_ERROR));
			});
		}else if(status == NetworkStatus.Error){
			if (client.ClientType == ClientType.WORLD) {
				TCPHelper.GameClient.Stop ();
			}
			ScheduleTask (delegate {
				LoadingTips.Hide (); 
				LoginManager.NotifyConnectError();						 
				OnDisConnected(client.ClientType);
			});	
		}		
	} 

	protected virtual void OnConnected(ClientType type){
		if (type == ClientType.WORLD) {
			TCPHelper.GameClient.Stop();
			LoginManager.LoginToWorldServer ();
		} else {
			LoginManager.LoginToGameServer ();
		}
		GameEventCenter.getEventCenter ().DispatchEvent (new GameEvent((int)GameEventCode.EVENT_CONNECT_SUCCESS,type));
    }

	protected virtual void OnDisConnected(ClientType type){
		GameEventCenter.getEventCenter ().DispatchEvent (new GameEvent((int)GameEventCode.EVENT_CONNECT_CLOSED,type));
	}

	protected virtual void OnDNSServerReady(ServerInfo serverInfo){
		LoadingTips.Hide (); 
		TCPHelper.InitWorldClient (serverInfo.ip, serverInfo.port); 
	}

	protected virtual void OnDNSServerError(string error){
		LoadingTips.Hide (); 
		MessageBox.Show(string.Format("DNS检测失败{0},是否重试?",error),delegate(MessageBoxButton button){
			if(button == MessageBoxButton.ButtonOk){
				LoadingTips.Show(null);
				DNSHelper.Request(this);
			}else{
				Application.Quit();
			}
		});		
	}
} 

