# coding=utf-8

##
# @file:   XlsxToCsv.py
# @author: zhuzizheng
# @brief:  批量修改xlsx文件为csv的工具
##
import sys
import os

path = os.getcwd() + '/Mac/py3-Tools/3rd'
sys.path.append(path)
path = os.getcwd() + '/Mac/py3-Tools/'
sys.path.append(path)

import xlrd
import os
import csv
import codecs
import sys
import re
from Misc.Util import *

EXCEL_PATH = os.getcwd() + "/DataConfig/"


# 检验是否含有中文字符
def is_contains_chinese(strs):
    for _char in strs:
        if '\u4e00' <= _char <= '\u9fa5':
            return True
    return False


# 将xlsx文件装换为csv
def xlsx_to_csv(excelPath, csvPath):
    print("转成 CSV,路径:" + os.path.abspath(csvPath))
    workbook = xlrd.open_workbook(excelPath, encoding_override='UTF-8-sig')
    sheets = workbook.nsheets
    value = re.compile(r'^[-+]?[-0-9]\d*\.\d*|[-+]?\.?[0-9]\d*$')
    for i in range(sheets):
        sheet = workbook.sheet_by_index(i)
        # 如果sheet名是//开头不转化成csv
        if sheet.name.startswith('#'):
            continue
        # 如果sheet名是包含sheet字段的不给打表
        if sheet.name.lower().find("sheet") >= 0:
            continue
        # 如果sheet名是汉字开头不转化成csv
        if is_contains_chinese(sheet.name):
            continue
        # 如果当前的 Excel 表中的数据为空,或者前 6 行没有数据,前 2 列没有数据,则不能进行转表
        if sheet.nrows < 6 or sheet.ncols < 2:
            continue
        csvFileName = csvPath + "/" + sheet.name + ".csv"
        with codecs.open(csvFileName, 'w', encoding='UTF-8-sig') as csvFile:
            write = csv.writer(csvFile)
            for row_num in range(sheet.nrows):

                data = []
                for col_num in range(sheet.ncols):
                    try:
                        col = str(sheet.cell_value(row_num, col_num))

                        if str(sheet.cell_value(2, col_num)) == "float" and row_num >= 5:
                            # 这个地方按照定点数的规则进行转换,float类型是 * 4294967296 存储,然后 /4294967295 再取出.
                            # 如果定点数改动了,这个地方的代码也需要跟随改动
                            if col.find(";") >= 0:
                                strList = col.split(";")
                                col = ""
                                for v in strList:
                                    if v != "":
                                        col = col + str((int)(float(v) * 4294967296)) + ";"
                            else:
                                if len(col) <= 0:
                                    col = 0
                                col = (int)(float(col) * 4294967296)
                        else:
                            result = value.match(col)
                            if col.endswith(".0") and result:
                                col = col.replace(".0", "")

                        data.append(col)
                    except IndexError:
                        pass
                write.writerow(data)


# 读取DataConfig目录下的所有xlsx文件
def read_all_file():
    # 记录当前文件夹内的 csv 文件,如果多余打表之后的 csv 文件,就需要删除多余的文件内容
    old_all_files = list(set(GetDirFiles(str(EXCEL_PATH))))
    old_all_csv_files = [x.replace('\\', '/').replace('//', '/') for x in old_all_files if x.split('.')[-1] == 'csv']
    old_all_csv_names = [str(x.split('/')[-1].replace('.csv', '')).lower() for x in old_all_csv_files]
    # 删除所有的 csv 的文件
    [os.remove(x) for x in old_all_csv_files]

    list_dirs = os.walk(EXCEL_PATH)
    for root, dirs, files in list_dirs:
        for f in files:
            if f.endswith(".xlsx"):
                excelPath = os.path.join(root, f)
                # 不让 Excel 的 缓存文件进入监测表格的代码里面
                if "$" in excelPath:
                    continue
                if "~" in excelPath:
                    continue
                if "@" in excelPath:
                    continue
                if "#" in excelPath:
                    continue
                if f.endswith("翻译表.xlsx") or f.endswith("翻译目录表.xlsx") or f.endswith("数值差异表.xlsx") or f.endswith(
                        "数值差异目录表.xlsx"):
                    continue
                csvPath = excelPath.replace('.xlsx', '')
                csvPath = os.path.dirname(csvPath)  # 将数据变为统一目录
                if not os.path.exists(csvPath):
                    os.mkdir(csvPath)

                xlsx_to_csv(excelPath, csvPath)

    new_all_files = list(set(GetDirFiles(EXCEL_PATH)))
    new_all_csv_files = [x.replace('\\', '/').replace('//', '/') for x in new_all_files if x.split('.')[-1] == 'csv']
    new_all_csv_names = [str(x.split('/')[-1].replace('.csv', '')).lower() for x in new_all_csv_files]
    # 需要删除的数据包括bytes文件,lua文件,csharp 文件,根据这个名字得到
    delete_names = list([name for name in old_all_csv_names if name not in new_all_csv_names])
    for name in delete_names:
        delete_all_data(str(name).lower())
        print("删除了" + name + "的 cs/lua/bytes等数据")


# def read_One_file():
#     # excelPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/1V1兵线参数配置表/1V1兵线参数配置表.xlsx"
#     # csvPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/1V1兵线参数配置表/"
#     excelPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/hd_活动配置表/hd_活动配置表.xlsx"
#     csvPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/hd_活动配置表/"
#     xlsx_to_csv(excelPath, csvPath)
#     print("单个 xlsx 转成 csv 结束=======================================================")

def read_One_file():
    csv_Path = str(sys.argv[1])
    name = csv_Path.split("/")
    name = name[len(name) - 1]
    excelPath = csv_Path + "/" + name + ".xlsx"
    # 不让 Excel 的 缓存文件进入监测表格的代码里面
    if "$" in excelPath:
        return
    if "~" in excelPath:
        return
    if "@" in excelPath:
        return
    if "#" in excelPath:
        return
    # 记录当前文件夹内的 csv 文件,如果多余打表之后的 csv 文件,就需要删除多余的文件内容
    old_all_files = list(set(GetDirFiles(str(sys.argv[1]))))
    old_all_csv_files = [x.replace('\\', '/').replace('//', '/') for x in old_all_files if x.split('.')[-1] == 'csv']
    old_all_csv_names = [str(x.split('/')[-1].replace('.csv', '')).lower() for x in old_all_csv_files]
    # 删除所有的 csv 的文件
    [os.remove(x) for x in old_all_csv_files]

    xlsx_to_csv(excelPath, csv_Path)

    new_all_files = list(set(GetDirFiles(str(sys.argv[1]))))
    new_all_csv_files = [x.replace('\\', '/').replace('//', '/') for x in new_all_files if x.split('.')[-1] == 'csv']
    new_all_csv_names = [str(x.split('/')[-1].replace('.csv', '')).lower() for x in new_all_csv_files]
    # 需要删除的数据包括bytes文件,lua文件,csharp 文件,根据这个名字得到
    delete_names = list([name for name in old_all_csv_names if name not in new_all_csv_names])
    for name in delete_names:
        delete_all_data(str(name).lower())
        print("删除了" + name + "的 cs/lua/bytes等数据")


def delete_all_data(name):
    path = os.path.dirname(os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/pbconf_res/"
    bytes_path = path + 'dataconfig_{}.bytes'.format(name)
    if os.path.isfile(bytes_path):
        os.remove(bytes_path)
    path = os.path.dirname(os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/ProtoGen/"
    cs_path = path + name + ".cs"
    if os.path.isfile(cs_path):
        os.remove(cs_path)
    path = os.path.dirname(
        os.getcwd()) + "/Assets/Extensions/Protobuf/protobuf_config/pbconf_res/gdata_bytes_list.bytes"
    dataconfig_name = 'dataconfig_{}.bytes'.format(name)
    with open(path, 'r+') as bytes_list_file:
        str_bytes = bytes_list_file.read()
        if "False" not in str_bytes:
            str_bytes = "False" + "\n" + str_bytes
        if dataconfig_name in str_bytes:
            str_bytes = str_bytes.replace(dataconfig_name, "")
            str_bytes = str_bytes.strip()
            str_bytes = str_bytes.replace("\n\n", "\n")
            bytes_list_file.seek(0)
            bytes_list_file.truncate()
            bytes_list_file.write(str_bytes)
    path = os.path.dirname(os.getcwd()) + "/Assets/lua/protobuf_conf_parser/config.lua"
    with open(path, "r+", encoding="utf8") as lua_file:
        delimiter = "---------" + name + "---------"
        allContent = lua_file.read()
        if delimiter in allContent:
            allListContent = str(allContent).split(delimiter)
            del allListContent[1]
            allContent = "".join(allListContent)
            allContent = allContent.replace("\n\n\n\n", "\n\n")
            lua_file.seek(0)
            lua_file.truncate()
            lua_file.write(allContent)


# mian
if __name__ == '__main__':
    if len(sys.argv) >= 2 and os.getcwd() in sys.argv[1]:
        read_One_file()
    else:
        read_all_file()
    print("====================xlsx 转成 csv 结束=======================================================")

# read_One_file()
