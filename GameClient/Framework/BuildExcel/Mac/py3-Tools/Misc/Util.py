from ExcelParser.Env import *

import os
import logging

def GenIndent(indent):
    return '    ' * indent

def GetDirFilesImpl(dir_path):
    all_files = []

    for home, dirs, files in os.walk(dir_path):
        for filename in files:
            file_path = os.path.join(home, filename)
            file_path = file_path.replace("\\", '/')
            all_files.append(file_path)

        for subdir in dirs:
            all_files += GetDirFilesImpl(os.path.join(home, subdir))

    return all_files

def GetDirFiles(dir_path):
    return list(set(GetDirFilesImpl(dir_path)))


class LogHelp :
    """日志辅助类"""
    _logger = None
    _close_imme = True

    @staticmethod
    def set_close_flag(flag):
        LogHelp._close_imme = flag

    @staticmethod
    def _initlog():
        LogHelp._logger = logging.getLogger()
        logfile = ConfigMgr.Parser_Log_File
        hdlr = logging.FileHandler(logfile, mode='w')
        formatter = logging.Formatter('%(asctime)s|%(levelname)s|%(lineno)d|%(funcName)s|%(message)s')
        hdlr.setFormatter(formatter)
        LogHelp._logger.addHandler(hdlr)
        LogHelp._logger.setLevel(logging.ERROR)

        LogHelp._logger.info("\n\n\n")
        LogHelp._logger.info("logger is inited!")

    @staticmethod
    def close() :
        if LogHelp._close_imme:
            import logging
            if LogHelp._logger is None :
                return
            logging.shutdown()
    
    @staticmethod
    def LogDebug(msg):
        LogHelp._logger.debug(msg)

    @staticmethod
    def LogInfo(msg):
        LogHelp._logger.info(msg)

    @staticmethod
    def LogWarn(msg):
        LogHelp._logger.warn(msg)
    
    @staticmethod
    def LogError(msg):
        LogHelp._logger.error(msg)
        raise Exception(msg)


LogHelp._initlog()