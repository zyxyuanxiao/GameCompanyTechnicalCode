--[[--
 * @Description: 配置数据中心，其他系统统一从这里拿配置数据

 * @Author:      armyshu
 * @FileName:    logic/common/config_data_center.lua
 * @DateTime:    2015-08-05 15:19:15
 ]]

---@class config_data_center
config_data_center = {}
---@type config_data_center
local this = config_data_center



--新增接口列表:
--GetEntryCount 获取表内数据条目的数量

--下面这些接口存在无法解决的效率问题，出于接口兼容（主要是业务逻辑的复杂性),全部保留
--getConfigDataByID ,最常用的方法
--getConfigDatasByID ,多列 key,重复 key 使用的方法
--getConfigData ,尽量少用的方法
--getOriginalConfigCopy ,尽量不用的方法
--setConfigProtobufItemsByName
--ClearElementDataDictByConfigName --无效方法
--getConfigDataByFunc 会返回一个对象,从这个地方读取
--getConfigDatasByFunc 会返回table,并且以键值对形式返回 [1]=xxx,[2]=xxx

--[[
    配置表

    支持的操作：
        获取配置表对象：
            local config = config_data_center.getConfigDataByID("dataconfig_battlechart", "ID", 2)

            简单查询:
                config.MinRank
                config.OpenDay

            数组操作：

                简单索引
                config.RewardList[1]
                config.RewardList[2]

                获取数组长度
                #config.RewardList

            遍历操作:

                遍历配置表:
                for key, value in pairs(config) do
                    --open_if_need print(key, value)
                end

                遍历数组：
                for index, value in pairs(config.RewardList) do
                    --open_if_need print(index, value)
                end

            打印表内容:
                --open_if_need print(tostring(config)) --注意：必须加tostring才能够正常打印
            
            注意：
                对于excel为空的optional字段，读取返回nil
    
    不支持的操作:
        对于配置表对象的任何修改行为.
        使用next()遍历配置表对象
]]
--require "logic/common/ios_check_mgr"

--[[
dgp = GData.GetDataPtr
for k, v in pairs(GData) do
    GData[k] = function(...)
        --open_if_need print(debug.traceback())
        return v(unpack({...}))
    end
end
GData.GetDataPtr = function(v1, v2, v3, v4)
    --open_if_need print(v1)
    return dgp(v1, v2, v3, v4)
end
]]
local config_env = require "ExcelConfig/gdata_lua_env"

local ENABLE_LUA_CACHE = false --FIXME:这个缓存模式需要在考虑

do
	local loader = loadfile "ExcelConfig/excel_config.lua"
	setfenv(loader, config_env)
	loader()
end

local all_config_map = config_env.GetAllConfig()

local CreateStructObj
local CreateArrayObj

local ToString_inner
ToString_inner = function(obj, sink, indentLevel)
	local iter_func = getmetatable(obj).__iter
	local function append(v)
		if v == nil then
			sink[#sink + 1] = "nil"
		else
			sink[#sink + 1] = tostring(v)
		end
	end
	local function indent(p)
		append(("    "):rep(p or indentLevel))
	end
	local function newline()
		append("\n")
	end

	newline()
	indent(indentLevel - 1)
	append("{")
	local no_value = true
	for key, value in iter_func(obj) do
		no_value = false
		newline()
		indent()

		-- name
		local keyType = type(key)
		if keyType == "string" then
			append(key)
		else
			append("[")
			append(key)
			append("]")
		end

		-- "="
		append(" = ")

		-- value
		local valueType = type(value)
		if valueType == "userdata" then
			ToString_inner(value, sink, indentLevel + 1)
		elseif valueType == "string" then
			append('"')
			append(value)
			append('"')
		else
			append(value)
		end
		append(',')
	end
	newline()
	indent(indentLevel - 1)
	append("}")
end

local ToString = function(obj)
	local sink = {}
	ToString_inner(obj, sink, 1)
	return table.concat(sink)
end

--local function GetPreciseDecimal(nNum, n)
--	if type(nNum) ~= "number" then
--		return nNum;
--	end
--
--	n = n or 0;
--	n = math.floor(n)
--	local fmt = '%.' .. n .. 'f'
--	local nRet = tonumber(string.format(fmt, nNum))
--
--	return nRet;
--end

local struct_obj_meta
struct_obj_meta = {
	__index = function(struct_obj, index)
		local obj_meta = getmetatable(struct_obj)

		if type(index) ~= "string" or obj_meta.template[index] == nil then
			return nil
		end

		local val
		local field_desc = obj_meta.template[index]
		if not next(field_desc.meta_info, nil) then
			return nil
		end
		if field_desc.meta_info.array_type then
			local array_ptr = GData.GetVariableAttributePtr(obj_meta.data_ptr, field_desc.variable_attr_index)
			if array_ptr then
				val = CreateArrayObj(array_ptr, field_desc)
			else
				fatal("struct obj should not get nil attribute ptr")
			end
		elseif field_desc.meta_info.type_desc.value_type then
			local optional_index = -1
			if field_desc.fixed_optional_index then
				optional_index = field_desc.fixed_optional_index
			end
			val = field_desc.meta_info.type_desc.reader(obj_meta.data_ptr, field_desc.fixed_offset, optional_index)
			-- if field_desc.meta_info.type_desc.type_name == "float" then
			-- 	val = tonumber(string.format('%.6f', val/4294967295))
			-- end
		else
			assert(false)
		end

		return val
	end,
	__newindex = function()
		fatal("配置表不允许修改！")
	end,
	__len = function(struct_obj)
		return 0
	end,
	__iter = function(struct_obj)
		return function(t, prev_key)
			local meta = getmetatable(t)
			local key = next(meta.template, prev_key)
			if key == 'array_type' then
				key = next(meta.template, key)
			end
			if not key then
				return nil, nil
			else
				return key, t[key]
			end
		end, struct_obj, nil
	end,
	__tostring = ToString
}

local ARRAY_COUNT_SIZE = 2 --array类型的前2字节用于描述数组元素个数
local array_obj_meta
array_obj_meta = {
	__index = function(array_obj, index)
		if type(index) ~= "number" then
			fatal("数组索引不是数字！")
			return nil
		end

		if math.floor(index) < index then
			fatal("数组索引不是正整数！")
			return nil
		end

		local obj_meta = getmetatable(array_obj)
		local item_count = array_obj_meta.__len(array_obj)
		if index > item_count or index < 1 then
			--[[if item_count == 0 then
                warning('数组为空')
            else
                warning(string.format("数组索引范围应在[%d, %d]之间", min, item_count))
            end]]
			return nil
		end

		local val
		local meta_info = obj_meta.template.meta_info
		if meta_info.type_desc and meta_info.type_desc.value_type then
			val = meta_info.type_desc.reader(obj_meta.data_ptr, meta_info.type_desc.byte_size * (index - 1) + ARRAY_COUNT_SIZE, -1)
			-- if meta_info.type_desc.type_name == "float" then
			-- 	val = tonumber(string.format('%.6f', val/4294967295))
			-- end
		else --struct
			local struct_ptr = GData.GetRepeatedStructElementPtr(obj_meta.data_ptr, index - 1)
			val = CreateStructObj(struct_ptr, meta_info)
		end

		return val
	end,
	__newindex = function()
		fatal("配置表不允许修改！")
	end,
	__len = function(array_obj)
		local obj_meta = getmetatable(array_obj)
		return GData.ReadUInt16(obj_meta.data_ptr, 0, -1)
	end,
	__iter = function(array_obj)
		local array_len = array_obj_meta.__len(array_obj)
		return function(t, index)
			index = index + 1
			if index > 0 and index <= array_len then
				return index, t[index]
			end
		end, array_obj, 0
	end,
	__tostring = ToString
}

local weak_ref_cache = setmetatable({}, {__mode = "k"})

local IsGdataObj = function(obj)
	if type(obj) ~= "userdata" then
		return false
	end
	return not (not weak_ref_cache[obj])
end

local AddToWeakRef = function(obj)
	weak_ref_cache[obj] = true
end
--[[
    remark:
    此处本可以用一个table简单的表示struct_obj与array_obj, 但是lua 5.1中 对于table，无法通过metatable定义pairs和#操作符。
    而逻辑代码中对配置表对象大量使用了pairs和#操作符. 因此为了支持pairs和#操作符， 采用userdata表示struct_obj和array_obj。
    并为userdata的metatable添加__len方法，以支持#操作符.
    对于pairs，在本文件末尾用自定义的pairs和ipairs函数覆盖了默认的pairs和ipairs。

    虽然绕过了这两个问题，但是这种方式带来了额外的负担。理想情况下，应该使用__iter进行遍历，使用__len方法获取长度， 使得struct_obj和array_obj可以用table表示
]]
CreateStructObj = function(data_ptr, struct_template)
	local struct_obj = GData.CreateConfigObj(struct_template, data_ptr, struct_obj_meta.__index, struct_obj_meta.__len, struct_obj_meta.__iter, struct_obj_meta.__tostring)
	AddToWeakRef(struct_obj)
	return struct_obj
end

CreateArrayObj = function(data_ptr, array_template)
	local array_obj = GData.CreateConfigObj(array_template, data_ptr, array_obj_meta.__index, array_obj_meta.__len, array_obj_meta.__iter, array_obj_meta.__tostring)
	AddToWeakRef(array_obj)
	return array_obj
end

-----------------------------------config_data_center -------------------------
local configMapTable = {
	-- ["dataconfig_dailygiftbag"] = "dataconfig_dailygiftbag_ios", -- 136 礼包
	-- ["dataconfig_weekcards"] = "dataconfig_dailygiftbag_ios", -- 周卡月卡
	-- ["dataconfig_activitybigprofitreward"] = "dataconfig_activitybigprofitreward_ios", -- 一本万利
	-- ["dataconfig_holidaygift"] = "dataconfig_holidaygift_ios", -- 计费点节点礼包
	-- ["dataconfig_activitytimelimitpackage"] = "dataconfig_activitytimelimitpackage_ios", -- 限时礼包
	-- ["dataconfig_bubble"] = "dataconfig_bubble_ios", -- 上新特惠
	-- ["dataconfig_activetag"] = "dataconfig_activetag_ios", -- 活动页签
	-- ["dataconfig_newoperateactivity"] = "dataconfig_newoperateactivity", -- 活动时间
	-- ["dataconfig_dicinfoconfig"] = "dataconfig_dicinfoconfig_ios", -- 字典表
	-- ["dataconfig_lventryopen"] = "dataconfig_lventryopen_ios", -- 入口等级开放表
	-- ["dataconfig_taskinfo"] = "dataconfig_taskinfo_ios", -- 任务信息
	-- ["dataconfig_tasknpc"] = "dataconfig_tasknpc_ios", -- 任务信息
	-- ["dataconfig_levelconfig"] = "dataconfig_levelconfig_ios", -- 关卡信息
	-- ["dataconfig_dialogueconfig"] = "dataconfig_dialogueconfig_ios", -- 情景对话
	-- ["dataconfig_worldbigmap"] = "dataconfig_worldbigmap_ios", -- 世界地图传送
	-- ["dataconfig_goldshopgoodsconfig"] = "dataconfig_goldshopgoodsconfig_ios" -- 元宝商城
}

local getIosCheckconfig_name = function(config_name)
	--if not ios_check_mgr.IsIosCheckVer() then
	--	return config_name
	--end

	local result = configMapTable[config_name]

	if result ~= nil then
		return result
	end

	return config_name
end

config_data_center = {}

local E_GET_DATA_ERROR_CODE = {
	CONFIG_FILE_NOT_EXITS = 1,
	KEY_NOT_EXIST = 2,
	KEY_UNIQUE_ERROR = 3,
	VALUE_NOT_FOUND = 4,
	PARAM_ILLEGAL = 5,
	OPTIONAL_KEY_NOT_FOUND = 6
}

local LogGDataError = function(errCode, unique, config_name, ...)
	local key_value_list = {...}
	local key_names = ""
	local index = 1
	local key_value_str = ""

	while index <= #key_value_list do
		key_names = key_names .. " " .. tostring(key_value_list[index])

		key_value_str = key_value_str .. tostring(key_value_list[index]) .. ":"
		key_value_str = key_value_str .. tostring(key_value_list[index + 1]) .. ", "

		index = index + 2
	end

	local key_name = key_value_list[1]
	if errCode == E_GET_DATA_ERROR_CODE.CONFIG_FILE_NOT_EXITS then
		fatal(string.format("config：%s do not have binary data", config_name))
	elseif errCode == E_GET_DATA_ERROR_CODE.KEY_NOT_EXIST then
		fatal(string.format("config：%s do not contain key column: %s", config_name, key_names))
	elseif errCode == E_GET_DATA_ERROR_CODE.KEY_UNIQUE_ERROR then
		fatal( string.format("config: %s key column: %s  %s unique", config_name, key_name, unique and "isn't" or "is"))
	elseif errCode == E_GET_DATA_ERROR_CODE.VALUE_NOT_FOUND then
		-- fatal(string.format("config：%s cannot find value with %s", config_name, key_value_str))
	elseif errCode == E_GET_DATA_ERROR_CODE.PARAM_ILLEGAL then
		fatal(string.format("get config with illegal param: %s %s", config_name, key_value_str))
	elseif errCode == E_GET_DATA_ERROR_CODE.OPTIONAL_KEY_NOT_FOUND then
		-- fatal(string.format("config：%s cannot find value with %s", config_name, key_value_str))
	else
		fatal(string.format("unknown error gdata code: %d", errCode))
	end
end

local modified_configs = {} --负责对应业务逻辑的人没时间改，那就只能这样了

local GetFromModifiedConfigs = function(config_name, single, ...)
	local config_array = modified_configs[config_name]
	if not config_array then
		return nil
	end

	local args = {...}
	local Same = function(_tab)
		local index = 0
		local flag = true
		while index < #args do
			key_name = args[index + 1]
			key_value = args[index + 2]

			if _tab[key_name] ~= key_value then
				flag = false
				break
			end
			index = index + 2
		end
		return flag
	end

	if single then
		for _, config in pairs(config_array) do
			if Same(config) then
				return config
			end
		end

		return nil
	else
		local ret = {}

		for _, config in pairs(config_array) do
			if Same(config) then
				ret[#ret + 1] = config
			end
		end

		if #ret == 0 then
			return nil
		end
		return ret
	end
end

function config_data_center.getConfigDataByID(config_name, key_name, key_value)
	if type(config_name) ~= "string" or type(key_name) ~= "string" then
		fatal("参数非法")
		return nil
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local modified = GetFromModifiedConfigs(config_name, true, key_name, key_value)
	if modified then
		return modified
	end

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return nil
	end

	local data_ptr, error_code = GData.GetDataPtr(config_name, key_name, key_value, true)
	if error_code then
		LogGDataError(error_code, true, config_name, key_name, key_value)
		return nil
	end

	return CreateStructObj(data_ptr, struct_template)
end

function config_data_center.getConfigDatasByID(config_name, key_name, key_value)
	if type(config_name) ~= "string" or type(key_name) ~= "string" then
		fatal("参数非法")
		return {}
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local modified = GetFromModifiedConfigs(config_name, false, key_name, key_value)
	if modified then
		return modified
	end

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return {}
	end

	local data_ptr_table, error_code = GData.GetDataPtr(config_name, key_name, key_value, false)
	if error_code then --查询失败时，第二个参数是errorcode
		LogGDataError(error_code , false, config_name, key_name, key_value)
		return {}
	end

	local ret = {}
	for _, data_ptr in pairs(data_ptr_table) do
		table.insert(ret, CreateStructObj(data_ptr, struct_template))
	end
	return ret
end

function config_data_center.getConfigDataByTwoID(config_name, key_name1, key_value1, key_name2, key_value2)
	if type(config_name) ~= "string" or type(key_name1) ~= "string" or type(key_name2) ~= "string" then
		fatal("参数非法")
		return nil
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local modified = GetFromModifiedConfigs(config_name, true, key_name1, key_value1, key_name2, key_value2)
	if modified then
		return modified
	end

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return nil
	end

	local data_ptr, error_code = GData.GetDataPtrByTwoID(config_name, key_name1, key_value1, key_name2, key_value2, true)
	if error_code then
		LogGDataError(error_code, true, config_name, key_name1, key_value1, key_name2, key_value2)
		return nil
	end

	return CreateStructObj(data_ptr, struct_template)
end

function config_data_center.getConfigDatasByTwoID(config_name, key_name1, key_value1, key_name2, key_value2)
	if type(config_name) ~= "string" or type(key_name1) ~= "string" or type(key_name2) ~= "string" then
		fatal("参数非法")
		return {}
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local modified = GetFromModifiedConfigs(config_name, false, key_name1, key_value1, key_name2, key_value2)
	if modified then
		return modified
	end

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return {}
	end

	local data_ptr_table, error_code = GData.GetDataPtrByTwoID(config_name, key_name1, key_value1, key_name2, key_value2, false)
	if error_code then
		LogGDataError(error_code, true, config_name, key_name1, key_value1, key_name2, key_value2)
		return {}
	end

	local ret = {}
	for _, data_ptr in pairs(data_ptr_table) do
		table.insert(ret, CreateStructObj(data_ptr, struct_template))
	end
	return ret
end

function config_data_center.getConfigDataByThreeID(
		config_name,
		key_name1,
		key_value1,
		key_name2,
		key_value2,
		key_name3,
		key_value3)
	if
	type(config_name) ~= "string" or type(key_name1) ~= "string" or type(key_name2) ~= "string" or
			type(key_name3) ~= "string"
	then
		fatal("参数非法")
		return nil
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local modified =
	GetFromModifiedConfigs(config_name, true, key_name1, key_value1, key_name2, key_value2, key_name3, key_value3)
	if modified then
		return modified
	end

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return nil
	end

	local data_ptr, error_code =
	GData.GetDataPtrByThreeID(
			config_name,
			key_name1,
			key_value1,
			key_name2,
			key_value2,
			key_name3,
			key_value3,
			true
	)
	if error_code then
		LogGDataError(error_code, true, config_name, key_name1, key_value1, key_name2, key_value2)
		return nil
	end

	return CreateStructObj(data_ptr, struct_template)
end

function config_data_center.getConfigDatasByThreeID(
		config_name,
		key_name1,
		key_value1,
		key_name2,
		key_value2,
		key_name3,
		key_value3)
	if
	type(config_name) ~= "string" or type(key_name1) ~= "string" or type(key_name2) ~= "string" or
			type(key_name3) ~= "string"
	then
		fatal("参数非法")
		return {}
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local modified =
	GetFromModifiedConfigs(config_name, false, key_name1, key_value1, key_name2, key_value2, key_name3, key_value3)
	if modified then
		return modified
	end

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return {}
	end

	local data_ptr_table, error_code =
	GData.GetDataPtrByThreeID(
			config_name,
			key_name1,
			key_value1,
			key_name2,
			key_value2,
			key_name3,
			key_value3,
			false
	)
	if error_code then
		LogGDataError(error_code, true, config_name, key_name1, key_value1, key_name2, key_value2)
		return {}
	end

	local ret = {}
	for _, data_ptr in pairs(data_ptr_table) do
		table.insert(ret, CreateStructObj(data_ptr, struct_template))
	end

	return ret
end

function config_data_center.getEntryCount(config_name)
	if type(config_name) ~= "string" then
		fatal("参数非法")
		return 0
	end

	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	return GData.GetEntryCount(config_name)
end


function config_data_center.GetGlobalConfigByID(id)
	return config_data_center.getConfigDataByID("dataconfig_globalconfig", "id", id).GlobalValue
end

function config_data_center.getConfigData(config_name, raw)
	if type(config_name) ~= "string" then
		return nil
	end
	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local struct_template = all_config_map[config_name]
	if not struct_template then
		fatal(string.format("cannot find lua descript info for config: %s！", config_name))
		return nil
	end
	if modified_configs[config_name] and not raw then
		return modified_configs[config_name]
	end

	local t, error_code = GData.GetConfigData(config_name)
	if error_code then
		LogGDataError(error_code, false, config_name)
		return nil
	end

	local ret = {}
	for index, data_ptr in pairs(t) do
		ret[index] = CreateStructObj(data_ptr, struct_template)
	end
	return ret
end

local debug_index = 0

local DeepCopy
DeepCopy = function(config_obj, visited)
	if visited[config_obj] then
		return
	end

	visited[config_obj] = true

	local ret = {}

	if not IsGdataObj(config_obj) then
		if not IsReleaseVer then
			warning("deepcopy with invalid config_obj")
		end
		return ret
	end

	local meta = getmetatable(config_obj)
	for k, v in meta.__iter(config_obj) do
		if IsGdataObj(v) then
			ret[k] = DeepCopy(v, visited)
		else
			ret[k] = v
		end
	end

	return ret
end

--[[
    退化为lua表，内存占用高，同config_data_center.getConfigData一样，不应该存在。以后会删除此方法.
]]
function config_data_center.getOriginalConfigCopy(config_name)
	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	local raw_configs = config_data_center.getConfigData(config_name, true)

	if not raw_configs then
		if not IsReleaseVer then
			warning(string.format("invalid config name: %s", config_name))
		end
		return {}
	end

	debug_config_name = config_name

	local t, visited = {}, {}
	for index, raw_config in pairs(raw_configs) do
		debug_index = index
		t[index] = DeepCopy(raw_config, visited)
	end
	return t
end

--[[
    深拷贝为lua表，内存占用高，同config_data_center.getConfigData一样，不应该存在。以后会删除此方法.
]]
function config_data_center.copyFromConfig(config_userdata_obj)
	return DeepCopy(config_userdata_obj, {})
end

function config_data_center.setConfigProtobufItemsByName(config_name, items)
	config_name = string.lower(config_name)
	config_name = getIosCheckconfig_name(config_name)

	modified_configs[config_name] = items
end

function config_data_center.ClearElementDataDictByConfigName(configName)
	--do nothing,接口兼容，防改漏报错
end

--[[--
 * @Description: 通过自定义函数得到配置数据
 * @param:        configName config对应的Bytes文件名
                  func       自定义函数
 * @return:      对应protobuf结构的items字段
 ]]
function config_data_center.getConfigDataByFunc(config_name, func)
	local ret = nil
	local configDataArray = config_data_center.getConfigData(config_name)

	if (func ~= nil and type(func) == "function") then
		for k, v in pairs(configDataArray) do
			if (func(v)) then
				ret = v
				break
			end
		end
	end

	return ret
end

--[[--
 * @Description: 通过自定义函数得到配置数据(多个)
 * @param:        configName config对应的Bytes文件名
                  func       自定义函数
 * @return:      对应protobuf结构的items字段
 ]]
function config_data_center.getConfigDatasByFunc(config_name, func)
	local ret = {}
	local configDataArray = config_data_center.getConfigData(config_name)

	if (func ~= nil and type(func) == "function") then
		for k, v in pairs(configDataArray) do
			if (func(v)) then
				table.insert(ret, v)
			end
		end
	end

	return ret
end

function config_data_center.PreLoadConfigData()
	--do nothing,接口兼容，防改漏报错
end

local gpairs = _G.pairs
_G.pairs = function(t)
	if IsGdataObj(t) then
		return getmetatable(t).__iter(t)
	else
		return gpairs(t)
	end
end

local gipairs = _G.ipairs
_G.ipairs = function(t)
	if IsGdataObj(t) then
		return getmetatable(t).__iter(t)
	else
		return gipairs(t)
	end
end

local getn = table.getn
local table_bk = table

_G.table = {}

for k, v in pairs(table_bk) do
	table[k] = v
end

table.getn = function(target)
	return #target
end

local gMaxN = table.maxn
table.maxn = function(target)
	if IsGdataObj(target) then
		return #target
	else
		return gMaxN(target)
	end
end

-------for debug-----------
function config_data_center.getTemplate(configName)
	return all_config_map[configName]
end