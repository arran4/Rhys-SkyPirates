## 2024-05-22 - Path Traversal in Save System
**Vulnerability:** User input from `InputField` was directly concatenated to `Application.persistentDataPath` in `TempSave.cs`, allowing traversal (e.g., `../../file`).
**Learning:** Unity's `Application.persistentDataPath` does not sandbox file operations; manual sanitization is required.
**Prevention:** Use `Path.GetFileName` to strip directory components from user-provided filenames before using them in file paths.
