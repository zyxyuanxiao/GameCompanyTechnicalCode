from .Env import StringMgr, ParsingEnv
from .PB_Wire import *
from Misc.Util import *
import re

gDecimalPartner = re.compile("^\d+\.\d+$")

class ValueObj:
    def __init__(self):
        self.row = 0
        self.col = 0
    
    def SetPos(self, row, col):
        self.row = row
        self.col = col

class Number(ValueObj):
    def __init__(self, signed, size):
        self.value = 0
        self.signed = signed
        self.size = size

    def Parse(self, string):
        if len(string) == 0:
            self.value = 0
            return

        if '.' in string:
            string = string.split('.')[0]
        self.value = int(string)

    def Marshal(self, binary_stream):
        binary_stream.write(self.value.to_bytes(self.size, byteorder="big", signed=self.signed))
        return self.size
    
    def MarshalPB(self, key_index, binary_stream):
        raise NotImplementedError

class UInt32(Number):
    def __init__(self):
        Number.__init__(self, False, 4)
    
    @staticmethod
    def GetSize():
        return 4

    @staticmethod
    def GenLua():
        return "UINT32"

    @staticmethod
    def GenCSharpTypeStr():
        return "UInt32"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadUInt32"

    @staticmethod
    def GenProto():
        return "uint32"

    @staticmethod
    def GenProtoDefault():
        return "0"

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, self.value)

class SInt32(Number):
    def __init__(self):
        Number.__init__(self, True, 4)
    
    @staticmethod
    def GetSize():
        return 4

    @staticmethod
    def GenLua():
        return "SINT32"

    @staticmethod
    def GenCSharpTypeStr():
        return "Int32"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadSInt32"

    @staticmethod
    def GenProto():
        return "sint32"

    @staticmethod
    def GenProtoDefault():
        return "0"

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, ZigZag_Sint32(self.value))

class Int32(Number):
    def __init__(self):
        Number.__init__(self, True, 4)

    @staticmethod
    def GetSize():
        return 4

    @staticmethod
    def GenLua():
        return "SINT32"

    @staticmethod
    def GenCSharpTypeStr():
        return "Int32"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadSInt32"

    @staticmethod
    def GenProto():
        return "int32"

    @staticmethod
    def GenProtoDefault():
        return "0"

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, TransferInt(self.value, 64)) #为啥pb里的int32的负数是按照64位编码？？？


class UInt64(Number):
    def __init__(self):
        Number.__init__(self, False, 8)

    @staticmethod
    def GetSize():
        return 8

    @staticmethod
    def GenLua():
        return "UINT64"

    @staticmethod
    def GenCSharpTypeStr():
        return "UInt64"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadUInt64"

    @staticmethod
    def GenProto():
        return "uint64"

    @staticmethod
    def GenProtoDefault():
        return "0"

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, self.value)

class SInt64(Number):
    def __init__(self):
        Number.__init__(self, True, 8)

    @staticmethod
    def GetSize():
        return 8

    @staticmethod
    def GenLua():
        return "SINT64"

    @staticmethod
    def GenCSharpTypeStr():
        return "Int64"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadSInt64"

    @staticmethod
    def GenProto():
        return "sint64"

    @staticmethod
    def GenProtoDefault():
        return "0"
        
    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, ZigZag_Sint64(self.value))

class Int64(Number):
    def __init__(self):
        Number.__init__(self, True, 8)

    @staticmethod
    def GetSize():
        return 8

    @staticmethod
    def GenLua():
        return "SINT64"

    @staticmethod
    def GenCSharpTypeStr():
        return "Int64"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadSInt64"

    @staticmethod
    def GenProto():
        return "int64"

    @staticmethod
    def GenProtoDefault():
        return "0"

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, TransferInt(self.value, 64))

class Float(Number):
    def __init__(self):
        Number.__init__(self, True, 8)

    @staticmethod
    def GetSize():
        return 8

    @staticmethod
    def GenLua():
        return "FLOAT"

    @staticmethod
    def GenCSharpTypeStr():
        return "float"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadFloat"

    @staticmethod
    def GenProto():
        return "float"

    @staticmethod
    def GenProtoDefault():
        return "0"

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, TransferInt(self.value, 64))



class Bool(ValueObj):
    def __init__(self):
        self.value = 0
        self.signed = False
    
    @staticmethod
    def GetSize():
        return 1

    def Parse(self, string):
        string = string.strip().lower()
        if len(string) == 0:
            self.value = 0
            return

        if string == "true":
            self.value = 1
        elif string == "false" or string == 'flase':
            self.value = 0
        else:
            self.value = 1 if int(string) != 0 else 0

    def Marshal(self, binary_stream):
        binary_stream.write(self.value.to_bytes(1, byteorder="big", signed=self.signed))
        return 1

    @staticmethod
    def GenLua():
        return "BOOL"

    @staticmethod
    def GenCSharpTypeStr():
        return "bool"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadBool"

    @staticmethod
    def GenProto():
        return "bool"

    @staticmethod
    def GenProtoDefault():
        return 'false'

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 0)
        varint_encoding(binary_stream, self.value)

class String(ValueObj):
    def __init__(self):
        self.value = 0
        self.string = ''
    
    @staticmethod
    def GetSize():
        return 4

    @staticmethod
    def GenLua():
        return "STRING"

    @staticmethod
    def GenCSharpTypeStr():
        return "string"

    @staticmethod
    def CSharpReaderName():
        return "GData.ReadString"

    @staticmethod
    def GenProto():
        return "string"

    @staticmethod
    def GenProtoDefault():
        return '""'

    def Parse(self, string):
        if len(string) == 0:
            self.value = 0 #string_index为0表示是空字符串
            return

        #if gDecimalPartner.match(string) != None:
           #LogHelp.LogError('配置表： {}, {}行 {}列 string中包含小数点，很可能是格式错误!'.format(ParsingEnv.GetCurSheetPath(), self.row, self.col))

        self.string = string
        self.value = StringMgr.GetStringIndex(string)

    def Marshal(self, binary_stream):
        if self.value != 0:
            StringMgr.AddStreamStringInfo(binary_stream, len(binary_stream.getbuffer()))

        binary_stream.write(self.value.to_bytes(4, byteorder="big", signed=False))
        return 4

    def MarshalPB(self, key_index, binary_stream):
        key_encoding(binary_stream, key_index, 2)

        b = self.string.encode('utf-8')
        varint_encoding(binary_stream, len(b))
        binary_stream.write(b)

class Date(String):
    def Parse(self, string):
        if len(string) == 0:
            self.value = 0 #string_index为0表示是空字符串
            return

        #if gDecimalPartner.match(string) != None:
           #LogHelp.LogError('配置表： {}, {}行 {}列 string中包含小数点，很可能是格式错误!'.format(ParsingEnv.GetCurSheetPath(), self.row, self.col))
       
        if not 'T' in string:
           LogHelp.LogError('配置表： {}, {}行 {}列 date时间格式错误，需要前面T开头'.format(ParsingEnv.GetCurSheetPath(), self.row, self.col))

        self.string = string.replace('T',"")
        self.value = StringMgr.GetStringIndex(self.string)
