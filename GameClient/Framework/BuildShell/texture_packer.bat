setlocal enabledelayedexpansion
if not exist %2 md %2
cd %1
SET PNGS=
for %%i in (*.png) do (
    SET PNGS=!PNGS! %%i
)
TexturePacker --smart-update --texture-format png --force-squared --format "phaser" %5 %6 --data "%2\%3".json.txt --sheet "%2\%3_{n}".png --maxrects-heuristics best --scale 1 --shape-padding 1 --multipack --max-size 1024 --opt "%4" --size-constraints POT%PNGS%