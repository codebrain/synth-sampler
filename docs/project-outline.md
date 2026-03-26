# Project outline

## Purpose

Build a Windows desktop application for hardware synthesizer automation, sound capture, and
sound-space indexing. The app should let the user select a synth, load SysEx patches, play
test MIDI patterns, record audio for each result, and eventually analyse those recordings as
vectors for similarity search across many instruments.

## Core user story

The user launches a desktop app. The app starts an internal web server, opens a WPF window
containing WebView2, and serves a React frontend. The React GUI sends typed messages to the
host application. The host owns all communication with audio and MIDI devices, all automation,
and all local storage. When the app closes, the server and host shut down cleanly.

## Functional scope

### Desktop shell
- single-click / double-click launch
- embedded WebView2 interface
- in-process localhost static file server
- clean startup and shutdown

### UI
- discover MIDI outputs and audio inputs
- choose synth profile
- import SysEx patches
- import MIDI patterns
- configure capture timing
- launch a batch run
- monitor progress and logs
- browse render history

### MIDI
- enumerate ports
- send notes / CC / program changes / SysEx
- receive SysEx where supported
- support RTP-MIDI as ordinary OS-exposed ports

### Audio
- enumerate capture devices
- start and stop capture
- save one take per patch/pattern combination
- attach metadata

### Job engine
- iterate SysEx patches
- iterate MIDI patterns
- wait between stages
- cancel gracefully
- emit progress events to UI

### Storage
- save WAV and JSON manifests locally
- save patch metadata and hashes
- save synth profile configuration
- later sync selected metadata to Postgres / Qdrant

### Analysis
- compute descriptors and embeddings from rendered audio
- persist embeddings
- index vectors into Qdrant
- visualise neighbourhoods later in UI

## Non-goals for first implementation
- browser-only hardware access
- cross-platform hardware support
- distributed execution
- live collaborative multi-user operation

## Technical choices
- **Desktop host:** WPF, .NET 8, WebView2
- **Frontend:** React + TypeScript + Vite
- **Bridge:** WebView2 postMessage
- **Database:** Postgres
- **Vector DB:** Qdrant
- **Analysis:** Python FastAPI worker
- **Hardware ownership:** native C# host

## Folder structure

```text
desktop/           WPF host, bridge, native services
frontend/          React UI
analysis-service/  Python API for feature extraction
docker/            compose and DB init
shared/            bridge contracts and schemas
docs/              architecture and project notes
```
