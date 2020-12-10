#include "GData.h"

#include <chrono> 

extern "C"
{
#include "lua.h"
#include "lauxlib.h"
#include "luaconf.h"
};

const char *ReadString(byte *data, int offset) {
	if (data == NULL) {
		return NULL;
	}

	std::uint32_t relative_offset = ReadNumber<std::uint32_t>(data, offset);
	if(relative_offset == 0)
		return NULL;

	return (const char *)(data + relative_offset + offset);
}

template<class T>
inline int lua_readInteger(lua_State *L)
{
	const void* ptr = lua_touserdata(L, 1);
	int offset = (int)(lua_tointeger(L, 2));
	int optional_index = (lua_tointeger(L, 3));
	if(optional_index >= 0 &&IsEmptyOptional((const byte*)ptr, optional_index))
	{
		lua_pushnil(L);
		return 1;
	}

	T v = ReadNumber<T>(ptr, offset);
	lua_pushinteger(L, v);
	return 1;
}

static int lua_ReadSInt32(lua_State *L)
{
	return lua_readInteger<std::int32_t>(L);
}

static int lua_ReadUInt32(lua_State *L)
{
	return lua_readInteger<std::uint32_t>(L);
}

static int lua_ReadUInt16(lua_State *L)
{
	return lua_readInteger<std::uint16_t>(L);
}

static int lua_ReadSInt64(lua_State *L)
{
	return lua_readInteger<std::int64_t>(L);
}

static int lua_ReadUInt64(lua_State *L)
{
	return lua_readInteger<std::uint64_t>(L);
}

static int lua_ReadFloat(lua_State *L)
{
    const void* ptr = lua_touserdata(L, 1);
    int offset = (int)(lua_tointeger(L, 2));
    int optional_index = (lua_tointeger(L, 3));
    if(optional_index >= 0 &&IsEmptyOptional((const byte*)ptr, optional_index))
    {
        lua_pushnil(L);
        return 1;
    }

    float v = ReadNumber<std::int64_t>(ptr, offset);
    float result = v/4294967295.0f;//根据定点数中的浮点数转换规律定制的
    char  str[32];
    sprintf(str, "%.6f", result);
    lua_pushnumber(L, atof(str));
    return 1;
//    return lua_readInteger<std::int64_t>(L);
}

static int lua_ReadBool(lua_State *L) {
	const void* ptr = lua_touserdata(L, 1);
	int offset = (int)lua_tointeger(L, 2);
	byte v = ReadByte(ptr, offset);
	lua_pushboolean(L, v);
	return 1;
}

static int lua_ReadString(lua_State *L) {
	const void* data_ptr = lua_touserdata(L, 1);
	int offset = (int)lua_tointeger(L, 2);
	const char *str = ReadString((byte*)data_ptr, offset);
	if (str != NULL) {
		lua_pushstring(L, str);
	}
	else {
		lua_pushnil(L);
	}
	return 1;
}

enum E_GET_DATA_ERROR_CODE {
	CONFIG_FILE_NOT_EXIST = 1, 
	KEY_NOT_EXIST, 
	KEY_UNIQUE_ERROR, 
	VALUE_NOT_FOUND, 
	PARAM_ILLEGAL, 
	OPTIONAL_KEY_NOT_FOUND,
};

static void OnKeyNotFound(const Config *config, lua_State *L, const std::string &key_name){
	if(!config)
		return;

	const Config::KeyDesc *keyDesc = config->GetKeyDesc(key_name);
	if(keyDesc != NULL && keyDesc->optional){
		lua_pushnumber(L, E_GET_DATA_ERROR_CODE::OPTIONAL_KEY_NOT_FOUND);
	}else{
		lua_pushnumber(L, E_GET_DATA_ERROR_CODE::VALUE_NOT_FOUND);
	}
}

template<typename T>
static void GetDataImpl(lua_State *L, const Config *config, const std::string &key_name, T key, bool unique) {
	if(!config)
		return;

	if (unique) {
		const byte *data_ptr = config->GetEntry(key_name, key);
		if (data_ptr != NULL) {
			lua_pushlightuserdata(L, (void*)data_ptr);
			lua_pushnil(L);
		}
		else {
			lua_pushnil(L);
			OnKeyNotFound(config, L, key_name);
		}
	}
	else {
		const std::vector<byte*> &data = config->GetEntries(key_name, key);

		if (data.size() == 0) {
			lua_pushnil(L);
			OnKeyNotFound(config, L, key_name);
		}
		else {
			lua_createtable(L, (int)(data.size()), 0);
			for (int i = 0; i < data.size(); i++) {
				lua_pushlightuserdata(L, data[i]);
				lua_rawseti(L, -2, i + 1);
			}
			lua_pushnil(L);
		}
	}

}

template<typename T1, typename T2>
static void GetDataImpl(lua_State *L, const Config *config, const std::string &key_name1, T1 key1, const std::string &key_name2, T2 key2, bool unique) {
	if(!config)
		return;
		
	if (unique) {
		const byte *data_ptr = config->GetEntry(key_name1, key1, key_name2, key2);
		if (data_ptr != NULL) {
			lua_pushlightuserdata(L, (void*)data_ptr);
			lua_pushnil(L);
		}
		else {
			lua_pushnil(L);
			lua_pushinteger(L, E_GET_DATA_ERROR_CODE::VALUE_NOT_FOUND);
		}
	}
	else {
		const std::vector<byte*> &data = config->GetEntries(key_name1, key1, key_name2, key2);

		if (data.size() == 0) {
			lua_pushnil(L);
			lua_pushinteger(L, E_GET_DATA_ERROR_CODE::VALUE_NOT_FOUND);
		}
		else {
			lua_createtable(L, (int)(data.size()), 0);
			for (int i = 0; i < data.size(); i++) {
				lua_pushlightuserdata(L, data[i]);
				lua_rawseti(L, -2, i + 1);
			}
			lua_pushnil(L);
		}
	}

}

template<typename T1, typename T2, typename T3>
static void GetDataImpl(lua_State *L, const Config *config, const std::string &key_name1, T1 key1, const std::string &key_name2, T2 key2, const std::string &key_name3, T3 key3, bool unique) {
	if(!config)
		return;

	if (unique) {
		const byte *data_ptr = config->GetEntry(key_name1, key1, key_name2, key2, key_name3, key3);
		if (data_ptr != NULL) {
			lua_pushlightuserdata(L, (void*)data_ptr);
			lua_pushnil(L);
		}
		else {
			lua_pushnil(L);
			lua_pushinteger(L, E_GET_DATA_ERROR_CODE::VALUE_NOT_FOUND);
		}
	}
	else {
		const std::vector<byte*> &data = config->GetEntries(key_name1, key1, key_name2, key2, key_name3, key3);

		if (data.size() == 0) {
			lua_pushnil(L);
			lua_pushinteger(L, E_GET_DATA_ERROR_CODE::VALUE_NOT_FOUND);
		}
		else {
			lua_createtable(L, (int)(data.size()), 0);
			for (int i = 0; i < data.size(); i++) {
				lua_pushlightuserdata(L, data[i]);
				lua_rawseti(L, -2, i + 1);
			}
			lua_pushnil(L);
		}
	}
}

static int lua_GetDataPtrByID(lua_State *L) {
	const char *config_name = lua_tostring(L, 1);
	if (!config_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}
	const Config *config = ConfigManager::Instance().GetConfig(config_name);
	if (config == NULL) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::CONFIG_FILE_NOT_EXIST);
		return 2;
	}

	const char *key_name = lua_tostring(L, 2);
	if (!key_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config::KeyDesc *desc = config->GetKeyDesc(key_name);
	if (!desc) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	bool unique = lua_toboolean(L, 4);
	if (unique != desc->unique) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_UNIQUE_ERROR);
		return 2;
	}

	switch (desc->keyType)
	{
	case Config::E_KEY_TYPE::SINT32:
		GetDataImpl(L, config, key_name, (std::int32_t)(lua_tointeger(L, 3)), desc->unique);
		break;
	case Config::E_KEY_TYPE::UINT32:
		GetDataImpl(L, config, key_name, (std::uint32_t)(lua_tointeger(L, 3)), desc->unique);
		break;
	case Config::E_KEY_TYPE::STRING:
		GetDataImpl(L, config, key_name, lua_tostring(L, 3), desc->unique);
		break;
	}

	return 2;
}

static int lua_GetDataPtrByTwoID(lua_State *L) {
	const char *config_name = lua_tostring(L, 1);
	if (!config_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}
	const Config *config = ConfigManager::Instance().GetConfig(config_name);
	if (config == NULL) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::CONFIG_FILE_NOT_EXIST);
		return 2;
	}

	const char *key_name1 = lua_tostring(L, 2);
	if (!key_name1) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config::KeyDesc *desc1 = config->GetKeyDesc(key_name1);
	if (!desc1) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	const char *key_name2 = lua_tostring(L, 4);
	if (!key_name1) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config::KeyDesc *desc2 = config->GetKeyDesc(key_name2);
	if (!desc2) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	bool unique = lua_toboolean(L, 6);

	if (desc1->keyType == Config::E_KEY_TYPE::SINT32) {
		if (desc2->keyType == Config::E_KEY_TYPE::SINT32) {
			GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), unique);
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::UINT32) {
			GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), unique);
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::STRING) {
			GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, lua_tostring(L, 5), unique);
		}
	}
	else if (desc1->keyType == Config::E_KEY_TYPE::UINT32) {
		if (desc2->keyType == Config::E_KEY_TYPE::SINT32) {
			GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), unique);
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::UINT32) {
			GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), unique);
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::STRING) {
			GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, lua_tostring(L, 5), unique);
		}
	}
	else if (desc1->keyType == Config::E_KEY_TYPE::STRING) {
		if (desc2->keyType == Config::E_KEY_TYPE::SINT32) {
			GetDataImpl(L, config, key_name1, lua_tostring(L, 3), key_name2, (std::int32_t)(lua_tointeger(L, 5)), unique);
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::UINT32) {
			GetDataImpl(L, config, key_name1, lua_tostring(L, 3), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), unique);
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::STRING) {
			GetDataImpl(L, config, key_name1, lua_tostring(L, 3), key_name2, lua_tostring(L, 5), unique);
		}
	}

	return 2;
}

static int lua_GetDataPtrByThreeID(lua_State *L) {
	const char *config_name = lua_tostring(L, 1);
	if (!config_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}
	const Config *config = ConfigManager::Instance().GetConfig(config_name);
	if (config == NULL) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::CONFIG_FILE_NOT_EXIST);
		return 2;
	}

	const char *key_name1 = lua_tostring(L, 2);
	if (!key_name1) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config::KeyDesc *desc1 = config->GetKeyDesc(key_name1);
	if (!desc1) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	const char *key_name2 = lua_tostring(L, 4);
	if (!key_name2) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config::KeyDesc *desc2 = config->GetKeyDesc(key_name2);
	if (!desc2) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	const char *key_name3 = lua_tostring(L, 6);
	if (!key_name3) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config::KeyDesc *desc3 = config->GetKeyDesc(key_name3);
	if (!desc3) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	bool unique = lua_toboolean(L, 8);

	if (desc1->keyType == Config::E_KEY_TYPE::SINT32) {
		if (desc2->keyType == Config::E_KEY_TYPE::SINT32) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::UINT32) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::STRING) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (std::int32_t)(lua_tointeger(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
	}
	else if (desc1->keyType == Config::E_KEY_TYPE::UINT32) {
		if (desc2->keyType == Config::E_KEY_TYPE::SINT32) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::UINT32) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::STRING) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (std::uint32_t)(lua_tointeger(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
	}
	else if (desc1->keyType == Config::E_KEY_TYPE::STRING) {
		if (desc2->keyType == Config::E_KEY_TYPE::SINT32) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (std::int32_t)(lua_tointeger(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::UINT32) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (std::uint32_t)(lua_tointeger(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
		else if (desc2->keyType == Config::E_KEY_TYPE::STRING) {
			if (desc3->keyType == Config::E_KEY_TYPE::SINT32) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (std::int32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::UINT32) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (std::uint32_t)(lua_tointeger(L, 7)), unique);
			}
			else if (desc3->keyType == Config::E_KEY_TYPE::STRING) {
				GetDataImpl(L, config, key_name1, (lua_tostring(L, 3)), key_name2, (lua_tostring(L, 5)), key_name3, (lua_tostring(L, 7)), unique);
			}
		}
	}

	return 2;
}

static std::chrono::time_point<std::chrono::system_clock> S_begin;
static int lua_ChronoStart(lua_State *L) {
	S_begin = std::chrono::system_clock::now();
	return 0;
}

static int lua_ChronoEnd(lua_State *L) {
	double microSeconds = (double)(std::chrono::duration_cast<std::chrono::microseconds>(std::chrono::system_clock::now() - S_begin)).count();
	lua_pushnumber(L, microSeconds);
	return 1;
}

static bool GetAllKeys_CommonCheck(lua_State *L, const Config * &config, const char * &keyName) {
	const char *config_name = lua_tostring(L, 1);
	if (!config_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return false;
	}

	config = ConfigManager::Instance().GetConfig(config_name);
	if (!config) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::CONFIG_FILE_NOT_EXIST);
		return false;
	}

	keyName = lua_tostring(L, 2);
	if (!keyName) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return false;
	}
	return true;
}

static int lua_GetAllKeys_UInt32(lua_State *L) {
	const Config *config = NULL;
	const char *keyName = NULL;

	if (!GetAllKeys_CommonCheck(L, config, keyName)) {
		return 2;
	}

	std::vector<std::uint32_t> keys = config->GetALlKeys<std::uint32_t,
		std::unordered_map<std::uint32_t, byte *>,
		std::unordered_map<std::uint32_t, std::vector<byte*>>>(keyName, Config::E_KEY_TYPE::UINT32);

	if (keys.size() == 0) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	lua_createtable(L, (int)(keys.size()), 0);
	for (int i = 0; i < keys.size(); i++) {
		lua_pushinteger(L, keys[i]);
		lua_rawseti(L, -2, i + 1);
	}

	return 1;
}

static int lua_GetAllKeys_SInt32(lua_State *L) {
	const Config *config = NULL;
	const char *keyName = NULL;

	if (!GetAllKeys_CommonCheck(L, config, keyName)) {
		return 2;
	}

	std::vector<std::int32_t> keys = config->GetALlKeys<std::int32_t,
		std::unordered_map<std::int32_t, byte *>,
		std::unordered_map<std::int32_t, std::vector<byte*>>>(keyName, Config::E_KEY_TYPE::SINT32);

	if (keys.size() == 0) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}


	lua_createtable(L, (int)(keys.size()), 0);
	for (int i = 0; i < keys.size(); i++) {
		lua_pushinteger(L, keys[i]);
		lua_rawseti(L, -2, i + 1);
	}

	return 1;
}


static int lua_GetAllKeys_String(lua_State *L) {
	const Config *config = NULL;
	const char *keyName = NULL;

	if (!GetAllKeys_CommonCheck(L, config, keyName)) {
		return 2;
	}

	std::vector<std::string> keys = config->GetALlKeys<std::string,
		std::unordered_map<std::string, byte *>,
		std::unordered_map<std::string, std::vector<byte*>>>(keyName, Config::E_KEY_TYPE::STRING);
	if (keys.size() == 0) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::KEY_NOT_EXIST);
		return 2;
	}

	lua_createtable(L, (int)(keys.size()), 0);
	for (int i = 0; i < keys.size(); i++) {
		lua_pushstring(L, keys[i].c_str());
		lua_rawseti(L, -2, i + 1);
	}

	return 1;
}

static int lua_GetEntryCount(lua_State *L) {
	const char *config_name = lua_tostring(L, 1);
	if (!config_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}
	const Config *config = ConfigManager::Instance().GetConfig(config_name);
	if (config == NULL) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::CONFIG_FILE_NOT_EXIST);
		return 2;
	}
	
	int count = config->GetEntryCount();
	lua_pushinteger(L, count);
	return 1;
}

static int lua_GetMemoryUsage(lua_State *L) {
	std::uint32_t mem = ConfigManager::Instance().GetMemoryUsage();
	lua_pushinteger(L, mem);
	return 1;
}

static int lua_GetConfigData(lua_State *L){
	const char *config_name = lua_tostring(L, 1);
	if (!config_name) {
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	const Config *config = ConfigManager::Instance().GetConfig(config_name);

	if(!config){
		lua_pushnil(L);
		lua_pushinteger(L, E_GET_DATA_ERROR_CODE::PARAM_ILLEGAL);
		return 2;
	}

	int count = config->GetEntryCount();

	lua_createtable(L, count, 0);
	for (int i = 0; i < count; i++) {
		lua_pushlightuserdata(L, (void*)(config->GetEntryPtr(i)));
		lua_rawseti(L, -2, i + 1);
	}
	lua_pushnil(L);
	return 2;
}

static int lua_Release(lua_State *L){
	ConfigManager::Release();
	return 0;
}

void SetTableFromLua(lua_State *L, const char *key, int value_stack_pos, int table_pos){
	lua_pushstring(L, key);
	lua_pushvalue(L, value_stack_pos);
	lua_rawset(L, table_pos);
}

static int lua_CreateConfigObj(lua_State *L){
    int top = lua_gettop(L);

	lua_newuserdata(L, 0);
	lua_createtable(L, 0, 0);

	SetTableFromLua(L, "template", 1, top + 2);
	SetTableFromLua(L, "data_ptr", 2, top + 2);

	SetTableFromLua(L, "__index", 3, top + 2);
	SetTableFromLua(L, "__len", 4, top + 2);
	SetTableFromLua(L, "__iter", 5, top + 2);
	SetTableFromLua(L, "__tostring", 6, top + 2);

	lua_setmetatable(L, top + 1);
	return 1;
}

static int lua_IsEmptyOptional(lua_State *L){
	const void *data_ptr = lua_touserdata(L, 1);
	if(data_ptr ==  NULL){
		lua_pushnil(L);
		return 1;
	}
	int optional_index = lua_tointeger(L, 2);
	if(IsEmptyOptional((const byte*)data_ptr, optional_index)){
		lua_pushboolean(L, 1);
		return 1;
	}else{
		lua_pushboolean(L, 0);
		return 1;
	}
}

static int lua_GetVariableAttributePtr(lua_State *L){
	const void *data_ptr = lua_touserdata(L, 1);
	if(data_ptr ==  NULL){
		lua_pushnil(L);
		return 1;
	}

	int variable_attribute_index = lua_tointeger(L, 2);
	const byte* ptr = GetVariableAttributePtr((const byte *)data_ptr, variable_attribute_index);
	if(ptr){
		lua_pushlightuserdata(L, (void*)ptr);
		return 1;
	}else{
		lua_pushnil(L);
		return 1;
	}
}

static int lua_GetRepeatedStructElementPtr(lua_State *L){
	const void *data_ptr = lua_touserdata(L, 1);
	if(data_ptr ==  NULL){
		lua_pushnil(L);
		return 1;
	}

	int index = lua_tointeger(L, 2);
	const byte *ptr = GetRepeatedStructElementPtr((const byte *)data_ptr, index);
	if(ptr) {
		lua_pushlightuserdata(L, (void*)ptr);
		return 1;
	}else{
		lua_pushnil(L);
		return 1;
	}
}

static const struct luaL_Reg GData_funcs[] = {
	{ "ReadUInt16",	lua_ReadUInt16},
	{ "ReadSInt32",	lua_ReadSInt32 },
	{ "ReadUInt32",	lua_ReadUInt32 },
	{ "ReadSInt64",	lua_ReadSInt64 },
	{ "ReadUInt64",	lua_ReadUInt64 },
	{ "ReadFloat" , lua_ReadFloat },
	{ "ReadBool",	lua_ReadBool},
	{ "ReadString",	lua_ReadString},

	{ "GetDataPtr",	lua_GetDataPtrByID},
	{ "GetDataPtrByTwoID",	lua_GetDataPtrByTwoID},
	{ "GetDataPtrByThreeID",	lua_GetDataPtrByThreeID},

	{ "GetAllKeysUInt32",	lua_GetAllKeys_UInt32},
	{ "GetAllKeysSInt32",	lua_GetAllKeys_SInt32},
	{ "GetAllKeysString",	lua_GetAllKeys_String},

	{ "GetEntryCount",	lua_GetEntryCount},

	{ "GetConfigData",	lua_GetConfigData},

	{ "ChronoStart",lua_ChronoStart},
	{ "ChronoEnd",	lua_ChronoEnd},

	{ "GetMemroyUsage",	lua_GetMemoryUsage},
	{ "CreateConfigObj",	lua_CreateConfigObj},

	{"Release", lua_Release},

	{ "IsEmptyOptional",	lua_IsEmptyOptional},//for debug 

	{ "GetVariableAttributePtr",	lua_GetVariableAttributePtr},

	{ "GetRepeatedStructElementPtr",	lua_GetRepeatedStructElementPtr},

	{ NULL, NULL }
};

extern "C"
{
	extern int luaopen_gdata(lua_State *L)
	{
		luaL_register(L, "GData", GData_funcs);
		return 1;
	}
};


