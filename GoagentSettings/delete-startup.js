function main()
{
   wsh = new ActiveXObject('WScript.Shell');
   fso = new ActiveXObject('Scripting.FileSystemObject');
   if(fso.FileExists(wsh.SpecialFolders("Startup") + "\\goagent.lnk")) {
	fso.DeleteFile(wsh.SpecialFolders("Startup") + "\\goagent.lnk");
	wsh.Popup('删除GoAgent启动项成功', 3, 'GoAgent 对话框', 64);
   }
}

main();
