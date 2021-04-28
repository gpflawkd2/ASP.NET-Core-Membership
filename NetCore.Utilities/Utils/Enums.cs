using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Utilities.Utils
{
    public class Enums
    {
        /// <summary>
        /// 암호화 유형
        /// </summary>
        public enum CryptoType
        {
            Unmanaged = 1,  // 관리되지 않는 암호화키

            Managed = 2,    // 관리되는 암호화키

            CngCbc = 3,     // Cng - Cbc Type

            CngGCM = 4      // Cng - GCM Type
        }
    }
}
