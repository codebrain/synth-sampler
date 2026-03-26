import type {
  AudioDeviceInfo,
  BatchCaptureRequest,
  BridgeEnvelope,
  JobProgressEvent,
  MidiDeviceInfo,
  SynthProfile,
} from "../types/bridge";

type PendingResolver = (payload: unknown) => void;

class HostBridge {
  private pending = new Map<string, PendingResolver>();
  private listeners = new Map<string, Array<(payload: unknown) => void>>();

  constructor() {
    window.addEventListener("message", () => undefined);

    if ((window as any).chrome?.webview) {
      (window as any).chrome.webview.addEventListener("message", (event: MessageEvent<BridgeEnvelope>) => {
        this.onMessage(event.data);
      });
    }
  }

  private onMessage(message: BridgeEnvelope) {
    if (message.direction === "event") {
      const handlers = this.listeners.get(message.type) ?? [];
      handlers.forEach((handler) => handler(message.payload));
      return;
    }

    const pending = this.pending.get(message.id);
    if (pending) {
      pending(message.payload);
      this.pending.delete(message.id);
    }
  }

  on<T>(type: string, handler: (payload: T) => void) {
    const handlers = this.listeners.get(type) ?? [];
    handlers.push(handler as (payload: unknown) => void);
    this.listeners.set(type, handlers);
  }

  async request<TResponse>(type: string, payload: unknown = {}): Promise<TResponse> {
    const id = crypto.randomUUID();
    const envelope: BridgeEnvelope = {
      id,
      type,
      direction: "request",
      payload
    };

    return new Promise<TResponse>((resolve) => {
      this.pending.set(id, resolve as PendingResolver);

      if ((window as any).chrome?.webview) {
        (window as any).chrome.webview.postMessage(envelope);
      } else {
        console.warn("WebView2 bridge unavailable. Running in browser-only mode.");
        resolve({} as TResponse);
      }
    });
  }

  getMidiDevices() {
    return this.request<MidiDeviceInfo[]>("midi:get-devices");
  }

  getAudioDevices() {
    return this.request<AudioDeviceInfo[]>("audio:get-devices");
  }

  getProfiles() {
    return this.request<SynthProfile[]>("profiles:get");
  }

  startBatch(request: BatchCaptureRequest) {
    return this.request<{ jobId: string }>("jobs:start-batch", request);
  }

  cancelBatch() {
    return this.request<{ cancelled: boolean }>("jobs:cancel");
  }
}

export const bridge = new HostBridge();
export type { AudioDeviceInfo, BatchCaptureRequest, JobProgressEvent, MidiDeviceInfo, SynthProfile };
