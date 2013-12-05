Alternate Resource Panel - %VERSIONSTRING%
----------------------------------
An Alternate display of resources in whole vessel and current stage

By Trigger Au

Forum Thread for latest: http://forum.kerbalspaceprogram.com/threads/60227-KSP-Alternate-Resource-Panel
Documentation Site: https://sites.google.com/site/kspalternateresourcepanel

INSTALLATION
******************* NOTE  ******************* NOTE ******************* NOTE *******************
IF YOU WANT TO MAINTAIN YOUR SETTINGS DO NOT COPY THE CONFIG.XML FILE OVER
******************* NOTE  ******************* NOTE ******************* NOTE *******************

Installing the plugin involves copying the plugin files into the correct location in the KSP aplication folder
1. Extract the Zip file you have downloaded to a temporary Location
2. Open the Extracted folder structure and open the KSPAlternateResourcePanel_v%VERSIONSTRING% Folder
3. Inside this you will find a GameData folder which contains all the content you will need
4. Open another window to your KSP application folder - We'll call this <KSP_OS>
5. Copy the Contents of the extracted GameData folder to the <KSP_OS>\GameData Folder
6. Start the Game and enjoy :)

TROUBLESHOOTING
The plugin records troubleshooting data in the "<KSP_OS>\KSP_Data\output_log.txt".
If there are errors in loading the config you can delete the "<KSP_OS>\GameData\TriggerTech\PluginData\KSPAlternateResourcePanel\config.xml" and restart the game

LICENSE
Licensed under Creative Commons Attribution-NonCommercial-Sharealike 3.0 Unported License. Visit the documentation site for more details and Attribution

VERSION HISTORY
Version 1.1.1.0		-	KSP Version: 0.22
- Finished 1st iteration of icons - includes icons for: Kethane, Deadly Reentry, Extra Planetary Launchpads, KSP Interstellar, Life Support By Bobcat, Near Future, TAC Life Support
- Made the settings button a little wider to make it visible behind other fixed buttons

Version 1.1.0.0		-	KSP Version: 0.22
- Changed loading method so that it parses the Icons folder
- Also changed byte loading method to use System.IO instead of KSP.IO - see if thats the Linux64bit problem
- Optional display of Instant consumption/rates
- Converted draw code to use windows so main window is draggable and clamped to screen
- Can lock window panel so you dont inadvertantly drag it
- Separated settings window so yiou can drag to any edge and settings moves around to stay visible (pretty chuffed this worked)
- Added a bunch of icons - more to come
- Added folder for user to put custom icons in for ones I missed

Version 1.0.3.0		-	KSP Version: 0.22
- Typo in the darn build Script

Version 1.0.2.0		-	KSP Version: 0.22
- Initial Release
- Displays Resource Status
- Displays separate status for Stagong resources
- Allows for Staging of vessel
- Allows for Space bar staging in Mapview