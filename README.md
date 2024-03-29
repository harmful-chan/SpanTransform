# SpanTransform

## 概述
+ 基于.NET Core 3.0
+ 为Span项目提供域名转换功能

## 实现
+ 程序提供三个角色：transverter, provider, user
+ transverter 负责接受收rivider提供得域名IP信息，保存至transform.xml文件中
+ provider 负责提供域名IP对照
+ user 获取服务IPa

## 使用方式
+ provider
```
st --role provider --operation update --domian www.span.com --adddress 113.112.185.220    //完整记录输入
st --role provider --operation update --domian www.span.com --address --wait    //等待输入address
st --role provider --operation update --domian  --address 113.112.185.220 --wait    //等待输入domian
>...
[ Output ] succeed    //写入成功返回记录
[www.span.com 113.112.185.220 2019-10-21(12:44:52)]  
[...]    
[ Output ] failed    //失败返回记录记录
[www.span.com 113.112.185.220 2019-10-21(12:44:52)]    
[...]
```
+ transverter 
```
st --role transverter --operation unwork    //关闭正在运行的st
st --role transverter --operation work --address x.x.x.x     //指定监控x.x.x.x 8898端口
st --role transverter --operation work --address --wait    //等待输入address ,监控该IP 8898端口
>...

```
+ user
```
st --role user --operation get --domian www.span.com  
st --role user --operation get --address 113.112.185.220 
...
[ Output ]succeed 
[www.span.com 113.112.185.220 2019-10-21(12:44:50)]  
[...] 
[ Output ] failed    //失败，找不到记录
```

## transform.xml格式
```
<?xml version='1.0' encoding='UTF-8'?>
<span>
	<mainframe domian="www.span.com" date="2019-10-17(13:40:50)">
		<record address="113.112.208.208" date="2019-10-17(13:40:50)"/>
		<record address="113.112.208.208" date="2019-10-17(13:40:50)"/>
		...
	</mainframe>
<span>
```

## 其他
+ runtime 下载地址：
+ [.net core runtime for window x64](https://download.visualstudio.microsoft.com/download/pr/f15b7c04-2900-4a14-9c01-ccd66a4323cc/17a6bbd44f0d0a85d219dd9e166a89ca/dotnet-runtime-3.0.0-win-x64.zip)
+ [.net core runtime for linux x64](https://download.visualstudio.microsoft.com/download/pr/a5ff9cbb-d558-49d1-9fd2-410cb1c8b095/a940644f4133b81446cb3733a620983a/dotnet-runtime-3.0.0-linux-x64.tar.gz)
