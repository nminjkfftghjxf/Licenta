SILVA-Mobile Android app for plant identification using external APIS
Google drive link to view app tutorial https://drive.google.com/file/d/1wyvb5j6JHN0rwCO0aiNOWaC7QOOukFzR/view?usp=drive_link

Installation steps:

1. Install Visual Studio Community, I used version 2022, with the packages:   
a) .NET Multi-platform App UI development,   
b) .NET desktop development,   
c) Desktop development with C++,   
d) Mobile development with C++,   
e) Visual Studio extension development.   
Approximately 8 GB needed for installation.

2. The project is of type .NET MAUI App.
Install the necessary NuGet packages, if they do not already exist: Navigation bar -> Project -> Manage NuGet Packages -> Browse window -> Search:
a) Camera.MAUI,
b) CommunityToolkit.Maui;
c) CommunityToolkit.MvvM,
d) CsvHelper,
e) Microsoft.AspNetCore.Mvc.Core,
f) Microsoft.Extensions.Logging.Debug,
g) Microsoft.Maui.Controls,
h) Microsoft.Maui.Essentials,
i) Microsoft.NET.ILLink.Tasks,
j) SkiaSharp.Extended.UI.Maui.

Troubleshooting steps:The application is made only for Android devices and was debugged on a physical Android device.
1. The phone must have developer mode enabled:   
a) Access the phone's settings   
b) Go to About phone
c) Access Software Information
d) Tap on Build number 7 times to enable developer mode, entry of the device's password/PIN may be required if it exists, it may be necessary to exit settings in order for developer mode to be displayed in settings
e) Enter Developer options and enable USB debugging

2. Use a data cable that connects the device to the PC 3. In Visual Studio Community, click on the compile button (the button with the filled green arrow) in the toolbar.
3. The button should display the name of the connected device. If the phone's name does not appear, click on the small white arrow on the right and find the respective name.
4. If the name still does not appear, it means that the cable being used is not a data cable. Following the steps above, the application should be able to be installed/run/debugged on the phone.
5. If debugging is desired on another type of emulator (Web, iPhone - although it doesn't make sense as the project does not contain the necessary packages/permissions for them) in the file "plante2.3.csproj" the TargetFramework must be uncommented and modified according to the requirements.



