---------chatfixmessage---------
Config "dataconfig_chatfixmessage" {
    UINT32			"Type", --unique key
    STRING			"Content",
    STRING_Array	"tag",
    STRING_Array	"indexID",
}
---------chatfixmessage---------

---------dirtyconf---------
Config "dataconfig_dirtyconf" {
    UINT32			"WordID", --unique key
    STRING			"DirtyWord",
    SINT32			"WordType":optional(),
}
---------dirtyconf---------

---------serverconfbuilding---------
Config "dataconfig_serverconfbuilding" {
    STRING			"C":optional(),
}
---------serverconfbuilding---------

---------expression---------
Config "dataconfig_expression" {
    UINT32			"ID", --unique key
    STRING			"PictureName":optional(),
    STRING			"Expressionname":optional(),
    UINT32			"canSelect":optional(),
    UINT32			"isDynamic":optional(),
    UINT32			"FrameNum":optional(),
}
---------expression---------

---------dictionary---------
Config "dataconfig_dictionary" {
    STRING			"id", --unique key
    STRING			"text":optional(),
    STRING			"value":optional(),
}
---------dictionary---------

---------tab---------
Config "dataconfig_tab" {
    UINT32			"TabID", --unique key
    STRING			"TabName":optional(),
    UINT32			"SpeakAllow":optional(),
    UINT32			"SpeakChannel":optional(),
    UINT32			"Error":optional(),
}
---------tab---------

---------chatpreandsuf---------
Config "dataconfig_chatpreandsuf" {
    UINT32			"ID", --unique key
    UINT32			"Type":optional(),
    UINT32			"NameDictID",
    UINT32			"DesDictID":optional(),
    UINT32			"OtherDictID":optional(),
}
---------chatpreandsuf---------

---------language---------
Config "dataconfig_language" {
    STRING			"id", --unique key
    STRING			"text":optional(),
}
---------language---------

---------fontmould---------
Config "dataconfig_fontmould" {
    STRING			"id", --unique key
    STRING			"FontType",
    SINT32			"Size":optional(),
    STRING			"MainColor":optional(),
    STRING			"GradientTop":optional(),
    STRING			"GradientBottom":optional(),
    SINT32			"OutlineSize":optional(),
    STRING			"OutlineColor":optional(),
    STRING			"OutlineAlpha":optional(),
    SINT32			"ShadowSize":optional(),
    STRING			"ShadowColor":optional(),
    STRING			"ShadowAlpha":optional(),
    SINT32			"HorizontalSpace":optional(),
    SINT32			"verticalSpace":optional(),
    SINT32			"FontStyleValue":optional(),
}
---------fontmould---------

---------uiadaptive---------
Config "dataconfig_uiadaptive" {
    STRING			"ModelName", --unique key
    SINT32			"NotchSize",
    SINT32			"PhoneHeight",
}
---------uiadaptive---------

---------chatconf---------
Config "dataconfig_chatconf" {
    UINT32			"ChannelID", --unique key
    STRING			"ChannelName":optional(),
    UINT32			"SpeakCoolTime",
    UINT32			"SpeakTimesMax",
    UINT32_Array	"Label",
    UINT32			"SpeakerType",
    UINT32			"RecipientType",
    UINT32			"MaxCacheCount",
    UINT32			"Time",
    UINT32			"CoolTime",
    UINT32			"SpeakChannel",
    UINT32			"OpenLevel",
}
---------chatconf---------

---------syschannelgoodssource---------
Config "dataconfig_syschannelgoodssource" {
    UINT32			"ID", --unique key
    STRING			"Source",
}
---------syschannelgoodssource---------

---------friend_intimate---------
Config "dataconfig_friend_intimate" {
    SINT32			"id", --unique key
    SINT32_Array	"intimateScope",
    SINT32			"specialPower",
}
---------friend_intimate---------

---------laceyface---------
Config "dataconfig_laceyface" {
    UINT32			"ID", --unique key
    STRING			"LaceyIcon":optional(),
    STRING			"LaceyName":optional(),
}
---------laceyface---------

---------errorcode---------
Config "dataconfig_errorcode" {
    SINT32			"id", --unique key
    STRING			"title":optional(),
    SINT32			"messageType",
    STRING			"text":optional(),
    STRING			"btn_text":optional(),
    SINT32			"exit_client":optional(),
    BOOL			"TimeBool":optional(),
    STRING			"Function":optional(),
}
---------errorcode---------

---------mail_list---------
Config "dataconfig_mail_list" {
    SINT32			"mailID", --unique key
    SINT32			"mailType",
    STRING			"mailTitle",
    STRING			"mailContent",
}
---------mail_list---------

---------wordtext---------
Config "dataconfig_wordtext" {
    UINT32			"ID", --unique key
    STRING			"WordStr":optional(),
    STRING			"WordDes":optional(),
    UINT32			"WordType":optional(),
}
---------wordtext---------

---------errorcode_lan---------
Config "dataconfig_errorcode_lan" {
    STRING			"errorLanguageId", --unique key
    STRING			"errorText":optional(),
}
---------errorcode_lan---------

---------minitab---------
Config "dataconfig_minitab" {
    UINT32			"TabID", --unique key
    STRING			"TabName":optional(),
    UINT32			"SpeakAllow":optional(),
    UINT32			"SpeakChannel":optional(),
    UINT32			"Error":optional(),
}
---------minitab---------

---------assetregion---------
Config "dataconfig_assetregion" {
    SINT32			"id", --unique key
    STRING			"Region",
}
---------assetregion---------

---------intimate_add---------
Config "dataconfig_intimate_add" {
    SINT32			"modelId", --unique key
    SINT32			"intimateAdd",
}
---------intimate_add---------

---------characterfishonconfig---------
Config "dataconfig_characterfishonconfig" {
    SINT32			"id", --unique key
    STRING			"name",
    STRING			"describe",
    SINT32			"quality",
    SINT32			"type",
    STRING			"uiName",
    STRING			"color1":optional(),
    STRING			"color2":optional(),
    STRING			"maskPath":optional(),
    STRING			"resourcePath":optional(),
}
---------characterfishonconfig---------

---------scenelevel_config---------
Config "dataconfig_scenelevel_config" {
    SINT32			"id", --unique key
    STRING			"sceneName",
    SINT32			"sceneType",
    SINT32			"SceneUIType",
    SINT32			"settlement",
    SINT32			"settleconfig",
    SINT32			"settlementNew":optional(),
    SINT32_Array	"settleparameter",
    BOOL			"reborn",
    UINT32			"teamCount",
    SINT32			"delayTime":optional(),
    SINT32			"maxTime":optional(),
    STRING			"missionName":optional(),
    STRING			"missionDes":optional(),
    STRING			"missionPic":optional(),
    STRING			"sceneconfig",
    STRING_Array	"desc",
    STRING			"map":optional(),
    STRING			"sceneBg",
    STRING			"camera":optional(),
    SINT32			"cameraTime":optional(),
    STRING_Array	"cameraSetting",
}
---------scenelevel_config---------

---------items_table---------
Config "dataconfig_items_table" {
    SINT32			"ItemId", --unique key
    SINT32			"sortId",
    STRING			"ItemName":optional(),
    STRING			"Description":optional(),
    STRING			"Icon":optional(),
    STRING			"Preview":optional(),
    SINT32			"ItemQuality":optional(),
    SINT32			"UseLevel":optional(),
    SINT32			"MaxNum":optional(),
    SINT32			"isSplit":optional(),
    SINT32			"ItemType",
    SINT32			"ItemSubType",
    SINT32			"isAutoUse":optional(),
    SINT32			"isUse":optional(),
    Struct_Array {
        SINT32			"EffectType":optional(),
        SINT32_Array	"Effect",
    }			"PropEffectParam",
    SINT32			"isSale",
    SINT32			"itemPrice":optional(),
    SINT32			"isPrecious":optional(),
    STRING			"itemGet":optional(),
    STRING			"itemJump":optional(),
}
---------items_table---------

---------main_mission---------
Config "dataconfig_main_mission" {
    UINT32			"main_level", --unique key
    UINT32			"need_exp",
    UINT32			"mission_reward":optional(),
    UINT32			"mission_reward_num":optional(),
}
---------main_mission---------

---------season_mission---------
Config "dataconfig_season_mission" {
    UINT32			"season_level", --unique key
    UINT32			"need_exp":optional(),
    UINT32			"normal_reward":optional(),
    UINT32			"normal_reward_num":optional(),
    UINT32			"vip_reward":optional(),
    UINT32			"vip_reward_num":optional(),
}
---------season_mission---------

---------season_mission_list---------
Config "dataconfig_season_mission_list" {
    UINT32			"mission_id", --unique key
    STRING			"mission_des":optional(),
    UINT32			"mission_type",
    UINT32			"mission_para",
    UINT32			"mission_reward",
    UINT32			"mission_reward_num",
}
---------season_mission_list---------

---------shop_gather---------
Config "dataconfig_shop_gather" {
    SINT32			"goodsID", --unique key
    SINT32			"itemID",
    SINT32			"goodsSort",
    SINT32			"goodsNum":optional(),
    SINT32			"goodsCurrency",
    SINT32			"goodsPrice",
    SINT32			"goodsWeight":optional(),
    SINT32			"goodsSale":optional(),
    SINT32			"limitType":optional(),
    SINT32			"limitNum":optional(),
    SINT32			"shopID":optional(),
    SINT32			"goodsBuy":optional(),
    SINT32			"MinLevel":optional(),
    SINT32			"MaxLevel":optional(),
    SINT32			"RechargeID":optional(),
    STRING			"MustList":optional(),
    UINT32			"preConditionID":optional(),
}
---------shop_gather---------

---------shop_roles---------
Config "dataconfig_shop_roles" {
    SINT32			"shopID", --unique key
    STRING			"shopName",
    SINT32			"limitRefresh",
    SINT32			"limitOpen":optional(),
    SINT32			"refreshTime":optional(),
    SINT32			"buyNum":optional(),
    SINT32_Array	"buyPrice",
    SINT32			"buyCurrency":optional(),
    SINT32			"refreshNum":optional(),
    STRING			"refreshText":optional(),
    STRING			"unitBg":optional(),
    STRING			"PicRes":optional(),
    SINT32			"tips":optional(),
}
---------shop_roles---------

---------shop_page---------
Config "dataconfig_shop_page" {
    SINT32			"PageID", --unique key
    STRING			"PageName",
    SINT32			"limitOpen":optional(),
    STRING			"PicRes":optional(),
    SINT32_Array	"AscriptionPage",
    SINT32			"SonDisplay":optional(),
}
---------shop_page---------

---------randomname1---------
Config "dataconfig_randomname1" {
}
---------randomname1---------

---------randomname3---------
Config "dataconfig_randomname3" {
}
---------randomname3---------

---------randomname2---------
Config "dataconfig_randomname2" {
}
---------randomname2---------

---------initconfig---------
Config "dataconfig_initconfig" {
}
---------initconfig---------

---------itemdrop---------
Config "dataconfig_itemdrop" {
    SINT32			"id", --unique key
    SINT32			"max_level",
    SINT32			"team_id",
    SINT32			"item_id":optional(),
    SINT32			"drop_type",
    SINT32			"radio":optional(),
    SINT32			"weight":optional(),
    STRING			"quantity":optional(),
    STRING_Array	"MustList",
}
---------itemdrop---------

---------channelconfig---------
Config "dataconfig_channelconfig" {
    STRING			"Name", --unique key
    UINT32			"AgeLimit",
    Struct_Array {
        UINT32			"AgeFrom":optional(),
        UINT32			"AgeBelow":optional(),
        UINT32			"SingleMaximum":optional(),
        UINT32			"TotalMaximum":optional(),
    }			"Parts",
}
---------channelconfig---------

---------global_config---------
Config "dataconfig_global_config" {
    UINT32			"id", --unique key
    UINT32			"global_value":optional(),
    UINT32_Array	"global_array",
    STRING			"global_str_value":optional(),
    STRING_Array	"global_str_array",
}
---------global_config---------

---------mission_reward---------
Config "dataconfig_mission_reward" {
    SINT32			"id", --unique key
    SINT32			"finish_reward",
    SINT32			"out_reward",
    STRING			"finish_medal",
    STRING			"out_medal",
}
---------mission_reward---------

---------ranklistinfo_config---------
Config "dataconfig_ranklistinfo_config" {
    UINT32			"id", --unique key
    STRING			"name",
    UINT32			"cup",
    UINT32_Array	"rankskinid",
    UINT32			"headIcon",
}
---------ranklistinfo_config---------

---------prop_config---------
Config "dataconfig_prop_config" {
    SINT32			"id", --unique key
    STRING			"illustrate":optional(),
    SINT32			"effectType",
    SINT32			"percent",
    SINT32			"time",
    STRING			"icon",
    STRING			"sceneEffect",
    STRING			"bodyEffect",
}
---------prop_config---------

---------mainroleinfo_config---------
Config "dataconfig_mainroleinfo_config" {
    UINT32			"heroid", --unique key
    STRING			"Hero_name",
    STRING			"head_path",
    UINT32			"level",
    UINT32			"locked",
}
---------mainroleinfo_config---------

---------rolelevelupinfo_config---------
Config "dataconfig_rolelevelupinfo_config" {
    UINT32			"id", --unique key
    SINT32			"attr1",
    SINT32			"attr2",
    SINT32			"attr3",
    SINT32			"attr4",
    SINT32			"attr5",
    SINT32			"exgold":optional(),
}
---------rolelevelupinfo_config---------

---------devicegrade---------
Config "dataconfig_devicegrade" {
    STRING			"DeviceName", --unique key
    UINT32			"Level",
    UINT32			"quality":optional(),
    UINT32			"lowestQuaToUseHighRes":optional(),
    UINT32			"highRes":optional(),
}
---------devicegrade---------

---------weightvalue---------
Config "dataconfig_weightvalue" {
    UINT32			"ID",
    UINT32			"Weight",
}
---------weightvalue---------

---------cpugrade---------
Config "dataconfig_cpugrade" {
    UINT32			"ID",
    UINT32			"Min",
    UINT32			"Max",
    UINT32			"Value",
}
---------cpugrade---------

---------cpucoregrade---------
Config "dataconfig_cpucoregrade" {
    UINT32			"ID",
    UINT32			"Min",
    UINT32			"Max",
    UINT32			"Value",
}
---------cpucoregrade---------

---------memorygrade---------
Config "dataconfig_memorygrade" {
    UINT32			"ID",
    UINT32			"Min",
    UINT32			"Max",
    UINT32			"Value",
}
---------memorygrade---------

---------androidgrade---------
Config "dataconfig_androidgrade" {
    UINT32			"ID",
    UINT32			"Min",
    UINT32			"Max",
    UINT32			"Value",
}
---------androidgrade---------

---------osgrade---------
Config "dataconfig_osgrade" {
    UINT32			"ID",
    UINT32			"Min",
    UINT32			"Max",
    UINT32			"Value",
}
---------osgrade---------

---------cpumodel---------
Config "dataconfig_cpumodel" {
    STRING			"Brand",
    STRING			"Model",
    UINT32			"Value",
}
---------cpumodel---------

---------player_icon---------
Config "dataconfig_player_icon" {
    UINT32			"icon_id", --unique key
    STRING			"icon_name",
}
---------player_icon---------

---------play_config---------
Config "dataconfig_play_config" {
    UINT32			"play_id", --unique key
    STRING			"play_name",
    UINT32_Array	"play_num",
    STRING			"play_bg_small":optional(),
    STRING			"play_bg_big":optional(),
    UINT32			"play_scene_id",
    UINT32			"play_maxnum",
    UINT32			"play_maxtime",
}
---------play_config---------

---------mission_config---------
Config "dataconfig_mission_config" {
    UINT32			"mission_id", --unique key
    UINT32			"mission_type",
    UINT32_Array	"reward_id",
}
---------mission_config---------

---------condition_type---------
Config "dataconfig_condition_type" {
    UINT32			"condition_id", --unique key
    UINT32			"is_overlay",
}
---------condition_type---------

---------reward_config---------
Config "dataconfig_reward_config" {
    UINT32			"reward_id", --unique key
    UINT32			"condition_id",
    UINT32_Array	"reward_para",
    UINT32			"reward_type",
    SINT32_Array	"reward_num",
    STRING			"reward_text":optional(),
    STRING			"medal_id":optional(),
    UINT32			"condition_priority":optional(),
}
---------reward_config---------

---------rank_season---------
Config "dataconfig_rank_season" {
    UINT32			"season_id", --unique key
    STRING			"season_name",
    STRING			"start_time",
    STRING			"end_time",
}
---------rank_season---------

---------rank_config---------
Config "dataconfig_rank_config" {
    UINT32			"condition_id", --unique key
    UINT32			"rank_big",
    UINT32			"rank_small",
    UINT32			"rank_star",
    UINT32			"need_score",
    UINT32			"rank_reset",
    STRING			"rank_name",
}
---------rank_config---------

---------season_reward---------
Config "dataconfig_season_reward" {
    UINT32			"reward_id", --unique key
    UINT32			"need_rank",
    UINT32			"drop_id",
}
---------season_reward---------

---------home_build---------
Config "dataconfig_home_build" {
    SINT32			"id", --unique key
    STRING			"para",
    STRING			"txt":optional(),
    STRING			"icon",
    SINT32			"quality",
    SINT32			"museum_value":optional(),
    SINT32			"homelevel_need",
    SINT32			"unlock_itemid":optional(),
    SINT32			"unlock_expend":optional(),
}
---------home_build---------

---------home_level---------
Config "dataconfig_home_level" {
    SINT32			"id", --unique key
    STRING			"txt":optional(),
    STRING			"icon",
    SINT32			"quality",
    SINT32			"museum_value":optional(),
    SINT32			"visitor_max",
    SINT32			"unlock_room",
    SINT32			"unlock_itemid":optional(),
    SINT32			"unlock_expend":optional(),
    STRING			"model":optional(),
    STRING			"default_texture":optional(),
    STRING			"default_color":optional(),
    STRING			"shadow":optional(),
}
---------home_level---------

---------furniture---------
Config "dataconfig_furniture" {
    SINT32			"id", --unique key
    STRING			"furniture_name":optional(),
    SINT32_Array	"occupy_space",
    SINT32			"furniture_type",
    SINT32			"theme":optional(),
    SINT32			"quality",
    SINT32			"museum_value":optional(),
    SINT32			"direct_purchase":optional(),
    SINT32			"unlock_itemid":optional(),
    SINT32			"unlock_expend":optional(),
    STRING			"icon":optional(),
    STRING			"model":optional(),
    STRING			"default_texture":optional(),
    SINT32			"color_tape",
    STRING			"default_color":optional(),
    STRING			"shadow":optional(),
}
---------furniture---------

---------signtype_config---------
Config "dataconfig_signtype_config" {
    UINT32			"SignType", --unique key
    STRING			"SignTypeExplain",
    UINT32			"SignCap",
    UINT32			"SignLife",
    UINT32			"SignArea",
    UINT32			"SignDis",
}
---------signtype_config---------

---------signlevel_config---------
Config "dataconfig_signlevel_config" {
    UINT32			"SignTypeId", --unique key
    UINT32			"SignType",
    UINT32			"QualityType",
    STRING			"QualityExplain",
    UINT32			"SignScore",
    STRING_Array	"SignModel",
}
---------signlevel_config---------

---------sub_bag_table---------
Config "dataconfig_sub_bag_table" {
    UINT32			"sub_table_id", --unique key
    STRING			"sub_table_name",
    UINT32_Array	"subtype_id",
    STRING_Array	"sort_type",
}
---------sub_bag_table---------

---------item_subtype---------
Config "dataconfig_item_subtype" {
    UINT32			"subtype_id", --unique key
    STRING			"subtype_name",
    STRING_Array	"sort_type",
}
---------item_subtype---------

---------main_bag_table---------
Config "dataconfig_main_bag_table" {
    UINT32			"main_table_id", --unique key
    STRING			"main_table_name",
    UINT32_Array	"sub_table_id",
    STRING_Array	"sort_type",
}
---------main_bag_table---------

---------expression_config---------
Config "dataconfig_expression_config" {
    UINT32			"expression_id", --unique key
    UINT32			"rank_big",
    STRING			"resource_name",
    UINT32			"item_id",
    UINT32			"is_unlock",
    UINT32			"unlock_type",
    UINT32			"unlock_price",
}
---------expression_config---------

---------qualitysetting---------
Config "dataconfig_qualitysetting" {
    UINT32			"quality", --unique key
    UINT32			"shadow",
    UINT32			"WarpetShadow",
    UINT32_Array	"shadowDis",
    UINT32			"audio_num",
    UINT32			"effect_num",
    UINT32			"selfBehitEffect_num",
    UINT32			"behitEffect_num",
    UINT32			"selfEffect",
    UINT32			"theOthersEffect",
    UINT32			"selfEffectInMultiplayer",
    UINT32			"theOthersEffectInMultiplayer",
    UINT32			"selfOutWardEffect",
    UINT32			"theOthersOutWardEffect",
    UINT32			"effect_level",
    UINT32			"skille_ffect_level",
    UINT32			"sceneEffect_hide",
    UINT32			"resetDataVisible",
    UINT32			"sceneObj_hide",
    UINT32			"disturEffectsLevel",
    UINT32			"cameraCuttingDistance",
    UINT32			"water",
    UINT32			"showModelNum",
    UINT32			"showModelMax",
    UINT32			"isHighlightNew",
    UINT32			"isHighlightQinHuang",
    UINT32			"isHighlightUnderPalace",
    UINT32			"antiAliasing",
    UINT32			"dialogueQuaNPC",
    UINT32			"attackBossEffect",
    UINT32			"resolution",
    UINT32			"unityQuality",
    UINT32			"riderHighEffect",
    UINT32			"RadialBlur",
    UINT32			"Dissolve",
    UINT32_Array	"IsDynamicBone",
    UINT32			"bloom":optional(),
    UINT32			"Dist",
    UINT32			"farDist",
    UINT32			"mostFarDist",
    UINT32			"shaderLod",
    UINT32			"ModelLod",
    UINT32			"otherModelLod",
    UINT32			"mainHallLod",
    UINT32			"otherHallLod",
    UINT32			"DepthOfField",
    UINT32			"AsyncCount",
    UINT32			"footEffectCount",
}
---------qualitysetting---------

---------defaultquality---------
Config "dataconfig_defaultquality" {
    UINT32			"Level", --unique key
    UINT32			"quality",
    UINT32			"quality_min",
    UINT32			"quality_max",
}
---------defaultquality---------

