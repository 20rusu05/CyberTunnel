# CyberTunnel - Cybersecurity Escape Tunnel

A first-person puzzle game built in Unity (URP) where players navigate through 5 rooms in an underground cyber tunnel, each containing a cybersecurity challenge. Solve all 5 to escape.

## Game Concept

**Genre:** First-Person Puzzle / Educational  
**Engine:** Unity 2022+ (Universal Render Pipeline)  
**Players:** Single player  
**Theme:** Cybersecurity / Hacking / Digital forensics

### Story

You are a cybersecurity trainee trapped in a simulated underground facility. To prove your skills and earn your certification, you must navigate through 5 secured rooms. Each room contains a cybersecurity challenge that tests a different skill — from classical cryptography to modern network security.

### Win Condition
Solve all 5 puzzles and reach the exit. Your completion time determines your rank.

### Rank System
| Time | Rank |
|------|------|
| < 3 min | ELITE HACKER |
| < 5 min | SKILLED ANALYST |
| < 10 min | JUNIOR OPERATIVE |
| > 10 min | TRAINEE |

---

## The 5 Rooms

### Room 1 — Caesar Cipher
- **Concept:** A message encrypted with Caesar cipher appears on the terminal
- **Challenge:** Player decrypts the message using the given shift value
- **Example:** `RSHQ SRUW 443` with shift 3 → `OPEN PORT 443`
- **Cybersecurity relevance:** Classical cryptography, encryption basics

### Room 2 — Vigenere Cipher
- **Concept:** A message encrypted with Vigenere cipher using a keyword
- **Challenge:** Player uses the key to decrypt the message
- **Example:** `HKVIYCNN` with key `CYBER` → `FIREWALL`
- **Cybersecurity relevance:** Polyalphabetic cipher, key-based encryption

### Room 3 — Binary Decode
- **Concept:** A binary-encoded cybersecurity command appears on screen
- **Challenge:** Player converts binary → ASCII text to reveal the command
- **Example:** `01000001 01001100 01001100 01001111 01010111 ...` → `ALLOW PORT 22`
- **Cybersecurity relevance:** Data encoding, binary representation, ASCII

### Room 4 — Hash Matching (Password Cracking)
- **Concept:** An MD5 hash is shown with multiple password options
- **Challenge:** Player identifies which password matches the hash
- **Example:** Hash `5f4dcc3b5aa765d61d8327deb882cf99` → `password`
- **Cybersecurity relevance:** Password hashing, dictionary attacks, hash lookup

### Room 5 — Cybersecurity Definitions
- **Concept:** Fill-in-the-blank definitions of cybersecurity terms
- **Challenge:** Player fills in terms like Firewall, Encryption, Phishing, Malware, Authentication
- **Example:** `"A ______ is a network security system that monitors and controls incoming and outgoing network traffic..."` → `Firewall`
- **Cybersecurity relevance:** Foundational cybersecurity knowledge

---

## Project Structure

```
Assets/
├── Scenes/
│   ├── MainMenu          # Main menu scene
│   └── GameScene          # The tunnel with all 5 rooms
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs       # Game state, room progression, timer
│   │   ├── SceneLoader.cs       # Scene transitions with fade
│   │   └── AudioManager.cs      # Sound effects and music
│   ├── Player/
│   │   ├── PlayerController.cs  # FPS movement (WASD + mouse)
│   │   └── PlayerInteraction.cs # Raycast interaction system (E key)
│   ├── Puzzles/
│   │   ├── PuzzleBase.cs        # Abstract base for all puzzles
│   │   ├── CaesarCipherPuzzle.cs
│   │   ├── VigenereCipherPuzzle.cs
│   │   ├── BinaryDecodePuzzle.cs
│   │   ├── HashMatchingPuzzle.cs
│   │   └── DefinitionsPuzzle.cs
│   ├── Room/
│   │   ├── Room.cs              # Room state and puzzle linking
│   │   ├── Door.cs              # Animated doors (locked/unlocked)
│   │   └── PuzzleTerminal.cs    # Interactable terminal object
│   └── UI/
│       ├── PuzzleUIManager.cs   # Puzzle display and input
│       ├── HUDManager.cs        # Timer, room progress, crosshair
│       ├── MainMenuUI.cs        # Start, settings, credits
│       └── PauseMenuUI.cs       # Pause with ESC
├── Prefabs/               # Reusable objects (doors, terminals, etc.)
├── Materials/             # Cyber-themed materials
├── Models/                # 3D models
├── Audio/                 # Sound effects and music
├── Animations/            # Door animations, effects
└── UI/                    # UI sprites, fonts
```

---

## Team Task Distribution (4 Members)

### Member 1 — Player & Core Systems
**Branch:** `feature/player-core`
- [ ] Player controller (FPS movement + camera)
- [ ] Interaction system (raycast + E key)
- [ ] GameManager (state, progression, timer)
- [ ] Scene transitions
- [ ] Pause menu

### Member 2 — Puzzle Logic (Cipher Rooms)
**Branch:** `feature/puzzle-logic`
- [ ] Caesar Cipher puzzle implementation
- [ ] Vigenere Cipher puzzle implementation
- [ ] Binary Decode puzzle implementation
- [ ] Hash Matching puzzle implementation
- [ ] Definitions puzzle implementation
- [ ] Puzzle testing and balancing

### Member 3 — Level Design & Environment
**Branch:** `feature/level-design`
- [ ] Tunnel corridor modeling/building
- [ ] 5 room layouts (walls, floor, ceiling)
- [ ] Door placement and setup
- [ ] Terminal placement
- [ ] Lighting (cyber/neon aesthetic)
- [ ] Materials and textures
- [ ] Particle effects (sparks, glow)

### Member 4 — UI & Audio
**Branch:** `feature/ui-audio`
- [ ] Main menu (Start, Settings, Credits, Quit)
- [ ] HUD (timer, room progress, crosshair)
- [ ] Puzzle UI panels (text input, multiple choice, definitions)
- [ ] Completion screen
- [ ] Sound effects (puzzle solved, door open, typing, etc.)
- [ ] Background music
- [ ] Visual polish and animations

---

## Git Workflow

### Branch Strategy
```
main                  ← stable builds only
├── feature/player-core
├── feature/puzzle-logic
├── feature/level-design
└── feature/ui-audio
```

### Rules
1. **Never push directly to main** — always use Pull Requests
2. **Pull before you start working** — `git pull origin main`
3. **Commit often with clear messages** — e.g., `added caesar cipher decryption logic`
4. **Don't modify the same scene simultaneously** — coordinate who edits GameScene
5. **Use prefabs** — changes to prefabs merge better than scene changes
6. **Test before merging** — make sure the game runs before merging to main

---

## Controls

| Key | Action |
|-----|--------|
| WASD | Move |
| Mouse | Look around |
| E | Interact with terminal |
| Shift | Sprint |
| ESC | Pause menu |

---

## Setup Instructions

### First Time (Creating the project)
1. Open Unity Hub → New Project → Universal 3D (URP)
2. In Unity: Edit → Project Settings → Editor
   - Version Control: `Visible Meta Files`
   - Asset Serialization: `Force Text`
3. Commit and push to GitHub

### Cloning (For teammates)
1. `git clone <repo-url>`
2. Open the folder in Unity Hub → Add project from disk
3. Unity will regenerate `Library/` automatically
4. Wait for import to finish, then you're ready

---

## Weekly Plan

### Week 1 — Foundation
- Create project structure ✅
- Player movement working
- One room with one puzzle (Caesar) working end-to-end
- Basic UI showing

### Week 2 — All Puzzles
- All 5 puzzle scripts functional
- Basic tunnel layout (5 rooms connected)
- Doors working
- HUD with timer and progress

### Week 3 — Polish
- Cyber aesthetic (neon lights, dark environment)
- Sound effects and music
- Main menu complete
- Completion screen with ranks

### Week 4 — Testing & Final
- Bug fixes
- Play testing
- Performance optimization
- Final presentation prep

---

## Tech Notes

- **Unity Version:** 2022.3 LTS or newer
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Language:** C#
- **Text rendering:** TextMeshPro (included with Unity)
- **Physics:** CharacterController for player movement
- **Interaction:** Physics.Raycast for terminal detection
