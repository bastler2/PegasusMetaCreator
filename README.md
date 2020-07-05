# PegasusMetaCreator
A little program for editing / importing games into the Pegasus frontend



I made this program because i bought the racial justice game bundle on Itch.io.
My problem was that i had many games in folders but no convenient?!!? way to look at all of them 

So I looked into solutions and found the Pegasus frontend, which you can configure by a file names metadata.pegasus.txt

I wanted it to be able to dynamicly adding peroperties and automaticly fill properties by regex matching the file path


Known bug: when importing properties which fieldType is textbox-L or textbox-XL and they are populated over multiple lines, it only reads the first line.


How to use it(without having to change the "default programm folder path" variable in the configurations tab):
1. Download the latest release here:
2. Put the folder in the same folder as your games are (default programm path = ..\)
3. Start the programm and select the game your wish to add 
4. Edit your properties (if you want to edit properties after you added the programm you must press "save changes")
5. Press "Add program"
6. Jump back to step 4 to add more programs
7. Press Export to export the metadata.pegasus.txt (default metadata path = ..\metadata.pegasus.txt)

Metadata Editor/Creator
  Add Programm = add current displayed properties to the "Added programs" list
  Save Changes = saves changes when editing properties after programm was added
  Export = exports the metadata.pegasus.txt file (path can be changed n the Configurations tab -> Default metadata file path)
  Import = imports the metadata.pegasus.txt file (path can be changed n the Configurations tab -> Default metadata file path)
  Delete programm = deletes selected programm from "Added programs"

Configurations
  Regex AutoFill
    SettingName = name of property which will get automaticly filled
    AppendFront = Add string to the front of the file path
    RegexFilter = Filter to use regular expression on the file path (this is applied before AppendFront and AppendEnd )
    AppendEnd = Add string to the end of the file path
    FullMatch = check if you want to use the full match, uncheck if you want to use Group match
    Group match = if FullMatch isn´t enabled this in the index of the matched group
  Settings
    Default program folder path = where should i look for executables
    Default metadata file path = where is the metadata.pegasus.txt file located
  Add properties
    Property name = name of property
    Field type =
      textbox = single line 
      textbox-L = multiline and bigger
      textbox-XL = multiline and bigger
