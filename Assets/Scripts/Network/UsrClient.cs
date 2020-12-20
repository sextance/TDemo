using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using NinjaMessage;
using System.Text;
using MsgPack.Serialization;
using Google.Protobuf;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;


namespace BaseFramework.Network
{

    public delegate void RpcCallBackDelegate(byte[] data);

    internal class SerializeDB
    {
        public string playerName { get; set; }
        public string uid { get; set; }
    }
    internal class UserClient : MonoBehaviour
    {
        private bool clientRun = false;
        private IProtocol netProtocol;
        private Login usrLogin;
        private NetClient netClient;
        private int index;
        private Timer heartTimer;
        private Timer timeoutTimer;
        private int heartCount = 0;
        private Int64 netTick = 0;
        private AutoResetEvent sendEvent = new AutoResetEvent(false);
        private SafeQueue<byte[]> sendQueue = new SafeQueue<byte[]>();
        private SafeQueue<byte[]> reciveQueue = new SafeQueue<byte[]>();
        private Dictionary<uint, DataPakage> sessionToCallback = new Dictionary<uint, DataPakage>();
        internal Thread SendThread;
        internal Thread ReceiveThread;
        private Dictionary<string, Action<Message>> severMonitorCallback = new Dictionary<string, Action<Message>>();

        public UserClient(Login ulogin, int idx, NetClient ncli)
        {
            netClient = ncli;
            index = idx;
            usrLogin = ulogin;
            // 超时检测定时器
            timeoutTimer = new Timer(new TimerCallback(timeoutCheck), null, Timeout.Infinite, Timeout.Infinite);
            regServerMonitor("isMatchSuccess", Match); 
            regServerMonitor("other_player_coming", other_player_coming);
            regServerMonitor("powerShow", powerShow);
        }

        private void powerShow(Message msg)
        {
            if (msg.OpCode == OPCODE.NotifyInfo)
            {
                var seq = msg.NotifyInfo.Sequence;
                if (seq > 0)
                {

                    object retParam = MessagePackDecoder<object>(msg.NotifyInfo.RpcParams);
                    TowerChange tc = JsonConvert.DeserializeObject<TowerChange>(retParam.ToString());
                    
                    switch (tc.OptType)
                    {
                        case OptionType.UPDATE_TOWER:
                            if(tc.towernum >= 0)
                            {
                                GameManager.gm.setEnemyTowerCount(tc.towernum);
                            }//TODO:与UI势力条绑定
                            break;
                        case OptionType.TOWER_CHANGE:


                            if (tc.ti != null)
                            {
                                Vector3 createEnemyPosition = new Vector3();
                                createEnemyPosition.x = tc.ti.x;
                                createEnemyPosition.y = tc.ti.y;
                                createEnemyPosition.z = tc.ti.z;
                                var a = new GameObject();
                                a.AddComponent<MonsterMake>().SetData(tc.ti.shapeid, createEnemyPosition,OptionType.TOWER_CHANGE);
                            }
                            break;
                        case OptionType.DESTORY_TOWER:
                            {
                                if(tc.ti != null)
                                {
                                    Vector3 CreateEnemyPosition = new Vector3();
                                    CreateEnemyPosition.x = tc.ti.x;
                                    CreateEnemyPosition.y = tc.ti.y;
                                    CreateEnemyPosition.z = tc.ti.z;
                                    var a = new GameObject();
                                    a.AddComponent<MonsterMake>().SetData(tc.ti.shapeid, CreateEnemyPosition, OptionType.DESTORY_TOWER);
                                }
                            }
                            break;
                        case OptionType.SCAN:
                            TowerChange st = new TowerChange();
                            foreach (TowerShape towerShape in GameManager.gm.towerShapes)
                            {
                                TowerInfo temp = new TowerInfo(towerShape.ShapeId, towerShape.transform.localPosition.x, towerShape.transform.localPosition.y, towerShape.transform.localPosition.z);
                                st.a.Add(temp);
                            }
                            st.OptType = OptionType.SCAN_MAKE;
                            rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(st), null);
                            break;
                        case OptionType.SCAN_MAKE://fetch行为
                            if(tc.a.Count > 0)
                            {
                                foreach(TowerInfo tif in tc.a)
                                {
                                    Vector3 v = new Vector3();
                                    v.x = tif.x;
                                    v.y = tif.y;
                                    v.z = tif.z;
                                    v.x += 1200f;
                                    TowerShape ts = Instantiate(TowerShapeFactory.tsf.prefabs[tif.shapeid],v,Quaternion.identity);
                                }
                            }
                            break;
                        case OptionType.GAME_OVER:
                            if(tc.result == "false")
                            {
                                Debug.Log("Game Over");
                                TowerChange uw = new TowerChange();
                                uw.result = "true";
                                uw.OptType = OptionType.GAME_OVER;
                                rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(uw), null);
                                SceneManager.LoadScene("EndScene");
                            }
                            else if(tc.result == "true")
                            {
                                Debug.Log("You Win");
                                SceneManager.LoadScene("EndScene");
                            }
                            break;
                    }
                    

                }

            }
        }


        private void other_player_coming(Message msg)
        {
            if (msg.OpCode == OPCODE.NotifyInfo)
            {
                var seq = msg.NotifyInfo.Sequence;
                if (seq > 0)
                {

                    object retParam = MessagePackDecoder<object>(msg.NotifyInfo.RpcParams);

                    DebugLogger.Debug(retParam.ToString());

                    Debug.Log("server callback");
                    // clientReceiveSeq = seq;
                }

            }
        }

        private void Match(Message msg)
        {
            if(msg.NotifyInfo.RpcParams != "0")
            {
                var seq = msg.NotifyInfo.Sequence;
                if(seq > 0)
                {
                    Boolean retParam = MessagePackDecoder<Boolean>(msg.NotifyInfo.RpcParams);
                    if (retParam)
                    {
                        DebugLogger.Debug("匹配成功");
                        /*LoginRequist.ucl.rpcCall("combat.getOtherId", null, (byte[] data) =>
                        {
                            var massage = BaseFramework.Network.UserClient.ProtobufDecoder(data);

                            if (massage.Response.RpcRsp != null)
                            {
                                var result = UserClient.MessagePackDecoder<object>(massage.Response.RpcRsp);

                                DebugLogger.Debug("OtherID callback: " + result);
                            }
                        });*/

                        SceneManager.LoadScene("TowerScene");
                    }
                    else
                    {
                        DebugLogger.Debug("匹配失败");
                    }

                    Debug.Log("server callback:isMatchSuccess");
                    // clientReceiveSeq = seq;
                }
            }
        }

        public void regServerMonitor(string funName, Action<Message> ac)
        {
            severMonitorCallback[funName] = ac;
        }



        // 对发送给服务端的数据进行打包
        public void rpcCall(string funcname, string parameters = null, RpcCallBackDelegate callback = null)
        {
            // generate protobuf msg
            var tmp = new NinjaMessage.Message
            {
                OpCode = NinjaMessage.OPCODE.RpcCall,
                Uid = int.Parse(netProtocol.Node.uid)
            };
            tmp.Request = new NinjaMessage.Request();
            tmp.Request.RpcFunc = funcname;

            // use msgpack to pack the parameters
            if(parameters != null)
            {
                string paramString = parameters;
                var serializer = MessagePackSerializer.Get<string>();
                using (var byteStream = new MemoryStream())
                {
                    serializer.Pack(byteStream, paramString);
                    byte[] tmpBytes = new byte[byteStream.Length];
                    int iter = 0;
                    byteStream.Seek(0, SeekOrigin.Begin);
                    StringBuilder myStringBuilder = new StringBuilder();
                    while (iter < tmpBytes.Length)
                    {
                        byte numIter = (byte)(byteStream.ReadByte());
                        char tempChar = '\u0000';
                        tempChar += (char)numIter;
                        myStringBuilder.Append(tempChar);
                        tmpBytes[iter++] = numIter;
                    }
                    tmp.Request.RpcParams = Convert.ToBase64String(tmpBytes);
                }
            }
            
            // use protobuf to encode message
            byte[] protoMsg;
            using (var byteStream = new MemoryStream())
            {
                tmp.WriteTo(byteStream);
                protoMsg = new byte[byteStream.Length];
                int iter = 0;
                byteStream.Seek(0, SeekOrigin.Begin);
                while (iter < protoMsg.Length)
                {
                    protoMsg[iter++] = (byte)(byteStream.ReadByte());
                }
            }

            // DebugLogger.Debug(tmp.Request.RpcParams);
            SendMessage(protoMsg, "RPC_CALL", callback);
        }

        private void RpcNotify(byte[] data)
        {
            var msg = ProtobufDecoder(data);

            DebugLogger.Debug(msg.OpCode.ToString());//NotifyInfo

            if (msg.OpCode == OPCODE.NotifyInfo)
            {
                var seq = msg.NotifyInfo.Sequence;
                if (seq > 0)
                {
                    DebugLogger.Debug(seq.ToString());//1
                    //GameNodeRpc.NotificationMsg.text = seq.ToString() + "\n";
                    // clientReceiveSeq = seq;
                }

                var rpcFunc = msg.NotifyInfo.RpcFunc;
                Debug.Log(rpcFunc.ToString());//isMatchSuccess
                if (severMonitorCallback.ContainsKey(rpcFunc.ToString()))
                {
                    severMonitorCallback[rpcFunc.ToString()](msg);//执行注册的函数isMatchSuccess
                }
                if (rpcFunc == null)
                {
                    DebugLogger.DebugError("RpcNotify wrong fucntion code");
                }
                else
                {
                    //object retParam = MessagePackDecoder<object>(msg.NotifyInfo.RpcParams);
                    //int i = 0;
                    //GameNodeRpc.NotificationMsg.text += retParam.ToString();
                    //Debug.Log(retParam.ToString());
                }
            }
        }

        public static Message ProtobufDecoder(byte[] data)
        {
            Message msg = new Message();
            return Message.Parser.ParseFrom(data);
        }

        public static T MessagePackDecoder<T>(string param)
        {
            var msgPackDecoder = MessagePackSerializer.Get<T>();
            byte[] byteArray = Convert.FromBase64String(param);
            T RspMsg;
            using (var rpcRspStream = new MemoryStream(byteArray))
            {
                RspMsg = msgPackDecoder.Unpack(rpcRspStream);
            }
            return RspMsg;
        }

        internal void SerializeRpcCallback(byte[] data)
        {
            var msg = ProtobufDecoder(data);

            if (msg.Response.RpcRsp != null)
            {
                var RspMsg = MessagePackDecoder<object>(msg.Response.RpcRsp);
                DebugLogger.Debug(RspMsg.ToString());

                SerializeDB serializedMsg;
                serializedMsg = JsonConvert.DeserializeAnonymousType<SerializeDB>(RspMsg.ToString(), new SerializeDB());
                GameNodeRpc.SerializedPlayername.text = serializedMsg.playerName;
                GameNodeRpc.SerializedUid.text = serializedMsg.uid;
            }
        }


        internal void LoginRpcCallback(byte[] data)
        {
            var msg = ProtobufDecoder(data);

            if(msg.Response.RpcRsp != null)
            {
                var needCreate = MessagePackDecoder<long>(msg.Response.RpcRsp);
                DebugLogger.Debug(needCreate.ToString());
                if(needCreate == 1)
                {
                    RpcCallBackDelegate createPlayerCallback = new RpcCallBackDelegate(userCreateCallback);
                    rpcCall("user.create", "player_" + netProtocol.Node.uid, createPlayerCallback);
                }
            }

        }

        private void userCreateCallback(byte[] data)
        {
            var msg = ProtobufDecoder(data);

            if (msg.Response.RpcRsp != null)
            {
                var createdUserInfo = MessagePackDecoder<object>(msg.Response.RpcRsp);

                SerializeDB serializedMsg;
                serializedMsg = JsonConvert.DeserializeAnonymousType<SerializeDB>(createdUserInfo.ToString(), new SerializeDB());

                DebugLogger.Debug("Created game agent for player" + serializedMsg.playerName);
            }
        }

        // 接受服务端发送的消息 调用相关的回调函数
        public void MyUpdate()
        {
            while (reciveQueue.Count > 0)
            {
                var msg = reciveQueue.Dequeue();
                uint session;
                var recData = DataPack.UnPack(msg, out session);
                if (recData.Length <= 0)
                {
                    // 心跳包，重置心跳计数
                    heartCount = 0;
                    continue;
                }
                if (session == 0)
                {
                    RpcNotify(recData);
                }
                else if (sessionToCallback.ContainsKey(session))
                {
                    var callback = sessionToCallback[session].CallBackFunc;
                    if (null != callback)
                    {
                        callback(recData);
                    }
                    // 加锁删除数据
                    lock (sessionToCallback)
                    {
                        sessionToCallback.Remove(session);
                        netProtocol.WaitRecive--;
                    }
                }
                else
                {
                    DebugLogger.DebugError("Session invalid:" + session);
                }
            }

        }

        private void timeoutCheck(object o)
        {
            if (netProtocol.WaitRecive > 20)
            {
                List<uint> delList = new List<uint>();
                var sessions = sessionToCallback.Keys;
                foreach (uint sess in sessions)
                {
                    if (sessionToCallback[sess].CheckExpire(netTick))
                    {
                        delList.Add(sess);
                    }
                }
                lock (sessionToCallback)
                {
                    foreach (uint sess in delList)
                    {
                        sessionToCallback.Remove(sess);
                    }
                }
            }
            timeoutTimer.Change(30000, Timeout.Infinite);
        }

        internal bool ChkException(Exception e)
        {
            if (e == null)
            {
                return false;
            }
            int err;
            if (e is SocketException)
            {
                err = (int)ErrorCode.SocketError;
            }
            else if (e is NetworkingException)
            {
                err = ((NetworkingException)e).code;
            }
            else
            {
                err = (int)ErrorCode.DataError;
            }
            DebugLogger.DebugNetworkError("ConnectLogin Error " + err + " " + e.GetType() + e.Message + e.StackTrace);
            return true;
        }

        public bool Login(string ip, int port, string secret)
        {
            // 连接服务器
            if (ChkException(usrLogin(ip, port, secret, out netProtocol)))
            {
                OnError("ConnectLogin");
                rpcCall("user.get_login_info", null, null);
                return false;
            }
            else
            {
                // 登陆成功
                DebugLogger.Debug("Login success");
            }
            RpcCallBackDelegate callback = new RpcCallBackDelegate(LoginRpcCallback);
            rpcCall("user.get_login_info", null, callback);

            // 启动发送和接收
            StartNetWorkService();
            return true;
        }

        internal void StartNetWorkService()
        {
            StartSendAndReceive();
            // 启动心跳
            startHeart(null);
            // 启动超时包检测
            timeoutTimer.Change(30000, Timeout.Infinite);
        }


        public string GetUserID()
        {
            return netProtocol.Node.uid;
        }

        // 向服务器发信并注册回调函数
        public void SendMessage(byte[] message, string opCode, RpcCallBackDelegate callback)
        {
            uint session = 0;
            if (callback != null)
            {
                session = NetUtil.GetSessionID();

                lock (sessionToCallback)
                {
                    sessionToCallback[session] = new DataPakage { CallBackFunc = callback, Session = session, cTick = netTick };
                    netProtocol.WaitRecive++;
                }
            }
            var data = DataPack.Pack(session, opCode, message);
            sendQueue.Enqueue(data);
            sendEvent.Set();
        }

        private void onRecive(byte[] msg)
        {
            reciveQueue.Enqueue(msg);
            if (heartTimer != null)
            {
                heartTimer.Change(5000, Timeout.Infinite);
            }
        }

        private void startHeart(object o)
        {
            heartTimer = new Timer(new TimerCallback(heartBeat), o, 3000, Timeout.Infinite);
        }


        private void heartBeat(object o)
        {
            if (heartCount > 3)
            {
                heartCount = 0;
                OnError("heart beat timeout");
                var e = netProtocol.OutException(NetException.HeartBeatTimeOut);
                if (e != null)
                {
                    throw e;
                }
            }
            else
            {
                sendQueue.Enqueue(new byte[3] { 0, 0x01, 0 });
                sendEvent.Set();
                heartCount++;
            }
            heartTimer.Change(3000, Timeout.Infinite);
        }

        // 开始接收和发送线程
        private void StartSendAndReceive()
        {
            clientRun = true;
            SendThread = new Thread(startSend);
            SendThread.Start();
            ReceiveThread = new Thread(startRecive);
            ReceiveThread.Start();
        }

        private void startSend()
        {
            Exception e = null;
            while (clientRun)
            {
                var msg = sendQueue.Dequeue();
                if (msg == null)
                {
                    sendEvent.WaitOne();
                    continue;
                }
                e = netProtocol.Send(msg);
                if (e != null)
                {
                    OnError(e.ToString());
                    if (clientRun)
                    {
                        throw e;
                    }
                }
            }
        }
        // 接收逻辑
        private void startRecive()
        {
            Exception e = null;
            while (clientRun)
            {
                try
                {
                    var msg = netProtocol.Read();
                    onRecive(msg);
                }
                catch (Exception ce)
                {
                    // 此处捕获的异常为不可恢复异常，直接退回重登
                    OnError(ce.ToString());
                    if (clientRun)
                    {
                        e = ce;
                        throw ce;
                    }
                }
            }
        }

        public void OnError(string err)
        {
            DebugLogger.DebugNetworkError("NetClient OnError:" + err);
            lock (sessionToCallback)
            {
                sessionToCallback.Clear();
                netProtocol.WaitRecive = 0;
            }
        }

        // 主动关闭连接
        public void Close()
        {
            netClient.Close(index);
        }

        public void CloseSelf()
        {
            Dispose();
        }

        public void Dispose()
        {
            clientRun = false;
            netProtocol.Close();
            heartTimer.Dispose();
            timeoutTimer.Dispose();

            lock (sessionToCallback)
            {
                sessionToCallback.Clear();
                netProtocol.WaitRecive = 0;
            }
        }
    }
}

