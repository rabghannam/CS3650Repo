# Settings System Setup Guide

## Overview
This guide covers setting up the **SettingsManager** and **SettingsPanel** to work with both MainMenu and PauseMenu.

## Files Created
- `SettingsManager.cs` - Manages settings data and persistence
- `SettingsPanel.cs` - UI controller for settings panel

## Step 1: Create SettingsManager

### Option A: Single Singleton (Recommended)
1. Create an empty GameObject in **MainMenuScene** → Name: **"SettingsManager"**
2. Add `SettingsManager` script component
3. This will automatically persist across scenes (DontDestroyOnLoad)

### Option B: Separate for Each Scene
1. Create SettingsManager in **MainMenuScene**
2. Create another SettingsManager in **GameScene**
3. Both will work independently (settings saved separately)

### Option C: Auto-Create (Best - Now Active!)
✅ **No manual setup needed!**
- SettingsManager now **auto-creates itself** when first accessed
- Works from **any starting scene** (MainMenu, GameScene, etc.)
- Still maintains singleton pattern with DontDestroyOnLoad
- **Recommended**: Just use `SettingsManager.Instance` anywhere - it handles the rest!

**Note:** Option C is now the default. You can still manually create SettingsManager instances if needed, but it's no longer required.

## Step 2: Create SettingsPanel Prefab

### Create the UI Panel
1. Right-click on **Canvas** → **UI → Panel** → Name: **"SettingsPanel"**
2. Set Anchor Preset to **Center-Center**
3. Add a **Vertical Layout Group** component for automatic layout

### Add UI Elements

**Volume Slider:**
- Right-click on SettingsPanel → **UI → Slider** → Name: "VolumeSlider"
- Set Min Value: 0, Max Value: 1
- Set Whole Numbers: false

**Fullscreen Toggle:**
- Right-click on SettingsPanel → **UI → Toggle** → Name: "FullscreenToggle"
- Change label text to "Fullscreen"

**Resolution Dropdown:**
- Right-click on SettingsPanel → **UI → Dropdown** → Name: "ResolutionDropdown"
- Set Dropdown event to populate with screen resolutions

**Apply Button:**
- Right-click on SettingsPanel → **UI → Button** → Name: "ApplyButton"
- Text: "APPLY"

**Back Button:**
- Right-click on SettingsPanel → **UI → Button** → Name: "BackButton"
- Text: "BACK"

### Add SettingsPanel Script
1. Select **SettingsPanel** GameObject
2. Add `SettingsPanel` script component
3. Drag UI references:
   - **volumeSlider**: VolumeSlider
   - **fullscreenToggle**: FullscreenToggle
   - **resolutionDropdown**: ResolutionDropdown
   - **applyButton**: ApplyButton
   - **backButton**: BackButton

### Create Prefab
1. **Important:** Make sure SettingsPanel is **disabled** (unchecked) in Inspector
2. Drag **SettingsPanel** from Hierarchy to **Project window** (Assets/Prefabs)
3. Delete the instance from Hierarchy (we'll instantiate fresh ones)

## Step 3: Setup in MainMenuScene

1. **Instantiate SettingsPanel Prefab:**
   - Drag prefab into Hierarchy as child of **Canvas**
   - Rename if needed (e.g., "MainMenuSettingsPanel")
   - **CRITICAL:** Make sure it's **disabled** (unchecked) in Inspector
   - The prefab should already be disabled, but verify

2. **Update MainMenu:**
   - Select **MainMenuManager**
   - Drag **SettingsPanel** instance to `settingsPanel` field
   - In **SettingsPanel** component:
     - Drag **MainMenuManager** to `mainMenu` field
     - Leave `pauseMenu` empty

3. **Update Settings Button:**
   - Settings button OnClick should call: `MainMenu → ShowSettings()`

## Step 4: Setup in GameScene (PauseMenu)

1. **Instantiate SettingsPanel Prefab:**
   - Drag prefab into Hierarchy as child of **Canvas**
   - Rename (e.g., "PauseMenuSettingsPanel")
   - **CRITICAL:** Make sure it's **disabled** (unchecked) in Inspector
   - The prefab should already be disabled, but verify

2. **Update PauseMenu:**
   - Select **PauseMenuManager**
   - Drag **SettingsPanel** instance to `settingsPanel` field
   - In **SettingsPanel** component:
     - Drag **PauseMenuManager** to `pauseMenu` field
     - Leave `mainMenu` empty

3. **Update Settings Button:**
   - Settings button OnClick should call: `PauseMenu → ShowSettings()`

## Step 5: Configure SettingsManager

In **SettingsManager** component (in MainMenuScene):
- Default settings are already configured:
  - Volume: 1.0 (100%)
  - Fullscreen: true
  - Resolution: 1920x1080

You can adjust defaults in the script if needed.

## How It Works

1. **MainMenu → ShowSettings()** → Hides main panel, shows SettingsPanel
2. **SettingsPanel** → User changes settings → Updates SettingsManager
3. **Apply Button** → Saves settings to PlayerPrefs and applies them
4. **Back Button** → Reverts to saved settings, returns to MainMenu

Same flow for PauseMenu.

## Features
- ✅ Volume slider (0-100%)
- ✅ Fullscreen toggle
- ✅ Resolution dropdown (auto-populated from system)
- ✅ Apply button to save
- ✅ Back button to return
- ✅ Settings persist via PlayerPrefs
- ✅ Works in both MainMenu and PauseMenu
- ✅ Prefab-based for reusability

## Troubleshooting

### "Settings menu is visible when playing"
**Solution:** 
1. Select your SettingsPanel GameObject
2. In Inspector, **uncheck** the box at the top (disables the GameObject)
3. Make sure your prefab is also disabled
4. Verify PauseMenu.cs and MainMenu.cs hide the panel in Start()

### Settings not saving
- Check if SettingsManager exists in the scene
- Verify PlayerPrefs is working (test with PlayerPrefs.SetInt("test", 1))

### Resolution dropdown empty
- Make sure the dropdown is assigned in SettingsPanel component
- Some platforms return empty resolutions array in editor

## Tips
- Make sure **AudioListener** exists in your scene (usually on main camera)
- Test different resolutions in editor to ensure they work
- Settings are saved to PlayerPrefs and loaded on startup