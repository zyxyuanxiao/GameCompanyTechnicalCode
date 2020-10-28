#!/bin/bash
# TexturePacker路径
TP=/Applications/TexturePacker.app/Contents/MacOS/TexturePacker

# 资源路径
SOURCE=$1
# echo $SOURCE >> filex.txt
# 目标路径
DEST=$2
# 文件名
NAME=$3

# echo $DEST >> filex.txt
# 定义pack_textures打包函数
# $1: Source Directory where the assets are located
# $2: Output File Name without extension
# $3: RGB Quality factor
# $4: Scale factor
# $5: Max-Size factor
# $6: Texture Type (PNG, PVR.CCZ)
# $7: Texture format
# $8: Texture dither
# $9: Texture dither param

pack_textures() {
	${TP} --smart-update \
        --texture-format $7 \
        --format "phaser" \
        --dither-type "FloydSteinbergAlpha"\
        --data "$2".json.txt \
        --sheet "$2".$6 \
        --maxrects-heuristics best \
        --scale $4 \
        --shape-padding 1 \
        --max-size $5 \
        --opt "$3" \
        --size-constraints AnySize \
        --trim-mode None \
        $1
}

# 如果目标文件夹不存在，则创建
if [ -d $DEST ];then
    :
else
    mkdir $DEST
fi

spriteSheetName=${NAME}

pack_textures ${SOURCE} ${DEST}/${spriteSheetName} RGBA4444 1 4096 png png