using System; 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;


public struct ServerInfo{
	public string ip;
	public int port;

	public ServerInfo(string ip,int port){
		this.ip = ip;
		this.port = port;
	}
}

public class DNSHelper{ 

	private static String dnsUrl = "http://112.126.83.225/redalert/servers";

	private static ServerInfo serverInfo = new ServerInfo("192.168.1.89",8739); 

	public delegate void OnDNSReadyEvent(ServerInfo serverInfo);
	public delegate void OnDNSErrorEvent(string error);

	public static event OnDNSReadyEvent OnDNSReady;
	public static event OnDNSErrorEvent OnDNSError;

	DNSHelper(){
	}

	public static void Request(MonoBehaviour context){
		WWW www = new WWW (dnsUrl);  
		context.StartCoroutine (WaitForRequest (www));
	}

	static IEnumerator WaitForRequest(WWW www){
		yield return www;
		if (www.error != null) {
			if(OnDNSError!=null){
				OnDNSError(www.error);
			}
		} else if(www.isDone){ 
			string[] lines = www.text.Split('\n');

			foreach(string line in lines){
				string[] props = line.Split(':');
				if(props.Length<2) continue;

				serverInfo.ip = props[0];
				int.TryParse(props[1],out serverInfo.port);
				if(OnDNSReady!=null){ 
#if UNITY_EDITOR
					//serverInfo.ip = "127.0.0.1";
#endif
					OnDNSReady(serverInfo);
				} 
			}
		} 
	}

	public static ServerInfo ServerInfo{
		get {return serverInfo;}
	}
} 

