---@class LuaTable : LuaBaseRef
---@field public Item Object
---@field public Item Object
---@field public Length number
LuaTable={ }
---@public
---@param key string
---@return LuaFunction
function LuaTable:RawGetLuaFunction(key) end
---@public
---@param key string
---@return LuaFunction
function LuaTable:GetLuaFunction(key) end
---@public
---@param name string
---@return void
function LuaTable:Call(name) end
---@public
---@param name string
---@return string
function LuaTable:GetStringField(name) end
---@public
---@param name string
---@return void
function LuaTable:AddTable(name) end
---@public
---@return Object[]
function LuaTable:ToArray() end
---@public
---@return string
function LuaTable:ToString() end
---@public
---@return LuaArrayTable
function LuaTable:ToArrayTable() end
---@public
---@return LuaDictTable
function LuaTable:ToDictTable() end
---@public
---@return LuaTable
function LuaTable:GetMetaTable() end
