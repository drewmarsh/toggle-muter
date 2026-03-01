<p align="center">
  <a href="https://github.com/drewmarsh/toggle-muter">
    <img src="toggle_muter_banner.png" width="598" alt="Banner">
  </a>
  <br><br>Toggle Muter uses a custom hotkey to mute/unmute the current application in focus. Available for <a href="#windows-portable-tray-application-download">Windows</a> as a lightweight system tray application and for <a href="#linux-bash-script-download">Linux</a> as a bash script.<br><br>
</p>

## [Windows (portable tray application) Download](https://github.com/drewmarsh/toggle-muter/releases/download/v1.0.1/toggle-muter-v1.0.1_portable.zip)
> [!NOTE]
> When you launch the application for the first time, a "Windows protected your PC" pop-up window may appear, preventing the "unrecognized app from starting". To start the application, click on <ins>More info</ins> and then click the `Run anyway` button.
<br>

## [Linux (bash script) Download](https://raw.githubusercontent.com/drewmarsh/toggle-muter/refs/heads/main/linux/toggle-muter-linux.sh)

**1. Download the script**
```bash
curl -o toggle-muter.sh https://raw.githubusercontent.com/drewmarsh/toggle-muter/refs/heads/main/linux/toggle-muter-linux.sh
```

**2. Grant the script execute permissions**
```bash
chmod +x toggle-muter.sh
```

**3. Install dependencies**
```bash
sudo apt install xdotool pulseaudio-utils libnotify-bin
```

**4. Bind it to a hotkey (varies depending on the distro)**

### **Linux Mint**:
- Navigate to `Keyboard` → `Shortcuts` → `Add custom shortcut`
- Name the shortcut and then set the command to the full path of the script (e.g. `/home/username/scripts/toggle-muter.sh`)
- Under `Keyboard shortcuts`, click the name of the shortcut that was just added
- Under `Keyboard bindings`, set the binding to your desired hotkey