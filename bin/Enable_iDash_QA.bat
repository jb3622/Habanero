@echo off
set path=c:\windows\Microsoft.Net\framework\v2.0.50727\
CasPol.exe -q -m -ag 1.2 -url \\emea\dcp\gbha\tds\Groups\iDash_QA\* FullTrust

pause