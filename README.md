# PegasusMetaCreator
A little program for editing / importing games into the Pegasus frontend



I made this program because i bought the racial justice game bundle on Itch.io.
My problem was that i had many games in folders but no convenient?!!? way to look at all of them 

So I looked into solutions and found the Pegasus frontend, which you can configure by a file names metadata.pegasus.txt

I wanted it to be able to dynamicly adding peroperties and automaticly fill properties by regex matching the file path


Known bug: when importing properties which fieldType is textbox-L or textbox-XL and they are populated over multiple lines, it only reads the first line.
