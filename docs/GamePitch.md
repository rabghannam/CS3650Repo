# CS365 Proposal: Cosmic Onslaught - Bullet Hell Roguelite

**By:** Eneko Diez, David Sanchez and Rabi Ghannam

---

## Pitch

*A 2D bullet hell roguelite where players navigate increasingly complex asteroid bullet patterns in a neon-drenched cosmos, collecting temporary power-ups during runs, earning permanent upgrades, and deploying temporal echoes to outmaneuver impossible odds.*

---

## Core Loop (30-Second Test)

1. **ACTION** → Player moves in 2D space (WASD/Arrow Keys) and fires manually (Space) or automatically (toggleable)
2. **FEEDBACK** → Asteroids fire intricate bullet patterns; player dodges while returning fire; damage causes screen shake and red flash
3. **REWARD** → Destroyed asteroids drop money (common) and temporary upgrades (rare); wave completion grants bonus cash
4. **REPEAT** → Each wave introduces new bullet patterns and enemy types, encouraging mastery and replay

---

## Core Gameplay

### Movement & Controls
- **Full 2D Movement**: WASD or Arrow Keys for complete freedom
- **Shooting Options**:
  - Manual firing (Space) for precision control
  - Auto-fire toggle for accessibility
- **Temporal Echo System** (Novel Mechanic):
  - **Record**: Hold Shift to record up to 3 seconds of movement
  - **Deploy**: Press E to spawn an echo that replays recorded path
  - **Echo Properties**:
    - Mimics player's exact movement for its duration
    - Fires in sync with player (double firepower)
    - Can collect drops automatically
    - Acts as decoy, drawing enemy bullet fire
    - Duration: Matches recorded time (max 3 seconds)
    - Cooldown: 15 seconds between deployments
  - **Upgrades Available**:
    - Echo Duration: Extend recording/deployment time
    - Echo Damage: Echo fires deal bonus damage
    - Echo Shield: Echo absorbs one hit before disappearing
- **Player Stats Displayed**:
  - Current/Max Health
  - Fire Rate (rounds per second)
  - Damage per shot
  - Wave number
  - Score
  - Echo Cooldown Timer

### Combat
- **Player**: Spaceship with base weapon, customizable through upgrades
- **Enemies**: Asteroids (polygons) that fire bullet patterns
  - **Health**: Determined by number of sides (triangle = least, dodecagon = most)
  - **Behavior**: Each asteroid type fires unique bullet patterns:
    - **Triangle**: Basic spread shot (3 bullets)
    - **Square**: Straight-line barrage
    - **Pentagon**: Spiral pattern
    - **Hexagon+**: Complex combinations (homing, walls, circles)
- **Bullet Patterns**: Over 20 unique patterns that scale in complexity with wave number

### Novel Mechanic: Temporal Echo System
A **non-typical bullet hell feature** that adds strategic depth and player expression. Unlike standard bullet hell mechanics (pure dodging, power-ups), the echo system allows players to **manipulate time itself** by recording and replaying their own movements.

- **Strategic Uses**:
  - **Offensive**: Deploy echo during boss fights for double damage output
  - **Defensive**: Use echo as decoy to absorb bullet fire intended for player
  - **Utility**: Send echo through dangerous areas to collect drops safely
  - **Scouting**: Record movement through unexplored pattern, deploy to test safety
- **Risk/Reward**: 15-second cooldown forces careful timing; poor placement wastes the ability
- **Skill Expression**: Master players chain echoes with movement to create complex dodging patterns

### Damage & Defense
- **Player**: Takes damage from enemy bullets and asteroid collisions
- **Invincibility Frames**: 1-second immunity after hit (toggleable in accessibility)
- **Visual Feedback**:
  - Player flashes red when hit
  - Screen shake on large explosions
  - Asteroids turn red when taking damage

---

## Upgrade System

### Temporary Upgrades (Dropped by Asteroids)
- **Duration**: Last until end of current run
- **Drop Rate**: 15% chance from any asteroid
- **Types**:
  - **Rapid Fire**: +50% fire rate for 10 seconds
  - **Health Boost**: +25% max health for current run
  - **Damage Amplifier**: +100% damage for 15 seconds
  - **Magnet**: Auto-collect nearby drops for 8 seconds
  - **Shield**: Absorbs one hit (visual bubble effect)

### Permanent Upgrades (Shop - Main Menu)
- **Currency**: Money dropped by destroyed asteroids
- **Available Upgrades**:
  - **+Max Health**: +10 to max health (max 100 total, base 50)
  - **+Fire Rate**: +1 round per second (max +5)
  - **+Base Damage**: +1 damage per shot (max +20)
  - **New Weapon Types**: Unlock spread shot, piercing shot, homing missiles
  - **Starting Shield**: Begin each run with one shield
  - **Echo Duration**: +0.5 seconds to recording/deployment (max +3 seconds total)
  - **Echo Damage**: Echo fires deal +25% damage (max +100%)
  - **Echo Shield**: Echo can absorb hits (1 use per deployment)
  - **Echo Cooldown**: -2 seconds from cooldown (min 5 seconds)

---

## Progression

### Run-Based Progression
- **Wave System**: Endless waves with 10-second rest period between each
- **Wave Scaling**:
  - Every 5 waves: New asteroid type introduced
  - Every 10 waves: "Boss Wave" with elite asteroid that fires multiple patterns simultaneously
- **Scoring**: Points awarded for:
  - Asteroid destruction (more sides = more points)
  - Perfect dodging (no damage taken in wave = bonus multiplier)
  - Chain bonuses (rapid consecutive kills)

### Permanent Progression
- **Shop Unlocks**: New permanent upgrades become available at milestones
- **High Scores**: Global and session-based leaderboards
- **Achievements**: Unlockable goals (e.g., "Survive 25 waves," "Destroy 100 hexagons")

---

## Art Style

### Visual Direction
- **Aesthetic**: Neon cyberpunk meets cosmic void
- **Color Palette**: Deep purples, electric blues, vibrant pinks against black space
- **Player Ship**: Sleek triangle design with glowing engine trail
- **Asteroids**: Wireframe polygons with neon edges; color indicates health (green → yellow → red)
- **Bullets**: 
  - Player: Bright cyan projectiles
  - Enemy: Varied colors by pattern type (red for basic, purple for homing, etc.)
- **Effects**:
  - Explosions: Particle bursts with color matching asteroid
  - Damage: Red crack effect on asteroids
  - Upgrades: Floating glowing orbs with icon indication
  - **Temporal Echo**: Semi-transparent cyan ghost ship with trailing light effect; recording phase shows slight time-distortion shimmer; deployment creates a "ripple" animation

### UI Design
- Clean, minimal HUD with neon outlines
- Health bar: Horizontal at bottom with gradient (green → red)
- Stats displayed in top corners with subtle glow

---

## Audio

### Sound Direction
- **Music**: Synthwave soundtrack with increasing intensity per wave tier
- **Sound Effects**:
  - Player firing: Sharp electronic "pew" sound
  - Asteroid destruction: Deep bass explosion with pitch variation by size
  - Bullet impact: Metallic "ting" for player hits on asteroids
  - Player hit: Alarm-like warning tone
  - Upgrade pickup: Ascending chime
  - Wave complete: Triumphant sting
  - **Temporal Echo**:
    - Recording: Subtle time-winding sound effect
    - Deployment: "Whoosh" sound with echoing tail
    - Echo firing: Lower-pitched version of player firing sound
- **Dynamic Audio**: Music tempo increases during boss waves; ambient space hum in rest periods

---

## Difficulty & Accessibility

### Difficulty Modes
- **Casual**: Slower bullet speeds, larger player hitbox, more health
- **Normal**: Balanced experience
- **Hard**: Faster bullets, tighter patterns, less health
- **Custom**: Adjustable sliders for bullet speed, player speed, damage

### Accessibility Options
- **Toggle Auto-Fire**: On/Off
- **Invincibility Frames**: On/Off
- **Screen Shake**: On/Off
- **Colorblind Mode**: Alternative indicators for damage states
- **Slow Motion**: Hold key to slow game to 50% speed (for learning patterns)
- **Hitbox Visualization**: Toggle to show player and bullet hitboxes

---

## Player Psychology & Motivation

| Type | How Game Appeals |
|------|------------------|
| **Achiever** | Wave counters, high scores, achievement system, permanent unlocks |
| **Explorer** | Discovering new bullet patterns, experimenting with echo deployment strategies and upgrade combinations |
| **Socializer** | Compete on leaderboards, share high scores |
| **Killer** | Mastering patterns, optimizing builds for maximum wave survival |

| Reward Schedule | Implementation |
|----------------|----------------|
| **Fixed** | Shop purchases, wave completion bonuses |
| **Variable** | Random temporary upgrade drops, rare high-value asteroids |
| **Ratio** | Money drops scale with asteroid health (more sides = more money) |

---

## Pacing Principles Applied

- **Early Wins**: First waves feature simple patterns; first temporary upgrade drops within 30 seconds; echo mechanic introduced in wave 3 tutorial
- **Gradually Increase Challenge**: New patterns introduced every 5 waves; difficulty scales smoothly; echo cooldown decreases as player unlocks upgrades
- **Rest Beats**: 10-second pause between waves; ambient music during breaks
- **Meaningful Choices**: Players choose between temporary upgrade types, permanent shop investments, and **when to deploy echoes** (offensive vs. defensive vs. collection)
