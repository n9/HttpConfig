using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace HttpConfig
{
    public enum UrlPermission
    {
        All,
        Registration,
        Delegation,
        Other,
    }

    public class Acl : ICloneable
    {
        private ArrayList _acl = new ArrayList();

        public Acl() { }

        public ArrayList Aces
        {
            get { return _acl; }
        }

        public object Clone()
        {
            Acl newAcl = new Acl();

            foreach(Ace entry in _acl)
                newAcl._acl.Add(new Ace(entry.User, entry.AccountNameMapped, entry.Permission, entry.OtherPerm));

            return newAcl;
        }

        public string FriendlyString
        {
            get
            {
                StringBuilder friendlyString = new StringBuilder();

                foreach(Ace entry in _acl)
                {
                    friendlyString.Append("(");
                    friendlyString.Append(entry.User);
                    friendlyString.Append(";");
                    friendlyString.Append(entry.Permission.ToString());
                    friendlyString.Append(")");
                }

                return friendlyString.ToString();
            }
        }

        public static Acl FromSddl(string sddl)
        {
            Acl newAcl = new Acl();

            string[] aceStrings = sddl.Split(new char[] { '(', ')' });

            // it's split on ( and ), so we have blanks every other item
            for(int i = 1; i < aceStrings.Length; i++)
            {
                if((i % 2) > 0)
                    newAcl._acl.Add(Ace.FromSddl(aceStrings[i]));
            }

            return newAcl;
        }

        public string ToSddl()
        {
            StringBuilder sddl = new StringBuilder();

            sddl.Append("D:");

            foreach(Ace ace in _acl)
                ace.AddSddl(sddl);

            return sddl.ToString();
        }
    }

    public class Ace
    {
        private string _user;
        private UrlPermission _permission;
        private string _otherPerm;
        private bool _accountNameMapped;

        private Ace() { }

        public Ace(string user, bool accountNameMapped, UrlPermission permission, string otherPerm)
        {
            _user              = user;
            _accountNameMapped = accountNameMapped;
            _permission        = permission;
            _otherPerm         = otherPerm;
        }

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        public bool AccountNameMapped
        {
            get { return _accountNameMapped; }
            set { _accountNameMapped = value; }
        }

        public UrlPermission Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }

        public string OtherPerm
        {
            get { return _otherPerm; }
            set { _otherPerm = value; }
        }

        public void AddSddl(StringBuilder sddl)
        {
            sddl.Append("(A;;");

            switch(_permission)
            {
                case UrlPermission.All:
                    sddl.Append("GA");
                    break;

                case UrlPermission.Registration:
                    sddl.Append("GX");
                    break;

                case UrlPermission.Delegation:
                    sddl.Append("GW");
                    break;

                case UrlPermission.Other:
                    sddl.Append(_otherPerm);
                    break;
            }

            sddl.Append(";;;");

            sddl.Append(_accountNameMapped ? EncodeSid() : _user);

            sddl.Append(")");
        }

        public static Ace FromSddl(string sddl)
        {
            string[] tokens = sddl.Split(';');

            if(tokens.Length != 6)
                throw new ArgumentException("Invalid SDDL string.  Too many or too few tokens.", "sddl");

            string permString = tokens[2];

            string stringSid = tokens[5];

            Ace ace = new Ace();

            switch(permString)
            {
                case "GA":
                    ace._permission = UrlPermission.All;
                    break;

                case "GX":
                    ace._permission = UrlPermission.Registration;
                    break;

                case "GW":
                    ace._permission = UrlPermission.Delegation;
                    break;

                default:
                    ace._permission = UrlPermission.Other;
                    ace._otherPerm = permString;
                    break;
            }

            ace._accountNameMapped = DecodeSid(stringSid, out ace._user);

            return ace;
        }

        private static bool DecodeSid(string stringSid, out string accountName)
        {
            IntPtr pSid     = IntPtr.Zero;
            IntPtr pAccount = IntPtr.Zero;
            IntPtr pDomain  = IntPtr.Zero;

            try
            {
                accountName = stringSid;

                if(!SecurityApi.ConvertStringSidToSid(stringSid, out pSid))
                    throw new Exception("ConvertStringSidToSid failed.  Error = " + Marshal.GetLastWin32Error().ToString());

                int accountLength = 0;
                int domainLength  = 0;

                SecurityApi.SidNameUse use;

                if(!SecurityApi.LookupAccountSid(null, pSid, pAccount, ref accountLength, pDomain, ref domainLength, out use))
                {
                    int error = Marshal.GetLastWin32Error();

                    if(error != (int)SecurityApi.Error.ERROR_INSUFFICIENT_BUFFER)
                    {
                        if((error == (int)SecurityApi.Error.ERROR_NONE_MAPPED) || (error == (int)SecurityApi.Error.ERROR_TRUSTED_RELATIONSHIP_FAILURE))
                            return false;
                        else
                            throw new Exception("LookupAccountSid failed.  Error = " + Marshal.GetLastWin32Error().ToString());
                    }
                }

                pAccount = Marshal.AllocHGlobal(accountLength * 2); // 2-byte unicode...we're using the "W" variety of the funcion

                pDomain = Marshal.AllocHGlobal(domainLength * 2); // 2-byte unicode...we're using the "W" variety of the funcion

                if(!SecurityApi.LookupAccountSid(null, pSid, pAccount, ref accountLength, pDomain, ref domainLength, out use))
                {
                    int error = Marshal.GetLastWin32Error();

                    if((error == (int)SecurityApi.Error.ERROR_NONE_MAPPED) || (error == (int)SecurityApi.Error.ERROR_TRUSTED_RELATIONSHIP_FAILURE))
                        return false;
                    else
                        throw new Exception("LookupAccountSid failed.  Error = " + error.ToString());
                }

                accountName = Marshal.PtrToStringUni(pDomain) + "\\" + Marshal.PtrToStringUni(pAccount);

                return true;
            }
            finally
            {
                if(pSid != IntPtr.Zero)
                    SecurityApi.LocalFree(pSid);

                if(pAccount != IntPtr.Zero)
                    Marshal.FreeHGlobal(pAccount);

                if(pDomain != IntPtr.Zero)
                    Marshal.FreeHGlobal(pDomain);
            }
        }

        private string EncodeSid()
        {
            IntPtr pSid       = IntPtr.Zero;
            IntPtr pStringSid = IntPtr.Zero;
            IntPtr pDomain    = IntPtr.Zero;

            try
            {
                int sidLength = 0;
                int domainLength  = 0;

                SecurityApi.SidNameUse use;

                if(!SecurityApi.LookupAccountName(null, _user, pSid, ref sidLength, pDomain, ref domainLength, out use))
                {
                    int error = Marshal.GetLastWin32Error();

                    if(error != (int)SecurityApi.Error.ERROR_INSUFFICIENT_BUFFER)
                        throw new Exception("LookupAccountName failed.  Error = " + Marshal.GetLastWin32Error().ToString());
                }

                pSid = Marshal.AllocHGlobal(sidLength);

                pDomain = Marshal.AllocHGlobal(domainLength * 2); // 2-byte unicode...we're using the "W" variety of the funcion

                if(!SecurityApi.LookupAccountName(null, _user, pSid, ref sidLength, pDomain, ref domainLength, out use))
                    throw new Exception("LookupAccountName failed.  Error = " + Marshal.GetLastWin32Error().ToString());

                if(!SecurityApi.ConvertSidToStringSid(pSid, out pStringSid))
                    throw new Exception("ConvertSidToStringSid failed.  Error = " + Marshal.GetLastWin32Error().ToString());

                return Marshal.PtrToStringUni(pStringSid);
            }
            finally
            {
                if(pSid != IntPtr.Zero)
                    SecurityApi.LocalFree(pSid);

                if(pStringSid != IntPtr.Zero)
                    SecurityApi.LocalFree(pStringSid);

                if(pDomain != IntPtr.Zero)
                    Marshal.FreeHGlobal(pDomain);
            }
        }
    }
}
