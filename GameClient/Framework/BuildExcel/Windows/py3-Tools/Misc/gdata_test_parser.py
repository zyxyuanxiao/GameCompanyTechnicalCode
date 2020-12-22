import io

def ParseBin(file_path):
    with open(file_path, 'rb') as f:
        content = f.read()
        bs = io.BytesIO(content)
        data_entry_count = int.from_bytes(bs.read(4), byteorder='big')
        print("数据条目数量: {}".format(data_entry_count))
        data_offset = int.from_bytes(bs.read(4), byteorder = 'big')
        print('data offset: {}'.format(data_offset))
        entry_size = int.from_bytes(bs.read(4), byteorder = 'big')
        print('data entry size: {}'.format(entry_size))
        string_offset = int.from_bytes(bs.read(4), byteorder = 'big')
        print('string offset:{}'.format(string_offset))
        string_count = int.from_bytes(bs.read(4), byteorder = 'big')
        print('string count:{}'.format(string_count))

        key_col_count = int.from_bytes(bs.read(1), byteorder='big')
        print('key col count: {}'.format(key_col_count))

        bs.seek(data_offset, 0)

        print(bs.read(entry_size))
        print(bs.read(entry_size))

        

if __name__ == "__main__":
    ParseBin("D:/WorkSpace/best2/Code/trunk/BuildDataConfig/Tool/TestOutput/Binary/dataconfig_auctionconfig.bytes")
