using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace Tests
{
    public class PathTraversalTests
    {
        [Test]
        public void Vulnerability_Reproduction_PathTraversal()
        {
            // Simulate the logic in TempSave.cs (OLD CODE)
            string persistentDataPath = "/Application/PersistentData";
            string userInput = "../../../../etc/passwd";

            // This is the vulnerable code pattern:
            string unsafePath = persistentDataPath + "/" + userInput + ".json";

            // Verify that the path traverses up.
            Assert.IsTrue(unsafePath.Contains(".."));
            Assert.IsTrue(unsafePath.Contains("/Application/PersistentData/../../../../etc/passwd.json"));
        }

        [Test]
        public void Sanitizer_Prevents_PathTraversal()
        {
            string persistentDataPath = "/Application/PersistentData";
            string userInput = "../../../../etc/passwd";

            // Use the sanitizer
            string safeName = PathSanitizer.SanitizeFileName(userInput);
            string safePath = Path.Combine(persistentDataPath, safeName + ".json");

            // Verify that the path does NOT traverse up
            Assert.IsFalse(safePath.Contains(".."));

            // Verify expected result
            // Path.GetFileName("../../../../etc/passwd") -> "passwd"
            Assert.AreEqual("passwd", safeName);

            // Test other cases
            Assert.AreEqual("default_save", PathSanitizer.SanitizeFileName(null));
            Assert.AreEqual("default_save", PathSanitizer.SanitizeFileName(""));
            Assert.AreEqual("default_save", PathSanitizer.SanitizeFileName("   "));
            Assert.AreEqual("default_save", PathSanitizer.SanitizeFileName("."));
            Assert.AreEqual("default_save", PathSanitizer.SanitizeFileName(".."));

            // Complex input with invalid chars and directory separators
            string complexInput = "folder/file:name*?.json";
            string sanitized = PathSanitizer.SanitizeFileName(complexInput);

            Assert.IsFalse(sanitized.Contains("/"));
            Assert.IsFalse(sanitized.Contains("\\"));
            Assert.IsFalse(sanitized.Contains(":"));
            Assert.IsFalse(sanitized.Contains("*"));
            Assert.IsFalse(sanitized.Contains("?"));
        }
    }
}
