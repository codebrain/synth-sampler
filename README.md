# Synth Sound Platform

A desktop-first synthesizer automation and sound-cataloguing platform.

## Project summary

This project is designed to:

- host a React GUI inside a WPF desktop application via WebView2
- communicate between React and C# through the WebView2 message bridge
- discover MIDI devices, including RTP-MIDI sessions exposed by the operating system
- send SysEx, CC, notes, and other MIDI messages to hardware synthesizers
- capture incoming audio while automated test patterns are played
- batch-render a matrix of `Synth Profile × SysEx Patch × MIDI Pattern`
- store rendered audio, patch metadata, and run manifests locally
- later extract audio embeddings and index them into Postgres + Qdrant
- support similarity search and map-style sound exploration across synths

## Final architecture

```text
WPF Desktop Host (C# / .NET 8 / WebView2)
├── In-process local static web server
├── WebView2 browser host
├── WebView2 message bridge
├── MIDI service
├── Audio capture service
├── Automation job runner
├── Local file storage
└── Optional database / vector services
    ├── Postgres (Docker)
    ├── Qdrant (Docker)
    └── Python analysis service (Docker)
```

## Why this architecture

This keeps direct hardware access in the native Windows host while preserving a modern React UI.
It also leaves room for a later analysis stack running in Docker containers without forcing
audio and MIDI hardware access through containers.

## Included layers

1. **Desktop host**
   - WPF shell
   - WebView2
   - internal static file server
   - graceful shutdown lifecycle

2. **Bridge**
   - typed message envelopes
   - request / response correlation IDs
   - unsolicited progress and log events

3. **Automation**
   - batch capture job model
   - sequential patch/pattern iteration
   - cancellation and progress support

4. **Storage**
   - deterministic file structure
   - metadata manifests
   - synth profiles

5. **Frontend**
   - React + TypeScript skeleton
   - bridge wrapper
   - device selection
   - job composer
   - progress / logs panels

6. **Future analysis stack**
   - FastAPI analysis service skeleton
   - Postgres init SQL
   - Qdrant + docker-compose scaffolding

## Suggested build order

1. Wire WebView2 and bridge
2. Implement MIDI device enumeration and send
3. Implement audio device enumeration and capture
4. Implement batch job runner
5. Persist renders and manifests
6. Add Postgres + Qdrant ingestion
7. Add Python embedding extraction
8. Add vector search and exploration UI
