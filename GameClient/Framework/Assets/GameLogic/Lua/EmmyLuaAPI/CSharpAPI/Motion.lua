---@class Motion : Object
---@field public averageDuration number
---@field public averageAngularSpeed number
---@field public averageSpeed Vector3
---@field public apparentSpeed number
---@field public isLooping boolean
---@field public legacy boolean
---@field public isHumanMotion boolean
---@field public isAnimatorMotion boolean
Motion={ }
---@public
---@param val boolean
---@return boolean
function Motion:ValidateIfRetargetable(val) end
