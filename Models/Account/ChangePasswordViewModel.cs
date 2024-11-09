namespace AuthAlbiWebSchool.Models.Account;

public class ChangePasswordViewModel
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}