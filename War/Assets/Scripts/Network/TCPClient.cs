
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

	//������stream�󻺴����Ϣ����
	protected Queue<TCPMessage> m_messages    = new Queue<TCPMessage>();

    protected ClientType        m_type        = ClientType.WORLD;
	protected string            m_ip          = null;
	protected int               m_port        = -1;
	protected volatile   NetworkStatus status = NetworkStatus.Idle;
	
    //����״̬�ı�Ļص���������ʱ������״̬�ı�ᴥ��һЩ���UI������Ȼ��UI�Ĳ�����д������
    //�����ṩ�˻ص��ӿ�
	public delegate void NetworkStatusEvent(TCPClient client, NetworkStatus status); 
	public event NetworkStatusEvent OnStatusChanged; 

	////[NoToLuaAttribute]
    //����綨�����Ϣ�������Ļص�
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
    /// ��״̬����Ϊidle�����ݲ��������Ƿ�Ҫ������յĻ���
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
    /// Ulua�ӿڵ�Send
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="data"></param>
    //public void Send(int cmd, LuaStringBuffer data)
    //{
    //    Send(cmd, data.buffer);
    //}

	////[NoToLuaAttribute]
    /// <summary>
    /// ������ṩ������������Э��Ľӿڣ�handler�Ǵ�����Ϣ�Ļص�����
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
    /// ������յ�������Ϣ�����У�����ʵ��������ģ������Ҫ���ܾ���Ҫ����
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
