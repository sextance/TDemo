# Tiny RPC

本项目实现了简易的RPC流程，能够实现客户端向服务端发送RPC请求并为该请求注册一个可选择的回调和参数列表。本项目已经为一些常用的RPC请求编写了一些实用的函数以供调用，减少开发的工作量。

## 客户端向服务端发RPC基本流程

### 发送RPC请求的入口函数

```csharp
public void rpcCall(string funcname, string parameters = null, RpcCallBackDelegate callback = null)
{
    ...
}
```

- `funcname`是RPC调用的服务端函数的名称
- `parameters`参数是传递给服务端函数的参数的名称，这个参数是通过msgpack打包生成的，可以包含任意数目的参数列表
- `callback`是为RPC请求注册的本地回调函数

```csharp
public delegate void RpcCallBackDelegate(byte[] data);
```

- `data`是protobuf打包生成的字节流，RPC请求的回调函数通过protobuf打包的信息来进行相应的处理

### 客户端和服务端数据的打包方式

protobuf编译的数据包的C#格式代码在`./Assets/Scripts/Network/Message.cs`中，它包含以下数据成员

```protobuf
package NinjaMessage;


//操作码，表示请求意图
enum OPCODE
{
    NONE                = 0; // 空请求只用于网络检测
    RPC_CALL            = 1; // 通用RPC
    NOTIFY_INFO         = 2; // 服务端notify
}


//通知码
enum NOTIFY_CODE
{
    NOTIFY_KICK_OFFLINE = 1; // 被踢下线通知
}


//标记码
enum FLAG_CODE
{
    TESTCODE            = 1;
}


//消息包
message Message
{
    required OPCODE opCode      = 1;
    required int32 uid          = 2;
    optional Request request    = 3;
    optional Response response  = 4;
    optional int32 timeNow      = 5; // 服务器时间
    optional string device      = 6; // 设备信息串
    optional NotifyInfo notifyInfo = 7;
}


//请求消息
message Request
{
    optional string rpcFunc     = 1;
    optional string rpcParams   = 2;
}


//返回消息
message Response
{
    required int32 errorCode    = 1;
    optional string rpcFunc     = 2;
    optional string rpcRsp      = 3;
}


message NotifyInfo
{
    required int64 sequence    = 1;
    optional string rpcFunc  = 2;
    optional string rpcParams   = 3;
}

```

打包的流程如下：

![img](./img/客户端打包流程.png)

## 服务端发送Notify流程

### 发送Notify的入口函数

```lua
function SERVER_CALL.notify_to_client(rpcFunc, ...)
```

- `rpcFunc`是服务端调用的客户端函数
- `...`是发送给客户端函数的参数列表，如果有多种类型的值，在C#中可以通过object list来解决

### 客户端的Notify测试调用方法

```C#
LoginRequist.ucl.rpcCall("notifytester.rpc_start_notify", "3", null);
```

- 第一个参数是服务端的notify启动函数
- 第二个参数是发送notify的次数
- 第三个参数是启动notify的本地回调，可以置空，也可以设置为任意编写好的客户端回调函数，用于处理启动Notify的回调处理

