#!/usr/bin/python
#coding=utf-8

##
# @file:   XlsxToCsv.py
# @author: yuexianliu
# @brief:  批量修改xlsx文件为csv的工具
##
import sys
import os
path = os.getcwd() + '/Windows/py3-Tools/3rd'
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
	print("转成 CSV,路径:" + os.path.abspath(csvPath))
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
		csvFileName = csvPath.encode('utf-8').decode('utf-8') + "/" + sheet.name + ".csv"
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

#读取DataConfig目录下的所有xlsx文件
def read_all_file ():
	list_dirs = os.walk(EXCEL_PATH)
	for root,dirs,files in list_dirs:
		for f in files:
			if "#" in f:
				continue
			if f.endswith(".xlsx"):
				excelPath = os.path.join(root, f)
				if excelPath.endswith("best.xlsx".encode('utf-8').decode('utf-8')) or excelPath.endswith("语言包整理表.xlsx".encode('utf-8').decode('utf-8')) or excelPath.endswith("国际化对照表.xlsx".encode('utf-8').decode('utf-8')) or excelPath.endswith("数值差异表.xlsx".encode('utf-8').decode('utf-8')) or excelPath.endswith("数值差异整理表.xlsx".encode('utf-8').decode('utf-8')):
					continue
				csvPath = excelPath.replace('.xlsx', '')
				csvPath = os.path.dirname(csvPath) #将数据变为统一目录
				if not os.path.exists(csvPath):
					os.mkdir(csvPath)

				xlsx_to_csv(excelPath, csvPath)

def read_One_file():
	# excelPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/1V1兵线参数配置表/1V1兵线参数配置表.xlsx"
	# csvPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/1V1兵线参数配置表/"
	excelPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/hd_活动配置表/hd_活动配置表.xlsx"
	csvPath = os.path.dirname(os.path.dirname(os.getcwd())) + "/DataConfig/Common/hd_活动配置表/"
	xlsx_to_csv(excelPath, csvPath)
	print("单个 xlsx 转成 csv 结束=======================================================")

# mian
if __name__ == '__main__':
	read_all_file()
	print("全部 xlsx 转成 csv 结束=======================================================")

	# read_One_file()

