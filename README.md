# SpanTransform

## 概述
+ 为Span项目提供域名转换功能
+ >stra ftp.span.com 
+ >113.112.109.223

## 实现
+ 程序提供三个角色：transverter, provider, user
+ transverter 负责接受收rivider提供得域名IP信息，保存至transform.xml文件中
+ provider 负责提供域名IP对照
+ user 获取服务IP

## 使用方式
+ provider
```
>st --role provider --domian www.span.com --adddress 113.112.185.220 --operation update 
>succeed www.span.com 113.112.185.220 2019-10-21(12:44:52)    //成功
>failed www.span.com 113.112.185.220 2019-10-21(12:44:52)    //失败返回最后更新记录
>failed null    //更新失败，记录为空
```
+ transverter 
```
>st --role transverter --operation start/reboot/stop
>succeed transverter  start/reboot/stop.
>failed transverter start/reboot/stop, because:...
```
+ user
```
>st --role user --domian www.span.com  --operation get
//>st --role user --address 113.112.185.220  --operation get
>succeed www.span.com 113.112.185.220 2019-10-21(12:44:50)
>failed null    //失败，找不到记录
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