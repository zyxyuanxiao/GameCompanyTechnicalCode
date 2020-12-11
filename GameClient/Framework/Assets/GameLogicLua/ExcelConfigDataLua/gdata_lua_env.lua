local DefineValueType = function(type_name, byte_size, reader, is_string)
    return {
        type_name = type_name,
        byte_size = byte_size,
        reader = reader,
        value_type = true,
        is_string = not (not is_string)
    }
end

local Type_UINT32 = DefineValueType("uint32", 4, GData.ReadUInt32)
local Type_SINT32 = DefineValueType("sint32", 4, GData.ReadSInt32)
local Type_UINT64 = DefineValueType("uint64", 8, GData.ReadUInt64)
local Type_SINT64 = DefineValueType("sint64", 8, GData.ReadSInt64)
local Type_FLOAT = DefineValueType("float", 8, GData.ReadFloat)
local Type_BOOL = DefineValueType("bool", 1, GData.ReadBool)
local Type_String = DefineValueType("string", 4, GData.ReadString, true)

local FieldDefine = function(type_desc)
    return function(field_name)
        return {
            field_name = field_name,
            type_desc = type_desc,
            __optional = false,
            optional = function(this)
                this.__optional = true
                return this
            end
        }
    end
end

local ArrayDefine = function(type_desc)
    return function(field_name)
        return {
            field_name = field_name,
            type_desc = type_desc,
            array_type = true
        }
    end
end

local env = {}
env.UINT32 = FieldDefine(Type_UINT32)
env.SINT32 = FieldDefine(Type_SINT32)
env.UINT64 = FieldDefine(Type_UINT64)
env.SINT64 = FieldDefine(Type_SINT64)
env.FLOAT = FieldDefine(Type_FLOAT)
env.BOOL = FieldDefine(Type_BOOL)
env.STRING = FieldDefine(Type_String)
--[[env.Struct = function(struct_desc)
    return FieldDefine(struct_desc)
end]]

env.UINT32_Array = ArrayDefine(Type_UINT32)
env.SINT32_Array = ArrayDefine(Type_SINT32)
env.UINT64_Array = ArrayDefine(Type_UINT64)
env.SINT64_Array = ArrayDefine(Type_SINT64)
env.FLOAT_Array = ArrayDefine(Type_FLOAT)
env.BOOL_Array = ArrayDefine(Type_BOOL)
env.STRING_Array = ArrayDefine(Type_String)
env.Struct_Array = ArrayDefine

local ARRAY_COUNT_SIZE = 2
local FIXED_HEAD_SiZE = 2
local ParseStructDesc
ParseStructDesc = function(struct_desc)
    local struct, fixed_offset, fixed_optional_index, variable_attr_index = {}, FIXED_HEAD_SiZE, 0, 0
    for i = 1, #struct_desc do
        local desc = struct_desc[i]
        local field = {}

        if desc.array_type then
            if desc.type_desc.value_type then
                field.meta_info = desc
            else
                field.meta_info = ParseStructDesc(desc.type_desc)
                field.meta_info.array_type = true
            end
            field.variable_attr_index = variable_attr_index
            variable_attr_index = variable_attr_index + 1
        elseif desc.type_desc.value_type then
            field.meta_info = desc
            if desc.__optional then
                field.fixed_optional_index  = fixed_optional_index
                fixed_optional_index = fixed_optional_index + 1
            end
            field.fixed_offset = fixed_offset
            fixed_offset = fixed_offset + desc.type_desc.byte_size
        else
            assert(false)
        end

        struct[desc.field_name] = field
    end
    return struct
end

local all_config_map = {}

env.Config = function(config_name)
    return function(config_desc)
        all_config_map[config_name] = ParseStructDesc(config_desc)
    end
end

env.GetAllConfig = function()
    return all_config_map
end
return env
