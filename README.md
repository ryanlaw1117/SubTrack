# SubTrack

SubTrack is a lightweight **WPF desktop application for Windows** that helps you track active and disabled subscriptions in one place. It focuses on clarity, simplicity, and local-first storage—no accounts, no cloud sync, no tracking.

---

## Features

- Track **monthly and yearly** subscriptions
<img width="783" height="445" alt="image" src="https://github.com/user-attachments/assets/4b775add-133b-4e17-82f9-da290cb5897d" />
<img width="782" height="440" alt="image" src="https://github.com/user-attachments/assets/5aa2356c-1d25-4047-b457-7037e44e8562" />


- Toggle between **Active** and **Disabled** subscriptions
<img width="783" height="440" alt="image" src="https://github.com/user-attachments/assets/08f1aebd-20a9-4248-886c-94843a35bd8b" />


- Sort Subscriptions by **Days Until Billing**, **Name**, and **Cost (High --> Low)**
<img width="782" height="440" alt="image" src="https://github.com/user-attachments/assets/d681e79a-4ead-4150-ad63-ac70a258ee5c" />

 
- Real-time **search** by subscription name or description
<img width="781" height="443" alt="image" src="https://github.com/user-attachments/assets/ebc06ed2-ea89-402c-b276-62872e145a22" />

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

