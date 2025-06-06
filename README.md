# VideoLogoBatch

🔧 A lightweight Windows desktop app to batch render videos with logo or text overlay using FFmpeg.

## Features

- 🖼️ Add PNG or text logo overlays
- 🎯 Choose position, opacity, margin
- ⚡ Batch process entire folders of videos
- 🧪 Preview before render
- 🚀 Supports GPU acceleration (via FFmpeg NVENC)

## How to Use

1. Download and extract the ZIP archive.
2. Run `VideoLogoBatch.exe` inside the `VideoLogoBatch/` folder.

## FFmpeg Required

To use this application, you must add FFmpeg manually:

1. Visit: https://www.gyan.dev/ffmpeg/builds/
2. Download: `ffmpeg-release-full.7z`
3. Extract and rename the folder to `ffmpeg`
4. Copy the `ffmpeg/` folder into the root of the app folder (where `.exe` is located)

```
VideoLogoBatch/
├── VideoLogoBatch.exe
├── ffmpeg/
│   └── bin/
│       └── ffmpeg.exe
```

## License

Apache License 2.0 – see `LICENSE.txt` for details.

---

🌐 GitHub: https://github.com/DEVCodeWithAI  
☕ Support: https://buymeacoffee.com/devcodewithai
