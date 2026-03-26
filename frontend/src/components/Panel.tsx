import { ReactNode } from "react";

type Props = {
  title: string;
  children: ReactNode;
};

export function Panel({ title, children }: Props) {
  return (
    <section style={{ background: "#1f2937", borderRadius: 12, padding: 16, border: "1px solid #374151" }}>
      <h2 style={{ marginTop: 0, fontSize: 18 }}>{title}</h2>
      {children}
    </section>
  );
}
