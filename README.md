<p align="center">
  <a href="https://github.com/drewmarsh/toggle-muter">
    <img src="toggle_muter_banner.png" width="598" alt="Banner">
  </a>
  <br><br>Toggle Muter is a lightweight system tray application that uses a custom hotkey to mute/unmute the current application in focus<br><br>
</p>

### [Windows (portable tray application) Download](https://github.com/drewmarsh/toggle-muter/releases/download/v1.0.1/toggle-muter-v1.0.1_portable.zip)
> [!NOTE]
> When you launch the application for the first time, a "Windows protected your PC" pop-up window may appear, preventing the "unrecognized app from starting". To start the application, click on  <ins>More info</ins> and then click the `Run anyway` button.

### [Linux (bash script) Download](https://raw.githubusercontent.com/drewmarsh/toggle-muter/main/linux/toggle-muter.sh)

**1. Download the script**
```bash
curl -o toggle-muter.sh https://raw.githubusercontent.com/drewmarsh/toggle-muter/main/linux/toggle-muter.sh
```

**2. Install dependencies**
```bash
sudo apt install xdotool pulseaudio-utils libnotify-bin
```

**3. Grant execute permissions**
```bash
chmod +x toggle-muter.sh
```

**4. Bind it to a hotkey**

This varies by distro and desktop environment. For example, on **Linux Mint**: go to *Keyboard → Shortcuts → Add custom shortcut*, set the command to the full path of the script (e.g. `/home/username/Scripts/toggle-muter.sh`), and assign your desired keybind.
