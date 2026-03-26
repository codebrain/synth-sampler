# Bridge message types

## Commands (React -> Host)
- `app:get-state`
- `midi:get-devices`
- `audio:get-devices`
- `profiles:get`
- `jobs:start-batch`
- `jobs:cancel`
- `storage:open-renders-folder`

## Events (Host -> React)
- `app:ready`
- `app:error`
- `midi:devices`
- `audio:devices`
- `profiles:list`
- `jobs:started`
- `jobs:progress`
- `jobs:completed`
- `jobs:failed`
- `log:entry`

## Envelope
```json
{
  "id": "uuid",
  "type": "jobs:start-batch",
  "direction": "request",
  "payload": {}
}
```
