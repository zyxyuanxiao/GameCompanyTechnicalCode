local UIBaseCtrl = require "UIController/UIBaseCtrl"
local UILoginCtrl = class("UILoginCtrl", UIBaseCtrl)

local loginCtrl = nil
local panelMgr = nil

function UILoginCtrl:Awake()
	panelMgr = MgrCenter:GetManager(ManagerNames.Panel)
	panelMgr:CreatePanel(self, UILayer.Common, UiNames.Login, self.OnCreateOK)
	logWarn("UILoginCtrl.Awake--->>")
end

--启动事件--
function UILoginCtrl:OnCreateOK(behaviour)
	self.gameObject = behaviour.gameObject
	self:InitBase()
	local adapterMgr = MgrCenter:GetManager(ManagerNames.Adapter)
	loginCtrl = adapterMgr:GetAdapter(LevelType.Login)

	behaviour:AddClick(self.btn_Start, self, self.OnStartClick)
	behaviour:AddClick(self.btn_Create, self, self.OnCreateClick)

	local rect = self.gameObject:GetComponent('RectTransform')
	if rect ~= nil then
		rect.anchorMin = Vector2.zero
		rect.anchorMax = Vector2.one
		rect.sizeDelta = Vector2.zero
		rect.anchoredPosition3D = Vector3.zero
	end
	self.txt_version.text = LuaHelper.GetVersionInfo()

	PlayerPrefs.DeleteKey("roleid")
	self:CheckExistCharacter()
	logWarn("OnCreateOK--->>"..self.gameObject.name)
end

function UILoginCtrl:CheckExistCharacter()
	local roleid = PlayerPrefs.GetInt("roleid", -1)
	local isExistRole = roleid > -1
	if isExistRole then
		local roleSex = PlayerPrefs.GetInt("rolesex", -1)
		local moduleMgr = MgrCenter:GetManager(ManagerNames.Module)
		local mainRoleModule = moduleMgr:GetModule(ModuleNames.MainRole)
		mainRoleModule:AssignMainRoleData(roleid, roleSex)
	end

	local matBtnCreate = nil
	if isExistRole then
		matBtnCreate = GrayMat
	end
	self.btn_Create.image.material = matBtnCreate
	self.btn_Create.interactable = not isExistRole

	local matBtnStart = nil
	if not isExistRole then
		matBtnStart = GrayMat
	end
	self.btn_Start.interactable = isExistRole
	self.btn_Start.image.material = matBtnStart
end

--创建角色--
function UILoginCtrl:OnCreateClick(go)
	Main.ShowUI(UiNames.ChooseActor)
	self.gameObject:SetActive(false)
end

function UILoginCtrl:OnShow()
	self:CheckExistCharacter()
	self.gameObject:SetActive(true)
end

--进入游戏--
function UILoginCtrl:OnStartClick(go)
	loginCtrl:StartLogin()
end

--关闭事件--
function UILoginCtrl:Close()
	self:Dispose()
	panelMgr:ClosePanel(UiNames.Login)
end

return UILoginCtrl