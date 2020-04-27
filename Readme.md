# WOA Deployer
The application to install the Full Windows 10 Experience into your favorite device!

![Deployer](Docs/Deployer.png)!
*(Pardon the looks, the look & feel will be improved as the stable version gets ready)*
 
Supported devices:
- Microsoft Lumia 950/XL
- Raspberry Pi 3

# **Super easy to use. No-hassle.**

Please keep reading carefully. All you need is here.

- One of the supported devices	
- A Windows 10 ARM64 Image (.wim). Please, check [this link](https://github.com/WOA-Project/guides/blob/master/GettingWOA.md) to get it.

### Additional requirements
- To run WOA Deployer you need a recent version of Windows 10 (please, use the latest to ensure it'll run correctly, don't open issues otherwise)
- .NET Framework 4.7.2 (should come included in recent versions of Windows)

# How do I use it?

*Documentation in progress!*

# Download it!

Download the latest version [here](https://github.com/WOA-Project/WOA-Deployer/releases/latest)

You can choose 2 different applications:
- **GUI**: Graphical tool. It's the perfect pick for the average user.
- **CLI**: Command line tool. It's designed to be used from the command line.

# Executing the tool
1. Extract the .zip
3. Find the .exe file
4. Run it (you will be prompted for admin rights)

# Show the love 🧡

Do you like my tool? Has it been useful for you?
Then, I hope you [👉 support my work](Docs/Donations.md)

# Need help?
For Lumia phones: don't hesitate to join our great [Telegram group 📱](https://t.me/joinchat/Ey6mehEPg0Fe4utQNZ9yjA)

# Credits and Acknowledgements
## Lumia

- [Ben Imbushuo](https://github.com/imbushuo) for his awesome work with UEFI and misc. stuff.
- [Gustave M.](https://twitter.com/gus33000) for his HUGE load of work on drivers, testing, fixing... For his support, suggestions, for testing and those neat pieces of code!
- René Lergner ([Heathcliff74XDA](http://www.twitter.com/Heathcliff74XDA)) for WPInternals and for the code to read info from the phone. You started everything 😉
- [Googulator](https://github.com/Googulator). For his work on the USB-C and for the great support. 
- Swift (AppleCyclone) for suggestions and his work with the rest of team.
- Abdel [ADeltaX](https://twitter.com/ADeltaXForce?s=17) for testing and for his work.

## Raspberry Pi Support

This WOA Deployer is possible because the great community behind it. I would like to thank the brilliant minds behind this technical wonder. If you think you should be listed, please, contact me using the e-mail address on my profile.

- [Andrei Warkentin](https://github.com/andreiw) for the **64-bit Pi UEFI**, UEFI Pi (HDMI, USB, SD/MMC) drivers, improved ATF and Windows boot/runtime support.
- [MCCI](https://mcci.com/) for their great contribution to the RaspberryPI WOA project:
  - for porting their **TrueTask USB stack** to Windows 10 ARM64, and allowing non-commercial use with this project ([see license](Docs/mcci_license.md))
  - for funding the site of the project http://pi64.win and the discourse site http://discourse.pi64.win
  - Special thanks to Terry Moore for all the great support and commitment, and for setting up the online presence for the project and its infrastructure.
- [Ard Bisheuvel](http://www.workofard.com/2017/02/uefi-on-the-pi/) for initial ATF and UEFI ports
- [Googulator](https://github.com/Googulator) for his method to install WOA in the Raspberry Pi
- Bas Timmer ([@NTauthority](https://twitter.com/ntauthority)) for leaving ample thick hints that led to the development of HypDXe and the first bootable WOA builds
- Microsoft for their original [32-bit UEFI for Pi](https://github.com/ms-iot/RPi-UEFI), [Windows BSP drivers](https://github.com/ms-iot/rpi-iotcore), and for removing the HAL check that required HypDxe in the first place, so now we can run any new build.
- Mario Bălănică for his [awesome tool](https://www.worproject.ml), and for tips and support :)
	- daveb77
    - thchi12
    - falkor2k15
    - driver1998
    - XperfectTR
    - woachk
    - novaspirit
    - zlockard 
     
    ...for everything from ACPI/driver work to installation procedures, testing and so on.
- Microsoft for the 32-bit IoT firmware.

In addition to:

- [Ian Johnson](https://github.com/ipjohnson) for his wonderful DI-IOC container: [Grace](https://github.com/ipjohnson/Grace)
- [Eric Zimmerman](https://github.com/EricZimmerman) for [The Registry Project](https://github.com/EricZimmerman/Registry)
- [Jan Karger](https://github.com/punker76) for his wonderful [MahApps.Metro](https://mahapps.com)
- [ReactiveUI](https://reactiveui.net)
- [Adam Hathcock](https://github.com/adamhathcock) for [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [Etcher](https://www.balena.io/etcher/), the perfect tool for flashing.

And our wonderful group at Telegram for their testing and support!
- [RaspberryPiWOA](https://t.me/raspberrypiwoa)

## Related projects
These are the related projects. The Core Packages comes from them. Big thanks!

- [RaspberryPiPkg](https://github.com/andreiw/RaspberryPiPkg)
- [Microsoft IoT-BSP](https://github.com/ms-iot/bsp)
- [Raspberry Pi ATF](https://github.com/andreiw/raspberry-pi3-atf)
- [WOR Project](https://www.worproject.ml) by [Mario Bălănică](https://github.com/mariobalanica)

In addition to:

- [Ian Johnson](https://github.com/ipjohnson) for his wonderful DI-IOC container: [Grace](https://github.com/ipjohnson/Grace)
- [Eric Zimmerman](https://github.com/EricZimmerman) for [The Registry Project](https://github.com/EricZimmerman/Registry)
- [Jan Karger](https://github.com/punker76) [MahApps.Metro](https://mahapps.com)
- [ReactiveUI](https://reactiveui.net)
- [Adam Hathcock](https://github.com/adamhathcock) for [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [Markdown XAML](https://github.com/theunrepentantgeek/Markdown.XAML)
- [Serilog](https://serilog.net/)
- [Thomas Galliker](https://www.linkedin.com/in/thomasgalliker/?originalSubdomain=ch) for his great [Value Converters](https://github.com/thomasgalliker/ValueConverters.NET)
