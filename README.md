# SubTrack

SubTrack is a lightweight **WPF desktop application for Windows** that helps you track active and disabled subscriptions in one place. It focuses on clarity, simplicity, and local-first storage—no accounts, no cloud sync, no tracking.

---

## Features

- Track **monthly and yearly** subscriptions
- Toggle between **Active** and **Disabled** subscriptions

  <img width="784" height="444" alt="image" src="https://github.com/user-attachments/assets/8d2366c8-4cec-46dd-bf82-ac814641f898" />

- Real-time **search** by subscription name or description
- Automatic monthly and yearly total calculations
- Visual indicator for time until the next billing date
- Optional **accent color** per subscription
- Add, edit, and delete subscriptions
- Local JSON storage (no internet required)



## System Requirements

- Windows 10 or Windows 11 (64-bit)
- .NET Desktop Runtime (required for framework-dependent builds)



## Download & Run

1. Go to the **Releases** section of this repository
2. Download the latest ZIP (for example: `SubTrack-v1.2.0-win64.zip`)
3. Extract the ZIP to any folder
4. Run:
`SubTrack.exe`

No installer is required.


## Creating a Desktop Shortcut (Recommended)

You can create a shortcut to run SubTrack from anywhere.

### Option 1 — Desktop Shortcut

1. Locate `SubTrack.exe`
2. Right-click it
3. Click **Send to → Desktop (create shortcut)**

### Option 2 — Shortcut Anywhere

1. Right-click `SubTrack.exe`
2. Click **Create shortcut**
3. Move the shortcut to:
- Desktop
- Documents
- Any folder you like


## Where Your Data Is Stored

SubTrack stores your data locally in your Windows user profile:
`%AppData%\SubTrack\`


This ensures your subscriptions persist between launches without setup or sign-in.


## Versioning & Updates

SubTrack uses semantic versioning:
MAJOR.MINOR.PATCH


- **PATCH** — Bug fixes
- **MINOR** — New features
- **MAJOR** — Breaking changes

See the **Releases** page or patch notes for detailed change history.

