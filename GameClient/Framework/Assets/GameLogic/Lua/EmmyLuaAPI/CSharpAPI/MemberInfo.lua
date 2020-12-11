---@class MemberInfo : Object
---@field public MemberType number
---@field public Name string
---@field public DeclaringType Type
---@field public ReflectedType Type
---@field public CustomAttributes IEnumerable
---@field public MetadataToken number
---@field public Module Module
MemberInfo={ }
---@public
---@param inherit boolean
---@return Object[]
function MemberInfo:GetCustomAttributes(inherit) end
---@public
---@param attributeType Type
---@param inherit boolean
---@return Object[]
function MemberInfo:GetCustomAttributes(attributeType, inherit) end
---@public
---@param attributeType Type
---@param inherit boolean
---@return boolean
function MemberInfo:IsDefined(attributeType, inherit) end
---@public
---@return IList
function MemberInfo:GetCustomAttributesData() end
---@public
---@param left MemberInfo
---@param right MemberInfo
---@return boolean
function MemberInfo.op_Equality(left, right) end
---@public
---@param left MemberInfo
---@param right MemberInfo
---@return boolean
function MemberInfo.op_Inequality(left, right) end
---@public
---@param obj Object
---@return boolean
function MemberInfo:Equals(obj) end
---@public
---@return number
function MemberInfo:GetHashCode() end
