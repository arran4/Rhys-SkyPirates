## 2024-05-22 - Path Traversal in Unity Save System
**Vulnerability:** `SaveLoadManager` accepted unsanitized file paths, and `TempSave` concatenated user input directly into paths, allowing directory traversal.
**Learning:** Unity's `Application.persistentDataPath` does not automatically sandbox file operations; developers must enforce it explicitly.
**Prevention:** Always sanitize user input with `Path.GetFileName` and validate final paths with `Path.GetFullPath` to ensure they reside within the intended directory.
