namespace CheckSum.Core.Results
{
    /// <summary>
    /// Статус результата обработки файла.
    /// Здесь он сделан игрушечный.
    /// </summary>
    public static class FileAnalyzeStatus
    {
        public static string Success => "Success";
        public static string Error => "Error";
        public static string ZeroFile => "Zero size file";
    }
}