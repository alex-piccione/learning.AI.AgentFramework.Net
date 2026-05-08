module PathNormalizer

open System.IO

/// Convert Windows path style (C:\\aaa) to Unix style (C:/aaa).
let normalizePath path = 
    Path.GetFullPath(path).Replace('\\', '/')

