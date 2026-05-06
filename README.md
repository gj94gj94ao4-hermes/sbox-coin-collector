# 🪙 Coin Collector — s&box Game

A simple multiplayer coin-collecting game mode built on the [s&box](https://sbox.game) game engine by Facepunch Studios.

## 🎮 Gameplay

- **2-minute rounds** — collect as many coins as you can before time runs out!
- Coins spawn randomly across the map and **spin + float** to attract your attention.
- Walk over a coin to pick it up — your score increases instantly.
- Coins respawn automatically so there's always something to chase.
- When the timer hits 0, the player with the most coins **wins**!
- After a 5-second intermission, a new round starts automatically.

## 🏗️ Architecture

```
CoinCollectorGame   — GameManager: round logic, coin spawning, time tracking
  └─ CoinCollectorPawn — Player entity: movement, coin pickup via overlap
  └─ Coin              — Collectible entity: spinning coin with bob animation
  └─ CoinCollectorHud  — HUD root panel
       ├─ ScorePanel       — Top-left coin count
       ├─ TimerPanel       — Top-center countdown (turns orange/red when low)
       ├─ AnnouncementPanel— Center screen "GO!" / winner announcement
       ├─ ToastPanel       — Floating "+1 coins!" feedback
       └─ CrosshairPanel   — Simple center crosshair
```

### Networking

| Attribute | Purpose |
|-----------|---------|
| `[Net]` | Server→Client replication (score, round time, etc.) |
| `[ClientRpc]` | Server-triggered client effects (toast notifications) |
| `[ConCmd.Server]` | Client→Server commands (`cc_restart`) |

## 📁 Project Structure

```
sbox-coin-collector/
├── addon.json                  — Addon manifest
├── code/
│   ├── CoinCollector.csproj    — C# project file
│   ├── CoinCollectorGame.cs    — Game manager & round logic
│   ├── CoinCollectorPawn.cs    — Player pawn (movement + pickup)
│   ├── Coin.cs                 — Collectible coin entity
│   ├── CoinCollectorHud.cs     — HUD entry point
│   └── UI/
│       ├── ScorePanel.cs/.scss      — Score display
│       ├── TimerPanel.cs/.scss      — Round timer
│       ├── AnnouncementPanel.cs/.scss— Announcements
│       ├── ToastPanel.cs/.scss      — Floating notifications
│       └── CrosshairPanel.cs/.scss  — Crosshair
├── maps/                       — Map files (create in s&box editor)
├── models/                     — Custom models (optional)
└── README.md
```

## 🚀 Getting Started

### Prerequisites

1. **s&box** — available on [Steam](https://store.steampowered.com/app/521580/sbox/)
2. **.NET 8 SDK** — for compiling C# code
3. An IDE like Visual Studio, VS Code, or JetBrains Rider

### Installation

1. Clone this repo into your s&box addons directory:
   ```bash
   # Default s&box addon path:
   git clone https://github.com/gj94gj94ao4-hermes/sbox-coin-collector.git \
       ~/sbox/addons/sbox-coin-collector
   ```
   Or wherever your s&box installation expects addons.

2. Open s&box Editor and the addon should appear in your addon list.

3. Click **Play** to start a local server with this game mode.

### Running in s&box

1. Launch s&box Editor
2. Create or open a map (any flat map works for testing)
3. Set the game mode to **Coin Collector** in the scene settings
4. Press **Play** — coins will spawn and you can start collecting!

### Console Commands

- `cc_restart` — Manually restart the current round (server only)

## ⚙️ Configuration

Editable in `CoinCollectorGame.cs`:

| Property | Default | Description |
|----------|---------|-------------|
| `RoundDuration` | 120 | Round length in seconds |
| `MaxCoins` | 15 | Max coins on the map at once |

## 🔧 Tech Details

- **Engine:** s&box (C# / .NET 8 / Source 2-based rendering)
- **Movement:** Built-in `WalkController` + `StandardPlayerAnimator`
- **Model:** Uses stock `citizen.vmdl` with random clothing
- **Coin model:** Uses `cointoss_coin.vmdl` (bundled with s&box)
- **UI:** s&box Razor-style panels (C# + SCSS)

## 📝 License

MIT — feel free to use, modify, and redistribute.

---

Built with ❤️ using [s&box](https://sbox.game) by Facepunch Studios.