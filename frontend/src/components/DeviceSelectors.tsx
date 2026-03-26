import { ChangeEvent } from "react";
import { Panel } from "./Panel";
import type { AudioDeviceInfo, MidiDeviceInfo, SynthProfile } from "../store/bridge";

type Props = {
  midiDevices: MidiDeviceInfo[];
  audioDevices: AudioDeviceInfo[];
  profiles: SynthProfile[];
  selectedMidi: string;
  selectedAudio: string;
  selectedProfile: string;
  onMidiChange: (value: string) => void;
  onAudioChange: (value: string) => void;
  onProfileChange: (value: string) => void;
};

export function DeviceSelectors(props: Props) {
  const onChange =
    (setter: (value: string) => void) =>
    (event: ChangeEvent<HTMLSelectElement>) =>
      setter(event.target.value);

  return (
    <Panel title="Devices and profile">
      <div style={{ display: "grid", gap: 12 }}>
        <label>
          <div style={{ marginBottom: 6 }}>Synth profile</div>
          <select value={props.selectedProfile} onChange={onChange(props.onProfileChange)} style={{ width: "100%", padding: 8 }}>
            <option value="">Select profile</option>
            {props.profiles.map((profile) => (
              <option key={profile.id} value={profile.id}>
                {profile.displayName}
              </option>
            ))}
          </select>
        </label>

        <label>
          <div style={{ marginBottom: 6 }}>MIDI output</div>
          <select value={props.selectedMidi} onChange={onChange(props.onMidiChange)} style={{ width: "100%", padding: 8 }}>
            <option value="">Select MIDI output</option>
            {props.midiDevices.filter((x) => x.isOutput).map((device) => (
              <option key={device.id} value={device.id}>
                {device.name}
              </option>
            ))}
          </select>
        </label>

        <label>
          <div style={{ marginBottom: 6 }}>Audio input</div>
          <select value={props.selectedAudio} onChange={onChange(props.onAudioChange)} style={{ width: "100%", padding: 8 }}>
            <option value="">Select audio input</option>
            {props.audioDevices.map((device) => (
              <option key={device.id} value={device.id}>
                {device.name}
              </option>
            ))}
          </select>
        </label>
      </div>
    </Panel>
  );
}
