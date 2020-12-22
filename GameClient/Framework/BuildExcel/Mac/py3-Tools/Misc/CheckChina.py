# encoding : utf-8

import csv
import os
import sys

csvConfig = [['path', 'csvname']]


def CheckSheetIsChina(checkStr):
    for ch in checkStr.decode('GB18030'):
        if u'\u4e00' <= ch <= u'\u9fff':
            return True
    return False


def CheckIsUseCol(checkArray, j):
    if checkArray[0][j] != "" and "c" in checkArray[0][j].lower():
        flag = True
        for i in range(1, 4):
            if len(checkArray[i][j]) == 0 and checkArray[i][j] == "":
                flag = False
        return flag
    return False


def OutCsv():
    csvPath = "translate.csv"
    with open(csvPath, 'w') as csvFile:
        write = csv.writer(csvFile, lineterminator='\n')
        write.writerows(csvConfig)


def Record(root, file, csvArray, j):
    rootStr = root.split('\\')
    cur_row = len(csvConfig) - 1
    if csvConfig[cur_row][0] == rootStr[len(rootStr) - 1] and csvConfig[cur_row][1] == file:
        for k in range(2, len(csvConfig[cur_row])):
            if csvConfig[cur_row][k] == csvArray[3][j]:
                return
        csvConfig[cur_row].append(csvArray[3][j])
    else:
        cur_row = cur_row + 1
        temp = ["", "", ""]
        csvConfig.append(temp)
        csvConfig[cur_row][0] = rootStr[len(rootStr) - 1]
        csvConfig[cur_row][1] = file
        csvConfig[cur_row][2] = csvArray[3][j]


def ScanChinaInCSV(path):
    if not os.path.exists(path):
        print
        path + " is not path"
    list_dirs = os.walk(path)

    for root, dirs, files in list_dirs:
        for f in files:
            if f.endswith(".csv") and not CheckSheetIsChina(f):
                csvPath = root + "\\" + f
                print
                csvPath
                csvArrays = []
                with open(csvPath, "r") as csvfile:
                    reader = csv.reader(csvfile)
                    for row in reader:
                        csvArrays.append(row)

                row = len(csvArrays)
                if row == 0 or csvArrays[0] == None:
                    continue
                col = len(csvArrays[0])
                for i in range(5, row):
                    for j in range(0, col):
                        try:
                            temp = csvArrays[i][j]
                        except BaseException as e:
                            print(root)
                            print(f)
                            print(i)
                            print(j)
                            continue
                        if CheckSheetIsChina(csvArrays[i][j]) and CheckIsUseCol(csvArrays, j):
                            Record(root, f.replace(".csv", ""), csvArrays, j)

    OutCsv()


if __name__ == "__main__":
    configPath = "DataConfig\Common"
    ScanChinaInCSV(configPath)
