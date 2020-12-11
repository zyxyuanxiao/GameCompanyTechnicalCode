---@class SkinnedMeshRenderer : Renderer
---@field public quality number
---@field public updateWhenOffscreen boolean
---@field public forceMatrixRecalculationPerRender boolean
---@field public rootBone Transform
---@field public bones Transform[]
---@field public sharedMesh Mesh
---@field public skinnedMotionVectors boolean
---@field public localBounds Bounds
SkinnedMeshRenderer={ }
---@public
---@param index number
---@return number
function SkinnedMeshRenderer:GetBlendShapeWeight(index) end
---@public
---@param index number
---@param value number
---@return void
function SkinnedMeshRenderer:SetBlendShapeWeight(index, value) end
---@public
---@param mesh Mesh
---@return void
function SkinnedMeshRenderer:BakeMesh(mesh) end
