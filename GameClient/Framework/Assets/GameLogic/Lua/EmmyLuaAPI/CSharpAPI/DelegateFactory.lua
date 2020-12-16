---@class DelegateFactory : Object
---@field public dict Dictionary`2
DelegateFactory={ }
---@public
---@return void
function DelegateFactory.Init() end
---@public
---@return void
function DelegateFactory.Register() end
---@public
---@param t Type
---@param func LuaFunction
---@return Delegate
function DelegateFactory.CreateDelegate(t, func) end
---@public
---@param t Type
---@param func LuaFunction
---@param self LuaTable
---@return Delegate
function DelegateFactory.CreateDelegate(t, func, self) end
---@public
---@param obj Delegate
---@param func LuaFunction
---@return Delegate
function DelegateFactory.RemoveDelegate(obj, func) end
---@public
---@param obj Delegate
---@param dg Delegate
---@return Delegate
function DelegateFactory.RemoveDelegate(obj, dg) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Action
function DelegateFactory:System_Action(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return UnityAction
function DelegateFactory:UnityEngine_Events_UnityAction(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Predicate
function DelegateFactory:System_Predicate_int(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Action
function DelegateFactory:System_Action_int(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Comparison
function DelegateFactory:System_Comparison_int(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Func`2
function DelegateFactory:System_Func_int_int(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return CameraCallback
function DelegateFactory:UnityEngine_Camera_CameraCallback(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return AdvertisingIdentifierCallback
function DelegateFactory:UnityEngine_Application_AdvertisingIdentifierCallback(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return LowMemoryCallback
function DelegateFactory:UnityEngine_Application_LowMemoryCallback(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return LogCallback
function DelegateFactory:UnityEngine_Application_LogCallback(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Action
function DelegateFactory:System_Action_bool(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Action
function DelegateFactory:System_Action_string(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Func
function DelegateFactory:System_Func_bool(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return PCMReaderCallback
function DelegateFactory:UnityEngine_AudioClip_PCMReaderCallback(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return PCMSetPositionCallback
function DelegateFactory:UnityEngine_AudioClip_PCMSetPositionCallback(func, self, flag) end
---@public
---@param func LuaFunction
---@param self LuaTable
---@param flag boolean
---@return Action
function DelegateFactory:System_Action_UnityEngine_AsyncOperation(func, self, flag) end
