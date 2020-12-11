---@class Input : Object
---@field public simulateMouseWithTouches boolean
---@field public anyKey boolean
---@field public anyKeyDown boolean
---@field public inputString string
---@field public mousePosition Vector3
---@field public mouseScrollDelta Vector2
---@field public imeCompositionMode number
---@field public compositionString string
---@field public imeIsSelected boolean
---@field public compositionCursorPos Vector2
---@field public eatKeyPressOnTextFieldFocus boolean
---@field public mousePresent boolean
---@field public touchCount number
---@field public touchPressureSupported boolean
---@field public stylusTouchSupported boolean
---@field public touchSupported boolean
---@field public multiTouchEnabled boolean
---@field public isGyroAvailable boolean
---@field public deviceOrientation number
---@field public acceleration Vector3
---@field public compensateSensors boolean
---@field public accelerationEventCount number
---@field public backButtonLeavesApp boolean
---@field public location LocationService
---@field public compass Compass
---@field public gyro Gyroscope
---@field public touches Touch[]
---@field public accelerationEvents AccelerationEvent[]
Input={ }
---@public
---@param axisName string
---@return number
function Input.GetAxis(axisName) end
---@public
---@param axisName string
---@return number
function Input.GetAxisRaw(axisName) end
---@public
---@param buttonName string
---@return boolean
function Input.GetButton(buttonName) end
---@public
---@param buttonName string
---@return boolean
function Input.GetButtonDown(buttonName) end
---@public
---@param buttonName string
---@return boolean
function Input.GetButtonUp(buttonName) end
---@public
---@param button number
---@return boolean
function Input.GetMouseButton(button) end
---@public
---@param button number
---@return boolean
function Input.GetMouseButtonDown(button) end
---@public
---@param button number
---@return boolean
function Input.GetMouseButtonUp(button) end
---@public
---@return void
function Input.ResetInputAxes() end
---@public
---@param joystickName string
---@return boolean
function Input.IsJoystickPreconfigured(joystickName) end
---@public
---@return String[]
function Input.GetJoystickNames() end
---@public
---@param index number
---@return Touch
function Input.GetTouch(index) end
---@public
---@param index number
---@return AccelerationEvent
function Input.GetAccelerationEvent(index) end
---@public
---@param key number
---@return boolean
function Input.GetKey(key) end
---@public
---@param name string
---@return boolean
function Input.GetKey(name) end
---@public
---@param key number
---@return boolean
function Input.GetKeyUp(key) end
---@public
---@param name string
---@return boolean
function Input.GetKeyUp(name) end
---@public
---@param key number
---@return boolean
function Input.GetKeyDown(key) end
---@public
---@param name string
---@return boolean
function Input.GetKeyDown(name) end
