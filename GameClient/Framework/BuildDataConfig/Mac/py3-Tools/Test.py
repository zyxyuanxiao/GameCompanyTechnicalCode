# -*- coding: UTF-8 -*-
'''
WARNING: 用于测试打表逻辑的代码，不能用于单独打表
'''

import csv
import os
import logging
import sys
import shutil


def GenSingleConfig(config_name, full_path, gen_bin, gen_csharp, gen_lua, gen_proto, gen_proto_bin):
    region = 'China'
    sheet = Sheet()
    try:
        sheet.Init(config_name, full_path)
    except Exception as e:
        print("解析配置表: {} 失败, 异常为: {}".format(config_name, e), e)
        logging.exception(e)

    if gen_bin:
        try:
            binary_file_name = ConfigMgr.Binary_Output_Dir + "dataconfig_{}.bytes".format(config_name.lower())
            with open(binary_file_name, "wb+") as bin_file:
                sheet.Marshal(bin_file, region)
        except Exception as e:
            print("配置表: {} 生成二进制失败, 异常为: {}\n".format(config_name, e), e)
            os.remove(binary_file_name)
    if gen_csharp:
        try:
            with open(ConfigMgr.CSharp_Desc_Output_Dir + "{}.cs".format(config_name), 'w+') as cs_file:
                cs_file.write(sheet.GenCSharpStr())
        except Exception as e:
            print("配置表: {} 生成C#文件失败, 异常为: {}".format(config_name, e), e)

    if gen_lua:
        try:
            with open(ConfigMgr.Lua_Desc_File, "w+") as lua_config_file:
                lua_config_file.write(sheet.GenLuaStr())
        except Exception as e:
            print("配置表: {} 生成lua文件失败, 异常为: {}".format(config_name, e), e)

    if gen_proto:
        try:
            with open(ConfigMgr.Proto_Output_Dir + 'dataconfig_{}.proto'.format(config_name.lower()), 'w+',
                      encoding='utf-8') as proto_file:
                proto_file.write(sheet.GenProto())
        except Exception as e:
            print("配置表: {} 生成proto文件失败, 异常为: {}".format(config_name, e), e)
            logging.exception(e)

    if gen_proto_bin:
        try:
            with open(ConfigMgr.Proto_Bin_Output_Dir + 'dataconfig_{}.data'.format(config_name.lower()),
                      'wb+') as pb_bin_file:
                sheet.MarshalPB(pb_bin_file, region)
        except Exception as e:
            print("配置表: {} 生成pb 二进制文件失败, 异常为: {}".format(config_name, e), e)
            logging.exception(e)


# 将修改好的 shell 替换其他文件夹里面的 shell 再设置名字
def ChangeAllShell():
    normalShell = "/Users/xlcw/Work/tw/Turing_iOS/BuildDataConfig/Mac/Mac-shell/BuildSingle.command"
    list_dirs = os.walk("/Users/xlcw/Work/tw/Turing_iOS/BuildDataConfig/DataConfig/Common/")
    for root, dirs, files in list_dirs:
        for name in files:
            path = root + "/" + "BuildSingle.commond"
            if os.path.isfile(path):
                os.remove(path)
                shutil.copy(normalShell, root + "/" + "BuildSingle.command")


# 将修改好的 shell 替换其他文件夹里面的 shell 再设置名字
def ChangeAllBat():
    normalShell = "C:/TW/BuildDataConfig/Windows/Windows-bat/single.bat"
    list_dirs = os.walk("C:/TW/BuildDataConfig/DataConfig/Common/")
    for root, dirs, files in list_dirs:
        for name in files:
            if ".bat" in name:
                path = root + "/" + name
                os.remove(path)
                print(path)
                if normalShell != path:
                    shutil.copyfile(normalShell, path)


if __name__ == "__main__":
    ChangeAllShell()
    # ChangeAllBat()
    # config_name = 'MonsterProperty'
    # full_path = 'D:/WorkSpace/best2/Code/trunk/BuildDataConfig/DataConfig/Common/sx_怪物属性表/MonsterProperty.csv'
    # GenSingleConfig(config_name, full_path, True, True, True, True, True)
