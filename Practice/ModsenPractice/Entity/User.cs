namespace ModsenPractice.Entity
{
    public class User
    {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; } // Например, "Admin" или "User"
    public string RefreshToken { get; set; } // Для хранения refresh токена
    public DateTime RefreshTokenExpiryTime { get; set; } // Время истечения refresh токена
    }
}