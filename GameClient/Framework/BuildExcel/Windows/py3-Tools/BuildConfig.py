from ExcelParser import Sheet
from ExcelParser.Env import ParsingEnv, ConfigMgr

import traceback

'''
    TODO:目前有一些自动生成的文件是废弃的()
'''

import csv
import os
import copy
import subprocess
from Misc.Util import *
from tkinter import messagebox


class BuildConfig:
    def __init__(self, absolutePath, region, CSharpLuaFlag, isBuildAll):
        self.region = region
        self.CSharpLuaFlag = str(CSharpLuaFlag).split(",")  # 是否打 Lua 或者 CS
        self.absolutePath = absolutePath
        self.isBuildAll = isBuildAll
        # 遍历当前文件夹,并取出所有的 csv 文件
        all_files = list(set(GetDirFiles(absolutePath)))
        all_csv_files = [x for x in all_files if x.split('.')[-1] == 'csv']
        self.absolute_paths = []
        for csv_file_path in all_csv_files:
            if "$" in csv_file_path:
                continue
            if "~" in csv_file_path:
                continue
            if "@" in csv_file_path:
                continue
            if "#" in csv_file_path:
                continue
            # 记录所有的csv路径 self.absolute_paths
            self.absolute_paths.append(csv_file_path)

    def Build(self):
        if self.isBuildAll:
            path = os.path.dirname(os.getcwd()) + "/Assets/lua/protobuf_conf_parser/config.lua"
            with open(path, "w+", encoding="utf8") as lua_file:
                pass
            path = os.path.dirname(
                os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/pbconf_res/gdata_bytes_list.bytes"
            with open(path, 'w+') as bytes_list_file:
                pass

        for path in self.absolute_paths:
            absolute_path = path.replace('\\', '/')
            absolute_path = absolute_path.replace('//', '/')
            config_name = absolute_path.split('/')[-1].split('.')[0]

            # 如果是翻译表变化了，只能通过强制打表打，潜规则
            if ParsingEnv.GetLocalization().HasLocaliztionInfo(config_name) \
                    or ParsingEnv.GetLocalization_value().HasLocaliztionInfo(config_name):
                ParsingEnv.GetLocalization().PrepareForParseConfig(config_name, self.region)

            try:
                self.__DoBuild(config_name, path)
                print("打表:{}  成功！".format(config_name))
            except ValueError as Argument:
                print("打表:{}  失败!, 错误为: {}".format(config_name, Argument))
                print(traceback.format_exc())
                messagebox.showerror(config_name,
                                     "如果修复不好,请找程序处理:\n" + "打表:{}  失败!, 错误为: {}\n".format(config_name,
                                                                                         Argument) + "请先检查数据是否为空\n" + str(
                                         traceback.format_exc()))
                raise
            except Exception as e:
                if str(e) == 'Server Is Need':
                    # 如果报错为不需要打表,则跳出打表
                    print("服务器使用,客户端无需打这张表:" + config_name)
                    pass
                else:
                    print("打表:{}  失败!, 错误为: {}".format(config_name, e))
                    print(traceback.format_exc())
                    messagebox.showerror(config_name,
                                         "如果修复不好,请找程序处理:\n" + "打表:{}  失败!, 错误为: {}".format(config_name, e) + str(
                                             traceback.format_exc()))
                    raise
            finally:
                pass

    def __DoBuild(self, config_name, full_path):
        sheet = Sheet.Sheet()
        sheet.Init(config_name, full_path)
        # print(self.CSharpLuaFlag, config_name, full_path)
        for flag in self.CSharpLuaFlag:
            if flag == "Lua":
                self.__Build_lua(sheet)
            if flag == "CSharp":
                self.__Build_cs(sheet)

        # 不管怎样,打 lua或者 打 cs 都会打二进制文件
        self.__Build_Client_Binary(sheet)
        # 不管怎样,都会打这个gdata_bytes_list
        self.__Build_GenBytesList(sheet)

    def __Build_cs(self, sheet):
        path = os.path.dirname(os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/ProtoGen/"
        file_path = path + sheet.config_name.lower() + ".cs"
        # 如果不存在则创建
        if not os.path.exists(file_path):
            with open(file_path, 'w+') as csharp_file:
                pass
        with open(file_path, 'r+', encoding='utf-8') as csharp_file:
            str_new = sheet.GenCSharpStr()
            str_old = csharp_file.read()
            if str_new != str_old:
                try:
                    csharp_file.seek(0)
                    csharp_file.truncate()
                    csharp_file.write(sheet.GenCSharpStr())
                except:
                    csharp_file.close()
                    os.remove(file_path)
                    raise

    def __Build_lua(self, sheet):
        path = os.path.dirname(os.getcwd()) + "/Assets/lua/protobuf_conf_parser/config.lua"
        with open(path, "r+", encoding="utf8") as lua_file:
            delimiter = "---------" + sheet.config_name.lower() + "---------"
            allContent = lua_file.read()
            if allContent.find(delimiter) > 0:
                allListContent = str(allContent).split(delimiter)
                # 旧文件里面的数据和当前打出的数据不一致时,重新给其赋值
                if allListContent[1] != sheet.GenLuaStr().strip():
                    allListContent[1] = delimiter + "\n" + sheet.GenLuaStr().strip() + "\n" + delimiter + "\n" + "\n"
                    allContent = "".join(allListContent)
                    allContent = allContent.replace("\n\n\n\n", "\n\n")
                    lua_file.seek(0)
                    lua_file.truncate()
                    lua_file.write(allContent)
            else:
                # 没有找到当前打表的 lua 数据,就需要向里面写入
                lua_file.write(delimiter + "\n")
                lua_file.write(sheet.GenLuaStr().strip() + "\n")
                lua_file.write(delimiter + "\n" + "\n")

    def __Build_Client_Binary(self, sheet):
        path = os.path.dirname(os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/pbconf_res/"
        file_path = path + 'dataconfig_{}.bytes'.format(sheet.config_name.lower())
        with open(file_path, 'wb+', ) as bin_file:
            try:
                sheet.Marshal(bin_file, self.region)
            except:
                bin_file.close()
                os.remove(file_path)
                raise

    def __Build_GenBytesList(self, sheet):
        path = os.path.dirname(
            os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/pbconf_res/gdata_bytes_list.bytes"
        name = 'dataconfig_{}.bytes'.format(sheet.config_name.lower())
        with open(path, 'r+') as bytes_list_file:
            str_bytes = bytes_list_file.read()
            if "False" not in str_bytes:
                str_bytes = "False" + "\n" + str_bytes
            if name not in str_bytes:
                str_bytes = str_bytes + "\n" + name
                bytes_list_file.seek(0)
                bytes_list_file.truncate()
                bytes_list_file.write(str_bytes)
