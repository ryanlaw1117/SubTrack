# SubTrack

SubTrack is a lightweight **WPF desktop application for Windows** that helps you track active and disabled subscriptions in one place. It focuses on clarity, simplicity, and local-first storage—no accounts, no cloud sync, no tracking.

---

## Features

- Track **monthly and yearly** subscriptions
<img width="1032" height="640" alt="image" src="https://github.com/user-attachments/assets/d7dd7ca4-3ed3-42d3-ba29-0ac264a6566f" />
<img width="1032" height="638" alt="image" src="https://github.com/user-attachments/assets/37d7fa5c-9f0b-4f16-8871-26b4f89bcc43" />

- Toggle between **Active** and **Disabled** subscriptions
<img width="1032" height="636" alt="image" src="https://github.com/user-attachments/assets/95aec7e1-5788-4b37-ad31-a906ec3a4abf" />
 
- Sort Subscriptions by **Days Until Billing**, **Name**, and **Cost (High --> Low)**
<img width="1032" height="641" alt="image" src="https://github.com/user-attachments/assets/4d005e2b-8080-4ac8-a544-feb678fab37b" />
 
- Real-time **search** by subscription name or description
<img width="1031" height="643" alt="image" src="https://github.com/user-attachments/assets/8bc5dd8c-4b4a-49a6-b22a-deaa1cf0d0d0" />

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
2. Download the latest ZIP (for example: `SubTrack-v0.6.0-win64.zip`)
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

