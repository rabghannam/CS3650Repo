# Pause Menu Setup Guide

## Files Created
- `PauseMenu.cs` - Main pause menu script
- Updated `Game.cs` - Added pause menu reference and pause state checking

## Unity Setup Instructions

### 1. Create Pause Menu UI Panel
1. In your scene, select the Canvas (or create one if it doesn't exist)
2. Right-click on Canvas → UI → Panel
3. Name it "PauseMenuPanel"
4. Set its Anchor Preset to "Stretch-Stretch" (cover entire screen)
5. Add an Image component with a semi-transparent background (e.g., black with 80% opacity)

### 2. Create Settings Panel
1. Right-click on Canvas → UI → Panel
2. Name it "SettingsPanel"
3. Set its Anchor Preset to "Center-Center" or position as desired
4. Add your settings UI elements (sliders, toggles, etc.)
5. **Hide this panel by default** (uncheck in Inspector)

### 3. Add Buttons to PauseMenuPanel
Create buttons as children of the PauseMenuPanel:

- **Resume Button**: 
  - Create UI → Button
  - Position: Center of screen
  - Text: "RESUME"
  - OnClick: Add PauseMenu component reference and call `Resume()` method

- **Settings Button**:
  - Create UI → Button  
  - Position: Below Resume
  - Text: "SETTINGS"
  - OnClick: Add PauseMenu component reference and call `ShowSettings()` method

- **Quit to Main Menu Button**:
  - Create UI → Button
  - Position: Below Settings
  - Text: "MAIN MENU"
  - OnClick: Add PauseMenu component reference and call `QuitToMainMenu()` method

- **Quit Game Button**:
  - Create UI → Button
  - Position: Below Main Menu button
  - Text: "QUIT GAME"
  - OnClick: Add PauseMenu component reference and call `QuitGame()` method

### 4. Add Back Button to SettingsPanel
1. In SettingsPanel, create UI → Button
2. Name it "BackButton"
3. Text: "BACK"
4. OnClick: Add PauseMenu component reference and call `BackFromSettings()` method

### 3. Add PauseMenu Script
1. Select an empty GameObject in your scene (or create one named "PauseMenuManager")
2. Add the `PauseMenu` script component
3. Drag the PauseMenuPanel to the `panel` field in the inspector
4. Drag the Game GameObject to the `game` field in the inspector

### 4. Update Game Manager
1. Select your Game GameObject
2. In the Game component, drag the PauseMenuManager to the `pauseMenu` field

### 5. Configure Scenes
- Update the `QuitToMainMenu()` method in PauseMenu.cs to use your actual main menu scene name
- Default is "MainMenu" - change it to match your project

## Usage
- Press **Escape** key to toggle pause menu on/off
- Game time stops when paused (Time.timeScale = 0)
- Game time resumes when unpaused (Time.timeScale = 1)

## Features
- ✅ Toggle with Escape key
- ✅ Stops game time
- ✅ Prevents player movement and game updates
- ✅ Resume functionality
- ✅ Settings panel navigation
- ✅ Quit to main menu
- ✅ Quit game
- ✅ Cannot pause when game is over
- ✅ Cannot pause when already paused

## Notes
- The pause menu panel should be a child of a Canvas
- Settings panel should also be a child of the same Canvas
- Buttons should have Button components with proper OnClick events
- Make sure your Canvas has a Graphic Raycaster component
- The pause menu works alongside the existing GameOverScreen
- Settings panel is hidden when game is resumed