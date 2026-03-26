import { useEffect, useState } from "react";
import { DeviceSelectors } from "../components/DeviceSelectors";
import { JobComposer } from "../components/JobComposer";
import { LogPanel } from "../components/LogPanel";
import { ProgressPanel } from "../components/ProgressPanel";
import { bridge, type AudioDeviceInfo, type BatchCaptureRequest, type JobProgressEvent, type MidiDeviceInfo, type SynthProfile } from "../store/bridge";

export function App() {
  const [midiDevices, setMidiDevices] = useState<MidiDeviceInfo[]>([]);
  const [audioDevices, setAudioDevices] = useState<AudioDeviceInfo[]>([]);
  const [profiles, setProfiles] = useState<SynthProfile[]>([]);
  const [selectedMidi, setSelectedMidi] = useState("");
  const [selectedAudio, setSelectedAudio] = useState("");
  const [selectedProfile, setSelectedProfile] = useState("");
  const [logs, setLogs] = useState<string[]>([]);
  const [activeJobId, setActiveJobId] = useState<string | null>(null);
  const [progress, setProgress] = useState<JobProgressEvent | null>(null);

  const appendLog = (message: string) => setLogs((prev) => [message, ...prev].slice(0, 200));

  useEffect(() => {
    async function init() {
      const [midi, audio, profileList] = await Promise.all([
        bridge.getMidiDevices(),
        bridge.getAudioDevices(),
        bridge.getProfiles()
      ]);

      setMidiDevices(midi);
      setAudioDevices(audio);
      setProfiles(profileList);
      appendLog("Bridge initialised.");
    }

    bridge.on<JobProgressEvent>("jobs:progress", (payload) => {
      setProgress(payload);
      appendLog(`[progress] ${payload.stage}: ${payload.message}`);
    });

    bridge.on<{ jobId: string; runFolder: string }>("jobs:started", (payload) => {
      setActiveJobId(payload.jobId);
      appendLog(`[started] ${payload.jobId} -> ${payload.runFolder}`);
    });

    bridge.on<{ jobId: string; runFolder: string }>("jobs:completed", (payload) => {
      appendLog(`[completed] ${payload.jobId} -> ${payload.runFolder}`);
      setActiveJobId(null);
    });

    bridge.on<{ jobId: string; error: string }>("jobs:failed", (payload) => {
      appendLog(`[failed] ${payload.jobId}: ${payload.error}`);
      setActiveJobId(null);
    });

    void init();
  }, []);

  const onStart = async (request: BatchCaptureRequest) => {
    const response = await bridge.startBatch(request);
    setActiveJobId(response.jobId);
    appendLog(`[queued] ${response.jobId}`);
  };

  const onCancel = async () => {
    await bridge.cancelBatch();
    appendLog("[cancel] requested");
  };

  return (
    <main style={{ padding: 20 }}>
      <header style={{ marginBottom: 20 }}>
        <h1 style={{ margin: 0 }}>Synth Studio</h1>
        <div style={{ color: "#9ca3af", marginTop: 8 }}>
          Desktop host + React UI + WebView2 bridge for synth automation and capture.
        </div>
      </header>

      <div style={{ display: "grid", gap: 16, gridTemplateColumns: "1.1fr 1.4fr 1fr" }}>
        <DeviceSelectors
          midiDevices={midiDevices}
          audioDevices={audioDevices}
          profiles={profiles}
          selectedMidi={selectedMidi}
          selectedAudio={selectedAudio}
          selectedProfile={selectedProfile}
          onMidiChange={setSelectedMidi}
          onAudioChange={setSelectedAudio}
          onProfileChange={setSelectedProfile}
        />

        <JobComposer
          synthProfileId={selectedProfile}
          midiOutputId={selectedMidi}
          audioInputId={selectedAudio}
          onStart={onStart}
          onCancel={onCancel}
        />

        <ProgressPanel progress={progress} activeJobId={activeJobId} />
      </div>

      <div style={{ marginTop: 16 }}>
        <LogPanel logs={logs} />
      </div>
    </main>
  );
}
