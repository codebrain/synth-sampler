create table if not exists synth_profiles (
    id text primary key,
    manufacturer text not null,
    model text not null,
    display_name text not null,
    created_at timestamptz not null default now()
);

create table if not exists patches (
    id uuid primary key,
    synth_profile_id text not null references synth_profiles(id),
    patch_name text not null,
    sysex_path text not null,
    sysex_sha256 text not null,
    created_at timestamptz not null default now()
);

create table if not exists renders (
    id uuid primary key,
    patch_id uuid not null references patches(id),
    midi_pattern_name text not null,
    audio_path text not null,
    metadata_path text,
    created_at timestamptz not null default now()
);

create table if not exists patch_embeddings (
    patch_id uuid not null references patches(id),
    source_audio_path text not null,
    vector_dimension int not null,
    embedding jsonb not null,
    created_at timestamptz not null default now()
);
