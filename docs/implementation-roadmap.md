# Sequential implementation roadmap

## Layer 1: Desktop shell
- done in skeleton: WPF host, WebView2 control, internal web server
- next: add packaging, icons, installer, and structured exception handling

## Layer 2: Bridge
- done in skeleton: JSON envelopes, dispatcher, typed frontend bridge
- next: add validation, richer error codes, and host-side command authorisation

## Layer 3: MIDI
- done in skeleton: service abstraction
- next: replace `StubMidiService` with a real implementation using your chosen Windows MIDI library

## Layer 4: Audio
- done in skeleton: service abstraction
- next: replace `StubAudioCaptureService` with a real capture implementation using a Windows audio library

## Layer 5: Automation
- done in skeleton: sequential batch runner
- next: add per-pattern capture durations, silence detection, retry strategy, and job history

## Layer 6: Storage
- done in skeleton: render paths and manifest writing
- next: store richer metadata and hashes, and add import/export helpers

## Layer 7: Frontend
- done in skeleton: React job composer, progress panel, log panel
- next: add actual file pickers, synth parameter panel, render browser, waveform preview

## Layer 8: Analysis + indexing
- done in skeleton: FastAPI service and docker compose for Postgres + Qdrant
- next: add real feature extraction, ingestion, and search endpoints
