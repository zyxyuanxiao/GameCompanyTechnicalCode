---@class LuaClient : MonoBehaviour
---@field public Instance LuaClient
LuaClient={ }
---@public
---@param ip string
---@return void
function LuaClient:OpenZbsDebugger(ip) end
---@public
---@return void
function LuaClient:Destroy() end
---@public
---@return LuaState
function LuaClient.GetMainState() end
---@public
---@return LuaLooper
function LuaClient:GetLooper() end
---@public
---@return LuaFunction
function LuaClient:GetOnSocketFunc() end
---@public
---@return LuaFunction
function LuaClient:GetOnSocketSocialFunc() end
---@public
---@return void
function LuaClient:AttachProfiler() end
---@public
---@return void
function LuaClient:DetachProfiler() end
---@public
---@return string
function LuaClient:LuaTraceback() end
