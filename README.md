# PegasusMetaCreator
A little program for editing / importing games into the Pegasus frontend

I made this program because I bought the racial justice game bundle on Itch.io.
My problem was that I had many games in folders but no convenience way to look at all of them.

So I looked into solutions and found the Pegasus frontend, which you can configure by a file names metadata.pegasus.txt

I wanted it to be able to dynamically adding properties and automatically fill properties by regex matching the file path

Known bug: when importing properties which fieldType is textbox-L or textbox-XL, and they are populated over multiple lines, it only reads the first line.

How to use it(without having to change the "default program folder path" variable in the configurations tab):
  1. Download the latest release here:
  2. Put the folder in the same folder as your games are (default program path = ..\)
  3. Start the program and select the game you wish to add
  4. Edit your properties (if you want to edit properties after you added the program you must press "save changes")
  5. Press "Add program"
  6. Jump back to step 4 to add more programs
  7. Press Export to export the metadata.pegasus.txt (default metadata path = ..\metadata.pegasus.txt)
   
# Info Metadata Editor/Creator tab
![Alt text](/PegasusMetaCreator/Main.PNG?raw=true)
**Metadata Editor/Creator options** | **Description**
:-----:|:-----:
Add Program | add current displayed properties to the "Added programs" list
Save Changes | saves changes when editing properties after program was added
Export | exports the metadata.pegasus.txt file (path can be changed n the Configurations tab -> Default metadata file path)
Import | imports the metadata.pegasus.txt file (path can be changed n the Configurations tab -> Default metadata file path)
Delete program | deletes selected program from "Added programs"


# Info Configurations tab
![Alt text](/PegasusMetaCreator/Config.PNG?raw=true)
Regex AutoFill = applys regex filter on game path, after that it can append at the front of the string or at the end with AppendFront and AppendEnd, if these 3 steps are finished it will automatically fill your set field when you are selecting a game
Info: The program comes with 3 default Regex AutoFill rules 
  1. game -> converts path like this: C:\SuperHot\Superhot.exe -> Superhot
  2. file -> 'converts' like this: C:\SuperHot\Superhot.exe -> C:\SuperHot\Superhot.exe (just for auto-fill without changing)
  3. assets.box_front -> converts like this C:\SuperHot\Superhot.exe -> C:\SuperHot\Superhot.png

**Regex AutoFill options** | **Description**
:-----:|:-----:
SettingName | name of property which will get automatically filled
AppendFront | Add string to the front of the file path
RegexFilter | Filter to use regular expression on the file path (this is applied before AppendFront and AppendEnd )
AppendEnd | Add string to the end of the file path
FullMatch | check if you want to use the full match
Group match | if FullMatch isn't enabled this in the index of the matched group

**Settings options** | **Description**
:-----:|:-----:
Default program folder path | where should I look for executables
Default metadata file path | where is the metadata.pegasus.txt file located

**Add properties options** | **Description**
:-----:|:-----:
|Property name | name of property
|Field type | textbox = single line
|Field type | textbox-L = multiline and bigger
|Field type | textbox-XL = multiline and bigger
