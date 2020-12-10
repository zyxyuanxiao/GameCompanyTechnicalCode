function import(moduleName, currentModuleName)
    local currentModuleNameParts
    local moduleFullName = moduleName
    local offset = 1

    while true do
        if string.byte(moduleName, offset) ~= 46 then -- .
            moduleFullName = string.sub(moduleName, offset)
            if currentModuleNameParts and #currentModuleNameParts > 0 then
                moduleFullName = table.concat(currentModuleNameParts, ".") .. "." .. moduleFullName
            end
            break
        end
        offset = offset + 1

        if not currentModuleNameParts then
            if not currentModuleName then
                local n,v = debug.getlocal(3, 1)
                currentModuleName = v
            end

            currentModuleNameParts = string.split(currentModuleName, ".")
        end
        table.remove(currentModuleNameParts, #currentModuleNameParts)
    end

    return require(moduleFullName)
end

local vistor = nil
local stack = {}

local function CanVistor(o)
    if o == typeof then
        return false
    end

    return type(o) == "table" or type(o) == "function"
end

local function FindObject(obj, dest)
    if dest == nil then
        return false
    end

    if vistor[dest] then
        return false
    end

    vistor[dest] = true
    local t = type(dest)

    if t == "table" then
        for k, v in pairs(dest) do                
            table.insert(stack, tostring(k) or "null")

            if k == obj or v == obj then
                return true
            end
            
            if CanVistor(k) and FindObject(obj, k) then
                return true
            end

            if CanVistor(v) and FindObject(obj, v) then
                return true
            end
                                    
            table.remove(stack)                                    
        end
    elseif t == "function" then
        local index = 1

        while true do
            local name, value = debug.getupvalue(dest, index)

            if name == nil then
                break
            end

            if name == obj or value == obj then
                table.insert(stack, tostring(name) or "null")
                return true
            end

            if CanVistor(value) and FindObject(obj, value) then
                return true
            end

            index = index + 1
        end
    end
    return false
end

local function ClearArray(t)
    local count = #t

    for i = 1, count do
        t[i] = nil
    end
end

function FindObjectInGlobal(obj)
    vistor = {}    
    setmetatable(vistor, {__mode = "k"})

    FindObject(obj, _G)

    if #stack > 0 then                    
        print("find obj in:"..table.concat(stack, "->").."->"..tostring(obj))
    end    
    
    ClearArray(stack)
    vistor = nil        
end
