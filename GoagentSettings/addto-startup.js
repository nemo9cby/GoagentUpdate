function CreateShortcut(target_path,dir_path)
{
   wsh = new ActiveXObject('WScript.Shell');
   link = wsh.CreateShortcut(wsh.SpecialFolders("Startup") + '\\goagent.lnk');
   link.TargetPath = target_path;
   link.Arguments = '"' + dir_path + '\\proxy.py"';
   link.WindowStyle = 7;
   link.Description = 'GoAgent';
   link.WorkingDirectory = dir_path;
   link.Save();
}

function main()
{
   wsh = new ActiveXObject('WScript.Shell');
   CreateShortcut('"' + WScript.Arguments(0) + '\\goagent.exe"',WScript.Arguments(0));
   wsh.Popup('成功加入GoAgent到启动项', 3, 'GoAgent 对话框', 64);
}

main();
