# Mechanics Design Document: Cosmic Onslaught

**MDA Framework Analysis**

This document applies the **Mechanics-Dynamics-Aesthetics (MDA)** framework to *Cosmic Onslaught*, a 2D bullet hell roguelite with temporal echo mechanics. The MDA framework (Hunicke, LeBlanc, Zubek, 2004) approaches game design through three interconnected lenses:
- **Mechanics**: The rules and procedures of the game at the level of data and algorithms
- **Dynamics**: The runtime behavior of the mechanics acting on player inputs and each other's outputs
- **Aesthetics**: The desirable emotional responses evoked in the player

---

## 1. Mechanics (Rules & Systems)

### 1.1 Core Movement Mechanics

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **2D Movement** | Player ship moves freely in 2D space using WASD or Arrow Keys | Speed: 300 units/sec (base) |
| **Movement Constraints** | Ship constrained to game boundaries with soft wall bounce | Bounce damping: 0.8 |
| **Acceleration** | Optional: Hold Shift to boost speed by 50% | Boost multiplier: 1.5x |
| **Friction** | Ship decelerates when no input (toggleable) | Friction coefficient: 0.95 |

### 1.2 Combat Mechanics

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **Firing** | Player fires primary weapon; manual (Space) or auto-fire (toggle) | Fire rate: 5 rounds/sec (base) |
| **Projectile Properties** | Player bullets: straight-line, instant hit detection | Speed: 600 units/sec, Damage: 1 (base) |
| **Hit Detection** | Pixel-perfect collision between player, bullets, and enemies | Hitbox: 16x16 (player), varies (enemies) |
| **Damage System** | Enemies take damage; health determined by polygon sides; player base health: 50 | Triangle: 3 HP, Square: 4 HP, Pentagon: 5 HP, Hexagon: 6 HP, etc. |
| **Invincibility Frames** | Player immune to damage for 1 second after hit (toggleable) | Duration: 1.0 sec |
| **Screen Shake** | Camera shakes on large explosions or player hits (toggleable) | Intensity: low/medium/high |

### 1.3 Temporal Echo Mechanics (Novel)

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **Recording** | Hold Shift to record player movement for up to N seconds | Max recording: 3.0 sec (upgradable) |
| **Storage** | One echo recording stored at a time; new recording overwrites | Storage: 1 slot |
| **Deployment** | Press E to spawn echo that replays recorded path | Cooldown: 15.0 sec (base) |
| **Echo Movement** | Echo replicates recorded path exactly | Accuracy: 100% |
| **Echo Firing** | Echo fires in sync with player during deployment | Damage multiplier: 1.0x (upgradable) |
| **Echo Collection** | Echo automatically collects drops it touches | Range: 32 units |
| **Echo Decoy** | Acts as decoy, drawing enemy bullet fire; with Echo Shield upgrade: absorbs hits | Absorption: 1 hit (upgradable) |
| **Echo Duration** | Echo persists for recorded length or until hit (if shielded) | Max: matches recording |
| **Echo Visual** | Semi-transparent cyan ghost with trailing light effect | Opacity: 60% |

### 1.4 Enemy Mechanics

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **Spawning** | Enemies spawn in waves at screen edges | Spawn rate: increases per wave |
| **Asteroid Types** | Polygons with 3-12 sides; more sides = more health & complex patterns | Types: 10 (triangle to dodecagon) |
| **Bullet Patterns** | Each enemy type fires unique patterns | Patterns: 20+ |
| **Pattern Types** | Spread, straight, spiral, homing, walls, circles, combinations | Complexity: scales with wave |
| **Boss Waves** | Every 10 waves: elite asteroid with combined patterns | Health: 50 HP, Multiple simultaneous patterns |
| **Drop System** | Destroyed enemies drop money (common) and upgrades (rare) | Money drop: 80%, Upgrade drop: 15% |

**Pattern Library:**
- **Triangle**: 3-way spread (120° angles)
- **Square**: 5-bullet straight barrage (horizontal)
- **Pentagon**: Outward spiral (rotating 360°)
- **Hexagon**: Inward spiral (contracting)
- **Heptagon**: Homing bullets (track player)
- **Octagon**: Wall pattern (vertical/horizontal lines)
- **Nonagon**: Circle pattern (360° ring)
- **Decagon**: Random angle spread
- **Hendecagon**: Wave pattern (sinusoidal)
- **Dodecagon**: Combined patterns (2-3 simultaneous)

### 1.5 Wave System Mechanics

| Mechanic | Description | Parameters |
|----------|-------------|------------|
| **Wave Progression** | Endless waves with increasing difficulty | Wave counter: infinite |
| **Wave Timer** | Each wave lasts until all enemies defeated or player dies | No time limit |
| **Inter-Wave Rest** | 10-second pause between waves | Duration: 10 sec |
| **Difficulty Scaling** | Enemy health, bullet speed, pattern complexity increase per wave | Health +5% every 5 waves, Speed +3% every 5 waves |
| **New Enemy Introduction** | New asteroid type every 5 waves | Introduction rate: 1 type/5 waves |
| **Boss Trigger** | Elite asteroid spawns every 10 waves | Frequency: every 10 waves |

### 1.6 Progression Mechanics

#### Temporary Upgrades (Run-Based)
| Mechanic | Effect | Duration | Drop Rate |
|----------|--------|----------|-----------|
| **Rapid Fire** | +50% fire rate | 10 sec | 15% |
| **Health Boost** | +25% max health | Until death | 15% |
| **Damage Amplifier** | +100% damage | 15 sec | 15% |
| **Magnet** | Auto-collect nearby drops | 8 sec | 15% |
| **Shield** | Absorbs one hit | Until hit | 15% |

#### Permanent Upgrades (Shop-Based)
| Mechanic | Effect | Cost | Max Level |
|----------|--------|------|-----------|
| **+Max Health** | +10 to max health | 100 money | 5 (max 100 total) |
| **+Fire Rate** | +1 round/sec | 150 money | 5 (+5 total) |
| **+Base Damage** | +1 damage/shot | 200 money | 20 (+20 total) |
| **New Weapon Types** | Unlock special weapons | 500 money | 3 types |
| **Starting Shield** | Begin with shield | 300 money | 1 |
| **Echo Duration** | +0.5 sec recording | 400 money | 6 (+3 sec total) |
| **Echo Damage** | +25% echo damage | 450 money | 4 (+100% total) |
| **Echo Shield** | Echo absorbs hits | 500 money | 1 |
| **Echo Cooldown** | -2 sec cooldown | 350 money | 5 (5 sec min) |

#### Scoring System
| Mechanic | Points | Multiplier |
|----------|--------|------------|
| Asteroid destruction | 10 × sides | ×1 |
| No-damage wave | +50% bonus | ×1.5 |
| Chain kill (within 2 sec) | +10 per consecutive kill | ×(1 + chain/10) |
| Boss defeat | 500 base | ×wave/10 |
| Perfect run (no hits) | 1000 bonus | ×1 |

### 1.7 Difficulty & Accessibility Mechanics

| Mechanic | Description | Options |
|----------|-------------|---------|
| **Difficulty Modes** | Preset configurations | Casual, Normal, Hard, Custom |
| **Bullet Speed** | Adjust enemy bullet velocity | 50%-200% |
| **Player Speed** | Adjust player movement speed | 50%-200% |
| **Damage Multiplier** | Adjust damage taken | 50%-200% |
| **Auto-Fire Toggle** | Enable/disable automatic firing | On/Off |
| **Invincibility Frames** | Enable/disable i-frames | On/Off |
| **Screen Shake** | Enable/disable camera shake | On/Off |
| **Colorblind Mode** | Alternative visual indicators | On/Off |
| **Slow Motion** | Hold key to slow game | 50% speed |
| **Hitbox Visualization** | Show collision boxes | On/Off |

---

## 2. Dynamics (Runtime Behavior)

### 2.1 Emergent Gameplay Patterns

| Dynamic | Description | Resulting Experience |
|---------|-------------|---------------------|
| **Pattern Recognition** | Players learn enemy bullet patterns through repetition | Mastery, Prediction |
| **Echo Chaining** | Players record movement through safe path, deploy echo to repeat | Strategic depth, Risk/Reward |
| **Upgrade Synergy** | Combining temporary upgrades (Rapid Fire + Damage Amp) creates exponential power spikes | Optimization, Experimentation |
| **Boss Rush** | Surviving to boss waves requires different strategies than regular waves | Adaptation, Tension |
| **Resource Management** | Players must choose between collecting drops now or positioning for safety | Decision-making, Trade-offs |
| **Echo Baiting** | Deploying echo to draw fire, then moving to safe position | Tactical play, Outsmarting |
| **Wave Pacing** | 10-second rest periods allow mental recovery and strategy planning | Flow state, Relief |

### 2.2 System Interactions

```
Movement System
  ├─> Positioning System
  │     ├─> Bullet Dodging
  │     │     ├─> Survival
  │     │     └─> Pattern Learning
  │     └─> Enemy Targeting
  │           └─> Damage Output
  ├─> Echo Recording
  │     └─> Echo Deployment
  │           ├─> Double Damage
  │           ├─> Decoy Effect
  │           └─> Drop Collection
  └─> Boundary Interaction
        └─> Bounce Mechanics

Combat System
  ├─> Damage Calculation
  │     ├─> Enemy Health Reduction
  │     │     └─> Wave Progression
  │     └─> Player Health Reduction
  │           └─> Death Condition
  ├─> Hit Detection
  │     └─> Collision Resolution
  └─> Projectile Spawning
        └─> Bullet Pattern Generation

Progression System
  ├─> Temporary Upgrades
  │     └─> Run-Based Power Spikes
  └─> Permanent Upgrades
        └─> Long-Term Character Growth

Wave System
  ├─> Difficulty Scaling
  │     └─> Challenge Curve
  └─> Enemy Spawning
        └─> Pattern Variety
```

### 2.3 Risk/Reward Systems

| Risk | Reward | Dynamic Created |
|------|--------|-----------------|
| Stay in dangerous area | Collect more drops | Resource vs. Safety |
| Deploy echo early | Double damage now | Timing Optimization |
| Deploy echo late | Save for boss | Strategic Conservation |
| Rush boss | Faster wave clear | Speed vs. Safety |
| Ignore drops | Better positioning | Immediate vs. Long-term |
| Use slow motion | Learn patterns | Knowledge vs. Time |

### 2.4 Player Strategies

**Offensive Strategies:**
- **Focus Fire**: Concentrate on one enemy at a time
- **Pattern Exploitation**: Attack when enemy is between firing phases
- **Echo Doubling**: Deploy echo during high-damage windows
- **Chain Building**: Maximize consecutive kills for combo bonuses

**Defensive Strategies:**
- **Weaving**: Move in figure-8 patterns to avoid bullets
- **Echo Decoy**: Deploy echo to absorb incoming fire
- **Safe Zones**: Identify and occupy areas with no bullet coverage
- **I-Frame Abuse**: Time hits to maximize invincibility windows

**Utility Strategies:**
- **Echo Collection**: Send echo through dangerous areas for drops
- **Scouting**: Use echo to test unknown patterns
- **Resource Triaging**: Prioritize which drops to collect
- **Position Reset**: Use echo to return to safe position

### 2.5 Difficulty Curve Dynamics

```
Wave 1-4:    Tutorial Phase
   │         └─> Simple patterns, low density
   │         └─> Introduce basic mechanics
   │
Wave 5-9:    Learning Phase  
   │         └─> New enemy type every wave
   │         └─> Pattern complexity increases
   │
Wave 10:     First Boss
   │         └─> Elite asteroid with combined patterns
   │         └─> Major difficulty spike
   │
Wave 11-14:  Mastery Phase
   │         └─> Faster bullets, tighter patterns
   │         └─> Echo becomes essential
   │
Wave 15-19:  Challenge Phase
   │         └─> Multiple pattern types simultaneously
   │         └─> Upgrades required to progress
   │
Wave 20+:     Expert Phase
   │         └─> Perfect dodging required
   │         └─> Echo chaining mandatory
   │         └─> High scores only possible with mastery
```

---

## 3. Aesthetics (Player Experience)

### 3.1 MDA Aesthetic Categories

The MDA framework identifies 8 categories of aesthetic experiences. Below is how *Cosmic Onslaught* delivers each:

| Aesthetic | Definition | Implementation in Cosmic Onslaught | Measurement |
|-----------|------------|--------------------------------------|-------------|
| **Sensation** | Game as sense-pleasure | Neon visuals, synthwave audio, screen shake, particle effects | Player reports of visual/audio satisfaction |
| **Fantasy** | Game as make-believe | Sci-fi spaceship, cosmic setting, time manipulation | Player immersion in role |
| **Narrative** | Game as drama | Wave progression, boss encounters, achievement storytelling | Player engagement with "story" of survival |
| **Challenge** | Game as obstacle course | Bullet patterns, wave scaling, boss fights | Wave reached, survival time |
| **Fellowship** | Game as social framework | Leaderboards, high score sharing | Community engagement, score competition |
| **Discovery** | Game as uncharted territory | New patterns, upgrade combinations, echo strategies | Number of mechanics discovered, strategies tried |
| **Expression** | Game as self-discovery | Echo deployment timing, movement style, upgrade choices | Player style diversity |
| **Submission** | Game as mindless pastime | Auto-fire mode, casual difficulty | Relaxation reports, session length |

### 3.2 Emotional Arcs

**Per-Wave Arc:**
```
Start → Anticipation (wave announcement)
       → Tension (pattern engagement)
       → Relief (enemy defeated)
       → Satisfaction (drops collected)
       → Rest (inter-wave break)
```

**Per-Run Arc:**
```
Begin → Excitement (new run)
      → Confidence (early waves)
      → Challenge (mid waves)
      → Stress (difficult patterns)
      → Triumph (boss defeated) OR Frustration (death)
```

**Long-Term Arc:**
```
Novice → Curiosity (learning mechanics)
       → Competence (mastering basics)
       → Mastery (optimizing strategies)
       → Expertise (creating new techniques)
```

### 3.3 Player Type Satisfaction

| Player Type | Aesthetics Delivered | Mechanics Supporting |
|-------------|----------------------|---------------------|
| **Achiever** | Challenge, Narrative | Wave counter, scoring, achievements, boss fights |
| **Explorer** | Discovery, Fantasy | Pattern variety, echo experimentation, upgrade combinations |
| **Socializer** | Fellowship | Leaderboards, score sharing |
| **Killer** | Challenge, Expression | Echo chaining, pattern mastery, high difficulty |

---

## 4. MDA Mapping

This section explicitly maps **Mechanics → Dynamics → Aesthetics** for key game systems.

### 4.1 Temporal Echo System

```
MECHANICS:
  - Hold Shift to record movement (max 3 sec)
  - Press E to deploy echo
  - Echo mimics recorded path
  - Echo fires in sync with player
  - Echo collects drops
  - Echo can absorb hits (if upgraded)
  - 15-second cooldown

DYNAMICS:
  ↓
  - Players learn optimal recording timing
  - Echo deployment creates offensive windows
  - Echo acts as decoy, reducing damage taken
  - Players experiment with echo positioning
  - Resource management (when to use echo)
  - Skill expression through creative echo uses

AESTHETICS:
  ↓
  - Sensation: Visual ghost effect, trailing light
  - Challenge: Timing, positioning, risk/reward
  - Discovery: Finding new echo uses
  - Expression: Personal playstyle with echo
  - Fantasy: Time manipulation feel
```

### 4.2 Bullet Pattern System

```
MECHANICS:
  - Each enemy type has unique bullet patterns
  - Patterns scale in complexity with wave number
  - 20+ distinct pattern types
  - Boss waves combine multiple patterns

DYNAMICS:
  ↓
  - Players recognize and memorize patterns
  - Pattern mastery enables prediction
  - Wave progression creates learning curve
  - Players develop muscle memory for dodging
  - Boss fights require pattern combination mastery

AESTHETICS:
  ↓
  - Challenge: Overcoming difficult patterns
  - Discovery: Learning new patterns
  - Sensation: Visual spectacle of bullet hell
  - Expression: Different dodging styles
```

### 4.3 Progression System

```
MECHANICS:
  - Temporary upgrades drop from enemies (15% rate)
  - Permanent upgrades purchased from shop
  - Money dropped by all enemies
  - Wave-based difficulty scaling
  - Scoring based on performance

DYNAMICS:
  ↓
  - Short-term goals (survive current wave)
  - Medium-term goals (unlock permanent upgrades)
  - Long-term goals (high scores, achievements)
  - Power spikes from temporary upgrades
  - Risk/reward in drop collection
  - Investment in permanent progression

AESTHETICS:
  ↓
  - Challenge: Overcoming increasingly difficult waves
  - Achievement: Unlocking new upgrades
  - Narrative: Story of character growth
  - Fellowship: Competing on leaderboards
```

### 4.4 Accessibility System

```
MECHANICS:
  - Difficulty modes (Casual/Normal/Hard/Custom)
  - Toggleable auto-fire
  - Toggleable invincibility frames
  - Toggleable screen shake
  - Colorblind mode
  - Slow motion mode
  - Hitbox visualization

DYNAMICS:
  ↓
  - Players tailor experience to preference
  - Reduced frustration for casual players
  - Maintained challenge for hardcore players
  - Inclusive design for various abilities
  - Customization enables optimal flow state

AESTHETICS:
  ↓
  - Submission: Relaxed gameplay options
  - Challenge: Maintained for those who want it
  - Sensation: Customized visual/audio experience
  - Discovery: Finding optimal personal settings
```

---

## 5. Design Principles Applied

### 5.1 Core Loop Validation (30-Second Test)

**ACTION → FEEDBACK → REWARD → REPEAT**

- **ACTION**: Player moves (WASD), fires (Space), records echo (Shift), deploys echo (E)
- **FEEDBACK**: Visual (bullets, explosions, echo), Audio (firing, hits, echo sounds), Haptic (screen shake)
- **REWARD**: Drops collected, enemies destroyed, score increased, upgrades obtained, echo deployed successfully
- **REPEAT**: Next enemy, next wave, next run

### 5.2 Player Psychology Alignment

| Psychology Principle | Implementation |
|---------------------|----------------|
| **Achiever Motivation** | Wave counter, scoring, achievements, boss fights |
| **Explorer Motivation** | Pattern discovery, echo experimentation, upgrade combinations |
| **Socializer Motivation** | Leaderboards, score sharing |
| **Killer Motivation** | Competition, high scores, mastery |
| **Fixed Rewards** | Shop purchases, wave bonuses |
| **Variable Rewards** | Random drop locations, rare upgrade drops |
| **Ratio Rewards** | Money drops scale with enemy health |

### 5.3 Difficulty Balancing

| Strategy | Implementation |
|----------|----------------|
| **Dynamic** | Difficulty scales with wave number; players can choose difficulty mode |
| **Selection** | Multiple difficulty modes, customizable options |
| **Accessibility** | Toggleable features, colorblind mode, slow motion |

**Flow State Maintenance:**
- Early waves: Easy patterns, generous drops → **Hook**
- Mid waves: Challenging but fair patterns → **Flow**
- Late waves: Very difficult, require mastery → **Mastery**
- Rest periods: 10-second breaks → **Prevent fatigue**
- Customizable: Players adjust to personal skill → **Optimal challenge**

### 5.4 Progression Design

| Progression Type | Implementation |
|-----------------|----------------|
| **Skill** | Player learns patterns, improves dodging, masters echo |
| **Power** | Permanent upgrades increase stats; temporary upgrades provide power spikes |
| **Content** | New enemy types, patterns, boss waves |
| **Story** | Implicit narrative of survival and growth |

**Pacing Principles:**
- ✅ Early wins: First drops within 30 seconds
- ✅ Gradually increase challenge: Pattern complexity scales
- ✅ Rest beats: 10-second inter-wave breaks
- ✅ Meaningful choices: Upgrade selection, echo timing, positioning

### 5.5 Anti-Patterns Avoided

| ❌ Don't | ✅ We Do |
|----------|----------|
| Design in isolation | Playtest-focused design with MDA analysis |
| Polish before fun | Core loop validated first (30-second test) |
| Force one way to play | Echo system enables multiple strategies |
| Punish excessively | Reward-focused: drops, upgrades, achievements |

---

## 6. Implementation Priority

### Phase 1: Core Loop (MVP)
1. Player movement (2D)
2. Basic shooting
3. Simple asteroid enemy (triangle, basic spread pattern)
4. Hit detection and damage
5. Wave system (basic)
6. 30-second test validation

### Phase 2: Core Mechanics
1. Temporal Echo system (record/deploy)
2. Additional enemy types (square, pentagon)
3. Bullet pattern variety
4. Temporary upgrades
5. Permanent upgrade shop
6. Scoring system

### Phase 3: Polish & Depth
1. All 10 enemy types with unique patterns
2. Boss waves
3. Echo upgrades
4. Difficulty modes
5. Accessibility options
6. Audio implementation
7. Visual polish

### Phase 4: Content Expansion
1. Additional bullet patterns
2. More temporary upgrade types
3. Additional permanent upgrades
4. Achievement system
5. Leaderboards
6. Balance tuning

---

## Summary

This MDA framework analysis provides a **comprehensive, player-centric** view of *Cosmic Onslaught*'s mechanics. By explicitly connecting Mechanics → Dynamics → Aesthetics, we ensure every system contributes to the desired player experience. The temporal echo mechanic, in particular, serves as a **novel differentiator** that creates unique dynamics (strategic echo deployment) and aesthetics (time manipulation fantasy, expression through creative uses).

The design prioritizes **flow state maintenance** through careful difficulty scaling, **player expression** through multiple viable strategies, and **accessibility** through customizable options—all while staying true to the bullet hell genre's core appeal of **mastery through pattern recognition**.