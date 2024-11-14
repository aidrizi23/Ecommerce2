using System.ComponentModel.DataAnnotations;

namespace AuthAlbiWebSchool.Models.Account;

public class DeleteAccountViewModel
{
    [ Required]
    [ DataType(DataType.Password)]
    [ Display(Name = "Password")]
    public string Password { get; set; }
}