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
Version 2.5.0.0        -    KSP Version: 0.24.2
- Fixed issue where basic button position triggers hover (Issue #40)
- Changed default button behaviour for new installs - now App Launcher button (Issue#39)
- Added App Launcher Button Exclusivity as an option (Issue #41)
- Added Window Movement Detection code and saving position after moves (Issue #42)
- Updated and added icons for a nubmer of packs (Issue #27 and Issue #22)
- Added option for replacing Stock Resources App (Issue #8)

Version 2.4.3.0        -    KSP Version: 0.24.2
- Compiled against 0.24 binaries
- Mad Resource Display Mutually exclusive to other apps (Issue #37)

Version 2.4.2.0        -    KSP Version: 0.24.0
- Fixed issue caused by overzealous commenting syndrome in previous release (Issue #36)
- Changed AppLauncher Icon to be more visible (Issue #35)

Version 2.4.1.0        -    KSP Version: 0.24.0
- Fixed issue with AppLauncher button not displaying in MapView (Issue #32)
- Added option to be able to choose left or right for Last Stage Bars (Issue #30)
- Stopped displaying the insertion icon when dragging of no change would occur (Issue #33)
- Adjusted Drag and Drop detection to better use the mouse position (Issue #34)

Version 2.4.0.0        -    KSP Version: 0.24.0
- Compiled against 0.24 binaries
- Changed Button Options so you can choose from Basic, Blizzy's Toolbar or KSP Launcher
- Added KSP Launcher Button stuff
- Changed texture loading to blurless method (from KAC Issue 33)

Version 2.3.0.0        -    KSP Version: 0.23.5
- Added Drag and Drop reordering to Resource Config (Issue #29)
- Added Insert Separator function in Resource Config (Issue #23)
- Added Default resource Layout for Stick Resources (Issue #27)
- Changed all Textures to be PNG to prevent blurring at lower texture qualities

Version 2.2.3.0        -    KSP Version: 0.23.5
- Made Version Check download truly background so no lock ups (Issue #20) - big thanks to Ted
- Fixed rates with tiny negative value not displaying minus indicator (Issue #17)
- Added Rate Display Options for under warp - can now choose UT or RealTime (RT) (Issue #16)
- Added option to disable split bars for all resources (Request #18)
- Added option to disable split bars per resources as well (Issue/Request #14)
- Fixed bug with Reserve display not working on initial vessel load (Issue #21)

Version 2.2.2.0        -    KSP Version: 0.23.5
- Removed links to spaceport
- Updated PluginFramework Tooltip issue
- Reworked split display to be Active and Reserve resources (Issue #1)
- Fixed Issues with dropdown and scrollview in resources Config (Issue #10)
- Adjusted display value for large values to use K and M suffixes (Issue #11)

Version 2.2.1.0		-	KSP Version: 0.23.5
- Added option for splitting display of All_Vessel resources on flow enabled
- Fixed Alarm firing on transitioning from Alert to Warn (Issue #6)
- Adjusted Iconset visuals to make more sense (I Hope :) )

Version 2.2.0.0		-	KSP Version: 0.23.5
- Added Support for HotRockets to AutoStaging
- Alternate rate display options
- Time remaining display options
- Hide When Full Option
- "Disable Display on Hover" option
- Changed version Check file

Version 2.1.3.0		-	KSP Version: 0.23.5
- 0.23.5 Recompile for new version of Unity/KSP

Version 2.1.2.0		-	KSP Version: 0.23
- Set AutoStaging to only watch for staged engine flameout - caters to Sepratron

Version 2.1.1.0		-	KSP Version: 0.23
- Very minor graphical fix on Unity window style

Version 2.1.0.0		-	KSP Version: 0.23
- Added API for other plugins to read details and acknowledge alarms
- Added Hide on empty setting - with delay
- Added Show All button - show hidden resources on click/hover
- Added Resource Transfer - same functionality from base part windows

Version 2.0.2.0		-	KSP Version: 0.23
- Fixed Window vis on alarm issue
- Fixed NaN rate display issue
- Verified image paths

Version 2.0.1.0		-	KSP Version: 0.23
- Fixed Alarm Noise issue
- Fixed resourceconfig window issues with Dropdowns

Version 2.0.0.0		-	KSP Version: 0.23
- Complete Code rewrite using KSPPluginFramework as base
- Updated some of the Icons
- Added ability to select resource and see part specific lists
- Added hover ability on part windows to highlight parts
- Added Resource Display options - Sort, grouping
- Added Resource Visibilty Options - Visible, Hidden or on Threshold
- Added Resource Monitors - set warning and alert thresholds
- Added Alarm Sounds + Acknowledgement
- Added Show on Alert for hidden window
- Added Update Checker from KAC

Version 1.2.1.0		-	KSP Version: 0.23
- Added EVA Propellant
- Added MFS Fuels
- Fixed issue with onhover when KSP ARP button hidden

Version 1.2.0.0		-	KSP Version: 0.23
- 0.23 Recompile for new version of Unity
- Added Option to choose to use Blizzies excellent toolbar - uses latebinding so no need to include DLL and no hard reference
- Added link in game so if common toolbar not installed people can jump to forum page
- Added option for users to choose display style - KSP/Unity
- Added feature so Mod owners can provide a Texture with their Mod and KSPARP will read that
- Added config so players can choose the order of precedence for the icon sets
- Fixed bug with GUILayout errors on initial display

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