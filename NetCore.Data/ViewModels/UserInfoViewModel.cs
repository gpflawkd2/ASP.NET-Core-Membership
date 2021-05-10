using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.ViewModels
{
    public class UserInfoViewModel
    {
        [Required(ErrorMessage = "사용자 아이디를 입력하세요.")]
        [MinLength(6, ErrorMessage = "사용자 아이디는 6자 이상 입력하세요.")]
        [Display(Name = "사용자 아이디")]
        public string UserId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "사용자 비밀번호는 6자 이상 입력하세요.")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }

        /****************************************************************/

        [Required(ErrorMessage = "사용자 이름을 입력하세요.")]
        [Display(Name = "사용자 이름")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "사용자 이메일을 입력하세요.")]
        [Display(Name = "사용자 이메일")]
        public string UserEmail { get; set; }

        public virtual ChangeInfoViewModel ChangeInfo { get; set; }

    }
}
