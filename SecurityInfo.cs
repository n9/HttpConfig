#region Copyright
/*
 * Copyright (c) 2004-2006 IP Commerce, INC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This document may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from IP Commerce, INC.
 *
 * Use, duplication or disclosure by the U.S. Government is subject
 * to restrictions set forth in an executed license agreement
 * and in subparagraph (c)(1) of the Commercial Computer
 * Software-Restricted Rights Clause at FAR 52.227-19; subparagraph
 * (c)(1)(ii) of the Rights in Technical Data and Computer Software
 * clause at DFARS 252.227-7013, subparagraph (d) of the Commercial
 * Computer Software--Licensing clause at NASA FAR supplement
 * 16-52.227-86; or their equivalent.
 *
 * Information in this document is subject to change without notice
 * and does not represent a commitment on the part of IP Commerce.
 *
 */
#endregion
using System;
using System.Runtime.InteropServices;
using System.Text;

// TODO: Handle multiple ACEs

namespace HttpConfig
{
    public enum UrlPermission
    {
        All,
        Registration,
        Delegation
    }

    public class SecurityInfo
    {
        private string        _fqdn;
        private UrlPermission _permission;

        private SecurityInfo() { }

        public SecurityInfo(string fqdn, UrlPermission permission)
        {
            _fqdn       = fqdn;
            _permission = permission;
        }

        public string ToSddl()
        {
            StringBuilder sddl = new StringBuilder();

            sddl.Append("D:(A;;");

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
            }

            sddl.Append(";;;");

            sddl.Append(EncodeSid());

            sddl.Append(")");

            return sddl.ToString();
        }

        public static SecurityInfo FromSddl(string sddl)
        {
            string[] tokens = sddl.Split(new char[] {':', ';'});

            if(tokens.Length != 7)
            {
                throw new ArgumentException("Invalid SDDL string.  Too many or too few tokens.", "sddl");
            }

            string permString = tokens[3];

            string stringSid = tokens[6].Substring(0, tokens[6].Length - 1);

            SecurityInfo info = new SecurityInfo();

            switch(permString)
            {
                case "GA":
                    info._permission = UrlPermission.All;
                    break;

                case "GX":
                    info._permission = UrlPermission.Registration;
                    break;

                case "GW":
                    info._permission = UrlPermission.Delegation;
                    break;

                default:
                    throw new ArgumentException("Invalid SDDL string.  Unrecognized permission identifier.", "sddl");
            }

            info._fqdn = DecodeSid(stringSid);

            return info;
        }        
    }    
}
