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
    //注意此TcpClient是C#网络库的
	private TcpClient            m_client        = null;
	private NetworkStream        m_networkStream = null; 
	private System.Timers.Timer  m_timer         = new System.Timers.Timer(SEND_TIMEOUT);
    

    bool                         m_readingHeader = true;
    //网络消息协议编号
    int                          m_cmd           = 0;
    //包的长度
    int                          m_totalSize     = 0;
    const int                    BUF_SIZE        = 1024 * 10;
    byte[]                       m_buffer        = new byte[BUF_SIZE];
    MemoryStream                 m_memStream     = new MemoryStream();

	public TCPClientAsync(ClientType type):base(type)
    { 
        //超时timer初始化
        m_timer.AutoReset  = false;
        m_timer.Elapsed   += new System.Timers.ElapsedEventHandler(
            OnConnectTimeout);
	}

	public override void Dispose()
    { 
		Reset ();
        m_timer.Dispose();
	}

    /// <summary>
    /// Reset操作
    /// 1.调用base的Reset
    /// 2.networkStream关
    /// 3.内存stream的指针设置到0
    /// 4.client关
    /// </summary>
    /// <param name="cleanup"></param>
	public override void Reset(bool cleanup = true)
    {
		base.Reset (cleanup);
        m_timer.Enabled = false;

		if (m_networkStream != null) 
        {
            m_networkStream.Close();
            m_networkStream = null;
		}

        if (m_client != null && m_client.Connected) 
        {
            m_client.Close();
            m_client = null;
		}
		m_memStream.Seek (0, SeekOrigin.Begin);
	}
	 
	/// <summary>
	/// 启动网络连接
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

		Assert.IsNull (m_client);

        m_client = new TcpClient();
        m_client.ReceiveTimeout = RECV_TIMEOUT;
        m_client.SendTimeout    = SEND_TIMEOUT;
        m_client.SendBufferSize = SEND_BUFF_SIZE;

        m_client.NoDelay = true;

        if (!m_client.NoDelay) 
        {
            Debug.LogErrorFormat("{0} NoDelay {1}", type, m_client.NoDelay);
		}
        Assert.IsTrue(m_client.NoDelay);

		try
        {
			Status     = NetworkStatus.Connecting;
            var result = m_client.BeginConnect(ip, port, OnConnect, null);
			m_timer.Enabled = true; 
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
	/// 发送消息
	/// </summary>
	/// <param name="cmd"></param>
	/// <param name="data"></param>
    public override  void Send(int cmd,byte[] data)
    {
		if (m_networkStream == null) {
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
			m_networkStream.BeginWrite (bytes, 0, bytes.Length, OnWrite, null);  	
			#if UNITY_EDITOR
            Debug.LogFormat("send {0} with len {1} at time {2}", cmd, data.Length, System.DateTime.Now);
            #endif
		}catch(IOException exe){
			Debug.LogError(exe);
			NotifyStatusChange(NetworkStatus.Fail);
			Status = NetworkStatus.Idle;
		}

	}
	 
    /// <summary>
    /// 网络Connect成功后的回调
    /// 
    /// </summary>
    /// <param name="result"></param>
	private void OnConnect(IAsyncResult result)
    {		
		m_client.EndConnect(result);
		m_timer.Enabled = false; 
		m_timer.Stop ();
		if (m_client.Connected)
        {
			Status = NetworkStatus.Connected;
			m_networkStream = m_client.GetStream (); 
			
			m_readingHeader = true; 
			m_totalSize = 0;
			
			m_memStream.Seek (0, SeekOrigin.Begin);
            m_memStream.SetLength(0);
			m_networkStream.BeginRead (m_buffer, 0, PACKAGE_HEADER_LENGTH, OnRead, null); 
			
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

	private void OnWrite(IAsyncResult result)
    {
		//Debug.Log ("OnWrite");
		m_networkStream.EndWrite (result);
	}

    /// <summary>
    /// 网络读取回调
    /// 解析字节流调用了OnRecieve()函数
    /// </summary>
    /// <param name="result"></param>
	private void OnRead(IAsyncResult result)
    {
        try
        {
			int readBytes = m_networkStream.EndRead (result);
			
			//Debug.LogFormat ("OnRead {0} :{1} ",readingHeader?"Header":"Body",readBytes);
			
			if (readBytes > 0)
            {
				m_memStream.Write (m_buffer, 0, readBytes);  
				OnRecieve ();
				if (m_readingHeader)
                {
					//Debug.Log ("OnRead New Header");
					m_networkStream.BeginRead (m_buffer, 0, 
                        PACKAGE_HEADER_LENGTH - (int)m_memStream.Length, OnRead, null);
				}
                else if (m_totalSize > 0)
                {
					//Debug.LogFormat ("OnRead New Body {0} {1}",totalSize,stream.Length);
					m_networkStream.BeginRead (m_buffer, 0, 
                        Math.Min(m_totalSize - (int)m_memStream.Length, BUF_SIZE), OnRead, null);
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
    /// 网络stream解析，解析完以后要调用父类的Process（）函数放入队列中
    /// 头一个int代表长度
    /// 后一个int代表协议号
	/// </summary>
	private void OnRecieve()
    {
		 
		if (m_readingHeader) 
        {
			if (m_memStream.Length == PACKAGE_HEADER_LENGTH) 
            {
                byte[] header = m_memStream.ToArray();
				
				m_totalSize = IPAddress.NetworkToHostOrder (System.BitConverter.ToInt32 (header, 0)) - 5;	
				m_cmd = IPAddress.NetworkToHostOrder (System.BitConverter.ToInt32 (header, 5));							 

				m_memStream.Seek (0, SeekOrigin.Begin);
                m_memStream.SetLength(0); 
				
				if (m_totalSize > 0) 
                {
					m_readingHeader = false;
				} 
                else 
                {
					Process(m_cmd, m_memStream.ToArray ());
				}
			}
            else if(m_memStream.Length > PACKAGE_HEADER_LENGTH)
            {
				Debug.LogError("Network Stream Error When Reading Message Header");
			}
		} 
        else 
        {
			if(m_memStream.Length == m_totalSize)
            {
				Process(m_cmd, m_memStream.ToArray());
				m_readingHeader = true;				
				m_memStream.Seek(0, SeekOrigin.Begin);
                m_memStream.SetLength(0);
				m_totalSize = 0;
			}
            else if(m_memStream.Length > m_totalSize)
            {
				Debug.LogError("Network Stream Error When Reading Message Body");
			}
		}		 
		
	} 

    //public virtual bool IsConnected
    //{
    //    get 
    //    {
    //        bool result = m_client != null &&
    //            m_client.Connected &&
    //            IsSocketConnected(m_client.Client);
    //        return result;
    //    }
    //}
} 
