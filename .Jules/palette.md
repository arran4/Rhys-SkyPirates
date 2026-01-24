## 2026-01-24 - Unity Selectable Base Calls
**Learning:** Unity's `Selectable` class overrides (OnPointerExit, OnSelect) must call `base` implementation to preserve built-in visual state transitions (highlight, selected color). Custom implementations often break this, leading to unresponsive UI.
**Action:** When overriding Unity UI event handlers on Selectables, always verify if `base.OnX` is needed for visual feedback.
