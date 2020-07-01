import os
import music21 as m21
from music21.converter.subConverters import ConverterMusicXML

us = m21.environment.UserSettings()
us_path = us.getSettingsPath()
if not os.path.exists(us_path):
    us.create()

# for windows
us['musescoreDirectPNGPath'] = 'D:/MuseScore/bin/MuseScore3.exe' 
us['musicxmlPath'] = 'D:/MuseScore/bin/MuseScore3.exe'
us['lilypondPath'] = 'D:/LilyPond/usr/bin/lilypond-windows.exe'

BASE_DIR = os.path.join( os.path.dirname( __file__ ), '..', 'Assets' )

s = m21.converter.parse('xmlFiles/Mary_had_a_Little_Lamb_-_variations_through_time.mxl')
s.write('lily.png', fp=BASE_DIR + '/output')

directories = os.listdir( BASE_DIR )

for item in directories:
    if item.endswith(".eps") or item.endswith(".tex") or item.endswith(".texi") or item.endswith(".count") or item.endswith("output"):
        os.remove( os.path.join( BASE_DIR, item ) )
    