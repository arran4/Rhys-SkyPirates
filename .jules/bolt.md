# Performance Learnings

## Caching WaitForSeconds
* **Impact:** High frequency allocation of `WaitForSeconds` in coroutines generates significant garbage over time.
* **Solution:** Cache the `WaitForSeconds` instance in a member variable, initializing it in `Start` or `Awake`.
* **Caveat:** Changing the delay value at runtime (e.g., via Inspector) won't update the cached instance immediately unless specific logic is added to handle it.
* **Verification:** Validated via standalone C# benchmark mocking `WaitForSeconds`.
