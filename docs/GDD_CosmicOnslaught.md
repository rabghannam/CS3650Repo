# Game Design Document: Cosmic Onslaught

**Version:** 1.0  
**Date:** 2026-09-24  
**Team:** Eneko Diez, David Sanchez, Rabi Ghannam  
**Course:** CS365 - Game Design

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Game Overview](#2-game-overview)
3. [Core Loop](#3-core-loop)
4. [Gameplay Mechanics](#4-gameplay-mechanics)
5. [Progression Systems](#5-progression-systems)
6. [Art & Audio Design](#6-art--audio-design)
7. [Controls](#7-controls)
8. [Difficulty & Accessibility](#8-difficulty--accessibility)
9. [Player Psychology & Motivation](#9-player-psychology--motivation)
10. [Balancing & Design Principles](#10-balancing--design-principles)
11. [Technical Specifications](#11-technical-specifications)
12. [Development Roadmap](#12-development-roadmap)
13. [Appendix: MDA Framework Analysis](#13-appendix-mda-framework-analysis)

---

## 1. Executive Summary

**Pitch:** *A 2D bullet hell roguelite where players navigate increasingly complex asteroid bullet patterns in a neon-drenched cosmos, collecting temporary power-ups during runs, earning permanent upgrades between runs, and deploying temporal echoes to outmaneuver impossible odds.*

**Genre:** Bullet Hell / Shoot 'em Up / Roguelite  
**Platform:** PC (Windows)  
**Target Audience:** Core gamers aged 16-35 who enjoy skill-based action games with strategic depth  
**Playtime:** 5-30 minutes per run; 10+ hours to master  
**Unique Selling Point:** Temporal Echo mechanic allows players to record and replay their movements, creating strategic depth uncommon in bullet hell games

---

## 2. Game Overview

### 2.1 Concept

*Cosmic Onslaught* is a **bullet hell roguelite** that combines the intense dodging and pattern recognition of classic shoot 'em ups with the progression systems of modern roguelites. The game's defining feature is the **Temporal Echo System**, a novel mechanic that lets players record their movement and deploy a ghost echo that replays those actions, adding layers of strategy beyond traditional bullet hell gameplay.

Players control a spaceship battling endless waves of asteroid enemies, each firing unique bullet patterns. The game features both **temporary upgrades** (collected during runs) and **permanent upgrades** (purchased between runs), creating a compelling loop of short-term excitement and long-term progression.

### 2.2 Setting & Theme

- **Setting:** Deep space, cosmic void
- **Theme:** Neon cyberpunk meets sci-fi minimalism
- **Mood:** Intense, electric, otherworldly
- **Tone:** High-energy with moments of calm between waves

### 2.3 Game Modes

- **Endless Mode**: Primary gameplay - survive as many waves as possible
- **Daily Challenge**: Special seed-based run with unique modifiers (future update)
- **Practice Mode**: Test specific wave patterns (future update)

---

## 3. Core Loop

### 3.1 30-Second Test (Validated)

Every game session delivers a satisfying loop within 30 seconds:

1. **ACTION**: Player moves in 2D space (WASD/Arrow Keys), fires at asteroids (Space/auto), records echo (Hold Shift)
2. **FEEDBACK**: Asteroids fire bullet patterns; player dodges while returning fire; visual feedback includes red flash on hit, screen shake on explosions, echo ghost effect
3. **REWARD**: Destroyed asteroids drop money (80% chance) and temporary upgrades (15% chance); wave completion grants bonus cash
4. **REPEAT**: Each wave introduces new bullet patterns; echo mechanic enables new strategies; endless progression hooks players

### 3.2 Extended Loop

```
START RUN
  ↓
Wave Announcement (2s)
  ↓
Enemy Spawn + Pattern Execution
  ↓
Player: Dodge + Attack + Echo Deployment
  ↓
Enemies Defeated → Drops Collected
  ↓
Inter-Wave Rest (10s)
  ↓
Permanent Upgrades Purchased (if run ended)
  ↓
NEW RUN
```

---

## 4. Gameplay Mechanics

### 4.1 Core Movement System

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **2D Movement** | Free movement in all directions using WASD or Arrow Keys | Base speed: 300 units/sec |
| **Boundary Handling** | Soft bounce at screen edges with damping | Bounce damping: 0.8 |
| **Boost** | Hold Shift to increase speed temporarily | Boost multiplier: 1.5x |
| **Friction** | Optional deceleration when no input | Friction coefficient: 0.95 (toggleable) |

### 4.2 Combat System

#### Player Combat
- **Primary Weapon**: Fires straight-line projectiles
- **Fire Modes**: Manual (Space) or Auto-Fire (toggleable)
- **Projectile Properties**:
  - Speed: 600 units/sec
  - Damage: 1 (base, upgradable)
  - Size: 4x4 pixels
  - Color: Bright cyan

#### Enemy Combat
- **Enemy Types**: 10 polygon-based asteroids (triangle to dodecagon)
- **Health System**: Health scales with number of sides (triangle = 3 HP, square = 4 HP, pentagon = 5 HP, etc.)
- **Bullet Patterns**: 20+ unique patterns per enemy type

**Pattern Library:**
- Triangle: 3-way spread (120° angles)
- Square: 5-bullet straight barrage (horizontal)
- Pentagon: Outward spiral (360° rotation)
- Hexagon: Inward spiral (contracting)
- Heptagon: Homing bullets (player tracking)
- Octagon: Wall patterns (vertical/horizontal lines)
- Nonagon: Circle patterns (360° ring)
- Decagon: Random angle spread
- Hendecagon: Wave patterns (sinusoidal)
- Dodecagon: Combined patterns (2-3 simultaneous)

#### Boss Mechanics
- **Frequency**: Every 10 waves
- **Type**: Elite asteroid with 50 HP
- **Behavior**: Fires 2-3 bullet patterns simultaneously
- **Reward**: 500 base points × (wave/10) multiplier

### 4.3 Temporal Echo System (Novel Mechanic)

The game's **unique differentiator** from traditional bullet hell titles.

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **Recording** | Hold Shift to record player movement | Max duration: 3.0 sec (upgradable to 6.0 sec) |
| **Storage** | One echo recording stored; new overwrites old | Storage slots: 1 |
| **Deployment** | Press E to spawn echo that replays recorded path | Cooldown: 15.0 sec (base, upgradable to 5.0 sec) |
| **Echo Movement** | Replicates recorded path with 100% accuracy | - |
| **Echo Firing** | Fires in sync with player | Damage multiplier: 1.0x (upgradable to 2.0x) |
| **Echo Collection** | Auto-collects drops within 32 units | Range: 32 units |
| **Echo Decoy** | Acts as decoy, drawing enemy bullet fire; with Echo Shield upgrade: absorbs 1 hit | Uses: 1 (upgradable to infinite) |
| **Echo Visual** | Semi-transparent cyan ghost with trailing light | Opacity: 60% |

**Strategic Uses:**
- **Offensive**: Double damage output during boss fights
- **Defensive**: Decoy to absorb incoming bullet fire
- **Utility**: Collect drops from dangerous areas safely
- **Scouting**: Test unknown patterns before committing

### 4.4 Damage & Health Systems

| Entity | Base Health | Damage Taken | Special Properties |
|--------|-------------|--------------|---------------------|
| Player | 50 | 1 per enemy bullet | Invincibility frames: 1.0 sec (toggleable) |
| Triangle | 3 | 1 per player bullet | - |
| Square | 4 | 1 per player bullet | - |
| Pentagon | 5 | 1 per player bullet | - |
| Hexagon | 6 | 1 per player bullet | - |
| ... | ... | ... | - |
| Dodecagon | 12 | 1 per player bullet | - |
| Boss | 50 | 1 per player bullet | Fires multiple patterns |

### 4.5 Drop & Collection System

| Drop Type | Chance | Effect | Duration |
|-----------|--------|--------|----------|
| Money | 80% | +10-50 money (scales with asteroid sides) | Permanent |
| Rapid Fire | 15% | +50% fire rate | 10 seconds |
| Health Boost | 15% | +25% max health | Until death |
| Damage Amplifier | 15% | +100% damage | 15 seconds |
| Magnet | 15% | Auto-collect nearby drops | 8 seconds |
| Shield | 15% | Absorbs one hit | Until hit |

---

## 5. Progression Systems

### 5.1 Temporary Progression (Run-Based)

**Purpose**: Provide immediate satisfaction and power spikes within a single run.

**Types:**
- Rapid Fire: Fire rate increase
- Health Boost: Temporary health pool expansion
- Damage Amplifier: Damage doubling
- Magnet: Automatic drop collection
- Shield: One-hit protection

**Drop Rates**: 15% per asteroid, one drop per asteroid

### 5.2 Permanent Progression (Shop-Based)

**Purpose**: Create long-term investment and character growth between runs.

**Currency**: Money (earned from destroying asteroids and wave bonuses)

**Available Upgrades:**

| Upgrade | Cost | Max Level | Effect |
|---------|------|-----------|--------|
| +Max Health | 100 | 5 | +10 to max health (max 100 total) |
| +Fire Rate | 150 | 5 | +1 round/sec (max +5) |
| +Base Damage | 200 | 20 | +1 damage/shot (max +20) |
| New Weapon Types | 500 | 3 | Unlock: Spread Shot, Piercing Shot, Homing Missiles |
| Starting Shield | 300 | 1 | Begin each run with shield |
| Echo Duration | 400 | 6 | +0.5 sec recording (max +3 sec) |
| Echo Damage | 450 | 4 | +25% echo damage (max +100%) |
| Echo Shield | 500 | 1 | Echo absorbs one hit |
| Echo Cooldown | 350 | 5 | -2 sec cooldown (min 5 sec) |

### 5.3 Scoring System

**Purpose**: Reward skillful play and create competition.

| Action | Points | Multiplier |
|--------|--------|------------|
| Asteroid destruction | 10 × sides | ×1 |
| No-damage wave | +50% bonus | ×1.5 |
| Chain kill (within 2s) | +10 per consecutive | ×(1 + chain/10) |
| Boss defeat | 500 | ×(wave/10) |
| Perfect run (no hits) | 1000 | ×1 |

### 5.4 Achievement System

**Purpose**: Provide long-term goals and reward exploration.

**Examples:**
- Survive 25 waves
- Defeat 100 hexagons
- Collect 5 temporary upgrades in one run
- Deploy 10 echoes in a single run
- Defeat a boss without taking damage
- Complete a perfect run (no hits, all drops collected)

---

## 6. Art & Audio Design

### 6.1 Visual Style

**Aesthetic:** Neon cyberpunk meets cosmic void

**Color Palette:**
- Primary: Deep purple (#2A0845), Electric blue (#00FFFF), Vibrant pink (#FF00FF)
- Secondary: Cyan (#00FFFF), Magenta (#FF00FF), White (#FFFFFF)
- Background: Black (#000000) with subtle starfield

**Art Style Details:**

| Element | Description |
|---------|-------------|
| **Player Ship** | Sleek triangle design with glowing engine trail; color: bright cyan |
| **Asteroids** | Wireframe polygons with neon edges; color indicates health (green → yellow → red) |
| **Player Bullets** | Small cyan projectiles with light trail |
| **Enemy Bullets** | Varied colors by pattern type (red for basic, purple for homing, etc.) |
| **Explosions** | Particle bursts matching asteroid color |
| **Damage Feedback** | Red crack effect on asteroids; screen flash on player hit |
| **Temporal Echo** | Semi-transparent cyan ghost ship with trailing light effect; recording phase shows time-distortion shimmer; deployment creates ripple animation |
| **UI** | Clean, minimal HUD with neon outlines; health bar with gradient (green → red) |

### 6.2 Audio Design

**Music:** Synthwave soundtrack
- **Tempo**: Increases with wave number
- **Intensity**: Scales with wave difficulty tier
- **Boss Waves**: Special high-intensity tracks
- **Rest Periods**: Ambient space hum

**Sound Effects:**

| Action | Sound | Description |
|--------|-------|-------------|
| Player firing | Sharp electronic "pew" | High-pitched, short |
| Asteroid destruction | Deep bass explosion | Pitch varies by asteroid size |
| Bullet impact | Metallic "ting" | Player bullets hitting asteroids |
| Player hit | Alarm-like warning tone | Urgent, attention-grabbing |
| Upgrade pickup | Ascending chime | Pleasant, rewarding |
| Wave complete | Triumphant sting | Short musical flourish |
| Temporal Echo recording | Subtle time-winding | Low volume, continuous |
| Temporal Echo deployment | "Whoosh" with echoing tail | Dynamic, satisfying |
| Echo firing | Lower-pitched "pew" | Matches player firing but deeper |

---

## 7. Controls

### 7.1 Default Controls (PC)

| Action | Key | Alternative |
|--------|-----|-------------|
| Move Up | W | ↑ |
| Move Down | S | ↓ |
| Move Left | A | ← |
| Move Right | D | → |
| Fire | Space | Mouse Left Click |
| Record Echo | Hold Shift | Hold Right Mouse |
| Deploy Echo | E | Mouse Right Click |
| Toggle Auto-Fire | Tab | - |
| Slow Motion | Left Ctrl | - |
| Pause | Escape | - |

### 7.2 Accessibility Controls

| Feature | Toggle | Default |
|---------|--------|---------|
| Auto-Fire | On/Off | Off |
| Invincibility Frames | On/Off | On |
| Screen Shake | On/Off | On |
| Colorblind Mode | On/Off | Off |
| Hitbox Visualization | On/Off | Off |
| Slow Motion Mode | Hold/Always | Hold |

---

## 8. Difficulty & Accessibility

### 8.1 Difficulty Modes

| Mode | Description | Bullet Speed | Player Speed | Damage |
|------|-------------|--------------|---------------|--------|
| **Casual** | For beginners | 75% | 125% | 75% |
| **Normal** | Balanced experience | 100% | 100% | 100% |
| **Hard** | For experts | 125% | 100% | 125% |
| **Custom** | Player-defined | 50%-200% | 50%-200% | 50%-200% |

### 8.2 Difficulty Scaling

**Per-Wave Progression:**
- Every **5 waves**: New asteroid type introduced
- Every **5 waves**: Enemy health +5%
- Every **5 waves**: Bullet speed +3%
- Every **10 waves**: Boss wave with elite asteroid

**Dynamic Difficulty Elements:**
- Pattern complexity increases with wave number
- Bullet density increases with wave number
- Enemy spawn rate increases with wave number

### 8.3 Accessibility Features

**Visual:**
- Colorblind mode (alternative indicators for damage states)
- Hitbox visualization (show collision boxes)
- Screen shake toggle
- Adjustable UI scale

**Audio:**
- Volume sliders (master, music, SFX)
- Subtitles for key events (wave start, boss appearance)

**Gameplay:**
- Auto-fire toggle
- Invincibility frames toggle
- Slow motion mode (hold to reduce speed to 50%)
- Custom difficulty sliders

**Input:**
- Full keyboard rebinding
- Controller support (future)

---

## 9. Player Psychology & Motivation

### 9.1 Motivation Types (Bartle's Taxonomy)

| Type | Driven By | Game Implementation |
|------|-----------|---------------------|
| **Achiever** | Goals, completion | Wave counter, scoring system, achievements, boss fights |
| **Explorer** | Discovery, secrets | Pattern discovery, echo experimentation, upgrade combinations, hidden strategies |
| **Socializer** | Interaction, community | Leaderboards, score sharing, community challenges |
| **Killer** | Competition, dominance | High scores, wave survival, mastery demonstration |

### 9.2 Reward Schedules

| Schedule | Effect | Implementation |
|----------|--------|----------------|
| **Fixed** | Predictable, reliable | Shop purchases, wave completion bonuses |
| **Variable** | Addictive, exciting | Random temporary upgrade drops, rare high-value asteroids |
| **Ratio** | Effort-based, satisfying | Money drops scale with asteroid health (more sides = more money) |

### 9.3 Flow State Design

**Avoiding Frustration:**
- Multiple difficulty modes
- Accessibility options
- Checkpoint-free but low-commitment (runs are short)

**Avoiding Boredom:**
- Endless wave progression
- Increasing pattern complexity
- Novel echo mechanic for strategic depth
- Varied temporary upgrades

**Maintaining Flow:**
- 10-second rest periods between waves
- Gradual difficulty scaling
- Clear feedback for all actions
- Meaningful player choices

---

## 10. Balancing & Design Principles

### 10.1 Core Loop Validation

✅ **ACTION**: Player moves (WASD), fires (Space), records echo (Shift), deploys echo (E)  
✅ **FEEDBACK**: Visual (bullets, explosions, echo), Audio (firing, hits), Haptic (screen shake)  
✅ **REWARD**: Drops collected, enemies destroyed, score increased, echo deployed  
✅ **REPEAT**: Endless waves with increasing variety

### 10.2 Progression Design Principles

| Principle | Implementation |
|-----------|----------------|
| **Early Wins** | First drops within 30 seconds, simple early patterns |
| **Gradually Increase Challenge** | Pattern complexity scales smoothly with waves |
| **Rest Beats** | 10-second pauses between waves with ambient music |
| **Meaningful Choices** | Upgrade selection, echo timing, positioning strategies |

### 10.3 Balancing Strategies

| Strategy | Implementation |
|----------|----------------|
| **Dynamic** | Difficulty scales with wave number; players can select difficulty mode |
| **Selection** | 4 difficulty modes + custom options |
| **Accessibility** | 8+ accessibility features for various player needs |

### 10.4 Anti-Patterns Avoided

| ❌ Don't | ✅ We Do | Implementation |
|----------|----------|----------------|
| Design in isolation | Playtest-focused design | MDA framework analysis, iterative testing |
| Polish before fun | Core loop validated first | 30-second test validation, mechanics-first approach |
| Force one way to play | Allow player expression | Echo system enables multiple strategies, various viable builds |
| Punish excessively | Reward progress | Drop-focused economy, achievement system, positive reinforcement |

---

## 11. Technical Specifications

### 11.1 Platform Requirements

- **Operating System**: Windows 10/11
- **Processor**: 2 GHz dual-core minimum
- **Memory**: 4 GB RAM minimum
- **Graphics**: Integrated graphics sufficient (bullet hell is 2D)
- **Storage**: 500 MB minimum

### 11.2 Development Tools

- **Engine**: Unity (recommended) or Godot
- **Language**: C# (Unity) or GDScript (Godot)
- **Art**: Aseprite (pixel art) or vector graphics
- **Audio**: BFXR or LMMS for sound effects, synthwave music libraries
- **Version Control**: Git

### 11.3 Performance Targets

- **Frame Rate**: 60 FPS target
- **Resolution**: 1920x1080 (scalable)
- **Bullet Limit**: 1000 simultaneous bullets (performance optimization required for late waves)
- **Load Times**: < 2 seconds between runs

---

## 12. Development Roadmap

### Phase 1: Core Loop Prototype (Week 1-2)
**Goal**: Validate 30-second test

- [ ] Player movement (2D)
- [ ] Basic shooting
- [ ] Simple asteroid enemy (triangle)
- [ ] Basic spread bullet pattern
- [ ] Hit detection
- [ ] Damage system
- [ ] Wave system (basic)
- [ ] **Validation**: 30-second test with 5 playtesters

### Phase 2: Core Mechanics (Week 3-4)
**Goal**: Implement all core systems

- [ ] Temporal Echo system (record/deploy)
- [ ] All 10 enemy types
- [ ] 10+ bullet patterns
- [ ] Temporary upgrade system (5 types)
- [ ] Permanent upgrade shop
- [ ] Basic scoring
- [ ] Difficulty modes
- [ ] **Validation**: Playtest with 10 participants

### Phase 3: Polish & Depth (Week 5-6)
**Goal**: Add depth and polish

- [ ] All 20+ bullet patterns
- [ ] Boss wave system
- [ ] Echo upgrades (4 types)
- [ ] Complete scoring system
- [ ] Achievement system
- [ ] Leaderboards
- [ ] Accessibility options
- [ ] Audio implementation
- [ ] Visual polish
- [ ] **Validation**: Closed beta with 20 playtesters

### Phase 4: Content & Balancing (Week 7-8)
**Goal**: Finalize and balance

- [ ] Additional bullet patterns
- [ ] More temporary upgrade types
- [ ] Additional permanent upgrades
- [ ] Balance tuning
- [ ] Bug fixing
- [ ] Performance optimization
- [ ] **Validation**: Open beta, final balancing

### Phase 5: Finalization (Week 9)
**Goal**: Submission-ready product

- [ ] Final bug fixes
- [ ] Documentation completion
- [ ] Trailer/video capture
- [ ] Submission package assembly

---

## 13. Appendix: MDA Framework Analysis

### Overview

The **Mechanics-Dynamics-Aesthetics (MDA)** framework provides a formal approach to understanding *Cosmic Onslaught* as a designed experience. This appendix summarizes the complete analysis from mechanics.md.

### Key MDA Mappings

#### Temporal Echo System
```
MECHANICS: Record (Shift), Deploy (E), Cooldown, Echo Properties
  ↓
DYNAMICS: Strategic echo use, decoy tactics, resource management, skill expression
  ↓
AESTHETICS: Sensation, Challenge, Discovery, Expression, Fantasy
```

#### Bullet Pattern System
```
MECHANICS: 20+ patterns, scaling complexity, enemy types
  ↓
DYNAMICS: Pattern recognition, muscle memory, prediction, mastery
  ↓
AESTHETICS: Challenge, Discovery, Sensation, Expression
```

#### Progression System
```
MECHANICS: Temporary upgrades, permanent shop, scoring, achievements
  ↓
DYNAMICS: Short-term power spikes, long-term investment, goal orientation
  ↓
AESTHETICS: Achievement, Narrative, Challenge, Fellowship
```

### Aesthetic Delivery

| MDA Category | Delivery Method | Success Metric |
|--------------|-----------------|----------------|
| Sensation | Neon visuals, synthwave audio, effects | Player satisfaction surveys |
| Fantasy | Sci-fi setting, time manipulation | Immersion reports |
| Narrative | Wave progression, boss encounters | Engagement with survival story |
| Challenge | Bullet patterns, wave scaling | Wave reached, survival time |
| Fellowship | Leaderboards, score sharing | Community engagement |
| Discovery | Pattern variety, echo strategies | Mechanics discovered, strategies tried |
| Expression | Echo deployment, movement style | Player style diversity |
| Submission | Auto-fire, casual mode | Relaxation reports, session length |

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2026-09-24 | Rabi Ghannam | Initial GamePitch.md creation |
| 0.2 | 2026-09-24 | Rabi Ghannam | Added Temporal Echo mechanic |
| 0.3 | 2026-09-24 | Rabi Ghannam | Created mechanics.md with MDA framework |
| 1.0 | 2026-09-24 | Eneko Diez, David Sanchez, Rabi Ghannam | Full GDD integration |

---

**End of Document**  
*This document is a living design reference. Update regularly as development progresses.*
