#encoding : utf-8

'''
使用方法: 在Tool目录下运行 python -m Tool.Misc.delete_wrong_md5_file
文件里写死了几个路径，自己改下就好
'''
import sys
import os
path = os.getcwd() + '\Tool\3rd'
sys.path.append(path)
path = os.getcwd() + '\Tool'
sys.path.append(path)

import io
import re
import csv
import EntryPoint

from ExcelParser.Env import ConfigMgr, ParsingEnv
from Misc.Util import *

P = r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data/'
def CheckMD5(md5_path):
    with open(md5_path, newline='', encoding='UTF-8-sig') as md5_file:
        csv_reader = csv.reader(md5_file)

        all_rows = []
        count = 0
        for row in csv_reader:
            all_rows.append(row)
            count += 1
            if count == 1:
                continue
            cs_flag, lua_flag = row[2], row[3]
            has_cs_output = 'cs' in cs_flag or 'cs' == cs_flag
            has_lua_output = 'lua' in lua_flag or 'lua' == lua_flag
            has_bytes_output = has_cs_output or has_lua_output

            if has_bytes_output:
                if not has_client_output(P + row[0]):
                    print(row[0] + ' does not have client output')
                    row[2], row[3] = '', ''
                    file_name = row[0].replace('\\', '/').split('/')[-1].split('.')[0].lower()
                    delete_files(file_name, has_cs_output, has_lua_output)

        with open(md5_path, 'w', newline='', encoding='UTF-8-sig') as csv_file:
            csv_writer = csv.writer(csv_file)
            csv_writer.writerows(all_rows)

'''
def MergeLuaFile():
    all_lua_file = GetDirFiles(r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\Client\lua_intermediate_file/')
    all_lua_file = [x for x in all_lua_file if x.split('.')[-1] == 'lua']

    all_lua_file.sort()

    with open(r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\Client\lua\config.lua', 'w+') as lua_desc_file:
        for path in all_lua_file:
            with open(path, 'r') as tmp_lua_file:
                lua_desc_file.writelines(tmp_lua_file.readlines())

            lua_desc_file.write("\n")
'''

P1 = r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\Client\bytes\China/'
P2 = r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\Client\cscripts/'
P3 = r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\Client\lua_intermediate_file/'
def delete_files(file_name, has_cs_output, has_lua_output):
    delete_file(P1 + 'dataconfig_' + file_name + '.bytes')
    
    if has_cs_output:
        delete_file(P2 + file_name + '.cs')
    
    if has_lua_output:
        delete_file(P3 + 'tmp_lua_' + file_name + '.lua')

def delete_file(path):
    if os.path.exists(path):
        os.remove(path)
        print('delete ', path)
    else:
        print("invalid path:", path)

def has_client_output(csv_path):
    try:
        with open(csv_path, newline='', encoding='GB18030') as csv_file:
            csv_reader = csv.reader(csv_file)

            first_line = next(csv_reader)
            has_client = False
            for i in range(len(first_line)):
                if 'c' in first_line[i] or 'C' in first_line[i]:
                    has_client = True
                    break

        return has_client
    except Exception as e:
        print(e)
        return True

if __name__ == '__main__':
    CheckMD5(r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\Md5_Common(cehua_China).csv')
    CheckMD5(r'D:\WorkSpace\best2\Code\trunk\BuildDataConfig\Data\MD5_yunying\MD5yunying_China.csv')

    ConfigMgr.SetRegion('China')
    EntryPoint.MergeLuaFile()
    EntryPoint.GenBytesList()