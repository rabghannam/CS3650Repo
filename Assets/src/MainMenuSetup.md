# Main Menu Setup Guide

## Files Created
- `MainMenu.cs` - Main menu controller script

## Unity Setup Instructions

### 1. Create Canvas & Panels
- Right-click in Hierarchy → **UI → Canvas** (creates Canvas + EventSystem)
- Right-click on Canvas → **UI → Panel** → Name it **"MainPanel"**
- Right-click on Canvas → **UI → Panel** → Name it **"SettingsPanel"** (hide it)
- Right-click on Canvas → **UI → Panel** → Name it **"CreditsPanel"** (hide it)
- Right-click on Canvas → **UI → Panel** → Name it **"UpgradesPanel"** (hide it)

### 2. Add Buttons to MainPanel
Create these buttons as children of MainPanel:

- **Play Button**: 
  - UI → Button → Name: "PlayButton"
  - Text: "PLAY"
  - OnClick: Add MainMenu component → `PlayGame()`

- **Settings Button**:
  - UI → Button → Name: "SettingsButton"  
  - Text: "SETTINGS"
  - OnClick: Add MainMenu component → `ShowSettings()`

- **Upgrades Button**:
  - UI → Button → Name: "UpgradesButton"
  - Text: "UPGRADES"
  - OnClick: Add MainMenu component → `ShowUpgrades()`

- **Credits Button**:
  - UI → Button → Name: "CreditsButton"
  - Text: "CREDITS"
  - OnClick: Add MainMenu component → `ShowCredits()`

- **Quit Button**:
  - UI → Button → Name: "QuitButton"
  - Text: "QUIT"
  - OnClick: Add MainMenu component → `QuitGame()`

### 3. Add Back Buttons to Sub-Panels
In **SettingsPanel**, **CreditsPanel**, and **UpgradesPanel**, add a Back button:
- UI → Button → Text: "BACK"
- OnClick: Add MainMenu component → `BackToMainMenu()`

### 4. Add MainMenu Script
- Create Empty GameObject → Name: **"MainMenuManager"**
- Add `MainMenu` script component
- Assign references:
  - **mainPanel**: Drag MainPanel
  - **settingsPanel**: Drag SettingsPanel  
  - **creditsPanel**: Drag CreditsPanel
  - **upgradesPanel**: Drag UpgradesPanel

### 5. Scene Setup
- Save your scene as **"MainMenu"** (File → Save Scene)
- Make sure your **GameScene** exists (or rename in script if using different name)
- In Build Settings, add both MainMenu and GameScene

## Usage
- Play button loads **"GameScene"**
- Settings/Credits panels can be navigated with Back buttons
- Quit exits the application