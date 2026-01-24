## 2024-10-26 - Unity UI Keyboard Parity
**Learning:** Unity UI components handling `IPointerExitHandler` for mouse interactions often miss `IDeselectHandler` for keyboard/gamepad navigation, leaving UI states (like tooltips) stuck.
**Action:** When auditing Unity UI scripts, always verify that `OnPointerExit` logic has a corresponding `OnDeselect` implementation.
