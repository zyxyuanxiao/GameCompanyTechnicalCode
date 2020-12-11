---@class TrackedReference : Object
TrackedReference={ }
---@public
---@param x TrackedReference
---@param y TrackedReference
---@return boolean
function TrackedReference.op_Equality(x, y) end
---@public
---@param x TrackedReference
---@param y TrackedReference
---@return boolean
function TrackedReference.op_Inequality(x, y) end
---@public
---@param o Object
---@return boolean
function TrackedReference:Equals(o) end
---@public
---@return number
function TrackedReference:GetHashCode() end
---@public
---@param exists TrackedReference
---@return boolean
function TrackedReference.op_Implicit(exists) end
