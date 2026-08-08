# Meta Quest 3 "VR Raw Screen Mirroring" - 1:1 Pixel Mapping

This project implements a high-precision "VR Raw Screen Mirroring" for the Meta Quest 3. It allows a second headset to act as a pure display slave to a primary PCVR session, maintaining 100% accuracy in stereoscopy, scale, and Field of View (FOV).

## 🎯 The Problem
Standard VR sharing solutions (Bigscreen, Skybox, Browsers) render the source video onto a virtual screen *inside* a 3D environment. This causes the Meta Quest's Compositor to apply lens distortion over an image that already contains its own perspective. This "double distortion" destroys 1:1 pixel mapping and creates scale/depth inaccuracies.

## 🛠 The Solution
This application bypasses the standard 3D rendering pipeline:
1. **Direct Buffer Injection:** Instead of a virtual screen in a 3D world, we use a custom UI-based rendering path that fills the Quest's eye buffers directly.
2. **Stereoscopic Splitting:** A custom shader (`EyeSplit.shader`) detects the hardware Eye Index (Left/Right) and pulls the corresponding half of a Side-by-Side (SBS) frame.
3. **Tracking Bypass:** Spatial tracking is neutralized, pinning the image to the user's vision (the "VR Raw Screen Mirroring" effect).
4. **Native Distortion:** By injecting into the native camera buffers, the Quest's hardware-level distortion exactly cancels out the rectilinear projection of the source, resulting in a perfect optical replica.

## 🎥 Live Streaming Architecture
To transition from static screenshots to a live feed, we use the following stack:
- **Source:** `OculusMirror.exe` (Symmetric SBS, Rectilinear).
- **Encoder (PC):** **OBS Studio v30.1+** with **NVIDIA NVENC HEVC (H.265)**.
- **Protocol:** **NDI 6 (Network Device Interface)** via the **DistroAV (obs-ndi) v6.1.1** plugin.
- **Confirmed OBS Settings:** 
  - **Resolution:** 3827 × 2047 px (Near 4K SBS).
  - **Frame Rate:** 60 FPS Fixed.
  - **Bitrate:** 150,000 Kbps (CBR).
  - **Preset:** P7: Slowest (Best Quality).
  - **Tuning:** Ultra High Quality (Multipass: Two Passes).
- **Receiver (Quest 3):** Unity-based NDI receiver (KlakNDI) feeding the `VRRawScreenMirroring.cs` script.
- **Network:** Wi-Fi 6 (802.11ax) 5GHz AX1500 Router (TP-Link Archer AX1500).

## 🚀 Current State
- **Status:** Functional static POC verified on hardware.
- **Input:** Static 1:1 SBS screenshot.
- **Rendering:** Unity Built-in Pipeline with MultiView support.
- **Branches:**
  - `main`: Glued-to-eyes mode (Tracking Bypassed).
  - `world-space-tracking`: Virtual Window mode (Manual Tracking Sync).

## 📋 Requirements
- Unity 6000.3.10f1 or later.
- Meta Quest 3 (Developer Mode enabled).
- OBS Studio + DistroAV (obs-ndi) v6.1.1.
- **NDI 6 Runtime** (Installed on PC).
- Wi-Fi 6 5GHz Network.

## 🔧 Setup & Build
1. Open the project in Unity.
2. Ensure **Oculus** is enabled in `Project Settings > XR Plug-in Management`.
3. Set **Stereo Rendering Mode** to `Multiview`.
4. Build and Run the `SampleScene` to your Quest 3.

---
## License

This project is licensed under the [MIT License](LICENSE).
