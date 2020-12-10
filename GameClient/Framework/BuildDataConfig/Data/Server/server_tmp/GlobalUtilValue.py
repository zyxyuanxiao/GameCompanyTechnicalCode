#! /usr/bin/env python
# coding=utf-8

# @file:   GlobalUtil.py
# @author: Roshan（修改yuexianliu）
# @brief:  打表工具拓展,支持国际化多语言支持,从中文表里读取国际化文本（变成数值差异表结构一样）

import xlrd
import sys

bestFilePath = unicode(r"..\..\..\DataConfig\Common\数值差异表.xlsx", "utf-8")
bestConfigFilePath = unicode(r"..\..\..\DataConfig\Common\数值差异目录表.xlsx", "utf-8")
bestDic = None
bestConfigDic = {}
repeList = []

# 计数相关
count_best_lost = 0
count_best_empty = 0
count_source_empty = 0

# 读国际化配置
localConfig = None
# for line in open(r"..\GlobalUtils\Config.txt"):
#     if line is not None and len(line) > 0:
#         localConfig = line
#         print("localConfig========> " + localConfig)

# --------------------------------------------------- Log start --------------------------------------------------#
#txtName = '..\\GlobalUtils\\GlobalUtils.log'
#f = file(txtName, "w+")

# 简单输出日志
def logOutput(message):
    print(message.decode('utf-8').encode(sys.getfilesystemencoding()))
    #f.write(message + "\n")


def closeLog():
    #f.close()
    # 计数日志输出
    with open(txtName, mode='r+') as f1:
        content = f1.read() 
        f1.seek(0,0)
        # 策划提的坑需求，没有错误连日志也不想输出
        if count_best_lost > 0:
            f1.write("<======TOTAL========> [ERROR]can not find key :  " + str(count_best_lost) + "\n")
        if count_best_empty > 0:
            f1.write("<======TOTAL========> [ERROR] value is None while translating ： " + str(count_best_empty) + "\n")
        if count_source_empty > 0:
            f1.write("<======TOTAL========> [WARNING] fieldValue is null , skip it : " + str(count_source_empty) + "\n\n")
        f1.write(content)
    



# --------------------------------------------------- Log end --------------------------------------------------#

# Best表结构
class BestItem(object):
    def __init__(self, key, excelName, sheetName, fieldName, id):
        self.key = key
        self.excelName = excelName
        self.sheetName = sheetName
        self.fieldName = fieldName
        self.id = id

    def SetLocalLanguage(self, local, sheet, row_num):
        if str(local) == str('china'):
            self.chinese = sheet.cell_value(row_num, 5)
        elif str(local) == str('taiwan'):
            self.taiwan = sheet.cell_value(row_num, 6)
        elif str(local) == str('korea'):
            self.korea = sheet.cell_value(row_num, 7)
        elif str(local) == str('vietnam'):
            self.vietnam = sheet.cell_value(row_num, 9)
        elif str(local) == str('japan'):
            self.japan = sheet.cell_value(row_num, 8)
        elif str(local) == str('english'):
            self.english = sheet.cell_value(row_num, 10)
        elif str(local) == str('thailand'):
            self.thailand = sheet.cell_value(row_num, 11)


# 1-Best读取到dic
def ReadBestFile(sheet_name,local):
    localConfig = local

    excelDic = {}
    if localConfig is None or localConfig == "china":
        logOutput("<============= localConfig is None ==========>")
        return excelDic

    ReadBestConfig()
    if (sheet_name is not None) and (not bestConfigDic.has_key(sheet_name)):
       logOutput("<============= not in Config ==========>")
       return excelDic

    logOutput("<============= localConfig is " + localConfig +"==========>")
    try:
        excel_file = xlrd.open_workbook(bestFilePath)
        sheet = excel_file.sheet_by_name("ValueDiff")
    except Exception as e:
        logOutput("<============= Read best.xlsx failed ==========>")
        print e
        return
    # 行数和列数
    row_count = len(sheet.col_values(0))
    col_count = len(sheet.row_values(0))
    # logOutput("<==================== sheet_name is " + str(sheet_name) + " ====================> ")
    # logOutput("Best File row_count =>" + str(row_count))
    # logOutput("Best File col_count =>" + str(col_count))
    for row_num in range(1, row_count):
        key = sheet.cell_value(row_num, 0)
        excelName = sheet.cell_value(row_num, 1)
        sheetName = sheet.cell_value(row_num, 2)
        fieldName = sheet.cell_value(row_num, 3)
        id = sheet.cell_value(row_num, 4)
        # 只把这个sheet内容放入内存
        if str(sheet_name) == sheetName:
            bestItem = BestItem(key, excelName, sheetName, fieldName, id)
            bestItem.SetLocalLanguage(localConfig, sheet, row_num)
            if (excelName is not None) and (not excelDic.has_key(key)):
                excelDic[key] = bestItem

    ReadBestConfig()
    return excelDic

# 读取BestConfig语言包整理表
def ReadBestConfig():
    try:
        excel_file = xlrd.open_workbook(bestConfigFilePath)
        sheet = excel_file.sheet_by_name("Sheet1")
    except Exception as e:
        logOutput("<============= ReadBestConfig failed ==========>")
        print e
        return
    # 行数和列数
    row_count = len(sheet.col_values(0))
    col_count = len(sheet.row_values(0))
    # logOutput("Best Config File row_count =>" + str(row_count))
    # logOutput("Best Config File col_count =>" + str(col_count))
    for row_num in range(1, row_count):
        sheetName = sheet.cell_value(row_num, 1)
        if (sheetName is not None) and (not bestConfigDic.has_key(sheetName)):
                bestConfigDic[sheetName] = []

        for col_num in range(2, col_count):
            if bestConfigDic[sheetName] is not None:
                value = sheet.cell_value(row_num, col_num)
                if value is not None and len(value) > 0:
                    bestConfigDic[sheetName].append(value)
                    # logOutput("sheet.cell_value(row_num, col_num)=============>"+sheet.cell_value(row_num, col_num))


def GetBestValueByLocal(local, item):
    if str(local) == str('chinese'):
        return item.chinese
    elif str(local) == str('taiwan'):
        return item.taiwan
    elif str(local) == str('korea'):
        return item.korea
    elif str(local) == str('vietnam'):
        return item.vietnam
    elif str(local) == str('japan'):
        return item.japan
    elif str(local) == str('english'):
        return item.english
    elif str(local) == str('thailand'):
        return item.thailand
    else:
        return None


# 从best表找到对应文字替换
def DoGlobalTranslate(fieldValue, sourceCsv, row, col , csvName, local):
    localConfig = local
    if localConfig is None or len(bestDic) == 0 :
        return None

    if fieldValue is None or len(fieldValue) <= 0:
        global count_source_empty 
        count_source_empty += 1
        #logOutput("[WARNING] fieldValue is null , skip it ,id => " +str(int(sourceCsv[row][0])) + "field => "+ str(sourceCsv[3][col]))
        return None
    else:
        # 读取id 和 sheet名和field名， 组成key
        id = str(int(float(sourceCsv[row][0])))
        sheetName = str(csvName)
        fieldName = str(sourceCsv[3][col])

        findKey1 = id + '_' + sheetName + '_' + fieldName

        # 对传递过来的表判断是否是repeated的结构嵌套类型
        if repeList == None or len(repeList) == 0:
            col_count = len(sourceCsv[0])
            for col_num in range(1, col_count):
                structType = sourceCsv[1][col_num]
                if str(structType) == "repeated":
                    if col_num < col_count - 1:
                        structType2 = sourceCsv[1][col_num + 1]
                        if str(structType2) == "optional_struct" or str(structType2) == "required_struct":
                            repeList.append(col_num)
        
        listNum = 0
        inListNum = 0
        isRepeated = False
        for num_i in range(len(repeList)):
            repeNum = sourceCsv[row][repeList[num_i]]
            if repeNum == None or str(repeNum) == "":
                continue

            rfDistance = sourceCsv[2][repeList[num_i] + 1]
            if rfDistance == None or str(rfDistance) == "":
                continue

            for num_j in range(int(repeNum)):
                minPos = int(repeList[num_i]) + 1
                maxPos = int(repeList[num_i]) + 1 + (int(num_j) + 1) * int(rfDistance)
                if col >= minPos and col<= maxPos:
                    listNum = num_i + 1
                    inListNum = num_j + 1
                    isRepeated = True
                    break   

        findKey2 = id + '_' + sheetName + '_' + fieldName + "list" + "_" + str(listNum) + "_" + str(inListNum) # 原表的第几个repeated的第几个值 
        if isRepeated:
            findKey = findKey2
        else:
            findKey = findKey1

        #logOutput("findKey==>"+str(findKey))

        #不用校检
        isRightField = True

        if isRightField == True:
            if bestDic.has_key(findKey) and bestDic[findKey] is not None:
                resultValue = GetBestValueByLocal(localConfig, bestDic[findKey])
                if resultValue is None or resultValue == "":
                    #logOutput("[ERROR] value is None while translating , findKey = " + findKey + " !!!")
                    global count_best_empty 
                    count_best_empty += 1
                    return None
                else:
                    # 一波骚操作，由于读best表是读的ascii,占位符替换要转成utf8. 但写入data时是unicode要decode回去
                    # logOutput("[translate success] ====>" + str(findKey))
                    return str(unicode(resultValue).encode('utf-8')).replace('￥', ' ').decode('utf-8')
            else:               
               #logOutput("[ERROR] can not find key ===> "+ str(findKey))
                global count_best_lost 
                count_best_lost += 1

    return None