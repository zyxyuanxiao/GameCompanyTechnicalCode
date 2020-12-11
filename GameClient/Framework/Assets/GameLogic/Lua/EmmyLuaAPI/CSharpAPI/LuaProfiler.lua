---@class LuaProfiler : Object
---@field public list List
LuaProfiler={ }
---@public
---@return void
function LuaProfiler.Clear() end
---@public
---@param name string
---@return number
function LuaProfiler.GetID(name) end
---@public
---@param id number
---@return void
function LuaProfiler.BeginSample(id) end
---@public
---@return void
function LuaProfiler.EndSample() end
