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
function LuaExportAfterNextFrame()
    rawSelfData=LoGetSelfData()
    if(rawSelfData==nil)
    then
        return;
    end
    campos= LoGetCameraPosition()
    pitch, bank, yaw = LoGetADIPitchBankYaw()
    Angular = LoGetAngularVelocity()
    Velocity = LoGetVectorVelocity()
    Wind = LoGetVectorWindVelocity()
    jsonstring='{"DATA":{'
    %%INSERTHERE%%
    jsonstring=jsonstring..'"Name":"'..rawSelfData["Name"]..'",'
    jsonstring=jsonstring..'"Country":"'..rawSelfData["Country"]..'",'
    jsonstring=jsonstring..'"Lat":"'..rawSelfData["LatLongAlt"]["Lat"]..'",'
    jsonstring=jsonstring..'"Long":"'..rawSelfData["LatLongAlt"]["Long"]..'",'
    jsonstring=jsonstring..'"Alt":"'..LoGetAltitudeAboveSeaLevel()..'",'
    jsonstring=jsonstring..'"Heading":"'..rawSelfData["Heading"]..'",'
    jsonstring=jsonstring..'"Pitch":"'..rawSelfData["Pitch"]..'",'
    jsonstring=jsonstring..'"Bank":"'..rawSelfData["Bank"]..'",'
    jsonstring=jsonstring..'"PositionX":"'..rawSelfData["Position"]["x"]..'",'
    jsonstring=jsonstring..'"PositionY":"'..rawSelfData["Position"]["y"]..'",'
    jsonstring=jsonstring..'"PositionZ":"'..rawSelfData["Position"]["z"]..'",'
    jsonstring=jsonstring..'"AngleOfAttack":"'..LoGetAngleOfAttack()..'",'
    jsonstring=jsonstring..'"AngleOfSideSlip":"'..LoGetAngleOfSideSlip()..'",'
    jsonstring=jsonstring..'"IndicatedAirSpeed":"'..LoGetIndicatedAirSpeed()..'",'
    jsonstring=jsonstring..'"TrueAirSpeed":"'..LoGetTrueAirSpeed()..'",'
    jsonstring=jsonstring..'"MachNumber":"'..LoGetMachNumber()..'",'
    jsonstring=jsonstring..'"RadarAltimeter":"'..LoGetAltitudeAboveGroundLevel()..'",'
    jsonstring=jsonstring..'"CamPosXX":"'..campos["x"]["x"]..'",'
    jsonstring=jsonstring..'"CamPosXY":"'..campos["x"]["y"]..'",'
    jsonstring=jsonstring..'"CamPosXZ":"'..campos["x"]["z"]..'",'
    jsonstring=jsonstring..'"CamPosYX":"'..campos["y"]["x"]..'",'
    jsonstring=jsonstring..'"CamPosYY":"'..campos["y"]["y"]..'",'
    jsonstring=jsonstring..'"CamPosYZ":"'..campos["y"]["z"]..'",'
    jsonstring=jsonstring..'"CamPosZX":"'..campos["z"]["x"]..'",'
    jsonstring=jsonstring..'"CamPosZY":"'..campos["z"]["y"]..'",'
    jsonstring=jsonstring..'"CamPosZZ":"'..campos["z"]["z"]..'",'
    jsonstring=jsonstring..'"CamPosPX":"'..campos["p"]["x"]..'",'
    jsonstring=jsonstring..'"CamPosPY":"'..campos["p"]["y"]..'",'
    jsonstring=jsonstring..'"CamPosPZ":"'..campos["p"]["z"]..'",'
    jsonstring=jsonstring..'"ADIPitch":"'..pitch..'",'
    jsonstring=jsonstring..'"ADIBank":"'..bank..'",'
    jsonstring=jsonstring..'"ADIYaw":"'..yaw..'",'
    jsonstring=jsonstring..'"AngularX":"'..Angular["x"]..'",'
    jsonstring=jsonstring..'"AngularY":"' ..Angular["y"]..'",'
    jsonstring=jsonstring..'"AngularZ":"' ..Angular["z"]..'",'
    jsonstring=jsonstring..'"VelocityX":"'..Velocity["x"]..'",'
    jsonstring=jsonstring..'"VelocityY":"'..Velocity["y"]..'",'
    jsonstring=jsonstring..'"VelocityZ":"'..Velocity["z"]..'",'
    jsonstring=jsonstring..'"WindX":"'..Wind["x"]..'",'
    jsonstring=jsonstring..'"WindY":"'..Wind["y"]..'",'
    jsonstring=jsonstring..'"WindZ":"'..Wind["z"]..'"}}'

    returnstring = jsonstring.."\n"
	socket.try(c:send(returnstring))
    aircraft_old=aircraft
    
end