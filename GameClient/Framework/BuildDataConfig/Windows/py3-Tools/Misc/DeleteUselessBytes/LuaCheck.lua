--[[
    从config_data_center中赋值过来下面两个表
]]

local dp = "dataconfig_"

local configMapTable =
{
	["dataconfig_dailygiftbag"] = "dataconfig_dailygiftbag_ios", -- 136 礼包
	["dataconfig_weekcards"] = "dataconfig_dailygiftbag_ios", -- 周卡月卡
	["dataconfig_activitybigprofitreward"] = "dataconfig_activitybigprofitreward_ios", -- 一本万利
	["dataconfig_holidaygift"] = "dataconfig_holidaygift_ios", -- 计费点节点礼包
	["dataconfig_activitytimelimitpackage"] = "dataconfig_activitytimelimitpackage_ios", -- 限时礼包
	["dataconfig_bubble"] = "dataconfig_bubble_ios", -- 上新特惠
	["dataconfig_activetag"] = "dataconfig_activetag_ios", -- 活动页签
	["dataconfig_operateactivity"] = "dataconfig_operateactivity_ios", -- 活动时间
	["dataconfig_dicinfoconfig"] = "dataconfig_dicinfoconfig_ios", -- 字典表
	["dataconfig_lventryopen"] = "dataconfig_lventryopen_ios", -- 入口等级开放表
	["dataconfig_taskinfo"] = "dataconfig_taskinfo_ios", -- 任务信息
	["dataconfig_tasknpc"] = "dataconfig_tasknpc_ios", -- 任务信息
	["dataconfig_levelconfig"] = "dataconfig_levelconfig_ios", -- 关卡信息
	["dataconfig_dialogueconfig"] = "dataconfig_dialogueconfig_ios", -- 情景对话
	["dataconfig_worldbigmap"] = "dataconfig_worldbigmap_ios", -- 世界地图传送
	["dataconfig_goldshopgoodsconfig"] = "dataconfig_goldshopgoodsconfig_ios", -- 元宝商城
}


local configInitFuncDict = {
	[dp.."buffconfig"] = function () require(pre.."buffconfig_pb") pdt[dp.."buffconfig"] = buffconfig_x end,
	[dp.."generalconfig"] = function () require(pre.."generalconfig_pb") pdt[dp.."generalconfig"] = generalconfig_x end,
	[dp.."globalconfig"] = function () require(pre.."globalconfig_pb") pdt[dp.."globalconfig"] = globalconfig_x end,
	[dp.."goodconfig"] = function () require(pre.."goodconfig_pb") pdt[dp.."goodconfig"] = goodconfig_x  end,
	[dp.."heroproperty"] = function () require(pre.."heroproperty_pb") pdt[dp.."heroproperty"] = heroproperty_x end,
	[dp.."levelclearancecondition"] = function () require(pre.."levelclearancecondition_pb") pdt[dp.."levelclearancecondition"] = levelclearancecondition_x end,
	[dp.."levelconfig"] = function () require(pre.."levelconfig_pb") pdt[dp.."levelconfig"] = levelconfig_x end,
	[dp.."levelconfig_ios"] = function () require(pre.."levelconfig_ios_pb") pdt[dp.."levelconfig_ios"] = levelconfig_ios_x end,
	[dp.."levelgroup"] = function () require(pre.."levelgroup_pb") pdt[dp.."levelgroup"] = levelgroup_x  end,
	[dp.."maincity"] = function () require(pre.."maincity_pb") pdt[dp.."maincity"] = maincity_x end,
	[dp.."monsterproperty"] = function ()  require(pre.."monsterproperty_pb") pdt[dp.."monsterproperty"] = monsterproperty_x end,
	[dp.."resconfig"] = function () require(pre.."resconfig_pb") pdt[dp.."resconfig"] = resconfig_x end,
	[dp.."skillconfig"] = function () require(pre.."skillconfig_pb") pdt[dp.."skillconfig"] = skillconfig_x end,
	[dp.."skilllearn"] = function () require(pre.."skilllearn_pb") pdt[dp.."skilllearn"] = skilllearn_x end,
	[dp.."skillperfconfig"] = function ()  require(pre.."skillperfconfig_pb") pdt[dp.."skillperfconfig"] = skillperfconfig_x end,
	[dp.."skilltips"] = function () require(pre.."skilltips_pb") pdt[dp.."skilltips"] = skilltips_x end,	--jn_技能学习配置
	[dp.."systemequip"] = function () require(pre.."systemequip_pb") pdt[dp.."systemequip"] = systemequip_x end,
	[dp.."worldattr"] = function () require(pre.."worldattr_pb") pdt[dp.."worldattr"] = worldattr_x end,
	[dp.."worldmap"] = function () require(pre.."worldmap_pb") pdt[dp.."worldmap"] = worldmap_x end,
	[dp.."expupgrade"] = function () require(pre.."expupgrade_pb") pdt[dp.."expupgrade"] = expupgrade_x end,
	[dp.."expupgrade_ios"] = function () require(pre.."expupgrade_ios_pb") pdt[dp.."expupgrade_ios"] = expupgrade_ios_x end,
	[dp.."dicinfoconfig"] = function () require(pre.."dicinfoconfig_pb") pdt[dp.."dicinfoconfig"] = dicinfoconfig_x end,
	[dp.."dicinfoconfig_ios"] = function () require(pre.."dicinfoconfig_ios_pb") pdt[dp.."dicinfoconfig_ios"] = dicinfoconfig_ios_x end,
	[dp.."forgeconfig"] = function () require(pre.."forgeconfig_pb") pdt[dp.."forgeconfig"] = forgeconfig_x  end,
	[dp.."specialmaterial"] = function () require(pre.."specialmaterial_pb") pdt[dp.."specialmaterial"] = specialmaterial_x end,
	[dp.."levelswitchresource"] = function () require(pre.."levelswitchresource_pb") pdt[dp.."levelswitchresource"] = levelswitchresource_x end,
	[dp.."idset"] = function () require(pre.."idset_pb") pdt[dp.."idset"] = idset_x end,
	[dp.."taskinfo"] = function () require(pre.."taskinfo_pb") pdt[dp.."taskinfo"] = taskinfo_x end,
	[dp.."taskinfo_ios"] = function () require(pre.."taskinfo_ios_pb") pdt[dp.."taskinfo_ios"] = taskinfo_ios_x end,
	[dp.."ghosttask"] = function () require(pre.."ghosttask_pb") pdt[dp.."ghosttask"] = ghosttask_x end,--镜像配置表
	[dp.."dialogueconfig"] = function () require(pre.."dialogueconfig_pb") pdt[dp.."dialogueconfig"] = dialogueconfig_x  end,--情景对话表
	[dp.."dialoguecamera"] = function () require(pre.."dialoguecamera_pb") pdt[dp.."dialoguecamera"] = dialoguecamera_x  end,--情景镜头表
	[dp.."dialoguechildnpc"] = function () require(pre.."dialoguechildnpc_pb") pdt[dp.."dialoguechildnpc"] = dialoguechildnpc_x  end,--情景召唤npc位置表
	[dp.."dialogueconfig_ios"] = function () require(pre.."dialogueconfig_ios_pb") pdt[dp.."dialogueconfig_ios"] = dialogueconfig_ios_x  end,
	[dp.."sceneprompt"] = function () require(pre.."sceneprompt_pb") pdt[dp.."sceneprompt"] = sceneprompt_x  end,
	[dp.."mall"] = function () require(pre.."mall_pb") pdt[dp.."mall"] = mall_x end,
	[dp.."shopconfig"] = function () require(pre.."shopconfig_pb") pdt[dp.."shopconfig"] = shopconfig_x end,
	[dp.."shopgoodsconfig"] = function () require(pre.."shopgoodsconfig_pb") pdt[dp.."shopgoodsconfig"] = shopgoodsconfig_x end,
	[dp.."chatconf"] = function () require(pre.."chatconf_pb") pdt[dp.."chatconf"] = chatconf_x end,
	[dp.."tab"] = function () require(pre.."tab_pb") pdt[dp.."tab"] = tab_x end,
	[dp.."cameraconfig"] = function () require(pre.."cameraconfig_pb") pdt[dp.."cameraconfig"] = cameraconfig_x end,
	[dp.."explevelconfig"] = function () require(pre.."explevelconfig_pb") pdt[dp.."explevelconfig"] = explevelconfig_x end,
	[dp.."professionstate"] = function () require(pre.."professionstate_pb") pdt[dp.."professionstate"] = professionstate_x end,
	[dp.."attrbasevalue"] = function () require(pre.."attrbasevalue_pb") pdt[dp.."attrbasevalue"] = attrbasevalue_x end,
	[dp.."attridratio"] = function () require(pre.."attridratio_pb") pdt[dp.."attridratio"] = attridratio_x end,
	[dp.."fitbasenum"] = function () require(pre.."fitbasenum_pb") pdt[dp.."fitbasenum"] = fitbasenum_x end,

	[dp.."leveldescription"] = function () require(pre.."leveldescription_pb") pdt[dp.."leveldescription"] = leveldescription_x end,
	[dp.."asidesdata"] = function () require(pre.."asidesdata_pb") pdt[dp.."asidesdata"] = asidesdata_x end,
	[dp.."npcinfo"] = function () require(pre.."npcinfo_pb") pdt[dp.."npcinfo"] = npcinfo_x end,
	[dp.."offresource"] = function () require(pre.."offresource_pb") pdt[dp.."offresource"] = offresource_x end,
	[dp.."textparttravel"] = function () require(pre.."textparttravel_pb") pdt[dp.."textparttravel"] = textparttravel_x end,
	[dp.."textgroup"] = function () require(pre.."textgroup_pb") pdt[dp.."textgroup"] = textgroup_x end,
	-- [dp.."secondattrrate"] = function () require(pre.."secondattrrate_pb") pdt[dp.."secondattrrate"] = secondattrrate_x end,
	[dp.."levelentrycondition"] = function () require(pre.."levelentrycondition_pb") pdt[dp.."levelentrycondition"] = levelentrycondition_x end,
	[dp.."drinkquestion"] = function () require(pre.."drinkquestion_pb") pdt[dp.."drinkquestion"] = drinkquestion_x end,
	[dp.."wanted"] = function () require(pre.."wanted_pb") pdt[dp.."wanted"] =wanted_x end,
	[dp.."wantedpos"] = function () require(pre.."wantedpos_pb") pdt[dp.."wantedpos"] =wantedpos_x end,
	[dp.."monsterfightattr"] = function () require(pre.."monsterfightattr_pb") pdt[dp.."monsterfightattr"] = monsterfightattr_x end,
	[dp.."notice"] = function () require(pre.."notice_pb") pdt[dp.."notice"] = notice_x end,
	[dp.."noticeindex"] = function () require(pre.."noticeindex_pb") pdt[dp.."noticeindex"] = noticeindex_x end,
	[dp.."fightingattr"] = function () require(pre.."fightingattr_pb") pdt[dp.."fightingattr"] = fightingattr_x end,
	[dp.."passiveskillconfig"] = function () require(pre.."passiveskillconfig_pb") pdt[dp.."passiveskillconfig"] = passiveskillconfig_x end,
	[dp.."playsonmap"] = function () require(pre.."playsonmap_pb") pdt[dp.."playsonmap"] = playsonmap_x end,
	[dp.."keydescribe"] = function () require(pre.."keydescribe_pb") pdt[dp.."keydescribe"] = keydescribe_x end,
	[dp.."specialattrrange"] = function () require(pre.."specialattrrange_pb") pdt[dp.."specialattrrange"] = specialattrrange_x end,
	[dp.."attrnamedesc"] = function () require(pre.."attrnamedesc_pb") pdt[dp.."attrnamedesc"] = attrnamedesc_x end, ---属性类型文字描述表
	[dp.."extraeffectconf"] = function () require(pre.."extraeffectconf_pb") pdt[dp.."extraeffectconf"] = extraeffectconf_x end,
	[dp.."worldmaparea"] = function () require(pre.."worldmaparea_pb") pdt[dp.."worldmaparea"] = worldmaparea_x end,
	[dp.."systemrule"] = function () require(pre.."systemrule_pb") pdt[dp.."systemrule"] = systemrule_x end,
	[dp.."difficultyconfig"] = function () require(pre.."difficultyconfig_pb") pdt[dp.."difficultyconfig"] = difficultyconfig_x end,
	[dp.."leveltocapacity"] = function () require(pre.."leveltocapacity_pb") pdt[dp.."leveltocapacity"] = leveltocapacity_x end,
	[dp.."guideinfo"] = function () require(pre.."guideinfo_pb") pdt[dp.."guideinfo"] = guideinfo_x end,
	[dp.."equipsuit"] = function () require(pre.."equipsuit_pb") pdt[dp.."equipsuit"] = equipsuit_x end,
	[dp.."skilldesc"] = function () require(pre.."skilldesc_pb") pdt[dp.."skilldesc"] = skilldesc_x end,
	[dp.."expandconfig"] = function () require(pre.."expandconfig_pb") pdt[dp.."expandconfig"] = expandconfig_x end,
	[dp.."activityconfig"] = function () require(pre.."activityconfig_pb") pdt[dp.."activityconfig"] = activityconfig_x end,
	[dp.."activityreward"] = function () require(pre.."activityreward_pb") pdt[dp.."activityreward"] = activityreward_x end,
	[dp.."sourceconfig"] = function () require(pre.."sourceconfig_pb") pdt[dp.."sourceconfig"] = sourceconfig_x end,
	[dp.."levelhelper"] = function () require(pre.."levelhelper_pb") pdt[dp.."levelhelper"] = levelhelper_x end,
	[dp.."revivecost"] = function () require(pre.."revivecost_pb") pdt[dp.."revivecost"] = revivecost_x end,
	[dp.."attrgroup"] = function () require(pre.."attrgroup_pb") pdt[dp.."attrgroup"] = attrgroup_x end,
	[dp.."singlelevelaward"] = function () require(pre.."singlelevelaward_pb") pdt[dp.."singlelevelaward"] = singlelevelaward_x end,
	[dp.."unlocklevel"] = function () require(pre.."unlocklevel_pb") pdt[dp.."unlocklevel"] = unlocklevel_x end,
	[dp.."battleassist"] = function () require(pre.."battleassist_pb") pdt[dp.."battleassist"] = battleassist_x end,
	[dp.."endlesslevelconfig"] = function () require(pre.."endlesslevelconfig_pb") pdt[dp.."endlesslevelconfig"] = endlesslevelconfig_x end,
	[dp.."singlevictoryani"] = function () require(pre.."singlevictoryani_pb") pdt[dp.."singlevictoryani"] = singlevictoryani_x end,
	[dp.."maincitysecretnewsdata"] = function () require(pre.."maincitysecretnewsdata_pb") pdt[dp.."maincitysecretnewsdata"] = maincitysecretnewsdata_x end,
	[dp.."mailtemplate"] = function () require(pre.."mailtemplate_pb") pdt[dp.."mailtemplate"] = mailtemplate_x end,
	[dp.."pvpexp"] = function () require(pre.."pvpexp_pb") pdt[dp.."pvpexp"] = pvpexp_x end,
	[dp.."groupinfo"] = function () require(pre.."groupinfo_pb") pdt[dp.."groupinfo"] = groupinfo_x end,
	[dp.."outwardconf"] = function () require(pre.."outwardconf_pb") pdt[dp.."outwardconf"] = outwardconf_x end, --yc_衣橱系统配置
	[dp.."outwardsuitconf"] = function () require(pre.."outwardsuitconf_pb") pdt[dp.."outwardsuitconf"] = outwardsuitconf_x end,--yc_衣橱系统配置
	[dp.."outwardunlockcondition"] = function () require(pre.."outwardunlockcondition_pb") pdt[dp.."outwardunlockcondition"] = outwardunlockcondition_x end,--yc_衣橱系统配置
	[dp.."outwarddye"] = function () require(pre.."outwarddye_pb") pdt[dp.."outwarddye"] = outwarddye_x end,
	[dp.."outwardunitposition"] = function () require(pre.."outwardunitposition_pb") pdt[dp.."outwardunitposition"] = outwardunitposition_x end,
	[dp.."charmlevel"] = function () require(pre.."charmlevel_pb") pdt[dp.."charmlevel"] = charmlevel_x end,--yc_衣橱系统配置
	[dp.."linedata"] = function () require(pre.."linedata_pb") pdt[dp.."linedata"] = linedata_x end,
	[dp.."robotattr"] = function () require(pre.."robotattr_pb") pdt[dp.."robotattr"] = robotattr_x end,
	[dp.."angerconfig"] = function () require(pre.."angerconfig_pb") pdt[dp.."angerconfig"] = angerconfig_x end,
	[dp.."foodeffect"] = function () require(pre.."foodeffect_pb") pdt[dp.."foodeffect"] = foodeffect_x end,
	[dp.."monsterspeak"] = function () require(pre.."monsterspeak_pb") pdt[dp.."monsterspeak"] = monsterspeak_x end,
	[dp.."chuangong"] = function () require(pre.."chuangong_pb") pdt[dp.."chuangong"] = chuangong_x end,
	[dp.."angeradd"] = function () require(pre.."angeradd_pb") pdt[dp.."angeradd"] = angeradd_x end,
	[dp.."worldbigmap"] = function () require(pre.."worldbigmap_pb") pdt[dp.."worldbigmap"] = worldbigmap_x end,
	[dp.."notifytemplate"] = function () require(pre.."notifytemplate_pb") pdt[dp.."notifytemplate"] = notifytemplate_x end,
	[dp.."levelrevive"] = function () require(pre.."levelrevive_pb") pdt[dp.."levelrevive"] = levelrevive_x end,
	[dp.."soulopen"] = function () require(pre.."soulopen_pb") pdt[dp.."soulopen"] = soulopen_x end,
	[dp.."fightsoulconfig"] = function () require(pre.."fightsoulconfig_pb") pdt[dp.."fightsoulconfig"] = fightsoulconfig_x end,
	[dp.."coresoul"] = function () require(pre.."coresoul_pb") pdt[dp.."coresoul"] = coresoul_x end,
	[dp.."levelratedesc"] = function () require(pre.."levelratedesc_pb") pdt[dp.."levelratedesc"] = levelratedesc_x end,
	[dp.."factionwelcome"] = function () require(pre.."factionwelcome_pb") pdt[dp.."factionwelcome"] = factionwelcome_x end,
	[dp.."factionbuild"] = function () require(pre.."factionbuild_pb") pdt[dp.."factionbuild"] = factionbuild_x end,
	[dp.."factionaward"] = function () require(pre.."factionaward_pb") pdt[dp.."factionaward"] = factionaward_x end,
	[dp.."factiondonation"] = function () require(pre.."factiondonation_pb") pdt[dp.."factiondonation"] = factiondonation_x end,
	[dp.."factionauthority"] = function () require(pre.."factionauthority_pb") pdt[dp.."factionauthority"] = factionauthority_x end, --帮会权限
	[dp.."authoritylist"] = function () require(pre.."authoritylist_pb") pdt[dp.."authoritylist"] = authoritylist_x end, --帮会权限列表
	--联盟的config文件
	[dp.."leagueauthority"] = function () require(pre.."leagueauthority_pb") pdt[dp.."leagueauthority"] = leagueauthority_x end, --lm_联盟系统表 福利
	[dp.."leagueauthoritylist"] = function () require(pre.."leagueauthoritylist_pb") pdt[dp.."leagueauthoritylist"] = leagueauthoritylist_x end, --lm_联盟系统表 福利
	[dp.."leaguewelfare"] = function () require(pre.."leaguewelfare_pb") pdt[dp.."leaguewelfare"] = leaguewelfare_x end,--lm_联盟系统表 福利
	[dp.."leagueadministration"] = function () require(pre.."leagueadministration_pb") pdt[dp.."leagueadministration"] = leagueadministration_x end,--lm_联盟系统表 福利
	[dp.."leagueactivitygroup"] = function () require(pre.."leagueactivitygroup_pb") pdt[dp.."leagueactivitygroup"] = leagueactivitygroup_x end,--lm_联盟系统表 活动页签
	[dp.."leaguetype"] = function () require(pre.."leaguetype_pb") pdt[dp.."leaguetype"] = leaguetype_x end,  --lm_联盟系统表 聚义堂模型相关
	[dp.."leaguebutton"] = function () require(pre.."leaguebutton_pb") pdt[dp.."leaguebutton"] = leaguebutton_x end,  --lm_联盟系统表 管理按钮相关
	[dp.."leaguetrain"] = function () require(pre.."leaguetrain_pb") pdt[dp.."leaguetrain"] = leaguetrain_x end,  --lm_联盟传功表
	[dp.."titlebox"] = function () require(pre.."titlebox_pb") pdt[dp.."titlebox"] = titlebox_x end,  --lm_联盟系统表 称号框
	[dp.."leaguetrain"] = function () require(pre.."leaguetrain_pb") pdt[dp.."leaguetrain"] = leaguetrain_x end,  --lm_联盟传功表
	[dp.."leaguerank"] = function () require(pre.."leaguerank_pb") pdt[dp.."leaguerank"] = leaguerank_x end,  --lm_联盟排行榜

	[dp.."picking"] = function () require(pre.."picking_pb") pdt[dp.."picking"] = picking_x end,
	[dp.."fishing"] = function () require(pre.."fishing_pb") pdt[dp.."fishing"] = fishing_x end,
	[dp.."activitytime"] = function () require(pre.."activitytime_pb") pdt[dp.."activitytime"] = activitytime_x end,--活动时间表
	[dp.."functionpre"] = function () require(pre.."functionpre_pb") pdt[dp.."functionpre"] = functionpre_x end,--活动时间表-功能预告
	[dp.."expression"] = function () require(pre.."expression_pb") pdt[dp.."expression"] = expression_x end,
	[dp.."ghostinfo"] = function () require(pre.."ghostinfo_pb") pdt[dp.."ghostinfo"] = ghostinfo_x end,
	[dp.."jhll_award"] = function () require(pre.."jhll_award_pb") pdt[dp.."jhll_award"] = jhll_award_x end,
	[dp.."stallitems"] = function () require(pre.."stallitems_pb") pdt[dp.."stallitems"] = stallitems_x end,
	[dp.."itemtypes"] = function () require(pre.."itemtypes_pb") pdt[dp.."itemtypes"] = itemtypes_x end,
	[dp.."factioninfo"] = function () require(pre.."factioninfo_pb") pdt[dp.."factioninfo"] = factioninfo_x end,
	[dp.."auctionconfig"] = function () require(pre.."auctionconfig_pb") pdt[dp.."auctionconfig"] = auctionconfig_x end,
	[dp.."achieve"] = function () require(pre.."achieve_pb") pdt[dp.."achieve"] = achieve_x end,--cj_成就
	[dp.."achievegroup"] = function () require(pre.."achievegroup_pb") pdt[dp.."achievegroup"] = achievegroup_x end,--cj_成就
	[dp.."strengthenconf"] = function () require(pre.."strengthenconf_pb") pdt[dp.."strengthenconf"] = strengthenconf_x end,
	[dp.."extrastrengthenadd"] = function () require(pre.."extrastrengthenadd_pb") pdt[dp.."extrastrengthenadd"] = extrastrengthenadd_x end,
	[dp.."strengthenuplevel"] = function () require(pre.."strengthenuplevel_pb") pdt[dp.."strengthenuplevel"] = strengthenuplevel_x end,
	[dp.."equipexp"] = function () require(pre.."equipexp_pb") pdt[dp.."equipexp"] = equipexp_x end,
	--[dp.."equiplevelupprop"] = function () require(pre.."equiplevelupprop_pb") pdt[dp.."equiplevelupprop"] = equiplevelupprop_x end,
	[dp.."chatfixmessage"] = function () require(pre.."chatfixmessage_pb") pdt[dp.."chatfixmessage"] = chatfixmessage_x end,
	[dp.."personalquestion"] = function () require(pre.."personalquestion_pb") pdt[dp.."personalquestion"] = personalquestion_x end,
	[dp.."awardinfo"] = function () require(pre.."awardinfo_pb") pdt[dp.."awardinfo"] = awardinfo_x end,
	[dp.."noticetips"] = function () require(pre.."noticetips_pb") pdt[dp.."noticetips"] = noticetips_x end,--gg_跑马灯公告
	[dp.."lingshi"] = function () require(pre.."lingshi_pb") pdt[dp.."lingshi"] = lingshi_x end,
	[dp.."lingshigrade"] = function () require(pre.."lingshigrade_pb") pdt[dp.."lingshigrade"] = lingshigrade_x end,
	[dp.."lingshirecommend"] = function () require(pre.."lingshirecommend_pb") pdt[dp.."lingshirecommend"] = lingshirecommend_x end,
	[dp.."usequenceinfo"] = function () require(pre.."usequenceinfo_pb") pdt[dp.."usequenceinfo"] = usequenceinfo_x end, --jq_剧情动画
	[dp.."openactivity"] = function () require(pre.."openactivity_pb") pdt[dp.."openactivity"] = openactivity_x end,
	[dp.."levelupreward"] = function () require (pre.."levelupreward_pb") pdt[dp.."levelupreward"] = levelupreward_x end,	--djjl_等级奖励表
	[dp.."itemcompose"] = function () require (pre.."itemcompose_pb") pdt[dp.."itemcompose"] = itemcompose_x end,	--物品合成
	[dp.."tasknpc"] = function () require (pre.."tasknpc_pb") pdt[dp.."tasknpc"] = tasknpc_x end,	--任务npc rw表
	[dp.."tasknpc_ios"] = function () require (pre.."tasknpc_ios_pb") pdt[dp.."tasknpc_ios"] = tasknpc_ios_x end,	--任务npc ios rw表
	[dp.."territory"] = function () require (pre.."territory_pb") pdt[dp.."territory"] = territory_x end,	--任务npc rw表
	[dp.."signrewards"] = function () require (pre.."signrewards_pb") pdt[dp.."signrewards"] = signrewards_x end,	--签到配置表
	[dp.."wishtype"] = function () require(pre.."wishtype_pb") pdt[dp.."wishtype"] = wishtype_x end, --许愿池

	[dp.."titleinfo"] = function () require(pre.."titleinfo_pb") pdt[dp.."titleinfo"] = titleinfo_x end, --称号
	[dp.."titletab"] = function () require(pre.."titletab_pb") pdt[dp.."titletab"] = titletab_x end, --称号
	[dp.."textcolor"] = function () require(pre.."textcolor_pb") pdt[dp.."textcolor"] = textcolor_x end, --称号

	[dp.."titleinfoconfig"] = function () require(pre.."titleinfoconfig_pb") pdt[dp.."titleinfoconfig"] = titleinfoconfig_x end, --新称号（称号信息表）
	[dp.."titletypeconfig"] = function () require(pre.."titletypeconfig_pb") pdt[dp.."titletypeconfig"] = titletypeconfig_x end, --新称号（称号类型表）

	[dp.."angertips"] = function () require(pre.."angertips_pb") pdt[dp.."angertips"] = angertips_x end, --怒气
	[dp.."questionnaire"] = function () require(pre.."questionnaire_pb") pdt[dp.."questionnaire"] = questionnaire_x end, --问卷调查
	[dp.."bless"] = function () require(pre.."bless_pb") pdt[dp.."bless"] = bless_x end, --fh_拜福神
	[dp.."follownpc"] = function () require(pre.."follownpc_pb") pdt[dp.."follownpc"] = follownpc_x end, --功能npc
	[dp.."npctalk"] = function () require(pre.."npctalk_pb") pdt[dp.."npctalk"] = npctalk_x end, --功能npc
	[dp.."yewaiboss"] = function () require(pre.."yewaiboss_pb") pdt[dp.."yewaiboss"] = yewaiboss_x end, --野外首领
	[dp.."sevday"] = function () require(pre.."sevday_pb") pdt[dp.."sevday"] = sevday_x end, --野外首领
	[dp.."washes"] = function () require(pre.."washes_pb") pdt[dp.."washes"] = washes_x end,   --洗练相关
	[dp.."washtemp"] = function () require(pre.."washtemp_pb") pdt[dp.."washtemp"] = washtemp_x end, --洗练温度
	[dp.."washstau"] = function () require(pre.."washstau_pb") pdt[dp.."washstau"] = washstau_x end, --洗练饱和度
	[dp.."evencutconf"] = function () require(pre.."evencutconf_pb") pdt[dp.."evencutconf"] = evencutconf_x end, --连斩
	[dp.."activitydst"] = function () require(pre.."activitydst_pb") pdt[dp.."activitydst"] = activitydst_x end, --活动目标
	[dp.."lventryopen"] = function () require(pre.."lventryopen_pb") pdt[dp.."lventryopen"] = lventryopen_x end, --入口等级开放
	[dp.."lventryopen_ios"] = function () require(pre.."lventryopen_ios_pb") pdt[dp.."lventryopen_ios"] = lventryopen_ios_x end, --入口等级开放
	[dp.."redbag"] = function () require(pre.."redbag_pb") pdt[dp.."redbag"] = redbag_x end, --红包
	[dp.."worldredbag"] = function () require(pre.."worldredbag_pb") pdt[dp.."worldredbag"] = worldredbag_x end, --世界红包
	[dp.."rewardpreview"] = function () require(pre.."rewardpreview_pb") pdt[dp.."rewardpreview"] = rewardpreview_x end, --奖励预览
	[dp.."rewardrecycle"] = function () require(pre.."rewardrecycle_pb") pdt[dp.."rewardrecycle"] = rewardrecycle_x end, --奖励找回
	[dp.."fumo"] = function () require(pre.."fumo_pb") pdt[dp.."fumo"] = fumo_x end, --灵石战斗力值
	[dp.."effectconfig"] = function () require(pre.."effectconfig_pb") pdt[dp.."effectconfig"] = effectconfig_x end,--特效表单
	[dp.."collectbehavior"] = function () require(pre.."collectbehavior_pb") pdt[dp.."collectbehavior"] = collectbehavior_x end,--采集行为表

	[dp.."showhide"] = function () require(pre.."showhide_pb") pdt[dp.."showhide"] = showhide_x end,   --物件显藏
	[dp.."sceneshowobjs"] = function () require(pre.."sceneshowobjs_pb") pdt[dp.."sceneshowobjs"] = sceneshowobjs_x end,   --场景物件动态控制表
	[dp.."levelspecialeffectinfo"] = function () require(pre.."levelspecialeffectinfo_pb") pdt[dp.."levelspecialeffectinfo"] = levelspecialeffectinfo_x end,--fb_副本特殊效果
	[dp.."sublse"] = function () require(pre.."sublse_pb") pdt[dp.."sublse"] = sublse_x end,--fb_副本特殊效果
	[dp.."blacksmithreduce"] = function () require(pre.."blacksmithreduce_pb") pdt[dp.."blacksmithreduce"] = blacksmithreduce_x end, --帮会铁匠铺表
	[dp.."blacksmithlist"] = function () require(pre.."blacksmithlist_pb") pdt[dp.."blacksmithlist"] = blacksmithlist_x end,--帮会铁匠铺表
	[dp.."jhll_matchscorerules"] = function () require(pre.."jhll_matchscorerules_pb") pdt[dp.."jhll_matchscorerules"] = jhll_matchscorerules_x end,--江湖历练
	[dp.."levelcarnivalreward"] = function () require(pre.."levelcarnivalreward_pb") pdt[dp.."levelcarnivalreward"] = levelcarnivalreward_x end,--全民等级开放
	[dp.."act_collect"] = function () require(pre.."act_collect_pb") pdt[dp.."act_collect"] = act_collect_x end,--hd_限时兑换收集 act_collect
	[dp.."collect_carnival"] = function () require(pre.."collect_carnival_pb") pdt[dp.."collect_carnival"] = collect_carnival_x end,--hd_限时兑换收集 collect_carnival
	[dp.."exchange_carnival"] = function () require(pre.."exchange_carnival_pb") pdt[dp.."exchange_carnival"] = exchange_carnival_x end,--hd_限时兑换收集 exchange_carnival

	-- body
	[dp.."jhll_preview"] = function () require(pre.."jhll_preview_pb") pdt[dp.."jhll_preview"] = jhll_preview_x end,--江湖历练
	[dp.."jhll_robot"] = function () require(pre.."jhll_robot_pb") pdt[dp.."jhll_robot"] = jhll_robot_x end,--江湖历练
	[dp.."equiprefinecost"] = function () require(pre.."equiprefinecost_pb") pdt[dp.."equiprefinecost"] = equiprefinecost_x end,--装备洗炼tipsUI
	[dp.."audioclipconfig"] = function () require(pre.."audioclipconfig_pb") pdt[dp.."audioclipconfig"] = audioclipconfig_x end,--声音配置
	[dp.."shenshou"] = function () require(pre.."shenshou_pb") pdt[dp.."shenshou"] = shenshou_x end,--ss_神兽
	[dp.."shenshouaddproperty"] = function () require(pre.."shenshouaddproperty_pb") pdt[dp.."shenshouaddproperty"] = shenshouaddproperty_x end,--ss_神兽
	[dp.."shenshoubreak"] = function () require(pre.."shenshoubreak_pb") pdt[dp.."shenshoubreak"] = shenshoubreak_x end,--ss_神兽
	[dp.."shenshouquality"] = function () require(pre.."shenshouquality_pb") pdt[dp.."shenshouquality"] = shenshouquality_x end,--ss_神兽
	[dp.."shenshouskill"] = function () require(pre.."shenshouskill_pb") pdt[dp.."shenshouskill"] = shenshouskill_x end,--ss_神兽
	[dp.."shenshougrow"] = function () require(pre.."shenshougrow_pb") pdt[dp.."shenshougrow"] = shenshougrow_x end,--ss_神兽
	[dp.."shenshouskillrandom"] = function () require(pre.."shenshouskillrandom_pb") pdt[dp.."shenshouskillrandom"] = shenshouskillrandom_x end,--ss_神兽
	[dp.."shenshoupositionopen"] = function () require(pre.."shenshoupositionopen_pb") pdt[dp.."shenshoupositionopen"] = shenshoupositionopen_x end,--ss_神兽
	[dp.."raremonster"] = function () require(pre.."raremonster_pb") pdt[dp.."raremonster"] = raremonster_x end,--rm_神兽
	[dp.."ride"] = function () require(pre.."ride_pb") pdt[dp.."ride"] = ride_x end,--zq_坐骑配置表
	[dp.."boitetask"] = function () require(pre.."boitetask_pb") pdt[dp.."boitetask"] = boitetask_x end,--jl_酒楼任务表
	[dp.."boiteaward"] = function () require(pre.."boiteaward_pb") pdt[dp.."boiteaward"] = boiteaward_x end,--jl_酒楼任务表
	[dp.."goldshopconfig"]= function () require(pre.."goldshopconfig_pb") pdt[dp.."goldshopconfig"] = goldshopconfig_x end,--gsc_元宝商城
	[dp.."goldshopgoodsconfig"]= function () require(pre.."goldshopgoodsconfig_pb") pdt[dp.."goldshopgoodsconfig"] = goldshopgoodsconfig_x end,--gsc_元宝商城
	[dp.."goldshopgoodsconfig_ios"]= function () require(pre.."goldshopgoodsconfig_ios_pb") pdt[dp.."goldshopgoodsconfig_ios"] = goldshopgoodsconfig_ios_x end,--gsc_元宝商城
	[dp.."mingjiangarea"] = function () require(pre.."mingjiangarea_pb") pdt[dp.."mingjiangarea"] = mingjiangarea_x end,--历代名将
	[dp.."mingjiangtips"] = function () require(pre.."mingjiangtips_pb") pdt[dp.."mingjiangtips"] = mingjiangtips_x end,--历代名将->七宗罪->上古神兽
	[dp.."mingjiang"] = function () require(pre.."mingjiang_pb") pdt[dp.."mingjiang"] = mingjiang_x end,--历代名将
	[dp.."strength"] = function() require(pre.."strength_pb") pdt[dp.."strength"] = strength_x end, --战力表
	[dp.."leveldropitemslist"] = function() require(pre.."leveldropitemslist_pb") pdt[dp.."leveldropitemslist"] = leveldropitemslist_x end, --关卡奖励预览表
	[dp.."globalquestion"] = function() require(pre.."globalquestion_pb") pdt[dp.."globalquestion"] = globalquestion_x end, --全服答题表
	[dp.."loginactivity"] = function() require(pre.."loginactivity_pb") pdt[dp.."loginactivity"] = loginactivity_x end, --登录活动
	[dp.."conscienceshopgoods"] = function() require(pre.."conscienceshopgoods_pb") pdt[dp.."conscienceshopgoods"] = conscienceshopgoods_x end, --全服答题表
	[dp.."honorconfig"] = function() require(pre.."honorconfig_pb") pdt[dp.."honorconfig"] = honorconfig_x end, --头衔配置
	[dp.."combatreward"] = function() require(pre.."combatreward_pb") pdt[dp.."combatreward"] = combatreward_x end, --头衔配置
	[dp.."onlinereward"] = function() require(pre.."onlinereward_pb") pdt[dp.."onlinereward"] = onlinereward_x end, --头衔配置
	[dp.."skillupgrade"] = function () require(pre.."skillupgrade_pb") pdt[dp.."skillupgrade"] = skillupgrade_x end,--技能升级表
	[dp.."headportrait"] = function () require(pre.."headportrait_pb") pdt[dp.."headportrait"] = headportrait_x end,--头像表
	[dp.."strengthenbreak"] = function () require(pre.."strengthenbreak_pb") pdt[dp.."strengthenbreak"] = strengthenbreak_x end,--部位突破
	[dp.."strengthenequiplimit"] = function () require(pre.."strengthenequiplimit_pb") pdt[dp.."strengthenequiplimit"] = strengthenequiplimit_x end,--装备等级强化
	[dp.."skyfallguildscorereward"] = function () require(pre.."skyfallguildscorereward_pb") pdt[dp.."skyfallguildscorereward"] = skyfallguildscorereward_x end,--tm_天墓禁地排名奖励预览表
	[dp.."skillreset"] = function () require(pre.."skillreset_pb") pdt[dp.."skillreset"] = skillreset_x end,--技能学习配置表
	[dp.."rolebaseattr"] = function () require(pre.."rolebaseattr_pb") pdt[dp.."rolebaseattr"] = rolebaseattr_x end,--sx_玩家基础属性
	[dp.."factionescort"] = function () require(pre.."factionescort_pb") pdt[dp.."factionescort"] = factionescort_x end,--bh_帮会押镖
	[dp.."buytimes"] = function () require(pre.."buytimes_pb") pdt[dp.."buytimes"] = buytimes_x end,--jc_演武场购买次数
	[dp.."stoneconfig"] = function () require(pre.."stoneconfig_pb") pdt[dp.."stoneconfig"] = stoneconfig_x end,--ss_神石配置表
	[dp.."friendshiplevel"] = function () require(pre.."friendshiplevel_pb") pdt[dp.."friendshiplevel"] = friendshiplevel_x end,--hy_好友配置表
	[dp.."friendshipadd"] = function () require(pre.."friendshipadd_pb") pdt[dp.."friendshipadd"] = friendshipadd_x end,--hy_好友配置表
	[dp.."continuousbonus"] = function () require(pre.."continuousbonus_pb") pdt[dp.."continuousbonus"] = continuousbonus_x end,--hy_好友配置表
	[dp.."warofroyalcityaward"] = function () require(pre.."warofroyalcityaward_pb") pdt[dp.."warofroyalcityaward"] = warofroyalcityaward_x end,--hc_皇城之战奖励
	[dp.."meetanswer"] = function () require(pre.."meetanswer_pb") pdt[dp.."meetanswer"] = meetanswer_x end,--ss_帮会答题
	[dp.."raremonsterpreview"] = function () require(pre.."raremonsterpreview_pb") pdt[dp.."raremonsterpreview"] = raremonsterpreview_x end,--rm_神兽
	[dp.."chatlink"] = function () require(pre.."chatlink_pb") pdt[dp.."chatlink"] = chatlink_x end,-- 字典配置表
	[dp.."gift"] = function () require(pre.."gift_pb") pdt[dp.."gift"] = gift_x end,--zb_主播
	[dp.."chatcolor"] = function () require(pre.."chatcolor_pb") pdt[dp.."chatcolor"] = chatcolor_x end,-- 字典配置表
	[dp.."multilevelaccount"] = function () require(pre.."multilevelaccount_pb") pdt[dp.."multilevelaccount"] = multilevelaccount_x end,--fb_多人副本结算
	[dp.."multilevelaccountmarkgrad"] = function () require(pre.."multilevelaccountmarkgrad_pb") pdt[dp.."multilevelaccountmarkgrad"] = multilevelaccountmarkgrad_x end,--fb_多人副本结算
	[dp.."multilevelaccountintimacygrad"] = function () require(pre.."multilevelaccountintimacygrad_pb") pdt[dp.."multilevelaccountintimacygrad"] = multilevelaccountintimacygrad_x end,--fb_多人副本结算
	[dp.."attrfighting"] = function () require(pre.."attrfighting_pb") pdt[dp.."attrfighting"] = attrfighting_x end,--zb_系统装备表
	[dp.."singlelevelaccount"] = function () require(pre.."singlelevelaccount_pb") pdt[dp.."singlelevelaccount"] = singlelevelaccount_x end,--fb_单人副本结算
	[dp.."newactivity"] = function () require(pre.."newactivity_pb") pdt[dp.."newactivity"] = newactivity_x end,--fy_等级封印
	[dp.."levelpreview"] = function () require(pre.."levelpreview_pb") pdt[dp.."levelpreview"] = levelpreview_x end,--fy_等级封印
	[dp.."news"] = function () require(pre.."news_pb") pdt[dp.."news"] = news_x end,--jj_江湖见闻
	[dp.."achievelevel"] = function () require(pre.."achievelevel_pb") pdt[dp.."achievelevel"] = achievelevel_x end,--cj_成就
	[dp.."scenemapcfg"] = function () require(pre.."scenemapcfg_pb") pdt[dp.."scenemapcfg"] = scenemapcfg_x end,--dt_小地图配置表
	[dp.."scenemapsign"] = function () require(pre.."scenemapsign_pb") pdt[dp.."scenemapsign"] = scenemapsign_x end,--dt_小地图资源表
	[dp.."factioncompeteconfig"] = function () require(pre.."factioncompeteconfig_pb") pdt[dp.."factioncompeteconfig"] = factioncompeteconfig_x end,--帮会争霸
	[dp.."factioncompeteshow"] = function () require(pre.."factioncompeteshow_pb") pdt[dp.."factioncompeteshow"] = factioncompeteshow_x end,--帮会争霸
	[dp.."growth"] = function () require(pre.."growth_pb") pdt[dp.."growth"] = growth_x end,--cz_成长之路
	[dp.."teammodel"] = function () require(pre.."teammodel_pb") pdt[dp.."teammodel"] = teammodel_x end,--tm_队伍模型
	[dp.."teamword"] = function () require(pre.."teamword_pb") pdt[dp.."teamword"] = teamword_x end,--tm_队伍模型
	[dp.."teamtargettab"] = function () require(pre.."teamtargettab_pb") pdt[dp.."teamtargettab"] = teamtargettab_x end,--tm_队伍模型
	[dp.."teamtarget"] = function () require(pre.."teamtarget_pb") pdt[dp.."teamtarget"] = teamtarget_x end,--tm_队伍模型
	[dp.."huaweielectric"]=function() require(pre.."huaweielectric_pb") pdt[dp.."huaweielectric"]=huaweielectric_x end,--华为电量配置表
	[dp.."dailygiftbag"]=function() require(pre.."dailygiftbag_pb") pdt[dp.."dailygiftbag"]=dailygiftbag_x end,--136礼包配置表
	[dp.."dailygiftbag_ios"]=function() require(pre.."dailygiftbag_ios_pb") pdt[dp.."dailygiftbag_ios"]=dailygiftbag_ios_x end,--136礼包配置表
	[dp.."newloginspree"]=function() require(pre.."newloginspree_pb") pdt[dp.."newloginspree"]=newloginspree_x end,--dldlb_登录大礼包表
	[dp.."activitybigprofitreward"]=function() require(pre.."activitybigprofitreward_pb") pdt[dp.."activitybigprofitreward"]=activitybigprofitreward_x end,--一本万利活动奖励表
	[dp.."activitybigprofitreward_ios"]=function() require(pre.."activitybigprofitreward_ios_pb") pdt[dp.."activitybigprofitreward_ios"]=activitybigprofitreward_ios_x end,--一本万利活动奖励表

	[dp.."lunhui"]=function() require(pre.."lunhui_pb") pdt[dp.."lunhui"]=lunhui_x end,--轮回头衔
	[dp.."rankinglist"]=function() require(pre.."rankinglist_pb") pdt[dp.."rankinglist"]=rankinglist_x end,--等级排行榜表
	[dp.."firstgiftpackage"]=function() require(pre.."firstgiftpackage_pb") pdt[dp.."firstgiftpackage"]=firstgiftpackage_x end, -- 首充礼包
	[dp.."equipreward"]=function() require(pre.."equipreward_pb") pdt[dp.."equipreward"]=equipreward_x end,--套装奖励
	[dp.."activitytimelimitpackage"]=function() require(pre.."activitytimelimitpackage_pb") pdt[dp.."activitytimelimitpackage"]=activitytimelimitpackage_x end,--xslb_限时礼包表
	[dp.."activitytimelimitpackage_ios"]=function() require(pre.."activitytimelimitpackage_ios_pb") pdt[dp.."activitytimelimitpackage_ios"]=activitytimelimitpackage_ios_x end,--xslb_限时礼包表
	[dp.."viplevelright"]=function() require(pre.."viplevelright_pb") pdt[dp.."viplevelright"]=viplevelright_x end, -- vip配置表新版
	[dp.."viptemplate"]=function() require(pre.."viptemplate_pb") pdt[dp.."viptemplate"]=viptemplate_x end, -- vip配置表新版
	[dp.."battlechart"]=function() require(pre.."battlechart_pb") pdt[dp.."battlechart"]=battlechart_x end,
	[dp.."buffdata"]=function() require(pre.."buffdata_pb") pdt[dp.."buffdata"]=buffdata_x end, -- 天墓禁地buff表
	[dp.."dreamlandconf"]=function() require(pre.."dreamlandconf_pb") pdt[dp.."dreamlandconf"]=dreamlandconf_x end, -- 千机幻境
	[dp.."skyfallchallenge"]=function() require(pre.."skyfallchallenge_pb") pdt[dp.."skyfallchallenge"]=skyfallchallenge_x end, -- 天墓禁地挑战
	[dp.."actfightaward"]=function() require(pre.."actfightaward_pb") pdt[dp.."actfightaward"]=actfightaward_x end, --活动奖励表
	[dp.."activitypreview"]=function() require(pre.."activitypreview_pb") pdt[dp.."activitypreview"]=activitypreview_x end, -- hdyg_活动预告
	[dp.."qianjiawardcount"]=function() require(pre.."qianjiawardcount_pb") pdt[dp.."qianjiawardcount"]=qianjiawardcount_x end, -- 千机幻境开始
	[dp.."qianjiaward"]=function() require(pre.."qianjiaward_pb") pdt[dp.."qianjiaward"]=qianjiaward_x end, -- 千机幻境结算
	[dp.."weekcards"]=function() require(pre.."weekcards_pb") pdt[dp.."weekcards"]=weekcards_x end, -- 周卡月卡
	[dp.."weekcards_ios"]=function() require(pre.."weekcards_ios_pb") pdt[dp.."WeekCards_ios"]=weekcards_ios_x end, -- 周卡月卡
	[dp.."skyfallrule"]=function() require(pre.."skyfallrule_pb") pdt[dp.."skyfallrule"]=skyfallrule_x end, -- 天墓禁地规则
	[dp.."chatpreandsuf"]=function() require(pre.."chatpreandsuf_pb") pdt[dp.."chatpreandsuf"]=chatpreandsuf_x end,--聊天前缀名和后缀名配置表
	[dp.."buffindex"]=function() require(pre.."buffindex_pb") pdt[dp.."buffindex"]=buffindex_x end,--PVP通用配置表
	[dp.."buffpicture"]=function() require(pre.."buffpicture_pb") pdt[dp.."buffpicture"]=buffpicture_x end,--PVP通用配置表
	[dp.."grouppage"]=function() require(pre.."grouppage_pb") pdt[dp.."grouppage"]=grouppage_x end,--bq_我要变强
	[dp.."framepage"]=function() require(pre.."framepage_pb") pdt[dp.."framepage"]=framepage_x end,--bq_我要变强_FramePage
	[dp.."powerpage"]=function() require(pre.."powerpage_pb") pdt[dp.."powerpage"]=powerpage_x end,--bq_我要变强_powerpage
	[dp.."limitedanimal"]=function() require(pre.."limitedanimal_pb") pdt[dp.."limitedanimal"]=limitedanimal_x end,--bq_我要变强_powerpage
	[dp.."dailyrecharge"]=function() require(pre.."dailyrecharge_pb") pdt[dp.."dailyrecharge"]=dailyrecharge_x end,--mrcz_每日充值
	[dp.."operateactivity"]=function() require(pre.."operateactivity_pb") pdt[dp.."operateactivity"]=operateactivity_x end,--yyhd_运营活动时间表
	[dp.."operateactivity_xh"]=function() require(pre.."operateactivity_xh_pb") pdt[dp.."operateactivity_xh"]=operateactivity_xh_x end,--yyhd_运营活动时间表
	[dp.."operateactivity_qx"]=function() require(pre.."operateactivity_qx_pb") pdt[dp.."operateactivity_qx"]=operateactivity_qx_x end,--yyhd_运营活动时间表
	[dp.."operateactivity_ios"]=function() require(pre.."operateactivity_ios_pb") pdt[dp.."operateactivity_ios"]=operateactivity_ios_x end,--yyhd_运营活动时间表
	[dp.."cumulativerecharge"]=function() require(pre.."cumulativerecharge_pb") pdt[dp.."cumulativerecharge"]=cumulativerecharge_x end,--lc_开服累充配表
	[dp.."renewpackage"]=function() require(pre.."renewpackage_pb") pdt[dp.."renewpackage"]=renewpackage_x end,--续费充值
	[dp.."playernum"]=function() require(pre.."playernum_pb") pdt[dp.."playernum"]=playernum_x end,--同频人数
	[dp.."shenshoutype"]=function() require(pre.."shenshoutype_pb") pdt[dp.."shenshoutype"]=shenshoutype_x end,--神兽类型名字
	[dp.."wldhgroup"]=function() require(pre.."wldhgroup_pb") pdt[dp.."wldhgroup"]=wldhgroup_x end,-- wl_武林大会 WLDHGroup
	[dp.."taskquestion"]=function() require(pre.."taskquestion_pb") pdt[dp.."taskquestion"]=taskquestion_x end,-- wl_武林大会 WLDHGroup
	--[dp.."guiderobotattrskill"]=function() require(pre.."guiderobotattrskill_pb") pdt[dp.."guiderobotattrskill"]=guiderobotattrskill_x end,-- drxshc_单人新手皇城机器人属性技能表 GuideRobotAttrSkill
	[dp.."robotfirstname"]=function() require(pre.."robotfirstname_pb") pdt[dp.."robotfirstname"]=robotfirstname_x end,-- jqr_机器人随机姓名表 RobotFirstName
	[dp.."robotmiddlename"]=function() require(pre.."robotmiddlename_pb") pdt[dp.."robotmiddlename"]=robotmiddlename_x end,-- jqr_机器人随机姓名表 RobotMiddleName
	[dp.."robotmalename"]=function() require(pre.."robotmalename_pb") pdt[dp.."robotmalename"]=robotmalename_x end,-- jqr_机器人随机姓名表 RobotMaleName
	[dp.."robotfemalename"]=function() require(pre.."robotfemalename_pb") pdt[dp.."robotfemalename"]=robotfemalename_x end,-- jqr_机器人随机姓名表 RobotFemaleName
	[dp.."killnotice"]=function() require(pre.."killnotice_pb") pdt[dp.."killnotice"]=killnotice_x end,-- PVP通用配置表 KillNotice
	[dp.."tabinfo"]=function() require(pre.."tabinfo_pb") pdt[dp.."tabinfo"]=tabinfo_x end,-- ui_配置表
	[dp.."monsternote"]=function() require(pre.."monsternote_pb") pdt[dp.."monsternote"]=monsternote_x end,-- zs_怪物注释表
	[dp.."holidaygift"]=function() require(pre.."holidaygift_pb") pdt[dp.."holidaygift"]=holidaygift_x end,-- 节日计费点礼包
	[dp.."holidaygift_ios"]=function() require(pre.."holidaygift_ios_pb") pdt[dp.."holidaygift_ios"]=holidaygift_ios_x end,-- 节日计费点礼包
	[dp.."levelprocessconfig"]=function() require(pre.."levelprocessconfig_pb") pdt[dp.."levelprocessconfig"]=levelprocessconfig_x end,-- fb_多人副本结算
	[dp.."wordtext"]=function() require(pre.."wordtext_pb") pdt[dp.."wordtext"]=wordtext_x end,--聊天 文字表情
	[dp.."laceyface"]=function() require(pre.."laceyface_pb") pdt[dp.."laceyface"]=laceyface_x end,--聊天 俏皮话
	[dp.."holidayoflanding"]=function() require(pre.."holidayoflanding_pb") pdt[dp.."holidayoflanding"]=holidayoflanding_x end,-- 节日登录礼包
	[dp.."marrysystem"]=function() require(pre.."marrysystem_pb") pdt[dp.."marrysystem"]=marrysystem_x end,--结婚
	[dp.."wedding"]=function() require(pre.."wedding_pb") pdt[dp.."wedding"]=wedding_x end,--结婚
	[dp.."weddingdescription"]=function() require(pre.."weddingdescription_pb") pdt[dp.."weddingdescription"]=weddingdescription_x end,--结婚
	[dp.."zhufu"]=function() require(pre.."zhufu_pb") pdt[dp.."zhufu"]=zhufu_x end,--结婚
	[dp.."sit"] = function () require(pre.."sit_pb") pdt[dp.."sit"] = sit_x end,--打坐表
	[dp.."lefttab"] = function () require(pre.."lefttab_pb") pdt[dp.."lefttab"] = lefttab_x end,--拍照页签表
	[dp.."filters"] = function () require(pre.."filters_pb") pdt[dp.."filters"] = filters_x end,--拍照滤镜表
	[dp.."dancelevel"] = function () require(pre.."dancelevel_pb") pdt[dp.."dancelevel"] = dancelevel_x end,--跳舞表
	[dp.."dancename"] = function () require(pre.."dancename_pb") pdt[dp.."dancename"] = dancename_x end,--跳舞表
	[dp.."danceadd"] = function () require(pre.."danceadd_pb") pdt[dp.."danceadd"] = danceadd_x end,--跳舞表
	[dp.."challenge"] = function () require(pre.."challenge_pb") pdt[dp.."challenge"] = challenge_x end,--仇人表
	[dp.."rolling"] = function () require(pre.."rolling_pb") pdt[dp.."rolling"] = rolling_x end,--财源滚滚
	[dp.."auctionpre"] = function () require(pre.."auctionpre_pb") pdt[dp.."auctionpre"] = auctionpre_x end,--拍卖系统表
	[dp.."shenshou"] = function () require(pre.."shenshou_pb") pdt[dp.."shenshou"] = shenshou_x end,--神兽表
	[dp.."shenshoubase"] = function () require(pre.."shenshoubase_pb") pdt[dp.."shenshoubase"] = shenshoubase_x end,--神兽基础表
	[dp.."shenshoubaseattr"] = function () require(pre.."shenshoubaseattr_pb") pdt[dp.."shenshoubaseattr"] = shenshoubaseattr_x end,--神兽基础属性表
	[dp.."shenshoustar"] = function () require(pre.."shenshoustar_pb") pdt[dp.."shenshoustar"] = shenshoustar_x end,--神兽升星表
	[dp.."shenshoucollect"] = function () require(pre.."shenshoucollect_pb") pdt[dp.."shenshoucollect"] = shenshoucollect_x end,--神兽收集表
	[dp.."beaststrengthenconfig"] = function () require(pre.."beaststrengthenconfig_pb") pdt[dp.."beaststrengthenconfig"] = beaststrengthenconfig_x end,--神兽天赋强化表
	[dp.."beaststrengthenbreak"] = function () require(pre.."beaststrengthenbreak_pb") pdt[dp.."beaststrengthenbreak"] = beaststrengthenbreak_x end,--神兽天赋突破表
	[dp.."beastextrastrengthenadd"] = function () require(pre.."beastextrastrengthenadd_pb") pdt[dp.."beastextrastrengthenadd"] = beastextrastrengthenadd_x end,--神兽天赋突破激活表
	[dp.."beaststrengthenuplevel"] = function () require(pre.."beaststrengthenuplevel_pb") pdt[dp.."beaststrengthenuplevel"] = beaststrengthenuplevel_x end,--神兽之力效果表
	[dp.."beaststrengthenlevellimits"] = function () require(pre.."beaststrengthenlevellimits_pb") pdt[dp.."beaststrengthenlevellimits"] = beaststrengthenlevellimits_x end,--神兽天赋-限制表
	[dp.."beastskilleffect"] = function () require(pre.."beastskilleffect_pb") pdt[dp.."beastskilleffect"] = beastskilleffect_x end,--神兽星级限制突破表
	[dp.."beastcollect"] = function () require(pre.."beastcollect_pb") pdt[dp.."beastcollect"] = beastcollect_x end,--神兽收集
	[dp.."wfexpand"] = function () require(pre.."wfexpand_pb") pdt[dp.."wfexpand"] = wfexpand_x end,--武器竞技
	[dp.."clonedlv"]=function() require(pre.."clonedlv_pb") pdt[dp.."clonedlv"]=clonedlv_x end,-- 分身等级表
	[dp.."clonedskill"]=function() require(pre.."clonedskill_pb") pdt[dp.."clonedskill"]=clonedskill_x end,-- 分身等级表
	[dp.."shenshoupreview"]=function() require(pre.."shenshoupreview_pb") pdt[dp.."shenshoupreview"]=shenshoupreview_x end,--神兽预览
	[dp.."raremonster10thpreview"]=function() require(pre.."raremonster10thpreview_pb") pdt[dp.."raremonster10thpreview"]=raremonster10thpreview_x end,--神兽十连抽预览
	[dp.."bufftriggercfg"] = function () require(pre.."bufftriggercfg_pb") pdt[dp.."bufftriggercfg"] = bufftriggercfg_x end,--buff触发效果表
	[dp.."shenshouformation"] = function () require(pre.."shenshouformation_pb") pdt[dp.."shenshouformation"] = shenshouformation_x end,--神兽阵法表
	[dp.."shenshoupositionlevel"] = function () require(pre.."shenshoupositionlevel_pb") pdt[dp.."shenshoupositionlevel"] = shenshoupositionlevel_x end,--神兽阵法位置限制表
	[dp.."shenshouformationproperty"] = function () require(pre.."shenshouformationproperty_pb") pdt[dp.."shenshouformationproperty"] = shenshouformationproperty_x end,
	[dp.."shenshouformationactivation"] = function () require(pre.."shenshouformationactivation_pb") pdt[dp.."shenshouformationactivation"] = shenshouformationactivation_x end,--神兽阵法属性表
	[dp.."beastcollect"]=function() require(pre.."beastcollect_pb") pdt[dp.."beastcollect"]=beastcollect_x end,
	[dp.."activetag"]=function() require(pre.."activetag_pb") pdt[dp.."activetag"]=activetag_x end, --活动页签
	--beastcollect
	[dp.."mondaycourtesy"]=function() require(pre.."mondaycourtesy_pb") pdt[dp.."mondaycourtesy"]=mondaycourtesy_x end,  --周一豪礼
	[dp.."drawtask"]=function() require(pre.."drawtask_pb") pdt[dp.."drawtask"]=drawtask_x end,                          --周一豪礼任务表
	[dp.."exchangebutton"]=function() require(pre.."exchangebutton_pb") pdt[dp.."exchangebutton"]=exchangebutton_x end,  --jh_交互选项表
	[dp.."exchangetype"]=function() require(pre.."exchangetype_pb") pdt[dp.."exchangetype"]=exchangetype_x end,  --jh_交互选项表
	[dp.."bwexchangetype"]=function() require(pre.."bwexchangetype_pb") pdt[dp.."bwexchangetype"]=bwexchangetype_x end,  --jh_交互选项表(跨服)
	[dp.."itemsource"]=function() require(pre.."itemsource_pb") pdt[dp.."itemsource"]=itemsource_x end,  --获取途径
	[dp.."itemsourcelist"]=function() require(pre.."itemsourcelist_pb") pdt[dp.."itemsourcelist"]=itemsourcelist_x end,  --获取途径
	[dp.."singlepenrecharge"]=function() require(pre.."singlepenrecharge_pb") pdt[dp.."singlepenrecharge"]=singlepenrecharge_x end,  --单笔充值
	[dp.."chinaarea"]=function() require(pre.."chinaarea_pb") pdt[dp.."chinaarea"]=chinaarea_x end,  --dqss_省市地区数据
	[dp.."tagname"]=function() require(pre.."tagname_pb") pdt[dp.."tagname"]=tagname_x end,  --kj_个人空间
	[dp.."topic"]=function() require(pre.."topic_pb") pdt[dp.."topic"]=topic_x end,  --kj_个人空间
	[dp.."limititem"]=function() require(pre.."limititem_pb") pdt[dp.."limititem"]=limititem_x end,  --xc道具表
	[dp.."cpquestion"]=function() require(pre.."cpquestion_pb") pdt[dp.."cpquestion"]=cpquestion_x end,  --cp问题填写
	[dp.."cpanswer"]=function() require(pre.."cpanswer_pb") pdt[dp.."cpanswer"]=cpanswer_x end,  --cp问题填写
	[dp.."initpet"]=function() require(pre.."initpet_pb") pdt[dp.."initpet"]=initpet_x end,  --cp宠物
	[dp.."petfeed"]=function() require(pre.."petfeed_pb") pdt[dp.."petfeed"]=petfeed_x end,  --cp宠物
	[dp.."petexpadd"]=function() require(pre.."petexpadd_pb") pdt[dp.."petexpadd"]=petexpadd_x end,  --cp宠物
	[dp.."random"]=function() require(pre.."random_pb") pdt[dp.."random"]=random_x end,  --cp任务
	[dp.."scene"]=function() require(pre.."scene_pb") pdt[dp.."scene"]=scene_x end,  --cp任务
	[dp.."nightscene"]=function() require(pre.."nightscene_pb") pdt[dp.."nightscene"]=nightscene_x end,  --cp夜间任务
	[dp.."xiaomingjiang"]=function() require(pre.."xiaomingjiang_pb") pdt[dp.."xiaomingjiang"]=xiaomingjiang_x end,  --xqz小七宗罪
	[dp.."robotattr4client"] = function () require(pre.."robotattr4client_pb") pdt[dp.."robotattr4client"] = robotattr4client_x end,--jqr_机器人属性技能表
	[dp.."cjconstellation"] = function () require(pre.."cjconstellation_pb") pdt[dp.."cjconstellation"] = cjconstellation_x end,--cj_创角
	[dp.."zhenxinhua"] = function () require(pre.."zhenxinhua_pb") pdt[dp.."zhenxinhua"] = zhenxinhua_x end,--cp_初见场景
	[dp.."factionwelfare"] = function () require(pre.."factionwelfare_pb") pdt[dp.."factionwelfare"] = factionwelfare_x end,--bh_帮会系统表
	[dp.."arrestwanted"] = function () require(pre.."arrestwanted_pb") pdt[dp.."arrestwanted"] = arrestwanted_x end,--cr_仇人系统
	[dp.."needlegame"] = function () require(pre.."needlegame_pb") pdt[dp.."needlegame"] = needlegame_x end,--cp小游戏表
	[dp.."skyfallzhufu"] = function () require(pre.."skyfallzhufu_pb") pdt[dp.."skyfallzhufu"] = skyfallzhufu_x end,--cp小游戏表
	[dp.."evaluate"] = function () require(pre.."evaluate_pb") pdt[dp.."evaluate"] = evaluate_x end,--cp小游戏表
	[dp.."petchat"] = function () require(pre.."petchat_pb") pdt[dp.."petchat"] = petchat_x end,--cp宠物
	[dp.."minitab"] = function () require(pre.."minitab_pb") pdt[dp.."minitab"] = minitab_x end,--lt_聊天配置表 MiniTab
	[dp.."shenshoulevel"] = function () require(pre.."shenshoulevel_pb") pdt[dp.."shenshoulevel"] = shenshoulevel_x end,--ss_神兽表 shenshoulevel
	[dp.."shenshouskills"] = function () require(pre.."shenshouskills_pb") pdt[dp.."shenshouskills"] = shenshouskills_x end,--ss_神兽技能表
	[dp.."shenshouskillbaropen"] = function () require(pre.."shenshouskillbaropen_pb") pdt[dp.."shenshouskillbaropen"] = shenshouskillbaropen_x end,--ss_神兽技能表
	[dp.."shenshouexpitem"] = function () require(pre.."shenshouexpitem_pb") pdt[dp.."shenshouexpitem"] = shenshouexpitem_x end,--ss_神兽技能表
	[dp.."cprule"] = function () require(pre.."cprule_pb") pdt[dp.."cprule"] = cprule_x end,--cprule cprule
	[dp.."modelgrowth"] = function () require(pre.."modelgrowth_pb") pdt[dp.."modelgrowth"] = modelgrowth_x end, --xyzp_幸运轮盘
	[dp.."packagedetails"] = function () require(pre.."packagedetails_pb") pdt[dp.."packagedetails"] = packagedetails_x end, --xyzp_幸运轮盘
	[dp.."wheeloffortune"] = function () require(pre.."wheeloffortune_pb") pdt[dp.."wheeloffortune"] = wheeloffortune_x end, --xyzp_幸运轮盘
	[dp.."shenshourecommend"] = function () require(pre.."shenshourecommend_pb") pdt[dp.."shenshourecommend"] = shenshourecommend_x end, --ss_神兽推荐
	[dp.."jjcshenshou"] = function () require(pre.."jjcshenshou_pb") pdt[dp.."jjcshenshou"] = jjcshenshou_x end, --jjc_竞技场神兽配置
	[dp.."uifenbao"] = function () require(pre.."uifenbao_pb") pdt[dp.."uifenbao"] = uifenbao_x end, --包体资源分包表 UIFenBao
	[dp.."scenefenbao"] = function () require(pre.."scenefenbao_pb") pdt[dp.."scenefenbao"] = scenefenbao_x end, --包体资源分包表 SceneFenBao
	[dp.."npcmissionfenbao"] = function () require(pre.."npcmissionfenbao_pb") pdt[dp.."npcmissionfenbao"] = npcmissionfenbao_x end, --包体资源分包表 NPCMissionFenBao
	[dp.."goodvoicetitletype"] = function () require(pre.."goodvoicetitletype_pb") pdt[dp.."goodvoicetitletype"] = goodvoicetitletype_x end, --江湖好声音 GoodVoiceTitleType
	[dp.."goodvoicetitle"] = function () require(pre.."goodvoicetitle_pb") pdt[dp.."goodvoicetitle"] = goodvoicetitle_x end, --江湖好声音 GoodVoiceTitle
	[dp.."voicegrade"] = function () require(pre.."voicegrade_pb") pdt[dp.."voicegrade"] = voicegrade_x end, --江湖好声音 VoiceGrade
	[dp.."limitfashion"] = function () require(pre.."limitfashion_pb") pdt[dp.."limitfashion"] = limitfashion_x end, --绝版时装
	[dp.."gameprograss"] = function () require(pre.."gameprograss_pb") pdt[dp.."gameprograss"] = gameprograss_x end, --CP小游戏
	[dp.."cp_homestead"] = function () require(pre.."cp_homestead_pb") pdt[dp.."cp_homestead"] = cp_homestead_x end, --CP_捉迷藏家具表
	[dp.."newjhll"] = function () require(pre.."newjhll_pb") pdt[dp.."newjhll"] = newjhll_x end, --新江湖历练
	[dp.."skilladvise"] = function () require(pre.."skilladvise_pb") pdt[dp.."skilladvise"] = skilladvise_x end, --技能方案推荐
	[dp.."musicgame"] = function () require(pre.."musicgame_pb") pdt[dp.."musicgame"] = musicgame_x end, --CP琴箫合奏
	[dp.."integraltreasurebox"] = function () require(pre.."integraltreasurebox_pb") pdt[dp.."integraltreasurebox"] = integraltreasurebox_x end,--jnh_新服嘉年华
	[dp.."damaoxian"] = function () require(pre.."damaoxian_pb") pdt[dp.."damaoxian"] = damaoxian_x end,--cp_初见场景 大冒险
	[dp.."activityfenbao"] = function () require(pre.."activityfenbao_pb") pdt[dp.."activityfenbao"] = activityfenbao_x end,--包体资源分包表
	[dp.."musicaction"] = function () require(pre.."musicaction_pb") pdt[dp.."musicaction"] = musicaction_x end,--琴箫合奏动作
	[dp.."skyfallbufflevel"] = function () require(pre.."skyfallbufflevel_pb") pdt[dp.."skyfallbufflevel"] = skyfallbufflevel_x end,--天墓禁地表
	[dp.."photoalbum"] = function () require(pre.."photoalbum_pb") pdt[dp.."photoalbum"] = photoalbum_x end,--CP约会
	[dp.."actplaydownload"] = function () require(pre.."actplaydownload_pb") pdt[dp.."actplaydownload"] = actplaydownload_x end,--边玩边下载奖励表
	[dp.."playerfenbao"] = function () require(pre.."playerfenbao_pb") pdt[dp.."playerfenbao"] = playerfenbao_x end,--包体资源分包表
	[dp.."shenshourecycleshopprice"] = function () require(pre.."shenshourecycleshopprice_pb") pdt[dp.."shenshourecycleshopprice"] = shenshourecycleshopprice_x end,--ss_神兽回收商店
	[dp.."shenshoprecycleshoprefresh"] = function () require(pre.."shenshoprecycleshoprefresh_pb") pdt[dp.."shenshoprecycleshoprefresh"] = shenshoprecycleshoprefresh_x end,--ss_神兽回收商店
	[dp.."levelcameratomonster"] = function () require(pre.."levelcameratomonster_pb") pdt[dp.."levelcameratomonster"] = levelcameratomonster_x end,--关卡怪物摄像机参数配置
	[dp.."doubleaction"] = function () require(pre.."doubleaction_pb") pdt[dp.."doubleaction"] = doubleaction_x end,--sr_双人动作表
	[dp.."ldmjtask"] = function () require(pre.."ldmjtask_pb") pdt[dp.."ldmjtask"] = ldmjtask_x end,--七宗罪任务
	[dp.."firstmeet"] = function () require(pre.."firstmeet_pb") pdt[dp.."firstmeet"] = firstmeet_x end,--初见场景-任务提示
	[dp.."notchadapter"] = function () require(pre.."notchadapter_pb") pdt[dp.."notchadapter"] = notchadapter_x end,--刘海屏适配表
	[dp.."bubble"] = function () require(pre.."bubble_pb") pdt[dp.."bubble"] = bubble_x end,--上新特惠 气泡表
	[dp.."bubble_ios"] = function () require(pre.."bubble_ios_pb") pdt[dp.."bubble_ios"] = bubble_ios_x end,--上新特惠 气泡表
	[dp.."cpprogresstext"] = function () require(pre.."cpprogresstext_pb") pdt[dp.."cpprogresstext"] = cpprogresstext_x end,--上新特惠 气泡表
	[dp.."jinlingcompeteconfig"] = function () require(pre.."jinlingcompeteconfig_pb") pdt[dp.."jinlingcompeteconfig"] = jinlingcompeteconfig_x end,--上新特惠 气泡表
	[dp.."jinlingcompeteshow"] = function () require(pre.."jinlingcompeteshow_pb") pdt[dp.."jinlingcompeteshow"] = jinlingcompeteshow_x end,--上新特惠 气泡表
	[dp.."tradecenterconfig"] = function () require(pre.."tradecenterconfig_pb") pdt[dp.."tradecenterconfig"] = tradecenterconfig_x end,--jy_交易中心
	[dp.."tradeitemtypes"] = function () require(pre.."tradeitemtypes_pb") pdt[dp.."tradeitemtypes"] = tradeitemtypes_x end,--jy_交易中心
	[dp.."defaultquality"] = function () require(pre.."defaultquality_pb") pdt[dp.."defaultquality"] = defaultquality_x end,  --gzd_高中低配设置
	[dp.."qualitysetting"] = function () require(pre.."qualitysetting_pb") pdt[dp.."qualitysetting"] = qualitysetting_x end,  --gzd_高中低配设置
	[dp.."syschannelgoodssource"]=function() require(pre.."syschannelgoodssource_pb") pdt[dp.."syschannelgoodssource"]=syschannelgoodssource_x end,--聊天 物品来源说明
	[dp.."chatframeconfig"]=function() require(pre.."chatframeconfig_pb") pdt[dp.."chatframeconfig"]=chatframeconfig_x end,--聊天表现配置
	[dp.."chatframeres"]=function() require(pre.."chatframeres_pb") pdt[dp.."chatframeres"]=chatframeres_x end,--聊天表现配置
	[dp.."auctiontabname"]=function() require(pre.."auctiontabname_pb") pdt[dp.."auctiontabname"]=auctiontabname_x end,--pm_拍卖系统
	[dp.."freepick"] = function () require(pre.."freepick_pb") pdt[dp.."freepick"] = freepick_x end, --任选礼盒表  --
	[dp.."powerinterface"] = function() require(pre.."powerinterface_pb") pdt[dp.."powerinterface"] = powerinterface_x end, --jm_战力对比配置表
	[dp.."progressgrowth"] = function() require(pre.."progressgrowth_pb") pdt[dp.."progressgrowth"] = progressgrowth_x end, --xynd_幸运扭蛋
	[dp.."charmlevel"] = function () require(pre.."charmlevel_pb") pdt[dp.."charmlevel"] = charmlevel_x end, --衣橱系统表：魅力等级
	[dp.."shenshoulefantian"] = function () require(pre.."shenshoulefantian_pb") pdt[dp.."shenshoulefantian"] = shenshoulefantian_x end, --sslft_神兽乐翻天
	[dp.."erroranti"] = function () require(pre.."erroranti_pb") pdt[dp.."erroranti"] = erroranti_x end, --问题机型
	[dp.."singleaction"] = function () require(pre.."singleaction_pb") pdt[dp.."singleaction"] = singleaction_x end, --单人动作表
	[dp.."masterlevel"] = function () require(pre.."masterlevel_pb") pdt[dp.."masterlevel"] = masterlevel_x end, --st_师徒表
	[dp.."taskbefore"] = function () require(pre.."taskbefore_pb") pdt[dp.."taskbefore"] = taskbefore_x end, --st_师徒表
	[dp.."cpaward"] = function () require(pre.."cpaward_pb") pdt[dp.."cpaward"] = cpaward_x end, --CP_特权表：恩爱奖励表
	[dp.."taskafter"] = function () require(pre.."taskafter_pb") pdt[dp.."taskafter"] = taskafter_x end, --st_师徒表
	[dp.."taskreward"] = function () require(pre.."taskreward_pb") pdt[dp.."taskreward"] = taskreward_x end, --st_师徒表

	[dp.."wardrobebox"] = function () require(pre.."wardrobebox_pb") pdt[dp.."wardrobebox"] = wardrobebox_x end, --yc_衣橱系统配置
	[dp.."newplay"] = function () require(pre.."newplay_pb") pdt[dp.."newplay"] = newplay_x end, --xw_新玩法预告界面
	[dp.."factiondonation_small"] = function () require(pre.."factiondonation_small_pb") pdt[dp.."factiondonation_small"] = factiondonation_small_x end,
	[dp.."factioninfo_small"] = function () require(pre.."factioninfo_small_pb") pdt[dp.."factioninfo_small"] = factioninfo_small_x end,
	[dp.."wardrobebox"] = function () require(pre.."wardrobebox_pb") pdt[dp.."wardrobebox"] = wardrobebox_x end, --yc_衣橱系统配置
	[dp.."newplay"] = function () require(pre.."newplay_pb") pdt[dp.."newplay"] = newplay_x end, --xw_新玩法预告界面
	[dp.."attacksuppress"] = function () require(pre.."attacksuppress_pb") pdt[dp.."attacksuppress"] = attacksuppress_x end, --gq_关卡战力压制
	[dp.."devicegrade"] = function () require(pre.."devicegrade_pb") pdt[dp.."devicegrade"] = devicegrade_x end, --gzd_高中低配设置
	[dp.."wing_update"] = function () require(pre.."wing_update_pb") pdt[dp.."wing_update"] = wing_update_x end, --yy_羽翼系统
	[dp.."wing_updateattribte"] = function () require(pre.."wing_updateattribte_pb") pdt[dp.."wing_updateattribte"] = wing_updateattribte_x end, --yy_羽翼系统
	[dp.."wing_change"] = function () require(pre.."wing_change_pb") pdt[dp.."wing_change"] = wing_change_x end, --yy_羽翼系统
	[dp.."wing_class"] = function () require(pre.."wing_class_pb") pdt[dp.."wing_class"] = wing_class_x end, --yy_羽翼系统
	[dp.."wing_soul"] = function () require(pre.."wing_soul_pb") pdt[dp.."wing_soul"] = wing_soul_x end, --yy_羽翼系统
	[dp.."group_hero_record"] = function () require(pre.."group_hero_record_pb") pdt[dp.."group_hero_record"] = group_hero_record_x end, --qyh_群英会
	[dp.."worldhonorrank"] = function () require(pre.."worldhonorrank_pb") pdt[dp.."worldhonorrank"] = worldhonorrank_x end, --jhry_江湖荣誉谱
	[dp.."honormark"] = function () require(pre.."honormark_pb") pdt[dp.."honormark"] = honormark_x end, --jhry_江湖荣誉谱
	[dp.."eventtype"] = function () require(pre.."eventtype_pb") pdt[dp.."eventtype"] = eventtype_x end, --jhry_江湖荣誉谱
	[dp.."activitytitle"] = function () require(pre.."activitytitle_pb") pdt[dp.."activitytitle"] = activitytitle_x end, --jhry_江湖荣誉谱
	[dp.."coupequestion"] = function () require(pre.."coupequestion_pb") pdt[dp.."coupequestion"] = coupequestion_x end, --cp_双人答题题库
	[dp.."cpquestionconfig"] = function () require(pre.."cpquestionconfig_pb") pdt[dp.."cpquestionconfig"] = cpquestionconfig_x end, --cp_双人答题配置
	[dp.."festivalwheeluiname"] = function () require(pre.."festivalwheeluiname_pb") pdt[dp.."festivalwheeluiname"] = festivalwheeluiname_x end, --xyzp_幸运轮盘
	[dp.."attr_order"] = function () require(pre.."attr_order_pb") pdt[dp.."attr_order"] = attr_order_x end, --羽翼属性加成的排序
	[dp.."group_hero_skilladvise"] = function () require(pre.."group_hero_skilladvise_pb") pdt[dp.."group_hero_skilladvise"] = group_hero_skilladvise_x end, -- qyh_群英会
	[dp.."agreementcontent"] = function () require(pre.."agreementcontent_pb") pdt[dp.."agreementcontent"] = agreementcontent_x end, -- yhxy_用户协议
	[dp.."uitimecontrol"] = function () require(pre.."uitimecontrol_pb") pdt[dp.."uitimecontrol"] = uitimecontrol_x end, -- ui时间控制表
	[dp.."backflow"] = function () require(pre.."backflow_pb") pdt[dp.."backflow"] = backflow_x end,--hy_活跃度表
	[dp.."combiranktype"] = function () require(pre.."combiranktype_pb") pdt[dp.."combiranktype"] = combiranktype_x end,--zh_组合战力排行榜
	[dp.."roomhuashancompetitionfinal"] = function () require(pre.."roomhuashancompetitionfinal_pb") pdt[dp.."roomhuashancompetitionfinal"] = roomhuashancompetitionfinal_x end, --华山论剑玩法
	[dp.."festival"] = function () require(pre.."festival_pb") pdt[dp.."festival"] = festival_x end,--jr_节日活动表
	[dp.."moduleswitch"] = function () require(pre.."moduleswitch_pb") pdt[dp.."moduleswitch"] = moduleswitch_x end, --mkkg_模块开关
	[dp.."festivalactivities"] = function () require(pre.."festivalactivities_pb") pdt[dp.."festivalactivities"] = festivalactivities_x end,--jr_节日活动表
	[dp.."shenshounirvana"] = function () require(pre.."shenshounirvana_pb") pdt[dp.."shenshounirvana"] = shenshounirvana_x end, --神兽涅槃表
	[dp.."worldboss"] = function () require(pre.."worldboss_pb") pdt[dp.."worldboss"] = worldboss_x end, --跨服世界boss表
	[dp.."dropteam"] = function () require(pre.."dropteam_pb") pdt[dp.."dropteam"] = dropteam_x end, --fbdl_副本掉落表
	[dp.."drop"] = function () require(pre.."drop_pb") pdt[dp.."drop"] = drop_x end, --fbdl_副本掉落表
    [dp.."roomhuashancompetitionfinal"] = function () require(pre.."roomhuashancompetitionfinal_pb") pdt[dp.."roomhuashancompetitionfinal"] = roomhuashancompetitionfinal_x end, --华山论剑玩法
    [dp.."acrosshuashancompet"] = function () require(pre.."acrosshuashancompet_pb") pdt[dp.."acrosshuashancompet"] = acrosshuashancompet_x end, --华山论剑玩法

	[dp.."fairy_class"] = function () require(pre.."fairy_class_pb") pdt[dp.."fairy_class"] = fairy_class_x end,
	[dp.."fairy_update"] = function () require(pre.."fairy_update_pb") pdt[dp.."fairy_update"] = fairy_update_x end,
	[dp.."fairy_updateattribte"] = function () require(pre.."fairy_updateattribte_pb") pdt[dp.."fairy_updateattribte"] = fairy_updateattribte_x end,
	[dp.."fairy_change"] = function () require(pre.."fairy_change_pb") pdt[dp.."fairy_change"] = fairy_change_x end,
	[dp.."fairy_soul"] = function () require(pre.."fairy_soul_pb") pdt[dp.."fairy_soul"] = fairy_soul_x end,
	[dp.."fairy_attr_order"] = function () require(pre.."fairy_attr_order_pb") pdt[dp.."fairy_attr_order"] = fairy_attr_order_x end,
	[dp.."monstergemconfig"] = function () require(pre.."monstergemconfig_pb") pdt[dp.."monstergemconfig"] = monstergemconfig_x end, --神兽宝石表
	[dp.."monstergemnum"] = function () require(pre.."monstergemnum_pb") pdt[dp.."monstergemnum"] = monstergemnum_x end, --神兽宝石表
	[dp.."territorycontest"] = function () require(pre.."territorycontest_pb") pdt[dp.."territorycontest"] = territorycontest_x end, --领地内容表

	--套装系统
	[dp.."suitequip"] = function () require(pre.."suitequip_pb") pdt[dp.."suitequip"] = suitequip_x end,--zb_系统套装表
	[dp.."suitattr"] = function () require(pre.."suitattr_pb") pdt[dp.."suitattr"] = suitattr_x end,--zb_系统套装表
	[dp.."suitforge"] = function () require(pre.."suitforge_pb") pdt[dp.."suitforge"] = suitforge_x end,--zb_系统套装表

	[dp.."monstergemre"] = function () require(pre.."monstergemre_pb") pdt[dp.."monstergemre"] = monstergemre_x end, --神兽宝石表
	[dp.."tempurl"] = function () require(pre.."tempurl_pb") pdt[dp.."tempurl"] = tempurl_x end, --预更新临时表格

	--英雄系统
	[dp.."hero_class"] = function () require(pre.."hero_class_pb") pdt[dp.."hero_class"] = hero_class_x end,--yx_英雄系统
	[dp.."hero_updateattribte"] = function () require(pre.."hero_updateattribte_pb") pdt[dp.."hero_updateattribte"] = hero_updateattribte_x end,--yx_英雄系统
	[dp.."hero_change"] = function () require(pre.."hero_change_pb") pdt[dp.."hero_change"] = hero_change_x end,--yx_英雄系统
	[dp.."hero_change_skill"] = function () require(pre.."hero_change_skill_pb") pdt[dp.."hero_change_skill"] = hero_change_skill_x end,--yx_英雄系统
	[dp.."hero_array"] = function () require(pre.."hero_array_pb") pdt[dp.."hero_array"] = hero_array_x end,--yx_英雄系统
	[dp.."hero_arraylist"] = function () require(pre.."hero_arraylist_pb") pdt[dp.."hero_arraylist"] = hero_arraylist_x end,--yx_英雄系统

	[dp.."questionlist"] = function () require(pre.."questionlist_pb") pdt[dp.."questionlist"] = questionlist_x end,--lm_联盟聚会表
	[dp.."drinkexpboost"] = function () require(pre.."drinkexpboost_pb") pdt[dp.."drinkexpboost"] = drinkexpboost_x end,--lm_联盟聚会表

	--洗练属性范围品质
	[dp.."washqua"] = function () require(pre.."washqua_pb") pdt[dp.."washqua"] = washqua_x end,--洗练属性

	[dp.."yinshe"] = function () require(pre.."yinshe_pb") pdt[dp.."yinshe"] = yinshe_x end,--czmb_充值面板   yinshe页签
	[dp.."yinshe_ios"] = function () require(pre.."yinshe_ios_pb") pdt[dp.."yinshe_ios"] = yinshe_ios_x end,--czmb_充值面板   yinshe_ios页签
	--[dp.."小写"] = function () require(pre.."小写_pb") pdt[dp.."小写"] = 小写sheet名字_x end,--最好注释下xlsx名啊，不然别人根本找不到是哪个xlsx里的
	-- 蒙面机制
	[dp.."mask"] = function () require(pre.."mask_pb") pdt[dp.."mask"] = mask_x end,-- mm_蒙面机制


	--跨服野外首领
	[dp.."acrossyewaiboss"] = function () require(pre.."acrossyewaiboss_pb") pdt[dp.."acrossyewaiboss"] = acrossyewaiboss_x end,

	--婉儿的委托
	[dp.."tasklib"] = function () require(pre.."tasklib_pb") pdt[dp.."tasklib"] = tasklib_x end,	--we_婉儿任务表
	[dp.."tasktype"] = function () require(pre.."tasktype_pb") pdt[dp.."tasktype"] = tasktype_x end,	--we_婉儿任务表
	----------------------------------------------------------捏脸-----------------------------------------------------------------------
	[dp.."facechange"] = function () require(pre.."facechange_pb") pdt[dp.."facechange"] = facechange_x end,
	[dp.."facetype"] = function () require(pre.."facetype_pb") pdt[dp.."facetype"] = facetype_x end,
	[dp.."bone"] = function () require(pre.."bone_pb") pdt[dp.."bone"] = bone_x end,
	[dp.."makeupoffset"] = function () require(pre.."makeupoffset_pb") pdt[dp.."makeupoffset"] = makeupoffset_x end,
	[dp.."faceinitvalue"] = function () require(pre.."faceinitvalue_pb") pdt[dp.."faceinitvalue"] = faceinitvalue_x end,
	[dp.."bodymatching"] = function () require(pre.."bodymatching_pb") pdt[dp.."bodymatching"] = bodymatching_x end,
	[dp.."tabmenu"] = function () require(pre.."tabmenu_pb") pdt[dp.."tabmenu"] = tabmenu_x end,
	[dp.."optionsmenu"] = function () require(pre.."optionsmenu_pb") pdt[dp.."optionsmenu"] = optionsmenu_x end,
	[dp.."hairmaterial"] = function () require(pre.."hairmaterial_pb") pdt[dp.."hairmaterial"] = hairmaterial_x end,
	[dp.."facecamera"] = function () require(pre.."facecamera_pb") pdt[dp.."facecamera"] = facecamera_x end,
	[dp.."facenet"] = function () require(pre.."facenet_pb") pdt[dp.."facenet"] = facenet_x end,
	[dp.."makeup"] = function () require(pre.."makeup_pb") pdt[dp.."makeup"] = makeup_x end,
	----------------------------------------------------------坐骑系统-------------------------------------------------------------------
	[dp.."attrInfluencedGroup"] = function () require(pre.."attrInfluencedGroup_pb") pdt[dp.."attrInfluencedGroup"] = attrinfluencedgroup_x end,--加成属性组
	[dp.."circleconfig"] = function () require(pre.."circleconfig_pb") pdt[dp.."circleconfig"] = circleconfig_x end,  --阵位
	[dp.."mountbaseconfig"] = function () require(pre.."mountbaseconfig_pb") pdt[dp.."mountbaseconfig"] = mountbaseconfig_x end, --所有神兽表
	[dp.."mountrealationconfig"] = function () require(pre.."mountrealationconfig_pb") pdt[dp.."mountrealationconfig"] = mountrealationconfig_x end, --羁绊
	[dp.."mountrealationgroupconfig"] = function () require(pre.."mountrealationgroupconfig_pb") pdt[dp.."mountrealationgroupconfig"] = mountrealationgroupconfig_x end, --羁绊组表
	[dp.."mountsoulrecommendgroup"] = function () require(pre.."mountsoulrecommendgroup_pb") pdt[dp.."mountsoulrecommendgroup"] = mountsoulrecommendgroup_x end, --阵灵
	[dp.."mountunlockedconfig"] = function () require(pre.."mountunlockedconfig_pb") pdt[dp.."mountunlockedconfig"] = mountunlockedconfig_x end, --解封
	[dp.."mountcollect"] = function () require(pre.."mountcollect_pb") pdt[dp.."mountcollect"] = mountcollect_x end, --坐骑排序表
	--坐骑阵灵
	[dp.."ridespriteconfig"] = function () require(pre.."ridespriteconfig_pb") pdt[dp.."ridespriteconfig"] = ridespriteconfig_x end, --阵灵
	[dp.."ridespritere"] = function () require(pre.."ridespritere_pb") pdt[dp.."ridespritere"] = ridespritere_x end, --推荐阵灵
	----------------------------------------------------------坐骑系统end-------------------------------------------------------------------
	[dp.."createrolecamera"] = function () require(pre.."createrolecamera_pb") pdt[dp.."createrolecamera"] = createrolecamera_x end,
	[dp.."createrolecameraglobalconfig"] = function () require(pre.."createrolecameraglobalconfig_pb") pdt[dp.."createrolecameraglobalconfig"] = createrolecameraglobalconfig_x end,

	--创角
	[dp.."createrole"] = function () require(pre.."createrole_pb") pdt[dp.."createrole"] = createrole_x end,

	--家园沐浴相关
	[dp.."npcaction"] = function () require(pre.."npcaction_pb") pdt[dp.."npcaction"] = npcaction_x  end,
	[dp.."homelandtopic"] = function () require(pre.."homelandtopic_pb") pdt[dp.."homelandtopic"] = homelandtopic_x  end,
	[dp.."topicdialogue"] = function () require(pre.."topicdialogue_pb") pdt[dp.."topicdialogue"] = topicdialogue_x  end,
	[dp.."spanpclist"] = function () require(pre.."spanpclist_pb") pdt[dp.."spanpclist"] = spanpclist_x  end,
	[dp.."homelandgive"] = function () require(pre.."homelandgive_pb") pdt[dp.."homelandgive"] = homelandgive_x  end,
	[dp.."homelandmutual"] = function () require(pre.."homelandmutual_pb") pdt[dp.."homelandmutual"] = homelandmutual_x  end,
	[dp.."judgement"] = function () require(pre.."judgement_pb") pdt[dp.."judgement"] = judgement_x  end,

	--弹簧骨骼数配置
	[dp.."dynamicbonesetting"] = function () require(pre.."dynamicbonesetting_pb") pdt[dp.."dynamicbonesetting"] = dynamicbonesetting_x end,
	[dp.."dynamicbonecoliderdata"] = function () require(pre.."dynamicbonecoliderdata_pb") pdt[dp.."dynamicbonecoliderdata"] = dynamicbonecoliderdata_x end,

	--鱼群捕鱼配置表
	[dp.."fishgroup"] = function () require(pre.."fishgroup_pb") pdt[dp.."fishgroup"] = fishgroup_x end,
	[dp.."fishconfig"] = function () require(pre.."fishconfig_pb") pdt[dp.."fishconfig"] = fishconfig_x end,
	[dp.."fishcatch"] = function () require(pre.."fishcatch_pb") pdt[dp.."fishcatch"] = fishcatch_x end,

	--任务队列配置表
	[dp.."missionqueue"] = function () require(pre.."missionqueue_pb") pdt[dp.."missionqueue"] = missionqueue_x end,

	[dp.."shenshoures"] = function () require(pre.."shenshoures_pb") pdt[dp.."shenshoures"] = shenshoures_x end,

	--月卡
	[dp.."monthcard"] = function () require(pre.."monthcard_pb") pdt[dp.."monthcard"] = monthcard_x end,
	[dp.."monthpopup"] = function () require(pre.."monthpopup_pb") pdt[dp.."monthpopup"] = monthpopup_x end,
	--运营活动弹簧
	[dp.."rewardpopup"] = function () require(pre.."rewardpopup_pb") pdt[dp.."rewardpopup"] = rewardpopup_x end,

	--秦皇相关
	[dp.."stageaward"] = function () require(pre.."stageaward_pb") pdt[dp.."stageaward"] = stageaward_x  end,

	[dp.."shenshi"] = function () require(pre.."shenshi_pb") pdt[dp.."shenshi"] = shenshi_x  end,
	[dp.."shenshougrow"] = function () require(pre.."shenshougrow_pb") pdt[dp.."shenshougrow"] = shenshougrow_x  end,
	[dp.."shenshougrowbreak"] = function () require(pre.."shenshougrowbreak_pb") pdt[dp.."shenshougrowbreak"] = shenshougrowbreak_x  end,
    [dp.."levelconfigextend"] = function () require(pre.."levelconfigextend_pb") pdt[dp.."levelconfigextend"] = levelconfigextend_x  end, --关卡配置信息拓展表

	--战宠相关
	[dp.."petbattle"] = function () require(pre.."petbattle_pb") pdt[dp.."petbattle"] = petbattle_x  end, --战宠相关配置

	--新手皇城
	[dp.."hczbshenshou"] = function () require(pre.."hczbshenshou_pb") pdt[dp.."hczbshenshou"] = hczbshenshou_x  end, --新手皇城神兽
	--新灵台幻境
	[dp.."fairyland"] = function () require(pre.."fairyland_pb") pdt[dp.."fairyland"] =fairyland_x end,  --新灵台幻境表
	[dp.."followenter"] = function () require(pre.."followenter_pb") pdt[dp.."followenter"] = followenter_x end,	--tm_队伍模型
}

local GetTable = function(t)
    local p = {}
    for k, _ in pairs(t) do
        p[#p + 1] = string.gsub(k, dp, '')
    end
    return p
end

f = io.open('lua_used_config_list', 'w+')
local WriteToFile = function(t)
    for _, v in pairs(t) do
        f:write(string.format('%s\n', v))
    end
end

WriteToFile(GetTable(configMapTable))
WriteToFile(GetTable(configInitFuncDict))
