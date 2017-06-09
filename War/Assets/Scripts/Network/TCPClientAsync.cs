
using System; 
using UnityEngine;
using System.Collections.Generic; 
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Assertions;
using System.Timers;
 
public class TCPClientAsync : TCPClient
{ 
    //ע���TcpClient��C#������
	private TcpClient client = null;
	private NetworkStream networkStream; 
	private System.Timers.Timer timer = new System.Timers.Timer(SEND_TIMEOUT);
	public TCPClientAsync(ClientType type):base(type)
    { 
        //��ʱtimer��ʼ��
		timer.AutoReset = false;
		timer.Elapsed += new System.Timers.ElapsedEventHandler(OnConnectTimeout);
	}

	public override void Dispose()
    { 
		Reset ();
		timer.Dispose ();
	}

    /// <summary>
    /// Reset����
    /// 1.����base��Reset
    /// 2.networkStream��
    /// 3.�ڴ�stream��
    /// 4.client��
    /// </summary>
    /// <param name="cleanup"></param>
	public override void Reset(bool cleanup = true)
    {
		base.Reset (cleanup);
		timer.Enabled = false;

		if (networkStream != null) 
        {
			networkStream.Close();
            networkStream = null;
		}
		
        if (client != null && client.Connected) 
        {  
			client.Close();
            client = null;
		}
		stream.Seek (0, SeekOrigin.Begin);
	}
	 
	/// <summary>
	/// ������������
	/// </summary>
	/// <returns></returns>
	public override bool Start()
    { 
        if (Status != NetworkStatus.Idle) 
        { 
			Debug.LogWarningFormat("tcp client start failed when status is {0}",status);
			//NotifyStatusChange(status);
			//return false;
		}
		 
		Reset (); 

		Assert.IsNull (client);  
  
		client = new TcpClient (); 
		client.ReceiveTimeout = RECV_TIMEOUT;
		client.SendTimeout = SEND_TIMEOUT; 
		client.SendBufferSize = SEND_BUFF_SIZE; 
		 
		client.NoDelay = true;

		if (!client.NoDelay) 
        {
			Debug.LogErrorFormat ("{0} NoDelay {1}",type, client.NoDelay);
		}
		Assert.IsTrue (client.NoDelay);

		try
        {
			Status = NetworkStatus.Connecting;
			var result = client.BeginConnect (ip, port,OnConnect,null);
			timer.Enabled = true; 
		}catch(Exception exe)
        {
			Debug.LogError(exe);
			NotifyStatusChange(NetworkStatus.Fail);
			Status = NetworkStatus.Idle;
		}
        Debug.Log("tcp client start");
		return true;
	}
	
	public override void Stop()
    { 
		Reset (); 
	} 
 
	
	//[NoToLuaAttribute]
	/// <summary>
	/// ������Ϣ
	/// </summary>
	/// <param name="cmd"></param>
	/// <param name="data"></param>
    public override  void Send(int cmd,byte[] data)
    {
		if (networkStream == null) {
			Debug.LogWarningFormat("tcp client not connected , status is {0}",status);
			return;
		}
		
		byte[] cmdBytes = System.BitConverter.GetBytes (IPAddress.HostToNetworkOrder (cmd));
		byte[] lengthBytes = System.BitConverter.GetBytes (IPAddress.HostToNetworkOrder (data.Length + 5)); 
		
		byte[] bytes = new byte[PACKAGE_HEADER_LENGTH + data.Length];  

		Array.Copy (lengthBytes,0,bytes,0,lengthBytes.Length);
		bytes [4] = 4;

		Array.Copy (cmdBytes,0,bytes,5,cmdBytes.Length);
		Array.Copy (data,0,bytes,PACKAGE_HEADER_LENGTH,data.Length); 

		try{
			networkStream.BeginWrite (bytes,0,bytes.Length,OnWrite,null);  	
			#if UNITY_EDITOR
            Debug.LogFormat("send {0} with len {1} at time {2}", cmd, data.Length, System.DateTime.Now);
            #endif
		}catch(IOException exe){
			Debug.LogError(exe);
			NotifyStatusChange(NetworkStatus.Fail);
			Status = NetworkStatus.Idle;
		}

	}
	 
	const int BUF_SIZE = 1024 * 10;

	byte[] buffer = new byte[BUF_SIZE];
	bool readingHeader = true;
	//������ϢЭ����
    int cmd = 0;
    //���ĳ���
	int totalSize = 0;

	MemoryStream stream = new MemoryStream();   

    /// <summary>
    /// ����Connect�ɹ���Ļص�
    /// 
    /// </summary>
    /// <param name="result"></param>
	private void OnConnect(IAsyncResult result)
    {		
		client.EndConnect(result);
		timer.Enabled = false; 
		timer.Stop ();
		if (client.Connected)
        {
			Status = NetworkStatus.Connected;
			networkStream = client.GetStream (); 
			
			readingHeader = true; 
			totalSize = 0;
			
			stream.Seek (0, SeekOrigin.Begin);
			stream.SetLength (0);
			networkStream.BeginRead (buffer, 0, PACKAGE_HEADER_LENGTH, OnRead, null); 
			
			NotifyStatusChange (NetworkStatus.Connected);
		} 
        else 
        { 
			NotifyStatusChange(NetworkStatus.Fail);
			Status = NetworkStatus.Idle;
		}
	}

    private void OnConnectTimeout(object source, System.Timers.ElapsedEventArgs e)
    {
        if (Status != NetworkStatus.Connected)
        {
            Debug.LogErrorFormat("Connect to {0} timeout", type);
            NotifyStatusChange(NetworkStatus.Timeout);
            Stop();
            Status = NetworkStatus.Fail;
        }
    }

	private void OnWrite(IAsyncResult result){
		//Debug.Log ("OnWrite");
		networkStream.EndWrite (result);
	}

    /// <summary>
    /// �����ȡ�ص�
    /// �����ֽ���������OnRecieve()����
    /// </summary>
    /// <param name="result"></param>
	private void OnRead(IAsyncResult result)
    {
        try
        {
			int readBytes = networkStream.EndRead (result);
			
			//Debug.LogFormat ("OnRead {0} :{1} ",readingHeader?"Header":"Body",readBytes);
			
			if (readBytes > 0)
            {
				stream.Write (buffer, 0, readBytes);  
				OnRecieve ();
				if (readingHeader)
                {
					//Debug.Log ("OnRead New Header");
					networkStream.BeginRead (buffer, 0, PACKAGE_HEADER_LENGTH - (int)stream.Length, OnRead, null);
				}
                else if (totalSize > 0)
                {
					//Debug.LogFormat ("OnRead New Body {0} {1}",totalSize,stream.Length);
					networkStream.BeginRead (buffer, 0, Math.Min(totalSize - (int)stream.Length,BUF_SIZE), OnRead, null);
				}
                else 
                {
					Debug.LogError ("OnRecieve Error"); 
				}
			}
            else if(Status == NetworkStatus.Connected)
            {
				Debug.LogErrorFormat ("{0} OnRead Error {0}",type,status);
				if(Status == NetworkStatus.Connected)
                { 
					NotifyStatusChange(NetworkStatus.Error);
					Reset(false);
				}
			}
            else
            {
				Debug.Log ("OnRead End");
				NotifyStatusChange(NetworkStatus.Error);
				Reset (false);
			}

		}catch(IOException exe){
			Debug.LogError (exe); 
			NotifyStatusChange(NetworkStatus.Error);
			Reset (false);
		}		
	}
 
	/// <summary>
    /// ����stream�������������Ժ�Ҫ���ø����Process�����������������
    /// ͷһ��int������
    /// ��һ��int����Э���
	/// </summary>
	private void OnRecieve()
    {
		 
		if (readingHeader) 
        {
			if (stream.Length == PACKAGE_HEADER_LENGTH) 
            {
				byte[] header = stream.ToArray ();
				
				totalSize = IPAddress.NetworkToHostOrder (System.BitConverter.ToInt32 (header, 0)) - 5;	
				cmd = IPAddress.NetworkToHostOrder (System.BitConverter.ToInt32 (header, 5));							 

				stream.Seek (0, SeekOrigin.Begin);
				stream.SetLength (0); 
				
				if (totalSize > 0) {
					readingHeader = false;
				} else {
					Process (cmd, stream.ToArray ());
				}
			}else if(stream.Length > PACKAGE_HEADER_LENGTH)
            {
				Debug.LogError("Network Stream Error When Reading Message Header");
			}
		} 
        else 
        {
			if(stream.Length == totalSize)
            {
				Process(cmd,stream.ToArray());
				readingHeader = true;				
				stream.Seek(0,SeekOrigin.Begin);
				stream.SetLength(0);
				totalSize = 0;
			}else if(stream.Length > totalSize)
            {
				Debug.LogError("Network Stream Error When Reading Message Body");
			}
		}		 
		
	} 

	public virtual bool IsConnected{
		get {return client!=null && client.Connected && IsSocketConnected(client.Client);}
	}
 
	
} 
