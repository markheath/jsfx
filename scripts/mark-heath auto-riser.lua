-- Auto-riser, by Mark Heath
-- v0.01 (10 Feb 2023) initial version (beta)

reaProject = 0
effectName = "ValhallaSupermassive"
presetName = "Synth Cloud"

function messageBox(message)
  reaper.ShowMessageBox("Must select exactly one item", "Auto-riser", 0)
end

function addFxToTake(take, fxName, preset)
  fx = reaper.TakeFX_AddByName(take, effectName, 1)
  setFx = reaper.TakeFX_SetPreset(take, fx, presetName)
  if not setFx then
    messageBox("Couldn't load preset " + presetName)
  end

end


-- 1. count how many items are selected, 0 for current project
mediaItemCount = reaper.CountSelectedMediaItems(reaProject)

if (mediaItemCount == 0) or (mediaItemCount > 1) then
  messageBox("Must select exactly one item")
  return
end

-- 2. get the one selected media item
mediaItem = reaper.GetSelectedMediaItem(reaProject, 0)

takeCount = reaper.CountTakes(mediaItem)

take = reaper.GetActiveTake(mediaItem)

-- 3. Find out if the take is MIDI
midi = reaper.TakeIsMIDI(take)
if midi then
  -- Render the MIDI to an audio take
  -- "Item: Render items to new take" 41999 (leaves it as MIDI)
  -- "Item: Apply track/take FX to items" 40209 (gets us to audio)
  reaper.Main_OnCommandEx(40209, 0, reaProject)
  -- now the active take should be the new one
  take = reaper.GetActiveTake(mediaItem)
end


-- 4. Add a very wet reverb to the current take (only if already doesn't exist)
addFxToTake(take, effectName, presetName)

-- Apply the FX first (40209 again)
reaper.Main_OnCommandEx(40209, 0, reaProject)
-- "Item: Reverse items to new take" 40270
reaper.Main_OnCommandEx(40270, 0, reaProject)
-- This take is the reversed take
take = reaper.GetActiveTake(mediaItem)

-- now add the FX onto the reversed take
addFxToTake(take, effectName, presetName)

  
  

