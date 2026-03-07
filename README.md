# Meta Quest 3 "Blind Monitor" - 1:1 Pixel Mapping

This project implements a high-precision "Blind Monitor" for the Meta Quest 3. It allows a second headset to act as a pure display slave to a primary PCVR session, maintaining 100% accuracy in stereoscopy, scale, and Field of View (FOV) without the distortion issues common in standard screen-sharing apps.

## 🎯 The Problem
Standard VR sharing solutions (Bigscreen, Skybox, Browsers) render the source video onto a virtual screen *inside* a 3D environment. This causes the Meta Quest's Compositor to apply lens distortion over an image that already contains its own perspective. This "double distortion" destroys 1:1 pixel mapping and creates scale/depth inaccuracies.

## 🛠 The Solution
This application bypasses the standard 3D rendering pipeline:
1. **Direct Buffer Injection:** Instead of a virtual screen in a 3D world, we use a custom UI-based rendering path that fills the Quest's eye buffers directly.
2. **Stereoscopic Splitting:** A custom shader (`EyeSplit.shader`) detects the hardware Eye Index (Left/Right) and pulls the corresponding half of a Side-by-Side (SBS) frame.
3. **Tracking Bypass:** Spatial tracking is neutralized, pinning the image to the user's vision (the "Blind Monitor" effect).
4. **Native Distortion:** By injecting into the native camera buffers, the Quest's hardware-level distortion exactly cancels out the rectilinear projection of the source, resulting in a perfect optical replica.

## 🚀 Current State (POC)
- **Status:** Functional Proof-of-Concept.
- **Input:** Static 1:1 SBS screenshot from `OculusMirror.exe`.
- **Rendering:** Unity Built-in Pipeline with MultiView support.
- **Verified:** 1:1 pixel alignment confirmed on Meta Quest 3 hardware.

## 📋 Requirements
- Unity 6000.3.10f1 or later.
- Meta Quest 3 (Developer Mode enabled).
- Android Build Support (ARM64).

## 🔧 Setup & Build
1. Open the project in Unity.
2. Ensure **Oculus** is enabled in `Project Settings > XR Plug-in Management`.
3. Set **Stereo Rendering Mode** to `Multiview`.
4. Build and Run the `SampleScene` to your Quest 3.

## 🗺 Roadmap
- [ ] **Live Streaming:** Implement a low-latency NDI or raw UDP stream from the PC.
- [ ] **Dynamic Resolution:** Auto-adjust buffers based on Oculus Mirror output size.
- [ ] **Asynchronous Timewarp (ATW) Toggle:** Investigate disabling ATW for zero-latency raw buffer display.

---
**Developed by Gabriel Pinheiro de Carvalho (gabrielpc4)**
