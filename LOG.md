# 更新记录

## 1.0.0 2019年10月28日12点11分
+ 初步完成

## 1.0.1 2019年10月29日23点17分

+ provider 添加 --wait 等待命令行输入字符串或地址
+ 修改 TransverterClient > TransSender
+ 修改 Clients 命名空间 为 Sender
+ 修改 Config添加CmdSerializer静态方法为实例 添加命令映射 组
+ 修改 Config删除构造函数，静态方法配置程序
+ 修改 Main 添加输入业务模型验证功能