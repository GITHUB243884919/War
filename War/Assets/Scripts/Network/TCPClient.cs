
using System; 
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Assertions;

public enum NetworkStatus
{
	Idle,
	Error,
	Fail,
	Timeout,
	Connecting,
	Connected
}

public enum ClientType
{
	WORLD,
	GAME
}
 
public abstract class TCPClient : IDisposable
{ 
	
	protected const int PACKAGE_HEADER_LENGTH = 9;
	protected const int SEND_TIMEOUT          = 5000;
	protected const int RECV_TIMEOUT          = 5000;
	protected const int SEND_BUFF_SIZE        = 1024;

	//解析完stream后缓存的消息队列
	protected Queue<TCPMessage> m_messages    = new Queue<TCPMessage>();

    protected ClientType        m_type        = ClientType.WORLD;
	protected string            m_ip          = null;
	protected int               m_port        = -1;
	protected volatile   NetworkStatus status = NetworkStatus.Idle;
	
    //定义状态改变的回调。比如有时候网络状态改变会触发一些相关UI操作，然而UI的操作不写在这里
    //而是提供了回调接口
	public delegate void NetworkStatusEvent(TCPClient client, NetworkStatus status); 
	public event NetworkStatusEvent OnStatusChanged; 

	////[NoToLuaAttribute]
    //给外界定义的消息处理函数的回调
    public delegate void MessageEvent(ref TCPMessage msg); 

	protected TCPClient(ClientType type)
    {
		m_type = type;
	}

	public virtual void Dispose()
    { 
	}

	public virtual bool Init(string ip,int port)
    {
		if (Status != NetworkStatus.Idle)
        {
			Debug.LogWarningFormat("tcp client init failed when status is {0}",status);
			return false;
		} 

		m_ip   = ip;
		m_port = port;  

		return true;
	}

    /// <summary>
    /// 把状态设置为idle并根据参数决定是否要清理接收的缓存
    /// </summary>
    /// <param name="cleanup"></param>
	public virtual void Reset(bool cleanup = true)
    {
		Status = NetworkStatus.Idle;
		if (cleanup) 
        {
			lock (m_messages) 
            {
				m_messages.Clear();
			}
		}
	}
	
	public abstract bool Start();
	
	public abstract void Stop();

	////[NoToLuaAttribute]
	public abstract void Send(int cmd, byte[] data);

    /// <summary>
    /// Ulua接口的Send
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="data"></param>
    //public void Send(int cmd, LuaStringBuffer data)
    //{
    //    Send(cmd, data.buffer);
    //}

	////[NoToLuaAttribute]
    /// <summary>
    /// 给外界提供真正处理网络协议的接口，handler是处理消息的回调函数
    /// </summary>
    /// <param name="handler"></param>
	public void Tick(MessageEvent handler)
    { 
		TCPMessage msg;
		lock (m_messages)
        { 
			if(m_messages.Count > 0)
            {
				msg = m_messages.Dequeue();  
				handler(ref msg);  
			}
		}

		/*if (Status==NetworkStatus.Connected && !IsConnected) {
			NotifyStatusChange(NetworkStatus.Error);
			Status = NetworkStatus.Idle;
		}*/
	}

	//[NoToLuaAttribute]
    /// <summary>
    /// 缓存接收到网络消息到队列，这里实现最基本的，如果需要解密就需要重载
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="data"></param>
	protected void Process(int cmd, byte[] data)
    {
		#if UNITY_EDITOR
        Debug.LogFormat("recv {0} with len {1} at time {2}", cmd, data.Length, System.DateTime.Now);
		#endif
		TCPMessage msg = new TCPMessage ();
		msg.cmd  = cmd;
		msg.data = data;
		
		lock(m_messages)
        {
			m_messages.Enqueue(msg);
		}
	}
	
	protected void NotifyStatusChange(NetworkStatus status)
    {
		if (OnStatusChanged != null) 
        {
			OnStatusChanged(this, status);
		}
	} 
	
	public virtual NetworkStatus Status
    {
		get
        { 
			return status;
		}
		
        protected set 
        { 
			this.status = value; 
		}
	}

	public ClientType ClientType
    {
		get {return m_type;}
	}

	public virtual bool IsConnected
    {
		get {return Status == NetworkStatus.Connected;}
	}

    //protected static bool IsSocketConnected(Socket s)
    //{
    //    try{
    //        return s.Connected && 
    //            !(
    //                (
    //                    s.Poll(1000, SelectMode.SelectRead) && 
    //                    (s.Available == 0)
    //                ) || !s.Connected
    //             );  
    //    }catch(Exception e){
    //        Debug.LogException(e); 
    //    }
    //    return false;
    //}
} 
