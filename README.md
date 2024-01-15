Winbox Stats Checker Core
=========================

There is no output, except a SqliteDB file that contains the records from the machine.

Captures the RAM, CPU and HDD usage as percentages. Dynamically calculates the drives. Does not run on Linux.


To build:

```sh
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true; Start-Sleep -Seconds 2; GetChildItem -Path . -Recurse -Filter "wbscc.pdb" -File | ForEach-Object {
  Remove-Item -Path $_.FullName -Force -Confirm:$false
}
```