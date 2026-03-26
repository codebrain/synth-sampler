import { useState } from "react";
import { Panel } from "./Panel";
import type { BatchCaptureRequest } from "../store/bridge";

type Props = {
  synthProfileId: string;
  midiOutputId: string;
  audioInputId: string;
  onStart: (request: BatchCaptureRequest) => Promise<void>;
  onCancel: () => Promise<void>;
};

export function JobComposer({ synthProfileId, midiOutputId, audioInputId, onStart, onCancel }: Props) {
  const [sysExFiles, setSysExFiles] = useState("patches/example-01.syx\npatches/example-02.syx");
  const [midiFiles, setMidiFiles] = useState("patterns/arp.mid\npatterns/chords.mid");
  const [outputFolder, setOutputFolder] = useState("renders");
  const [settleDelayMs, setSettleDelayMs] = useState(1500);
  const [tailMs, setTailMs] = useState(1000);

  const buildRequest = (): BatchCaptureRequest => ({
    synthProfileId,
    midiOutputId,
    audioInputId,
    outputFolder,
    settleDelayMs,
    tailMs,
    sysExFiles: sysExFiles.split("\n").map((s) => s.trim()).filter(Boolean),
    midiPatternFiles: midiFiles.split("\n").map((s) => s.trim()).filter(Boolean)
  });

  return (
    <Panel title="Batch capture job">
      <div style={{ display: "grid", gap: 12 }}>
        <label>
          <div style={{ marginBottom: 6 }}>SysEx files</div>
          <textarea rows={6} value={sysExFiles} onChange={(e) => setSysExFiles(e.target.value)} style={{ width: "100%", padding: 8 }} />
        </label>

        <label>
          <div style={{ marginBottom: 6 }}>MIDI pattern files</div>
          <textarea rows={4} value={midiFiles} onChange={(e) => setMidiFiles(e.target.value)} style={{ width: "100%", padding: 8 }} />
        </label>

        <div style={{ display: "grid", gridTemplateColumns: "2fr 1fr 1fr", gap: 12 }}>
          <label>
            <div style={{ marginBottom: 6 }}>Output folder</div>
            <input value={outputFolder} onChange={(e) => setOutputFolder(e.target.value)} style={{ width: "100%", padding: 8 }} />
          </label>

          <label>
            <div style={{ marginBottom: 6 }}>Settle delay (ms)</div>
            <input type="number" value={settleDelayMs} onChange={(e) => setSettleDelayMs(Number(e.target.value))} style={{ width: "100%", padding: 8 }} />
          </label>

          <label>
            <div style={{ marginBottom: 6 }}>Tail (ms)</div>
            <input type="number" value={tailMs} onChange={(e) => setTailMs(Number(e.target.value))} style={{ width: "100%", padding: 8 }} />
          </label>
        </div>

        <div style={{ display: "flex", gap: 12 }}>
          <button onClick={() => onStart(buildRequest())} style={{ padding: "10px 14px" }}>
            Start batch
          </button>
          <button onClick={() => onCancel()} style={{ padding: "10px 14px" }}>
            Cancel batch
          </button>
        </div>
      </div>
    </Panel>
  );
}
