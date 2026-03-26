export type BridgeEnvelope<T = unknown> = {
  id: string;
  type: string;
  direction: "request" | "response" | "event" | "error";
  payload: T;
};

export type MidiDeviceInfo = {
  id: string;
  name: string;
  isInput: boolean;
  isOutput: boolean;
};

export type AudioDeviceInfo = {
  id: string;
  name: string;
  channels: number;
};

export type SynthProfile = {
  id: string;
  displayName: string;
  manufacturer: string;
  model: string;
  parameters: Array<{
    name: string;
    type: string;
    number: number;
    min: number;
    max: number;
  }>;
};

export type BatchCaptureRequest = {
  synthProfileId: string;
  midiOutputId: string;
  audioInputId: string;
  outputFolder: string;
  settleDelayMs: number;
  tailMs: number;
  sysExFiles: string[];
  midiPatternFiles: string[];
};

export type JobProgressEvent = {
  jobId: string;
  currentPatchIndex: number;
  patchCount: number;
  currentPatternIndex: number;
  patternCount: number;
  stage: string;
  message: string;
};
