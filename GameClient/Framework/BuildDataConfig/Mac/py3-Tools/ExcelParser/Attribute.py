from .ValueType import *
from .Env import *
from Misc.Util import *
from .PB_Wire import *
from Misc.Util import *

import re
import io

check_ascii_regexp = re.compile(r'[^\x00-\x7f]')

MASK_EXRORT_TO_CLIENT = 0x1
MASK_EXPORT_TO_SERVER = 0x2

ARRAY_COUNT_SIZE = 2 #每个repeated类型 前两字节是数组元素个数

def ExportToClient(mask):
    if mask == 0:
        return True

    return mask & MASK_EXRORT_TO_CLIENT == MASK_EXRORT_TO_CLIENT

def ExportToServer(mask):
    if mask == 0:
        return True

    return mask & MASK_EXPORT_TO_SERVER == MASK_EXPORT_TO_SERVER

def GenComment(comment, indent):
    s = GenIndent(indent) + '/**\n'
    for line in comment.split('\n'):
        s += GenIndent(indent + 1) + line + '\n'
    s += GenIndent(indent) + '*/\n'
    return s


class Attribute:
    def __init__(self, col_number, value_type, name, export_mask):
        self.col_number = col_number
        self.value_type_obj = value_type()
        self.value_type = value_type
        self.name = name.strip()
        self.export_mask = export_mask

        self.is_key = False
        self.key_binary_stream = None
        self.key_unique = False
        self.key_unique_check_set = set()


        if check_ascii_regexp.search(self.name):
            LogHelp.LogError('配置表：{} 属性名不能含有非Ascii字符.错误的属性名为:{} '.format(ParsingEnv.GetCurSheetPath(), self.name))

    def GetSize(self, export_mask):
        if (self.export_mask & export_mask) == 0:
            return 0

        return self.value_type.GetSize()
    
    def GetLocaleContent(self, content, row_number, region):
        if ParsingEnv.GetLocalization_value().HasLocaliztionInfo(ParsingEnv.GetCurSheet()) and isinstance(self.value_type_obj, Number):
            temp = ParsingEnv.GetLocalization_value().DoGlobalTranslate(content,\
                ParsingEnv.GetCurCSVData(), row_number, self.col_number, ParsingEnv.GetCurSheet(), region)
            if temp != None and len(temp) > 0:
                content = temp
        elif ParsingEnv.GetLocalization().HasLocaliztionInfo(ParsingEnv.GetCurSheet()) and isinstance(self.value_type_obj, String):
            temp = ParsingEnv.GetLocalization().DoGlobalTranslate(content,\
                ParsingEnv.GetCurCSVData(), row_number, self.col_number, ParsingEnv.GetCurSheet(), region)
            if temp != None and len(temp) > 0:
                content = temp
            else:
                temp = ParsingEnv.GetLocalization_value().DoGlobalTranslate(content,\
                    ParsingEnv.GetCurCSVData(), row_number, self.col_number, ParsingEnv.GetCurSheet(), region)
                if temp != None and len(temp) > 0:
                    content = temp
        return content


    def Parse(self, csv_row, row_number, region):
        raise NotImplementedError

    def Marshal(self, fixed_stream):
        if not ExportToClient(self.export_mask):
            return

        ParseLog.Log("{} : {}".format(self.name, self.value_type_obj.value))
        self.value_type_obj.Marshal(fixed_stream)
        if self.is_key:
           self.value_type_obj.Marshal(self.key_binary_stream)

    def GenLua(self, indent):
        raise NotImplementedError
    
    def GenCSharp(self, indent, relative_offset):
        if not ExportToClient(self.export_mask):
            return ''

        if self.value_type == String or self.value_type == Date:
            return self.__GenStringCSharp(indent, relative_offset)
        else:
            return self.__GenNormalCSharp(indent, relative_offset)

    def GenProto(self, indent, index):
        raise NotImplementedError

    def MarshalPB(self, index, binary_stream):
        raise NotImplementedError

    def __GenNormalCSharp(self, indent, relative_offset):
        return GenIndent(indent) + 'public {} {}\n'.format(self.value_type.GenCSharpTypeStr(), self.name) \
            + GenIndent(indent) + '{\n' + GenIndent(indent + 1) + 'get\n' + GenIndent(indent + 1) + '{\n'\
            + GenIndent(indent + 2) + 'return {}(m_dataPtr, {});\n'.format(self.value_type.CSharpReaderName(), relative_offset) \
            + GenIndent(indent + 1) + '}\n' + GenIndent(indent) + '}\n'

    def __GenStringCSharp(self, indent, relative_offset):
        return GenIndent(indent) + 'public {} {}\n'.format(self.value_type.GenCSharpTypeStr(), self.name) \
            + GenIndent(indent) + '{\n' \
            + GenIndent(indent + 1) + 'get\n'\
            + GenIndent(indent + 1) + '{\n'\
            + GenIndent(indent + 2) + "return {}(m_dataPtr, {});\n".format(self.value_type.CSharpReaderName(), relative_offset) \
            + GenIndent(indent + 1) + "}\n" \
            + GenIndent(indent) + "}\n"

    def SetAsKey(self, unique, optional = False):
        assert isinstance(self.value_type_obj, UInt32) or isinstance( self.value_type_obj, SInt32) \
            or isinstance(self.value_type_obj, String) or isinstance( self.value_type_obj, Int32) 

        if isinstance(self.value_type_obj, UInt32):
            key_type = 1
        elif isinstance(self.value_type_obj, SInt32) or isinstance(self.value_type_obj, Int32):
            key_type = 2
        elif isinstance(self.value_type_obj, String):
            key_type = 3
        else:
            LogHelp.LogError('配置表：{} 无效的key类型: {}'.format(ParsingEnv.GetCurSheetPath(), type(self.value_type_obj)))


        #key_type 低4位用于标记key类型， 高4位用于标记各种key的特性
        if unique:
            key_type |= 0x80 #unique key的最高位为1
        if optional:
            key_type |= 0x40

        self.is_key = True
        self.key_binary_stream = io.BytesIO()
        self.key_binary_stream.write(StringMgr.GetStringIndex(self.name).to_bytes(4, byteorder="big", signed=False))
        self.key_binary_stream.write(key_type.to_bytes(1, byteorder = "big", signed=False))

        self.key_unique = unique

    def GetKeyStream(self):
        return self.key_binary_stream
    
    def IsKey(self):
        return self.is_key
    
    def GenClientDesc(self, relative_offset, indent):
        if not ExportToClient(self.export_mask):
            return '', relative_offset


        desc = GenIndent(indent) +  'name : ' + self.name + '\n' \
            + GenIndent(indent) + 'relative offset: ' + str(relative_offset)

        return desc, relative_offset + self.GetSize(MASK_EXRORT_TO_CLIENT)
    

class Optional(Attribute):
    def __init__(self, col_number, value_type, name, export_mask):
        Attribute.__init__(self, col_number, value_type, name, export_mask)
        self.content = None
        self.empty_optional = False

    def GetSize(self, export_mask):
        if (self.export_mask & export_mask) == 0:
            return 0

        return self.value_type.GetSize() 

    def Parse(self, csv_row, row_number, region):
        if self.value_type != String:
            self.content = csv_row[self.col_number].strip()
        else:
            self.content = csv_row[self.col_number]

        self.content = self.GetLocaleContent(self.content, row_number, region)

        self.value_type_obj.Parse(self.content)
        self.empty_optional = len(self.content) == 0


    def Marshal(self, fixed_stream, optional_stream):
        if not ExportToClient(self.export_mask):
            return

        Attribute.Marshal(self, fixed_stream)
        ParsingEnv.MarshalOptionalData(self.name, self.empty_optional, optional_stream)

    def GenLua(self, indent):
        if not ExportToClient(self.export_mask):
            return ''

        if self.is_key:
            if self.key_unique:
                return GenIndent(indent) + self.value_type.GenLua() + '\t\t\t\"{}\":optional(), --unique key\n'.format(self.name)
            else:
                return GenIndent(indent) + self.value_type.GenLua() + '\t\t\t\"{}\":optional(), --multi key\n'.format(self.name)
        else:
            return GenIndent(indent) + self.value_type.GenLua() + '\t\t\t\"{}\":optional(),\n'.format(self.name)


    def GenProto(self, indent, index):
        if not ExportToServer(self.export_mask):
            return '', index

        comment = ParsingEnv.GetCommentRow()[self.col_number]
        return GenComment(comment, indent) + GenIndent(indent) + 'optional {} {} = {} [ default = {}];\n'.format(self.value_type.GenProto(), self.name, index, self.value_type.GenProtoDefault()), index + 1

    def MarshalPB(self, key_index, binary_stream):
        if not ExportToServer(self.export_mask):
            return key_index
        
        if len(self.content) == 0 and self.value_type != Bool: #继承自之前的打表工具的逻辑： Bool一定会产生值，不管是不是optional(这也太扯了)
            return key_index + 1 #空的optional不产生数据
        
        self.value_type_obj.MarshalPB(key_index, binary_stream)
        return key_index + 1


class Required(Attribute):
    def __init__(self, col_number, value_type, name, export_mask, in_repeated_struct = False):
        Attribute.__init__(self, col_number, value_type, name, export_mask)
        self.in_repeated_struct = in_repeated_struct

    def GenCSharp(self, indent, relative_offset):
        if not ExportToClient(self.export_mask):
            return ''

        csharp_str = Attribute.GenCSharp(self, indent, relative_offset)
        if self.is_key:
            if self.key_unique:
                return '//unique key\n' + csharp_str
            else:
                return '//multi key\n' + csharp_str
        else:
            return csharp_str

    def GenLua(self, indent):
        if not ExportToClient(self.export_mask):
            return ''

        if self.is_key:
            if self.key_unique:
                return GenIndent(indent) + self.value_type.GenLua() + '\t\t\t\"{}\", --unique key\n'.format(self.name)
            else:
                return GenIndent(indent) + self.value_type.GenLua() + '\t\t\t\"{}\", --multi key\n'.format(self.name)
        else:
            return GenIndent(indent) + self.value_type.GenLua() + '\t\t\t\"{}\",\n'.format(self.name)


    def Parse(self, csv_row, row_number, region):
        if self.value_type != String:
            content = csv_row[self.col_number].strip()
        else:
            content = csv_row[self.col_number]

        #无法检查 repeated中的required列应该有数据但是没填 的情况
        if len(content) == 0 and not ConfigMgr.Disable_Require_Check and not self.in_repeated_struct:
            LogHelp.LogError('配置表：{} {}列，{}行， required列值不能为空.'.format(ParsingEnv.GetCurSheetPath(), self.col_number + 1,row_number + 1))

        content = self.GetLocaleContent(content, row_number, region)
        self.value_type_obj.SetPos(row_number + 1, self.col_number + 1)
        self.value_type_obj.Parse(content)

        if self.key_unique:
            assert(not self.in_repeated_struct)
            if self.value_type_obj.value in self.key_unique_check_set and not ConfigMgr.Disable_Unique_Key_Check:
                LogHelp.LogError('配置表：{} {}列，{}行， unique key列中有重复的key值：'.format(ParsingEnv.GetCurSheetPath(), self.col_number + 1,row_number + 1))
            else:
                self.key_unique_check_set.add(self.value_type_obj.value)

    def GenProto(self, indent, index):
        if not ExportToServer(self.export_mask):
            return '', index

        comment = ParsingEnv.GetCommentRow()[self.col_number]
        return GenComment(comment, indent) \
            + GenIndent(indent) + 'required {} {} = {};\n'.format(self.value_type.GenProto(), self.name, index), index + 1

    def MarshalPB(self, key_index, binary_stream):
        if not ExportToServer(self.export_mask):
            return key_index
        
        self.value_type_obj.MarshalPB(key_index, binary_stream)
        return key_index + 1


class RepeatedValueType:
    def __init__(self, col_num, value_type, name, max_num, export_mask):
        self.__items = []
        self.__col_num = col_num
        self.__value_type = value_type
        self.__name = name.strip()
        self.__max_num = max_num
        self.export_mask = export_mask
        self.name = name.strip()

        i = 0
        while i < max_num:
            self.__items.append(self.__value_type())
            i += 1
        
        self.__cur_length = 0

    def SetMaxNum(self, max_num):
        assert(max_num >= self.__max_num)

        self.__max_num = max_num
        for i in range(len(self.__items), max_num):
            self.__items.append(self.__value_type())
    
    def GetMaxNum(self):
        return self.__max_num

    def GetSize(self, export_mask):
        if (self.export_mask & export_mask) == 0:
            return 0

        return self.__max_num * self.__value_type.GetSize() + ARRAY_COUNT_SIZE 

    def Parse(self, csv_row, row_number, region):
        if self.__value_type != String:
            content = csv_row[self.__col_num].strip()
        else:
            content = csv_row[self.__col_num]

        if len(content) > 0:
            value_str_array = content.split(';')
            value_str_array = [i for i in value_str_array if(len(str(i))!=0)]
            self.__cur_length = len(value_str_array)
        else:
            self.__cur_length = 0

        for i in range(self.__cur_length):
            self.__items[i].Parse(value_str_array[i])

        if self.__cur_length < self.__max_num:
            for i in range(self.__cur_length, self.__max_num):
                self.__items[i].Parse('')

    def Marshal(self, variable_stream):
        if not ExportToClient(self.export_mask):
            return

        s = self.name + ' : '
        new_stream = io.BytesIO()
        new_stream.write(self.__cur_length.to_bytes(2, byteorder="big", signed=False))

        for i in range(self.__cur_length):
            s += str(self.__items[i].value) + ', '
            self.__items[i].Marshal(new_stream)
        
        ParseLog.Log(s)
        ParsingEnv.UpdateStreamingStringOffset(new_stream, variable_stream)
        variable_stream.write(new_stream.getbuffer())

    def GenLua(self, indent):
        if not ExportToClient(self.export_mask):
            return ''

        return GenIndent(indent) + '{}_Array\t\"{}\",\n'.format(self.__value_type.GenLua(), self.__name)

    def GenCSharp(self, indent, variable_attr_index):
        if not ExportToClient(self.export_mask):
            return ''

        '''
        if self.__value_type is String:
            return GenIndent(indent) + 'public {}Array {}\n'.format(self.__value_type.GenCSharpTypeStr(), self.__name)\
            + GenIndent(indent) + '{\n'\
            + GenIndent(indent + 1) + 'get\n'\
            + GenIndent(indent + 1) + '{\n'\
            + GenIndent(indent + 2) + "var tmp = new {}Array();\n".format(self.__value_type.GenCSharpTypeStr())\
            + GenIndent(indent + 2) + "return tmp;\n"\
            + GenIndent(indent + 1) + '}\n'\
            + GenIndent(indent) + '}\n\n'
        else:
        '''
        return GenIndent(indent) + 'public {}Array {}\n'.format(self.__value_type.GenCSharpTypeStr(), self.__name)\
        + GenIndent(indent) + '{\n'\
        + GenIndent(indent + 1) + 'get\n'\
        + GenIndent(indent + 1) + '{\n'\
        + GenIndent(indent + 2) + "var tmp = new {}Array();\n".format(self.__value_type.GenCSharpTypeStr())\
        + GenIndent(indent + 2) + "tmp.Init(GData.GetVariableAttributePtr(m_dataPtr, {}));\n".format(variable_attr_index) \
        + GenIndent(indent + 2) + "return tmp;\n" \
        + GenIndent(indent + 1) + '}\n'\
        + GenIndent(indent) + '}\n\n'

    def GenProto(self, indent, index):
        if not ExportToServer(self.export_mask):
            return '', index

        comment = ParsingEnv.GetCommentRow()[self.__col_num]
        return GenComment(comment, indent) \
            + GenIndent(indent) + 'repeated {} {} = {};\n'.format(self.__value_type.GenProto(), self.__name, index), index + 1

    def MarshalPB(self, key_index, binary_stream):
        if not ExportToServer(self.export_mask):
            return key_index
        
        if self.__cur_length == 0:
            return key_index + 1# 长度为0的repeated不产生数据
        
        for i in range(self.__cur_length):
            self.__items[i].MarshalPB(key_index, binary_stream) #repeated共用同一个key_index

        return key_index + 1

    def GenClientDesc(self, relative_offset, indent):
        if not ExportToClient(self.export_mask):
            return '', relative_offset


        desc = GenIndent(indent) +  'name : ' + self.__name + '\n' \
            + GenIndent(indent) + 'relative offset: ' + str(relative_offset)

        return desc, relative_offset + self.GetSize(MASK_EXRORT_TO_CLIENT)


class RepeatedStruct:
    def __init__(self, start_col, maxCount, name, export_mask):
        self.__start_col = start_col
        self.__max_count = maxCount
        self.__structs = []
        self.name = name.strip()
        self.export_mask = export_mask

        self.__cur_count = 0

    def AppendStruct(self, struct):
        self.__structs.append(struct)
    
    def GetStruct(self, index):
        return self.__structs[index]

    def GetSize(self, export_mask):
        if (self.export_mask & export_mask) == 0:
            return 0

        total_len = 0
        for struct in self.__structs:
            total_len += struct.GetSize(export_mask)
        return total_len + ARRAY_COUNT_SIZE 

    def Parse(self, csv_row, row_number, region):
        content = str(csv_row[self.__start_col]).strip()
        if len(content) == 0:
            self.__cur_count = 0
        else:
            self.__cur_count = int(float(content))

        if self.__cur_count > self.__max_count:
            print("配置表:{} 第{}行 repeated数量： {}超过最大数量: {}.".format(ParsingEnv.GetCurSheet(), row_number, self.__cur_count, self.__max_count))
            self.__cur_count = self.__max_count

        for i in range(self.__cur_count):
            self.__structs[i].Parse(csv_row, row_number, region)

    def Marshal(self, variable_stream):
        if not ExportToClient(self.export_mask):
            return

        ParseLog.Log('marshal struct array: {}, count: {} '.format(self.name, self.__cur_count))
        ParseLog.IncreaseIndent()
        new_stream = io.BytesIO()
        new_stream.write(self.__cur_count.to_bytes(2, byteorder="big", signed=False))

        new_data_stream = io.BytesIO()

        for i in range(self.__cur_count):
            offset = len(new_data_stream.getbuffer()) + 2 * self.__cur_count + 2
            new_stream.write(offset.to_bytes(2, byteorder="big", signed=False))

            new_fixed_stream, new_optional_stream, new_variable_meta, new_variable_stream = io.BytesIO(), io.BytesIO(), io.BytesIO(), io.BytesIO()
            self.__structs[i].Marshal(new_fixed_stream, new_optional_stream, new_variable_meta, new_variable_stream)
            struct_stream = ParsingEnv.MergeStream(new_fixed_stream, new_optional_stream, new_variable_meta, new_variable_stream)
            ParsingEnv.UpdateStreamingStringOffset(struct_stream, new_data_stream)
            new_data_stream.write(struct_stream.getbuffer())

        ParseLog.DecreaseIndent()

        ParsingEnv.UpdateStreamingStringOffset(new_data_stream, new_stream)
        new_stream.write(new_data_stream.getbuffer())
        ParsingEnv.UpdateStreamingStringOffset(new_stream, variable_stream)
        variable_stream.write(new_stream.getbuffer())

    def GenLua(self, indent):
        if not ExportToClient(self.export_mask):
            return ''

        return self.__structs[0].GenLua(indent) + '\t\t\t\"{}\",\n'.format(self.name)

    def GenCSharp(self, indent, variable_attr_index):
        if not ExportToClient(self.export_mask):
            return ''

        return GenIndent(indent) + 'public StructArray<InternalType_{}> {}\n'.format(self.name, self.name)\
            + GenIndent(indent) + '{\n'\
            + GenIndent(indent + 1) + 'get\n' \
            + GenIndent(indent + 1) + '{\n'\
            + GenIndent(indent + 2) + "var tmp = new StructArray<InternalType_{}>();\n".format(self.name)\
            + GenIndent(indent + 2) + "tmp.Init(GData.GetVariableAttributePtr(m_dataPtr, {}));\n".format(variable_attr_index) \
            + GenIndent(indent + 2) + "return tmp;\n" \
            + GenIndent(indent + 1) + '}\n' \
            + GenIndent(indent) + '}\n\n' \
            + self.__structs[0].GenCSharp(indent, 'InternalType_{}'.format(self.name))


    def GenProto(self, indent, index):
        if not ExportToServer(self.export_mask):
            return '', index 

        comment = ParsingEnv.GetCommentRow()[self.__start_col + 1]
        return self.__structs[0].GenProtoInnerStruct(self.name, indent, comment) + GenIndent(indent) + 'repeated InternalType_{} {} = {};\n'.format(self.name, self.name, index), index + 1
    
    def MarshalPB(self, key_index, binary_stream):
        if not ExportToServer(self.export_mask):
            return key_index
        
        #if self.__cur_count == 0:
            #return key_index + 1 # 长度为0的repeated不产生数据
        
        for i in range(self.__cur_count):
            bs = self.__structs[i].MarshalPBInner() #repeated共用同一个key_index
            b = bs.getbuffer()

            key_encoding(binary_stream, key_index, 2)
            varint_encoding(binary_stream, len(b))
            binary_stream.write(b)

        return key_index + 1

    def GenClientDesc(self, relative_offset, indent):
        if not ExportToClient(self.export_mask):
            return '', relative_offset

        desc = GenIndent(indent) +  'name : ' + self.name + '\n' \
            + GenIndent(indent) + 'relative offset: ' + str(relative_offset)

        return desc, relative_offset + self.GetSize(MASK_EXRORT_TO_CLIENT)

'''
不支持直接嵌套struct， 只支持struct - repeated - struct
'''
class Struct:
    def __init__(self):
        self.__attribute_list = [] #optional, required, repeatedvaluetype, repeatedstruct
        self.name = "Empty"
        self.lua_str = ""
        self.csharp_str = ''
        self.export_mask = 0
    
    def SetName(self, name):
        self.name = name
    
    def SetExportMask(self, mask):
        self.export_mask = mask

    def AppendAttribute(self, attri):
        self.__attribute_list.append(attri)
    
    def GetAttri(self, index):
        return self.__attribute_list[index]

    def GetAllAttri(self):
        return self.__attribute_list

    def Parse(self, csv_row, row_number, region):
        for attri in self.__attribute_list:
            attri.Parse(csv_row, row_number, region)

    def GetSize(self, export_mask):
        if self.export_mask != 0 and (self.export_mask & export_mask) == 0:
            return 0

        total_size = 0
        for attr in self.__attribute_list:
            total_size += attr.GetSize(export_mask)
        return total_size
            
    def GenLua(self, indent):
        if not ExportToClient(self.export_mask):
            return ''

        if self.name != "Empty": #说明是root struct，也就是配置文件中的Config
            self.lua_str = GenIndent(indent) +  'Config \"%s\" {\n'% ("dataconfig_" + self.name.lower())
        else:
            self.lua_str = GenIndent(indent) +  'Struct_Array {\n'

        for attr in self.__attribute_list:
            self.lua_str += attr.GenLua(indent + 1)
        self.lua_str += GenIndent(indent) + '}'
        if self.name != "Empty":
            self.lua_str += "\n\n\n"
        return self.lua_str
        
    def GenCSharp(self, indent, inner_struct_name = ''):
        if not ExportToClient(self.export_mask):
            return ''

        if self.name != 'Empty':
            self.__GenConfig()
        else:
            self.__GenInnerStruct(indent, inner_struct_name)

        fixed_head_size = 2
        fixed_offset, variable_attr_index = fixed_head_size, 0
        for attr in self.__attribute_list:
            if self.__IsVariableAttribute(attr) and ExportToClient(attr.export_mask):
                self.csharp_str += attr.GenCSharp(indent + 1, variable_attr_index)
                variable_attr_index += 1
            else:
                self.csharp_str += attr.GenCSharp(indent + 1, fixed_offset)
                fixed_offset += attr.GetSize(0x1)

        self.csharp_str += GenIndent(indent) + '}\n\n'


        if self.name != 'Empty':
            self.csharp_str +=  self.__GenCSharpArray()
            self.csharp_str += '\n}' #终结 namespace

        return self.csharp_str

    #为了使自动生成的C#代码能够兼容兼容旧逻辑(XXXXXXArray.items)
    def __GenCSharpArray(self):
        array_str = 'public class %sArray \n{\n'% (self.name) \
            + GenIndent(1) + 'public {}Array(List<{}> data)\n'.format(self.name, self.name) \
            + GenIndent(1) + '{\n' \
            + GenIndent(2) + 'items = data;\n'\
            + GenIndent(1) + '}\n\n' \
            + GenIndent(1) + 'public readonly List<{}> items;\n'.format(self.name)\
            + '}'
        return array_str
    
    def GenProtoInnerStruct(self, repeated_name, indent, comment):
        if not ExportToServer(self.export_mask):
            return ''

        message_str = GenComment(comment, indent)
        message_str += GenIndent(indent) +  'message InternalType_%s{\n'%(repeated_name)
        proto_index = 1
        for i in range(len(self.__attribute_list)):
            s, proto_index = self.__attribute_list[i].GenProto(indent + 1, proto_index)
            message_str += s
        message_str += GenIndent(indent) + '}\n\n'

        return message_str

    '''
        用于root struct产生message
    '''
    def GenMessage(self):
        message_str = '/**\n* @file:   dataconfig_{}.proto\n* @brief: ----auto generate----\n*/\n\npackage dataconfig;\n\n'.format(self.name.lower()) 

        message_str += 'message %s{\n' % (self.name)
        proto_index = 1
        for i in range(len(self.__attribute_list)):
            s, proto_index = self.__attribute_list[i].GenProto(1, proto_index)
            message_str += s
        message_str += '}\n\n' \
            + 'message %sArray {\n'%(self.name) \
            + GenIndent(1) + 'repeated {} items = 1;\n'.format(self.name) \
            + '}\n'

        return message_str
        
        
    def __GenInnerStruct(self, indent, inner_struct_name):
        csharp_name = '{} : ConfigInit'.format(inner_struct_name)    
        self.csharp_str = \
            GenIndent(indent) + 'public struct %s\n'%(csharp_name) \
            + GenIndent(indent) + '{\n'\
            + GenIndent(indent + 1) + 'public static %s DummyObj = new %s();\n' %(inner_struct_name, inner_struct_name) \
            + GenIndent(indent + 1) + 'private IntPtr m_dataPtr;\n'\
            + GenIndent(indent + 1) + 'public void Init(IntPtr dataPtr)\n'\
            + GenIndent(indent + 1) + '{\n' \
            + GenIndent(indent + 2) + 'm_dataPtr = dataPtr;\n' \
            + GenIndent(indent + 1) + '}\n\n'\
            + GenIndent(indent + 1) + 'public bool Invalid()\n' \
            + GenIndent(indent + 1) + '{\n' \
            + GenIndent(indent + 2) + 'return m_dataPtr == IntPtr.Zero;\n' \
            + GenIndent(indent + 1) + '}\n\n'\

    def __GenConfig(self):
        self.csharp_str = '//----auto generate----\n\n using System;\nusing  System.Collections.Generic;\n\nnamespace dataconfig{\n'
        self.csharp_str += 'public partial struct %s \n{\n'% (self.name) \
            + GenIndent(1) + 'public static %s DummyObj = new %s();\n' %(self.name, self.name) \
            + GenIndent(1) + 'public static Func<IntPtr, {}> Ctor = Creator;\n'.format(self.name)\
            + GenIndent(1) + 'private IntPtr m_dataPtr;\n' \
            + GenIndent(1) + 'public {}(IntPtr dataPtr)\n'.format(self.name) \
            + GenIndent(1) + '{\n' \
            + GenIndent(2) + 'm_dataPtr = dataPtr;\n\n'\
            + GenIndent(1) + '}\n' \
            + GenIndent(1) + 'public static {} Creator(IntPtr dataPtr)\n'.format(self.name) \
            + GenIndent(1) + '{\n' \
            + GenIndent(2) + 'return new {}(dataPtr);\n'.format(self.name)\
            + GenIndent(1) + '}\n\n' \
            + GenIndent(1) + 'public bool Invalid()\n' \
            + GenIndent(1) + '{\n' \
            + GenIndent(2) + 'return m_dataPtr == IntPtr.Zero;\n' \
            + GenIndent(1) + '}\n\n'\
    
    def __IsVariableAttribute(self, attri):
        return isinstance(attri, RepeatedStruct) or isinstance(attri, RepeatedValueType)
    
    def __IsFixedAttribute(self, attri):
        return isinstance(attri, Optional) or isinstance(attri, Required)

    def Marshal(self, fixed_stream, optional_stream, variable_meta, variable_stream):
        if not ExportToClient(self.export_mask):
            return 

        variable_attri_count = 0
        for attri in self.__attribute_list:
            if self.__IsVariableAttribute(attri) and ExportToClient(attri.export_mask):
                variable_attri_count += 1

        variable_meta.write(variable_attri_count.to_bytes(2, byteorder='big', signed=False))

        for attri in self.__attribute_list:
            if isinstance(attri, Required):
                attri.Marshal(fixed_stream)
            elif isinstance(attri, Optional):
                attri.Marshal(fixed_stream, optional_stream)
            elif self.__IsVariableAttribute(attri):
                if ExportToClient(attri.export_mask):
                    variable_meta.write(len(variable_stream.getbuffer()).to_bytes(2, byteorder='big', signed=False))
                    attri.Marshal(variable_stream)
            else:
                assert(False)
        ParsingEnv.FinishMarshalOptionalData(optional_stream)
    
    def MarshalPBInner(self):
        binary_stream = io.BytesIO()
        proto_index = 1
        for i in range(len(self.__attribute_list)):
            proto_index = self.__attribute_list[i].MarshalPB(proto_index, binary_stream)
        return binary_stream
    
    '''
        用于root struct产生message
    '''
    def MarshalMessage(self, binary_stream):
        proto_index = 1
        for i in range(len(self.__attribute_list)):
            proto_index = self.__attribute_list[i].MarshalPB(proto_index, binary_stream)
        
    #for debug
    def GenClientDesc(self, relative_offset, indent):
        if not ExportToClient(self.export_mask):
            return '', relative_offset

        s = ''
        for attri in self.__attribute_list:
            desc, relative_offset = attri.GenClientDesc(relative_offset, indent + 1)
            s += desc + '\n'

        s += GenIndent(indent) + ' size : ' + str(relative_offset)
        return s