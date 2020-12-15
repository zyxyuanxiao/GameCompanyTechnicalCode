--//////////////////////////日志控制相关 start ////////////////////////

TIME_NAME = {
    SECOND = 1,
    MINUTE = 2,
    HOUR = 3,
    DAY = 4
}

LOG = {
    task = 1, -- 任务
    skill = 2, -- 技能
    ai = 3, -- AI
    level_single = 4, -- 单人副本
    level_multi = 5, -- 多人副本
    login = 6, -- 登录
    world = 7, -- 世界地图
    hall = 8, -- 大厅
    store = 9, -- 商店
    back_pack = 10, -- 背包
    friend = 11, -- 好友
    team = 12, -- 组队
    anyTest = 13,
    scene_ui = 14,
    gs = 15, -- 游戏状态
    pvp = 16, -- pvp
    huangcheng = 17, -- huangcheng
    tttower = 18
}

-- 模块的前缀
local logFilterDict = {
    [LOG.task] = "[task]",
    [LOG.skill] = "[skill]",
    [LOG.ai] = "[ai]",
    [LOG.level_single] = "[level_single]",
    [LOG.level_multi] = "[level_multi]",
    [LOG.login] = "[login]",
    [LOG.world] = "[world]",
    [LOG.hall] = "[hall]",
    [LOG.back_pack] = "[back_pack]",
    [LOG.friend] = "[friend]",
    [LOG.anyTest] = "[anyTest]",
    [LOG.scene_ui] = "[scene_ui]",
    [LOG.gs] = "[LOG.gs]",
    [LOG.pvp] = "[LOG.pvp]",
    [LOG.huangcheng] = "[LOG.huangcheng]",
    [LOG.tttower] = "[LOG.tttower]"
}

BestLog = {}
BestLog.filterCode = nil
 -- 过滤码默认为空

local resMgr = nil

local prefixLog = ""
local function MakePrefixStr(filterCode)
    if (filterCode ~= nil and logFilterDict[filterCode] ~= nil) then
        prefixLog = logFilterDict[filterCode]
    else
        prefixLog = ""
    end
end


----错误日志--
--function fatal(str)
--    Debugger.LogError(str)
--end

--错误日志--
function fatal(str,...)
    local f_str = str
    local num_args = {...}
    for _, v in pairs(num_args) do
        f_str = f_str .."  " .. v
    end
    Debugger.LogError(f_str)
end

function Info(str, filterCode ,bolColor)
    if (BestLog.filterCode == filterCode or BestLog.filterCode == nil) then
        MakePrefixStr(filterCode)
        local strValue = str
        if (type(str) ~= "string") then
            strValue = tostring(str)
        end
        local content = ""
        if IsReleaseVer then
            content = prefixLog..strValue
        else
            content = bolColor and AddColorToString(prefixLog..strValue) or prefixLog..strValue
        end
        print("Info:"..content)
    end
end

function AddColorToString(str)
    if (type(str) ~= "string") then
        str = tostring(str)
    end
    local content = string.format("<color=Orange> %s </color>",str)
    return content
end

--设置日志等级--
--[[function SetLoggerLevel(level)
	Util.SetLogLevel(level);
end]]
--设置日志打印开关--
--[[function SetWriteLoggerToggle(toggle)
	Util.SetLogLevel(toggle);
end]]
--//////////////////////////日志控制相关 end ////////////////////////


function try(block)
	local main = block.main
	local catch = block.catch
	local finally = block.finally
	assert(main)

	-- try to call it
	local ok, errors = xpcall(main, debug.traceback)
	if not ok then
		-- run the catch function
		if catch then
			catch(errors)
		end
	end

	-- run the finally function
	if finally then
		finally(ok, errors)
	end

	-- ok?
	if ok then
		return errors
	end
end

--查找对象--
function find(str)
    return GameObject.Find(str)
end

function destroy(obj)
    if (not IsNil(obj)) then
        GameObject.Destroy(obj)
    end
end

function destroyImmediate(obj)
    if (not IsNil(obj)) then
        GameObject.DestroyImmediate(obj)
    end
end

function newobject(prefab)
    if prefab ~= nil then
        return GameObject.Instantiate(prefab)
    else
        return nil
    end
end

--同步加载资源对象 ----Start----------->
function newNormalObjSync(path, type)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    local abp = AssetBundleParams.New(path, type)
    local retGo = resMgr:LoadNormalObjSync(abp)

    return retGo
end

function newSceneResidentMemoryObjSync(path, type)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    local abp = AssetBundleParams.New(path, type)
    local retGo = resMgr:LoadSceneResidentMemoryObjSync(abp)

    return retGo
end

function newResidentMemoryObjSync(path, type)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    local abp = AssetBundleParams.New(path, type)
    local retGo = resMgr:LoadResidentMemoryObjSync(abp)

    return retGo
end
--同步加载资源对象 ----End----------->

--异步加载资源对象 ----Start----------->
function newNormalObjAsync(path, type, loadCallback, isSort)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    local abp = AssetBundleParams.New(path, type)
    if isSort ~= nil then
        abp.IsSort = isSort
    end
    resMgr:LoadNormalObjAsync(abp, Best.AssetBundleInfo.LoadAssetCompleteHandler(loadCallback))
end

function newSceneResidentMemoryObjAsync(path, type, loadCallback, isSort)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    local abp = AssetBundleParams.New(path, type)
    if isSort ~= nil then
        abp.IsSort = isSort
    end
    resMgr:LoadSceneResidentMemoryObjAsync(abp, Best.AssetBundleInfo.LoadAssetCompleteHandler(loadCallback))
end

function newResidentMemoryObjAsync(path, type, loadCallback, isSort)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    local abp = AssetBundleParams.New(path, type)
    if isSort ~= nil then
        abp.IsSort = isSort
    end
    resMgr:LoadResidentMemoryObjAsync(abp, Best.AssetBundleInfo.LoadAssetCompleteHandler(loadCallback))
end
--异步加载资源对象 ----End----------->

--同步加载UI预设 ----Start----------->
function newNormalUI(path, parent)
    local prefab = newNormalObjSync(path, typeof(GameObject))
    local obj = newNormalUIprefab(prefab, parent)
    if obj ~= nil then
        local ctrl = obj:AddComponent(typeof(Best.LuaDestroyBundle))
        ctrl.BundleName = path
        ctrl.ResType = typeof(GameObject)
    end

    return obj
end

function newSceneResidentMemoryUI(path, parent)
    local prefab = newSceneResidentMemoryObjSync(path, typeof(GameObject))
    local obj = newNormalUIprefab(prefab, parent)
    if obj ~= nil then
        local ctrl = obj:AddComponent(typeof(Best.LuaDestroyBundle))
        ctrl.BundleName = path
        ctrl.ResType = typeof(GameObject)
    end

    return obj
end

function newResidentMemoryUI(path, parent)
    local prefab = newResidentMemoryObjSync(path, typeof(GameObject))
    local obj = newNormalUIprefab(prefab, parent)
    if obj ~= nil then
        local ctrl = obj:AddComponent(typeof(Best.LuaDestroyBundle))
        ctrl.BundleName = path
        ctrl.ResType = typeof(GameObject)
    end

    return obj
end
--同步加载UI预设 ----End----------->

--异步加载UI预设 ----Start----------->
function newNormalUIAsync(path, func, isSort)
    UISys.Instance:DisableUICamera()
    newNormalObjAsync(path, typeof(GameObject), func, isSort)
end

function newSceneResidentMemoryUIAsync(path, func, isSort)
    UISys.Instance:DisableUICamera()
    newSceneResidentMemoryObjAsync(path, typeof(GameObject), func, isSort)
end

function newResidentMemoryUIAsync(path, func, isSort)
    UISys.Instance:DisableUICamera()
    newResidentMemoryObjAsync(path, typeof(GameObject), func, isSort)
end

function newUIAsyncCallback(abi)
    local go = newNormalUIprefab(abi.mainObject)
    local ctrl = go:AddComponent(typeof(Best.LuaDestroyBundle))
    ctrl.BundleName = abi.GoPath
    ctrl.ResType = abi.GoType
    UISys.Instance:EnableUICamera()

    return go
end
--异步加载UI预设 ----End----------->

function newNormalUIprefab(prefab, parent)
    local obj = newobject(prefab)

    --parent, default ui root
    local ui_root_trans = parent
    if ui_root_trans == nil then
        ui_root_trans = UISys.Instance.transform
    end

    if obj ~= nil and obj.transform.parent ~= ui_root_trans then
        obj.transform.parent = ui_root_trans
        obj.transform.localScale = Vector3.one
        obj.transform.localPosition = Vector3.zero
    end

    return obj
end

function unloadobj(path, type)
    if resMgr == nil then
        resMgr = AppKernel.GetResourceMgr()
    end
    resMgr:UnloadResource(path, type)
end

--[[--
 * @Description: 通过名字获取子控件
 * @param:       父控件的Transform,子控件名字
 * @return:      返回子控件Transform
 ]]
---@public
---@param go Transform
---@param str string
---@return Transform
function child(go, str)
    if go == nil then
        print("go == nil")
        return nil
    end
    return go:Find(str)
end

--[[--
 * @Description: 通过scale来显示或隐藏继承于NUIwidget的控件不会引起panel的rebuild，提升效率
 * @param:       要设置显示或隐藏的的Transform,显示/隐藏
 * @tips:        要注意初始localScale不为1的物体
 ]]
function setUIActiveByScale(trans, active)
    if trans == nil then
        Trace("trans == nil")
        return;
    end

    if active then
        trans.localScale = Vector3.one;
    else
        trans.localScale = Vector3.zero;
    end
end

--[[--
 * @Description: 通过名字获取子控件（深度优先搜索，拿到即止，不需要指定路径，但需要细心规划控件名字，不要重名）
 * @param:       trans      父控件的Transform
                 childName  子控件名字
 * @return:      子控件Transform
 ]]
function child_ext(trans, childName)
    local ret = nil
    local childNum = trans.childCount

    for k = 0, childNum - 1 do
        local childTrans = trans:GetChild(k)
        if (childTrans ~= nil) then
            if (childTrans.gameObject.name == childName) then
                ret = childTrans
                break
            else
                ret = child_ext(childTrans, childName)
                if (ret ~= nil) then
                    break
                end
            end
        end
    end

    return ret
end

--[[--
 * @Description: 获取子控件组件
 * @param:       控件transfrom，组件名
 * @return:      返回子控件Transform
 ]]
function componentGet(trans, typeName)
    if trans == nil then
        print("componentGet trans is nil")
        return nil
    end
    --print(typeName)
    return trans.gameObject:GetComponent(typeName)
end

--[[--
 * @Description: 获取子控件组件
 * @param:       控件transform，子控件名称 , 组件名
 * @return:      返回子控件Transform
 ]]
function subComponentGet(trans, childCompName, typeName)
    if trans == nil then
        print("subComponentGet trans is nil")
        return nil
    end
    local transChild = child(trans, childCompName)
    if transChild == nil then
        return nil
    end
    return transChild.gameObject:GetComponent(typeName)
end

--[[--
 * @Description: 获取子控件组件
 * @param:       控件transform
 		         子控件名称 (非路径方式)
 		         组件名
 * @return:      返回子控件Transform
 ]]
function subComponentGet_ext(trans, childCompName, typeName)
    if trans == nil then
        print("subComponentGet_ext trans is nil")
        return nil
    end
    local transChild = child_ext(trans, childCompName)
    if transChild == nil then
        return nil
    end
    return transChild.gameObject:GetComponent(typeName)
end

function findPanel(str)
    local obj = find(str)
    if obj == nil then
        fatal(str .. " is null")
        return nil
    end
    return obj:GetComponent("BaseLua")
end

--[[--
 * @Description: 按钮点击事件注册  不添加scale效果
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallback_NoScale(trans, para1, para2, para3)
    if (type(para1) == "string") then
        local child_trans = trans:Find(para1)
        if (child_trans ~= nil) then
            local btnObj = child_trans.gameObject

            if (para3 ~= nil) then
                UIEventListener.Get(btnObj).onClick = function(...)
                    para2(para3, ...)
                end
            else
                UIEventListener.Get(btnObj).onClick = para2
            end
        else
            fatal("can not find the control, its name is: " .. para1)
        end
    elseif (type(para1) == "function") then
        if (para2 == nil) then
            UIEventListener.Get(trans.gameObject).onClick = function()
                --ui_audio_mgr.PlayClickAudio(trans.gameObject.name)
                --版审临时添加按钮音效
                if AudioManager.Instance then
                    AudioManager.Instance:Play2DAudio("play_outfight_ui_Btn",false)
                end
                para1(trans.gameObject)
            end
        else
            UIEventListener.Get(trans.gameObject).onClick = function(...)
                --ui_audio_mgr.PlayClickAudio(trans.gameObject.name)
                --版审临时添加按钮音效
                if AudioManager.Instance then
                    AudioManager.Instance:Play2DAudio("play_outfight_ui_Btn",false)
                end
                para1(para2,para3, ...)
            end
        end
    end
    if (para1 == nil) then
        print("para1 is nil")
    end
end

function PlayAudio()
    --ui_audio_mgr.PlayClickAudio("Default")
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallback(trans, para1, para2, para3)
    addClickCallback_NoScale(trans, para1, para2, para3)
    --统一添加buttonscale
    DialogAnimTool.AddButtonScalor(trans.gameObject)
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallback_ext(trans, para1, para2)
    if (type(para1) == "string") then
        local child_trans = child_ext(trans, para1)
        if (child_trans ~= nil) then
            local btnObj = child_trans.gameObject
            UIEventListener.Get(btnObj).onClick = para2
        else
            fatal("can not find the control, its name is: " .. para1)
        end
    elseif (type(para1) == "function") then
        UIEventListener.Get(trans.gameObject).onClick = para1
    end
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addDBClickCallbackSelf(go, callback)
    if (go ~= nil) then
        UIEventListener.Get(go).onDoubleClick = callback
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallbackSelf(go, callback, self)
    if (go ~= nil) then
        if self ~= nil then
            UIEventListener.Get(go).onClick = function(obj)
                callback(self, obj)
            end
        else
            UIEventListener.Get(go).onClick = callback
        end
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressedCallback(parentTrans, controlName, callback)
    local trans = parentTrans:Find(controlName)
    if (trans ~= nil) then
        local btnObj = trans.gameObject
        UIEventListener.Get(btnObj).onPress = callback
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressedCallbackSelf(parentTrans, controlName, callback, self)
    local trans = parentTrans:Find(controlName)
    if (trans ~= nil) then
        local btnObj = trans.gameObject
        if self ~= nil then
            UIEventListenerEx.GetEx(btnObj).onPressed = function(obj)
                callback(self, obj)
            end
        else
            UIEventListenerEx.GetEx(btnObj).onPressed = callback
        end
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addDropCallbackSelf(go, callback, self)
    if (go ~= nil) then
        if (self == nil) then
            UIEventListenerEx.GetEx(go).onDrop = callback
        else
            UIEventListenerEx.GetEx(go).onDrop = function(...)
                callback(self, ...)
            end
        end
    else
        fatal("can not drop the nil control")
    end
end

--[[--
 * @Description: 按钮点击事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addDragCallbackSelf(go, callback)
    if (go ~= nil) then
        UIEventListenerEx.GetEx(go).onDrag = callback
    else
        fatal("can not drop the nil control")
    end
end

function addDragStartCallbackSelf(go, callback, self)
    if (go ~= nil) then
        if (self == nil) then
            UIEventListenerEx.GetEx(go).onDragStart = callback
        else
            UIEventListenerEx.GetEx(go).onDragStart = function(...)
                callback(self, ...)
            end
        end
    else
        fatal("can not drag the nil control")
    end
end

function addDragEndCallbackSelf(go, callback, self)
    if (go ~= nil) then
        if (self == nil) then
            UIEventListenerEx.GetEx(go).onDragEnd = callback
        else
            UIEventListenerEx.GetEx(go).onDragEnd = function(...)
                callback(self, ...)
            end
        end
    else
        fatal("can not drag the nil control")
    end
end

function addSelectCallbackSelf(go, callback, self)
    if (go ~= nil) then
        if self ~= nil then
            UIEventListener.Get(go).onSelect = function(...)
                callback(self, ...)
            end
        else
            UIEventListener.Get(go).onSelect = callback
        end
    else
        fatal("can not drag the nil control")
    end
end

--[[--
 * @Description: press事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressBoolCallback(parentTrans, controlName, callback, self)
    local trans = parentTrans:Find(controlName)
    if (trans ~= nil) then
        local btnObj = trans.gameObject
        if self ~= nil then
            UIEventListenerEx.GetEx(btnObj).onPress = function(...)
                callback(self, ...)
            end
        else
            UIEventListenerEx.GetEx(btnObj).onPress = callback
        end
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description: press事件注册
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressBoolCallbackSelf(go, callback, self)
    if (go ~= nil) then
        if self ~= nil then
            UIEventListener.Get(go).onPress = function(...)
                callback(self, ...)
            end
        else
            UIEventListener.Get(go).onPress = callback
        end
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description: 添加Tween动画结束回调
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addTweenFinishedCallback(parentTrans, controlName, callback)
    local tween = subComponentGet(parentTrans, controlName, "UITweener")

    if (trans ~= nil) then
        tween:AddOnFinished(EventDelegate.Callback(this.OnComboTweenFinish))
    else
        fatal("can not find the control, its name is: " .. controlName)
    end
end

--[[--
 * @Description:         添加UIInput输入框输入变化回调
 * @param:obj            obj,可以是Transform,GameObject,UIInput类型
 * @param:callback       回调函数,回调一个UIInput对象
 ]]
function addInputChangeCallBack(obj,callback)
    local input = obj.gameObject:GetComponent(typeof(UIInput))
    if input.onChange then
        EventDelegate.Add(input.onChange, EventDelegate.Callback(function()
            callback(input)
        end))
    else
        fatal("This GameObject can not find the UIInput, GameObject name is: " .. obj.name)
    end
end

--[[--
 * @Description:         添加PopupList输入框输入变化回调
 * @param:obj            obj,可以是Transform,GameObject,PopupList类型
 * @param:callback       回调函数,回调一个PopupList对象
 ]]
function addPopupListChangeCallBack(obj,callback)
    local popupList = obj.gameObject:GetComponent(typeof(UIPopupList))
    if popupList.onChange then
        EventDelegate.Add(popupList.onChange, EventDelegate.Callback(function()
            callback(popupList)
        end))
    else
        fatal("This GameObject can not find the UIPopupList, GameObject name is: " .. obj.name)
    end
end

--[[--
 * @Description: 添加uitoggle结束回调
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function initToggleObj(parentTrans, controlName, callback, self)
    local toggleObj = nil
    if controlName ~= nil then
        toggleObj = subComponentGet(parentTrans, controlName, "UIToggle")
    else
        toggleObj = componentGet(parentTrans, "UIToggle")
    end

    if (toggleObj ~= nil) then
        if self ~= nil then
            EventDelegate.Add(
                toggleObj.onChange,
                EventDelegate.Callback(
                    function()
                        callback(self)
                    end
                )
            )
        else
            EventDelegate.Add(toggleObj.onChange, EventDelegate.Callback(callback))
        end
    end

    return toggleObj
end

--分割string
function splitString(s, p)
    local rt = {}
    string.gsub(
        s,
        "[^" .. p .. "]+",
        function(w)
            table.insert(rt, w)
        end
    )
    return rt
end

--[[--
 * @Description: 将Uint64转换成字符串
 * @param:       myUint64 server下发的uint64结构体
 * @return:      字符串
 ]]
function toUint64String(myUint64)
    if myUint64 == nil then
        return ""
    end
    local highStr = tostring(myUint64.High)
    local lowStr = tostring(myUint64.Low)
    return highStr .. "-" .. lowStr
end

function stringToMyint64(str)
    local list = splitString(str, "-")
    return toMyUint64(tonumber(list[1]), tonumber(list[2]))
end

--[[--
 * @Description: 将两个uint32转换成protobuf的MyUint64
 ]]
function toMyUint64(high, low)
    local myUint64 = MyUint64()
    myUint64.High = high
    myUint64.Low = low
    return myUint64
end

--[[--
 * @Description: MyUint64 赋值
 ]]
function setMyUint64(myUint64, high, low)
    myUint64.High = high
    myUint64.Low = low
end

function CopyMyUint64(myUint64_out, myUint64_in)
    myUint64_out.High = myUint64_in.High
    myUint64_out.Low = myUint64_in.Low
end

--[[--
 * @Description: 判断两个MyUint64是否相等
 ]]
function MyUint64Equals(myUint64_1, myUint64_2)
    if (myUint64_1 == nil or myUint64_2 == nil) then
        return false
    end

    return myUint64_1.High == myUint64_2.High and myUint64_1.Low == myUint64_2.Low
end

--[[--
 * @Description: 将protobuf的MyUint64转换成Uint64Helper
 * @param:       myUint64 protobuf的MyUint64
 * @return:      Uint64Helper
 ]]
function myUint64ToLuaInt64(myUint64)
    if myUint64 == nil then
        myUint64 = {Low = 0, High = 0}
    end
    return int64.new(myUint64.Low, myUint64.High)
end

--function MyUint64ToString(my64)
--    return tostring(myUint64ToLuaInt64(my64))
--end

function destroyAllChild(trans)
    local childNum = trans.childCount
    for k = 0, childNum - 1 do
        local childTrans = trans:GetChild(k)
        if (childTrans ~= nil) then
            destroy(childTrans.gameObject)
        end
    end
end

--[[--
 * @Description: 一系列帮助函数
 ]]
function Vector3ToTriple(vec3, trip)
    trip.x = math.ceil(vec3.x * 100)
    trip.y = math.ceil(vec3.y * 100)
    trip.z = math.ceil(vec3.z * 100)
end

function TripleToVector3(trip)
    local vec3 = Vector3.zero
    if trip ~= nil then
        vec3.x = (trip.x) / 100
        vec3.y = (trip.y) / 100
        vec3.z = (trip.z) / 100
    end
    return vec3
end

function CopyTriple(outDat, inData)
    outDat.x = inData.x
    outDat.y = inData.y
    outDat.z = inData.z
end

function PosEquals(v1, v2, precision)
    if precision == nil then
        precision = 0.01
    end

    if math.abs(v1.x - v2.x) < precision and math.abs(v1.z - v2.z) < precision then
        return true
    else
        return false
    end
end

function DirEquals(v1, v2)
    if math.abs(v1.x - v2.x) < 0.01 and math.abs(v1.z - v2.z) < 0.01 then
        return true
    else
        return false
    end
end

function DirCalc(v1, v2)
    local Dir = v1 - v2
    Dir.y = 0
    Dir:SetNormalize()
    return Dir
end

function SetLableName(parentTrans, name, text)
    local label = subComponentGet(parentTrans, name, "UILabel")
    if label ~= nil then
        label.text = text
    end
end

function TimeSecToString(sec)
    if (sec ~= nil) then
        local intTime = math.floor(sec)
        return string.format("%02d:%02d", math.floor(intTime / 60), math.floor(intTime % 60))
    else
        return ""
    end
end

--[[--
 * @Description: 将毫秒转化为 天时分秒的格式  不足1秒大于0 默认返回1秒  大于1秒后面尾数舍去
 * @param:       msec (毫秒)
 * @return:      day(天) hour(时) minute(分) second(秒)
 ]]
function TimeMillisecondToParams(msec)
    local timesprit = {1000, 1000 * 60, 1000 * 60 * 60, 1000 * 60 * 60 * 24}
    local time_array = {0, 0, 0, 0}
    if msec ~= nil and msec > 0 then
        local len = table.getn(timesprit)

        local isLessthenSec = true
        local timeMod = msec
        for i = len, 1, -1 do
            local intsTime = timeMod / timesprit[i]
            time_array[i] = math.floor(intsTime)
            if time_array[i] ~= 0 and isLessthenSec == true then
                isLessthenSec = false
            end
            timeMod = timeMod % timesprit[i]
            if timeMod == 0 then
                break
            end
        end

        if timeMod ~= 0 and isLessthenSec == true then
            time_array[TIME_NAME.SECOND] = time_array[TIME_NAME.SECOND] + 1
        end
    end
    --print(time_array[TIME_NAME.DAY].."天 "..time_array[TIME_NAME.HOUR].."h "..time_array[TIME_NAME.MINUTE].."m "..time_array[TIME_NAME.SECOND].."s")
    return time_array[TIME_NAME.DAY], time_array[TIME_NAME.HOUR], time_array[TIME_NAME.MINUTE], time_array[
        TIME_NAME.SECOND
    ]
end

function Vector3.DistanceXZ(va, vb)
    return math.sqrt((va.x - vb.x) ^ 2 + (va.z - vb.z) ^ 2)
end

function GetCurrSceneType()
    return game_scene.getCurSceneType()
end

--[[--
 * @Description: 递归设置UI角色层
 ]]
function RecursiveSetLayerVal(node, layer)
    if (node == nil) then
        return
    end

    node.gameObject.layer = layer
    for i = 1, node.childCount do
        local child = node:GetChild(i - 1)
        if (child ~= nil) then
            RecursiveSetLayerVal(child, layer)
        end
    end
end

--[[--
 * @Description: Restart ParticleSystem In Children
 ]]
function RestartParticleSystem(go)
    if (go == nil) then
        return
    end
    local childrenParticleSystems = go:GetComponentsInChildren(typeof(UnityEngine.ParticleSystem))
    local len = childrenParticleSystems.Length - 1
    if len >= 0 then
        for i = 0, len do
            childrenParticleSystems[i]:Simulate(0, true, true)
            childrenParticleSystems[i]:Play(true)
        end
    end
end

function GetCurrSceneID()
    return game_scene.GetCurSceneID
end

function SetHeroCameraFollow(gameObject)
    if (gameObject ~= nil) then
        local cameraObj = GameObject.FindGameObjectWithTag("MainCamera")
        if (cameraObj ~= nil) then
            local tpCamera = cameraObj:GetComponent("ThirdPersonCameraHall")
            if (tpCamera ~= nil) then
                tpCamera:SetFollowObject(gameObject)
            end
        end
    end
end

local trieFilter = nil

--[[--
 * @Description: 替换掉str中的脏字和敏感词，变温*号，返回替换后的字符串；如果没有脏字
                 和敏感词，返回原字符串
 ]]
function CheckAndReplaceForBadWords(str)
    if (trieFilter == nil) then
        trieFilter = TrieFilter.GetInstance()
    end
    return trieFilter:Replace(str)
end

function AddNavMeshComponent(go)
    local navMeshAgent = go:GetComponent(typeof(UnityEngine.NavMeshAgent))
    if (navMeshAgent == nil) then
        go:AddComponent(typeof(UnityEngine.NavMeshAgent))
        navMeshAgent.enabled = false
    end
end

--[[--
 * @Description: 根据locID得到复活位置和朝向
 ]]
function GetReiveTransformByLocID(locID)
    local retPos, retRot = nil
    local transformArray = nil
    if (locID ~= nil) then
        local levelConfig = map_controller.GetCurMapConfig()
        if (locID == 1) then
            transformArray = levelConfig.bornPoint_1cm
        elseif (locID == 2) then
            transformArray = levelConfig.bornPoint_2cm
        elseif (locID == 3) then
            transformArray = levelConfig.bornPoint_3cm
        end
    end

    if (transformArray ~= nil) then
        retPos = Vector3.New(transformArray[1] / 100, transformArray[2] / 100, transformArray[3] / 100)
        retRot = Vector3.New(transformArray[4] / 100, transformArray[5] / 100, transformArray[6] / 100)
    end

    return retPos, retRot
end

--[[--
 * @Description: 根据BelongID得到Group索引
 ]]
function GetGroupIDFromBelongIdx(belongIdx)
    local ret = 0
    if (belongIdx >= 100) then
        ret = math.floor(belongIdx / 100)
    end

    return ret
end

--[[--
 * @Description: 根据BelongID得到Batch索引
 ]]
function GetBatchIDFromBelongIdx(belongIdx)
    local ret = belongIdx
    if (belongIdx >= 100) then
        ret = belongIdx % 100
    end

    return ret
end

--[[--
 * @Description: 给UILabel设置文字，超出fixedLength的部分，用...表示
 	注意，label应该要被设置成resizeFreely的overFlow方式，否则没有用
 ]]
function SetLabelTextByShort(label, text, fixedLength)
    label.text = text
    label:UpdateNGUIText()

    local stringLen = NGUIText.CalculatePrintedSize(text).x
    if (stringLen > fixedLength) then
        local chars = Utils.splitWord(text)
        local okFlag = false
        local currEndPos = chars.size - 1
        while (not okFlag) do
            currEndPos = currEndPos - 1
            local currText = ""
            for j = 1, currEndPos do
                local charTmp = chars:at(j)
                currText = currText .. charTmp
            end
            currText = currText .. "..."
            label.text = currText
            label:UpdateNGUIText()
            if (NGUIText.CalculatePrintedSize(currText).x < fixedLength) then
                okFlag = true
            end
        end
    end
end

function LoadUI()
    -- body
end

--根据玩家金币是否充足 设置UILabel 颜色
function SetUILabelColorByMoney(UILabel)
    local _money = lua_data_center.GetPlayerMoney()
    if _money < tonumber(UILabel.text) then
        UILabel.color = Color(255 / 255, 0 / 255, 0 / 255)
    else
        UILabel.color = Color(255 / 255, 255 / 255, 255 / 255)
    end
end

--根据玩家钻石是否充足 设置UILabel 颜色
function SetUILabelColorByDiamond(UILabel)
    local _money = lua_data_center.GetPlayerHardCurrency()
    if _money < tonumber(UILabel.text) then
        UILabel.color = Color(255 / 255, 0 / 255, 0 / 255)
    else
        UILabel.color = Color(255 / 255, 255 / 255, 255 / 255)
    end
end

-- table中是否包含该key
function ContainsKey(tableValue, key)
    if (tableValue == nil) then
        print("tabel is nil")
        return false
    end
    if (key == nil) then
        print("key is nil")
        return false
    end
    for k, v in pairs(tableValue) do
        if (k == key) then
            return true
        end
    end
    return false
end

-- 时间显示
function SetScendToTime(attackTime)
    local minute = math.fmod(math.floor(attackTime/60), 60)
    if minute < 10 then 
        minute = "0"..tostring(minute)
    else
        minute = tostring(minute)
    end
    local second = math.fmod(attackTime, 60)
    if second < 10 then 
        second = "0"..tostring(second)
    else
        second = tostring(second)
    end    
    local rtTime = minute..':'..second
    return rtTime
end

--时间显示接口，只显示天
function GetSecondToTimeStr1(Time)
    if Time <= 0 then
        return "已过期"
    end
    local szText = ""
    local unit = 0
    local Day = math.floor(Time / (3600 * 24))
    if Day > 0 then
        unit = unit + 1
        if Day < 10 then
            szText = szText .. "0" .. Day .. "天"
        else
            szText = szText .. Day .. "天"
        end

        if unit >= 1 then
            return szText
        end
        Time = Time % (3600 * 24)
    else
        return "不足一天"    
    end
  
    return szText
end



--[[--
 * @Description: 把秒换算成以天，小时、分钟、秒显示的文字(只输出两个最大的单位) 输出格式(例如00天00时，00时00分)
 * @param:       秒
 * @return:      string
 ]]
 function GetSecondToTimeStr2(Time)
    if Time <= 0 then
        return "0" .. "秒"
    end
    local szText = ""
    local unit = 0
    local Day = math.floor(Time / (3600 * 24))
    if Day > 0 then
        unit = unit + 1
        if Day < 10 then
            szText = szText .. "0" .. Day .. "天"
        else
            szText = szText .. Day .. "天"
        end

        if unit >= 2 then
            return szText
        end
        Time = Time % (3600 * 24)
    end
    local Hour = math.floor(Time / 3600)
    if Hour > 0 then
        unit = unit + 1
        if Hour < 10 then
            szText = szText .. "0" .. Hour .. "时"
        else
            szText = szText .. Hour .. "时"
        end
        if unit >= 2 then
            return szText
        end
        Time = Time % 3600
    end

    local Min = math.floor(Time / 60)
    if Min > 0 then
        unit = unit + 1
        if Min < 10 then
            szText = szText .. "0" .. Min .. "分"
        else
            szText = szText .. Min .. "分"
        end
        if unit >= 2 then
            return szText
        end
        Time = Time % 60
    end
    if Time < 10 then
        szText = szText .. "0" .. Time .. "秒"
    else
        szText = szText .. Time .. "秒"
    end
    return szText
end


--[[--
 * @Description: 把秒换算成以天，小时、分钟、秒显示的文字
 * @param:       秒
 * @return:      string
 ]]
 function GetSecondToTimeStr3(Time)
    if Time <= 0 then
        return "0" .. "秒"
    end
    local szText = ""
    local unit = 0
    local Day = math.floor(Time / (3600 * 24))
    if Day > 0 then
      
        if Day < 10 then
            szText = szText .. "0" .. Day .. "天"
        else
            szText = szText .. Day .. "天"
        end

        Time = Time % (3600 * 24)
    end
    local Hour = math.floor(Time / 3600)
    if Hour > 0 then
   
        if Hour < 10 then
            szText = szText .. "0" .. Hour .. "时"
        else
            szText = szText .. Hour .. "时"
        end
    
        Time = Time % 3600
    end

    local Min = math.floor(Time / 60)
    if Min > 0 then
     
        if Min < 10 then
            szText = szText .. "0" .. Min .. "分"
        else
            szText = szText .. Min .. "分"
        end
        Time = Time % 60
    end
    if Time < 10 then
        szText = szText .. "0" .. Time .. "秒"
    else
        szText = szText .. Time .. "秒"
    end
    return szText
end


--截取中英混合的UTF8字符串，endIndex可缺省
function SubStringUTF8(str, startIndex, endIndex)
    if startIndex < 0 then
        startIndex = SubStringGetTotalIndex(str) + startIndex + 1
    end

    if endIndex ~= nil and endIndex < 0 then
        endIndex = SubStringGetTotalIndex(str) + endIndex + 1
    end

    if endIndex == nil then
        return string.sub(str, SubStringGetTrueIndex(str, startIndex))
    else
        return string.sub(str, SubStringGetTrueIndex(str, startIndex), SubStringGetTrueIndex(str, endIndex + 1) - 1)
    end
end

--获取中英混合UTF8字符串的真实字符数量
function SubStringGetTotalIndex(str)
    local curIndex = 0
    local i = 1
    local lastCount = 1
    repeat
        lastCount = SubStringGetByteCount(str, i)
        i = i + lastCount
        curIndex = curIndex + 1
    until (lastCount == 0)
    return curIndex - 1
end

function SubStringGetTrueIndex(str, index)
    local curIndex = 0
    local i = 1
    local lastCount = 1
    repeat
        lastCount = SubStringGetByteCount(str, i)
        i = i + lastCount
        curIndex = curIndex + 1
    until (curIndex >= index)
    return i - lastCount
end

--返回当前字符实际占用的字符数
function SubStringGetByteCount(str, index)
    local curByte = string.byte(str, index)
    local byteCount = 1
    if curByte == nil then
        byteCount = 0
    elseif curByte > 0 and curByte <= 127 then
        byteCount = 1
    elseif curByte >= 192 and curByte <= 223 then
        byteCount = 2
    elseif curByte >= 224 and curByte <= 239 then
        byteCount = 3
    elseif curByte >= 240 and curByte <= 247 then
        byteCount = 4
    end
    return byteCount
end
---------------------

----[[
-- 把表转换成类似这种格式的字符串{a='b';};，字符串用可见文本形式表示，可限制打印层数，nest_Max默认8层
--便于完整打印输出表，方便查看数据
--提交之前都要删除，供本地使用
--使用的时候将LuaTabletostring(),string_toLuaString(str),lsm_vardump(object, label, nest_Max)3个方法注释解除
--]]
----[[
-- 把表转换成类似这种格式的字符串{a='b';};，字符串用可见文本形式表示，可限制打印层数，nest_Max默认8层
--便于完整打印输出表，方便查看数据
--提交之前都要删除，供本地使用
--使用的时候将LuaTabletostring(),string.toLuaString(str),lsm_vardump(object, label, nest_Max)3个方法注释解除
--]]
function LuaTabletostring(t, nest_Max)
    local oldFunc = string.toLuaString
    string.toLuaString = tostring
    local tableString = lsm_vardump(t, "", nest_Max)
    string.toLuaString = oldFunc
    return tableString:sub(8)
end

-- 扩展string方法，转换到lua源码格式的字符串
function string.toLuaString(str)
    local buffer = {'"'}
    for i = 1, string.len(str) do
        table.insert(buffer, [[\]] .. string.byte(str, i))
    end
    table.insert(buffer, '"')
    return table.concat(buffer)
end

function lsm_vardump(object, label, nest_Max)
    local lookupTable = {}
    local result = {}
    local nest_Max = nest_Max or 8
    local function _v(v)
        if type(v) == "string" then
            v = v:toLuaString()
        end
        return tostring(v)
    end

    local function _key(k)
        if type(k) == "string" then
            return '["' .. k .. '"]'
        elseif type(k) == "table" then
            return '["' .. tostring(k.name or k.full_name or "table") .. '"]'
        elseif type(k) == "userdata" then
            return "[" .. "userdata" .. "]"
        else
            return "[" .. k .. "]"
        end
    end

    local function _vardump(object, label, indent, nest)
        label = label or "<var>"
        local s_Key = _key(label)
        if SubStringUTF8(s_Key, 3, 3) ~= "_" or s_Key == '["_fields"]' then
            local postfix = ""
            if nest > 1 then
                postfix = ","
            end
            if type(object) ~= "table" then
                result[#result + 1] = string.format("%s%s = %s%s", indent, s_Key, _v(object), postfix)
            elseif not lookupTable[object] then
                lookupTable[object] = true
                if s_Key ~= '["_fields"]' then
                    result[#result + 1] = string.format("%s%s = {", indent, s_Key)
                end
                local indent2 = indent .. "    "
                local keys = {}
                local values = {}
                for k, v in pairs(object) do
                    keys[#keys + 1] = k
                    values[k] = v
                end
                table.sort(
                    keys,
                    function(a, b)
                        if type(a) == "number" and type(b) == "number" then
                            return a < b
                        else
                            return tostring(a) < tostring(b)
                        end
                    end
                )
                if nest <= nest_Max then
                    for i, k in ipairs(keys) do
                        if s_Key ~= '["_fields"]' then
                            _vardump(values[k], k, indent2, nest + 1)
                        else
                            _vardump(values[k], k, indent2, nest)
                        end
                    end
                else
                    result[#result + 1] = "truncated......"
                end
                if s_Key ~= '["_fields"]' then
                    result[#result + 1] = string.format("%s}%s", indent, postfix)
                end
            end
        end
    end
    _vardump(object, label, "", 1)

    return table.concat(result, "\n")
end

--判断是否是自己
function IsSelf(id)
    if id.Low == lua_data_center.PlayerID.Low and id.High == lua_data_center.PlayerID.High then
        return true
    else
        return false
    end
end

--比较两个MyUInt64
function CompareMyUInt64(param1, param2)
    if param1.Low == param2.Low and param1.High == param2.High then
        return true
    else
        return false
    end
end

---@清理前后的空格
function CleanUpText(info)
    if info == nil or info == "" then
       return ""
    end   
    return string.match(info,"%s*(.-)%s*$")   
end

--时间戳转换
function timestamp_convert(time)
    local timetable={}
    timetable.day = math.floor(time/86400)
    timetable.hour = math.floor(time%86400/3600)
    timetable.min = math.floor(time%3600/60)
    timetable.sec = math.floor(time%60)

    if timetable.hour<=9 then
        timetable.hour="0"..timetable.hour
    end
    if timetable.min<=9 then
        timetable.min="0"..timetable.min
    end
    if timetable.sec<=9 then
        timetable.sec="0"..timetable.sec
    end
    
    return timetable
end

---@本地时间 秒单位
function GetLocalTime()
    return tonumber(System.DateTime.Now.Ticks * 0000000.1)
end

function GetWeek(number)
    if number >= 7 then
        number = number % 7
    end
    if number == 1 then
        return "星期一"        
    end
    if number == 2 then
        return "星期二"        
    end
    if number == 3 then
        return "星期三"        
    end
    if number == 4 then
        return "星期四"        
    end
    if number == 5 then
        return "星期五"        
    end
    if number == 6 then
        return "星期六"        
    end
    if number == 7 then
        return "星期日"        
    end
end

--==============================--
--desc:优化创建gird/table下的Item,用于数据不多（数据多用虚拟列表），但有优化的余地
--@grid:NGUI的UIGrid/UITable，必传数据
--@count:要创建的数量，一般为数据的长度
--@call: 完成后的回调,call(gameObject,index)
--使用方式：在layout节点下放置一个模版对象
--==============================--
function OptimizeLoadItemForLayout(layout, count, call, frame, endCall)
    if IsNil(layout) then
        return ;
    end
    if layout.transform.childCount <= 0 then
        return ;
    end
    local itemPrefab = layout.transform:GetChild(0);
    if IsNil(itemPrefab) then
        return ;
    end

    local function createItem()
        while (not IsNil(layout) and layout.transform.childCount < count) do
            coroutine.step(frame);
            if not IsNil(layout) and layout.transform.childCount < count then
                local newObj = NGUITools.AddChild(layout.gameObject, itemPrefab.gameObject);
                newObj.name = layout.transform.childCount - 1;
                newObj:SetActive(true);
                if call then
                    call(newObj, layout.transform.childCount - 1);
                end
                layout.repositionNow = true;
                if layout.transform.childCount == count then
                    if endCall then
                        endCall();
                    end
                end
            end
        end
    end

    for i = 0, layout.transform.childCount - 1 do
        local child = layout.transform:GetChild(i);
        child.gameObject.name = i;
        child.gameObject:SetActive(i < count);
        if i < count and call then
            call(child.gameObject, i);
        end
    end

    if layout.transform.childCount < count then
        if frame and frame > 0 then
            coroutine.start(createItem);
        else
            while (layout.transform.childCount < count) do
                local newObj = NGUITools.AddChild(layout.gameObject, itemPrefab.gameObject);
                newObj.name = layout.transform.childCount - 1;
                newObj:SetActive(true);
                if call then
                    call(newObj, layout.transform.childCount - 1);
                end
            end
            layout.repositionNow = true;
            if endCall then
                endCall();
            end
        end
    else
        layout.repositionNow = true;
        if endCall then
            endCall();
        end
    end
end

----[[
--	打印堆栈 add by zdj
--	提交之前都要删除，供本地使用
--]]
function print_stack(...)
    local out = { '[TraceBack]' }
    local n = select('#', ...)
    for i = 1, n, 1 do
        local v = select(i, ...)
        out[#out + 1] = tostring(v)
    end
    out[#out + 1] = '\n'
    out[#out + 1] = debug.traceback("", 2)
    print(table.concat(out, ' '))
end


--验证字符串是否符合规则
function  CheckLoginString(str)    
    --账号密码只能为数字加字母
    if string.find(str, "%W") ~= nil then
        UISys.Instance:ShowMessageTips(Utils.GetText('账号密码只能使用字母或数字组合！'))
        return false
    end
    
    if string.len(str)<1  then
        UISys.Instance:ShowMessageTips(Utils.GetText('账号或密码为空！'))
        return false
    end

	if string.len(str)<6 then
        UISys.Instance:ShowMessageTips(Utils.GetText('账号或密码长度过短！'))
        return false
    end

	if  string.len(str)>20 then    
		UISys.Instance:ShowMessageTips(Utils.GetText('账号或密码长度过长！'))
        return false
    end
     
    return true
end