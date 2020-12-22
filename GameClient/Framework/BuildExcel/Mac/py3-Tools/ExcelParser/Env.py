'''
Parser的上下文， 用于解析配置， 字符串去重等
'''
from configparser import ConfigParser

import os
import io

class ConfigMgr:
    @staticmethod
    def Init():
        cfg = ConfigParser()
        # print(os.getcwd() + "/Mac/py3-Tools/Ini/config.ini")
        
        cfg.read(os.getcwd() + "/Mac/py3-Tools/Ini/config.ini", encoding= 'utf-8')
        ConfigMgr.Client_CKey_Tag = cfg.get("Parser", "CLIENT_CKEY_TAG")
        ConfigMgr.Client_CSKey_Tag = cfg.get("Parser", "CLIENT_CSKEY_TAG")
        ConfigMgr.Non_Unique_Key_Tag = cfg.get("Parser", "NONUNIQUE_TAG")

        ConfigMgr.EnableCompress = cfg.getboolean('Parser', "EnableCompress")
        ConfigMgr.EnableMerge = cfg.getboolean('Parser', "EnableMerge")
        ConfigMgr.MergeFileSize = cfg.getint('Parser', 'MergeFileSize')

        ConfigMgr.Binary_List_File_Name = cfg.get('Parser', 'Binary_List_File_Name')
        ConfigMgr.Binary_Output_Dir = os.getcwd() + cfg.get('Parser', 'Binary_Output_Dir')
        ConfigMgr.Parser_Log_File = os.getcwd() + cfg.get('Parser', 'Parser_Log_File')
        ConfigMgr.Lua_Desc_File = os.getcwd() + cfg.get('Parser', 'Lua_Desc_File')
        ConfigMgr.Temp_Lua_Desc_Path = os.getcwd() + cfg.get('Parser', 'Temp_Lua_Desc_Path')
        ConfigMgr.CSharp_Desc_Output_Dir = os.getcwd() + cfg.get('Parser', 'CSharp_Desc_Output_Dir')
        ConfigMgr.Proto_Output_Dir = os.getcwd() + cfg.get('Parser', 'Proto_Output_Dir')
        ConfigMgr.Proto_Bin_Output_Dir = os.getcwd() + cfg.get('Parser', 'Proto_Bin_Output_Dir')

        ConfigMgr.Disable_Key_Column_Check = cfg.getboolean('Debug', 'Disable_Key_Column_Check')
        ConfigMgr.Disable_Require_Check = cfg.getboolean('Debug', 'Disable_Require_Check')
        ConfigMgr.Disable_Unique_Key_Check = cfg.getboolean('Debug', 'Disable_Unique_Key_Check')
        ConfigMgr.Enable_Parse_Log = cfg.getboolean('Debug', 'Enable_Parse_Log')
        ConfigMgr.Parsing_Log_Path = cfg.get('Debug', 'Parse_Log')
        ConfigMgr.Enable_Entry_Binary_Log = cfg.getboolean('Debug', 'Enable_Entry_Binary_Log')
        ConfigMgr.Entry_Binary_Log_path = cfg.get('Debug', 'Entry_Binary_Log_path')

        ConfigMgr.Optional_Qualifer = cfg.get('Excel', 'Optional_Qualifer')
        ConfigMgr.Required_Qualifer = cfg.get('Excel', 'Required_Qualifer')
        ConfigMgr.Repeated_Qualifer = cfg.get('Excel', 'Repeated_Qualifer')
        ConfigMgr.Value_Type_UInt32 = cfg.get('Excel', 'Value_Type_UInt32')
        ConfigMgr.Value_Type_SInt32 = cfg.get('Excel', 'Value_Type_SInt32')
        ConfigMgr.Value_Type_Int32 = cfg.get('Excel', 'Value_Type_Int32')
        ConfigMgr.Value_Type_UInt64 = cfg.get('Excel', 'Value_Type_UInt64')
        ConfigMgr.Value_Type_SInt64 = cfg.get('Excel', 'Value_Type_SInt64')
        ConfigMgr.Value_Type_Int64  = cfg.get('Excel', 'Value_Type_Int64')
        ConfigMgr.Value_Type_Bool  = cfg.get('Excel', 'Value_Type_Bool')
        ConfigMgr.Value_Type_String = cfg.get('Excel', 'Value_Type_String')
        ConfigMgr.Value_Type_Date = cfg.get('Excel', 'Value_Type_Date')
        ConfigMgr.Value_Type_Float = cfg.get('Excel', 'Value_Type_Float')

        ConfigMgr.Value_Excel_Path = os.getcwd() + cfg.get('Localization', 'Value_Excel_Path')
        ConfigMgr.Value_Config_Excel_Path = os.getcwd() + cfg.get('Localization', 'Value_Config_Excel_Path')
        ConfigMgr.Localization_Excel_Path = os.getcwd() + cfg.get('Localization', 'Localization_Excel_Path')
        ConfigMgr.Localization_Excel_Config_Path = os.getcwd() + cfg.get('Localization', 'Localization_Excel_Config_Path')
        ConfigMgr.Localization_Log_File = os.getcwd() + cfg.get('Localization', 'Localization_Log_File')

        ConfigMgr.CSV_Path = cfg.get('MD5', 'CSV_Path')
        ConfigMgr.Yunying_MD5_Path_Prefix = cfg.get('MD5', 'Yunying_MD5_Path_Prefix')
        ConfigMgr.CeHua_MD5_Path_Prefix  = cfg.get('MD5', 'CeHua_MD5_Path_Prefix')
        ConfigMgr.Common_MD5_File  = cfg.get('MD5', 'Common_MD5_File')

    @staticmethod
    def SetRegion(region):
        ConfigMgr.Binary_Output_Dir += '/' + region + '/'
        ConfigMgr.Proto_Bin_Output_Dir += '/' + region + '/'


class StringMgr:
    global_string_list = []
    reverse_index_map = {}

    stream_string_info = {}

    @staticmethod
    def SetGlobalStringList(global_string_list):
        StringMgr.global_string_list = global_string_list
        StringMgr.reverse_index_map = {}

    @staticmethod
    def GetStringIndex(string):
        if string in StringMgr.reverse_index_map:
            return StringMgr.reverse_index_map[string]
        else:
            StringMgr.global_string_list.append(string)
            index = len(StringMgr.global_string_list)
            StringMgr.reverse_index_map[string] = index
            return index
    
    @staticmethod
    def ResetStreamStringInfo():
        StringMgr.stream_string_info = {}
    
    @staticmethod
    def AddStreamStringInfo(stream, offset):
        if stream not in StringMgr.stream_string_info:
            StringMgr.stream_string_info[stream] = []
        StringMgr.stream_string_info[stream].append(offset)

    @staticmethod
    def GetStreamStringInfo(stream):
        if stream in StringMgr.stream_string_info:
            return StringMgr.stream_string_info[stream]

        return None
    
    @staticmethod
    def RemoveStreamStringInfo(stream):
        del StringMgr.stream_string_info[stream]

class OptionalData:
    def __init__(self):
        self.optional_data = 0
        self.optional_index = 0

class ParsingEnv:
    cur_sheet_name = ""
    cur_sheet_path = ""
    cur_row = 1
    cur_colmun = 1
    err_msg = ""
    comment_row = []
    localization = None
    localization_value = None
    cur_csv_datas = []

    stream_2_optional_data = dict()

    @staticmethod
    def Init():
        ConfigMgr.Init()

    @staticmethod
    def SetErrorMsg(msg):
        ParsingEnv.err_msg = msg
    
    @staticmethod
    def SetCurSheet(name, path):
        ParsingEnv.cur_sheet_name = name
        ParsingEnv.cur_sheet_path = path

    @staticmethod
    def GetCurSheet():
        return ParsingEnv.cur_sheet_name

    @staticmethod
    def GetCurSheetPath():
        return ParsingEnv.cur_sheet_path
    
    @staticmethod
    def SetLocalization(locale, locale_value):
        ParsingEnv.localization = locale
        ParsingEnv.localization_value = locale_value

    #翻译表
    @staticmethod
    def GetLocalization():
        return ParsingEnv.localization

    #翻译数值表
    @staticmethod
    def GetLocalization_value():
        return ParsingEnv.localization_value
    
    @staticmethod
    def SetCommentRow(comment_row):
        ParsingEnv.comment_row = comment_row

    @staticmethod
    def GetCommentRow():
        return ParsingEnv.comment_row

    @staticmethod
    def SetCurCSVData(csv_datas):
        ParsingEnv.cur_csv_datas = csv_datas
    
    @staticmethod
    def GetCurCSVData():
        return ParsingEnv.cur_csv_datas
    
    @staticmethod
    def MarshalOptionalData(name, bool_optional, optional_meta_stream):
        if optional_meta_stream not in ParsingEnv.stream_2_optional_data:
            ParsingEnv.stream_2_optional_data[optional_meta_stream] = OptionalData()

        stream_data = ParsingEnv.stream_2_optional_data[optional_meta_stream]
        ParseLog.Log("optional info, name: {} , optional_index: {}, optional: {}".format(name, len(optional_meta_stream.getbuffer()) * 8 + stream_data.optional_index, bool_optional))
        if bool_optional:
            stream_data.optional_data |= 0x1 << stream_data.optional_index
        stream_data.optional_index += 1

        if stream_data.optional_index == 8:
            ParsingEnv.__DoMarshalOptional(optional_meta_stream)

    @staticmethod
    def __DoMarshalOptional(optional_meta_stream):
        stream_data = ParsingEnv.stream_2_optional_data[optional_meta_stream]
        bin_data = bin(stream_data.optional_data)[2:]
        if len(bin_data) < 8:
            bin_data = '0' * (8 - len(bin_data)) + bin_data
        reverse_bin_data = bin_data[::-1]
        data = int(reverse_bin_data, 2)
        optional_meta_stream.write(data.to_bytes(1, byteorder = 'big', signed = False))
        stream_data.optional_data = 0x0
        stream_data.optional_index = 0
    

    @staticmethod
    def FinishMarshalOptionalData(optional_meta_stream):
        if optional_meta_stream not in ParsingEnv.stream_2_optional_data:
            return

        stream_data = ParsingEnv.stream_2_optional_data[optional_meta_stream]
        if stream_data.optional_index == 0:
            return

        ParsingEnv.__DoMarshalOptional(optional_meta_stream)       
    
    @staticmethod
    def UpdateStreamingStringOffset(to_be_merged_stream, merged_stream):
        stream_info = StringMgr.GetStreamStringInfo(to_be_merged_stream)
        if stream_info != None:
            offset_t = len(merged_stream.getbuffer())
            for offset in stream_info:
                StringMgr.AddStreamStringInfo(merged_stream, offset + offset_t)
            StringMgr.RemoveStreamStringInfo(to_be_merged_stream)

    @staticmethod
    def MergeStream(fixed_stream, optional_stream, variable_meta, variable_stream):
        record_data_stream = io.BytesIO()
        fixed_len = len(fixed_stream.getbuffer())
        record_data_stream.write(fixed_len.to_bytes(2, byteorder='big', signed=False))
        ParsingEnv.UpdateStreamingStringOffset(fixed_stream, record_data_stream)
        record_data_stream.write(fixed_stream.getbuffer())

        optional_len = len(optional_stream.getbuffer())
        record_data_stream.write(optional_len.to_bytes(1, byteorder='big', signed=False))
        record_data_stream.write(optional_stream.getbuffer())

        record_data_stream.write(variable_meta.getbuffer())
        ParsingEnv.UpdateStreamingStringOffset(variable_stream, record_data_stream)
        record_data_stream.write(variable_stream.getbuffer())

        return record_data_stream
    
    @staticmethod
    def RewriteStringInfo(data_stream, string_index_2_offset, extra_offset, seek_offset):
        data_stream.seek(seek_offset)
        index = int.from_bytes(data_stream.read(4), byteorder = 'big', signed = False)
        if index > 0:
            strings_offset = string_index_2_offset[index - 1]
            final_offset = strings_offset + extra_offset
            data_stream.seek(seek_offset)
            data_stream.write(final_offset.to_bytes(4, byteorder = "big", signed = False))

class ParseLog:
    log_file = None
    indent = 1
    @staticmethod
    def Init(path):
        if not ConfigMgr.Enable_Parse_Log:
            return
        ParseLog.log_file = open(path, 'w+', encoding = 'utf-8')
        ParseLog.indent = 1
    
    @staticmethod
    def IncreaseIndent():
        if not ConfigMgr.Enable_Parse_Log:
            return
        ParseLog.indent += 1
    
    @staticmethod
    def DecreaseIndent():
        if not ConfigMgr.Enable_Parse_Log:
            return
        ParseLog.indent -= 1
    
    @staticmethod
    def Log(msg):
        if not ConfigMgr.Enable_Parse_Log:
            return
        ParseLog.log_file.write('    ' * ParseLog.indent + msg + '\n')
    

ParsingEnv.Init()