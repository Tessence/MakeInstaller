@echo off
set src_file=%1
set tgt_file=%2
set save_file=%3


set export_script=rh_export.txt
set combine_script=rh_combine.txt
set resname=resources
set iconame=favicon
echo clear cache.
del %combine_script%
del %export_script%
del %save_file%
del %resname%.rc 
del %resname%.res
del %iconame%.ico
del export.log
del combine.log
del rh.ini

echo get res.
echo [FILENAMES] > %export_script%
echo Open=%src_file% >> %export_script%
echo Log=export.log >> %export_script%
echo [COMMANDS] >> %export_script%
echo -extract %iconame%.ico, ICONGROUP, 32512, 0 >> %export_script%
echo -extract %resname%.rc, VERSIONINFO, 1, 0 >> %export_script%

rh.exe -script %export_script%
rh.exe -open %resname%.rc -save %resname%.res -action compile -log NUL



echo combine res.
echo [FILENAMES] > %combine_script%
echo Open=%tgt_file% >> %combine_script%
echo Save=%save_file% >> %combine_script%
echo Log=combine.log >> %combine_script%
echo [COMMANDS] >> %combine_script%
echo -addoverwrite %resname%.res, VERSIONINFO, 1, 0 >> %combine_script%
echo -addoverwrite %iconame%.ico,  ICONGROUP, 32512, 0 >> %combine_script%

rh.exe -script %combine_script%

echo combine finish