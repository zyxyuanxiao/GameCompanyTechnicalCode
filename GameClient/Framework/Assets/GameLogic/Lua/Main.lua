---@class Main
Main = {}


function Main.Main()
	
	local a = System.String.New("aaaaaa")
	
	print(a)
	
	print("<color=green>Lua Already Started</color>")
end

function Main.OnApplicationQuit()
	
	
	print("<color=red>Lua Already Destroy</color>")
end



return Main