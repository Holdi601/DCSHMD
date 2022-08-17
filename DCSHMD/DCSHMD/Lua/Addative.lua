local lfs = require('lfs')
LUA_PATH = "?;?.lua;"..lfs.currentdir().."/Scripts/?.lua"
local aircraft  = ''
local aircraft_old  = ''
function LuaExportStart()
	package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
    package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
    socket = require("socket")
    host = "localhost"
    port = 8532
    c = socket.try(socket.connect(host, port)) -- connect to the listener socket
    c:setoption("tcp-nodelay",true) -- set immediate transmission mode
end
function LuaExportStop()
	socket.try(c:send("exit")) -- to close the listener socket
  	c:close()
end

function roundNum(number, round)
    if(round==0)
    then
        return math.floor(number+0.5)
    end
    if(round<0)
    then
        iterMax =round * -1
        iter = 0
        numnew = number
        while(iter < iterMax)
        do
            numnew = numnew / 10
            iter=iter+1
        end
        iter=0
        numnew = math.floor(numnew+0.5)
        while(iter < iterMax)
        do
            numnew = numnew * 10
            iter=iter+1
        end
        return numnew
    end
    iterMax =round
    iter = 0
    numbnew = number
    while(iter < iterMax)
    do
        numbnew = numbnew * 10
        iter=iter+1
    end
    iter=0
    numbnew = math.floor(numbnew+0.5)
    while(iter < iterMax)
    do
        numbnew = numbnew / 10
        iter=iter+1
    end
    return numbnew
end

function M2Ft(number)
    return (number*3.280839895)
end

function R2Deg(number)
    return (number*(180/math.pi))
end

function DegBVecs(ax, ay, az, bx, by, bz)
	topNum = ax * bx + ay * by + az * bz
	botLeft = ax * ax + ay * ay + az * az
	botRight = bx * bx + by * by + bz * bz
	bLsq = math.sqrt(botLeft)
	bRsq = math.sqrt(botRight)
	botNum = bLsq * bRsq
	bac = topNum / botNum
    return math.acos(bac)
end

function MS2Kn(number)
    return (number*1.9438444924)
end

function LuaExportAfterNextFrame()
    rawSelfData=LoGetSelfData()
    if(rawSelfData==nil)
    then
        return;
    end
    campos= LoGetCameraPosition()
    pitch, bank, yaw = LoGetADIPitchBankYaw()
    Wind = LoGetVectorWindVelocity()
    Accelleration=LoGetAccelerationUnits()
    ViewVec=LoGetCameraPosition()['z']
    jsonstring='{'
    %%INSERTHERE%%

    returnstring = jsonstring.."\n"
	socket.try(c:send(returnstring))
    aircraft_old=aircraft
    
end