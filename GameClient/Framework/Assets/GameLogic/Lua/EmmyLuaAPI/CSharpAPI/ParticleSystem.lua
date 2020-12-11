---@class ParticleSystem : Component
---@field public safeCollisionEventSize number
---@field public startDelay number
---@field public loop boolean
---@field public playOnAwake boolean
---@field public duration number
---@field public playbackSpeed number
---@field public enableEmission boolean
---@field public emissionRate number
---@field public startSpeed number
---@field public startSize number
---@field public startColor Color
---@field public startRotation number
---@field public startRotation3D Vector3
---@field public startLifetime number
---@field public gravityModifier number
---@field public maxParticles number
---@field public simulationSpace number
---@field public scalingMode number
---@field public automaticCullingEnabled boolean
---@field public isPlaying boolean
---@field public isEmitting boolean
---@field public isStopped boolean
---@field public isPaused boolean
---@field public particleCount number
---@field public time number
---@field public randomSeed number
---@field public useAutoRandomSeed boolean
---@field public proceduralSimulationSupported boolean
---@field public main MainModule
---@field public emission EmissionModule
---@field public shape ShapeModule
---@field public velocityOverLifetime VelocityOverLifetimeModule
---@field public limitVelocityOverLifetime LimitVelocityOverLifetimeModule
---@field public inheritVelocity InheritVelocityModule
---@field public forceOverLifetime ForceOverLifetimeModule
---@field public colorOverLifetime ColorOverLifetimeModule
---@field public colorBySpeed ColorBySpeedModule
---@field public sizeOverLifetime SizeOverLifetimeModule
---@field public sizeBySpeed SizeBySpeedModule
---@field public rotationOverLifetime RotationOverLifetimeModule
---@field public rotationBySpeed RotationBySpeedModule
---@field public externalForces ExternalForcesModule
---@field public noise NoiseModule
---@field public collision CollisionModule
---@field public trigger TriggerModule
---@field public subEmitters SubEmittersModule
---@field public textureSheetAnimation TextureSheetAnimationModule
---@field public lights LightsModule
---@field public trails TrailModule
---@field public customData CustomDataModule
ParticleSystem={ }
---@public
---@param position Vector3
---@param velocity Vector3
---@param size number
---@param lifetime number
---@param color Color32
---@return void
function ParticleSystem:Emit(position, velocity, size, lifetime, color) end
---@public
---@param particle Particle
---@return void
function ParticleSystem:Emit(particle) end
---@public
---@param particles Particle[]
---@param size number
---@param offset number
---@return void
function ParticleSystem:SetParticles(particles, size, offset) end
---@public
---@param particles Particle[]
---@param size number
---@return void
function ParticleSystem:SetParticles(particles, size) end
---@public
---@param particles Particle[]
---@return void
function ParticleSystem:SetParticles(particles) end
---@public
---@param particles NativeArray
---@param size number
---@param offset number
---@return void
function ParticleSystem:SetParticles(particles, size, offset) end
---@public
---@param particles NativeArray
---@param size number
---@return void
function ParticleSystem:SetParticles(particles, size) end
---@public
---@param particles NativeArray
---@return void
function ParticleSystem:SetParticles(particles) end
---@public
---@param particles Particle[]
---@param size number
---@param offset number
---@return number
function ParticleSystem:GetParticles(particles, size, offset) end
---@public
---@param particles Particle[]
---@param size number
---@return number
function ParticleSystem:GetParticles(particles, size) end
---@public
---@param particles Particle[]
---@return number
function ParticleSystem:GetParticles(particles) end
---@public
---@param particles NativeArray
---@param size number
---@param offset number
---@return number
function ParticleSystem:GetParticles(particles, size, offset) end
---@public
---@param particles NativeArray
---@param size number
---@return number
function ParticleSystem:GetParticles(particles, size) end
---@public
---@param particles NativeArray
---@return number
function ParticleSystem:GetParticles(particles) end
---@public
---@param customData List
---@param streamIndex number
---@return void
function ParticleSystem:SetCustomParticleData(customData, streamIndex) end
---@public
---@param customData List
---@param streamIndex number
---@return number
function ParticleSystem:GetCustomParticleData(customData, streamIndex) end
---@public
---@return PlaybackState
function ParticleSystem:GetPlaybackState() end
---@public
---@param playbackState PlaybackState
---@return void
function ParticleSystem:SetPlaybackState(playbackState) end
---@public
---@return Trails
function ParticleSystem:GetTrails() end
---@public
---@param trailData Trails
---@return void
function ParticleSystem:SetTrails(trailData) end
---@public
---@param t number
---@param withChildren boolean
---@param restart boolean
---@param fixedTimeStep boolean
---@return void
function ParticleSystem:Simulate(t, withChildren, restart, fixedTimeStep) end
---@public
---@param t number
---@param withChildren boolean
---@param restart boolean
---@return void
function ParticleSystem:Simulate(t, withChildren, restart) end
---@public
---@param t number
---@param withChildren boolean
---@return void
function ParticleSystem:Simulate(t, withChildren) end
---@public
---@param t number
---@return void
function ParticleSystem:Simulate(t) end
---@public
---@param withChildren boolean
---@return void
function ParticleSystem:Play(withChildren) end
---@public
---@return void
function ParticleSystem:Play() end
---@public
---@param withChildren boolean
---@return void
function ParticleSystem:Pause(withChildren) end
---@public
---@return void
function ParticleSystem:Pause() end
---@public
---@param withChildren boolean
---@param stopBehavior number
---@return void
function ParticleSystem:Stop(withChildren, stopBehavior) end
---@public
---@param withChildren boolean
---@return void
function ParticleSystem:Stop(withChildren) end
---@public
---@return void
function ParticleSystem:Stop() end
---@public
---@param withChildren boolean
---@return void
function ParticleSystem:Clear(withChildren) end
---@public
---@return void
function ParticleSystem:Clear() end
---@public
---@param withChildren boolean
---@return boolean
function ParticleSystem:IsAlive(withChildren) end
---@public
---@return boolean
function ParticleSystem:IsAlive() end
---@public
---@param count number
---@return void
function ParticleSystem:Emit(count) end
---@public
---@param emitParams EmitParams
---@param count number
---@return void
function ParticleSystem:Emit(emitParams, count) end
---@public
---@param subEmitterIndex number
---@return void
function ParticleSystem:TriggerSubEmitter(subEmitterIndex) end
---@public
---@param subEmitterIndex number
---@param particle Particle
---@return void
function ParticleSystem:TriggerSubEmitter(subEmitterIndex, particle) end
---@public
---@param subEmitterIndex number
---@param particles List
---@return void
function ParticleSystem:TriggerSubEmitter(subEmitterIndex, particles) end
---@public
---@return void
function ParticleSystem.ResetPreMappedBufferMemory() end
