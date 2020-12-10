import sys
import os
path = os.getcwd() + '/Windows/py3-Tools/3rd/'
sys.path.append(path)

from xlrd import *
import time

# Best表结构
class BestItem(object):
    def __init__(self, key, excelName, sheetName, fieldName, id):
        self.key = key
        self.excelName = excelName
        self.sheetName = sheetName
        self.fieldName = fieldName
        self.id = id

    def SetLocalLanguage(self, local, sheet, row_num):
        if str(local) == str('China'):
            self.chinese = sheet.cell_value(row_num, 5)
        elif str(local) == str('TaiWan'):
            self.taiwan = sheet.cell_value(row_num, 6)
        elif str(local) == str('Korea'):
            self.korea = sheet.cell_value(row_num, 7)
        elif str(local) == str('Vietnam'):
            self.vietnam = sheet.cell_value(row_num, 9)
        elif str(local) == str('Japan'):
            self.japan = sheet.cell_value(row_num, 8)
        elif str(local) == str('English'):
            self.english = sheet.cell_value(row_num, 10)
        elif str(local) == str('Thailand'):
            self.thailand = sheet.cell_value(row_num, 11)

def logOutput(message):
    print(message)

def GetBestValueByLocal(local, item):
    if str(local) == str('China'):
        return item.chinese
    elif str(local) == str('TaiWan'):
        return item.taiwan
    elif str(local) == str('Korea'):
        return item.korea
    elif str(local) == str('Vietnam'):
        return item.vietnam
    elif str(local) == str('Japan'):
        return item.japan
    elif str(local) == str('English'):
        return item.english
    elif str(local) == str('Thailand'):
        return item.thailand
    else:
        print("invalid local: {}", local)
        return None

class Localization:
    def __init__(self, localization_excel_path, Localization_excel_config_path, log_file_path, is_value_localization):       
        t_begin = time.time()
        self.count_best_lost = 0
        self.count_best_empty = 0
        self.count_source_empty = 0
        self.is_value_localization = is_value_localization
        if log_file_path != None:
            self.log_file = open(log_file_path, 'w+')
        else:
            self.log_file = None
        self.localization_sheet = None
        self.__InitConfigExcel(Localization_excel_config_path)
        self.localization_excel_path = localization_excel_path

        self.repeList = []
        self.sheet_localization_info = dict()

    def __InitLocalizationExcel(self, localization_excel_path):
        try:
            t_begin = time.time()
            path = os.getcwd() + localization_excel_path
            excel_file = open_workbook(path)
            self.localization_sheet = excel_file.sheet_by_name("ValueDiff" if self.is_value_localization else "Best")
            print("打开 {} 耗时：{}".format(localization_excel_path, time.time() - t_begin))
        except Exception as e:
            logOutput("<============= Read {} failed ==========>".format(localization_excel_path))
            print(e)

    def __InitConfigExcel(self, Localization_excel_config_path):
        self.bestConfigDic = dict()
        try:
            excel_file = open_workbook(Localization_excel_config_path)
            sheet = excel_file.sheet_by_name("Sheet1")
        except Exception as e:
            logOutput("<============= ReadBestConfig failed ==========>")
            print(e)
            return

        # 行数和列数
        row_count = len(sheet.col_values(0))
        col_count = len(sheet.row_values(0))
        # logOutput("Best Config File row_count =>" + str(row_count))
        # logOutput("Best Config File col_count =>" + str(col_count))
        for row_num in range(1, row_count):
            sheetName = sheet.cell_value(row_num, 1)
            if (sheetName is not None) and not (sheetName in self.bestConfigDic.keys()):
                    self.bestConfigDic[sheetName] = []
    
            for col_num in range(2, col_count):
                if self.bestConfigDic[sheetName] is not None:
                    value = sheet.cell_value(row_num, col_num)
                    if value is not None and len(value) > 0:
                        self.bestConfigDic[sheetName].append(value)

    def HasLocaliztionInfo(self, sheetName):
        return sheetName in self.bestConfigDic

    def PrepareForParseConfig(self, sheet_name, region):
        if self.localization_sheet == None:
            self.__InitLocalizationExcel(self.localization_excel_path)

        if self.localization_sheet == None or self.bestConfigDic == None:
            logOutput("<============= Init failed, cannot do localization==========>")
            return

        if region == None or region == "china":
            logOutput("<============= localConfig is None ==========>")
            return 
        if (sheet_name is not None) and (sheet_name not in self.bestConfigDic):
            logOutput("<============= not in Config ==========>")
            return 

        #logOutput("<============= region is " + region +"==========>")
        self.sheet_localization_info = dict()
        
        row_count = len(self.localization_sheet.col_values(0))
        #col_count = len(self.localization_sheet.row_values(0))
        # logOutput("<==================== sheet_name is " + str(sheet_name) + " ====================> ")
        # logOutput("Best File row_count =>" + str(row_count))
        # logOutput("Best File col_count =>" + str(col_count))
        for row_num in range(1, row_count):
            key = self.localization_sheet.cell_value(row_num, 0)
            excelName = self.localization_sheet.cell_value(row_num, 1)
            sheetName = self.localization_sheet.cell_value(row_num, 2)
            fieldName = self.localization_sheet.cell_value(row_num, 3)
            id = self.localization_sheet.cell_value(row_num, 4)

            # 只把这个sheet内容放入内存
            if str(sheet_name) == sheetName:
                bestItem = BestItem(key, excelName, sheetName, fieldName, id)
                bestItem.SetLocalLanguage(region, self.localization_sheet, row_num)
                if (excelName is not None) and not(key in self.sheet_localization_info.keys()):
                    self.sheet_localization_info[key] = bestItem

    def DoGlobalTranslate(self, fieldValue, sourceCsv, row, col , csvName, region):
        if region is None or len(self.sheet_localization_info) == 0:
            return None

        if not self.is_value_localization and region == "China":
            return None

        if fieldValue is None or len(fieldValue) <= 0:
            self.count_source_empty += 1
            #logOutput("[WARNING] fieldValue is null , skip it ,id => " +str(int(sourceCsv[row][0])) + "field => "+ str(sourceCsv[3][col]))
            return None
        else:
            # 读取id 和 sheet名和field名， 组成key
            id = str(int(float(sourceCsv[row][0])))
            sheetName = str(csvName)
            fieldName = str(sourceCsv[3][col])

            findKey1 = id + '_' + sheetName + '_' + fieldName

            # 对传递过来的表判断是否是repeated的结构嵌套类型
            self.repeList = []
            col_count = len(sourceCsv[0])
            for col_num in range(1, col_count):
                structType = sourceCsv[1][col_num]
                if str(structType) == "repeated":
                    if col_num < col_count - 1:
                        structType2 = sourceCsv[1][col_num + 1]
                        if str(structType2) == "optional_struct" or str(structType2) == "required_struct":
                            self.repeList.append(col_num)

            listNum = 0
            inListNum = 0
            isRepeated = False
            for num_i in range(len(self.repeList)):
                repeNum = sourceCsv[row][self.repeList[num_i]]
                if repeNum == None or str(repeNum) == "":
                    continue
                 
                rfDistance = sourceCsv[2][self.repeList[num_i] + 1]
                if rfDistance == None or str(rfDistance) == "":
                    continue
                 
                for num_j in range(int(repeNum)):
                    minPos = int(self.repeList[num_i]) + 1
                    maxPos = int(self.repeList[num_i]) + 1 + (int(num_j) + 1) * int(rfDistance)
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

                # 先校验下是否是中文列
                if self.is_value_localization:
                    isRightField = True
                else:
                    isRightField = False
                    if  sheetName in self.bestConfigDic:
                        for v in self.bestConfigDic[sheetName]:
                            if v == fieldName:
                                isRightField = True

                if isRightField == True:
                    if (findKey in self.sheet_localization_info.keys()) and self.sheet_localization_info[findKey] is not None:
                        resultValue = GetBestValueByLocal(region, self.sheet_localization_info[findKey])
                        if resultValue is None or resultValue == "":
                            #logOutput("[ERROR] value is None while translating , findKey = " + findKey + " !!!")
                            self.count_best_empty += 1
                            return None
                        else:
                            # 一波骚操作，由于读best表是读的ascii,占位符替换要转成utf8. 但写入data时是unicode要decode回去
                            # logOutput("[translate success] ====>" + str(findKey))
                            return str(resultValue.encode('utf-8')).replace('￥', ' ')
                    else:               
                        #logOutput("[ERROR] can not find key ===> "+ str(findKey))
                        self.count_best_lost += 1

        return None


    
    def Done(self):
        if self.count_best_lost > 0:
            self.log_file.write("<======TOTAL========> [ERROR]can not find key :  " + str(self.count_best_lost) + "\n")
        if self.count_best_empty > 0:
            self.log_file.write("<======TOTAL========> [ERROR] value is None while translating ： " + str(self.count_best_empty) + "\n")
        if self.count_source_empty > 0:
            self.log_file.write("<======TOTAL========> [WARNING] fieldValue is null , skip it : " + str(self.count_source_empty) + "\n\n")

        if self.log_file:
            self.log_file.close()