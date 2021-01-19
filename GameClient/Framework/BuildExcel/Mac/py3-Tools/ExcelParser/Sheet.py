from .ValueType import *
from .Env import *
from .Attribute import *
from .PB_Wire import *

import csv

'''
    支持的数据类型：
        value type: uint32, sint32, uint64, sint64, bool, string ,float (其中uint32, sint32, string可以用作key列)
        composed type: repeated , struct(只能出现在repeated中，不可单独出现)
    支持的修饰符：
        qualifier: optional, required
    
    配置表结构的EBNF：
    config ::= <struct>
    struct ::= {<value_type>} {repeated}
    repeated ::= <value_type> | <struct>
    value_type ::=  uint32 | sint32 | uint64 | sint64 | bool | string  | int32 | int64

    第四版（定长+变长）：
    layout:
        每一个表的二进制数据结构如下(n行)：
        gdataqqq | version_code(暂时没用上) | 数据条目数量 |  strings偏移量(没有时为0) | strings长度 (没有时为0) | data_meta偏移量 | data长度
          64bit       32bit                   32bit         32bit                         32bit                   32bit           32bit

        
        key:
                key列的数量(p)
                    8bit

                第一种key的名称的strings_offset |  key类型 (8bit) |                 key1的值                    | key2的值 | .... | keyn的值
                32 bit                          1代表uint32,       32bit(uint32, sint32, strings_offset中的一种)
                                                2代表sint32,
                                                3代表strings_offset
                                                最高位为1表示是unique key， 为0表示非unique key
                                                次高位为1表示是optional的key列
                第二种key格式同上
                ...
                第p种key


        data_meta:
            (offset是相对于data起始位置的offset)
            数据条目1 offset | 数据条目2 offset| ...... | 数据条目N offset
                32bit           32bit                       32bit
        
        data:
            每一条数据包括两个部分: fixed_part 与variable_part。 variable_part 只包括数组， fixed_part包含除了variable_part以外的所有数据
            fixed_length  |      fixed_part             | optional_meta_len     |   optional_meta_data  |       variable_part
              16bit            长度等于fixed_length        8bit(单位是byte)           1表示空，8bit对齐
                            (fixed_length = 0时没有fixed_part)                   (optional_meta_len = 0时没有optional_meta_data)
            
            variable_part:
                假设有r个repeated字段(offset是相对于本data起始位置的offset)：
                repeated attribute count  |repeated attribute 1  offset| repeated attribute 2 offset | ... | repeated attribute r offset| repeated data 1| ... | repeated data n
                   16 bit(r)                        16 bit                          16bit                               16bit
                
            repeated data:
                数组元素数量 c :
                    16 bit

                数组元素如果是value_type:
                    repeated data就是value_type数组

                数组元素如果是struct:
                    struct 1 offset | ... | struct c offset | struct 1 | ... | struct c
                        16bit                   16bit
        
        strings (配置表中没有string时 此部分数据完全不存在)
                字符串1
                字符串2
                ...
                字符串n


第三版二进制格式：
    序列化:
        每一个表的二进制数据结构如下(n行)：
            数据条目数量 |  data偏移量 | 每一条data的长度 | strings偏移量(没有时为0) | strings数量 (没有时为0) | optional meta数据的偏移量 
                 32bit      32bit           32bit           32bit                       32bit                   32bit              

            key:
                key列的数量（至少有一列作为key)
                    8bit

                第一种key的名称的string_index |  key类型 (8bit) |                 key1的值                    | key2的值 | .... | keyn的值
                32 bit                          1代表uint32,       32bit(uint32, sint32, string_index中的一种)
                                                2代表sint32,
                                                3代表string_index
                                                最高位为1表示是unique key， 为0表示非unique key
                                                次高位为1表示是optional的key列（仅用来判断数据是否存在，不查询，key不存在时不输出日志）

                第二种key的名称的string_index |   key类型(8bit)  |                  key1的值                    | key2的值 | .... | keyn的值
                32 bit                          1代表uint32        32bit(uint32, sint32, string_index中的一种)
                                                2代表sint32,
                                                3代表string_index
                                                最高位为1表示是unique key，为0表示非unique key
                                                次高位为1表示是optional的key列（仅用来判断数据是否存在，不查询，key不存在时不输出日志）
                ...

            data_optional_meta(lua层读取为空的optional字段时需返回nil，此meta数据用于描述optional是否为空, C#层读取optional时使用类型默认值即可):
                optional列数量
                    16bit
                (如果有m个optional列(只有value type)，则此处有m * n bit数据，为0表示非空，为1表示空. 如果m * n不是8的整数，则在二进制末尾补齐余数位0)
            
            data:
                数据条目1  (每一条数据条目的长度都相等)
                数据条目2
                ...
                数据条目N
        
            strings (配置表中没有string时 此部分数据完全不存在)
                字符串1
                字符串2
                ...
                字符串n


        数据编码方式如下:
            optional全部都写入
            uint32，sint32全都是32bit
            uint64, sint64全都是64bit
            bool 是8bit
            string全都是32bit的string_array的index(从1开始， 0表示空字符串)
            repeated全部按照最大长度存储,每一项repeated起始位置有2byte用于存储该repeated的实际长度
            struct是上述全部内容按顺序序列化的结果
'''

'''
    废弃的数据编码方案(仅做备份用，不需要阅读):
        第二版方案：
                index (根struct没有此项)
                varint
            1. 长度非0的repeated或者使用默认值的optionnal，完全不存在于二进制中。
            2. uint32, sint32, uint64, sint64, bool, string(只存储string_index)
                        index       |       data
               varint(一般小于255)         varint
            3. repeated:
                        index       |       element_count   |   serialized_data 
                varint(一般小于255)             varint          序列化后的value type或者struct
            4. struct:
                        index       |       length          |   serialized_data
                varint(一般小于255)             varint          序列化后的value type或者repeated

        第一版方案：
            index数量
                16bit

            index:
                index1的raw_data的长度 | index2的raw_data的长度 | .... | indexn的raw_data的长度
                    varint                  varint                          varint
                
                对于required和非默认值的optional，都会有有效的长度。
                为空或者默认值的optional，长度为0.
                repeated/struct被视为内嵌的数据条目。在母条目中只占1个index

            raw_data:
                二进制流
'''

VALUE_TYPE_MAP = {
    ConfigMgr.Value_Type_UInt32: UInt32,
    ConfigMgr.Value_Type_SInt32: SInt32,
    ConfigMgr.Value_Type_Int32: Int32,
    ConfigMgr.Value_Type_UInt64: UInt64,
    ConfigMgr.Value_Type_SInt64: SInt64,
    ConfigMgr.Value_Type_Int64: Int64,
    ConfigMgr.Value_Type_Bool: Bool,
    ConfigMgr.Value_Type_String: String,
    ConfigMgr.Value_Type_Date: Date,
    ConfigMgr.Value_Type_Float: Float,
}


def GetExportMask(content):
    raw_content = content
    content = content.strip().lower()
    c_key = ConfigMgr.Client_CKey_Tag.lower()
    cs_key = ConfigMgr.Client_CSKey_Tag.lower()
    cn_key = (ConfigMgr.Client_CKey_Tag + "\n" + ConfigMgr.Non_Unique_Key_Tag).lower()
    csn_key = (ConfigMgr.Client_CSKey_Tag + "\n" + ConfigMgr.Non_Unique_Key_Tag).lower()

    export_mask = 0
    if content == 'c' or content == c_key or content == cn_key:
        export_mask = MASK_EXRORT_TO_CLIENT
    elif content == 's':
        export_mask = MASK_EXPORT_TO_SERVER
    elif content == 'cs' or content == cs_key or content == csn_key:
        export_mask = MASK_EXPORT_TO_SERVER | MASK_EXRORT_TO_CLIENT

    return export_mask

global IsServerBuild

class Sheet:
    def __init__(self):
        global IsServerBuild
        IsServerBuild = False
        self.config_name = ""
        self.file_full_path = ""

        self.first_line = []
        self.second_line = []
        self.third_line = []
        self.fourth_line = []
        self.fifth_line = []
        self.data_row_num = 0

        self.root_struct = None
        self.active_struct_stack = []
        self.active_struct = None
        self.global_string_list = []
        self.key_attri_list = []
        self.data_entry_len = 0

        self.data_rows = []
        self.real_row_number = []
        self.full_csv_datas = []

        self.is_repeated_struct = False  # repeated struct中的所有属性都是optional的
        self.client_optional_attribute_count = 0

    def __IsValueType(self, type_str):
        return type_str in VALUE_TYPE_MAP

    def __GetValueType(self, col_num):
        value_type_str = self.third_line[col_num]

        if self.__IsValueType(value_type_str):
            return VALUE_TYPE_MAP[value_type_str]
        else:
            LogHelp.LogError(
                '配置表：{} {}行，{}列， 期望:值类型， 实际: {}'.format(ParsingEnv.GetCurSheetPath(),  col_num,3, value_type_str))

    def __CreateOptional(self, col_num, export_mask):
        value_type = self.__GetValueType(col_num)
        name = self.fourth_line[col_num]
        op = Optional(col_num, value_type, name, export_mask)
        self.active_struct.AppendAttribute(op)

        if ConfigMgr.Client_CKey_Tag in self.first_line[col_num] or ConfigMgr.Client_CSKey_Tag in self.first_line[
            col_num]:
            if export_mask & MASK_EXRORT_TO_CLIENT == MASK_EXRORT_TO_CLIENT:
                op.SetAsKey(not (ConfigMgr.Non_Unique_Key_Tag in self.first_line[col_num]), True)
                self.key_attri_list.append(op)
            else:
                LogHelp.LogError(" {}列是只导出服务器数据的列，但是被错误的添加了CLIENT_KEY标记".format(self.fourth_line[col_num]))

        if export_mask & MASK_EXRORT_TO_CLIENT == MASK_EXRORT_TO_CLIENT:
            self.client_optional_attribute_count += 1

        return col_num + 1

    def __CreateRequired(self, col_num, export_mask):
        value_type = self.__GetValueType(col_num)
        name = self.fourth_line[col_num]
        req = Required(col_num, value_type, name, export_mask, self.is_repeated_struct)
        self.active_struct.AppendAttribute(req)

        if ConfigMgr.Client_CKey_Tag in self.first_line[col_num] or ConfigMgr.Client_CSKey_Tag in self.first_line[
            col_num]:
            if export_mask & MASK_EXRORT_TO_CLIENT == MASK_EXRORT_TO_CLIENT:
                req.SetAsKey(not (ConfigMgr.Non_Unique_Key_Tag in self.first_line[col_num]))
                self.key_attri_list.append(req)
            else:
                LogHelp.LogError("{}列是只导出服务器数据的列，但是被错误的添加了CLIENT_KEY标记".format(self.fourth_line[col_num]))

        return col_num + 1

    def __PushActiveStruct(self, struct):
        self.active_struct_stack.append(struct)
        self.active_struct = struct

    def __PopActiveStruct(self, name):
        pre_struct = self.active_struct_stack.pop()
        if False and ExportToClient(pre_struct.export_mask):
            client_attri_count = 0
            has_attri = False
            for attri in pre_struct.GetAllAttri():
                if isinstance(attri, Attribute):
                    has_attri = True
                    if ExportToClient(attri.export_mask):
                        client_attri_count += 1
            if has_attri and client_attri_count == 0:
                LogHelp.LogError(
                    "配置表： {}  {}在生成客户端数据时为空结构体(结构体为C或CS，但是所有结构体内属性都为S)!".format(ParsingEnv.GetCurSheetPath(), name))

        self.active_struct = self.active_struct_stack[len(self.active_struct_stack) - 1]

    def __GetRepeatedValueMaxLen(self, col_num):
        max_len = 0
        for data_row in self.data_rows:
            content = data_row[col_num].strip()
            local_len = len(content.split(';'))
            max_len = max(max_len, local_len)

        return max_len

    def __CreateRepeated(self, col_num, export_mask):
        flag = self.third_line[col_num]

        if self.__IsValueType(flag):
            name = self.fourth_line[col_num]
            item_type = VALUE_TYPE_MAP[flag]
            max_len = self.__GetRepeatedValueMaxLen(col_num)
            self.active_struct.AppendAttribute(RepeatedValueType(col_num, item_type, name, max_len, export_mask))
            return col_num + 1
        else:
            try:
                struct_max_count = int(flag)
            except:
                LogHelp.LogError(
                    '配置表：{} {}列，{}行， 期望:整数  实际: {}'.format(ParsingEnv.GetCurSheetPath(), col_num + 1,4, flag))

            name = self.fourth_line[col_num + 1]

            try:
                struct_element_num = int(self.third_line[col_num + 1].strip())
            except:
                LogHelp.LogError('配置表：{} {}列，{}行， 期望:整数  实际: {}'.format(ParsingEnv.GetCurSheetPath(), col_num + 2,3, 
                                                                        self.third_line[col_num + 1].strip()))

            if (col_num + 2 + struct_element_num * struct_max_count) > len(self.first_line):
                struct_max_count = int((len(self.first_line) - (col_num + 2)) / struct_element_num)

            struct_array = RepeatedStruct(col_num, struct_max_count, name, export_mask)
            col_num += 2
            repeated_max_num = [0] * struct_element_num
            for i in range(struct_max_count):
                struct = Struct()
                struct.SetExportMask(export_mask)
                self.__PushActiveStruct(struct)
                self.is_repeated_struct = True
                local_index = 0
                # 这个地方是为了兼容策划在struct中穿插注释列。但是会导致如果策划配置错了，不好查错
                while local_index < struct_element_num and col_num < len(self.first_line):
                    attr_export_mask = GetExportMask(self.first_line[col_num])
                    if attr_export_mask != 0:
                        col_num = self.__CreateAttribute(col_num, attr_export_mask)
                        new_attri = struct.GetAttri(local_index)
                        if isinstance(new_attri, RepeatedValueType):
                            max_num = new_attri.GetMaxNum()
                            if max_num > repeated_max_num[local_index]:
                                repeated_max_num[local_index] = max_num

                        local_index += 1
                    else:
                        col_num += 1

                struct_array.AppendStruct(struct)
                self.__PopActiveStruct(name)
                self.is_repeated_struct = False

            for i in range(struct_element_num):
                repeated_max_count = repeated_max_num[i]
                if repeated_max_count == 0:
                    continue

                for j in range(struct_max_count):
                    inner_struct = struct_array.GetStruct(j)
                    repeated_attri = inner_struct.GetAttri(i)
                    repeated_attri.SetMaxNum(repeated_max_count)

            self.active_struct.AppendAttribute(struct_array)

            return col_num

    def __CreateAttribute(self, col_num, export_mask):
        try:
            qualifer = self.second_line[col_num].strip().lower()
        except:
            LogHelp.LogError('配置表：{} 列数超过最大值'.format(ParsingEnv.GetCurSheetPath()))

        if qualifer == ConfigMgr.Optional_Qualifer:
            col_num = self.__CreateOptional(col_num, export_mask)
        elif qualifer == ConfigMgr.Required_Qualifer:
            col_num = self.__CreateRequired(col_num, export_mask)
        elif qualifer == ConfigMgr.Repeated_Qualifer:
            col_num = self.__CreateRepeated(col_num, export_mask)
        else:
            qualifer = '空' if len(qualifer) == '' else qualifer
            LogHelp.LogError(
                '配置表：{} {} 行， {}列， 期望为：required或optional或repeated, 实际为: {}'.format(ParsingEnv.GetCurSheetPath(),
                                                                                   col_num + 1, 2, qualifer))

        return col_num

    # 建立root_struct
    def Init(self, config_name, file_full_path):
        ParsingEnv.SetCurSheet(config_name, file_full_path)
        ParseLog.Init(ConfigMgr.Parsing_Log_Path)
        self.config_name = config_name
        self.file_full_path = file_full_path

        self.global_string_list = []
        StringMgr.SetGlobalStringList(self.global_string_list)
        StringMgr.ResetStreamStringInfo()

        self.root_struct = Struct()
        self.root_struct.SetName(config_name)
        self.__PushActiveStruct(self.root_struct)

        self.full_csv_datas = []

        with open(file_full_path, newline='', encoding='UTF-8-sig') as csv_file:
            col_num = 0

            reader = csv.reader(csv_file)

            self.first_line = next(reader)
            self.second_line = next(reader)
            self.third_line = next(reader)
            self.fourth_line = next(reader)
            self.fifth_line = next(reader)

            self.full_csv_datas.append(self.first_line)
            self.full_csv_datas.append(self.second_line)
            self.full_csv_datas.append(self.third_line)
            self.full_csv_datas.append(self.fourth_line)
            self.full_csv_datas.append(self.fifth_line)

            row = 5
            for data_row in reader:
                row += 1
                emptyIndex = 0
                for emptyCheck in data_row:
                    if self.second_line[emptyIndex] == ConfigMgr.Required_Qualifer:
                        if str(emptyCheck).strip() == "":
                            raise ValueError("第:" + str(row) + "行", "第"+str(emptyIndex+1) + "列")
                    emptyIndex += 1
                if len(data_row) == 0:
                    continue

                if not data_row[0].startswith('//'):
                    self.data_rows.append(data_row)
                    self.real_row_number.append(row)

                self.full_csv_datas.append(data_row)

            self.data_row_num = len(self.data_rows)

            ParsingEnv.SetCommentRow(self.fifth_line)
            # 建立struct
            while col_num < len(self.first_line):
                export_mask = GetExportMask(self.first_line[col_num])
                if export_mask != 0:
                    col_num = self.__CreateAttribute(col_num, export_mask)
                else:
                    col_num += 1

    def GenLuaStr(self):
        return self.root_struct.GenLua(0)

    def GenCSharpStr(self):
        return self.root_struct.GenCSharp(0)

    def GenProto(self):
        global IsServerBuild
        IsServerBuild = True
        content = self.root_struct.GenMessage()
        IsServerBuild = False
        return content

    def MarshalPB(self, binary_stream, region):
        global IsServerBuild
        IsServerBuild = True
        ParsingEnv.SetCurCSVData(self.full_csv_datas)
        for i in range(self.data_row_num):
            data_row = list(self.data_rows[i]) # 深拷贝对象,防止串数据
            real_row_number = self.real_row_number[i]
            for index in range(len(data_row)):
                if "float" in str(self.third_line[index]):
                    # raise Exception("\nfloat 类型不能用于服务器打表上面,将 Excel 的 CS 中的S 去除,或者换成int格式\n")
                    data_row[index] = str(float(data_row[index])/4294967296)
                    # print(index, self.third_line[index], data_row[index])
            self.root_struct.Parse(data_row, real_row_number, region)
            bs = io.BytesIO()
            self.root_struct.MarshalMessage(bs)
            key_encoding(binary_stream, 1, 2)  # message的wire type是2， index永远是1
            buffer = bs.getbuffer()
            varint_encoding(binary_stream, len(buffer))
            binary_stream.write(buffer)
        IsServerBuild = False

    # 产生二进制数据
    def Marshal(self, byte_file_writer, region):
        if self.data_row_num == 0:
            # 配置表中没数据，不处理
            return

        ParsingEnv.SetCurCSVData(self.full_csv_datas)

        if len(self.key_attri_list) == 0 and not ConfigMgr.Disable_Key_Column_Check:
            LogHelp.LogError('配置表:{} 没有配置key列，无法导出数据!'.format(ParsingEnv.GetCurSheetPath()))

        data_stream = io.BytesIO()
        data_offset_array, data_offset = [], 0
        ParseLog.Log('start marshal sheet: {}'.format(self.config_name))
        ParseLog.IncreaseIndent()
        for i in range(len(self.data_rows)):
            data_row = self.data_rows[i]
            real_row_number = self.real_row_number[i]

            self.root_struct.Parse(data_row, real_row_number, region)
            fixed_stream, optional_stream, variable_meta, variable_stream = io.BytesIO(), io.BytesIO(), io.BytesIO(), io.BytesIO()
            self.root_struct.Marshal(fixed_stream, optional_stream, variable_meta, variable_stream)
            record_data_stream = ParsingEnv.MergeStream(fixed_stream, optional_stream, variable_meta, variable_stream)

            ParsingEnv.UpdateStreamingStringOffset(record_data_stream, data_stream)

            data_offset_array.append(data_offset)
            data_offset += len(record_data_stream.getbuffer())
            data_stream.write(record_data_stream.getbuffer())

            if ConfigMgr.Enable_Entry_Binary_Log:
                with open(ConfigMgr.Entry_Binary_Log_path + "{}_{}".format(self.config_name, i), 'wb') as log_file:
                    log_file.write(record_data_stream.getbuffer())
            ParseLog.Log('\n\n')
        ParseLog.DecreaseIndent()

        if (len(data_stream.getbuffer()) == 0):
            return

        strings_data_stream = io.BytesIO()
        string_index_2_offset, local_offset = [], 0
        if len(self.global_string_list) > 0:
            for s in self.global_string_list:
                string_index_2_offset.append(local_offset)
                s_bytes = s.encode('utf-8') + b'\0'
                strings_data_stream.write(s_bytes)
                local_offset += len(s_bytes)

        # 修正data_stream中的string offset
        len_data_stream = len(data_stream.getbuffer())
        data_streaming_string_info = StringMgr.GetStreamStringInfo(data_stream)
        if data_streaming_string_info != None:
            for offset in data_streaming_string_info:
                ParsingEnv.RewriteStringInfo(data_stream, string_index_2_offset, len_data_stream - offset, offset)

        key_stream = io.BytesIO()
        key_stream.write((len(self.key_attri_list).to_bytes(1, byteorder="big", signed=False)))
        for key_attri in self.key_attri_list:
            key_attr_stream = key_attri.GetKeyStream()
            ParsingEnv.RewriteStringInfo(key_attr_stream, string_index_2_offset, len_data_stream, 0)
            if isinstance(key_attri.value_type_obj, String):
                seek_offset = 5
                while seek_offset < len(key_attr_stream.getbuffer()):
                    ParsingEnv.RewriteStringInfo(key_attr_stream, string_index_2_offset, len_data_stream, seek_offset)
                    seek_offset += 4

            key_stream.write(key_attr_stream.getbuffer())

        # 输出最终的二进制包

        # gdataqqq | version_code(暂时没用上) | 数据条目数量 |  strings偏移量(没有时为0) | strings数量 (没有时为0) | data_meta偏移量
        #  64bit       32bit                   32bit         32bit                         32bit                   32bit
        # key_stream
        # optional_meta_stream
        # data_stream
        # strings_stream

        final_stream = io.BytesIO()

        # 写入header部分
        header_len = 32
        final_stream.write(b'gdataqqq')
        version_code = 0
        final_stream.write(version_code.to_bytes(4, byteorder='big', signed=False))
        final_stream.write(self.data_row_num.to_bytes(4, byteorder='big', signed=False))
        data_meta_offset = header_len + len(key_stream.getbuffer())
        data_offset_bias = data_meta_offset + 4 * self.data_row_num
        string_offset = data_offset_bias + len(data_stream.getbuffer())
        final_stream.write(string_offset.to_bytes(4, byteorder='big', signed=False))
        final_stream.write(len(strings_data_stream.getbuffer()).to_bytes(4, byteorder='big', signed=False))
        final_stream.write(data_meta_offset.to_bytes(4, byteorder='big', signed=False))
        final_stream.write(len(data_stream.getbuffer()).to_bytes(4, byteorder='big', signed=False))

        data_meta_stream = io.BytesIO()
        for offset in data_offset_array:
            data_meta_stream.write(offset.to_bytes(4, byteorder='big', signed=False))

        # 写入数据部分
        final_stream.write(key_stream.getbuffer())
        final_stream.write(data_meta_stream.getbuffer())
        final_stream.write(data_stream.getbuffer())
        final_stream.write(strings_data_stream.getbuffer())

        byte_file_writer.write(final_stream.getbuffer())

        # with open("debug_sheet_info", 'w') as f:
        #    f.write(self.root_struct.GenClientDesc(0, 0))
