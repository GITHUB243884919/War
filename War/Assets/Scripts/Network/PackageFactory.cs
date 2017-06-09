using System; 
using UnityEngine;
using Dynasty;
using LuaInterface;

public abstract class PackageFactory
{

	protected PackageFactory ()
	{
	}

	[NoToLuaAttribute]
	public abstract void  OnMessage(int cmd,byte[] data);
 
} 

public class ErrorPackageFactory: PackageFactory{
	public override void  OnMessage(int cmd,byte[] data){
		ResponseHead msg = ResponseHead.Deserialize (data);  
		Debug.LogErrorFormat ("Network Error {0} with {1}",msg.RequestCmd,msg.Err);
		//MessageTips.Show (msg.Err);
	}
}
	

public class LuaPackageFactory:PackageFactory{

	public delegate void MessageHandler(int cmd,LuaStringBuffer data);

	public MessageHandler handler;

	[NoToLuaAttribute]
	public override void  OnMessage(int cmd,byte[] data){
		if (handler != null) {
			handler(cmd,new LuaStringBuffer(data));
		}
	}

	public LuaPackageFactory(MessageHandler handler){
		this.handler = handler;
	}
}
