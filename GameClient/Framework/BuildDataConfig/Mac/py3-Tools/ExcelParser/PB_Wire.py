import io
import math

'''
    只支持有限的pb的数据类型（像是浮点数和枚举在打表中都没用到)
    wire type:
    0	Varint	int32, int64, uint32, uint64, sint32, sint64, bool
    2	Length-delimited	string, bytes, embedded messages, packed repeated fields
'''

'''
    对int64, int32类型进行转化
'''
def TransferInt(number, offset):
    if number >= 0:
        return number
    else:
        return number + (1 << offset)

'''
    对sint32进行编码
'''
def ZigZag_Sint32(number):
    return (number << 1) ^ (number >> 31)

'''
    对sint64进行编码
'''
def ZigZag_Sint64(number):
    return (number << 1) ^ (number >> 63)

#varint wire type:0
def key_encoding(b, key_index, wire_type):
    key_value = key_index << 3 | wire_type
    varint_encoding(b, key_value)

def varint_encoding(b, number):
    while number >= 0x80:
        remainder = number % 0x80
        remainder |= 0x80
        b.write(remainder.to_bytes(1, byteorder='big',signed = False) )
        number >>= 7
    b.write(number.to_bytes(1, byteorder='big',signed=False))

def varint_decoding(b, i):
    value = 0
    while (b[i] & 0x80) == 0x80:
        value += b[i] &0x7F * (0x80 ** i)
        i += 1

    value += b[i] * (0x80 ** i)
    return value, i


