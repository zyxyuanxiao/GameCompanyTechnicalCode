用途：
已lua中的config_data_center.lua和C#代码中的ResBinData为参照，找出这两个文件中所有用到的dataconfig_XXX，认为是正在使用中的配置文件。
遍历客户端配置目录，所有不在“正在使用中”列表的文件全部删除。

对照“正在使用中”列表和MD5Common文件，找出所有在“正在使用中”列表但是不在MD5Common中的文件，认为是漏配置在MD5Common、