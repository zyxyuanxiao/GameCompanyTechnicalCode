Config "dataconfig_achieve" {
    UINT32 "ID", --multi key
    UINT32 "DisplayJudge",
    STRING "AwardDesc",
    UINT32 "Step", --multi key
    STRING "Name",
    STRING "Desc",
    STRING "MoreDesc",
    UINT32 "Group", --multi key
    UINT32 "Point",
    UINT32 "RewardItemID",
    UINT32 "Gold",
    UINT32 "TitleID",
    UINT32 "EventNum",
    UINT32 "CheckType",
    UINT32[3] "Channel";
}



Config "dataconfig_achievegroup" {
    UINT32 "Group",
    STRING "GroupName",
}



Config "dataconfig_achievelevel" {
    UINT32 "PointLevel", --unique key
    STRING "PointAwardicon",
    UINT32 "Point",
    STRING "AwardName",
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNumber",
    }[4] "ElementNumber",
}



Config "dataconfig_acrosshuashancompet" {
    UINT32 "State", --unique key
    UINT32 "ActStateType",
    UINT32 "Turn",
    UINT32 "GroupFightTurn",
    UINT32 "Step",
    UINT32 "MaxTeamNum",
    UINT32 "GroupNum",
    STRING "ScenceTips",
    STRING "FightTips",
    STRING "ScenceSecTips",
}



Config "dataconfig_acrossyewaiboss" {
    UINT32 "BossID", --multi key
    UINT32 "BossLev", --multi key
    UINT32 "MapID",
    SINT32[3] "Coordination";
    UINT32[3] "AttackBonusItem";
    UINT32 "LastBonusItem",
    UINT32 "UnionWinBonusGroup",
    UINT32 "GangWinBonusGroup",
    UINT32 "JoinBonusGroup",
    UINT32[3] "ScaleUI";
    SINT32[3] "RotationUI";
    STRING "Icon",
    UINT32 "RewardPreview",
    UINT32 "ModeID",
}



Config "dataconfig_act_collect" {
    UINT32 "CarnivalID",
    STRING "Title",
    UINT32 "Type",
    UINT32[2] "ChildID";
    STRING "Label",
    STRING "StartTime",
    STRING "EndTime",
    STRING "DisplayStart",
    STRING "ForceClose",
    STRING "BannerPic",
    STRING "ActDesc",
}



Config "dataconfig_actfightaward" {
    UINT32 "ID",
    UINT32 "ActID", --multi key
    UINT32 "AwardType", --multi key
    UINT32 "Number",
    STRING "Des",
}



Config "dataconfig_activitybonus" {
}



Config "dataconfig_activityconfig" {
    UINT32 "id",
    STRING "name",
    STRING "icon",
    UINT32 "Type",
    UINT32[1] "Value";
    UINT32 "NeedTimes",
    UINT32 "ActiveValue",
    UINT32 "OpenLevel",
    UINT32 "Button",
    UINT32 "TimeType",
    STRING "Word",
    Struct {
        STRING "OpenTime",
        STRING "CloseTime",
    }[5] "Time",
    UINT32 "Clink",
}



Config "dataconfig_activitydst" {
    SINT32 "id", --multi key
    UINT32 "SubID",
    UINT32 "LevelId",
    Struct {
        STRING "title",
        STRING[3] "content";
    }[8] "dstInfos",
}



Config "dataconfig_activityfenbao" {
    SINT32 "ID",
    SINT32 "IDShort", --unique key
    SINT32 "Pakage",
}



Config "dataconfig_activitypreview" {
    UINT32 "ID",
    STRING "TabName",
    STRING "Title",
    STRING "Content",
    UINT32 "UIType",
    UINT32 "Link",
    UINT32 "Url",
    UINT32[1] "Func";
    UINT32[1] "UrlNum";
    STRING "LinkName",
    UINT32 "ActivityID",
    UINT32 "ActivityLevel",
    UINT32 "LevelLimit",
    UINT32 "openDay",
    UINT32 "openDayLimit",
    UINT32 "type",
    STRING "startTime",
    STRING "endTime",
    UINT32[1] "week";
    Struct {
        STRING "dayStartTime",
        STRING "dayEndTime",
    }[10] "Clock",
}



Config "dataconfig_activityreward" {
    UINT32 "id", --unique key
    STRING "name",
    UINT32 "ActNeed",
    UINT32 "MaxNum",
    UINT32 "RewardID",
    UINT32 "PreviewID",
}



Config "dataconfig_activitytime" {
    UINT32 "ActivityID", --unique key
    STRING "ActivityName",
    UINT32 "GuideID",
    UINT32 "GuideNewID",
    UINT32 "ActivityLevel",
    UINT32 "LevelLimit",
    UINT32 "TrOpenDay",
    UINT32 "openDay",
    UINT32 "openDayClose",
    UINT32 "ActivityType", --multi key
    STRING "ActivityIcon",
    UINT32[3] "PlayType";
    STRING[4] "Picture";
    UINT32[10] "Quote";
    UINT32 "publishUITab",
    UINT32 "JoinIn",
    STRING "SpecialShow",
    UINT32 "ShowPara",
    STRING "RunningShow",
    UINT32 "Notice",
    STRING "Forecast",
    UINT32 "ForecastPara",
    UINT32 "StopType",
    UINT32[3] "ActiveType";
    UINT32[7] "AwardType";
    UINT32[7] "StarLevel";
    UINT32 "ForecastLevel",
    UINT32 "type",
    STRING "startTime",
    STRING "endTime",
    UINT32[4] "week";
    Struct {
        STRING "dayStartTime",
        STRING "dayEndTime",
    }[10] "Clock",
    UINT32 "maxNum",
    UINT32 "ActiveNum",
    UINT32 "Active",
    STRING "Txt",
    UINT32 "RewardPreview",
    UINT32 "RecycleNum",
    UINT32 "RewardPerfect",
    UINT32 "RewardCommon",
    UINT32[30] "LevelID";
    UINT32 "Join",
    UINT32 "Help",
    UINT32 "MessageID",
    UINT32[2] "NpcID";
    Struct {
        UINT32 "Num",
        UINT32[1] "LevelList";
    }[3] "LevelConcat",
    UINT32 "TeamType",
    UINT32 "SpecialList",
    STRING "ActivityPic",
    UINT32 "EnterType",
    UINT32 "MemoID",
    STRING "LimitActivityPic",
    STRING "LimitActivityName",
    UINT32 "FirstReward",
    UINT32 "CaptainReward",
    UINT32 "PlayerNum",
    UINT32 "OverSever",
    UINT32 "FullTimeShow",
    UINT32 "IsOpenMicrophone",
    SINT32[1] "MicrophonePosition";
}



Config "dataconfig_activitytitle" {
    UINT32 "ActivityID", --unique key
    STRING "SpriteName",
}



Config "dataconfig_actorcamp" {
    UINT32 "CampID",
    Struct {
        UINT32 "nParam",
    }[9] "paramList",
}



Config "dataconfig_actplaydownload" {
    UINT32 "id",
    UINT32 "condition",
    Struct {
        UINT32 "item_id",
        UINT32 "num",
    }[2] "AwardList",
}



Config "dataconfig_agreementcontent" {
    STRING "ChannelID", --unique key
    STRING "Content",
}



Config "dataconfig_angeradd" {
    UINT32 "WeaponType", --unique key
    UINT32 "SkillOneAddValue",
    STRING "SkillOneAddValueDesc",
    UINT32 "SkillTwoAddValue",
    STRING "SkillTwoAddValueDesc",
    UINT32 "SkillThreeAddValue",
    STRING "SkillThreeAddValueDesc",
}



Config "dataconfig_angerconfig" {
    UINT32 "WeaponType", --unique key
    UINT32 "UpperLimit",
    UINT32 "Consume",
    UINT32 "AttackNum",
    UINT32 "CritNum",
    UINT32 "HitNum",
    UINT32 "IncreaseTime",
    UINT32 "IncreaseNum",
    UINT32[2] "AngerEffect";
    UINT32[3] "GuideSkill";
}



Config "dataconfig_angertips" {
    UINT32 "BuffID", --unique key
    STRING "BuffDesc",
    STRING "BuffTips",
}



Config "dataconfig_arrestwanted" {
    UINT32 "ID",
    UINT32 "Time",
    UINT32 "MoneyType",
    UINT32 "NumBer",
}



Config "dataconfig_artifactactivate" {
    UINT32 "ArtifactID",
    SINT32 "Activate",
    STRING "ArtifactName",
    UINT32 "ArtifactType",
    STRING "lowContortEffectName",
    STRING "szResFile",
    SINT32[16] "UiModelData";
    SINT32[16] "UiModelData2";
    STRING "Atlas",
    STRING "Icon",
    UINT32 "CostItemID",
    UINT32 "CostItemNum",
    UINT32 "CostItemFragmentID",
    Struct {
        UINT32 "keyID",
        UINT32 "Value",
    }[6] "ActiProperty",
    Struct {
        UINT32 "keyID",
        UINT32 "Value",
    }[1] "ActifactProperty",
    Struct {
        UINT32 "EffectID",
        UINT32 "EffectValue",
    }[2] "AttributeAdditional",
    UINT32 "ActiFightNum",
    UINT32 "ActivatebuffID",
    UINT32 "ActiBuffFightNum",
}



Config "dataconfig_artifactbase" {
    UINT32 "ArtifactID",
    STRING "ArtifactName",
    UINT32 "ArtifactType",
    UINT32 "ArtifactLock",
    UINT32 "Redenvelopesid",
    UINT32 "Artifactrank",
}



Config "dataconfig_artifactgem" {
    UINT32 "GemID",
    UINT32 "ArtifactID",
    UINT32[4] "GemNumber";
    UINT32 "GemLevel",
    UINT32 "Quality",
    UINT32 "ComposeLevel",
    Struct {
        UINT32 "key",
        UINT32 "value",
    }[3] "AddAttrInfo",
    UINT32 "EffectID",
    UINT32 "EffectValue",
    UINT32 "GradeActibuffID",
    UINT32 "MaxNumber",
    UINT32 "NextLevelID",
    UINT32 "Fighting",
    UINT32 "Identification",
    UINT32 "RMBAuctionDirectPrice",
}



Config "dataconfig_artifactgrade" {
    UINT32 "ArtifactID",
    UINT32 "Grade",
    UINT32 "GradeLevel",
    UINT32 "CostItemID",
    UINT32 "CostItemNum",
    Struct {
        UINT32 "keyID",
        UINT32 "Value",
    }[6] "GradeProperty",
    UINT32 "GradeActibuffID",
    UINT32 "ActiGradeFightNum",
}



Config "dataconfig_artigemlock" {
    UINT32 "ArtifactID",
    UINT32 "LevelLock",
    UINT32 "GemNumber",
}



Config "dataconfig_artigemrec" {
    UINT32 "ArtifactID",
    UINT32 "GemNumber",
    UINT32 "RecLevel_min",
    UINT32 "RecLevel_max",
    Struct {
        UINT32 "LingShiID",
        UINT32[1] "LingshiList";
    }[3] "RecommendList",
}



Config "dataconfig_asidesdata" {
    SINT32 "ID", --unique key
    SINT32 "type",
    SINT32 "dialogueID",
    STRING[8] "textContents";
    STRING "textShowSpeed",
    SINT32 "bgTexture",
    SINT32 "operatorCode",
    SINT32[1] "operatorParas";
}



Config "dataconfig_attacksuppress" {
    UINT32 "suppressId", --multi key
    UINT32 "fightRatio",
    UINT32[1] "selfPlayerBuff";
    UINT32[1] "selfShenShouBuff";
    UINT32[1] "enemyPlayerBuff";
    UINT32[1] "enemyShenShouBuff";
}



Config "dataconfig_attr_order" {
    UINT32 "AttrID", --unique key
    UINT32 "Init",
    UINT32 "Order",
}



Config "dataconfig_attrbasevalue" {
    UINT32 "Grade",
    UINT32 "QuaID",
    Struct {
        UINT32 "AttrID",
        UINT32 "ValueMin",
        UINT32 "ValueMax",
    }[24] "AttrRangeList",
}



Config "dataconfig_attrfighting" {
    UINT32 "LevelRange",
    UINT32 "EquipTypeID",
    Struct {
        UINT32 "RangeMin",
        UINT32 "RangeMax",
        UINT32 "Fighting",
    }[6] "RandRangeList",
}



Config "dataconfig_attrgroup" {
    UINT32 "AttrGroupID", --unique key
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrRangeID",
    }[2] "BaseAttrList",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrRangeID",
    }[12] "GeneAttrOneList",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrRangeID",
    }[2] "SuitAttrList",
    Struct {
        UINT32 "BuffID",
        UINT32 "SkillID",
    }[3] "SuitBuffList",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrRangeID",
    }[12] "GeneAttrTwoList",
    Struct {
        UINT32 "AttrPercID",
        UINT32 "AttrRangeID",
    }[3] " SpecialAttrPercList",
    Struct {
        UINT32 "SkillID",
    }[24] "AttrSkillList",
}



Config "dataconfig_attridratio" {
    UINT32 "AttrID",
    Struct {
        UINT32 "PartID",
        UINT32 "RatioValue",
    }[10] "PartRangeList",
}



Config "dataconfig_attrinfluencedgroup" {
    UINT32 "AttrGroup",
    UINT32 "AttrList",
}



Config "dataconfig_attrnamedesc" {
    UINT32 "keyID",
    STRING "type",
    STRING "nkeyDesc",
    STRING "DetailDesc",
    UINT32 "EqualType",
    UINT32 "AttrShowType",
    UINT32 "AttrShowSeq",
    UINT32[7] "AttrShowRoleType";
}



Config "dataconfig_attrrange" {
    UINT32 "AttrRangeID", --unique key
    UINT32 "AttrMin",
    UINT32 "AttrMax",
    UINT32 "AttrRangeWeightID",
}



Config "dataconfig_auctionconfig" {
    UINT32 "activityID", --unique key
    STRING "Name",
    UINT32 "prepareTime",
    UINT32 "lastTime",
}



Config "dataconfig_auctionpre" {
    UINT32 "ActiId",
    STRING "AcName",
    UINT32 "ActimeId",
    UINT32 "AucId",
    UINT32 "AuctionPre",
    STRING "Acdisc",
    STRING "AucStartTime",
}



Config "dataconfig_auctiontabname" {
    UINT32 "ID",
    STRING "Name",
    UINT32 "Order",
    UINT32[10] "ItemType";
}



Config "dataconfig_audioclipconfig" {
    STRING "name", --unique key
    STRING "path",
    STRING "desc",
}



Config "dataconfig_authoritylist" {
    UINT32 "id",
    STRING "name",
    UINT32 "type",
    UINT32 "show",
}



Config "dataconfig_awardinfo" {
    UINT32 "ID",
    UINT32 "MinRank",
    UINT32 "MaxRank",
    STRING "Icon",
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNum",
    }[2] "RewardList",
    UINT32 "RewardPreview",
    UINT32 "RewarditemID",
}



Config "dataconfig_backflow" {
    UINT32 "id",
    STRING "name",
    UINT32 "RewardID",
    UINT32 "PreviewID",
    UINT32 "TipsID",
    UINT32 "Openday",
    UINT32 "IndexesID",
    UINT32 "Substitute",
}



Config "dataconfig_battleassist" {
    UINT32 "ID",
    STRING "IconName",
}



Config "dataconfig_battlechart" {
    UINT32 "ID", --unique key
    UINT32 "MinRank",
    UINT32 "MaxRank",
    UINT32 "Openlevel",
    UINT32 "OpenDay",
    UINT32 "CostDay",
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNum",
    }[5] "RewardList",
}



Config "dataconfig_beastcollect" {
    UINT32 "ID",
    UINT32 "Type",
    STRING "name",
    Struct {
        UINT32 "condition",
        UINT32 "Nparam",
        UINT32 "Mparam",
    }[1] "Limit",
    UINT32 "condition",
    UINT32 "Nparam",
    UINT32 "Mparam",
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNum",
    }[1] "Award",
    STRING "Atlas",
    STRING "Icon",
    STRING "Des",
}



Config "dataconfig_beastextrastrengthenadd" {
    UINT32 "ID", --multi key
    UINT32 "NeedLevel",
    UINT32 "Weapon", --multi key
    Struct {
        UINT32 "nkey",
        UINT32 "value",
    }[5] "ExtraAttrlist",
    UINT32 "Fighting",
}



Config "dataconfig_beastskilleffect" {
    UINT32 "ID",
    UINT32 "level",
    UINT32 "Maxlevel",
    STRING "name",
    UINT32 "condition",
    UINT32 "Nparam",
    UINT32[3] "Mparam";
    UINT32 "Value",
    STRING "Atlas",
    STRING "buffIcon",
    STRING "buffDesc",
    STRING "levelDesc",
}



Config "dataconfig_beaststrengthenbreak" {
    UINT32 "ID", --multi key
    UINT32 "StrengthenLimit",
    Struct {
        UINT32 "Item",
        UINT32 "Num",
    }[1] "BreakCost",
}



Config "dataconfig_beaststrengthenconfig" {
    UINT32 "ID", --multi key
    UINT32 "StrengthenLevel", --multi key
    UINT32 "StrengthenStep",
    STRING "StrenthenIcon",
    Struct {
        UINT32 "AttrID",
        UINT32 "value",
    }[7] "BaseProperty",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[1] "StrengCost",
    UINT32 "Cost",
    UINT32 "Fighting",
}



Config "dataconfig_beaststrengthenlevellimits" {
    UINT32 "RoleMinLevel",
    UINT32 "StrengthenMaxLevel",
}



Config "dataconfig_beaststrengthenuplevel" {
    UINT32 "StarLevel",
    UINT32 "BreakLevel",
}



Config "dataconfig_blacksmithconfig" {
    UINT32 "BlacksmithLevel",
    UINT32 "ExpForGrade",
    UINT32 "WhiteWeight",
    UINT32 "GreenWeight",
    UINT32 "BlueWeight",
    UINT32 "PurpleWeight",
    UINT32 "OrangeWeight",
}



Config "dataconfig_blacksmithlist" {
    UINT32 "EquipStage",
    UINT32 "BuildingID",
    UINT32 "Level",
    UINT32 "EquipConType",
    UINT32 "EquipID",
    STRING "EquipName",
    UINT32 "EquipLevel",
    UINT32 "ID",
    UINT32 "Num",
    UINT32 "BlueCost",
    UINT32 "GreenRate",
    UINT32 "BlueRate",
    UINT32 "PurpleRate",
    UINT32 "OrangeRate",
}



Config "dataconfig_blacksmithreduce" {
    UINT32 "BlacksmithLevel", --unique key
    UINT32 "CostReduce",
}



Config "dataconfig_bless" {
    UINT32 "ID",
    UINT32 "time",
    UINT32 "Score",
    UINT32 "DropID",
    Struct {
        SINT32 "Down",
        UINT32 "Up",
        UINT32 "Weight",
    }[4] "DropList",
}



Config "dataconfig_bodymatching" {
    UINT32 "menuID", --unique key
    UINT32[6] "hairKind";
    STRING[1] "hairIcon";
    UINT32[1] "faceID";
    UINT32[1] "clothID";
}



Config "dataconfig_boiteaward" {
    UINT32 "ID",
    Struct {
        UINT32 "ItemID",
        UINT32 "Num",
    }[2] "Award",
    UINT32 "DropID",
    UINT32 "RewardPreID",
}



Config "dataconfig_boitetask" {
    UINT32 "ID", --unique key
    STRING "Name",
    STRING "Icon",
    UINT32 "Rank",
    UINT32 "NextTask",
    UINT32 "HelpMoney",
    UINT32 "PerfectMoney",
    UINT32 "CanHelp",
    UINT32 "Weight",
    Struct {
        UINT32 "ActionType",
        SINT32 "Param",
        SINT32 "Param2",
        SINT32[7] "TargetInfo";
        UINT32 "NeedNum",
    }[1] "CompleteLimit",
    UINT32 "HelpAward",
    UINT32 "DropTeam",
    UINT32 "RewardPreID",
}



Config "dataconfig_bone" {
    UINT32 "BoneID",
    UINT32 "ModelId", --unique key
    STRING "BoneName",
    SINT32[3] "PosMax";
    SINT32[3] "PosMin";
    SINT32[3] "RotaMax";
    SINT32[3] "RotaMin";
    SINT32[3] "ScaleMax";
    SINT32[3] "ScaleMin";
}



Config "dataconfig_bubble" {
    UINT32 "ID", --unique key
    UINT32 "IntervalTime",
    Struct {
        STRING "Text",
    }[6] "Dialogue",
}



Config "dataconfig_bubble_ios" {
    UINT32 "ID",
    UINT32 "IntervalTime",
    Struct {
        STRING "Text",
    }[6] "Dialogue",
}



Config "dataconfig_buffconfig" {
    UINT32 "nBuffID", --unique key
    STRING "Name",
    STRING "Desc",
    STRING "BuffIcon",
    UINT32 "buffRateType",
    UINT32 "buffRate",
    UINT32 "nStepbuff",
    UINT32 "buffArea",
    UINT32 "nLastTime",
    UINT32 "nStepTime",
    UINT32 "TimeShow",
    UINT32 "nStatePriID",
    UINT32 "uStateID",
    UINT32 "uSkillID",
    UINT32 "clearWhenDie",
    UINT32 "clearWhenPass",
    UINT32 "offLineHandle",
    UINT32[2] "clearWhenCondition";
    UINT32 "buffNature",
    UINT32 "mutexFlag",
    UINT32 "mutexID",
    UINT32 "mutexLevel",
    UINT32 "mutexType",
    UINT32 "num",
    UINT32[1] "layerBuff";
    UINT32 "IDSet",
    UINT32 "buffEffectID",
    UINT32 "hangPoint",
    STRING "buffEffect",
    UINT32 "uiEffect",
    STRING "buffMaterials",
    STRING "buffEndEffect",
    UINT32 "EndEffectTime",
    UINT32 "ChainID",
    UINT32[6] "buffTriggerID";
    UINT32 "nBuffLogicID",
    Struct {
        UINT32 "key",
        SINT32 "value",
        UINT32 "stepid",
    }[6] "paramList",
    UINT32 "ndotLogicID",
    UINT32 "dotTime",
    Struct {
        UINT32 "paramkey",
        SINT32 "paramvalue",
        UINT32 "stepid",
    }[4] "dotEffect",
    UINT32 "resid",
    UINT32 "buffType",
    UINT32[1] "rejectBuffType";
    UINT32 "ShowPosition",
    UINT32 "modelScale",
    UINT32[1] "skillPerfIDList";
}



Config "dataconfig_buffdata" {
    UINT32 "id",
    STRING "name",
    UINT32 "buffid",
    STRING "bufficon",
    UINT32 "maxcount",
    UINT32 "type",
    UINT32 "isActive",
    UINT32 "CD",
    UINT32 "enterCD",
    UINT32 "condition",
    UINT32 "Nparam",
    UINT32[1] "Mparam";
    UINT32 "buffEffect",
    STRING "lockdec",
    UINT32 "buffdec", --multi key
    UINT32 "idtype",
    UINT32 "level",
    STRING[1] "effectName";
    STRING "levelUpTips",
    UINT32[1] "tipsParam";
}



Config "dataconfig_buffindex" {
    SINT32 "ID",
    SINT32 "buffid",
    STRING "infoshow",
    STRING "infolost",
    STRING "infoadd",
}



Config "dataconfig_buffpicture" {
    SINT32 "KillBuff",
    SINT32 "KillPicture",
}



Config "dataconfig_bufftriggercfg" {
    UINT32 "ID", --unique key
    UINT32 "lv",
    UINT32 "triggerCondition",
    SINT32[2] "triggerParams1";
    SINT32[20] "triggerParams2";
    UINT32 "probability",
    UINT32 "cdTime",
    UINT32 "resetCDWhenRevive",
    UINT32 "triggerLogicID",
    SINT32[1] "logicParams1";
    SINT32[7] "logicParams2";
    SINT32[2] "logicParams3";
    UINT32[1] "triggerTarget";
    SINT32[1] "targetParams1";
    SINT32[1] "targetParams2";
    SINT32[1] "targetParams3";
    SINT32[1] "targetParams4";
    UINT32 "delSelf",
    UINT32 "triggerPerform",
}



Config "dataconfig_buytimes" {
    UINT32 "Num", --unique key
    UINT32 "Cost",
    UINT32 "Times",
}



Config "dataconfig_bwexchangebutton" {
    UINT32 "ID",
    STRING "Name",
    UINT32 "Group",
    UINT32 "Order",
}



Config "dataconfig_bwexchangetype" {
    UINT32 "ID", --unique key
    UINT32[7] "GroupList";
}



Config "dataconfig_cameraconfig" {
    UINT32 "id", --unique key
    UINT32 "fov",
    UINT32 "battleToDistance",
    SINT32 "pitchAngleSD",
    SINT32 "yawAngleSD",
    UINT32 "distanceSD",
    UINT32 "isUseConfig",
    SINT32 "pitchAngleTD",
    SINT32 "yawAngleTD",
    UINT32 "distanceTD",
    STRING "postProcessProfile",
    SINT32 "rePitchAngle",
    UINT32 "autoMoveDelay",
    UINT32 "autoMoveDurationTime",
    UINT32 "autoMoveSpeed",
    SINT32 "autoMoveAngle",
    UINT32 "autoMoveHeartbeat",
    UINT32 "isTargetFocus",
    UINT32 "targetFocusType",
    UINT32 "horizontalFocusAngle",
    SINT32 "minPitchAngle",
    SINT32 "maxPitchAngle",
    UINT32 "fixationPointDeviation",
    UINT32 "focusSpeed",
}



Config "dataconfig_challenge" {
    UINT32 "NumberTime", --unique key
    UINT32 "Time",
    UINT32 "TimeUP",
    UINT32 "MoneyType",
    UINT32 "NumBer",
    UINT32 "Reward",
    UINT32 "Num",
    UINT32 "MoneyMax",
    UINT32 "Decrease",
}



Config "dataconfig_charmlevel" {
    UINT32 "CharmLevel", --unique key
    UINT32 "CharmNum",
    Struct {
        UINT32 "ReWardItem",
        UINT32 "ReWardNum",
    }[4] "Reward",
}



Config "dataconfig_chatcolor" {
    SINT32 "id",
    STRING "smallcolor",
    STRING "bigcolor",
}



Config "dataconfig_chatconf" {
    UINT32 "ChannelID", --unique key
    STRING "ChannelName",
    UINT32 "SpeakCoolTime",
    UINT32 "SpeakTimesMax",
    UINT32[2] "Label";
    UINT32 "SpeakerType",
    UINT32 "RecipientType",
    UINT32 "MaxCacheCount",
    UINT32 "Time",
    UINT32 "CoolTime",
    UINT32 "SpeakChannel",
    UINT32 "OpenLevel",
}



Config "dataconfig_chatfixmessage" {
    UINT32 "Type", --unique key
    STRING "Content",
    STRING[1] "tag";
    STRING[1] "indexID";
}



Config "dataconfig_chatframeconfig" {
    UINT32 "frameID", --unique key
    STRING "frameName",
    UINT32 "frameList",
    UINT32 "frameResID",
    UINT32 "bubbleResID",
    UINT32 "limitTime",
    UINT32 "showFrame",
    STRING "obtainFrame",
}



Config "dataconfig_chatframeres" {
    UINT32 "resID", --unique key
    UINT32 "frameType",
    STRING "resRoute",
}



Config "dataconfig_chatlink" {
    SINT32 "id", --unique key
    STRING "info",
    STRING "linktext",
    SINT32 "type",
}



Config "dataconfig_chatpreandsuf" {
    UINT32 "ID",
    UINT32 "Type",
    UINT32 "NameDictID",
    UINT32 "DesDictID",
    UINT32 "OtherDictID",
}



Config "dataconfig_chatrouteconfig" {
    STRING "routeID",
}



Config "dataconfig_chinaarea" {
    UINT32 "ID", --unique key
    STRING "AreaName",
}



Config "dataconfig_chuangong" {
    UINT32 "level", --unique key
    UINT32 "TrainTimes",
    UINT32 "BeTrainTimes",
    UINT32 "TrainContribution",
    UINT32 "TrainExp",
    UINT32 "BeTrainExp",
    UINT32 "BeNPCTrainExp",
}



Config "dataconfig_circleconfig" {
    UINT32 "CircleSitID", --unique key
    STRING "Name",
    UINT32 "LevelLimit",
    STRING "Des",
    UINT32[10] "ActiveLevel";
    UINT32 "MountSoulRecommendGroup",
}



Config "dataconfig_cjconstellation" {
    UINT32 "id", --unique key
    STRING "name",
    UINT32 "atlas",
    STRING "icon",
    UINT32[2] "startDate";
    UINT32[2] "endDate";
    STRING "brief",
    UINT32[3] "fate";
    SINT32[2] "focusPos";
}



Config "dataconfig_clbcheckchoose" {
    UINT32 "CheckId", --unique key
    STRING "Name",
    UINT32 "ActivityLevel",
    UINT32 "LevelLimit",
    UINT32 "LevelId",
    UINT32 "Length",
}



Config "dataconfig_clonedconfig" {
    UINT32 "ClonedID", --unique key
    STRING "name",
    UINT32 "weapon",
    UINT32[2] "friendEffectId";
    UINT32[1] "enemyEffectId";
    UINT32[1] "friendEnterEffectId";
    UINT32[1] "enemyEnterEffectId";
    UINT32[1] "friendDieEffectId";
    UINT32[1] "enemyDieEffectId";
}



Config "dataconfig_clonedlv" {
    UINT32 "lv", --multi key
    UINT32 "weapon", --multi key
    SINT32 "weaponModel",
    SINT32[4] "clonedSkill";
    UINT32 "clonedSkill1",
    UINT32 "clonedSkill2",
    UINT32 "clonedFighting",
    UINT32 "Hp",
    UINT32 "OutAttack",
    UINT32 "OutDefence",
    UINT32 "InterAttack",
    UINT32 "InterDefence",
    UINT32 "FireAttack",
    UINT32 "FireDefence",
    UINT32 "WaterAttack",
    UINT32 "WaterDefence",
    UINT32 "EarthAttack",
    UINT32 "EarthDefence",
    UINT32 "WoodAttack",
    UINT32 "WoodDefence",
    UINT32 "AllDefence",
    UINT32 "IgnoreAllDef",
    UINT32 "CritRate",
    UINT32 "AntiCritRate",
    UINT32 "CritHurt",
    UINT32 "AntiCritHurt",
    UINT32 "HitRate",
    UINT32 "Dodge",
    UINT32 "HurtToHp",
    UINT32 "Speed",
}



Config "dataconfig_clonedskill" {
    UINT32 "clonedSkillID", --unique key
    STRING "name",
    UINT32 "skillQuaID",
    UINT32 "fighting",
    UINT32 "tpye",
    UINT32 "skillID",
    SINT32[1] "buffID";
    SINT32[1] "hostBuffID";
    STRING "desc",
    STRING "effectDesc",
    STRING "iconAtals",
    STRING "iconName",
    UINT32 "costProp",
}



Config "dataconfig_collect_carnival" {
    UINT32 "CollectID",
    STRING "CollectDesc",
    UINT32 "CollectType",
    STRING[2] "CollectParam";
    UINT32 "CollectNeedCnt",
    UINT32 "AwardPic",
    UINT32 "AwardID",
    UINT32 "AwardNum",
}



Config "dataconfig_collectbehavior" {
    UINT32 "ID", --unique key
    UINT32 "Type",
    UINT32[4] "Condition";
    UINT32 "Behavior",
    STRING "VideoName",
    UINT32 "VideoTime",
    UINT32 "SkillID",
    UINT32 "SkillTime",
    UINT32 "CollectType",
    UINT32 "CollectCD",
    STRING "text",
    UINT32 "Showicon",
}



Config "dataconfig_combatreward" {
    UINT32 "Id",
    UINT32 "Unlocklevel",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[4] "AwardList",
}



Config "dataconfig_conscienceshopgoods" {
    UINT32 "GoodsID", --unique key
    UINT32 "Limit",
    UINT32 "CurrencyType",
    UINT32 "CurrencyNum",
    UINT32 "ItemID",
    UINT32 "ItemNum",
}



Config "dataconfig_continuousbonus" {
    UINT32 "LowerLimit",
    UINT32 "Limit",
    UINT32 "Intimacy",
}



Config "dataconfig_coresoul" {
    UINT32 "CoreLevel",
    UINT32 "MaterialID",
    UINT32 "Num",
    UINT32 "Special",
    UINT32 "CoreOpen",
    UINT32 "NormalLimit",
    Struct {
        UINT32 "nParamID",
        UINT32 "nParamNum",
    }[4] "SoulList",
}



Config "dataconfig_coupequestion" {
    UINT32 "ID", --unique key
    STRING "Question",
    UINT32 "RightAnswer",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
}



Config "dataconfig_cp_homestead" {
    UINT32 "ID", --unique key
    STRING "icon",
    STRING "prefabPath",
}



Config "dataconfig_cpanswer" {
    UINT32 "ID", --unique key
    STRING "Question",
    UINT32 "QuesID", --multi key
    UINT32 "GeType", --multi key
    UINT32 "ColorType",
}



Config "dataconfig_cpaward" {
    UINT32 "ID",
    UINT32 "affectionate",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[3] "DropList",
}



Config "dataconfig_cpprogresstext" {
    UINT32 "ID", --unique key
    STRING "Text",
    STRING "Desc",
}



Config "dataconfig_cpquestion" {
    UINT32 "ID",
    UINT32 "Type",
    UINT32 "UIType",
    UINT32 "GeType",
    STRING "Question",
}



Config "dataconfig_cpquestionconfig" {
    UINT32 "Round",
    UINT32 "Rightpoint",
    UINT32 "Wrongpoint",
    SINT32[3] "Position";
    UINT32 "boxid", --unique key
}



Config "dataconfig_cprule" {
    UINT32 "ID", --unique key
    STRING[3] "Picture";
}



Config "dataconfig_createrole" {
    UINT32 "Id",
    STRING "desc",
    UINT32 "isOpen",
    UINT32 "isCreate",
    UINT32 "occId", --multi key
    UINT32 "genderId", --multi key
    UINT32 "modelId",
    UINT32 "dynamicBoneId",
    STRING "lowModelPath",
    STRING "highModelPath",
    STRING "soundPath",
    STRING "occSprite",
    STRING "occSpider",
    STRING "attSprite",
    UINT32 "difficulty",
    STRING "EnterAni",
    UINT32 "MenuID",
    Struct {
        UINT32 "equipID",
    }[2] "noobEquip",
}



Config "dataconfig_createrolecamera" {
    UINT32 "Id",
    STRING "Name",
    SINT32 "OffsetY",
    SINT32 "pAngle",
    SINT32 "yAngle",
    SINT32 "minDistance",
    SINT32 "distance",
    SINT32 "maxDistance",
    SINT32 "OffsetX",
    SINT32 "Threshold",
    SINT32 "Intensity",
    SINT32 "FaceOffsetY",
    SINT32 "FaceOffsetX",
    SINT32 "FaceDistance",
    SINT32 "MinPitchAngle",
    SINT32 "MaxPitchAngle",
    SINT32 "fov",
}



Config "dataconfig_createrolecameraglobalconfig" {
    UINT32 "ID", --unique key
    SINT32 "zoomSpeed",
    SINT32 "baseXSpeed",
    SINT32 "baseYSpeed",
    SINT32 "cameraChangeSpeed",
    SINT32 "slideSpeed",
}



Config "dataconfig_damaoxian" {
    UINT32 "ID", --unique key
    STRING "Question",
}



Config "dataconfig_danceadd" {
    UINT32 "Id",
    UINT32 "AddType",
    UINT32 "AddNum",
}



Config "dataconfig_dancearea" {
    UINT32 "ID",
    UINT32 "Level",
    Struct {
        UINT32[3] "Data1";
    }[2] "coordinate",
}



Config "dataconfig_dancelevel" {
    UINT32 "Level", --unique key
    UINT32 "Exp",
    UINT32 "ExpNum",
}



Config "dataconfig_dancename" {
    UINT32 "ID",
    UINT32 "Level", --multi key
    UINT32 "Sex", --multi key
    UINT32 "Skill",
    STRING "DanceName",
}



Config "dataconfig_defaultquality" {
    UINT32 "Level", --unique key
    UINT32 "quality",
    UINT32 "quality_min",
    UINT32 "quality_max",
}



Config "dataconfig_devicegrade" {
    STRING "DeviceName",
    UINT32 "Level",
    UINT32 "quality",
    UINT32 "lowestQuaToUseHighRes",
    UINT32 "highRes",
}



Config "dataconfig_dialoguecamera" {
    UINT32 "ID", --unique key
    SINT32[3] "PosOffset";
    SINT32[3] "RotationOffset";
}



Config "dataconfig_dialoguechildnpc" {
    UINT32 "ID", --unique key
    SINT32[3] "PosOffset";
    SINT32[3] "RotationOffset";
}



Config "dataconfig_dialogueconfig" {
    UINT32 "ID", --unique key
    SINT32 "WhichMode",
    BOOL "isNeedHero",
    Struct {
        SINT32 "npcID",
        SINT32 "isRoleTalk",
        SINT32 "side",
        SINT32 "isRoleSound",
        STRING "soundPath",
        SINT32 "time",
        SINT32 "isContinuous",
        STRING "text",
        SINT32 "moodID",
        SINT32 "moodtime",
        SINT32 "cameraPosID",
        SINT32 "focal",
        SINT32[1] "childNpcID";
        SINT32[1] "childNpcPosID";
        SINT32[1] "dontNeedNpc";
    }[6] "info_list",
    Struct {
        UINT32 "operatorCode",
        UINT32[2] "paras";
        STRING "Text",
    }[4] "Operatorlist",
}



Config "dataconfig_dialogueconfig_ios" {
    UINT32 "ID",
    SINT32 "WhichMode",
    Struct {
        SINT32 "npcID",
        SINT32 "isRoleTalk",
        SINT32 "side",
        SINT32 "isRoleSound",
        STRING "soundPath",
        SINT32 "time",
        SINT32 "isContinuous",
        STRING "text",
        SINT32 "moodID",
        SINT32 "moodtime",
    }[6] "info_list",
    Struct {
        UINT32 "operatorCode",
        UINT32[2] "paras";
        STRING "Text",
    }[4] "Operatorlist",
}



Config "dataconfig_dicinfoconfig" {
    SINT32 "id", --unique key
    STRING "info",
    UINT32 "type",
    UINT32 "operation",
}



Config "dataconfig_dicinfoconfig_ios" {
    SINT32 "id",
    STRING "info",
    UINT32 "type",
    UINT32 "operation",
}



Config "dataconfig_difficultyconfig" {
    SINT32 "id",
    SINT32 "capacityCoefficient",
}



Config "dataconfig_diffpreload" {
    UINT32 "maxLvl",
    UINT32 "minLvl",
    Struct {
        UINT32 "SceneID",
        STRING[36] "assetPathList";
    }[5] "SceneInfo",
}



Config "dataconfig_doubleaction" {
    UINT32 "ID", --unique key
    BOOL "CanMove",
    STRING "ActionName",
    STRING "Invite",
    UINT32 "Talk",
    Struct {
        UINT32[2] "ModelID";
        STRING[2] "Animator";
        STRING[2] "RunMotion";
        STRING[2] "StandMotion";
    }[6] "ActionData",
}



Config "dataconfig_dreamlandconf" {
    UINT32 "id", --unique key
    Struct {
        UINT32 "LevelID",
        UINT32 "Weight",
    }[3] "LevelInfos",
}



Config "dataconfig_drinkexpboost" {
    UINT32 "ID", --unique key
    UINT32 "AddExp",
    UINT32 "Cost",
    UINT32 "Reward",
}



Config "dataconfig_drinkquestion" {
    SINT32 "ID",
    STRING "Question",
    UINT32 "RightAnswer",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
    SINT32 "NodeID", --unique key
}



Config "dataconfig_drop" {
}



Config "dataconfig_dropteam" {
    UINT32 "TeamID", --unique key
    UINT32[84] "NomalDrop";
    UINT32[10] "EliteDrop";
    UINT32[10] "GoblinDrop";
    UINT32[10] "BossDrop";
    UINT32[1] "SpecialDrop";
    Struct {
        SINT32 "MonsterID",
        UINT32[2] "DropID";
    }[25] "DesignatedDrop",
}



Config "dataconfig_dynamicbonecoliderdata" {
    UINT32 "ColliderID",
    STRING "BipName",
    SINT32[3] "Center";
    UINT32 "Radius",
    UINT32 "Height",
    UINT32 "Direction",
    UINT32 "Bound",
}



Config "dataconfig_dynamicbonesetting" {
    UINT32 "dynamicBoneId",
    STRING "Root",
    UINT32 "UpDateRate",
    UINT32 "Damping",
    UINT32 "Elasticity",
    UINT32 "Siffness",
    UINT32 "Inert",
    UINT32 "Radius",
    UINT32 "EndLength",
    UINT32[1] "EndOffset";
    UINT32[1] "Gravity";
    UINT32[1] "Force";
    UINT32 "ColliderID",
    UINT32 "ExdusionsSize",
    UINT32 "FreezeAxis",
    UINT32 "DistantDisable",
    STRING "ReferenceObject",
    UINT32 "DistanceToObject",
}



Config "dataconfig_effectconfig" {
    UINT32 "id", --unique key
    STRING "note",
    STRING "effectName",
    STRING "lowEffectName",
    STRING "lowContortEffectName",
    UINT32 "lastTime",
    UINT32 "delayTime",
    UINT32 "hangPoint",
    SINT32[1] "hangPointOffset";
    UINT32 "hangPoint2",
    SINT32[1] "hangPointOffset2";
    BOOL "isFollow",
    UINT32 "behitEffectDirection",
    SINT32[2] "XaxisRotation";
    SINT32[2] "YaxisRotation";
    SINT32 "conSexType",
    SINT32 "conCampType",
    UINT32[2] "effectIdSet";
    UINT32 "uiEffType",
    UINT32 "uiEffLevel",
    UINT32 "visible",
}



Config "dataconfig_encouraging" {
    UINT32 "EncouragingTime", --unique key
    UINT32 "BuffIdEnc",
    UINT32 "EncContent",
}



Config "dataconfig_endlesslevelconfig" {
    UINT32 "WaveNum",
    Struct {
        UINT32 "RewardGoodsID",
        UINT32 "RewardGoodsNum",
    }[5] "RewardList",
}



Config "dataconfig_equipexp" {
    UINT32 "Grade",
    UINT32 "NextGradeWhite",
    UINT32 "NextGradeGreen",
    UINT32 "NextGradeBlue",
    UINT32 "NextGradePurple",
    UINT32 "NextGradeOrange",
    SINT32 "white",
    SINT32 "green",
    SINT32 "blue",
    SINT32 "purple",
    SINT32 "orange",
    UINT32 "Coin",
    UINT32 "Crit",
}



Config "dataconfig_equiplevelupprop" {
    UINT32 "Level",
    UINT32 "Position",
    UINT32 "QuaID",
    UINT32 "LevelUP",
    Struct {
        UINT32 "iKey",
        UINT32 "iValue",
    }[23] "paramList",
}



Config "dataconfig_equiprefinecost" {
    UINT32 "EquipLevel", --multi key
    UINT32 "EquipConType", --multi key
    Struct {
        UINT32 "AttrQuality",
        UINT32 "NeedItem",
        UINT32 "NeedNum",
    }[5] "EquipRefine",
}



Config "dataconfig_equipreward" {
    UINT32 "ID",
    UINT32 "Num",
    UINT32 "Level",
    UINT32 "Quality",
    Struct {
        UINT32 "ItemID",
        UINT32 "Num",
    }[5] "Reward",
}



Config "dataconfig_equipsuit" {
    UINT32 "SuitID", --unique key
    STRING "SuitName",
    UINT32[27] "ItemNo";
    Struct {
        UINT32 "NeedNum",
        UINT32[1] "nParamID";
        SINT32[1] "nParamNum";
        UINT32[1] "extraeffect";
    }[3] "paramList",
}



Config "dataconfig_erroranti" {
    STRING "id", --unique key
}



Config "dataconfig_evaluate" {
    UINT32 "ID",
    UINT32 "GameID", --unique key
    UINT32[3] "MarksSection";
}



Config "dataconfig_evencutconf" {
    SINT32 "ID", --unique key
    SINT32 "MaxEvenCut",
    Struct {
        UINT32 "KillNum",
        UINT32 "TextID",
    }[3] "KillEnemyArray",
}



Config "dataconfig_eventtype" {
    UINT32 "EventTypeID",
    UINT32 "LevelID",
    STRING "EventType",
    UINT32 "IsAttend",
}



Config "dataconfig_exchange_carnival" {
    UINT32 "ExchangeID",
    UINT32 "CollectPic",
    UINT32 "ItemID",
    UINT32 "ItemNum",
    UINT32 "ExchangeCount",
    Struct {
        UINT32 "AwardID",
        UINT32 "AwardNum",
    }[3] "AwardInfo",
}



Config "dataconfig_exchangebutton" {
    UINT32 "ID", --unique key
    STRING "Name",
    UINT32 "Group",
    UINT32 "Order",
}



Config "dataconfig_exchangetype" {
    UINT32 "ID", --unique key
    UINT32[11] "GroupList";
}



Config "dataconfig_expandconfig" {
    UINT32 "SlotNum", --unique key
    UINT32 "CostMoney",
    UINT32 "CostGood",
}



Config "dataconfig_explevelconfig" {
    UINT32 "DifficultyID",
    UINT32 "WaveID",
    UINT32[10] "MonsterIDList";
}



Config "dataconfig_exploss" {
}



Config "dataconfig_expression" {
    UINT32 "ID",
    STRING "PictureName",
    STRING "Expressionname",
    UINT32 "canSelect",
    UINT32 "isDynamic",
    UINT32 "FrameNum",
}



Config "dataconfig_expupgrade" {
    UINT32 "Grade", --unique key
    UINT32 "NextGradeExp",
    UINT32 "UnlockDay",
    SINT32 "Point",
    Struct {
        UINT32 "ActionID",
        UINT32 "Param1",
        UINT32 "Param2",
    }[15] "AfterAction",
}



Config "dataconfig_expupgrade_diff" {
    UINT32 "Grade",
    UINT32 "NextGradeExp",
    UINT32 "UnlockDay",
    SINT32 "Point",
    Struct {
        UINT32 "ActionID",
        UINT32 "Param1",
        UINT32 "Param2",
    }[15] "AfterAction",
}



Config "dataconfig_expupgrade_ios" {
    UINT32 "Grade",
    UINT32 "NextGradeExp",
    UINT32 "UnlockDay",
    SINT32 "Point",
    Struct {
        UINT32 "ActionID",
        UINT32 "Param1",
        UINT32 "Param2",
    }[15] "AfterAction",
}



Config "dataconfig_extraeffectconf" {
    UINT32 "ID", --unique key
    STRING "StrDesc",
    UINT32 "Condition",
    UINT32 "SkillID",
    UINT32[5] "AttackPoind";
    UINT32 "EnemyEffectID",
    UINT32 "FriendEffectID",
    UINT32 "SelfEffectID",
    UINT32 "NpcID",
}



Config "dataconfig_extrastrengthenadd" {
    UINT32 "ID", --multi key
    UINT32 "NeedLevel",
    UINT32 "Weapon", --multi key
    Struct {
        UINT32 "nkey",
        UINT32 "value",
    }[5] "ExtraAttrlist",
    UINT32 "Fighting",
}



Config "dataconfig_facecamera" {
    UINT32 "ModelID", --unique key
    SINT32[3] "Forward";
    UINT32 "HorizontalMax",
    UINT32 "VerticalMax",
    UINT32 "VerticalMin",
    UINT32 "EyeSpeed",
    UINT32 "HeadSpeed",
    SINT32 "HeadVerticalOffset",
    SINT32 "BodyOffset",
    UINT32 "StartDistance",
}



Config "dataconfig_facechange" {
    UINT32 "ID",
    UINT32 "Type",
    STRING "Name",
    UINT32 "MakeUpType",
    UINT32 "OppositeNumber",
    Struct {
        UINT32 "ID",
        UINT32 "Property",
        UINT32[3] "Parms";
    }[6] "Bones",
}



Config "dataconfig_faceinitvalue" {
    UINT32 "ID",
    UINT32 "ModelID", --multi key
    UINT32 "PreFaceID",
    STRING "InitValue",
}



Config "dataconfig_facenet" {
    UINT32 "ID",
    UINT32 "Type",
    STRING "titleName",
    STRING "NetName",
    UINT32[3] "Offset";
}



Config "dataconfig_facetype" {
    UINT32 "Type", --multi key
    STRING "path",
    STRING "baseTexString",
    STRING "TexPath",
}



Config "dataconfig_factionauthority" {
    UINT32 "PositionID", --unique key
    UINT32 "PowerID",
    STRING "PositionName",
    UINT32 "PositionNum",
    UINT32 "ApprovalAuthority",
    UINT32 "BuildAuthority",
    UINT32 "UpdateAuthority",
    UINT32 "KickOffAuthority",
    UINT32[6] "SpecifyPositionAutority";
    UINT32 "DismissAuthority",
    UINT32 "ModifyManifesto",
    UINT32 "ModifyNotice",
    UINT32 "AotuApprovalAuthority",
    UINT32 "MoneyRate",
    UINT32 "LevelUp",
    UINT32 "EditEmail",
    UINT32 "IsManager",
    UINT32[14] "Authority";
    UINT32[6] "AutorityAllow";
    UINT32[6] "KickOff";
    UINT32 "JoinLeague",
}



Config "dataconfig_factionaward" {
    UINT32 "Income",
    UINT32 "WagesID",
    UINT32 "WagesNum",
}



Config "dataconfig_factionbuild" {
    UINT32 "BuildID", --unique key
    STRING "BuildName",
    UINT32 "BuildMaxGrade",
    UINT32[10] "UpGradeMoney";
    STRING "BuildWord",
    UINT32[10] "BuildNeedLevel";
}



Config "dataconfig_factioncompeteconfig" {
    UINT32 "ActivityID", --unique key
    STRING "ActivityName",
    STRING "Brief",
    UINT32 "ActivityLevel",
    UINT32 "OpenDay",
    UINT32 "EndDay",
    UINT32 "CloseDay",
    UINT32 "PreEndMail",
    UINT32 "EndMail",
}



Config "dataconfig_factioncompeteshow" {
    UINT32 "RewardID",
    UINT32 "ActivityID", --multi key
    UINT32 "OrderMin",
    UINT32 "OrderMax",
    UINT32 "DayItemID",
    UINT32 "DayItemNum",
    STRING "Title",
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNum",
    }[3] "Reward",
}



Config "dataconfig_factiondonation" {
    UINT32 "Times",
    UINT32 "DonationID",
    UINT32 "DonationNum",
    UINT32 "DonationAwradID",
    UINT32 "DonationAwardNum",
    UINT32 "FamilyMoney",
}



Config "dataconfig_factiondonation_small" {
    UINT32 "Times",
    UINT32 "DonationID",
    UINT32 "DonationNum",
    UINT32 "DonationAwradID",
    UINT32 "DonationAwardNum",
    UINT32 "FamilyMoney",
}



Config "dataconfig_factionescort" {
    UINT32 "ID", --unique key
    UINT32 "Weight",
    UINT32 "FactionMoneyCost",
    UINT32 "DropTeamID",
    UINT32 "CaptureMoneyRatio",
    UINT32 "CaptureExpRatio",
    UINT32 "NPCID",
    STRING "QualityName",
}



Config "dataconfig_factionescortpath" {
    UINT32 "ID",
    UINT32[3] "Pos";
}



Config "dataconfig_factioninfo" {
    UINT32 "Grade",
    UINT32 "FameForUpGrade",
    UINT32 "MaxMember",
    UINT32 "WelfareItemNo",
    UINT32 "Num",
    UINT32 "ShopId",
    UINT32 "WelfareTimes",
    UINT32 "WelfareCoolDown",
    UINT32 "LevelMoney",
    STRING "BuildWord",
    UINT32 "NeedDays",
}



Config "dataconfig_factioninfo_small" {
    UINT32 "Grade",
    UINT32 "FameForUpGrade",
    UINT32 "MaxMember",
    UINT32 "WelfareItemNo",
    UINT32 "Num",
    UINT32 "ShopId",
    UINT32 "WelfareTimes",
    UINT32 "WelfareCoolDown",
    UINT32 "LevelMoney",
    STRING "BuildWord",
    UINT32 "NeedDays",
}



Config "dataconfig_factionwelcome" {
    UINT32 "ID", --unique key
    STRING "Word",
}



Config "dataconfig_factionwelfare" {
    UINT32 "id",
    STRING "Name",
    UINT32 "Type",
    UINT32 "value",
    UINT32 "Order",
    UINT32 "Level",
    STRING "Icon",
    STRING "Opentime",
    STRING "WelfareDec",
    STRING "Button",
}



Config "dataconfig_fairy_attr_order" {
    UINT32 "AttrID", --unique key
    UINT32 "Init",
    UINT32 "Order",
}



Config "dataconfig_fairy_change" {
    UINT32 "FairyID", --unique key
    UINT32 "ResID", --unique key
    UINT32 "FairyScale",
    STRING "Name",
    UINT32 "FightNum",
    STRING "ActivateDec",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[4] "Attr",
    UINT32 "FairysPoint",
    UINT32 "OpenServerLevel",
}



Config "dataconfig_fairy_class" {
    UINT32 "FairyClass", --unique key
    UINT32 "ResID", --unique key
    UINT32 "FairyScale",
    STRING "Name",
    STRING "Atlas",
    STRING "Icon",
    UINT32 "Level",
    UINT32 "StarNum",
    UINT32 "FairysPoint",
}



Config "dataconfig_fairy_soul" {
    UINT32 "SoulID", --multi key
    UINT32 "tupoClass", --multi key
    UINT32 "OpenClass",
    UINT32 "OpenStar",
    UINT32 "ShowClass",
    UINT32 "ActivateItemID",
    UINT32 "tupoActivateItemID",
    UINT32 "tupoActivateItemNum",
    UINT32 "tupoNum",
    UINT32 "ActivateTimes",
    UINT32 "FightNum",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[4] "Attr",
}



Config "dataconfig_fairy_update" {
    UINT32 "FairyClass", --multi key
    UINT32 "Star",
    UINT32 "CostItemID",
}



Config "dataconfig_fairy_updateattribte" {
    UINT32 "FairyClass", --multi key
    UINT32 "Star",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[6] "Attr",
}



Config "dataconfig_fairyland" {
    UINT32 "ID", --unique key
    STRING "LevelName",
    STRING "Name",
    UINT32 "BossID",
    UINT32 "LevelID",
    STRING "BossIcon",
    UINT32 "magnification",
    UINT32 "BossScale",
    SINT32[3] "BossPosition";
    SINT32[3] "BossAngle";
    UINT32 "Grade",
    UINT32 "Recommend",
    Struct {
        UINT32 "MarkTime",
        UINT32 "MarkPower",
    }[6] "Mark",
    UINT32 "LevelExp",
    UINT32 "FirstBonus",
    UINT32 "Bonus",
    UINT32 "SpecialBOSSID",
}



Config "dataconfig_femalename" {
}



Config "dataconfig_festival" {
    UINT32 "id", --unique key
    STRING "Festival",
    STRING "Iocn",
    STRING "Title",
    STRING "page",
    UINT32 "openDay",
    UINT32 "Duration",
    STRING "StartTime",
    STRING "EndTime",
    Struct {
        SINT32[3] "Positions";
        SINT32 "Scale",
    }[6] "activityList",
}



Config "dataconfig_festivalactivities" {
    UINT32 "id", --unique key
    UINT32 "IndexesID",
    STRING "name",
    STRING "OpenDay",
    STRING "CloseDay",
    STRING "Label",
    STRING "Iocn",
    UINT32 "Type",
    UINT32 "EnterID",
    UINT32 "ActivityID",
    UINT32 "textID",
}



Config "dataconfig_fightingattr" {
    UINT32 "AttrID", --unique key
    UINT32 "WeightNum",
}



Config "dataconfig_fightsoulconfig" {
    UINT32 "Grade",
    UINT32 "MaterialID1",
    UINT32 "Num1",
    UINT32 "MaterialID2",
    UINT32 "Rate",
    Struct {
        UINT32 "nParamID",
        UINT32 "nParamNum",
    }[6] "SoulList",
}



Config "dataconfig_filters" {
    UINT32 "FilterID",
    STRING "FilterName",
    UINT32 "IsRGB",
    UINT32 "IsGray",
    UINT32 "IsTexture",
    UINT32 "RedValue",
    UINT32 "GreenValue",
    UINT32 "BlueValue",
    UINT32 "BrightnessValue",
    UINT32 "ContrastValue",
    UINT32 "SaturationValue",
    STRING "ResoucePath",
}



Config "dataconfig_firstmeet" {
    UINT32 "order",
    UINT32 "id",
    STRING "name",
    STRING "desc",
}



Config "dataconfig_firstnamec" {
}



Config "dataconfig_fishcatch" {
    UINT32 "AreaID", --unique key
    STRING "NameID",
    UINT32 "IntroID",
    UINT32 "CatchTime",
    UINT32 "GoodID",
    UINT32 "GoodNum",
    Struct {
        UINT32 "DropID",
        UINT32 "DropTimes",
        UINT32 "MaxTypes",
    }[3] "DropList",
}



Config "dataconfig_fishconfig" {
    UINT32 "FishID", --unique key
    STRING "Name",
    SINT32[1] "EnemyTags";
    STRING "PlayerTag",
    STRING "FishTag",
    STRING "Path",
    UINT32 "FishType", --multi key
    UINT32 "Rarity",
    UINT32 "FishEffect",
    UINT32[5] "LevelUp";
    UINT32[5] "LevelEffect";
    UINT32 "GoodID", --unique key
    UINT32 "MaxNum",
    STRING "BigIcon",
    STRING "RarityLevel",
}



Config "dataconfig_fishdrop" {
    UINT32 "ID",
    UINT32 "DropID",
    UINT32 "FishID",
    UINT32 "Weight",
}



Config "dataconfig_fishgroup" {
    UINT32 "ID", --unique key
    UINT32 "FishID",
    UINT32[5] "FishNum";
    UINT32 "IsFullPath",
    UINT32 "CreateRange",
    UINT32[3] "OffsetRange";
    UINT32 "DelayMove",
    UINT32 "ShakeSpeedRange",
    UINT32 "ShakeFrequencyRange",
    UINT32 "Duration",
    UINT32 "Smooth",
    UINT32 "ChangeStateDistance",
    UINT32 "EscapeTime",
    UINT32 "EscapeSpeed",
    UINT32 "GatherSpeed",
    UINT32 "GatherAddSpeed",
    UINT32 "GatherTurnAroundTime",
    UINT32 "FollowSpeed",
    UINT32 "FollowTime",
    UINT32 "OrientationMode",
    UINT32 "WrapMode",
    UINT32 "AutoStart",
    UINT32 "AutoClose",
    SINT32[3] "RootPos";
    Struct {
        SINT32[3] "Pos";
    }[10] "PosList",
}



Config "dataconfig_fishing" {
    UINT32 "ID",
    UINT32 "restoreTime",
    UINT32 "maxTimes",
    Struct {
        UINT32 "dropWeight",
        UINT32 "itemID",
    }[4] "dropInfo",
}



Config "dataconfig_fitbasenum" {
    UINT32 "AttrID",
    Struct {
        UINT32 "AttrID",
        SINT32 "FitValue",
    }[24] "FitBaseValueList",
}



Config "dataconfig_followenter" {
    UINT32 "ID",
    UINT32[5] "ActivityID";
}



Config "dataconfig_follownpc" {
    UINT32 "ID", --unique key
    STRING "Name",
    UINT32 "BuffID",
    Struct {
        UINT32 "TaskID",
        UINT32 "Award",
        UINT32 "CloseValue",
    }[4] "TaskInfo",
    Struct {
        UINT32 "ItemID",
        UINT32 "IsExclusive",
        UINT32 "CloseDown",
        UINT32 "CloseUp",
    }[4] "PresentInfo",
    Struct {
        UINT32 "CloseValue",
        STRING "Text",
    }[2] "NPCInfo",
}



Config "dataconfig_foodeffect" {
    UINT32 "ID", --unique key
    UINT32 "AddExp",
    UINT32 "Cost",
    UINT32 "Reward",
}



Config "dataconfig_forgeconfig" {
    UINT32 "Level",
    UINT32 "EquipConType",
    UINT32 "ID",
    UINT32 "Num",
    UINT32 "CostGold",
    UINT32 "BlueCost",
    UINT32 "PurpleCost",
    UINT32 "OrangeCost",
}



Config "dataconfig_fourlikevalley" {
    UINT32 "id",
    Struct {
        UINT32 "LevelID",
        UINT32 "Weight",
        UINT32 "Lock",
        UINT32 "Robot",
        UINT32 "RobotActivated",
        UINT32 "RobotStrengthen",
    }[4] "LevelInfos",
}



Config "dataconfig_foxmodel" {
    UINT32 "Index",
    UINT32 "ResID",
    STRING "Name",
    UINT32 "ModelScale",
    STRING[2] "Action";
    SINT32[16] "UiModelData";
}



Config "dataconfig_framepage" {
    UINT32 "ObjectiveID", --unique key
    STRING "Name",
    STRING "IconName",
    UINT32 "JumpID",
    UINT32 "SortID",
    STRING "childDesc",
    UINT32 "LevelID",
    UINT32 "TypeID", --multi key
    UINT32 "ChildID",
}



Config "dataconfig_freepick" {
    UINT32 "ID",
    UINT32 "ItemID",
    UINT32 "ItemNum",
}



Config "dataconfig_friendshipadd" {
    UINT32 "Id", --unique key
    STRING "Description",
    UINT32 "AddType",
    UINT32[12] "Data1";
    UINT32 "AddNum",
    UINT32 "NumLimit",
}



Config "dataconfig_friendshiplevel" {
    UINT32 "Level",
    UINT32 "MaxNum",
    UINT32 "Exp",
    STRING "Description",
    STRING "TextColor",
    STRING "TextOutline",
    UINT32 "IsMax",
}



Config "dataconfig_fumo" {
    UINT32 "Level",
    UINT32 "Fighting",
}



Config "dataconfig_functionpre" {
    UINT32 "FunPreID",
    UINT32 "PreOpID",
    STRING "ActivityName",
    STRING "FunctionIcon",
    STRING "FunctionPic",
    STRING[1] "Description";
    STRING "Description2",
}



Config "dataconfig_furnitureconfig" {
    UINT32 "id",
    STRING "name",
    UINT32 "statenum",
    UINT32 "playernum",
}



Config "dataconfig_gameprograss" {
    UINT32 "ID", --multi key
    UINT32 "Stage", --multi key
    UINT32 "Time",
}



Config "dataconfig_generalconfig" {
    UINT32 "id", --unique key
    STRING "desc",
    STRING "para1",
    STRING "para2",
    STRING "para3",
}



Config "dataconfig_ghostinfo" {
    UINT32 "ID", --unique key
    UINT32 "TriggerID",
    UINT32 "GhostSequenceID",
    UINT32 "GhostScenarioID",
    UINT32 "GhostDialID",
    Struct {
        UINT32 "isSingle",
        UINT32 "GhostLevelID",
        UINT32 "isEntrance",
    }[1] "GhostLevelInfo",
    Struct {
        UINT32 "TaskID",
        UINT32 "minLevel",
    }[1] "ConditionInfo",
}



Config "dataconfig_ghosttask" {
    UINT32 "ID",
    STRING "Title",
    STRING "Desc",
    UINT32 "TaskType",
    UINT32[1] "GetTaskNpcID";
    UINT32 "GetTalkID",
    UINT32[2] "WorkingTaskNpcID";
    UINT32 "WorkingTalkID",
    UINT32[1] "FinishTaskNpcID";
    UINT32 "FinishTalkID",
    UINT32 "TimeLimit",
    UINT32 "IsCanGiveUp",
    UINT32[1] "NextTask";
    UINT32 "IsDerictAward",
    UINT32 "IsGetRewardWithNpc",
    UINT32[1] "BeforTaskList";
    UINT32 "BelongTask",
    Struct {
        UINT32 "ActionID",
        UINT32 "param1",
        UINT32 "param2",
    }[2] "AfterComplete",
    UINT32 "LevelMin",
    UINT32 "LevelMax",
    UINT32 "MustCount",
    Struct {
        UINT32 "IsMust",
        UINT32 "ActionType",
        SINT32 "Param",
        UINT32 "Param2",
        UINT32 "NeedNum",
        SINT32[9] "TargetInfo";
        UINT32 "IsSpecial",
        SINT32[1] "TargetPositions";
        STRING "TaskDes",
        STRING "FinishDes",
    }[3] "CompleteLimit",
    UINT32 "GoldNum",
    UINT32 "EXPNum",
    Struct {
        UINT32 "ItemID",
        UINT32 "Num",
    }[4] "NomalAward",
    Struct {
        UINT32 "Weight",
        UINT32 "ItemID",
        UINT32 "Num",
    }[5] "RandomAward",
    Struct {
        Struct {
            UINT32 "ItemID",
            UINT32 "Num",
        }[2] "SigleAward",
    }[5] "ChooseAward",
    UINT32 "PickID",
    UINT32 "TaskMonstePreload",
    Struct {
    }[4] "GetItem",
}



Config "dataconfig_gift" {
    UINT32 "ID", --unique key
    UINT32[4] "FixedNums";
    UINT32 "ItemID",
    UINT32 "Gold",
    UINT32 "AddValue",
    UINT32 "Icon",
    STRING "Name",
    UINT32 "CntID",
    STRING "Effect",
}



Config "dataconfig_globalconfig" {
    UINT32 "id", --unique key
    UINT32 "GlobalValue",
    UINT32[8] "GlobalArry";
    STRING[2] "Time";
}



Config "dataconfig_globalquestion" {
    UINT32 "ID", --unique key
    STRING "Question",
    UINT32 "RightAnswer",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
}



Config "dataconfig_goldshopconfig" {
    UINT32 "ShopID", --unique key
    STRING "ShopName",
    UINT32 "Weight",
    UINT32 "ShopType",
    STRING "OpenTime",
    STRING "CloseTime",
    UINT32 "Level",
    UINT32 "FlashDay",
    STRING "FlashTime",
    UINT32 "ZoneOpenDay",
    UINT32[1] "WeekOpenDays";
    UINT32 "ZoneOpenDayClose",
}



Config "dataconfig_goldshopconfig_diff" {
    UINT32 "ShopID",
    STRING "ShopName",
    UINT32 "Weight",
    UINT32 "ShopType",
    STRING "OpenTime",
    STRING "CloseTime",
    UINT32 "Level",
    UINT32 "FlashDay",
    STRING "FlashTime",
    UINT32 "ZoneOpenDay",
    UINT32[1] "WeekOpenDays";
    UINT32 "ZoneOpenDayClose",
}



Config "dataconfig_goldshopgoodsconfig" {
    UINT32 "GoodID", --unique key
    UINT32 "ShopID",
    UINT32 "ItemID",
    UINT32 "PosID",
    UINT32 "CurrencyID",
    UINT32 "CurrencyNum",
    UINT32 "Discount",
    UINT32 "Limit",
    UINT32 "VIPlevel",
    UINT32 "VIPlimit",
    UINT32 "OpenDay",
    UINT32 "CloseDay",
    UINT32 "NoviceRecommend",
}



Config "dataconfig_goldshopgoodsconfig_diff" {
    UINT32 "GoodID",
    UINT32 "ShopID",
    UINT32 "ItemID",
    UINT32 "PosID",
    UINT32 "CurrencyID",
    UINT32 "CurrencyNum",
    UINT32 "Discount",
    UINT32 "Limit",
    UINT32 "VIPlevel",
    UINT32 "VIPlimit",
    UINT32 "OpenDay",
    UINT32 "CloseDay",
    UINT32 "NoviceRecommend",
}



Config "dataconfig_goldshopgoodsconfig_ios" {
    UINT32 "GoodID",
    UINT32 "ShopID",
    UINT32 "ItemID",
    UINT32 "PosID",
    UINT32 "CurrencyID",
    UINT32 "CurrencyNum",
    UINT32 "Discount",
    UINT32 "Limit",
    UINT32 "VIPlevel",
    UINT32 "VIPlimit",
    UINT32 "OpenDay",
    UINT32 "CloseDay",
    UINT32 "NoviceRecommend",
}



Config "dataconfig_goodconfig" {
    UINT32 "ID", --unique key
    STRING "Name",
    STRING "Desc",
    STRING "Atlas",
    STRING "Icon",
    UINT32 "IconType",
    STRING "Model",
    UINT32 "Scale",
    UINT32 "labelType",
    UINT32 "Sequence",
    UINT32 "GoodTypeID",
    UINT32 "QuaID",
    STRING "GoodDesc",
    STRING "GoodEffect",
    UINT32 "SellFlag",
    UINT32 "SaleType",
    UINT32 "Price",
    UINT32 "UseCondition",
    UINT32 "UseConditionValue",
    UINT32 "UseFlag",
    UINT32 "ShowTips",
    UINT32 "FuncID",
    Struct {
        UINT32 "EffectID",
        SINT32 "EffectValue",
    }[4] "UseEffectParamList",
    UINT32 "SumMax",
    UINT32 "ExistTimeType",
    UINT32 "ExistTimeValue",
    UINT32 "AutoPick",
    UINT32 "broadcast",
    UINT32 "IsAutoOpen",
    UINT32 "ReplaceID",
    UINT32 "ReplaceNum",
    UINT32 "ShenShouQuality",
    UINT32 "NirvanaID",
    UINT32 "MaxUseTimes",
    UINT32 "DayUseTimes",
    UINT32 "AuctionStartPrice",
    UINT32 "AuctionFinalPrice",
    UINT32 "CanGift",
    UINT32 "IsCard",
    UINT32 "EquipTypeID",
}



Config "dataconfig_goodvoicetitle" {
    UINT32 "TitleConfigID", --unique key
    UINT32 "TitleType",
    UINT32 "CompareType",
    STRING "Question",
    STRING "Answer",
}



Config "dataconfig_goodvoicetitletype" {
    UINT32 "TitleType", --unique key
    STRING "Name",
}



Config "dataconfig_group_hero_dictionary" {
    STRING "key",
    UINT32 "value",
}



Config "dataconfig_group_hero_prograsss" {
    UINT32 "Stage",
    UINT32 "Time",
}



Config "dataconfig_group_hero_record" {
    UINT32 "WinRateMin",
    UINT32 "WinRateMax",
    UINT32 "DictionaryID",
}



Config "dataconfig_group_hero_skilladvise" {
    UINT32 "Type", --multi key
    SINT32[4] "SkillArray";
    STRING "Name",
    STRING "Desc",
    UINT32 "level",
}



Config "dataconfig_groupindex" {
    UINT32 "GroupId",
    STRING "GroupName",
    UINT32[14] "CheckIdGroup";
}



Config "dataconfig_groupinfo" {
}



Config "dataconfig_grouppage" {
    UINT32 "TypeID",
    STRING "Name",
    Struct {
        UINT32 "childType",
        STRING "childName",
    }[15] "childTypeList",
}



Config "dataconfig_growth" {
    UINT32 "ID",
    UINT32 "DisplayType",
    UINT32[5] "ActivityID";
    UINT32[2] "OpenLevelID";
}



Config "dataconfig_guarddata" {
    UINT32 "id", --unique key
    UINT32 "guardType",
    SINT32[1] "guardParams";
    BOOL "needShowArea",
}



Config "dataconfig_guideinfo" {
    UINT32 "ID", --unique key
    Struct {
        STRING "text",
        SINT32[6] "textParas";
        SINT32[5] "effectFrameScale";
        SINT32[4] "frameScale";
        UINT32[2] "highlightScale";
        STRING "tipsText",
        STRING "soundPath",
        SINT32 "time",
    }[13] "guideList",
}



Config "dataconfig_hairmaterial" {
    UINT32 "optionsID", --unique key
    STRING "path",
    STRING "iconName",
}



Config "dataconfig_hczbshenshou" {
    UINT32 "LevelID", --multi key
    UINT32 "ShenShouID", --unique key
    UINT32 "Star",
    UINT32 "StarLevel",
    UINT32 "Belong", --multi key
}



Config "dataconfig_headportrait" {
    UINT32 "ID", --unique key
    UINT32 "model", --multi key
    UINT32 "occid",
    UINT32 "sex",
    STRING "icon",
    STRING "bigIcon",
    STRING "bust",
    STRING "bustAtlas",
    STRING "mainIcon",
    STRING "HeroBustAtlas",
    STRING "HeroBust",
}



Config "dataconfig_hero_array" {
    UINT32 "HeroNum", --unique key
    UINT32 "AttrIncrease",
    UINT32 "HeroElseTimes",
    Struct {
        UINT32 "HeroElseBuff",
        UINT32 "BuffLock",
        UINT32 "IconLock",
        UINT32 "BuffLockLevel",
    }[6] "Skill",
    UINT32 "FightNum",
}



Config "dataconfig_hero_arraylist" {
    UINT32 "HeroListNum", --unique key
    UINT32 "ArrayLock",
}



Config "dataconfig_hero_change" {
    UINT32 "HeroID", --unique key
    STRING "Name",
    UINT32 "ResID",
    UINT32 "HeroScale",
    UINT32 "HeroIcon",
    STRING "HeroIconEffects",
    UINT32 "HeroBGID",
    STRING[1] "HeroBGM";
    STRING[1] "HeroRandomBGM";
    STRING "BGMVolume",
    UINT32 "FightNum",
    UINT32[1] "Gender";
    STRING "ActivateDec",
    UINT32 "ActivateItemID", --unique key
    UINT32 "ActivateItemNum",
    UINT32 "ActivateJumpType",
    UINT32 "ActivateJumpParam",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[6] "Attr",
    UINT32 "HeroChangeID", --unique key
    UINT32 "HeroAppSkill",
    UINT32[4] "HeroNormalSkill";
    Struct {
        UINT32 "HeroSkillID",
        UINT32 "HeroSkill_ItemID",
        UINT32 "HeroSkill_ItemNum",
        UINT32 "SkillType",
        UINT32 "SkillFightNum",
    }[6] "SKI",
    STRING "Text",
    UINT32 "HeroScale_Tips",
    SINT32[3] "HerosPoint";
    SINT32[3] "HerosRotation";
    STRING "HeroCamera",
}



Config "dataconfig_hero_change_skill" {
    UINT32 "HeroID", --multi key
    UINT32 "HeroClass", --multi key
    UINT32 "MaxSkillNum",
    UINT32 "SkillNum",
    Struct {
        UINT32 "SkillID",
        UINT32 "SkillLevel",
        UINT32 "UnlockLevel",
    }[6] "Attr",
}



Config "dataconfig_hero_class" {
    UINT32 "HeroClass", --unique key
    STRING "Name",
    UINT32 "ResID",
    UINT32 "HeroScale",
    UINT32 "Level",
    UINT32 "StarNum",
    UINT32 "HeroBGID",
    STRING[1] "HeroBGM";
    STRING[2] "HeroRandomBGM";
    STRING "BGMVolume",
    UINT32 "HeroChangeID", --unique key
    UINT32 "HeroAppSkill",
    UINT32[4] "HeroNormalSkill";
    Struct {
        UINT32 "SkillID",
        UINT32 "SkillLevel",
        UINT32 "UnlockLevel",
    }[6] "Attr",
    STRING "Text",
    STRING "HeroCamera",
}



Config "dataconfig_hero_updateattribte" {
    UINT32 "HeroClass", --multi key
    UINT32 "Star", --multi key
    UINT32 "BlessingLimit",
    UINT32 "CostItemID",
    UINT32 "CostItemNum",
    UINT32 "FightNum",
    UINT32 "StarNum", --multi key
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[6] "Attr",
}



Config "dataconfig_heroproperty" {
    UINT32 "id", --unique key
    STRING "name",
    UINT32 "belong",
    STRING "desc",
    UINT32 "resID",
}



Config "dataconfig_homelandgive" {
    UINT32 "id",
    SINT32 "giveType",
    STRING "giftName",
    STRING "giftIcon",
    SINT32 "favor",
    SINT32 "cost",
    STRING "effect",
}



Config "dataconfig_homelandmutual" {
    UINT32 "id",
    STRING "mutualName",
    STRING "icon",
    STRING "aniName",
    SINT32 "price",
}



Config "dataconfig_homelandtopic" {
    UINT32 "id",
    STRING "topicName",
    SINT32 "startID",
}



Config "dataconfig_honorconfig" {
    UINT32 "ID", --unique key
    UINT32 "Weapon", --multi key
    UINT32 "IDNext",
    UINT32 "HonorLV", --multi key
    STRING "Name",
    STRING "color",
    STRING "Icon_3",
    STRING "Describe",
    UINT32 "DownValue",
    Struct {
        UINT32 "Type",
        UINT32 "Value",
    }[7] "Property",
    UINT32 "NeedItem",
    UINT32 "NeedNum",
    UINT32 "PromoteFighting",
    STRING "Picname",
}



Config "dataconfig_honormark" {
    UINT32 "LevelID", --unique key
    UINT32 "Mark",
}



Config "dataconfig_honorratio" {
    UINT32 "ID",
    UINT32 "attack",
    UINT32 "behit",
    UINT32 "ratio",
}



Config "dataconfig_huashancompetitionfinalarena" {
}



Config "dataconfig_huaweielectric" {
    SINT32 "id",
    STRING "modelNum", --unique key
    SINT32 "electric",
    SINT32 "hintID",
}



Config "dataconfig_idset" {
    UINT32 "id", --unique key
    UINT32 "type",
    UINT32[129] "set";
}



Config "dataconfig_initpet" {
    UINT32 "ID", --unique key
    STRING "initName",
    STRING "femaleName",
    STRING "maleName",
    UINT32[2] "Exp";
    UINT32[3] "NpcID";
    UINT32[3] "Scale";
}



Config "dataconfig_itemcompose" {
    UINT32 "TargetID", --unique key
    UINT32 "IndexID", --unique key
    UINT32 "Coin",
    UINT32 "AutoOpen",
    UINT32 "AutoCombine",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[5] "MaterialList",
    UINT32 "ButtonName",
}



Config "dataconfig_itemsource" {
    UINT32 "ID", --unique key
    UINT32[5] "cognateID";
    UINT32[8] "source";
}



Config "dataconfig_itemsourcelist" {
    UINT32 "ID", --unique key
    STRING "Name",
    STRING "Iocn",
    UINT32 "EnterID",
    UINT32 "Type",
    UINT32 "value",
    UINT32 "TimeType",
    Struct {
        UINT32 "OpenTime",
        UINT32 "EndTime",
    }[5] "childTypeList",
}



Config "dataconfig_itemtypes" {
    UINT32 "TypeID",
    STRING "Name",
    Struct {
        UINT32 "childType",
        STRING "childName",
    }[15] "childTypeList",
}



Config "dataconfig_jhll_award" {
    UINT32 "id", --unique key
    UINT32 "ItemNum",
    UINT32 "ShowBoxes",
}



Config "dataconfig_jhll_matchscorerules" {
    UINT32 "level", --unique key
    UINT32 "addscore",
    UINT32 "matchdownlimit",
    UINT32 "matchuplimit",
    UINT32 "itemnum",
    UINT32[2] "previewid";
}



Config "dataconfig_jhll_preview" {
    UINT32 "ID", --multi key
    UINT32 "Level", --multi key
    Struct {
        UINT32 "Item",
        UINT32 "num",
        UINT32 "maxNum",
    }[3] "Items",
    Struct {
        UINT32 "Item",
        UINT32 "num",
        UINT32 "maxNum",
    }[3] "RandomItems",
}



Config "dataconfig_jhll_robot" {
    UINT32 "level",
    UINT32 "robot_id",
    UINT32 "LevelID",
    UINT32 "ModelID",
    UINT32 "HeadIconId",
    UINT32 "Hp",
    UINT32 "HpSpeed",
    UINT32 "OutAttack",
    UINT32 "InterAttack",
    UINT32 "OutDefence",
    UINT32 "InterDefence",
    UINT32 "FireAttack",
    UINT32 "FireDefence",
    UINT32 "WaterAttack",
    UINT32 "WaterDefence",
    UINT32 "EarthAttack",
    UINT32 "EarthDefence",
    UINT32 "WoodAttack",
    UINT32 "WoodDefence",
    UINT32 "AllDefence",
    UINT32 "IgnoreAllDef",
    UINT32 "CritRate",
    UINT32 "AntiCritRate",
    UINT32 "CritHurt",
    UINT32 "AntiCritHurt",
    UINT32 "HitRate",
    UINT32 "Dodge",
    UINT32 "HurtToHp",
    UINT32 "WinRate",
    UINT32 "Fighting",
    UINT32 "Job",
    UINT32 "Weapon",
    UINT32 "Clothes",
    UINT32[4] "Skill1";
    UINT32[1] "Skill2";
    UINT32[1] "Skill3";
    UINT32[1] "Skill4";
    UINT32 "Honor",
}



Config "dataconfig_jinlingcompeteconfig" {
    UINT32 "ActivityID", --unique key
    STRING "ActivityName",
    STRING "Brief",
    STRING "Remarks",
    UINT32 "ActivityLevel",
    UINT32 "OpenDay",
    UINT32 "EndDay",
    UINT32 "EndMail",
}



Config "dataconfig_jinlingcompeteshow" {
    UINT32 "RewardID",
    UINT32 "ActivityID", --multi key
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNum",
    }[3] "Reward",
}



Config "dataconfig_jiuxi" {
    UINT32 "ID",
    UINT32 "JiuxiID",
    UINT32 "JiuxiCount",
}



Config "dataconfig_jjcshenshou" {
    UINT32 "LevelID", --multi key
    UINT32 "ShenShouID", --multi key
    UINT32 "Star",
    UINT32 "StarLevel",
    UINT32 "Fighting",
    UINT32[2] "Skill";
    UINT32 "StrengLevel",
    UINT32 "EnduranceLevel",
    UINT32 "QualityLevel",
    UINT32 "DexterityLevel",
    UINT32 "ShenfaLevel",
}



Config "dataconfig_judgement" {
    UINT32 "Id",
    UINT32 "Min",
    UINT32 "Max",
    STRING "Pic",
    UINT32 "Judgement",
    STRING "Level",
}



Config "dataconfig_keydescribe" {
    UINT32 "keyID",
    STRING "keyName",
    STRING "type",
    STRING "keyDesc",
}



Config "dataconfig_killnotice" {
    SINT32 "ID",
    SINT32 "KillMin",
    SINT32 "KillMax",
    SINT32 "killtips",
    SINT32 "dietips",
}



Config "dataconfig_laceyface" {
    UINT32 "ID",
    STRING "LaceyIcon",
    STRING "LaceyName",
}



Config "dataconfig_ldmjtask" {
    UINT32 "TaskID",
    STRING "TaskDescription",
    STRING "TaskHint",
}



Config "dataconfig_leagueactivitygroup" {
    UINT32 "id",
    STRING "groupName",
    UINT32[6] "activityGroup";
}



Config "dataconfig_leagueadministration" {
    UINT32 "id",
    STRING "name",
    UINT32[2] "Jurisdiction";
}



Config "dataconfig_leagueauthority" {
    UINT32 "PositionID", --unique key
    UINT32 "PowerID",
    STRING "PositionName",
    UINT32 "IsManager",
    UINT32[7] "btninfo";
    UINT32 "PositionNum",
    UINT32[1] "Selfeffect";
    UINT32[1] "Othereffect";
    UINT32 "ModifyNotice",
    UINT32 "EditEmail",
    UINT32 "ApprovalAuthority",
    UINT32[6] "Authority";
    UINT32[5] "AutorityAllow";
    UINT32 "AotuApprovalAuthority",
    UINT32[4] "SpecifyPositionAutority";
    UINT32 "BuildAuthority",
    UINT32 "UpdateAuthority",
    UINT32 "KickOffAuthority",
    UINT32 "DismissAuthority",
    UINT32 "ModifyManifesto",
    UINT32 "MoneyRate",
    UINT32 "LevelUp",
    UINT32[1] "KickOff";
}



Config "dataconfig_leagueauthoritylist" {
    UINT32 "id",
    STRING "name",
    UINT32 "type",
    UINT32 "show",
}



Config "dataconfig_leaguebutton" {
    UINT32 "ID",
    UINT32 "label",
    UINT32 "value",
    UINT32 "Sequence",
    UINT32 "NewEnum",
}



Config "dataconfig_leaguerank" {
    UINT32 "ranking", --multi key
    UINT32 "FactionType", --multi key
    UINT32 "PositionID", --multi key
    UINT32 "PreviewID",
    UINT32 "Dropteam",
}



Config "dataconfig_leaguetrain" {
    UINT32 "level", --unique key
    UINT32 "TrainTimes",
    UINT32 "BeTrainTimes",
    UINT32 "TrainContribution",
    UINT32 "TrainExp",
    UINT32 "BeTrainExp",
    UINT32 "BeNPCTrainExp",
}



Config "dataconfig_leaguetype" {
    UINT32 "SystemOpen",
    UINT32 "Open",
    UINT32 "type",
    UINT32 "Restrict",
    UINT32 "Openday",
    UINT32 "Closeday",
    Struct {
        UINT32 "ID",
    }[6] "index",
}



Config "dataconfig_leaguewelfare" {
    UINT32 "id",
    STRING "Name",
    UINT32 "Type",
    UINT32 "value",
    UINT32 "Order",
    UINT32 "Level",
    STRING "Icon",
    STRING "Opentime",
    STRING "WelfareDec",
    STRING "Button",
}



Config "dataconfig_lefttab" {
    UINT32 "TabID",
    STRING "TabName",
    UINT32 "UIType",
    Struct {
        STRING "SubTabName",
        STRING "SubTabIcon",
    }[6] "TabInfo",
}



Config "dataconfig_levelcameratomonster" {
    UINT32 "levelID", --unique key
    UINT32 "monsterID",
    SINT32[3] "pos";
    UINT32 "cameraHight",
    UINT32 "fov",
    UINT32 "isFollow",
}



Config "dataconfig_levelcarnivalreward" {
    UINT32 "Level",
    UINT32 "TargetLevel",
    Struct {
        UINT32 "PlayerNum",
        Struct {
            UINT32 "ItemID",
            UINT32 "ItemNum",
        }[2] "CommonReward",
        Struct {
            UINT32 "ItemID",
            UINT32 "ItemNum",
        }[1] "VipReward",
    }[5] "LevelActivityReward",
}



Config "dataconfig_levelclearancecondition" {
    UINT32 "id", --unique key
    STRING "LevelDes",
    BOOL "iskilledAllMaster",
    UINT32[2] "killedMasterIDList";
    BOOL "isLimitedTime",
    UINT32 "interDialogueID",
    UINT32 "asidesID",
    UINT32[1] "ProNpcID";
    UINT32 "defeatedID",
    UINT32[1] "ProMonster";
    STRING "condition1",
    STRING "condition2",
    STRING "condition3",
}



Config "dataconfig_levelconfig" {
    UINT32 "id", --unique key
    SINT32 "levelType",
    SINT32 "RecommendPersonNum",
    SINT32 "difficultyID",
    SINT32 "MapType",
    STRING "sceneName",
    STRING "levelDataName",
    STRING "name",
    UINT32 "uiType",
    SINT32 "canAutoPlay",
    SINT32 "autoPlayRange",
    UINT32 "groupID",
    STRING "sceneIcon",
    UINT32 "lifeTimeInS",
    UINT32 "allowMaxMonster",
    UINT32 "reviveMaxCount",
    SINT32[6] "bornPoint_1cm";
    SINT32[6] "bornPoint_2cm";
    SINT32[6] "bornPoint_3cm";
    SINT32[6] "bornPoint_4cm";
    UINT32 "entryConditionID",
    UINT32 "clearanceConditionID",
    UINT32 "dropGroupID",
    UINT32 "selfadaption",
    STRING "LevelDescription",
    STRING "LoadingPic",
    UINT32 "NavigationOpen",
    UINT32 "PlayEnterType",
    UINT32 "ThiefLevelOpen",
    UINT32 "ThiefLevelFirst",
    UINT32[3] "ThiefLevel";
    UINT32[3] "ThiefLevelRate";
    STRING "strMscName",
    STRING "EventName",
    STRING[7] "BankNames";
    UINT32[3] "HelpID";
    SINT32[3] "cameraPos";
    UINT32[2] "assetPathIdList";
    UINT32[3] "prehotAssetPathIdList";
    SINT32 "bossMeet",
    UINT32 "LevelExReward",
    UINT32 "SpecialChallengeType",
    UINT32[2] "SpecialChallengeParas";
    UINT32 "AngerInitialValue",
    UINT32 "IsFightLevel",
    UINT32 "PkType",
    UINT32 "PkField",
    UINT32 "CtrlResistance",
    UINT32[28] "GhostList";
    UINT32 "Pathfinding",
    UINT32 "IfCanOut",
    UINT32 "not_ride",
    UINT32 "not_runfast",
    UINT32 "SpecialRecoverHp",
    SINT32 "EvenCutNo",
    UINT32 "isShowTarget",
    SINT32 "volumPer",
    STRING "cameraPosAdjust",
    SINT32 "ShowSafety",
    SINT32 "ShowSwitch",
    SINT32 "FightImage",
    UINT32 "IsMakeTeam",
    UINT32 "IsFollowFight",
    UINT32 "IsTransmit",
    UINT32 "StrengthID",
    STRING "LevelPic",
    UINT32 "IsFollowFightIn",
    UINT32 "IsGrayScene",
    UINT32 "isShowCombo",
    UINT32 "TeamerModel",
    UINT32 "notSpeedLoading",
    UINT32 "clonedType",
    UINT32 "StartNumber",
    UINT32 "SubNumber",
    UINT32 "ClearSkillCD",
    UINT32[1] "furnitureID";
    UINT32 "SplitScene",
    UINT32 "herosUsable",
    UINT32 "bigworld",
    UINT32 "RelatedID",
    UINT32 "WarSpoil",
    UINT32 "medicine",
    UINT32 "PetBattle",
}



Config "dataconfig_levelconfig_ios" {
    UINT32 "id",
    SINT32 "levelType",
    SINT32 "RecommendPersonNum",
    SINT32 "difficultyID",
    SINT32 "MapType",
    STRING "sceneName",
    STRING "levelDataName",
    STRING "name",
    UINT32 "uiType",
    SINT32 "canAutoPlay",
    SINT32 "autoPlayRange",
    UINT32 "groupID",
    STRING "sceneIcon",
    UINT32 "lifeTimeInS",
    UINT32 "allowMaxMonster",
    UINT32 "reviveMaxCount",
    SINT32[6] "bornPoint_1cm";
    SINT32[6] "bornPoint_2cm";
    SINT32[6] "bornPoint_3cm";
    SINT32[6] "bornPoint_4cm";
    UINT32 "entryConditionID",
    UINT32 "clearanceConditionID",
    UINT32 "dropGroupID",
    UINT32 "selfadaption",
    STRING "LevelDescription",
    STRING "LoadingPic",
    UINT32 "NavigationOpen",
    UINT32 "PlayEnterType",
    UINT32 "ThiefLevelOpen",
    UINT32 "ThiefLevelFirst",
    UINT32[3] "ThiefLevel";
    UINT32[3] "ThiefLevelRate";
    STRING "strMscName",
    UINT32[3] "HelpID";
    SINT32[3] "cameraPos";
    UINT32[1] "assetPathIdList";
    UINT32[3] "prehotAssetPathIdList";
    SINT32 "bossMeet",
    UINT32 "LevelExReward",
    UINT32 "SpecialChallengeType",
    UINT32[2] "SpecialChallengeParas";
    UINT32 "AngerInitialValue",
    UINT32 "IsFightLevel",
    UINT32 "PkType",
    UINT32 "PkField",
    UINT32 "CtrlResistance",
    UINT32[26] "GhostList";
    UINT32 "Pathfinding",
    UINT32 "IfCanOut",
    UINT32 "not_ride",
    UINT32 "SpecialRecoverHp",
    SINT32 "EvenCutNo",
    UINT32 "isShowTarget",
    SINT32 "volumPer",
    STRING "cameraPosAdjust",
    SINT32 "ShowSafety",
    SINT32 "ShowSwitch",
    SINT32 "FightImage",
    UINT32 "IsMakeTeam",
    UINT32 "IsFollowFight",
    UINT32 "IsTransmit",
    UINT32 "StrengthID",
    STRING "LevelPic",
    UINT32 "IsFollowFightIn",
    UINT32 "IsGrayScene",
    UINT32 "isShowCombo",
    UINT32 "TeamerModel",
    UINT32 "notSpeedLoading",
    UINT32 "clonedType",
    UINT32 "StartNumber",
    UINT32 "SubNumber",
    UINT32 "ClearSkillCD",
    UINT32[1] "furnitureID";
}



Config "dataconfig_levelconfigextend" {
    UINT32 "id", --unique key
    UINT32 "modelHideDis",
    UINT32 "hubHideDis",
    UINT32 "hubCoverHide",
}



Config "dataconfig_leveldescription" {
    SINT32 "levelType",
    STRING "title",
    STRING "ActivityDescription",
    STRING "RewardDescription",
    STRING "titlePic",
}



Config "dataconfig_leveldropitemslist" {
    UINT32 "levelID",
    STRING "BossIcon",
    UINT32 "magnification",
    Struct {
        UINT32 "dropItemID",
        UINT32 "dropItemNumber",
    }[3] "dropItemCount",
}



Config "dataconfig_levelentrycondition" {
    UINT32 "id",
    UINT32 "requiredLevel",
    Struct {
        STRING "startTime",
        STRING "endTime",
    }[2] "EntryTimeList",
}



Config "dataconfig_levelgroup" {
    UINT32 "id", --unique key
    STRING "strGroupName",
    STRING "bgPicName",
    STRING "roadPicName",
    Struct {
        SINT32[2] "IconPlaceDate";
    }[6] "IconPlace",
    UINT32[4] "fieldLevelList";
    UINT32[32] "smallLevelList";
    UINT32 "smallLevelTotalEntryNum",
    STRING "strDesc",
    UINT32[2] "giftList";
}



Config "dataconfig_levelhelper" {
    UINT32 "Id",
    STRING "Icon",
}



Config "dataconfig_levelmail" {
}



Config "dataconfig_levelpreview" {
    UINT32 "ID",
    UINT32 "Level",
    UINT32 "ActivityID",
    UINT32[3] "Item";
}



Config "dataconfig_levelprocessconfig" {
    UINT32 "id", --unique key
    STRING "state",
    BOOL "isHide",
    UINT32 "endCountDown",
}



Config "dataconfig_levelratedesc" {
    SINT32 "Grade",
    UINT32 "Rate",
}



Config "dataconfig_levelrevive" {
    UINT32 "ID",
    UINT32 "ReviveType",
    SINT32 "specialBornPointIndex",
    SINT32 "ReviveTimeOut",
    Struct {
        UINT32 "ReviveConsumeID",
        UINT32 "ReviveConsumeValue",
    }[5] "ReviveConsumeList",
}



Config "dataconfig_levelspecialeffectinfo" {
    UINT32 "ID", --unique key
    UINT32[3] "effectList";
    UINT32[3] "para1";
}



Config "dataconfig_levelswitchresource" {
    UINT32 "id", --unique key
    UINT32 "aimLevelID",
    SINT32[3] "aimPosition";
    SINT32[3] "aimRotation";
    SINT32 "WorldMapPoint",
}



Config "dataconfig_leveltocapacity" {
    SINT32 "level",
    SINT32 "capacity",
}



Config "dataconfig_levelupreward" {
    UINT32 "Id",
    UINT32 "Unlocklevel",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[4] "AwardList",
}



Config "dataconfig_limitfashion" {
    UINT32 "ID",
    UINT32 "NeedMoney",
    UINT32 "ItemId",
    UINT32 "ItemNum",
    Struct {
        STRING "Text",
    }[5] "Dialogue",
    UINT32 "ModelID",
    UINT32 "ModID",
    STRING "PictureID",
}



Config "dataconfig_limititem" {
    UINT32 "ID",
    UINT32 "ServerLevel",
}



Config "dataconfig_linedata" {
    UINT32 "id", --unique key
    UINT32 "StartPointId",
    STRING "StartPointName",
    UINT32 "EndPointId",
    STRING "EndPointName",
    UINT32 "AwardTime",
    UINT32 "RewardId",
    UINT32 "SpeedRewardId",
}



Config "dataconfig_lingshi" {
    UINT32 "ID", --unique key
    UINT32[4] "TypeID";
    UINT32 "QuaID",
    UINT32 "ComposeLevel",
    Struct {
        UINT32 "key",
        UINT32 "value",
    }[3] "AddAttrInfo",
    UINT32 "ExtrID",
    UINT32 "UninstallCostID",
    UINT32 "UninstallCost",
    UINT32 "MaxNumber",
    UINT32 "Branch",
    UINT32 "NextLevelID",
    UINT32 "Fighting",
    UINT32 "Type",
}



Config "dataconfig_lingshigrade" {
    UINT32 "ID", --multi key
    UINT32 "Level",
    UINT32 "Num",
    UINT32 "Weapon", --multi key
    Struct {
        UINT32 "key",
        UINT32 "value",
    }[5] "AddAttrInfo",
    UINT32 "Fighting",
}



Config "dataconfig_lingshirecommend" {
    UINT32 "Job", --multi key
    UINT32 "TypeID", --multi key
    UINT32 "ZoneLevel",
    Struct {
        UINT32 "LingShiID",
        UINT32[2] "LingshiList";
    }[3] "RecommendList",
}



Config "dataconfig_localchinese1" {
    SINT32 "ID",
    STRING "LanguageStr",
}



Config "dataconfig_lunhui" {
    UINT32 "Index",
    UINT32 "ID", --unique key
    UINT32 "MinRank",
    UINT32 "MaxRank",
    UINT32 "OpenLevel",
    UINT32 "OpenDay",
    UINT32 "CostDay",
    Struct {
        UINT32 "ItemID",
        UINT32 "ItemNum",
    }[4] "RewardList",
    UINT32 "AwardTitle",
    UINT32 "TextID",
}



Config "dataconfig_lventryopen" {
    UINT32 "ID", --unique key
    STRING "OpenName",
    UINT32 "OpenLevel",
    STRING "OpenIconName",
    UINT32 "IsOpenShow",
    UINT32 "Determine",
    UINT32 "Openday",
}



Config "dataconfig_lventryopen_ios" {
    UINT32 "ID",
    STRING "OpenName",
    UINT32 "OpenLevel",
    STRING "OpenIconName",
    UINT32 "IsOpenShow",
    UINT32 "Determine",
    UINT32 "Openday",
}



Config "dataconfig_mailtemplate" {
    UINT32 "Type", --unique key
    STRING "Title",
    STRING "Content",
    STRING[2] "tag";
    STRING[2] "indexID";
}



Config "dataconfig_maincity" {
    UINT32 "id",
    STRING "sceneName",
    STRING "name",
}



Config "dataconfig_maincitysecretnewsdata" {
    SINT32 "ID",
    SINT32 "Type",
    STRING "TitleSpriteName",
    STRING[4] "textContents";
    STRING "textStaytime",
    SINT32 "HearsayProbability",
}



Config "dataconfig_makeup" {
    UINT32 "ID",
    UINT32 "Type",
    UINT32 "AddRemove",
    UINT32 "offsetId",
    UINT32 "ModelID",
    STRING "path",
    STRING "MenuSpriteName",
    UINT32 "MeshID",
    UINT32 "width",
    UINT32 "height",
}



Config "dataconfig_makeupoffset" {
    UINT32 "ID",
    SINT32 "Xoffset",
    SINT32 "Yoffset",
}



Config "dataconfig_malename" {
}



Config "dataconfig_mall" {
    UINT32 "ID", --unique key
    STRING "MallName",
    UINT32 "Num",
    UINT32[2] "IDList";
}



Config "dataconfig_marrysystem" {
    UINT32 "MarrySystemType", --multi key
    UINT32 "ID", --multi key
    STRING "Name",
    UINT32 "ConditionTextID",
    UINT32 "AwardTextID",
    UINT32 "RoleGrade",
    UINT32 "IntimacyGrade",
    UINT32 "CPIntimacy",
    Struct {
        UINT32 "NeedID",
        UINT32 "NeedNum",
    }[2] "NeedItems",
    UINT32 "DressID",
    UINT32 "HairID",
    UINT32 "StepID",
    UINT32 "TitleID",
    STRING "EffectsName",
    Struct {
        UINT32 "Type",
        UINT32 "Param1",
        UINT32 "Param2",
    }[10] "AwardList",
    UINT32 "InvationCardCount",
    UINT32 "XiTangCount",
    UINT32 "RedHandID",
    UINT32 "PreviewID",
    UINT32 "LanguageID",
}



Config "dataconfig_mask" {
    UINT32 "ServerNumber", --unique key
    UINT32 "IsUnionMask",
    UINT32 "TipCount",
    UINT32 "BanLookPlayer",
    Struct {
        UINT32 "ActivityID",
        UINT32 "ServerType",
        UINT32 "levelType",
    }[1] "MaskLists",
}



Config "dataconfig_masterlevel" {
    UINT32 "ZoneLevel", --unique key
    UINT32 "Level",
}



Config "dataconfig_materialscore" {
    UINT32 "ID", --unique key
    UINT32 "Gid",
    UINT32 "KillMax",
    UINT32 "KillMin",
    UINT32 "content",
    UINT32 "dropGroupID",
}



Config "dataconfig_meetanswer" {
    UINT32 "ID", --unique key
    STRING "desc",
    STRING "AnswerWord",
}



Config "dataconfig_mingjiang" {
    UINT32 "ID", --unique key
    UINT32 "GroupID", --multi key
    UINT32 "Severlevel",
    UINT32 "LevelID",
    UINT32 "Area",
    UINT32 "BossID", --unique key
    STRING "BossName",
    STRING "AreaName",
    UINT32 "ShenshouID",
    UINT32 "ShenshouSize",
    STRING "Album",
    STRING "Icon",
    STRING "xdt_icon",
    UINT32[2] "offset";
    STRING "TexturePath",
    UINT32 "Bonus",
    UINT32 "UintScore",
}



Config "dataconfig_mingjiangarea" {
    UINT32 "ID",
    UINT32[8] "Area";
    UINT32[3] "SparePoints";
    UINT32 "Birth",
    UINT32 "InternationalID",
}



Config "dataconfig_mingjiangtips" {
    UINT32 "ID", --unique key
    UINT32 "TipsType", --multi key
    UINT32 "Stage", --multi key
    UINT32 "Area",
    UINT32 "BossHp",
    STRING "Content",
    UINT32 "StageTime",
}



Config "dataconfig_minitab" {
    UINT32 "TabID",
    STRING "TabName",
    UINT32 "SpeakAllow",
    UINT32 "SpeakChannel",
    UINT32 "Error",
}



Config "dataconfig_missionqueue" {
    UINT32 "ID", --multi key
    UINT32 "Tittle",
    UINT32 "FirstTxtID",
    UINT32 "SecondType",
    UINT32 "SecondTxt",
    UINT32 "ButtonID",
    STRING "ButtonTxt",
    UINT32 "RedDot",
    UINT32 "status", --multi key
}



Config "dataconfig_moduleswitch" {
    UINT32 "id", --unique key
    UINT32 "state",
    STRING "DirPath",
    STRING "FilePath",
    STRING "IgnorePath",
}



Config "dataconfig_monsterfightattr" {
    UINT32 "ID",
    UINT32 "AttrType",
    UINT32 "levelType",
    UINT32 "level",
    STRING "exp",
    STRING "Hp",
    STRING "OutAttack",
    STRING "OutDefence",
    STRING "InterAttack",
    STRING "InterDefence",
    STRING "FireAttack",
    STRING "FireDefence",
    STRING "WaterAttack",
    STRING "WaterDefence",
    STRING "EarthAttack",
    STRING "EarthDefence",
    STRING "WoodAttack",
    STRING "WoodDefence",
    STRING "CritRate",
    STRING "AntiCritRate",
    STRING "HitRate",
    STRING "Dodge",
}



Config "dataconfig_monstergemconfig" {
    UINT32 "ID", --unique key
    UINT32 "Position", --multi key
    UINT32 "Quality",
    UINT32 "ComposeLevel",
    Struct {
        UINT32 "AttrType",
        UINT32 "Value",
    }[3] "AddAttrInfo",
    UINT32 "NextLevelID",
    UINT32 "Fighting",
    UINT32 "Identification",
}



Config "dataconfig_monstergemnum" {
    UINT32 "Level",
    UINT32 "Number1",
    UINT32 "Number2",
    UINT32 "Number3",
    UINT32 "Number4",
    UINT32 "Number5",
}



Config "dataconfig_monstergemre" {
    UINT32 "Job", --multi key
    UINT32 "ZoneLevel", --multi key
    UINT32 "Position", --multi key
    Struct {
        UINT32 "LingShiID",
        UINT32[3] "LingShiList";
    }[5] "RecommendList",
}



Config "dataconfig_monsternote" {
    SINT32 "id", --unique key
    STRING "info",
    UINT32 "type",
}



Config "dataconfig_monsterproperty" {
    UINT32 "id", --unique key
    STRING "name",
    UINT32 "type",
    UINT32 "dropType",
    UINT32 "dropteamID",
    BOOL "IsCanBattle",
    UINT32[2] "targetPriority";
    UINT32 "restoreTime",
    UINT32 "belong",
    UINT32 "influence",
    STRING "desc",
    UINT32 "resID",
    UINT32 "speed",
    UINT32 "walkSpeed",
    SINT32 "acce",
    UINT32 "BulletMoveID",
    UINT32 "hitAwayProbability",
    UINT32 "monsterHpBarEffect",
    UINT32 "hpLevelNum",
    UINT32 "dropModeID",
    SINT32 "guardRange",
    STRING "AI",
    UINT32 "guardID",
    UINT32 "priorTargetType",
    UINT32 "priorTargetParam",
    UINT32 "attackInternal",
    UINT32[8] "skilllist";
    UINT32[8] "releaseWeight";
    UINT32[8] "HpLimit";
    UINT32[8] "LowHpLimit";
    UINT32[8] "oneTimeSkillList";
    UINT32[8] "oneTimeLimit";
    UINT32[2] "scenarioLevelID";
    UINT32[2] "scenarioActionType";
    UINT32[2] "scenarioParam";
    UINT32[5] "comboSkills";
    UINT32 "targetTypeVisible",
    STRING "intervalVisible",
    UINT32 "trigTimesVisible",
    UINT32 "trigTimeVisible",
    UINT32 "triggerTarget",
    UINT32 "triggerType",
    UINT32 "existTime",
    UINT32 "triggerTimes",
    UINT32 "destroyCondition",
    UINT32 "disappearTime",
    UINT32[5] "specialSkills";
    UINT32 "attributeFrom",
    UINT32 "radius",
    UINT32 "scaleFactor",
    UINT32 "intervalTime",
    UINT32 "summonMaxNum",
    UINT32 "DistReferncePoint",
    UINT32 "existDist",
    BOOL "StopInFirstTriggered",
    UINT32 "fightAttrID",
    SINT32 "deviation",
    SINT32 "BatiBuff",
    SINT32 "BatiValue",
    SINT32 "BaitiRecoverInterval",
    STRING "strMscName",
    STRING "strPartClr",
    SINT32 "bossInfo",
    STRING "strCMRPath",
    BOOL "hideHud",
    UINT32[8] "HPTrigger";
    UINT32[8] "HPEvenType";
    SINT32[8] "HPEvenParam1";
    SINT32[3] "skillTrigger";
    UINT32[3] "SkillEvenType";
    SINT32[3] "SkillEvenParam1";
    SINT32[3] "SkillTriggerTimes";
    UINT32 "Score",
    UINT32 "Right",
    UINT32 "triggerRadius",
    UINT32[1] "PickedNum";
    UINT32[1] "PickedTime";
    UINT32 "ChaseDis",
    STRING "MiniIcon",
    UINT32 "MiniIconAngle",
    SINT32[1] "skillNeedNotify";
    SINT32 "TargetType",
    SINT32 "TargetParam",
    SINT32 "SpecialBorn",
    SINT32 "SpecialStand",
    SINT32 "BornEffect",
    SINT32 "MonsterBornEffect",
    SINT32 "DeathEffect",
    SINT32 "IsHeadName",
    STRING "BossIcon",
    UINT32 "BrokenScreen",
    UINT32 "IsAttackedRun",
    UINT32 "MonsterNote",
    BOOL "IsFollowMaster",
    UINT32[1] "AIBuffTargetList";
    UINT32[1] "AIBuffIDList";
    UINT32 "AIRunSkill",
}



Config "dataconfig_monsterspeak" {
    SINT32 "id", --unique key
    STRING "info",
}



Config "dataconfig_mountbaseconfig" {
    UINT32 "MountID", --unique key
    UINT32 "MountGroup", --multi key
    UINT32 "RelationGroup",
    STRING "Name",
    STRING "Atlas",
    STRING "ActivityIcon",
    UINT32 "MountResID", --unique key
    UINT32 "MountPosition",
    UINT32 "MountQuality",
    UINT32 "StarNum", --multi key
    UINT32 "MaxStar",
    UINT32 "StarLevel", --multi key
    UINT32 "MaxStarLevel",
    Struct {
        UINT32 "Key",
        UINT32 "Value",
    }[5] "AttrList",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialCost",
    }[2] "StarUpCost",
    UINT32 "MountIDAfterStarUp",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialCost",
    }[2] "NirvanaCost",
    UINT32 "MountIDAfterNirvana",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialReturn",
    }[2] "RepiReturn",
    Struct {
        UINT32 "ReboMaterialID",
        UINT32 "ReboMaterialReturn",
    }[2] "ReboReturn",
    UINT32 "ReboCost",
    UINT32 "ReboID",
    Struct {
        UINT32 "NirMaterialID",
        UINT32 "NirMaterialReturn",
    }[2] "NirReturn",
    UINT32 "FightingEffect",
    UINT32 "StarCostValue",
}



Config "dataconfig_mountcollect" {
    UINT32 "MountGroup", --unique key
    UINT32 "CollectWeight",
}



Config "dataconfig_mountrealationconfig" {
    UINT32 "RelationTeam", --multi key
    STRING "Des",
    STRING "Desc",
    STRING "Atlas",
    STRING "Icon",
    UINT32 "MountInGroupNeedNum",
    Struct {
        UINT32 "Key",
        UINT32 "Value",
    }[2] "AttrList",
    UINT32 "EnableSkill",
    UINT32 "ActiveLevel",
}



Config "dataconfig_mountrealationgroupconfig" {
    UINT32 "RelationGroup", --unique key
    STRING "Des",
    UINT32[9] "RelationTeam";
}



Config "dataconfig_mountsoulrecommendgroup" {
    UINT32 "CircleSitID",
    UINT32 "Level",
    UINT32 "RoleType",
    UINT32 "MountSoulRecommend",
}



Config "dataconfig_mountunlockedconfig" {
    UINT32 "MountGroup", --multi key
    UINT32 "TotalPhase",
    UINT32 "UnlockedPhase", --multi key
    UINT32 "PhaseTotalStarNum",
    UINT32 "UnlockedPhaseStarNum", --multi key
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialCost",
    }[2] "UnlockCost",
    UINT32 "MaxStarAttr",
    UINT32 "AttrProduct",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialReturn",
    }[2] "RelockCostReturn",
    UINT32 "UnlockedCostValue",
    UINT32 "PowerProduct",
}



Config "dataconfig_multilevelaccount" {
    SINT32 "levelType", --unique key
    STRING "levelName",
    SINT32 "baseMark",
    SINT32 "fullPassMarkTime",
    SINT32 "passTimeFullMark",
    SINT32 "passMarkTimeMax",
    SINT32 "completeMark",
    SINT32 "peerFriendMark",
    SINT32 "addFriendMark",
    SINT32 "sameFactionMark",
    SINT32 "applayFactionMark",
    SINT32 "inviteMark",
    STRING "bottonPrompt",
}



Config "dataconfig_multilevelaccountintimacygrad" {
    SINT32 "MinIntimacyGrad",
    SINT32 "Multiple",
}



Config "dataconfig_multilevelaccountmarkgrad" {
    SINT32 "Grad",
    SINT32 "MinMark",
    SINT32 "ExpIncrease",
    SINT32 "IntimacyIncrease",
}



Config "dataconfig_musicaction" {
    UINT32 "ID",
    UINT32 "ModelID", --multi key
    UINT32 "ActionType", --multi key
    UINT32 "SkillID",
}



Config "dataconfig_musicgame" {
    SINT32 "ID", --unique key
    STRING "MusicName",
    UINT32 "MusicLevel",
    UINT32 "MusicSpeed",
    UINT32 "MusicTime",
    UINT32 "BestBaseScore",
    UINT32 "GoodBaseScore",
    UINT32 "NormalBaseScore",
    UINT32 "BestBorder",
    UINT32 "GoodBorder",
    UINT32 "UIEffectValue",
    UINT32 "HumanEffectValue",
    UINT32 "CValue",
    UINT32 "BValue",
    UINT32 "AValue",
    UINT32 "SValue",
}



Config "dataconfig_needlegame" {
    UINT32 "ID", --unique key
    UINT32[2] "angle";
}



Config "dataconfig_newjhll" {
    UINT32 "ID",
    UINT32 "LevelID",
    UINT32 "OpenDay",
    STRING "MapResName",
    UINT32[3] "Rewards";
}



Config "dataconfig_newplay" {
    UINT32 "ID",
    STRING "Picture",
    UINT32 "isdisplay",
}



Config "dataconfig_newspecialfastion" {
    UINT32 "Index",
    UINT32 "Id",
    UINT32[10] "Unlocklevel";
    UINT32 "ItemId",
    UINT32 "Count",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[8] "Gift",
    UINT32 "Type",
    STRING "Textdisplay",
    UINT32[2] "Group";
    SINT32 "Y",
    UINT32 "VIPmodID",
    STRING "VIPmodContrast",
    UINT32 "ModID",
    STRING "Buttons",
    UINT32 "HorseID",
    UINT32 "IsRecharge",
    UINT32 "PayType",
    UINT32 "PayId",
    UINT32 "YuanbaoPrice",
    UINT32 "Discount",
    UINT32 "PurchaseTimes",
    UINT32 "RechargeId",
    UINT32 "MarqueeID",
    UINT32[2] "SortID";
    UINT32 "IsHaveSkillBook",
    UINT32 "YuanbaoValue",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[2] "SkillBook",
    STRING "StartTime",
    STRING "EndTime",
    UINT32 "IsFestival",
}



Config "dataconfig_newspecialfastion_ios" {
    UINT32 "Index",
    UINT32 "Id",
    UINT32[9] "Unlocklevel";
    UINT32 "ItemId",
    UINT32 "Count",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[8] "Gift",
    UINT32 "Type",
    STRING "Textdisplay",
    UINT32[1] "Group";
    UINT32 "VIPmodID",
    STRING "VIPmodContrast",
    UINT32 "ModID",
    STRING "Buttons",
    UINT32 "HorseID",
    UINT32 "IsRecharge",
    UINT32 "PayType",
    UINT32 "PayId",
    UINT32 "YuanbaoPrice",
    UINT32 "Discount",
    UINT32 "PurchaseTimes",
    UINT32 "RechargeId",
    UINT32 "MarqueeID",
    UINT32[1] "SortID";
    UINT32 "IsHaveSkillBook",
    UINT32 "YuanbaoValue",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[2] "SkillBook",
    STRING "StartTime",
    STRING "EndTime",
    UINT32 "IsFestival",
}



Config "dataconfig_nightscene" {
    UINT32 "ID", --unique key
    UINT32 "Type",
    UINT32 "MonsterID",
    STRING "Name",
    STRING "Atlas",
    STRING "Icon",
    UINT32 "QuaID",
    STRING "SummonID",
    UINT32 "Time",
    SINT32[3] "Pos";
    SINT32[3] "Rotate";
    SINT32[3] "Scale";
}



Config "dataconfig_notchadapter" {
    SINT32 "id",
    STRING "brand",
    STRING "modelNum",
    SINT32 "phoneWidth",
    SINT32 "phoneHeight",
    SINT32 "notchWidth",
    SINT32 "rightAdaptWidth",
}



Config "dataconfig_notice" {
    UINT32 "ID", --unique key
    UINT32 "type",
    UINT32 "ShowChat",
    STRING "txt",
    UINT32 "sort",
    STRING[3] "tag";
    STRING[3] "indexID";
}



Config "dataconfig_noticeindex" {
    UINT32 "id", --unique key
    STRING "configName",
    STRING "fieldName",
    STRING "idName",
    BOOL "NeedReadColor",
    BOOL "IsChangeChinese",
    BOOL "IsNeedShenShouMap",
    UINT32 "BUIndex",
}



Config "dataconfig_noticetips" {
    UINT32 "ID",
    STRING "txt",
    STRING[1] "tag";
    STRING[1] "indexID";
}



Config "dataconfig_notifytemplate" {
    UINT32 "Type", --unique key
    UINT32 "NotifyType",
    UINT32 "CancelTime",
    STRING "Title",
    STRING "Content",
    STRING "Button",
    STRING[1] "tag";
    STRING "indexID",
    UINT32 "eventType",
}



Config "dataconfig_npcaction" {
    UINT32 "ID", --unique key
    UINT32 "NpcID",
    UINT32 "Action",
    UINT32 "Move",
    UINT32 "Loop",
    UINT32[6] "TargetLocation";
    UINT32 "MoveSpeed",
    UINT32 "AnimSpeed",
}



Config "dataconfig_npcinfo" {
    UINT32 "ID", --unique key
    STRING "Name",
    STRING "NpcTitle",
    STRING "MapTitle",
    SINT32 "MapOrder",
    STRING "NamePostion",
    SINT32 "MapShow",
    Struct {
        UINT32[12] "levelIDList";
        UINT32[3] "DialogueIDList";
        UINT32[1] "StoryDialogueList";
    }[5] "levelDialogueList",
    UINT32[11] "NPCActList";
    UINT32[1] "FightNPCActList";
    STRING "RunSpeed",
    STRING "RunAnimSpeed",
    STRING "Stand",
    STRING "Talk",
    STRING "Special01",
    STRING[6] "bubbleTexts";
    UINT32 "ResID",
    STRING "iconName",
    SINT32[1] "MoodLocation";
    UINT32 "interactType",
    BOOL "stateExtraControl",
    BOOL "canRotate",
    SINT32[9] "cameraAdjust";
    UINT32 "noCameraAnim",
    UINT32 "PatrolType",
    UINT32 "HighModel",
    Struct {
        STRING "soundPath",
    }[4] "soundList",
    UINT32 "SoundSpac",
    UINT32 "SoundFlag",
    UINT32 "Sharp",
    UINT32 "SpecialNpc",
    UINT32 "SpecialState",
}



Config "dataconfig_npcmissionfenbao" {
    SINT32 "Name", --unique key
    SINT32 "Pakage",
}



Config "dataconfig_npctalk" {
    UINT32 "NPCid", --unique key
    STRING "Text1",
    STRING "Text2",
    STRING "Text3",
    STRING "Text4",
    STRING "Text5",
    Struct {
        UINT32 "TaskID",
        UINT32 "LevelID",
        STRING "Text",
    }[2] "NPCTalkinfo1",
}



Config "dataconfig_offresource" {
    UINT32 "id", --unique key
    STRING "CityName",
    Struct {
        SINT32 "ItemID",
        SINT32 "Num",
    }[1] "ImpResource",
}



Config "dataconfig_oldseverprotect" {
}



Config "dataconfig_onlinereward" {
    UINT32 "Id",
    UINT32 "Unlocklevel",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[4] "AwardList",
}



Config "dataconfig_optionsmenu" {
    UINT32 "optionsID", --unique key
    UINT32 "YiChuID",
    STRING "chbeizhu",
    STRING "titleName",
    STRING "spriteName",
    STRING "atlas",
    UINT32 "operationType",
    UINT32 "TetureType",
    UINT32 "twin",
    STRING "prefabPath",
    UINT32[1] "ColorMaterial";
    STRING "meshPath",
    UINT32 "offsetId",
    STRING "texturesPath",
    UINT32 "textureswidth",
    UINT32 "texturesheight",
    STRING "colorPath",
    UINT32 "forbid",
    UINT32 "TcolorValue",
    UINT32 "TsaturationValue",
    UINT32 "TalphaValue",
    UINT32[2] "changeBoneID";
    STRING[4] "changeBoneIDlabel";
    UINT32[3] "TRSchange";
    STRING[3] "TRSlabel";
}



Config "dataconfig_outwardconf" {
    UINT32 "OutWardID", --unique key
    UINT32 "CreateRole",
    UINT32 "OutWardType",
    UINT32[3] "dynamicBoneId";
    UINT32 "OutWardSubType",
    UINT32 "IsRecommend",
    UINT32[3] "Gender";
    STRING "Atlas",
    UINT32 "OutWardLevel",
    STRING "IconID",
    STRING "ModelFile",
    STRING "HairModel",
    STRING "EffectFile",
    UINT32[1] "EffectPath";
    STRING "OutWardDes",
    UINT32 "SuitID",
    UINT32 "CornerType",
    UINT32 "LimitTime",
    UINT32 "FightNum",
    Struct {
        UINT32 "PropertyID",
        UINT32 "Value",
    }[5] "Property",
    STRING "UnlockLimitTime",
    UINT32 "OpenDay",
    STRING "OpenDayClock",
    UINT32 "CharmNum",
    UINT32 "GoodsID",
    UINT32[1] "effectID";
    UINT32 "OpenServerLevel1",
    UINT32 "UnlockID1",
    UINT32 "OpenServerLevel2",
    UINT32 "UnlockID2",
    UINT32[10] "DyeID";
    UINT32 "CanGift",
}



Config "dataconfig_outwarddye" {
    UINT32 "DyeID", --unique key
    STRING "DyeName",
    UINT32 "DyeGrade",
    STRING "AtlasName",
    STRING "Icon",
    STRING "Body",
    STRING "HairModel",
    STRING "SpecialModel",
    UINT32 "CharmNum",
    UINT32 "FightNum",
    Struct {
        UINT32 "PropertyID",
        UINT32 "Value",
    }[5] "Property",
    STRING "MtrName",
    UINT32 "GoodsID",
    UINT32 "CanGift",
    UINT32[1] "effectID";
    UINT32 "CornerType",
    UINT32 "DiscountRatio",
    UINT32 "DiscountPeriod",
    UINT32 "UnlockServerGrade1",
    UINT32 "UnlockID1",
    UINT32 "UnlockServerGrade2",
    UINT32 "UnlockID2",
}



Config "dataconfig_outwardsuitconf" {
    UINT32 "OutWardSuitID", --unique key
    UINT32[1] "Gender";
    STRING "Atlas",
    STRING "OutWardSuitIconID",
    UINT32 "OutWardLevel",
    UINT32 "ShowAdornID",
    UINT32 "ShowClothesID",
    UINT32 "ShowHairID",
    UINT32 "ShowWeaponID",
    STRING "OutWardDes",
    Struct {
        UINT32 "PropertyID",
        UINT32 "Value",
        UINT32 "FightNum",
    }[5] "Property",
    Struct {
        UINT32 "Value",
    }[3] "EffectGrp",
    UINT32 "ShowFootID",
    UINT32 "ShowTitelID",
    UINT32 "WaistID",
    UINT32 "CharmNum",
    UINT32 "SuitNum",
    UINT32 "ResID",
    UINT32 "FairyID",
    STRING "UnlockLimitTime",
    UINT32 "OpenDay",
    STRING "OpenDayClock",
    UINT32 "openserverlevel1",
    UINT32 "UnlockId1",
    UINT32 "openserverlevel2",
    UINT32 "UnlockId2",
}



Config "dataconfig_outwardtablist" {
    UINT32 "TabID",
    STRING "TabName",
    UINT32 "TabLevel",
    UINT32 "ParentTabLevel",
    UINT32 "SameLevelSort",
    UINT32 "IsShield",
}



Config "dataconfig_outwardunitposition" {
    UINT32 "UnitID", --multi key
    STRING "UnitName",
    UINT32 "ActorMode", --multi key
    SINT32 "PosX",
    SINT32 "PosY",
    SINT32 "PosZ",
    SINT32 "AngleX",
    SINT32 "AngleY",
    SINT32 "AngleZ",
    SINT32 "ModelAngleX",
    SINT32 "ModelAngleY",
    SINT32 "ModelAngleZ",
}



Config "dataconfig_outwardunlockcondition" {
    UINT32 "UnLockID", --unique key
    STRING "UnLockDesc",
    STRING "FromDesc",
    UINT32 "nUnLockType",
    UINT32 "nParam1",
    UINT32 "nParam2",
    UINT32 "discount",
}



Config "dataconfig_passiveskillconfig" {
    UINT32 "nPassiveSkillID", --unique key
    STRING "szPassiveSkillName",
    UINT32 "nLevel",
    UINT32 "nWeaponType",
    UINT32[3] "BuffSet";
    STRING "szDesc",
    STRING "skillEffectDesc",
    STRING "szPictureAtlas",
    STRING "szPicture",
    Struct {
        UINT32 "nParamID",
        SINT32 "nParamNum",
        SINT32 "nStepID",
    }[2] "paramList",
    UINT32 "condition",
    UINT32 "EnemyEffectID",
    UINT32 "FriendEffectID",
    UINT32 "SelfEffectID",
    UINT32 "NpcID",
    SINT32 "LevelUpType",
}



Config "dataconfig_personalquestion" {
    UINT32 "ID", --unique key
    STRING "Question",
    UINT32 "RightAnswer",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
}



Config "dataconfig_personalquestionconfig" {
}



Config "dataconfig_petbattle" {
    UINT32 "ID", --unique key
    UINT32 "SwitchCD",
    SINT32 "RevivalCD",
    UINT32 "AutoBattleCD",
    UINT32 "DisplayTime",
    UINT32 "ChangeCD",
    UINT32[1] "ResetCD";
    UINT32 "WarningRange",
    SINT32[3] "Position1";
    SINT32[3] "PositionSpecial";
    SINT32[3] "Position2";
    SINT32[3] "Position3";
    SINT32[3] "Position4";
    SINT32[3] "Position5";
    UINT32 "Distance1",
    UINT32 "Distance2",
    UINT32 "Distance3",
    UINT32 "Distance4",
    UINT32 "SpeedUp",
    UINT32 "StartFight",
    UINT32 "ShenShouFormation",
}



Config "dataconfig_petchat" {
    UINT32 "ID",
    STRING "Text",
    UINT32 "parameter",
    UINT32 "default",
    UINT32 "ActionEnum",
}



Config "dataconfig_petexpadd" {
    SINT32 "addType",
    STRING "desc",
}



Config "dataconfig_petfeed" {
    SINT32 "FoodID",
    SINT32 "AddExp",
}



Config "dataconfig_photoalbum" {
    UINT32 "ID",
    STRING "SceneName",
    UINT32 "Lock",
    UINT32 "Type",
    STRING "PictureRes",
}



Config "dataconfig_picking" {
    UINT32 "ID",
    UINT32 "levelID",
    UINT32 "FunID",
    UINT32 "FunParam",
    Struct {
        UINT32 "startIdx",
        UINT32 "endIdx",
        UINT32 "Monsterid",
    }[3] "dropInfo",
    STRING "text",
}



Config "dataconfig_playerfenbao" {
    STRING "Player",
    UINT32 "Pakage",
}



Config "dataconfig_playernum" {
    UINT32 "ID", --unique key
    UINT32 "StandardNum",
    UINT32 "MinNum",
    UINT32 "MaxNum",
    UINT32 "HPBarEffect",
}



Config "dataconfig_playsonmap" {
    SINT32 "PlaysID", --unique key
    STRING "PlaysName",
    SINT32 "isShowOnWorld",
    STRING "IconForWorld",
    SINT32 "isShowOnLevel",
    STRING "IconForLevel",
    SINT32 "isShowOnPoint",
    STRING "IconForPoint",
    SINT32 "isShowOnAdvenarea",
    STRING "IconForAdvenarea",
    SINT32 "isShowOnSecret",
    STRING "IconForSecret",
    SINT32 "isShowOnHall",
    STRING "IconForHall",
    STRING "DescOfPlay",
    STRING "RewardShowID",
    STRING "NPCmodle",
}



Config "dataconfig_powerinterface" {
    UINT32 "powerID", --unique key
    STRING "powerName",
    UINT32 "powerList",
    UINT32 "powerLink",
}



Config "dataconfig_powerpage" {
    UINT32 "LevelID", --multi key
    UINT32 "Power",
    UINT32 "AddPower",
    UINT32 "IntensifyPower",
    UINT32 "GemPower",
    UINT32 "AnimalPower",
    UINT32 "TitlePower",
    UINT32 "ClothingPower",
    UINT32 "WeaponPower",
    UINT32 "AnimalLevel",
    UINT32 "AnimalIntensify",
    UINT32 "AnimalStar",
    UINT32 "AnimalSkill",
    UINT32 "AnimalFormation",
    UINT32 "EvaluationLevel",
    STRING "PowerDesc",
    STRING "EvaluationDesc",
}



Config "dataconfig_preupdateaward" {
    UINT32 "ActivitiesID",
    UINT32 "LvLimits",
    UINT32 "ID",
    STRING "BinaryVersion",
    UINT32 "DropID",
}



Config "dataconfig_professionstate" {
    UINT32 "id", --multi key
    STRING "desc",
    UINT32 "SexModeID", --multi key
    UINT32 "modeID", --multi key
    UINT32 "atkType",
    UINT32 "elemType",
    STRING "fightState",
    STRING "UIState",
    STRING "runSound",
    STRING "fastRunSound",
    UINT32 "CityWeaponPoint",
    SINT32[3] "BackPosOffset";
    UINT32 "FightWeaponPoint",
    SINT32[3] "WeaponScale";
    STRING "EndCmrAni",
    Struct {
        UINT32 "effectGua",
        STRING "EndCmrEff",
    }[3] "EffectGrp",
    SINT32[3] "WingScale";
    SINT32[3] "WingPosOffset";
}



Config "dataconfig_protectplayerhaemorrhage" {
    UINT32 "ID",
}



Config "dataconfig_punctuation" {
}



Config "dataconfig_pvpexp" {
    UINT32 "LVID",
    STRING "Name",
    UINT32 "EXP",
}



Config "dataconfig_qianjiaward" {
    UINT32 "ID",
    STRING "Dec",
    UINT32 "DownLimit",
    UINT32 "UpLimit",
    UINT32 "DropID",
}



Config "dataconfig_qianjiawardcount" {
    UINT32 "ID", --unique key
    UINT32 "basescore",
    UINT32 "survivalscore",
    UINT32 "timescore",
    UINT32 "killscore",
    UINT32 "fullscore",
}



Config "dataconfig_qualitysetting" {
    UINT32 "quality", --unique key
    UINT32 "shadow",
    UINT32 "audio_num",
    UINT32 "effect_num",
    UINT32 "effect_level",
    UINT32 "skille_ffect_level",
    UINT32 "sceneEffect_hide",
    UINT32 "resetDataVisible",
    UINT32 "sceneObj_hide",
    UINT32 "disturEffectsLevel",
    UINT32 "cameraCuttingDistance",
    UINT32 "water",
    UINT32 "showModelNum",
    UINT32 "showModelMax",
    UINT32 "isHighlightNew",
    UINT32 "isHighlightQinHuang",
    UINT32 "isHighlightUnderPalace",
    UINT32 "antiAliasing",
    UINT32 "dialogueQuaNPC",
    UINT32 "attackBossEffect",
    UINT32 "resolution",
    UINT32 "unityQuality",
    UINT32 "riderHighEffect",
    UINT32 "RadialBlur",
    UINT32 "Dissolve",
    UINT32[4] "IsDynamicBone";
    UINT32 "isPostProcess",
    UINT32 "Dist",
    UINT32 "farDist",
    UINT32 "mostFarDist",
    UINT32 "shaderLod",
    UINT32 "ModelLod",
}



Config "dataconfig_questionlist" {
    UINT32 "ID", --unique key
    STRING "Question",
    UINT32 "RightAnswer",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
}



Config "dataconfig_questionnaire" {
    SINT32 "OpenLv",
    SINT32 "Clicktimes",
    STRING "LinkDress",
}



Config "dataconfig_random" {
    UINT32 "ID", --unique key
    STRING "Desc",
    UINT32 "Times",
    UINT32 "Sharing",
}



Config "dataconfig_raremonster" {
    UINT32 "ID", --unique key
    UINT32 "Type",
    UINT32 "PurchaseItem",
    UINT32 "ConsumeType",
    UINT32 "ConsumeNum",
    UINT32 "FreeTime",
}



Config "dataconfig_raremonster10thpreview" {
    UINT32 "Times",
    UINT32 "ID",
}



Config "dataconfig_raremonsterpreview" {
    UINT32 "Type",
    UINT32 "ShenShouID",
    UINT32 "LevelMax",
}



Config "dataconfig_redbag" {
    UINT32 "Id", --unique key
    STRING "Name",
    UINT32 "RbType",
    UINT32 "DefaultGold",
    UINT32 "AddGold",
    UINT32 "MaxGold",
    UINT32 "DefaultNum",
    UINT32 "AddNum",
    UINT32 "MaxNum",
    UINT32 "ResetType",
    UINT32 "TriggerType",
    UINT32 "TrigParam1",
    UINT32 "TrigParam2",
}



Config "dataconfig_resconfig" {
    UINT32 "ResID", --unique key
    STRING "szResName",
    STRING "szResFile",
    SINT32 "Scale",
    SINT32 "ImportantNPC",
}



Config "dataconfig_revivecost" {
    UINT32 "num",
    UINT32 "cost",
    UINT32 "time",
}



Config "dataconfig_reward" {
}



Config "dataconfig_rewardpreview" {
    UINT32 "ID", --multi key
    UINT32 "DisplayID",
    UINT32 "Level",
    Struct {
        UINT32 "Item",
        UINT32 "num",
        UINT32 "NotAlways",
    }[12] "Items",
}



Config "dataconfig_rewardrecycle" {
    UINT32 "ID", --multi key
    UINT32 "Level",
    UINT32 "ConsumeType",
    UINT32 "ConsumeNum",
    UINT32 "PreviewItem",
}



Config "dataconfig_ride" {
    UINT32 "ID",
    STRING "name",
    UINT32 "ResID",
    UINT32 "UIScale",
    UINT32 "Scale",
    UINT32 "speed",
    STRING "runSound",
    STRING "OnRideEffect",
    UINT32 "hangPoint1",
    STRING "UnderRideEffect",
    UINT32 "hangPoint2",
    UINT32 "actorRunAction",
    UINT32 "actorIdleAction",
    UINT32 "actorGoStopAction",
    STRING "RideBinder",
    SINT32[3] "BinderPos";
    SINT32[3] "BoxScale";
    SINT32[3] "BoxPos";
}



Config "dataconfig_ridespriteconfig" {
    UINT32 "ID", --unique key
    UINT32 "Position",
    UINT32 "Quality",
    UINT32 "ComposeLevel",
    Struct {
        UINT32 "AttrType",
        UINT32 "Value",
    }[3] "AddAttrInfo",
    UINT32 "NextLevelID",
    UINT32 "Fighting",
    UINT32 "Identification",
}



Config "dataconfig_ridespritere" {
    UINT32 "Job", --multi key
    UINT32 "ZoneLevel", --multi key
    UINT32 "Position", --multi key
    Struct {
        UINT32 "LingShiID",
        UINT32[3] "LingShiList";
    }[5] "RecommendList",
}



Config "dataconfig_robotattr" {
    UINT32 "LevelID", --unique key
    UINT32 "RobotAttrID",
    UINT32 "ModeType",
    UINT32 "Honor",
    Struct {
        UINT32 "Job",
        UINT32 "Weapon",
        UINT32 "Clothes",
        UINT32[4] "Skill1";
        UINT32[1] "Skill2";
        UINT32[1] "Skill3";
        UINT32[1] "Skill4";
    }[24] "RobotList",
    UINT32 "ShenShouStartNumber",
    Struct {
        UINT32 "clonedWeapon",
        UINT32[1] "clonedSkill";
    }[4] "clonedArray",
}



Config "dataconfig_robotattr4client" {
    UINT32 "RobotID", --multi key
    UINT32 "LevelID", --multi key
    UINT32 "RobotJob",
    UINT32 "strength",
    UINT32 "Endurance",
    UINT32 "Quality",
    UINT32 "Dexterity",
    UINT32 "Shenfa",
    UINT32 "Hp",
    UINT32 "HpSpeed",
    UINT32 "OutAttack",
    UINT32 "OutDefence",
    UINT32 "InterAttack",
    UINT32 "InterDefence",
    UINT32 "FireAttack",
    UINT32 "FireDefence",
    UINT32 "WaterAttack",
    UINT32 "WaterDefence",
    UINT32 "EarthAttack",
    UINT32 "EarthDefence",
    UINT32 "WoodAttack",
    UINT32 "WoodDefence",
    UINT32 "AllDefence",
    UINT32 "IgnoreAllDef",
    UINT32 "CritRate",
    UINT32 "AntiCritRate",
    UINT32 "CritHurt",
    UINT32 "AntiCritHurt",
    UINT32 "HitRate",
    UINT32 "Dodge",
    UINT32 "HurtToHp",
    UINT32 "DefAddHP",
    UINT32 "AttrEffectRate",
    UINT32 "DefAttrEffectRate",
    UINT32 "ErtigoRate",
    UINT32 "DefErtigoRate",
    UINT32 "FixedBodyRate",
    UINT32 "DefFixedBodyRate",
    UINT32 "SlowRate",
    UINT32 "DefSlowRate",
    UINT32 "SlientRate",
    UINT32 "DefSlientRate",
    UINT32 "AttrEffectTm",
    UINT32 "DefAttrEffectTm",
    UINT32 "ErtigoTm",
    UINT32 "DefErtigoTm",
    UINT32 "FixedBodyTm",
    UINT32 "DefFixedBodyTm",
    UINT32 "SlowTm",
    UINT32 "DefSlowTm",
    UINT32 "SilentTm",
    UINT32 "DefSilentTm",
    UINT32 "DefNegativeEffectRate",
    UINT32 "DefNegativeEffectTm",
    UINT32[4] "Skill1";
    UINT32[1] "Skill2";
    UINT32[1] "Skill3";
    UINT32[1] "Skill4";
    UINT32[1] "AngerSkill";
    UINT32[5] "FashionClothes";
    UINT32[1] "FashionWeapon";
    UINT32[1] "tiltle";
}



Config "dataconfig_robotattr4server" {
}



Config "dataconfig_robotattrskill" {
    UINT32 "LevelID",
    UINT32 "RobotJob",
    UINT32 "ActID",
    UINT32 "strength",
    UINT32 "Endurance",
    UINT32 "Quality",
    UINT32 "Dexterity",
    UINT32 "Shenfa",
    UINT32 "Hp",
    UINT32 "HpSpeed",
    UINT32 "OutAttack",
    UINT32 "OutDefence",
    UINT32 "InterAttack",
    UINT32 "InterDefence",
    UINT32 "FireAttack",
    UINT32 "FireDefence",
    UINT32 "WaterAttack",
    UINT32 "WaterDefence",
    UINT32 "EarthAttack",
    UINT32 "EarthDefence",
    UINT32 "WoodAttack",
    UINT32 "WoodDefence",
    UINT32 "AllDefence",
    UINT32 "IgnoreAllDef",
    UINT32 "CritRate",
    UINT32 "AntiCritRate",
    UINT32 "CritHurt",
    UINT32 "AntiCritHurt",
    UINT32 "HitRate",
    UINT32 "Dodge",
    UINT32 "HurtToHp",
    UINT32 "DefAddHP",
    UINT32 "AttrEffectRate",
    UINT32 "DefAttrEffectRate",
    UINT32 "ErtigoRate",
    UINT32 "DefErtigoRate",
    UINT32 "FixedBodyRate",
    UINT32 "DefFixedBodyRate",
    UINT32 "SlowRate",
    UINT32 "DefSlowRate",
    UINT32 "SlientRate",
    UINT32 "DefSlientRate",
    UINT32 "AttrEffectTm",
    UINT32 "DefAttrEffectTm",
    UINT32 "ErtigoTm",
    UINT32 "DefErtigoTm",
    UINT32 "FixedBodyTm",
    UINT32 "DefFixedBodyTm",
    UINT32 "SlowTm",
    UINT32 "DefSlowTm",
    UINT32 "SilentTm",
    UINT32 "DefSilentTm",
    UINT32 "DefNegativeEffectRate",
    UINT32 "DefNegativeEffectTm",
    UINT32 "Weapon",
    UINT32 "Clothes",
    UINT32[4] "Skill1";
    UINT32[1] "Skill2";
    UINT32[1] "Skill3";
    UINT32[1] "Skill4";
    UINT32[1] "AngerSkill";
    UINT32[5] "FashionClothes";
    UINT32[8] "FashionWeapon";
    UINT32[2] "tiltle";
}



Config "dataconfig_robotfemalename" {
    SINT32 "id",
    STRING "FemaleName",
}



Config "dataconfig_robotfirstname" {
    SINT32 "id",
    STRING "FirstName",
}



Config "dataconfig_robotmalename" {
    SINT32 "id",
    STRING "MaleName",
}



Config "dataconfig_robotmiddlename" {
    SINT32 "id",
    STRING "MiddleName",
}



Config "dataconfig_robotpetattr" {
    UINT32 "LevelID",
    UINT32 "ShenShouID",
    UINT32 "Star",
    UINT32 "StarLevel",
    UINT32[1] "Skill";
    UINT32 "SkillID",
    UINT32 "ProtectorSkill",
    UINT32 "ShenShouGrowLevel",
    UINT32 "ShenShouGrowLevelDivide",
    UINT32 "ID",
    UINT32 "StrengthenLevel",
    UINT32 "StrengthenStep",
}



Config "dataconfig_robscoreratio" {
    UINT32 "IndexID",
    UINT32 "ScoreMin",
    UINT32 "ScoreMax",
}



Config "dataconfig_rolebaseattr" {
    UINT32 "level", --unique key
    UINT32 "Weapon",
    UINT32 "strength",
    UINT32 "Endurance",
    UINT32 "Quality",
    UINT32 "Dexterity",
    UINT32 "Shenfa",
    UINT32 "Hp",
    UINT32 "HpSpeed",
    UINT32 "OutAttack",
    UINT32 "OutDefence",
    UINT32 "InterAttack",
    UINT32 "InterDefence",
    UINT32 "FireAttack",
    UINT32 "FireDefence",
    UINT32 "WaterAttack",
    UINT32 "WaterDefence",
    UINT32 "EarthAttack",
    UINT32 "EarthDefence",
    UINT32 "WoodAttack",
    UINT32 "WoodDefence",
    UINT32 "AllDefence",
    UINT32 "IgnoreAllDef",
    UINT32 "CritRate",
    UINT32 "AntiCritRate",
    UINT32 "CritHurt",
    UINT32 "AntiCritHurt",
    UINT32 "HitRate",
    UINT32 "Dodge",
    UINT32 "Block",
    UINT32 "AntiBlock",
    UINT32 "BlockHurtRate",
    UINT32 "DefBlockHurtRate",
    UINT32 "HolyAttack",
    UINT32 "HolyDefence",
    UINT32 "HurtToHp",
    UINT32 "DefAddHP",
    UINT32 "AttrEffectRate",
    UINT32 "DefAttrEffectRate",
    UINT32 "ErtigoRate",
    UINT32 "DefErtigoRate",
    UINT32 "FixedBodyRate",
    UINT32 "DefFixedBodyRate",
    UINT32 "SlowRate",
    UINT32 "DefSlowRate",
    UINT32 "SlientRate",
    UINT32 "DefSlientRate",
    UINT32 "AttrEffectTm",
    UINT32 "DefAttrEffectTm",
    UINT32 "ErtigoTm",
    UINT32 "DefErtigoTm",
    UINT32 "FixedBodyTm",
    UINT32 "DefFixedBodyTm",
    UINT32 "SlowTm",
    UINT32 "DefSlowTm",
    UINT32 "SilentTm",
    UINT32 "DefSilentTm",
    UINT32 "DefNegativeEffectRate",
    UINT32 "DefNegativeEffectTm",
    UINT32 "StrengthToOutAttack",
    UINT32 "StrengthToInterAttack",
    UINT32 "EnduranceToOutDefence",
    UINT32 "EnduranceToInterDefence",
    UINT32 "EnduranceToBlock",
    UINT32 "DexterityToHitRate",
    UINT32 "DexterityCirtRate",
    UINT32 "ShenfaToDodgeRate",
    UINT32 "ShenfaToAntiCritRateRate",
    UINT32 "ShenfaToAllDefRate",
    UINT32 "ShenfaToAntiBlock",
    UINT32 "QualityToHpRate",
    UINT32 "QualityToAntiCritRateRate",
    UINT32 "AllDefToFireDefRate",
    UINT32 "AllDefToWaterDefRate",
    UINT32 "AllDefToEarthDefRate",
    UINT32 "AllDefToWoodDefRate",
    UINT32 "AttrEffectRateToErtigoRate",
    UINT32 "AttrEffectRateToFixedBodyRate",
    UINT32 "AttrEffectRateToSlowRate",
    UINT32 "AttrEffectRateToSlientRate",
    UINT32 "DefRateToErtigoRate",
    UINT32 "DefRateToFixedBodyRate",
    UINT32 "DefRateToSlowRate",
    UINT32 "DefRateToSlientRate",
    UINT32 "AttrEffectTmToErtigoTmRate",
    UINT32 "AttrEffectTmToFixedBodyTmRate",
    UINT32 "AttrEffectTmToSlowTmRate",
    UINT32 "AttrEffectTmToSlientTmRate",
    UINT32 "DefAttrEffectTmToDefErtigoTmRate",
    UINT32 "DefAttrEffectTmToDefFixedBodyTmRate",
    UINT32 "DefAttrEffectTmToDefSlowTmRate",
    UINT32 "DefAttrEffectTmToDefSlientTmRate",
    UINT32 "Fighting",
}



Config "dataconfig_roledata" {
}



Config "dataconfig_roleplay" {
    UINT32 "ID", --unique key
    UINT32 "Type",
    UINT32 "Param",
}



Config "dataconfig_rolling" {
    UINT32 "Id", --unique key
    UINT32 "TiredNum",
    UINT32 "VipLevel",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
        UINT32 "Weight",
    }[3] "AwardList",
}



Config "dataconfig_roomhuashancompetition" {
    UINT32 "ID",
    UINT32 "EnterTime",
    UINT32 "ReadyWaitTime",
    UINT32 "FightTime",
    UINT32 "BalanceTime",
}



Config "dataconfig_roomhuashancompetitionfinal" {
    UINT32 "State", --unique key
    UINT32 "Time",
    UINT32 "StateType",
    STRING "ScenceTips",
    STRING "FightTips",
    STRING "ScenceSecTips",
}



Config "dataconfig_scene" {
    UINT32 "TaskID", --unique key
    STRING "TaskIntroduce",
    UINT32 "ID", --multi key
    UINT32 "Group",
    STRING "Desc",
    UINT32 "Times",
    UINT32 "Sharing",
    UINT32 "UsageCount",
    UINT32 "Sex",
    UINT32[1] "PickID";
    SINT32[2] " PreTask";
    SINT32[4] "TargetInfo";
    SINT32[1] "TargetAction";
    UINT32 "renwuchuan",
    UINT32 "start",
    UINT32 "Posttask",
    UINT32 "receive",
    UINT32 "usability",
    UINT32 "IsOpenMicrophone",
}



Config "dataconfig_scenefenbao" {
    UINT32 "Id", --unique key
    UINT32 "Pakage",
}



Config "dataconfig_scenemapcfg" {
    UINT32 "id", --unique key
    STRING "name",
    STRING "MapName",
    SINT32 "MapRate",
    SINT32 "Xoffset",
    SINT32 "Yoffset",
    SINT32 "MapSize",
    SINT32 "SafeXoffset",
    SINT32 "SafeZoffset",
    SINT32 "ShowSignLine",
    STRING "SignDetail",
}



Config "dataconfig_scenemapsign" {
    UINT32 "id", --multi key
    SINT32 "SignId",
    SINT32 "SignType",
    STRING "SignIcon",
    STRING "SignName",
    SINT32 "MapRate",
    SINT32 "Xoffset",
    SINT32 "Zoffset",
    SINT32 "MapSize",
}



Config "dataconfig_sceneprompt" {
    UINT32 "id", --unique key
    Struct {
        STRING "npcIcon",
        STRING "Atlas",
        STRING "npcName",
        UINT32 "time",
        UINT32 "nsoundeTime",
        UINT32 "nsoundPath",
        STRING "text",
        SINT32[3] "pos";
        SINT32 "nisRoleTalk",
    }[4] "Items",
}



Config "dataconfig_sceneshowobjs" {
    UINT32 "ID", --unique key
    Struct {
        UINT32 "ObjID",
        UINT32 "ResID",
        SINT32[3] "Pos";
        SINT32[3] "Rotate";
        SINT32[3] "Scale";
    }[1] "SceneObjs",
}



Config "dataconfig_scoretyperatio" {
    UINT32 "IndexID",
    UINT32 "AllScore",
    UINT32 "RewardScore",
    UINT32 "UnRewardScore",
}



Config "dataconfig_seaconfig" {
    UINT32 "ID", --unique key
    UINT32[4] "LevelUp";
    UINT32[4] "LevelEffect";
    UINT32 "DailyTimes",
    UINT32 "OtherTimes",
}



Config "dataconfig_seagathering" {
    UINT32 "GatherID", --unique key
    UINT32 "Level",
    UINT32 "MonsterID",
    UINT32 "GatherDuration",
    UINT32 "MinRespawn",
    UINT32 "MaxRespawn",
    UINT32 "GoodID",
}



Config "dataconfig_secondattrrate" {
    SINT32 "Level", --unique key
    SINT32 "StrengthToAttack",
    SINT32 "StrengthToSkillHit",
    SINT32 "InterForceToAttack",
    SINT32 "InterForceToSkillHit",
    SINT32 "EnduranceToDefence",
    SINT32 "EnduranceToHp",
    SINT32 "SpiritToDefence",
    SINT32 "SpiritToMp",
    SINT32 "AgilityToCriHit",
    SINT32 "AgilityToCriHitHp",
    SINT32 "CriHitToRate",
    SINT32 "SubDamage",
    SINT32 "AttrAtk",
    SINT32 "AttrDef",
}



Config "dataconfig_sevday" {
    UINT32 "WorldLev",
    Struct {
        UINT32 "BossID",
        UINT32 "BossNub",
    }[9] "BossIDK",
}



Config "dataconfig_shenshi" {
    UINT32 "FilterID", --unique key
    UINT32 "IsRGB",
    UINT32 "IsGray",
    UINT32 "IsTexture",
    UINT32 "RedValue",
    UINT32 "GreenValue",
    UINT32 "BlueValue",
    UINT32 "BrightnessValue",
    UINT32 "ContrastValue",
    UINT32 "SaturationValue",
    UINT32[3] "RoleColor";
    UINT32 "Power",
}



Config "dataconfig_shenshoprecycleshoprefresh" {
    UINT32 "RefreshNum", --unique key
    UINT32 "NeedItem1",
    UINT32 "NeedNum1",
    UINT32 "NeedItem2",
    UINT32 "NeedNum2",
}



Config "dataconfig_shenshou" {
    UINT32 "ID", --unique key
    STRING "Name",
    UINT32 "Quality",
    UINT32 "Type",
    UINT32 "ResID",
    UINT32 "Scale",
    STRING "Atlas",
    STRING "Icon",
    UINT32 "RideIndex",
    UINT32 "QuaID",
    UINT32 "AddPercent",
    UINT32[1] "SkillID";
    UINT32 "RandSkillID",
    UINT32 "ReRandSkillID",
    Struct {
        UINT32 "Type",
        UINT32 "Value",
    }[4] "Property",
    UINT32 "DownPercent",
    UINT32 "UpPercent",
    UINT32 "WashItem",
    UINT32 "WashNum",
    Struct {
        UINT32 "NeedItem",
        UINT32 "NeedNum",
    }[1] "Item",
    UINT32 "ExFightNum",
    UINT32 "ShenShouType",
}



Config "dataconfig_shenshouaddproperty" {
    UINT32 "ID",
    UINT32 "Key",
    Struct {
        UINT32 "NeedItem",
        UINT32 "NeedNum",
    }[1] "Item",
    UINT32 "AddValueMin",
    UINT32 "AddValueMax",
    UINT32 "Fighting",
}



Config "dataconfig_shenshoubase" {
    UINT32 "ID", --unique key
    STRING "Name",
    UINT32 "Quality",
    UINT32 "NirvanaID",
    UINT32 "Type",
    UINT32 "QuaID",
    UINT32 "DecomposeItem", --multi key
    UINT32 "DecomposeNum",
    UINT32 "ComposeNum",
    UINT32 "ShenShouType",
    UINT32 "Noumenon",
    UINT32 "MoodValueMax",
    UINT32 "SkillMax",
    UINT32[8] "SkillProb";
    UINT32 "StrengthGrow",
    UINT32 "EnduranceGrow",
    UINT32 "QualityGrow",
    UINT32 "DexterityGrow",
    UINT32 "ShenfaGrow",
    UINT32 "StrengthenGrowID",
    Struct {
        UINT32 "CollectType",
        UINT32 "Value",
    }[2] "ExProperty",
    UINT32 "ExFightNum",
    UINT32[2] "ExtraStar";
    STRING "Story",
    UINT32 "SkillUseProb",
    UINT32 "ProtectorSkillUseProb",
}



Config "dataconfig_shenshoubaseattr" {
    UINT32 "ID", --unique key
    UINT32 "Strength",
    UINT32 "Endurance",
    UINT32 "Quality",
    UINT32 "Dexterity",
    UINT32 "Shenfa",
    UINT32 "Hp",
    UINT32 "HpSpeed",
    UINT32 "OutAttack",
    UINT32 "OutDefence",
    UINT32 "InterAttack",
    UINT32 "InterDefence",
    UINT32 "FireAttack",
    UINT32 "FireDefence",
    UINT32 "WaterAttack",
    UINT32 "WaterDefence",
    UINT32 "EarthAttack",
    UINT32 "EarthDefence",
    UINT32 "WoodAttack",
    UINT32 "WoodDefence",
    UINT32 "AllDefence",
    UINT32 "IgnoreAllDef",
    UINT32 "CritRate",
    UINT32 "AntiCritRate",
    UINT32 "CritHurt",
    UINT32 "AntiCritHurt",
    UINT32 "HitRate",
    UINT32 "Dodge",
    UINT32 "HurtToHp",
    UINT32 "DefAddHP",
    UINT32 "AttrEffectRate",
    UINT32 "DefAttrEffectRate",
    UINT32 "ErtigoRate",
    UINT32 "DefErtigoRate",
    UINT32 "FixedBodyRate",
    UINT32 "DefFixedBodyRate",
    UINT32 "SlowRate",
    UINT32 "DefSlowRate",
    UINT32 "SlientRate",
    UINT32 "DefSlientRate",
    UINT32 "AttrEffectTm",
    UINT32 "DefAttrEffectTm",
    UINT32 "ErtigoTm",
    UINT32 "DefErtigoTm",
    UINT32 "FixedBodyTm",
    UINT32 "DefFixedBodyTm",
    UINT32 "SlowTm",
    UINT32 "DefSlowTm",
    UINT32 "SilentTm",
    UINT32 "DefSilentTm",
    UINT32 "DefNegativeEffectRate",
    UINT32 "DefNegativeEffectTm",
    UINT32 "StrengthToOutAttack",
    UINT32 "EnduranceToOutDefence",
    UINT32 "DexterityToHitRate",
    UINT32 "DexterityCirtRate",
    UINT32 "ShenfaToDodgeRate",
    UINT32 "ShenfaToAntiCritRateRate",
    UINT32 "QualityToHpRate",
    UINT32 "Fighting",
    UINT32 "SkillID",
    UINT32 "ProtectorSkill",
    UINT32 "CommonSkillNum",
    UINT32 "CommonSkillOpenNum",
    UINT32[1] "CommonSkillID";
    UINT32[2] "RecommendSkill";
    UINT32 "AddPercent",
    UINT32 "BasePercent",
}



Config "dataconfig_shenshoubreak" {
    UINT32 "ID",
    UINT32 "BreakLevel",
    UINT32 "Limit",
    Struct {
        UINT32 "Type",
        UINT32 "AddValue",
    }[4] "BreakValue",
    Struct {
        UINT32 "NeedItem",
        UINT32 "NeedNum",
    }[1] "Item",
}



Config "dataconfig_shenshoubuff" {
    UINT32 "Type", --multi key
    UINT32 "Quality", --multi key
    UINT32 "BuffId",
    UINT32 "DicId",
    UINT32 "SkillCd",
    STRING "iconAtals",
    STRING "iconName",
    UINT32 "Choose",
}



Config "dataconfig_shenshoucollect" {
    UINT32 "ShenShouCollectID",
    UINT32 "ShenShouCollectNum",
    UINT32 "ShenShouCollectReward", --unique key
    STRING "CollectText",
}



Config "dataconfig_shenshouexpitem" {
    UINT32 "ID",
    UINT32 "AddHeart",
}



Config "dataconfig_shenshouformation" {
    UINT32 "ID",
    STRING "Name",
    UINT32 "OpenLevel",
    Struct {
        UINT32 "Type",
        UINT32 "Rate",
    }[5] "PositionProperty",
}



Config "dataconfig_shenshouformationactivation" {
    UINT32 "ID",
    UINT32 "Quality",
    UINT32 "Number",
    Struct {
        UINT32 "key",
        UINT32 "value",
    }[5] "AddAttrInfo",
    UINT32 "Power",
    UINT32 "Function",
}



Config "dataconfig_shenshouformationproperty" {
    UINT32 "ID", --multi key
    UINT32 "Level",
    UINT32 "NeedStar", --multi key
    Struct {
        UINT32 "Type",
        UINT32 "Value",
    }[2] "PositionProperty",
    UINT32 "ExFightNum",
}



Config "dataconfig_shenshougrow" {
    UINT32 "ShenShouID", --multi key
    UINT32 "ShenShouGrowLevel", --multi key
    UINT32 "ShenShouGrowLevelDivide", --multi key
    UINT32 "ShenShouGrowValue",
    UINT32 "ShenShouGrowPower",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[2] "MaterialList",
}



Config "dataconfig_shenshougrowbreak" {
    UINT32 "Quality", --multi key
    UINT32 "ShenShouGrowLevel", --multi key
    UINT32 "Starlimit",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[2] "MaterialList",
}



Config "dataconfig_shenshouinheritconfig" {
    Struct {
        UINT32 "MaterialID",
    }[2] "InheritCost",
}



Config "dataconfig_shenshoulevel" {
    UINT32 "Level", --unique key
    UINT32 "Fighting",
}



Config "dataconfig_shenshounirvana" {
    UINT32 "IndexID", --unique key
    UINT32 "Nirvana",
    UINT32 "Openday",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[3] "MaterialList",
}



Config "dataconfig_shenshoupositionlevel" {
    UINT32 "ID", --unique key
    UINT32 "OpenLevel",
}



Config "dataconfig_shenshoupositionopen" {
    UINT32 "ID",
    UINT32 "OpenLevel",
}



Config "dataconfig_shenshoupreview" {
    UINT32 "ID", --unique key
    UINT32 "NeedZoneLevel",
    Struct {
        STRING "Des",
    }[4] "LocationDes",
    STRING "Character",
    STRING "CharacterDes",
    UINT32 "Sword",
    UINT32 "Harmer",
    UINT32 "Bow",
    UINT32 "Umbrella",
}



Config "dataconfig_shenshouquality" {
    UINT32 "ID",
    UINT32 "PropertyQuality",
    UINT32 "DownValue",
    UINT32 "UpValue",
    UINT32 "BornWeight",
    UINT32 "WashWeight",
    UINT32 "WashEnsureWeight",
    UINT32 "AwakeWeight",
    UINT32 "Fighting",
}



Config "dataconfig_shenshourecommend" {
    UINT32 "ID", --multi key
    UINT32 "ListID",
    STRING "Title",
    Struct {
        UINT32 "ShenShouID",
    }[10] "List",
}



Config "dataconfig_shenshourecycleshopprice" {
    UINT32 "ID", --unique key
    UINT32 "Price",
    UINT32 "MoneyType",
}



Config "dataconfig_shenshoures" {
    UINT32 "ID",
    UINT32 "Star",
    STRING "Name",
    UINT32 "ResID",
    UINT32 "Scale",
    UINT32 "GetScale",
    UINT32 "PreScale",
    UINT32 "FormationScale",
    SINT32 "Rotation",
    STRING "Atlas",
    STRING "ActivityIcon",
    STRING "CirCleAtlas",
    STRING "CircleActivityIcon",
    UINT32 "RideIndex",
    UINT32 "FlowersShenshou",
    UINT32 "BattleShenshou",
    UINT32 "WildShenshou",
    Struct {
        STRING "Type",
        UINT32 "Value",
    }[3] "BaseProperty",
}



Config "dataconfig_shenshoushoplevel" {
    UINT32 "Level",
    UINT32 "NeedExp",
    UINT32 "WareNum1",
    UINT32 "WareNum2",
    UINT32 "WareNum3",
    UINT32 "WareNum4",
    STRING "WareDes",
    Struct {
        UINT32 "Item",
        UINT32 "Num",
    }[2] "Reward",
}



Config "dataconfig_shenshoushoprefresh" {
    UINT32 "RefreshNum",
    UINT32 "NeedItem1",
    UINT32 "NeedNum1",
    UINT32 "NeedItem2",
    UINT32 "NeedNum2",
}



Config "dataconfig_shenshoushopware" {
    UINT32 "ID",
    UINT32 "WareType",
    UINT32 "Quality",
    UINT32 "ShenShouID",
    UINT32 "ItemID",
    UINT32 "Num",
    UINT32 "SellType",
    UINT32 "Price",
    UINT32 "Weight",
    UINT32 "OpenLevel",
    UINT32 "CloseLevel",
}



Config "dataconfig_shenshouskill" {
    UINT32 "ID",
    UINT32 "Quality",
    UINT32 "Type",
    STRING "icon",
    STRING "name",
    Struct {
        UINT32 "NeedItem",
        UINT32 "NeedNum",
    }[1] "Item",
    Struct {
        UINT32 "ObjType",
        UINT32 "Type",
        UINT32 "Num",
    }[1] "Effect",
    UINT32 "ExFightNum",
}



Config "dataconfig_shenshouskillbaropen" {
    UINT32 "SkillBar", --unique key
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[1] "UpgradeMaterial",
}



Config "dataconfig_shenshouskillrandom" {
    UINT32 "ID",
    UINT32 "SkillID",
}



Config "dataconfig_shenshouskillrecommend" {
    UINT32 "ShenShouType",
    UINT32 "ServerLimit",
    Struct {
        UINT32 "Skill",
    }[6] "RecommendSkill",
}



Config "dataconfig_shenshouskills" {
    UINT32 "ShenShouSkillID", --unique key
    UINT32 "kind", --multi key
    UINT32 "SkillUpgraded",
    UINT32 "SkillType",
    STRING "name",
    UINT32 "skillQuaID",
    UINT32 "fighting",
    UINT32 "tpye",
    UINT32 "rushSkillID",
    UINT32 "skillID",
    SINT32[1] "buffID";
    SINT32[1] "hostBuffID";
    STRING "desc",
    STRING "effectDesc",
    STRING "iconAtals",
    STRING "iconName",
    UINT32 "costProp",
    Struct {
        UINT32 "MaterialID",
        UINT32 "MaterialNum",
    }[1] "UpgradeMaterial",
    UINT32 "SkillInLevelMax",
}



Config "dataconfig_shenshoustar" {
    UINT32 "ID", --multi key
    UINT32 "Star", --multi key
    UINT32 "StarLevel", --multi key
    Struct {
        UINT32 "Type",
        UINT32 "Value",
    }[5] "BaseProperty",
    Struct {
        UINT32 "CollectType",
        UINT32 "Value",
    }[2] "ExtraProperty",
    UINT32 "ExGuardFightNum",
    UINT32 "NextCostType",
    UINT32 "TotalNum",
    Struct {
        UINT32 "Renascence",
        UINT32 "Condition",
        UINT32 "Num",
    }[2] "CostParam",
    UINT32 "NeedCopper",
    UINT32 "NeedLevel",
    UINT32 "StrengthGrow",
    UINT32 "EnduranceGrow",
    UINT32 "QualityGrow",
    UINT32 "DexterityGrow",
    UINT32 "ShenfaGrow",
    UINT32 "BaseAttrRate",
    UINT32 "ChangeIcon",
}



Config "dataconfig_shenshoutype" {
    UINT32 "ShenShouType", --unique key
    STRING "Name",
}



Config "dataconfig_shopconfig" {
    UINT32 "ID", --unique key
    STRING "Name",
    UINT32 "MallType",
    STRING "TabName",
    UINT32 "Type",
    STRING "Caption",
    SINT32 "MoneyType",
}



Config "dataconfig_shopgoodsconfig" {
    UINT32 "GoodsID", --unique key
    UINT32 "ShopID",
    UINT32 "PosID",
    UINT32 "ItemID",
    UINT32[1] "BuyUpperLimit";
    UINT32 "GoodsUnitPrice",
    UINT32 "GoodsLabel",
    UINT32 "Discount",
    UINT32 "BuyAuthority",
    UINT32 "SingelBuyNum",
    UINT32 "OpenLevel",
    UINT32 "CloseLevel",
}



Config "dataconfig_showhide" {
    SINT32 "ID", --multi key
    SINT32 "TaskID",
    SINT32 "PlayerLevel",
    SINT32 "ServerTime",
    Struct {
        UINT32 "Type",
        UINT32 "param1",
        UINT32 "param2",
    }[3] "ShowWhat",
}



Config "dataconfig_signrewards" {
    UINT32 "month", --unique key
    STRING "specialRewardIcon",
    Struct {
        UINT32 "rewardID",
        UINT32 "rewardNum",
        UINT32 "VipLevel",
        UINT32 "Times",
    }[31] "signRewards",
    Struct {
        UINT32 "rewardID",
        UINT32 "rewardNum",
        UINT32 "Days",
    }[6] "SpecialRewards",
    Struct {
        UINT32 "rewardID",
        UINT32 "rewardNum",
        UINT32 "Days",
    }[1] "LoginAward",
}



Config "dataconfig_singleaction" {
    UINT32 "ID",
    STRING "ActionName",
    UINT32 "Sex", --multi key
    UINT32 "Skill",
    UINT32 "Talk",
    STRING "Icon",
}



Config "dataconfig_singlelevelaccount" {
    UINT32 "id", --unique key
    UINT32 "SSSRankTime",
    UINT32 "SSRankTime",
    UINT32 "SRankTime",
    UINT32 "ARankTime",
    UINT32 "BRankTime",
    UINT32 "CRankTime",
}



Config "dataconfig_singlelevelaward" {
    UINT32 "Id", --unique key
    UINT32[4] "RewardEqu";
    UINT32[1] "RewardMat";
    STRING "SPcondition",
    Struct {
        UINT32 "IndexID",
        UINT32 "Quantity",
    }[3] "SPRewardMat",
}



Config "dataconfig_singlevictoryani" {
    STRING "SceneName", --unique key
    SINT32[3] "victoryPos";
    SINT32[3] "victoryDir";
}



Config "dataconfig_sit" {
    UINT32 "Level", --unique key
    UINT32 "RestoreRate",
    UINT32 "RestoreValue",
}



Config "dataconfig_skilladvise" {
    UINT32 "skillID",
    UINT32 "id", --unique key
    Struct {
        STRING "name",
        STRING "fullName",
        STRING "desc",
        UINT32 "isEquip",
        UINT32 "priority",
        UINT32 "weight",
    }[3] "paramList",
}



Config "dataconfig_skillconfig" {
    UINT32 "nSkillID", --unique key
    STRING "szSkillFileName",
    UINT32 "nActionTime",
    UINT32 "priority",
    UINT32 "uStateID",
    UINT32 "uType",
    UINT32 "weaponType",
    UINT32 "uCategory",
    UINT32 "uTargetType",
    UINT32 "uCDTime",
    UINT32[3] "uCDCumulativeNumber";
    SINT32 "nBatiExpend",
    Struct {
        UINT32[5] "EnemyEffectID";
        UINT32[2] "FriendEffectID";
        UINT32[5] "SelfEffectID";
        UINT32 "NpcID",
    }[16] "EffectIDArray",
    UINT32 "isLockTarget",
    UINT32 "lockAngle",
    UINT32 "lockInnerDist",
    UINT32 "lockOutDist",
    SINT32 "AutoFightPriority",
    SINT32 "AutoFightWeight",
    UINT32 "pvpNotMoveDis",
    UINT32 "Distance",
    STRING "skillIconAtals",
    STRING "skillIconName",
    STRING "szSkillName",
    STRING "skillDesc",
    STRING "skillEffectDesc",
    SINT32 "weaponModelRule",
    BOOL "isBrokenScreen",
    BOOL "bOneEffect",
    UINT32 "cdGroup",
    SINT32 "PoBatiLevel",
    UINT32 "farestDistance",
    UINT32 "nearestDistance",
    UINT32[1] "actorHPPercent";
    UINT32 "ConsumeType",
    UINT32 "ConsumeVal",
    UINT32 "ConsumeStepID",
    SINT32 "LevelUpType",
    SINT32 "exShowTime",
    UINT32 "AngerCorrect",
    UINT32 "AngerBuffAward",
    SINT32 "LocationPoint",
    SINT32[5] "SummonPoint";
    SINT32 "NextSkillID",
    UINT32 "IntervalCD",
    UINT32 "TypeDamage",
    UINT32 "PublicCD",
    UINT32 "showscop",
}



Config "dataconfig_skilldesc" {
    UINT32 "nSkillID",
    STRING "skillDesc",
}



Config "dataconfig_skilllearn" {
    SINT32 "SkillLevel",
    SINT32 "MoneyType",
    SINT32 "LowMoneyNum",
    SINT32 "MidMoneyNum",
    SINT32 "HighMoneyNum",
}



Config "dataconfig_skillperfconfig" {
    UINT32 "nID", --unique key
    UINT32 "sType",
    UINT32 "EffectLevelType",
    UINT32 "nStatePriID",
    UINT32[1] "skillIDList";
    UINT32 "condition",
    UINT32[2] "targetHPCondition";
    UINT32[1] "selfBuffCon";
    UINT32 "pr",
    UINT32 "nParamIsSkillPerfKeyAttrID",
    Struct {
        UINT32 "nKey",
        SINT32 "nParam",
        SINT32 "nStepID",
    }[8] "paramList",
}



Config "dataconfig_skillreset" {
    SINT32 "ID", --unique key
    UINT32 "Consume",
}



Config "dataconfig_skilltips" {
    SINT32 "TipsNo", --unique key
    SINT32 "Weapon", --multi key
    SINT32 "TipsType", --multi key
    SINT32[4] "SkillArray";
    STRING "strName",
    SINT32 "atktype",
    SINT32 "TipsSpecial",
    SINT32[1] "SkillPointArray";
}





Config "dataconfig_skyfallbufflevel" {
    UINT32 "BuffId",
    UINT32 "Level",
    UINT32 "BeastFight",
    STRING "buffdec",
    STRING "nextdec",
    STRING "effdec",
}



Config "dataconfig_skyfallchallenge" {
    UINT32 "ID",
    STRING "title",
    STRING "desc",
}



Config "dataconfig_skyfallguildscorereward" {
    UINT32 "OrderMin",
    UINT32 "OrderMax",
    Struct {
        UINT32 "Item",
        UINT32 "Num",
    }[1] "Reward",
}



Config "dataconfig_skyfallrule" {
    UINT32 "key", --unique key
    UINT32[4] "BossResID";
    UINT32[5] "ScoreRate";
    UINT32 "RobTimes",
    UINT32 "RobLimit",
    UINT32 "RobCD",
    UINT32 "BeRobCD",
}



Config "dataconfig_skyfallzhufu" {
    UINT32 "ID",
    UINT32 "BeastFight",
    UINT32 "ZhuFu",
}



Config "dataconfig_soulopen" {
    SINT32 "Position",
    SINT32 "OpenLevel",
    STRING "Name",
    Struct {
        SINT32 "LevelLimit",
        STRING "Resource",
    }[5] "SoulList",
}



Config "dataconfig_sourceconfig" {
    UINT32 "ID", --unique key
    Struct {
        UINT32 "TypeID",
        UINT32[7] "IndexIDs1";
        UINT32[3] "IndexIDs2";
        STRING[1] "Txt";
    }[5] "SourceTypeList",
}



Config "dataconfig_spaeventrewards" {
}



Config "dataconfig_spageneralrewards" {
}



Config "dataconfig_spainteraction" {
}



Config "dataconfig_spanpclist" {
    UINT32 "NPCID",
    UINT32 "DailyLimit",
    UINT32[4] "TopicID";
    STRING "SpriteName",
    STRING "SpriteFigure",
}



Config "dataconfig_specialattrrange" {
    UINT32 "AttrRangeID", --unique key
    UINT32 "ValueMin",
    UINT32 "ValueMax",
    UINT32 "AttrRangeWeightID",
}



Config "dataconfig_specialmaterial" {
    UINT32 "ID",
    UINT32 "EquipConType",
    UINT32 "Level",
    STRING "ResultEquipName",
    UINT32 "ExtraCost",
}



Config "dataconfig_specialname" {
}



Config "dataconfig_stageaward" {
    UINT32 "IndexID",
    UINT32 "ServerLevel", --unique key
    UINT32[5] "Level";
    UINT32[5] "HighestScore";
    UINT32[5] "RewardGood";
}



Config "dataconfig_stallitems" {
    UINT32 "ID", --unique key
    STRING "name",
    UINT32 "Price",
    UINT32 "Type1ID",
    UINT32[1] "Type2ID";
    UINT32 "PriceStep",
    UINT32 "PriceStepMax",
    UINT32 "PriceStepMin",
    UINT32 "PriceDown",
    UINT32 "PriceUp",
    UINT32 "LevelDown",
    UINT32 "LevelUp",
    UINT32 "SaleMax",
    UINT32 "ServerLV",
    UINT32 "EquipTypeID",
    UINT32 "ServerLVMax",
}



Config "dataconfig_staterelationconfig" {
    UINT32 "nID",
    Struct {
        UINT32 "nParam",
    }[31] "paramList",
}



Config "dataconfig_stepconfig" {
    UINT32 "ID", --unique key
    Struct {
        SINT32 "nStep",
    }[10] "paramList",
}



Config "dataconfig_stoneconfig" {
    UINT32 "ID",
    STRING "Name",
    UINT32 "StartID",
    UINT32 "FinishID",
    UINT32 "NPCID",
    STRING "StoneModelName",
    STRING "ActivityIcon",
    STRING "Description",
    STRING "ParasNpcModel",
    STRING "BgResName",
}



Config "dataconfig_strength" {
    UINT32 "id",
    UINT32 "StrengthValue",
}



Config "dataconfig_strengthenbreak" {
    UINT32 "Position", --multi key
    UINT32 "BreakLevel", --multi key
    UINT32 "StrengthenLimit",
    Struct {
        UINT32 "Item",
        UINT32 "Num",
    }[1] "BreakCost",
    UINT32 "CostCopper",
}



Config "dataconfig_strengthenconf" {
    UINT32 "Position", --multi key
    UINT32 "StrengthenLevel", --multi key
    UINT32 "StrengthenRate",
    UINT32 "MaterialID",
    UINT32 "MaterialNum",
    UINT32 "Cost",
    UINT32 "FailAdd",
    Struct {
        UINT32 "key",
        UINT32 "value",
    }[6] "AddAttrInfo",
    UINT32 "Fighting",
}



Config "dataconfig_strengthenequiplimit" {
    UINT32 "Position", --multi key
    UINT32 "EquipLevel", --multi key
    UINT32 "StrengthenLimit",
}



Config "dataconfig_strengthenuplevel" {
    UINT32 "PlayerLevel", --unique key
    UINT32 "StrengthenLimit",
}



Config "dataconfig_sublse" {
    UINT32 "ID",
}



Config "dataconfig_suitattr" {
    UINT32 "SuitID", --multi key
    UINT32 "LevelRange",
    UINT32 "SuitNum",
    Struct {
        UINT32 "AttrID",
        UINT32 "value",
    }[3] "SuitAttrList",
    UINT32 "Fighting",
    UINT32 "PerfectNum",
    Struct {
        UINT32 "AttrID",
        UINT32 "value",
    }[3] "PerfectAttrList",
    UINT32 "PerfectFighting",
}



Config "dataconfig_suitequip" {
    UINT32 "SuitID", --multi key
    STRING "SuitName",
    UINT32 "SuitEquipID", --multi key
    UINT32 "EquipID", --multi key
    UINT32[4] "EquipList";
    UINT32 "LevelRange",
    UINT32 "Recommend", --multi key
    STRING "Desc",
    STRING "EquipTypeDesc",
}



Config "dataconfig_suitforge" {
    UINT32 "SuitEquipID", --unique key
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[3] " ForgeNeed",
    UINT32 "SuitEquipTypeID",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[3] "UpgradeNeed",
    Struct {
        SINT32 "ItemID",
        UINT32 "ItemNum",
    }[2] "RefineNeed",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrMin",
        UINT32 "AttrMax",
    }[2] "SuitAttrRangeList",
}



Config "dataconfig_suitperfect" {
    UINT32 "SuitEquipTypeID",
    UINT32 "Perfect",
    UINT32 "Rate",
}



Config "dataconfig_suitrefine" {
}



Config "dataconfig_syschannelgoodssource" {
    UINT32 "ID", --unique key
    STRING "Source",
}



Config "dataconfig_systemequip" {
    UINT32 "ID", --unique key
    UINT32 "LevelRange",
    STRING "Name",
    STRING "Desc",
    STRING "Atlas",
    STRING "Icon",
    STRING "Model",
    UINT32 "labelType",
    UINT32 "ItemLevel",
    UINT32 "QuaID",
    UINT32 "SuitID",
    UINT32 "EquipTypeID",
    UINT32 "AttrGroupID",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrNum",
    }[2] "EquipBaseAttr",
    UINT32 "AttrRangeGroupID",
    UINT32 "AttrQuaPreset",
    UINT32 "LimitTypeID",
    UINT32 "StrengthenLevelMax",
    STRING "ItemSource",
    UINT32 "SellFlag",
    UINT32 "Price",
    UINT32 "DecomFlag",
    Struct {
        UINT32 "MateTypeID",
        UINT32 "MateNum",
    }[2] "MaterialList",
    UINT32 "TimeLimit",
    UINT32 "EquipNumMax",
    UINT32 "EquipConType",
    STRING "DropModelID",
    UINT32 "RandomAttr",
    UINT32 "size",
    UINT32 "broadcast",
    UINT32 "Forge",
    UINT32 "AuctionStartPrice",
    UINT32 "AuctionFinalPrice",
    UINT32 "LevelUpMaterial",
    UINT32 "LevelUpMax",
    UINT32 "EnchantingNum",
    UINT32 "Wash",
    UINT32[1] "Recommend";
    UINT32 "SaleType",
    UINT32 "Fighting",
    UINT32 "WashSpell",
    UINT32 "IsDesc",
    STRING "WashSpellDesc",
    UINT32 "EquipID",
    UINT32 "SuitEquipTypeID",
    UINT32 "SuitFighting",
}



Config "dataconfig_systemrule" {
    UINT32 "ID", --unique key
    STRING "title",
    STRING "txt",
}



Config "dataconfig_tab" {
    UINT32 "TabID", --unique key
    STRING "TabName",
    UINT32 "SpeakAllow",
    UINT32 "SpeakChannel",
    UINT32 "Error",
}



Config "dataconfig_tabinfo" {
    UINT32 "ID", --unique key
    STRING "Text",
    STRING "Icon",
}



Config "dataconfig_tabmenu" {
    UINT32 "menuID", --multi key
    STRING "chbeizhu",
    STRING "meanName",
    Struct {
        STRING "childName",
        UINT32 "CameraID",
        STRING "spriteName",
        UINT32[8] "optionsID";
        UINT32 "kind",
    }[6] "childMenu",
}



Config "dataconfig_tagname" {
    SINT32 "id", --unique key
    STRING "name",
    UINT32 "type",
}



Config "dataconfig_taskafter" {
    UINT32 "TaskID", --unique key
    STRING "TaskContent",
    UINT32 "EventNum",
}



Config "dataconfig_taskbefore" {
    UINT32 "TaskID", --unique key
    STRING "TaskContent",
    UINT32 "TaskEventID",
    UINT32 "EventType",
    UINT32 "Step",
    UINT32[2] "EventParam";
    UINT32 "EventNum",
    UINT32 "GoodID",
    UINT32 "GoodNum",
    UINT32 "BeTrainExp",
    UINT32 "TaskSort",
}



Config "dataconfig_taskdetails" {
    UINT32 "TaskID",
    STRING "TaskName",
    UINT32 "AddType",
    UINT32 "Param",
    UINT32 "ActivityID",
}



Config "dataconfig_taskinfo" {
    UINT32 "ID", --unique key
    STRING "Title",
    STRING "Desc",
    UINT32 "TaskType",
    UINT32[1] "GetTaskNpcID";
    UINT32 "GetTalkID",
    UINT32[2] "WorkingTaskNpcID";
    UINT32 "WorkingTalkID",
    UINT32[2] "FinishTaskNpcID";
    UINT32 "FinishTalkID",
    UINT32 "TimeLimit",
    UINT32 "IsCanGiveUp",
    UINT32[2] "NextTask";
    UINT32 "IsDerictAward",
    UINT32 "IsGetRewardWithNpc",
    UINT32[1] "BeforTaskList";
    UINT32 "BelongTask",
    Struct {
        UINT32 "ActionID",
        UINT32 "param1",
        UINT32 "param2",
    }[2] "AfterComplete",
    UINT32 "LevelMin",
    UINT32 "LevelMax",
    UINT32 "MustCount",
    Struct {
        UINT32 "IsMust",
        UINT32 "ActionType",
        SINT32 "Param",
        UINT32 "Param2",
        UINT32 "NeedNum",
        SINT32[10] "TargetInfo";
        UINT32 "IsSpecial",
        SINT32[15] "TargetPositions";
        STRING "TaskDes",
        STRING "FinishDes",
    }[3] "CompleteLimit",
    UINT32 "GoldNum",
    UINT32 "EXPNum",
    Struct {
        UINT32 "ItemID",
        UINT32 "Num",
    }[4] "NomalAward",
    Struct {
        UINT32 "Weight",
        UINT32 "ItemID",
        UINT32 "Num",
    }[5] "RandomAward",
    Struct {
        Struct {
            UINT32 "ItemID",
            UINT32 "Num",
        }[2] "SigleAward",
    }[5] "ChooseAward",
    UINT32 "PickID",
    UINT32 "TaskMonstePreload",
    Struct {
    }[4] "GetItem",
    UINT32 "SpecialTask",
}



Config "dataconfig_taskinfo_ios" {
    UINT32 "ID",
    STRING "Title",
    STRING "Desc",
    UINT32[1] "GetTaskNpcID";
    UINT32 "GetTalkID",
    UINT32[2] "WorkingTaskNpcID";
    UINT32 "WorkingTalkID",
    UINT32[2] "FinishTaskNpcID";
    UINT32 "FinishTalkID",
    UINT32 "TimeLimit",
    UINT32 "IsCanGiveUp",
    UINT32[2] "NextTask";
    UINT32 "IsDerictAward",
    UINT32 "IsGetRewardWithNpc",
    UINT32[1] "BeforTaskList";
    UINT32 "BelongTask",
    Struct {
        UINT32 "ActionID",
        UINT32 "param1",
        UINT32 "param2",
    }[2] "AfterComplete",
    UINT32 "LevelMin",
    UINT32 "LevelMax",
    UINT32 "MustCount",
    Struct {
        UINT32 "IsMust",
        UINT32 "ActionType",
        SINT32 "Param",
        UINT32 "Param2",
        UINT32 "NeedNum",
        SINT32[10] "TargetInfo";
        UINT32 "IsSpecial",
        SINT32[15] "TargetPositions";
        STRING "TaskDes",
        STRING "FinishDes",
    }[3] "CompleteLimit",
    UINT32 "GoldNum",
    UINT32 "EXPNum",
    Struct {
        UINT32 "ItemID",
        UINT32 "Num",
    }[4] "NomalAward",
    Struct {
        UINT32 "Weight",
        UINT32 "ItemID",
        UINT32 "Num",
    }[5] "RandomAward",
    Struct {
        Struct {
            UINT32 "ItemID",
            UINT32 "Num",
        }[2] "SigleAward",
    }[5] "ChooseAward",
    UINT32 "PickID",
    UINT32 "TaskMonstePreload",
    Struct {
    }[4] "GetItem",
}



Config "dataconfig_tasklib" {
    UINT32 "TaskTeamID", --unique key
    UINT32 "NextID",
    STRING "Chapter",
    STRING "Name",
    STRING "Des",
    STRING "Banner",
    Struct {
        UINT32 "Task",
    }[3] "Tasks",
    Struct {
        UINT32 "Item",
        UINT32 "Num",
        STRING "Des",
        UINT32 "Shine",
    }[3] "Reward",
}



Config "dataconfig_tasknpc" {
    UINT32 "TaskID",
    UINT32 "NPCID",
    UINT32 "LevelID",
    SINT32[6] "NPCLocation";
    UINT32 "DestroyTaskID",
}



Config "dataconfig_tasknpc_ios" {
    UINT32 "TaskID",
    UINT32 "NPCID",
    UINT32 "LevelID",
    SINT32[6] "NPCLocation";
    UINT32 "DestroyTaskID",
}



Config "dataconfig_taskquestion" {
    UINT32 "ID", --unique key
    STRING "Question",
    UINT32 "RightAnswer",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
    UINT32 "EXP",
    UINT32 "npcID",
}



Config "dataconfig_taskreward" {
    UINT32 "TaskNum", --unique key
    UINT32 "GoodID",
    UINT32 "GoodNum",
    UINT32 "BeTrainExp",
}



Config "dataconfig_tasktype" {
    UINT32 "TaskID", --unique key
    STRING "TaskName",
    UINT32 "TaskType",
    UINT32 "AddType",
    UINT32 "Param",
    UINT32 "ActivityID",
}



Config "dataconfig_teammodel" {
    UINT32 "ModelID",
    UINT32 "Relationship",
    UINT32[7] "Button";
}



Config "dataconfig_teamtarget" {
    UINT32 "ID",
    UINT32 "Type",
    UINT32 "ActivityID",
    UINT32 "LevelID",
    UINT32 "RecommendLevel",
    UINT32 "Openday",
}



Config "dataconfig_teamtargettab" {
    UINT32 "ID",
    STRING "Name",
    UINT32[5] "IDList";
}



Config "dataconfig_teamword" {
    UINT32 "ID",
    STRING "Words",
}



Config "dataconfig_tempurl" {
    STRING "PackageName",
    STRING "URL",
}



Config "dataconfig_territory" {
    UINT32 "ID", --unique key
    STRING "Name",
    STRING "Desc",
    UINT32 "Type",
    STRING "ligeanceType",
    UINT32 "UIAttach",
    UINT32[2] "DownList";
    UINT32 "UpID",
    UINT32 "LevelID",
    UINT32[3] "LingShiIDList";
    UINT32 "MaxDeclWarCnt",
    UINT32 "LeastDeclCost",
    UINT32 "LeastAddPercent",
}



Config "dataconfig_territorycontest" {
    UINT32 "ID", --unique key
    STRING "Name",
    UINT32 "LimitTopPercent",
    UINT32 "DeathSpeedBuff",
    UINT32 "ReviveTime",
    UINT32 "OriginLimitTime",
    UINT32[2] "TakeStoneBuff";
    UINT32 "TakeStoneTime",
    UINT32 "PointTick",
    UINT32[4] "StonePoint";
    UINT32 "StageTime",
    UINT32 "LostLimitTime",
    UINT32[3] "LostLimitDis";
}



Config "dataconfig_textcolor" {
    UINT32 "ID", --unique key
    UINT32[3] "Top";
    UINT32[3] "Bottom";
    UINT32[3] "Outline";
}



Config "dataconfig_textgroup" {
    UINT32 "textGroupID", --unique key
    UINT32 "textBelong",
    Struct {
        STRING "group",
    }[2] "groupList",
}



Config "dataconfig_textparttravel" {
    UINT32 "textPartID", --unique key
    Struct {
        STRING "text",
    }[4] "textList",
}



Config "dataconfig_titlebox" {
    UINT32 "ActivityID",
    UINT32 "PositionName",
    UINT32 "QuaID",
    UINT32 "itemID",
    Struct {
        UINT32 "PropertyID",
        UINT32 "Value",
    }[5] "Property",
}



Config "dataconfig_titleinfo" {
    UINT32 "ID",
    UINT32 "Tab", --multi key
    STRING "Title",
    STRING "Brief",
    UINT32 "Type",
    Struct {
        UINT32 "Type",
        UINT32 "Param",
    }[4] "UnlockList",
    UINT32 "LimitTm",
    UINT32 "QuaID",
    UINT32 "TextColor",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrValue",
    }[5] "AddAttr",
}



Config "dataconfig_titleinfoconfig" {
    UINT32 "TitleID", --unique key
    STRING "Name",
    UINT32 "TitleType", --multi key
    UINT32 "Sequence",
    UINT32 "LimitTm",
    STRING "Resource",
    STRING "GetMethod",
    Struct {
        UINT32 "Type",
        UINT32 "Param",
    }[4] "UnlockList",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrValue",
    }[5] "CollectAttr",
    UINT32 "CollectPower",
    Struct {
        UINT32 "AttrID",
        UINT32 "AttrValue",
    }[5] "UseAttr",
    UINT32 "UsePower",
}



Config "dataconfig_titletab" {
    UINT32 "ID",
    STRING "Text",
}



Config "dataconfig_titletypeconfig" {
    UINT32 "TitleType",
    STRING "Name",
}



Config "dataconfig_topic" {
    UINT32 "ID", --unique key
    STRING "Title",
    UINT32 "typeco",
    STRING "PicName",
    STRING "TopicPicName",
    UINT32 "MainTopic",
    UINT32[6] "startTime";
    UINT32[6] "endTime";
}



Config "dataconfig_topicdialogue" {
    UINT32 "id",
    STRING "question",
    SINT32 "index",
    Struct {
        STRING "answer",
        SINT32 "jumpID",
        SINT32 "favor",
        SINT32 "showTime",
    }[2] "items",
}



Config "dataconfig_tradecenterconfig" {
    UINT32 "ItemId",
    UINT32 "Type1ID",
    UINT32[1] "Type2ID";
    UINT32 "BasicPrice",
    UINT32 "BasicCount",
    UINT32 "Tax",
    SINT32 "MoneyType",
    UINT32 "ServerLV",
    UINT32 "EquipTypeID",
    UINT32 "IsShow",
}



Config "dataconfig_tradeitemtypes" {
    UINT32 "TypeID",
    STRING "Name",
    Struct {
        UINT32 "childType",
        STRING "childName",
    }[15] "childTypeList",
}



Config "dataconfig_uifenbao" {
    STRING "Name", --unique key
    STRING "Name2",
    UINT32 "Pakage",
}



Config "dataconfig_unlocklevel" {
    UINT32 "id", --unique key
    Struct {
        SINT32[4] "slotIndex";
        SINT32[4] "lowLevel";
    }[7] "btnConfigArray",
}



Config "dataconfig_usequenceinfo" {
    SINT32 "id", --unique key
    Struct {
        STRING "res",
    }[19] "src",
}



Config "dataconfig_vipemail" {
}



Config "dataconfig_viplevelright" {
    UINT32 "VipLevel",
    UINT32 "NeedMoney",
    STRING "TabMark",
    UINT32 "MaxView",
    Struct {
        UINT32 "ItemId",
        UINT32 "ItemNum",
    }[5] "Gift",
    UINT32 "OriPrice",
    UINT32 "Price",
    STRING "ModelPath",
    UINT32 "ModID",
    UINT32 "ImgType",
    UINT32 "ImgUrl",
    STRING "MainPrivilege",
    UINT32 "MainPrivilegeUrl",
    UINT32 "VipLogo",
    UINT32 "HeadPortraitLevel",
    UINT32 "WorldSpeechTime",
    UINT32 "DonateTime",
    UINT32 "ArenaTime",
    UINT32 "VipStore",
    UINT32 "VipStoreUnlock",
    UINT32 "SanqiuDan",
    UINT32 "IntimacyRate",
    UINT32 "StallTime",
    UINT32 "RedRate",
    UINT32 "OfflineExpRate",
    UINT32 "FamilyGiftTime",
    UINT32 "FamilySpeekTime",
    UINT32 "OneMoney",
    UINT32 "ThreeMoney",
    UINT32 "SixMoney",
    UINT32 "SpeechIntervalTime",
    UINT32 "PassTime",
    UINT32 "SufferPassTime",
    UINT32 "LevelDisparity",
    UINT32 "HelpTime",
    UINT32 "SeekHelpTime",
    UINT32 "ByfordGodTime",
    UINT32 "StallItemNum",
    UINT32 "FontanaTreviRate",
    UINT32 "AddFriendsNum",
    UINT32 "ShenShouShopRefreshTime",
    UINT32 "ShenShouRecycleShopTime",
    UINT32 "ShiTuLevelDisparity",
    UINT32 "LevepUpTips",
    UINT32 "WingAutoUpdate",
    UINT32 "FairyAutoUpdate",
    UINT32 "HeroAutoUpdate",
    UINT32 "ContinuousConstruction",
    UINT32 "GiftClothe",
    UINT32 "FairylandTime",
}



Config "dataconfig_viptemplate" {
    STRING "Id", --unique key
    Struct {
        STRING "Text",
        SINT32 "Type",
    }[3] "Template",
}



Config "dataconfig_voicegrade" {
    UINT32 "VoiceGrade", --unique key
    STRING "VoiceGradeName",
    UINT32 "DropTeamID",
}



Config "dataconfig_wanted" {
    UINT32 "wantedID",
    UINT32 "level",
    UINT32 "MonsterID",
    STRING "Name",
    STRING "Description",
    UINT32 "Max",
    UINT32 "LimitTime",
    UINT32 "AwardID",
    UINT32 "RankAwardID",
    UINT32 "PosID",
    STRING "PicName",
}



Config "dataconfig_wantedpos" {
    UINT32 "PosID",
    STRING "MapName",
    UINT32 "MapID",
}



Config "dataconfig_wardrobebox" {
    UINT32 "GoodsID", --unique key
    Struct {
        UINT32 "OutWardID",
        UINT32 "OutWardType",
    }[5] "OutWardList",
    UINT32 "Cost",
}



Config "dataconfig_warofroyalcityaward" {
    UINT32 "Score",
    UINT32 "GoodsID",
    UINT32 "GoodsNum",
}



Config "dataconfig_washes" {
    UINT32 "Level", --multi key
    UINT32 "EquipTypeID", --multi key
    UINT32 "GoodID",
    UINT32 "GoodNum",
    UINT32 "AdvancedGoodID",
    UINT32 "AdvancedGoodNum",
    UINT32 "TemperatureAdd",
    Struct {
        UINT32 "Key",
        SINT32 "NumMin",
        SINT32 "NumMax",
    }[17] "AddAttrInfo",
}



Config "dataconfig_washqua" {
    UINT32 "LevelRange",
    UINT32 "EquipConType",
    UINT32 "AttrRangeGroup", --multi key
    UINT32 "AttrWeight",
    UINT32 "AttrID", --multi key
    Struct {
        UINT32 "RangeMin",
        UINT32 "RangeMax",
        UINT32 "QuaID",
        UINT32 "Fighting",
    }[6] "AttrQuaRangeList",
}



Config "dataconfig_washstau" {
    UINT32 "Level",
    UINT32 "Rate",
}



Config "dataconfig_washtemp" {
    UINT32 "Level",
    UINT32 "Temperature",
    UINT32 "CostDown",
}



Config "dataconfig_wavereminders" {
    UINT32 "Wave",
    UINT32 "BigWave", --unique key
    UINT32 "CountTime",
    UINT32 "LocalId",
}



Config "dataconfig_wedding" {
    UINT32 "Stage", --unique key
    STRING "PhaseName",
    UINT32 "PhaseTime",
    STRING "PhaseDescription",
}



Config "dataconfig_weddinganniversary_preview" {
    UINT32 "WeddingType",
    UINT32 "Stage",
    UINT32[1] "ItemNum";
    UINT32 "Gender",
    UINT32[3] "Preview_ItemID";
}



Config "dataconfig_weddingcamera" {
    UINT32 "CameraID",
    SINT32[3] "CameraPos";
    UINT32 "CameraHight",
    UINT32 "CameraFov",
}



Config "dataconfig_weddingdescription" {
    UINT32 "ID", --unique key
    STRING "Title",
    STRING "Text",
}



Config "dataconfig_wfexpand" {
    UINT32 "Number",
}



Config "dataconfig_wing_change" {
    UINT32 "WingID", --unique key
    UINT32 "ResID", --unique key
    UINT32 "WingScale",
    STRING "Name",
    UINT32 "FightNum",
    STRING "ActivateDec",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[4] "Attr",
    UINT32 "WingsPoint",
    UINT32 "OpenServerLevel",
}



Config "dataconfig_wing_class" {
    UINT32 "WingClass", --unique key
    UINT32 "ResID", --unique key
    UINT32 "WingScale",
    STRING "Name",
    STRING "Atlas",
    STRING "Icon",
    UINT32 "Level",
    UINT32 "StarNum",
    UINT32 "WingsPoint",
}



Config "dataconfig_wing_soul" {
    UINT32 "SoulID", --multi key
    UINT32 "tupoClass", --multi key
    UINT32 "OpenClass",
    UINT32 "OpenStar",
    UINT32 "ShowClass",
    UINT32 "ActivateItemID",
    UINT32 "tupoActivateItemID",
    UINT32 "tupoActivateItemNum",
    UINT32 "tupoNum",
    UINT32 "ActivateTimes",
    UINT32 "FightNum",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[4] "Attr",
}



Config "dataconfig_wing_update" {
    UINT32 "WingClass", --multi key
    UINT32 "Star",
    UINT32 "CostItemID",
}



Config "dataconfig_wing_updateattribte" {
    UINT32 "WingClass", --multi key
    UINT32 "Star",
    Struct {
        UINT32 "AttrType",
        UINT32 "AttrValue",
    }[6] "Attr",
}



Config "dataconfig_wishtype" {
    UINT32 "WishType",
    UINT32 "WishCost",
    UINT32 "WishGetMoney",
}



Config "dataconfig_wldhgroup" {
    UINT32 "GroupID", --unique key
    STRING "GroupDesc",
}



Config "dataconfig_wordtext" {
    UINT32 "ID",
    STRING "WordStr",
    STRING "WordDes",
    UINT32 "WordType",
}



Config "dataconfig_worldattr" {
    UINT32 "ID",
    UINT32 "PlayerMoveSpeed",
    UINT32[4] "CameraHeightList";
}



Config "dataconfig_worldbigmap" {
    UINT32 "ID",
    UINT32 "LevelID",
    UINT32 "UiLevel",
    UINT32 "TransferLevel",
    STRING "Name",
    UINT32 "IsMainMap",
}



Config "dataconfig_worldboss" {
    UINT32 "GroupID",
    STRING "name",
    UINT32 "GroupType",
    UINT32 "CombatEffectiveness",
    UINT32 "BossID",
    STRING "BossName",
    UINT32 "MapID",
    STRING "PicName",
    UINT32 "Bonus",
}



Config "dataconfig_worldhonorrank" {
    UINT32 "HonorID",
    UINT32 "ActivityID", --multi key
    UINT32 "LevelID", --multi key
    Struct {
        UINT32 "EventTypeID",
        UINT32 "EventNum",
    }[4] "EventList",
    Struct {
        UINT32 "GoodID",
        UINT32 "GoodNum",
    }[4] "AwardList",
    UINT32 "DropTeamID",
}



Config "dataconfig_worldmap" {
    UINT32 "ID", --unique key
    STRING "CityName",
    STRING "iconPath",
    UINT32 "uiBaseID",
    UINT32 "typeID",
    UINT32[8] "StoryTask";
    UINT32 "BigBossID",
    SINT32[3] "BigBossPoint";
    UINT32 "AreaID",
}



Config "dataconfig_worldmaparea" {
    UINT32 "ID", --unique key
    UINT32[6] "PointList";
    UINT32 "SwitchPoint",
}



Config "dataconfig_worldredbag" {
    UINT32 "Id", --unique key
    STRING "Name",
    STRING "Key",
    UINT32 "Num",
    STRING "StartTime",
    STRING "EndTime",
}



Config "dataconfig_worldvoiceredpacket" {
    UINT32 "RPConfigID",
    UINT32[3] "TitleType";
    STRING "Name",
    UINT32 "Grade",
    UINT32 "Weapon",
    UINT32 "IconID",
    UINT32 "DefaultGold",
    UINT32 "DefaultNum",
    UINT32 "ConsalationItemID",
    UINT32 "ConsalationItemNum",
}



Config "dataconfig_worldvoiceredpackettitle" {
    UINT32 "RPTitleID",
    UINT32 "TitleType",
    STRING "Question",
    STRING "Answer",
}



Config "dataconfig_worldvoiceredpackettitletype" {
    UINT32 "TitleType",
    STRING "Name",
}



Config "dataconfig_xiaomingjiang" {
    UINT32 "GroupID",
    UINT32 "RankMin",
    UINT32 "RankMax",
    UINT32 "BossID", --unique key
    STRING "BossName",
    UINT32 "MapID",
    UINT32 "RobotNumber",
    UINT32 "AI1",
    UINT32 "AI2",
}



Config "dataconfig_yewaiboss" {
    UINT32 "BossID",
    UINT32 "BossLev",
    UINT32 "MapID",
    UINT32 "AHID",
    Struct {
        SINT32[3] "coordinate";
    }[5] "Coordination",
    UINT32 "GangWinBonusGroup",
    UINT32 "JoinBonusGroup",
    UINT32[3] "ScaleUI";
    SINT32[3] "RotationUI";
    STRING "Album",
    STRING "Icon",
    UINT32 "RewardPreview",
    UINT32 "ModeID",
    STRING "MapIcon",
}



Config "dataconfig_zhenxinhua" {
    UINT32 "ID", --unique key
    STRING "Question",
    STRING "Answer1",
    STRING "Answer2",
    STRING "Answer3",
    STRING "Answer4",
}



Config "dataconfig_zhufu" {
    UINT32 "ID",
    STRING "Description",
    UINT32 "ChatlinkID",
}



Config "dataconfig_zmlactivitytime" {
}



