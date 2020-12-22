#encoding : utf-8
import os
import sys
import time


import BuildConfig
from ExcelParser.Env import ConfigMgr, ParsingEnv
from Locale import Localization
from Misc.Util import *
import csv
import shutil


def main():
    start = time.time()
    print("开始生成二进制文件,lua/cs 代码文件======================")

    localization = Localization.Localization(ConfigMgr.Localization_Excel_Path,
                                             ConfigMgr.Localization_Excel_Config_Path, ConfigMgr.Localization_Log_File,
                                             False)
    localization_value = Localization.Localization(ConfigMgr.Value_Excel_Path, ConfigMgr.Value_Config_Excel_Path, None,
                                                   True)
    ParsingEnv.SetLocalization(localization, localization_value)

    region = sys.argv[1]  # 地区
    ConfigMgr.SetRegion(region)  # 地区
    CSharpLuaFlag = sys.argv[2]  # 是打CS 还是打 Lua ,或者全打
    absolutePath = os.getcwd() + "/DataConfig/"
    isBuildAll = True
    if len(sys.argv) >= 4:
        absolutePath = sys.argv[3]
        isBuildAll = False
    shutil.rmtree(ConfigMgr.Binary_Output_Dir)
    shutil.rmtree(ConfigMgr.CSharp_Desc_Output_Dir)
    os.mkdir(ConfigMgr.Binary_Output_Dir)
    os.mkdir(ConfigMgr.CSharp_Desc_Output_Dir)

    build_cfg = BuildConfig.BuildConfig(absolutePath, region, CSharpLuaFlag, isBuildAll)
    build_cfg.Build()

    end = time.time()
    print("已生成二进制文件,lua/cs 代码文件", "time===========>" + str(end - start))

    localization.Done()
    localization_value.Done()


if __name__ == '__main__':
    main()
