---@class AsyncOperation : YieldInstruction
---@field public isDone boolean
---@field public progress number
---@field public priority number
---@field public allowSceneActivation boolean
AsyncOperation={ }
---@public
---@param value Action
---@return void
function AsyncOperation:add_completed(value) end
---@public
---@param value Action
---@return void
function AsyncOperation:remove_completed(value) end
