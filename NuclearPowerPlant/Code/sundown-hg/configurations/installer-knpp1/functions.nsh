 Function IsDotNETInstalled
   Push $0
   Push $1

   StrCpy $0 1
   System::Call "mscoree::GetCORVersion(w .r2, i ${NSIS_MAX_STRLEN}, *i) i .r1"
   StrCmp $1 0 +2
     StrCpy $0 0

   Pop $1
   Exch $0
 FunctionEnd
