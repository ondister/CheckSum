using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace CheckSum.WpfDemo
{
    /// <summary>
    ///     Валидатор для проверки наличия папки
    /// </summary>
    public class DirectoryValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var validationresult = new ValidationResult(false, "Папка не доступна или не существует");
            try
            {
                if (value is string folderName && new DirectoryInfo(folderName).Exists)
                {
                    validationresult = new ValidationResult(true, null);
                }
            }
            catch
            {
                return new ValidationResult(false, "Возникла ошибка при проверке");
            }

            return validationresult;
        }
    }
}