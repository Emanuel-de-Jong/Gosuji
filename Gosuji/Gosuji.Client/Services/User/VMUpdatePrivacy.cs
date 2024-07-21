using Gosuji.Client.Resources.Translations;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Services.User
{
    public class VMUpdatePrivacy
    {
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [MinLength(2, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [MaxLength(30, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$",
            ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(ValidateMessages))]
        public string Email { get; set; }

        [MinLength(6, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$",
            ErrorMessageResourceName = "PasswordCharError", ErrorMessageResourceType = typeof(ValidateMessages))]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [Compare("NewPassword", ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
        public string ConfirmNewPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
        public string CurrentPassword { get; set; }
    }
}
