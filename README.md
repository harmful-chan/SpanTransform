# SpanTransform

## ����
+ ΪSpan��Ŀ�ṩ����ת������

## ʵ��
+ �����ṩ������ɫ��transverter, provider, user
+ transverter ���������rivider�ṩ������IP��Ϣ��������transform.xml�ļ���
+ provider �����ṩ����IP����
+ user ��ȡ����IPa

## ʹ�÷�ʽ
+ provider
```
>st --role provider --operation update --domian www.span.com --adddress 113.112.185.220  
>...
>succeed [www.span.com 113.112.185.220 2019-10-21(12:44:52)]  [...]...    //�ɹ�
>failed [www.span.com 113.112.185.220 2019-10-21(12:44:52)]    //ʧ�ܷ��������¼�¼
>failed null    //����ʧ�ܣ���¼Ϊ��
```
+ transverter 
```
>st --role transverter --operation work/unwork
>...
```
+ user
```
>st --role user --operation get --domian www.span.com  
>st --role user --operation get --address 113.112.185.220 
>...
>succeed [www.span.com 113.112.185.220 2019-10-21(12:44:50)]  [...] ...
>failed null    //ʧ�ܣ��Ҳ�����¼
```

## transform.xml��ʽ
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