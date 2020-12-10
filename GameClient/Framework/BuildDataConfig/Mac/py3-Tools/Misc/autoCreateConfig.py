#!/usr/bin/python
#coding=utf-8

##
# @file:   md5check.py
# @author: yuexianliu
# @brief: 1.自动新增新的csv到md5config表（在data有数据的，用于合入之后的操作）
#		  2.通过读Data来判断填md5，打表规则的信息
##

import time
import os
import csv
import sys
import hashlib
import subprocess

#md5配置文件
CONFIG_PATH = "..\Data\md5config.csv"

#csv表位置
CSV_PATH = "..\DataConfig"
CSV_POSTFIX = ".csv"

#data中文件的前缀
DATA_PREFIX = "dataconfig_"

#客户端的lua以及后缀
CLIENT_LUA_PATH = "..\Data\Client\lua"
LUA_POSTFIX = "_pb.lua"

#客户端的lua_small以及后缀，因为和lua互斥，但是结构都是存在lua文件夹中，但是生成的bytes存放在bytes_small
CLIENT_LUA_SMALL_PATH = "..\Data\Client\bytes_small"
LUA_SMALL_POSTIX = ".bytes"

#客户端的cs位置以及其后缀
CLIENT_CS_PATH = "..\Data\Client\cscripts"
CS_POSTFIX = ".cs"

#服务器路径以及需要截取的位置data和后缀
SERVER_PATH = "..\Data\Server\server_bytes" 
SERVER_POSTFIX = ".data"
SERVER_PATH_PREFIX = "..\Data\Server\server_bytes\\"
SERVER_PATH_POSTFIX = "\data"

#内存暂存md5配置文件的内容
md5Config = []

#配置表表头
configHead =['csvPos','md5','md5_single','cs','lua','svr','diffCountry']

#配置表表头位置
CSVPOS = 0
MD5 = 1
MD5_single = 2
CS = 3
LUA = 4
SVR = 5
DIFFCOUNTRY = 6

#配置表中的flag
LUA_CONFIG = "lua"
LUA_SMALL_CONFIG = "lua_small"
CS_CONFIG = "cs"
SERVER_CONFIG = "svr"

#小服的判断 --恶心死了
SMALL_CHECK = "小服".decode("utf-8")
SMALL_CHECK_POSTFIX ="_small"

#.bat名字
CS_NAME = "xlsc_cs.bat"
CS_NAME_SMALL = "xlsc_cs_small.bat"
LUA_NAME = "xlsc_lua.bat"
LUA_NAME_SMALL = "xlsc_lua_small.bat"
SVR_NAME = "xlsc_svr.bat"
SVR_NAME_SMALL = "xlsc_svr_small.bat"

#svr除了zone的位置处理
STEP1_SERVER_TMP =".\Data\Server\server_tmp\\"
STEP2_SERVER_DATA =".\Data\Server\server_bytes\\"

#国家配置（因为有不同目录相同表，要对对国家做判断，增加了后期维护量，是原来自己key结构设计不好）
countryConfig = {"China","Taiwan","Korean","Vietnam"}

#获取md5配置文件
def get_md5Config():
	if not os.path.isfile(CONFIG_PATH):
		print(CONFIG_PATH + " is not exit")
		print("=============creat file=============")
		with open(CONFIG_PATH, 'w', encoding='UTF-8-sig') as csvFile:
			write = csv.writer(csvFile,lineterminator='\n')
			write.writerow(configHead)

	with open(CONFIG_PATH,'r', encoding='UTF-8-sig') as csvfile:
		reader = csv.reader(csvfile)
		for row in reader:
			md5Config.append(row)

#设置md5配置文件
def set_md5Config():
	with open(CONFIG_PATH,'w', encoding='UTF-8-sig') as csvFile:
		write = csv.writer(csvFile,lineterminator='\n')
		write.writerows(md5Config)

#存取字典方式
def get_dirs_dic(path,postfix):
	#字典里面的结构差不多是这样的
	CACHE_DICTIONARY = {'csvname':'csvpath'}
	if not os.path.exists(path):
		print(path + " is not exit")
	list_dirs = os.walk(path)
	for root,dirs,files in list_dirs:
		for f in files:
			if f.endswith(postfix):
				fname = f.replace(postfix,"").lower()
				fdir = os.path.join(root,f)	

				for country in countryConfig:
					if country in root:
						fname = fname + country

				if SMALL_CHECK in fdir:
					fname = fname + SMALL_CHECK_POSTFIX

				CACHE_DICTIONARY.update({str(fname):str(fdir)})

	return CACHE_DICTIONARY

#读取client的方法
def read_client_data(dic,path,postfix,flag,col):
	list_dirs = os.walk(path)
	for root,dirs,files in list_dirs:
		for f in files:
			if f.endswith(postfix):
				f = f.replace(DATA_PREFIX,"")
				f = f.replace(postfix,"")

				for country in countryConfig:
					if country in root:
						f = f + country

				if flag == LUA_SMALL_CONFIG:
					f = f + SMALL_CHECK_POSTFIX
				try:
					isExists = isExistsInMd5Config_Client(dic[f],flag,col)
					if not isExists:
						newConfig =[dic[f],'','','','','','','']
						newConfig[col] = flag
						newConfig[MD5] = md5Calaulate(dic[f])
						newConfig[MD5_single] = md5Calaulate(dic[f])
						md5Config.append(newConfig)
				except KeyError:
					print("============="+ f + CSV_POSTFIX +" not exit=============")

#查找是否存在md5config里面
def isExistsInMd5Config_Client(key,flag,col):
	row_count = len(md5Config)
	col_count = len(md5Config[0])
	for i in range(1,row_count):
		if str(md5Config[i][CSVPOS]).rstrip() == str(key).rstrip():
			md5Config[i][col] = flag
			md5Config[i][MD5] = md5Calaulate(key)
			md5Config[i][MD5_single] = md5Calaulate(key)
			return True

	return False
	
#读取服务器的方法，可以和客户端合并，没必要，整洁明了一点，不要那么多判断，看着眼睛疼
def read_server_data(dic,path,postfix,flag,col):
	list_dirs = os.walk(path)
	for root,dirs,files in list_dirs:
		for f in files:
			if f.endswith(postfix):
				f = f.replace(DATA_PREFIX,"")
				f = f.replace(postfix,"")
				for country in countryConfig:
					if country in root:
						f = f + country

				try:
					isExists = isExistsInMd5Config_Server(dic[f],flag,col)
					if not isExists:
						newConfig =[dic[f],'','','','','']
						newConfig[col] = flag
						newConfig[MD5] = md5Calaulate(dic[f])
						newConfig[MD5_single] = md5Calaulate(dic[f])
						md5Config.append(newConfig)
				except KeyError:
					print("============="+ f + CSV_POSTFIX +" not exit=============")

#查找是否存在md5config里面
def isExistsInMd5Config_Server(key,flag,col):
	row_count = len(md5Config)
	col_count = len(md5Config[0])
	for i in range(1,row_count):
		if str(md5Config[i][CSVPOS]).rstrip() == str(key).rstrip():
			md5Config[i][col] = flag
			md5Config[i][MD5] = md5Calaulate(key)
			md5Config[i][MD5_single] = md5Calaulate(key)
			return True

	return False

#计算md5
def md5Calaulate(ppath):
	f = open(ppath,'rb')
	md5obj = hashlib.md5()
	md5obj.update(f.read())
	hash = md5obj.hexdigest()
	f.close()
	md5Values = str(hash)
	return md5Values

#读取顺序
def read_all_file():
	get_md5Config()
	dir_path_dic=get_dirs_dic(CSV_PATH,CSV_POSTFIX)
	read_client_data(dir_path_dic,CLIENT_LUA_PATH,LUA_POSTFIX,LUA_CONFIG,LUA)
	read_client_data(dir_path_dic,CLIENT_CS_PATH,CS_POSTFIX,CS_CONFIG,CS)
	read_client_data(dir_path_dic,CLIENT_LUA_SMALL_PATH,LUA_SMALL_POSTIX,LUA_SMALL_CONFIG,LUA)
	read_server_data(dir_path_dic,SERVER_PATH,SERVER_POSTFIX,SERVER_CONFIG,SVR)

# mian
if __name__ == '__main__':
	start = time.time()
	read_all_file()
	set_md5Config()
	end = time.time()
	print("time===========>" + str(end-start))