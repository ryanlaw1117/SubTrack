# SubTrack

SubTrack is a lightweight **WPF desktop application for Windows** that helps you track active and disabled subscriptions in one place. It focuses on clarity, simplicity, and local data storage—no accounts, no cloud sync, no tracking.


## Features

* Track **monthly and yearly** subscriptions
* Toggle between **Active** and **Disabled** subscriptions
* Automatic total cost calculations
* Visual indicator for time until next billing date
* Add, edit, and delete subscriptions
* Local JSON storage (no internet required)


## System Requirements

* Windows 10 or Windows 11 (64-bit)
* .NET Desktop Runtime (required if using the framework-dependent build)


## Download & Run

1. Go to the **Releases** section of this repository
2. Download the latest ZIP (for example: `SubTrack_v1.0.2_win-x64.zip`)
3. Extract the ZIP to any folder
4. Run:

   ```
   SubTrack.exe
   ```

No installer is required.


## Creating a Desktop Shortcut (Recommended)

You can create a shortcut to run SubTrack from anywhere.

### Option 1 — Desktop Shortcut

1. Locate `SubTrack.exe`
2. Right-click it
3. Click **Send to → Desktop (create shortcut)**

You can now launch SubTrack from your desktop.


### Option 2 — Shortcut Anywhere (Desktop, Taskbar, Folder)

1. Right-click `SubTrack.exe`
2. Click **Create shortcut**
3. Move the shortcut to:

   * Desktop
   * Documents
   * Any folder you like

## Where Your Data Is Stored

SubTrack stores your subscriptions **locally** in your Windows user profile:

```
%AppData%\SubTrack\
```

This allows your data to persist between app launches without requiring setup or sign-in.


## Versioning & Updates

SubTrack uses semantic versioning:

```
MAJOR.MINOR.PATCH
```

* **PATCH** — Bug fixes
* **MINOR** — New features
* **MAJOR** — Breaking changes

Check `PATCH_NOTES.md` for detailed change history.


## Known Limitations

* Color selection is present but not fully implemented yet
* No cloud sync or backup (by design)
* Windows-only

