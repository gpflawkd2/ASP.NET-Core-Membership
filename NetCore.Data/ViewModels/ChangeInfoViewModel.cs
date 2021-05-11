using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCore.Data.ViewModels
{
    public class ChangeInfoViewModel
    {
        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 사용자 이메일
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        /// <summary>
        /// true : 전부 Data가 같은 경우, false: 하나라도 다른 경우
        /// </summary>
        /// <param name="other">비교할 다른 클래스</param>
        /// <returns></returns>
        public bool Equals(UserInfoViewModel other)
        {
            if(!string.Equals(UserName, other.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(UserEmail, other.UserEmail, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
