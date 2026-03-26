import type { JobProgressEvent } from "../store/bridge";
import { Panel } from "./Panel";

type Props = {
  progress: JobProgressEvent | null;
  activeJobId: string | null;
};

export function ProgressPanel({ progress, activeJobId }: Props) {
  return (
    <Panel title="Progress">
      {!activeJobId ? (
        <div>No active job.</div>
      ) : (
        <div style={{ display: "grid", gap: 10 }}>
          <div><strong>Job ID:</strong> {activeJobId}</div>
          <div><strong>Stage:</strong> {progress?.stage ?? "pending"}</div>
          <div><strong>Message:</strong> {progress?.message ?? "Waiting..."}</div>
          <div><strong>Patch:</strong> {progress?.currentPatchIndex ?? 0} / {progress?.patchCount ?? 0}</div>
          <div><strong>Pattern:</strong> {progress?.currentPatternIndex ?? 0} / {progress?.patternCount ?? 0}</div>
        </div>
      )}
    </Panel>
  );
}
