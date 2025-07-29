namespace App.Domain;

public class SignInResult
{
    public bool Succeeded { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsNotAllowed { get; set; }
}
