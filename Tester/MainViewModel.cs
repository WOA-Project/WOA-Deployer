using System.Collections.Generic;
using System.Reactive;
using System.Windows;
using Deployer;
using Deployer.UI;
using ReactiveUI;

namespace Tester
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var dialogService = new Dialog();
            

            OpenCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var options = new List<Option>()
                {
                    new Option("OK", OptionValue.OK),
                    new Option("Cancel", OptionValue.Cancel),
                };

                var markdown = @"# **Super easy to use. No-hassle.**

![Image](Image.jpg)

Please keep reading carefully. All you need is here.

## It allows Dual Booting Windows 10 Mobile
You don't have to ditch Windows 10 Phone 😃 You can **keep it!**! Using this tool, you can enable Dual Boot in literally 2 clicks. Please, check [this video](https://www.youtube.com/watch?v=3j2rWL4hHGc) to see a demo of this feature.

# Requirements
- A Lumia 950 or 950 XL with **unlocked bootloader** that can correctly enter *Mass Storage Mode*
	- You can unlock the bootloader using this tool: [WPInternals](http://www.wpinternals.net). You can follow [this guide](https://github.com/WOA-Project/guides/blob/master/BL-unlock.md).
- A USB-C cable to connect the Lumia to your PC
- A Windows 10 ARM64 Image (.wim). Please, check [this link](https://github.com/WOA-Project/guides/blob/master/GettingWOA.md) to get it.
# Executing the tool
1. Extract the .zip to a folder in you PC
2. Navigate to the GUI folder
3. Find the .exe file
4. Run it

## Important
This tool requires Windows 10 to run. If you get deployment errors, please, update.

# Show the love 🧡

Do you like my tool? Has it been useful for you?
Then, I hope you [👉 support my work](Docs/Donations.md)

# Credits and Acknowledgements
- [Ben Imbushuo](https://github.com/imbushuo) for his awesome work with UEFI and misc stuff.
- [Gustave M.](https://twitter.com/gus33000) for his HUGE load of work on drivers, testing, fixing... For his support, suggestions, for testing and those neat pieces of code!
- René Lergner ([Heathcliff74XDA](http://www.twitter.com/Heathcliff74XDA)) for WPInternals and for the code to read info from the phone. You started everthing 😉
- [Googulator](https://github.com/Googulator). For his work on the USB-C and for the great support. 
- Swift (AppleCyclone) for suggestions and his work with the rest of team.
- Abdel [ADeltaX](https://twitter.com/ADeltaXForce?s=17) for testing and for his work.
";
                var option = await dialogService.PickOptions(markdown, options, "Images");

                if (option.OptionValue == OptionValue.OK)
                {
                    MessageBox.Show("Yes was selected!");
                }
            });
        }

        public ReactiveCommand<Unit, Unit> OpenCommand { get; set; }
    }

   
}