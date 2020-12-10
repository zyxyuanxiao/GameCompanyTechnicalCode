#!/usr/bin/python
#coding=utf-8

##
# @file:   XlsxToCsv.py
# @author: yuexianliu
# @brief:  批量修改xlsx文件为csv的工具
##
import sys
import os

path = os.getcwd() + '/Windows/py3-Tools/3rd/'
sys.path.append(path)

import xlrd
import os
import csv
import codecs
import sys
import re

EXCEL_PATH = os.getcwd() + "/DataConfig/"


#检验是否含有中文字符
def is_contains_chinese(strs):
    for _char in strs:
        if '\u4e00' <= _char <= '\u9fa5':
            return True
    return False

#将xlsx文件装换为csv
def xlsx_to_csv(excelPath, csvPath):
	print("转 CSV,路径为:" + os.path.abspath(csvPath))
	workbook = xlrd.open_workbook(excelPath, encoding_override='utf-8')
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
		csvFileName = csvPath.encode('gbk').decode('gbk') + "/" + sheet.name + ".csv"
		with codecs.open(csvFileName, 'w', encoding='utf-8') as csvFile:
			write = csv.writer(csvFile)
			for row_num in range(sheet.nrows):
				
				data = []
				for col_num in range(sheet.ncols):
					try:
						col = str(sheet.cell_value(row_num, col_num))
						#将所有的 float 转换为 FIntOfFloat
						if col == "float":
							col = "fintoffloat"
												
						if str(sheet.cell_value(2, col_num)) == "fintoffloat" and row_num >= 5:
							#这个地方按照定点数的规则进行转换,float类型是 * 4294967296 存储,然后 /4294967295 再取出.
							#如果定点数改动了,这个地方的代码也需要跟随改动
								if col.find(";") >= 0:
									strList = col.split(";")
									col = ""
									for v in strList:
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

def read_One_file():
	csvPath = sys.argv[1]
	name = csvPath.split("\\")
	name = [x for x in name if x]
	name = name[len(name) - 1]
	excelPath = csvPath + name + ".xlsx"
	xlsx_to_csv(excelPath, csvPath)
	print(name + ".xlsx 转成  " + name +".csv 结束=======================================================")

# mian
if __name__ == '__main__':
	read_One_file()

