---@class LuaFunction : LuaBaseRef
LuaFunction={ }
---@public
---@return void
function LuaFunction:Dispose() end
---@public
---@return number
function LuaFunction:BeginPCall() end
---@public
---@return void
function LuaFunction:PCall() end
---@public
---@return void
function LuaFunction:EndPCall() end
---@public
---@return void
function LuaFunction:Call() end
---@public
---@param args Object[]
---@return Object[]
function LuaFunction:LazyCall(args) end
---@public
---@param args number
---@return void
function LuaFunction:CheckStack(args) end
---@public
---@return boolean
function LuaFunction:IsBegin() end
---@public
---@param num number
---@return void
function LuaFunction:Push(num) end
---@public
---@param n number
---@return void
function LuaFunction:Push(n) end
---@public
---@param n LayerMask
---@return void
function LuaFunction:PushLayerMask(n) end
---@public
---@param un number
---@return void
function LuaFunction:Push(un) end
---@public
---@param num number
---@return void
function LuaFunction:Push(num) end
---@public
---@param un number
---@return void
function LuaFunction:Push(un) end
---@public
---@param b boolean
---@return void
function LuaFunction:Push(b) end
---@public
---@param str string
---@return void
function LuaFunction:Push(str) end
---@public
---@param ptr IntPtr
---@return void
function LuaFunction:Push(ptr) end
---@public
---@param lbr LuaBaseRef
---@return void
function LuaFunction:Push(lbr) end
---@public
---@param o Object
---@return void
function LuaFunction:Push(o) end
---@public
---@param o Object
---@return void
function LuaFunction:Push(o) end
---@public
---@param t Type
---@return void
function LuaFunction:Push(t) end
---@public
---@param e Enum
---@return void
function LuaFunction:Push(e) end
---@public
---@param array Array
---@return void
function LuaFunction:Push(array) end
---@public
---@param v3 Vector3
---@return void
function LuaFunction:Push(v3) end
---@public
---@param v2 Vector2
---@return void
function LuaFunction:Push(v2) end
---@public
---@param v4 Vector4
---@return void
function LuaFunction:Push(v4) end
---@public
---@param quat Quaternion
---@return void
function LuaFunction:Push(quat) end
---@public
---@param clr Color
---@return void
function LuaFunction:Push(clr) end
---@public
---@param ray Ray
---@return void
function LuaFunction:Push(ray) end
---@public
---@param bounds Bounds
---@return void
function LuaFunction:Push(bounds) end
---@public
---@param hit RaycastHit
---@return void
function LuaFunction:Push(hit) end
---@public
---@param t Touch
---@return void
function LuaFunction:Push(t) end
---@public
---@param buffer LuaByteBuffer
---@return void
function LuaFunction:Push(buffer) end
---@public
---@param o Object
---@return void
function LuaFunction:PushObject(o) end
---@public
---@param args Object[]
---@return void
function LuaFunction:PushArgs(args) end
---@public
---@param buffer Byte[]
---@param len number
---@return void
function LuaFunction:PushByteBuffer(buffer, len) end
---@public
---@param buffer Byte[]
---@param length number
---@return void
function LuaFunction:PushByteBufferExt(buffer, length) end
---@public
---@return number
function LuaFunction:CheckNumber() end
---@public
---@return boolean
function LuaFunction:CheckBoolean() end
---@public
---@return string
function LuaFunction:CheckString() end
---@public
---@return Vector3
function LuaFunction:CheckVector3() end
---@public
---@return Quaternion
function LuaFunction:CheckQuaternion() end
---@public
---@return Vector2
function LuaFunction:CheckVector2() end
---@public
---@return Vector4
function LuaFunction:CheckVector4() end
---@public
---@return Color
function LuaFunction:CheckColor() end
---@public
---@return Ray
function LuaFunction:CheckRay() end
---@public
---@return Bounds
function LuaFunction:CheckBounds() end
---@public
---@return LayerMask
function LuaFunction:CheckLayerMask() end
---@public
---@return number
function LuaFunction:CheckLong() end
---@public
---@return number
function LuaFunction:CheckULong() end
---@public
---@return Delegate
function LuaFunction:CheckDelegate() end
---@public
---@return Object
function LuaFunction:CheckVariant() end
---@public
---@return Char[]
function LuaFunction:CheckCharBuffer() end
---@public
---@return Byte[]
function LuaFunction:CheckByteBuffer() end
---@public
---@param t Type
---@return Object
function LuaFunction:CheckObject(t) end
---@public
---@return LuaFunction
function LuaFunction:CheckLuaFunction() end
---@public
---@return LuaTable
function LuaFunction:CheckLuaTable() end
---@public
---@return LuaThread
function LuaFunction:CheckLuaThread() end
