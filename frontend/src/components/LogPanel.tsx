import { Panel } from "./Panel";

type Props = {
  logs: string[];
};

export function LogPanel({ logs }: Props) {
  return (
    <Panel title="Log">
      <div style={{ maxHeight: 280, overflow: "auto", display: "grid", gap: 8 }}>
        {logs.length === 0 ? <div>No log entries yet.</div> : logs.map((log, index) => (
          <div key={index} style={{ fontFamily: "Consolas, monospace", fontSize: 13, whiteSpace: "pre-wrap" }}>
            {log}
          </div>
        ))}
      </div>
    </Panel>
  );
}
