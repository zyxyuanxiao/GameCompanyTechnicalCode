---@class LuaByteBuffer : ValueType
---@field public buffer Byte[]
---@field public Length number
LuaByteBuffer={ }
---@public
---@param stream MemoryStream
---@return LuaByteBuffer
function LuaByteBuffer.op_Implicit(stream) end
