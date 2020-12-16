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

        private void OnEnable()
        {
            SerializeButton = GameObject.Find("SerilizeRpc").GetComponent<Button>();
            SerializeButtonText = SerializeButton.transform.Find("Text").GetComponent<Text>();

            NotificationButton = GameObject.Find("NotificationRpc").GetComponent<Button>();
            NotificationButtonText = NotificationButton.transform.Find("Text").GetComponent<Text>();

            SerializedPlayername = GameObject.Find("playername").GetComponent<Text>();
            SerializedUid = GameObject.Find("uid").GetComponent<Text>();
            NotificationMsg = GameObject.Find("NoficationMsg").GetComponent<Text>();


            SerializeButtonText.text = "序列化";
            NotificationButtonText.text = "Notify";

            SerializedPlayername.color = Color.white;
            SerializedPlayername.text = "用户名";
            SerializedUid.color = Color.white;
            SerializedUid.text = "UID";
            NotificationMsg.color = Color.white;
            NotificationMsg.text = "Notify 消息";

            SerializeButton.onClick.AddListener(SerializeRpc);
            NotificationButton.onClick.AddListener(StartNotify);
        }

        void SerializeRpc()
        {
            RpcCallBackDelegate callback = new RpcCallBackDelegate(LoginRequist.ucl.SerializeRpcCallback);
            LoginRequist.ucl.rpcCall("user.serialize", null, callback);
        }

        void StartNotify()
        {
            LoginRequist.ucl.rpcCall("notifytester.rpc_start_notify", "3", null);
        }
    }
}
