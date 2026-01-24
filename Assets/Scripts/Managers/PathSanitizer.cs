using System.IO;

public static class PathSanitizer
{
    /// <summary>
    /// Sanitizes a file name by removing invalid characters and ensuring no path traversal.
    /// </summary>
    /// <param name="fileName">The user input file name.</param>
    /// <returns>A safe file name.</returns>
    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "default_save";

        // Use Path.GetFileName to strip any directory path information.
        // This effectively neutralizes ".." traversal attempts in the path.
        string name = Path.GetFileName(fileName);

        // Replace invalid file name characters with an underscore
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            name = name.Replace(c, '_');
        }

        // Additional safety: if the name is strictly "." or ".." (which might be valid names on some systems but dangerous)
        if (name == "." || name == "..")
        {
            return "default_save";
        }

        // Final check to ensure we have a valid name
        if (string.IsNullOrWhiteSpace(name))
            return "default_save";

        return name;
    }
}
