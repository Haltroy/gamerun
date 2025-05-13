# gamerun script

One simple easy-to-use script that wraps around these for gaming:
 - [MangoHUD](https://github.com/flightlessmango/MangoHud)
 - [Gamemode](https://github.com/FeralInteractive/gamemode)
 - [Libstrangle](https://gitlab.com/torkel104/libstrangle)
 - and set-up dedicated GPU (via [prime-run](https://archlinux.org/packages/extra/any/nvidia-prime/) or [switcherooctl](https://gitlab.freedesktop.org/hadess/switcheroo-control/))
 
Currently only intended for Nvidia users with Nvidia's drivers (both propriety and open-sourced) with just switcherooctl as a backup solution. However, any request to add support for AMD and Intel or Nouveau is welcomed. 

# IMPORTANT

I decided to make this script into an actual full-fledged application where you can even edit stuff (enabling/disabling components or changing their configuration) per app via GUI. The development of that version will continue on [GitLab](https://gitlab.com/Haltroy) (currently I haven't started it so here's my profile instead). App will be Linux only (just like this script).

This script should still work, except there won't be any updates on it starting now (not like there are people use this shit and actually reading this anyways).

# INSTALLATION

1. Clone the repository with `git clone https://github.com/haltroy/gamerun.git` or [Download ZIP](https://github.com/Haltroy/gamerun/archive/refs/heads/main.zip)
2. On Arch Linux (and Arch-based distributions), use `makepkg -si` to install it directly or (for all distributions)
   copy the `gamerun` script to either:
    - `/bin`: `# cp gamerun /bin` (Requires privileges)
    - `/usr/bin`: `# cp gamerun /bin` (Requires privileges)
    - `/usr/local/bin`: `# cp gamerun /bin` (Requires privileges)
    - `$HOME/.local/bin`: `# cp gamerun /bin` (need to add this path to PATH variable)
        - For BASH: add `export $PATH="$HOME/.local/bin:$PATH"` to your `$HOME/.bashrc`
        - For ZSH: add `export $PATH="$HOME/.local/bin:$PATH"` to your `$HOME/.zshrc`
        - For FISH: run `fish_add_path "$HOME/.local/bin"`
3. Run games with `gamerun` command.
   - For Steam, open up properties for game and add `gamerun %command%` to the Additional Argument section.
   
## UNINSTALL

Just either remove the script or use `pacman -R gamerun` on Arch & Arch-based systems.

# CONFIGURE

Configuration is made with multiple files.

 - For system-wide configuration, add files to `/etc/gamerun/` folder
 - For personal configuration, add files to `$HOME/.config/gamerun/` folder
 
Here's a list of each file and what they configure:
 - `pre_launch`: Script to run before launching. Useful for pausing crypto mining or setting up compositor for gaming.
 - `post_launch`: Script to run after launching. Useful for reverting stuff from pre-launch script.
 - `strangle_vars`: Additional arguments to add to libstrangle (ex. `60:40` tells game to run on 60 FPS on AC and 40 FPS on battery)
 - `disable_strangle`: Disables libstrangle if this file exists.
 - `disable_gpu`: Disables switching ot dedicated GPU if this file exists.
 - `disable_gamemode`: Disables Gamemode if this file exists.
 - `disable_mangohud`: Disables MangoHUD if this file exists.

**NOTE:** You don't need to use this for special launchers like MultiMC/Prism Launcher or Bottles since they have the same settings in their own Settings menu.

# DEVELOPMENT

~Any requests and PRs are welcomed. Just don't forget to update the SHA512 checksum on PKGBUILD file if you changed anything on the main script.~

Future development of this script halted in favor of an upcoming project.

I originally intended this for my own personal use (hence why there's not enough stuff here in the first place) but I thought maybe others can use it as well.


# WELL KNOWN ISSUES

### Game doesn't launch:
This usually caused by libstrangle. You can:
 - Remove it
 - Disable it (via `touch $HOME/.config/gamerun/disable_strangle`)
 - Disable DLSYM hijacking with `echo "-n" > $HOME/.config/gamerun/strangle_vars`
 - Make it Vulkan only (`echo "-k" > $HOME/.config/gamerun/strangle_vars`)
 - or use `libstrangle-git` package instead.

You can limit FPS using MangoHUD too.

If disabling libstrangle didn't work, make an issue [here](https://github.com/Haltroy/gamerun/issues/new).

### Game doesn't launch with *X* comonent (component missing)

Then you might be mssing the *X* component already. Install it via package manager. Also cheeck if it's accessible. If not, find it's directory and add it to PATH variable:
````bash
# In ~/.bashrc and/or ~/.zshrc and/or ~/.profile
export PATH=$PATH:full_path_of_directory_here
````

If you still having issues, try manually downlaoding the package from your distribution's online package search and look into its contents. Linux distros usually put the files in their relative path so a component in /usr/bin/ in the package will be extracted to /usr/bin.
If it still doesn't work, contact to your distro's maintainer.

Some componenets (like gamemode, libstrangle and discreet GPU switching) don't have an UI. Only MangoHUD has it.

### Component *X* is missing in the script

Feel free to make an issue [here](https://github.com/Haltroy/gamerun/issues/new) or submit your own code [here](https://github.com/Haltroy/gamerun/pulls).
