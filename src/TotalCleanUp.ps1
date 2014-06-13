dir -Include bin -Recurse | del -Recurse
dir -Include packages -Recurse | del -Recurse
dir -Include obj -Recurse | del -Recurse
dir -Include _Resharper* -Recurse | del -Recurse

Get-ChildItem -Include *.orig -Recurse | del 
Get-ChildItem -Include *.reg -Recurse | del
Get-ChildItem -Include *.suo -Recurse -Force | del -Force