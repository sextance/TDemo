using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BaseFramework.Network
{
    public class GameNodeRpc : MonoBehaviour
    {
        Button SerializeButton;
        Text SerializeButtonText;
        Button NotificationButton;
        Text NotificationButtonText;
        internal static Text SerializedPlayername;
        internal static Text SerializedUid;
        internal static Text NotificationMsg;
        static bool matchFlag;

        private void OnEnable()
        {
            //SerializeButton = GameObject.Find("SerilizeRpc").GetComponent<Button>();
            //SerializeButtonText = SerializeButton.transform.Find("Text").GetComponent<Text>();
            matchFlag = false;
            NotificationButton = GameObject.Find("PVPButton").GetComponent<Button>();
            NotificationButtonText = GameObject.Find("PVPTextLabel").GetComponent<Text>();
            //NotificationButtonText = NotificationButton.transform.Find("Text").GetComponent<Text>();

           // SerializedPlayername = GameObject.Find("playername").GetComponent<Text>();
           // SerializedUid = GameObject.Find("uid").GetComponent<Text>();
            //NotificationMsg = GameObject.Find("NoficationMsg").GetComponent<Text>();


            //SerializeButtonText.text = "序列化";
            //NotificationButtonText.text = "Notify";

            /*SerializedPlayername.color = Color.white;
            SerializedPlayername.text = "用户名";
            SerializedUid.color = Color.white;
            SerializedUid.text = "UID";
            NotificationMsg.color = Color.white;
            NotificationMsg.text = "Notify 消息";*/

            //SerializeButton.onClick.AddListener(SerializeRpc);
            NotificationButton.onClick.AddListener(StartNotify);
        }

        void SerializeRpc()
        {
            RpcCallBackDelegate callback = new RpcCallBackDelegate(LoginRequist.ucl.SerializeRpcCallback);
            LoginRequist.ucl.rpcCall("user.serialize", null, callback);
        }

        void StartNotify()
        {   
            if(!matchFlag)
            {
                NotificationButtonText.text = "取消匹配";
                matchFlag = !matchFlag;
                LoginRequist.ucl.rpcCall("combat.start_match", null, (byte[] data) =>
                {   

                    var msg = BaseFramework.Network.UserClient.ProtobufDecoder(data);

                    if (msg.Response.RpcRsp != null)
                    {
                        var result = UserClient.MessagePackDecoder<object>(msg.Response.RpcRsp);

                        DebugLogger.Debug("start_match callback: " + result);
                        //NotificationMsg.text = result.ToString();
                    }
                });
            }
            else
            {
                NotificationButtonText.text = "新的匹配";
                matchFlag = !matchFlag;
                LoginRequist.ucl.rpcCall("combat.cancel_match", null, (byte[] data) =>
                {
                    var msg = UserClient.ProtobufDecoder(data);

                    if (msg.Response.RpcRsp != null)
                    {
                        object result = UserClient.MessagePackDecoder<object>(msg.Response.RpcRsp);
                        //TODO:UI按钮改回“新的匹配”
                        DebugLogger.Debug("cancel_match callback: " + result);
                        //NotificationMsg.text = result.ToString();
                    }
                });
            }     
        }
    }
}
