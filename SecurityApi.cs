using System;
using System.Runtime.InteropServices;

namespace HttpConfig
{
    public class SecurityApi
    {
        public enum SidNameUse
        {
            SidTypeUser           = 1,
            SidTypeGroup          = 2,
            SidTypeDomain         = 3,
            SidTypeAlias          = 4,
            SidTypeWellKnownGroup = 5,
            SidTypeDeletedAccount = 6,
            SidTypeInvalid        = 7,
            SidTypeUnknown        = 8,
            SidTypeComputer       = 9
        }

        public enum Error
        {
            SUCCESS                            = 0,
            ERROR_INSUFFICIENT_BUFFER          = 122,
            ERROR_NONE_MAPPED                  = 1332,
			ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789,
        }

        private const int TOKEN_QUERY                 = 0x0008;
        private const int ERROR_INSUFFICIENT_BUFFER   = 122;
        private const int ERROR_NO_TOKEN              = 1008;
        private const int SecurityImpersonation       = 2;
        private const int WinBuiltinAdministratorsSid = 26;

        [DllImport("Advapi32.dll", SetLastError=true, EntryPoint="ConvertSidToStringSidW")]
        public static extern bool ConvertSidToStringSid(
            IntPtr Sid,
            out IntPtr StringSid);

        [DllImport("Advapi32.dll", SetLastError=true, EntryPoint="ConvertStringSidToSidW")]
        public static extern bool ConvertStringSidToSid(
            [MarshalAs(UnmanagedType.LPWStr)]string StringSid,
            out IntPtr Sid);

        [DllImport("Kernel32.dll", SetLastError=true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("Advapi32.dll", SetLastError=true, EntryPoint="LookupAccountSidW")]
        public static extern bool LookupAccountSid(
            [MarshalAs(UnmanagedType.LPWStr)]string lpSystemName,
            IntPtr lpSid,
            IntPtr lpName, // string
            ref int cchName,
            IntPtr lpReferencedDomainName, // string
            ref int cchReferencedDomainName,
            out SidNameUse peUse);

        [DllImport("Advapi32.dll", SetLastError=true, EntryPoint="LookupAccountNameW")]
        public static extern bool LookupAccountName(
            [MarshalAs(UnmanagedType.LPWStr)]string lpSystemName,
            [MarshalAs(UnmanagedType.LPWStr)]string lpAccountName,
            IntPtr Sid,
            ref int cbSid,
            IntPtr ReferencedDomainName,
            ref int cchReferencedDomainName,
            out SidNameUse peUse);

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool CheckTokenMembership(
            IntPtr TokenHandle,
            IntPtr SidToCheck,
            out bool IsMember);

        [DllImport("Advapi32", SetLastError=true)]
        private static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            int DesiredAccess,
            out IntPtr TokenHandle);

        [DllImport("Kernel32", SetLastError = true)]
        private static extern bool CloseHandle(
            IntPtr hObject);

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool CreateWellKnownSid(
            int WellKnownSidType,
            IntPtr DomainSid,
            IntPtr pSid,
            ref int cbSid);

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool OpenThreadToken(
            IntPtr ThreadHandle,
            int DesiredAccess,
            bool OpenAsSelf,
            out IntPtr TokenHandle);

        [DllImport("Kernel32", SetLastError = true)]
        private static extern IntPtr GetCurrentThread();

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool ImpersonateSelf(
            int ImpersonationLevel);

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool InitializeAcl(
            IntPtr pAcl,
            int nAclLength,
            int dwAclRevision);

        [DllImport("Advapi32", SetLastError = true)]
        private static extern int GetLengthSid(
            IntPtr pSid);

        [DllImport("Advapi32", SetLastError = true)]
        private static extern bool AddAccessAllowedAce(
            IntPtr pAcl,
            int dwAceRevision,
            int AccessMask,
            IntPtr pSid);

        [DllImport("Advapi32", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern int SetNamedSecurityInfo(
            string pObjectName,
            int ObjectType,
            int SecurityInfo,
            IntPtr psidOwner,
            IntPtr psidGroup,
            IntPtr pDacl,
            IntPtr pSacl);

        [DllImport("Advapi32", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern int GetNamedSecurityInfo(
            string pObjectName,
            int ObjectType,
            int SecurityInfo,
            int ppsidOwner,
            int ppsidGroup,
            out IntPtr ppDacl,
            int ppSacl,
            out IntPtr ppSecurityDescriptor);

        [DllImport("Advapi32", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern int SetEntriesInAcl(
            int cCountOfExplicitEntries,
            IntPtr pListOfExplicitEntries, // PEXPLICIT_ACCESS
            IntPtr OldAcl,
            out IntPtr NewAcl);

        [DllImport("Advapi32", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern void BuildExplicitAccessWithName(
            IntPtr pExplicitAccess,
            string pTrusteeName,
            int AccessPermissions,
            int AccessMode,
            int Inheritance);

        public static bool IsAdmin
        {
            get
            {
                int err;
                IntPtr hToken = IntPtr.Zero;
                IntPtr pSid   = IntPtr.Zero;

                try
                {
                    // Not necessary to CloseHandle()
                    IntPtr hThread = GetCurrentThread();

                    if(!OpenThreadToken(hThread, TOKEN_QUERY, true, out hToken))
                    {
                        err = Marshal.GetLastWin32Error();
                        if(err != ERROR_NO_TOKEN)
                            throw new Exception("OpenThreadToken failed (" + Marshal.GetLastWin32Error() + ").");

                        if(!ImpersonateSelf(SecurityImpersonation))
                            throw new Exception("ImpersonateSelf failed (" + Marshal.GetLastWin32Error() + ").");

                        if(!OpenThreadToken(hThread, TOKEN_QUERY, true, out hToken))
                            throw new Exception("OpenThreadToken failed (" + Marshal.GetLastWin32Error() + ").");
                    }

                    pSid = GetSid(WinBuiltinAdministratorsSid);

                    bool isAdmin;
                    if(!CheckTokenMembership(hToken, pSid, out isAdmin))
                        throw new Exception("CheckTokenMembership failed (" + Marshal.GetLastWin32Error() + ").");

                    return isAdmin;
                }
                finally
                {
                    if(hToken != IntPtr.Zero)
                        CloseHandle(hToken);

                    if(pSid != IntPtr.Zero)
                        Marshal.FreeHGlobal(pSid);

                    if(!RevertToSelf())
                        throw new Exception("RevertToSelf failed (" + Marshal.GetLastWin32Error() + ").");
                }
            }
        }

        private static IntPtr GetSid(int sidType)
        {
            int sidSize = 0;
            CreateWellKnownSid(sidType, IntPtr.Zero, IntPtr.Zero, ref sidSize);

            int err = Marshal.GetLastWin32Error();
            if(err != ERROR_INSUFFICIENT_BUFFER)
                throw new Exception("CreateWellKnownSid failed (" + err + ").");

            IntPtr pSid = Marshal.AllocHGlobal(sidSize);

            if(!CreateWellKnownSid(sidType, IntPtr.Zero, pSid, ref sidSize))
            {
                Marshal.FreeHGlobal(pSid);
                throw new Exception("CreateWellKnownSid failed (" + Marshal.GetLastWin32Error() + ").");
            }

            return pSid;
        }
    }
}
