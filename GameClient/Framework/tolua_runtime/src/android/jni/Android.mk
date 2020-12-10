
LOCAL_PATH := $(call my-dir)

#build luajit
include $(CLEAR_VARS)
LOCAL_MODULE := libluajit
LOCAL_SRC_FILES := libluajit.a
include $(PREBUILT_STATIC_LIBRARY)

#build raw tolua
include $(CLEAR_VARS)
LOCAL_MODULE := raw_tolua
LOCAL_C_INCLUDES := $(LOCAL_PATH)/../../luajit-2.1/src
LOCAL_C_INCLUDES += $(LOCAL_PATH)/../../

LOCAL_CFLAGS :=  -O2 -std=gnu99
LOCAL_SRC_FILES :=	../../tolua.c \
					../../int64.c \
					../../uint64.c \
					../../pb.c \
					../../lpeg.c \
					../../struct.c \
					../../cjson/strbuf.c \
					../../cjson/lua_cjson.c \
					../../cjson/fpconv.c \
 					
LOCAL_WHOLE_STATIC_LIBRARIES += libluajit
include $(BUILD_STATIC_LIBRARY)

#build tolua with gdata
include $(CLEAR_VARS)
LOCAL_MODULE := tolua
LOCAL_C_INCLUDES := $(LOCAL_PATH)/../../gdata
LOCAL_C_INCLUDES += $(LOCAL_PATH)/../../luajit-2.1/src

LOCAL_SRC_FILES := ../../gdata/CSharpExport.cpp \
				   ../../gdata/GData.cpp \
				   ../../gdata/LuaExport.cpp 

LOCAL_WHOLE_STATIC_LIBRARIES += raw_tolua 
include $(BUILD_SHARED_LIBRARY)