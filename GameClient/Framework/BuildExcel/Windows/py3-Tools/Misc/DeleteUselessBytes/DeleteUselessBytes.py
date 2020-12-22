import io
import re
import os
import csv

CSHARP_FILE = 'csharp_list'
LUA_FILE = 'lua_used_config_list'

def GetCSharpUsedDataConfigList(resbindata_file_path):
    csharp_used_dataconfig_set = set()
    target = re.compile(r'\"dataconfig_(\S+)\"')
    
    with open(resbindata_file_path, 'r', encoding='utf-8') as resbindata:
        for line in resbindata.readlines():
            matches = target.findall(line)
            if len(matches) > 0:
                for match in matches:
                    if match not in csharp_used_dataconfig_set:
                        csharp_used_dataconfig_set.add(match)
    
    with open(CSHARP_FILE, 'w+') as csharp:
        for name in list(csharp_used_dataconfig_set):
            csharp.write('{}\n'.format(name))

GetCSharpUsedDataConfigList('D:/WorkSpace/best2/Code/trunk/Assets/CC_Scripts/LogicSystem/DataCenter/ResBinData.cs')


def GetDirFiles(dir_path):
    all_files = []

    for home, dirs, files in os.walk(dir_path):
        for filename in files:
            file_path = os.path.join(home, filename)
            file_path = file_path.replace("\\", '/')
            all_files.append(file_path)
        
        for subdir in dirs:
            all_files += GetDirFiles(os.path.join(home, subdir))

    return all_files

def GetToDelete(path, in_use_set, surfix):
    all_files = GetDirFiles(path)
    all_files = [x for x in all_files if x.split('.')[-1] == surfix]
    to_delete = []
    for file in all_files:
        config_name = file.split('/')[-1].split('.')[0].replace('dataconfig_', '')
        config_name = config_name.replace('_pb', '')
        if config_name not in in_use_set:
            to_delete.append(file)
    return to_delete

def DeleteAll(path_list):
    for path in path_list:
        try:
            os.remove(path)
            os.remove('{}.meta'.format(path))
        except:
            pass

def GetFilesInMD5Config(path):
    all_md5_files = set()
    with open(path, 'r') as csv_file:
        reader = csv.reader(csv_file)
        next(reader)

        for row in reader:
            config_name = row[0].split('\\')[-1].split('.')[0].lower().strip()
            all_md5_files.add(config_name)

    return all_md5_files

def GetAllBytes():
    path = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/Client/bytes'
    all_files = GetDirFiles(path)
    all_files = [x for x in all_files if x.split('.')[-1] == 'bytes']

    all_bytes = set()
    for path in all_files:
        config_name = path.split('/')[-1].split('.')[0].replace('dataconfig_', '')
        all_bytes.add(config_name)
    
    return all_bytes

def WriteToFile(filename, list):
    with open(filename, 'w+') as f:
        for config_name in list:
            f.write('{}\n'.format(config_name))

if __name__ == '__main__':
    in_use_set = set()
    with open(CSHARP_FILE, 'r') as csharp:
        for line in csharp.readlines():
            name = line.strip().lower()
            if len(name) > 0:
                in_use_set.add(name)

    with open(LUA_FILE, 'r') as lua_file:
        for line in lua_file.readlines():
            name = line.strip().lower()
            if len(name) > 0:
                name = name.replace('dataconfig_', '')
                in_use_set.add(name)

    path = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/Client/bytes'
    to_delete = GetToDelete(path, in_use_set, 'bytes')

    path = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/Client/cscripts'
    to_delete2 = GetToDelete(path, in_use_set, 'cs')

    path = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/Client/lua'
    to_delete3 = GetToDelete(path, in_use_set, 'lua')

    to_delete = to_delete3 + to_delete2 + to_delete

    md5_common = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/Md5_Common(cehua_China).csv'
    md5_yunying_china = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/MD5_yunying/MD5yunying_China.csv'
    md5_yunying_korea = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/MD5_yunying/MD5yunying_Korea.csv'
    md5_yunying_taiwan = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/MD5_yunying/MD5yunying_TaiWan.csv'
    md5_yunying_vietnam = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Data/MD5_yunying/MD5yunying_Vietnam.csv'
    

    set_md5 = GetFilesInMD5Config(md5_common).union(GetFilesInMD5Config(md5_yunying_china)). \
        union(GetFilesInMD5Config(md5_yunying_korea)).union(GetFilesInMD5Config(md5_yunying_taiwan)).union(GetFilesInMD5Config(md5_yunying_vietnam))

    to_delete_but_in_md5_config = []

    filtered_to_delete = []
    for file in to_delete:
        config_name = file.split('/')[-1].split('.')[0].replace('dataconfig_', '')
        config_name = config_name.replace('_pb', '')
        if config_name in set_md5:
            to_delete_but_in_md5_config.append(config_name)
        else:
            filtered_to_delete.append(file)

    WriteToFile('in_md5_config_but_not_in_use', to_delete_but_in_md5_config)

    DeleteAll(filtered_to_delete)

    WriteToFile("all_configs_in_md5", list(set_md5))

    all_bytes = GetAllBytes()
    totally_useless = []
    used_but_not_in_md5 = []
    for used in in_use_set:
        if used not in set_md5:
            if used not in all_bytes:
                totally_useless.append(used)
            else:
                used_but_not_in_md5.append(used)
    
    WriteToFile('used_but_not_in_md5', used_but_not_in_md5)
    WriteToFile('totally_useless', totally_useless)

    bytes_not_in_md5 = []
    for byte in all_bytes:
        if byte not in set_md5:
            bytes_not_in_md5.append(byte)
    WriteToFile("bytes_file_not_in_md5", bytes_not_in_md5)