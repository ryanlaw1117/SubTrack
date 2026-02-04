# SubTrack

SubTrack is a lightweight **WPF desktop application for Windows** that helps you track active and disabled subscriptions in one place. It focuses on clarity, simplicity, and local-first storage—no accounts, no cloud sync, no tracking.

---

## Features

- Track **monthly and yearly** subscriptions
<img width="782" height="442" alt="image" src="https://github.com/user-attachments/assets/3b91d628-aab4-4ed5-9591-5b918d3c8dac" />
<img width="783" height="442" alt="image" src="https://github.com/user-attachments/assets/47ee0e3b-47e0-4ecc-8625-09d1b24daee7" />

- Toggle between **Active** and **Disabled** subscriptions
<img width="784" height="444" alt="image" src="https://github.com/user-attachments/assets/8d2366c8-4cec-46dd-bf82-ac814641f898" />

- Real-time **search** by subscription name or description
<img width="782" height="441" alt="image" src="https://github.com/user-attachments/assets/f5dc89e1-8cde-4900-9ba5-6932bfc126e1" />

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
<img width="778" height="232" alt="image" src="https://github.com/user-attachments/assets/6a40f305-7298-49aa-b89c-8bf89d4d0888" />

4. Run:
`SubTrack.exe`
<img width="819" height="454" alt="image" src="https://github.com/user-attachments/assets/67477612-fc42-4e8f-87e8-1c6ee9e39688" />

No installer is required.


## Creating a Desktop Shortcut (Recommended)

You can create a shortcut to run SubTrack from anywhere.

1. Right-click `SubTrack`
2. Click **Show More Options**
<img width="1088" height="688" alt="image" src="https://github.com/user-attachments/assets/9e0df516-b6b1-480f-893a-152526e6b14f" />
3. Click **Create shortcut**
<img width="826" height="621" alt="image" src="https://github.com/user-attachments/assets/195b22e2-a08e-4f56-b73c-43b5d8e9f80b" />
 
4. Move the shortcut to:
    - Desktop
    - Documents
    - Anywhere you'd like

## Where Your Data Is Stored

SubTrack stores your data locally in your Windows user profile:
`%AppData%\SubTrack\`
This ensures your subscriptions persist between launches without setup or sign-in.


## Versioning & Update Patches

SubTrack uses semantic versioning:
MAJOR.MINOR.PATCH

- **PATCH** — Bug fixes
- **MINOR** — New features
- **MAJOR** — Breaking changes

See the **Releases** page or patch notes for detailed change history.

