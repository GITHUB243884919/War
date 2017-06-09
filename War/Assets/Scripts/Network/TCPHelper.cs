using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

public class TCPHelper
{
    private static Dictionary<int, PackageFactory> factories = new Dictionary<int, PackageFactory>();

    public static void RegisterFactory(int cmd, PackageFactory factory)
    {
        factories[cmd] = factory;
    }

    private static TCPClient worldClient = new TCPClientAsync(ClientType.WORLD);
    private static TCPClient gameClient = new TCPClientAsync(ClientType.GAME);
    private static List<int> cmdCache = new List<int>() ;
    private static List<int> repeatableCmds = new List<int>() { 11000, 100001, 90001};
    public static bool InitWorldClient(string ip, int port)
    {
        return worldClient.Init(ip, port);
    }

    public static bool InitGameClient(string ip, int port)
    {
        return gameClient.Init(ip, port);
    }

    public static void OnDestroy()
    {
        gameClient.Stop();
        gameClient.Dispose();

        worldClient.Stop();
        worldClient.Dispose();

    }

    [NoToLuaAttribute]
    public static void Tick()
    {
        if (worldClient != null)
        {
            worldClient.Tick(OnMessage);
        }
        if (gameClient != null)
        {
            gameClient.Tick(OnMessage);
        }
    }

    public static void RemoveFromCmdCache(int cmd)
    {
        if (cmdCache.Contains(cmd))
        {
            cmdCache.Remove(cmd);
        }
    }
    private static void OnMessage(ref TCPMessage msg)
    {
        try
        {
            RemoveFromCmdCache(msg.cmd - 1);
            PackageFactory factory = null;
            if (factories.TryGetValue(msg.cmd, out factory))
            {
                factory.OnMessage(msg.cmd, msg.data);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    public static TCPClient WorldClient
    {
        get { return worldClient; }
    }

    public static TCPClient GameClient
    {
        get { return gameClient; }
    }
    public static void AddRepeatableCmd(int cmd)
    {
        if (!repeatableCmds.Contains(cmd))
        {
            repeatableCmds.Add(cmd);
        }
    }

    public static bool IsCmdRepeatable(int cmd)
    {
        return repeatableCmds.Contains(cmd);
    }

    public static bool IsCmdCacheEmpty()
    {
        return cmdCache.Count == 0;
    }
    private static bool IsCmdSendable(int cmd)
    {
        if (cmdCache.Contains(cmd))
        {
            if (IsCmdRepeatable(cmd))
            {
                return true;
            }
            else
            {
                Debug.LogFormat("cmd {0} hasn't return",cmd);
                return false;
            }
        }
        else
        {
            cmdCache.Add(cmd);
            return true;
        }
    }

    public static void ClearCmdCache()
    {
        cmdCache.Clear();
    }
    [NoToLuaAttribute]
    public static void SendToWorldServer(int cmd, object msg)
    {
        try
        {
            SendToServer(worldClient, cmd, msg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [NoToLuaAttribute]
    public static void SendToGameServer(int cmd, object msg)
    {
        try
        {
            SendToServer(gameClient, cmd, msg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private static void SendToServer(TCPClient client, int cmd, object msg)
    {
        if (IsCmdSendable(cmd))
        {
            System.Type type = msg.GetType();
            PropertyInfo property = type.GetProperty("Cmd");
            property.SetValue(msg, cmd, null);

            MethodInfo method = type.GetMethod("SerializeToBytes");
            byte[] result = method.Invoke(null, new object[] { msg }) as byte[];
            client.Send(cmd, result);
        }
    }

    public static void SendToWorldServer(int cmd, LuaStringBuffer msg)
    {
        if (IsCmdSendable(cmd))
        {
            try
            {
                worldClient.Send(cmd, msg.buffer);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public static void SendToGameServer(int cmd, LuaStringBuffer msg)
    {
        if (IsCmdSendable(cmd))
        {
            try
            {
                gameClient.Send(cmd, msg.buffer);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
