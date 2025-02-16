#  Translate SRT Files From English To Arabic By Google Translate


## [SRT Translation Project](SRT%20Translation)

### Program.cs
- Get all SRT files:
```csharp
  var tf = new TranslationForm
  {
    SrtFiles = Directory.GetFiles(fbd.SelectedPath, "*.srt", SearchOption.AllDirectories)
  };
```

### Form1.cs
- Check SRT file name:
```csharp
  private readonly char[] chars = { ' ', '_', '-', '.' };
  private bool EndsWith(string s, string lang)
  {
    foreach (char c in chars)
      if (s.EndsWith($"{c}{lang}.srt", StringComparison.CurrentCultureIgnoreCase))
        return true;
      return false;
  }
```

- Select SRT files and output file name:

    In TranslationForm_Load() method:
```csharp
  if (EndsWith(s, "en"))
    output = s.Substring(0, s.Length - 6) + "ar.srt";
  else if (EndsWith(s, "english"))
    output = s.Substring(0, s.Length - 11) + "Arabic.srt";
  else if (!EndsWith(s, "ar") && !EndsWith(s, "arabic"))
    output = s.Substring(0, s.Length - 4) + "_ar.srt";
  else
    continue;
```

- Read lines of the file, execute the translation and write the new file:

    In TranslateFile() method:
```csharp
  string[] lines = File.ReadAllLines(ftc.FilePath);
  await TranslateAsync(lines, "en", "ar", ftc);
  File.WriteAllLines(ftc.OutputFilePath, lines);
```

To be continued...


