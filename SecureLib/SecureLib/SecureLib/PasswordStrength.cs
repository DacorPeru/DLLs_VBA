using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace SecureLib
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PasswordStrength
    {
        public string Evaluate(string password)
        {
            int score = 0;

            if (password.Length >= 8) score++;
            if (Regex.IsMatch(password, "[A-Z]")) score++;
            if (Regex.IsMatch(password, "[a-z]")) score++;
            if (Regex.IsMatch(password, "[0-9]")) score++;
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) score++;

            if (score <= 2) return "Débil";
            else if (score == 3) return "Media";
            else if (score == 4) return "Fuerte";
            else return "Muy Fuerte";
        }
    }
}
